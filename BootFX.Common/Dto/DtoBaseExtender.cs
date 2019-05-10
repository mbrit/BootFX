// BootFX - Application framework for .NET applications
// 
// File: DtoBaseExtender.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BootFX.Common;
using System.Reflection;
using BootFX.Common.Data;
using System.Collections;
using System.Text;
using BootFX.Common.Entities;

namespace BootFX.Common.Dto
{
    public static class DtoBaseExtender
    {
        public static T ToDto<T>(this IDtoCapable item)
            where T : IDtoBase
        {
            var dto = ToDto(item);
            return (T)dto;
        }

        public static IDtoBase ToDto(this IDtoCapable item)
        {
            var type = DtoType.GetDtoType(item.GetType());
            var dto = (IDtoBase)Activator.CreateInstance(type.Type);

            // raise...
            dto.OnInitializingFromConcrete(item);

            // walk...
            foreach (var field in type.Fields)
            {
                if (field.CanGetValueFromConcreteItem)
                {
                    var value = field.GetValueFromConcreteItem(item);
                    dto[field] = value;
                }
            }

            // end...
            dto.OnInitializedFromConcrete(item);

            // return...
            return dto;
        }

        //public static PagedDataResult<T> ToDtos<T>(this IEnumerable<IDtoCapable> items, PagedDataRequest request)
        //    where T : IDtoBase
        //{
        //    throw new NotImplementedException("This operation has not been implemented.");
        //}

        public static List<T> ToDtos<T>(this IEnumerable<IDtoCapable> items)
            where T : IDtoBase
        {
            var dtoType = DtoType.InferDtoType(items);

            // get...
            using (var conn = Database.CreateConnection(dtoType.ConcreteEntityType))
            {
                conn.BeginTransaction();
                try
                {
                    var dtos = new List<T>();
                    foreach (var item in items)
                        dtos.Add(item.ToDto<T>());

                    // next -- preload any linked data that we can find...
                    foreach (var link in dtoType.Links)
                    {
                        var map = new Dictionary<long, IDtoBase>();
                        foreach (var dto in dtos)
                        {
                            // get...
                            var parentId = dto.GetValue<long>(link.ReferenceField);
                            if (parentId != 0 && !(map.ContainsKey(parentId)))
                                map[parentId] = null;
                        }

                        // load everything up...
                        var pages = Runtime.Current.GetPages(map.Keys, 500);
                        foreach (var page in pages)
                        {
                            var filter = new SqlFilter(link.Link.ParentEntityType);
                            var keyField = link.Link.ParentEntityType.GetKeyFields()[0];
                            filter.Constraints.AddValueListConstraint(keyField, page);

                            // add...
                            var gots = filter.ExecuteEntityCollection();
                            foreach (IDtoCapable got in gots)
                            {
                                var id = ConversionHelper.ToInt64(filter.EntityType.Storage.GetValue(got, keyField));
                                if (map.ContainsKey(id))
                                    map[id] = got.ToDto();
                            }
                        }

                        // go back and fill them in...
                        foreach (var dto in dtos)
                        {
                            var parentId = dto.GetValue<long>(link.ReferenceField);
                            if (parentId != 0)
                                dto.SetLink(link, map[parentId]);
                        }
                    }

                    // ok...
                    conn.Commit();

                    // return...
                    return dtos;
                }
                catch (Exception ex)
                {
                    conn.Rollback();
                    throw new InvalidOperationException("The operation failed", ex);
                }
            }
        }

        public static DtoType GetDtoType(this IDtoBase dto)
        {
            return DtoType.GetDtoType(dto.GetType());
        }

        public static T GetConcrete<T>(this IDtoBase dto)
            where T : IDtoCapable
        {
            return (T)dto.GetConcrete();
        }

        public static IDtoCapable GetConcrete(this IDtoBase dto)
        {
            var type = DtoType.GetDtoType(dto.GetType());
            if (dto.IsNew())
                return type.CreateConcreteInstance();
            else
            {
                // load...
                var item = (IDtoCapable)type.ConcreteEntityType.Persistence.GetById(new object[] { dto.Id }, OnNotFound.ReturnNull);
                return item;
            }
        }

        public static IDtoCapable ToConcrete(this IDtoBase dto)
        {
            var dtoType = dto.GetDtoType();
            var concrete = dtoType.CreateConcreteInstance();
            dto.PatchConcrete(concrete);

            // return...
            return concrete;
        }

        public static void PatchConcrete(this IDtoBase dto, IDtoCapable concrete, IEnumerable<string> permittedFields = null)
        {
            var dtoType = dto.GetDtoType();

            // ok...
            var wasNew = dto.IsNew();
            concrete.OnPatching(dto, wasNew);

            // if we don't have permitted fields, get them, but skip the key...
            if (permittedFields == null)
            {
                var toUse = new List<string>();
                var et = concrete.GetType().ToEntityType();
                foreach (EntityField field in et.Fields)
                {
                    if (!(field.IsKey()))
                        toUse.Add(field.Name);
                }

                // set...
                permittedFields = toUse;
            }

            // walk...
            var values = dto.GetTouchedValues();
            foreach(var field in values.Keys)
            {
                var value = values[field];

                // are we allowed to set it?
                if(permittedFields.Contains(field.Name, StringComparer.InvariantCultureIgnoreCase))
                {
                    // member...
                    var member = concrete.EntityType.GetMember(field.Name, OnNotFound.ReturnNull);
                    if (member != null)
                    {
                        var canSet = true;
                        if (member is EntityField && ((EntityField)member).IsKey())
                            canSet = false;

                        if (canSet)
                            member.SetValue(concrete, value, BootFX.Common.Entities.SetValueReason.UserSet);
                    }
                    else
                    {
                        var prop = concrete.EntityType.Type.GetProperty(field.Name, BindingFlags.Instance | BindingFlags.Public);
                        if (prop != null && prop.CanWrite)
                        {
                            try
                            {
                                prop.SetValue(concrete, value);
                            }
                            catch (Exception ex)
                            {
                                throw new InvalidOperationException(string.Format("Failed to set value '{0}' on '{1}'.\r\nValue: {2}", prop.Name, prop.DeclaringType.Name, value), ex);
                            }
                        }
                        else
                        {
                            // don't do anything in here -- this needs to be loose....
                        }
                    }
                }
            }

            // signal...
            concrete.OnPatched(dto, wasNew);
        }

        public static T ToConcrete<T>(this IDtoBase dto)
            where T : IDtoCapable
        {
            return (T)dto.ToConcrete();
        }

        public static bool IsNew(this IDtoBase dto)
        {
            return dto.Id == 0;
        }

        public static IDtoCapable GetAndPatchConcrete(this IDtoBase dto, IEnumerable<string> permittedFields = null)
        {
            var concrete = dto.GetConcrete();
            dto.PatchConcrete(concrete, permittedFields);

            // return....
            return concrete;
        }

        public static T GetAndPatchConcrete<T>(this IDtoBase dto, IEnumerable<string> permittedFields = null)
            where T : IDtoCapable
        {
            return (T)dto.GetAndPatchConcrete(permittedFields);
        }

        public static T GetValue<T>(this IDtoBase dto, DtoField field)
        {
            return ConversionHelper.ChangeType<T>(dto[field]);
        }
    }
}
