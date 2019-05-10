// BootFX - Application framework for .NET applications
// 
// File: DtoType.cs
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
using BootFX.Common.Data;
using BootFX.Common.Entities;
using System.Text;

namespace BootFX.Common.Dto
{
    public class DtoType : IEntityType
    {
        public Type Type { get; private set; }
        public EntityType ConcreteEntityType { get; private set; }
        public List<DtoField> Fields { get; private set; }
        public List<DtoLink> Links { get; private set; }

        public const string IdKey = "Id";
        public const string IdJsonKey = "id";

        internal DtoType(Type dtoType, EntityType concreteType)
        {
            this.Type = dtoType;
            this.ConcreteEntityType = concreteType;
            this.Fields = new List<DtoField>();
            this.Links = new List<DtoLink>();

            //// walk...
            //foreach (DtoFieldAttribute attr in type.GetCustomAttributes(typeof(DtoFieldAttribute), true))
            //{
            //    var useName = attr.Name;
            //    if (attr.Name == IdKey)
            //        useName = this.ConcreteEntityType.GetKeyFields()[0].Name;

            //    var prop = props.Where(v => string.Compare(v.Name, useName, true) == 0).FirstOrDefault();
            //    if (prop == null)
            //        throw new InvalidOperationException(string.Format("Property '{0}' was not found on type '{1}'.", attr.Name, concreteType.Name));

            //    // add...
            //    this.Fields.Add(new DtoField(useName, attr.JsonName, prop));
            //}

            // look for any properties that have json properties...
            var dtoProps = dtoType.GetProperties();
            foreach (var dtoProp in dtoProps)
            {
                try
                {
                    // get...
                    var attrs = (DtoFieldAttribute[])dtoProp.GetCustomAttributes(typeof(DtoFieldAttribute), false);
                    if (attrs.Any())
                    {
                        var member = concreteType.GetMember(dtoProp.Name, OnNotFound.ThrowException);
                        this.Fields.Add(new DtoEntityMemberField(dtoProp.Name, attrs[0].JsonName, dtoProp, member));
                    }
                    else
                    {
                        var linkAttrs = (DtoLinkAttribute[])dtoProp.GetCustomAttributes(typeof(DtoLinkAttribute), false);
                        if (linkAttrs.Any())
                        {
                            var link = (ChildToParentEntityLink)concreteType.Links.GetLink(dtoProp.Name, OnNotFound.ThrowException);
                            this.Links.Add(new DtoLink(dtoProp.Name, linkAttrs[0].JsonName, dtoProp, link));
                        }
                        else
                        {
                            var jsonNetAttrs = dtoProp.GetCustomAttributes(JsonNetMetadata.JsonPropertyAttributeType, false);
                            if (jsonNetAttrs.Any())
                            {
                                var name = JsonNetMetadata.GetPropertyNameFromAttribute(jsonNetAttrs[0]);
                                this.Fields.Add(new DtoAdHocField(dtoProp.Name, name, dtoProp));
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException(string.Format("Failed when processing '{0}'.", dtoProp.Name), ex);
                }
            }

            //  fixup the links...
            foreach (var link in this.Links)
            {
                var field = link.Link.GetLinkFields()[0];
                var dtoField = this.GetFieldByName(field.Name, false);
                if (dtoField == null)
                {
                    throw new InvalidCastException(string.Format("DTO link '{0}' on '{1}' is non-functional as the referred field '{2}' is not defined on the DTO.",
                        link.Name, this.Type.Name, field.Name));
                }

                // set...
                link.ReferenceField = dtoField;
            }
        }

        internal static DtoType GetDtoType(EntityType et, bool throwIfNotFound = true)
        {
            return GetDtoType(et.Type, throwIfNotFound);
        }

        internal static DtoType GetDtoType(Type type, bool throwIfNotFound = true)
        {
            if (typeof(IDtoCapable).IsAssignableFrom(type))
            {
                var dtoType = type.ToEntityType().DtoType;
                if (dtoType != null)
                    return dtoType;
                else
                {
                    if(throwIfNotFound)
                        throw new InvalidOperationException(string.Format("Concrete type '{0}' does not have a DTO type defined.", type));
                    else
                        return null;
                }
            }
            else if (typeof(IDtoBase).IsAssignableFrom(type))
            {
                var et = EntityType.GetEntityTypeByDtoType(type, throwIfNotFound);
                return et.DtoType;
            }
            else
                throw new NotSupportedException(string.Format("Cannot handle '{0}'.", type));
        }

        internal DtoField GetFieldByJsonName(string jsonName, bool throwIfNotFound = true)
        {
            var field = this.Fields.Where(v => string.Compare(v.JsonName, jsonName, true) == 0).FirstOrDefault();
            if (field != null)
                return field;
            else
            {
                if (throwIfNotFound)
                    throw new InvalidOperationException(string.Format("A field with JSON name '{0}' was not found.", jsonName));
                else
                    return null;
            }
        }

        internal DtoField GetFieldByName(string name, bool throwIfNotFound = true)
        {
            var field = this.Fields.Where(v => string.Compare(v.Name, name, true) == 0).FirstOrDefault();
            if (field != null)
                return field;
            else
            {
                if (throwIfNotFound)
                    throw new InvalidOperationException(string.Format("A field with name '{0}' was not found.", name));
                else
                    return null;
            }
        }

        internal DtoLink GetLinkByJsonName(string jsonName, bool throwIfNotFound = true)
        {
            var Link = this.Links.Where(v => string.Compare(v.JsonName, jsonName, true) == 0).FirstOrDefault();
            if (Link != null)
                return Link;
            else
            {
                if (throwIfNotFound)
                    throw new InvalidOperationException(string.Format("A link with JSON name '{0}' was not found.", jsonName));
                else
                    return null;
            }
        }

        internal DtoLink GetLinkByName(string name, bool throwIfNotFound = true)
        {
            var Link = this.Links.Where(v => string.Compare(v.Name, name, true) == 0).FirstOrDefault();
            if (Link != null)
                return Link;
            else
            {
                if (throwIfNotFound)
                    throw new InvalidOperationException(string.Format("A link with name '{0}' was not found.", name));
                else
                    return null;
            }
        }

        internal T CreateConcreteInstance<T>()
            where T : IDtoCapable
        {
            return (T)this.CreateConcreteInstance();
        }

        internal IDtoCapable CreateConcreteInstance()
        {
            return (IDtoCapable)this.ConcreteEntityType.CreateInstance();
        }

        internal T CreateDtoInstance<T>()
        {
            return (T)this.CreateDtoInstance();
        }

        internal IDtoBase CreateDtoInstance()
        {
            return (IDtoBase)Activator.CreateInstance(this.Type);
        }

        EntityType IEntityType.EntityType
        {
            get
            {
                return this.ConcreteEntityType;
            }
        }

        public string Name
        {
            get
            {
                return this.Type.Name;
            }
        }

        internal static DtoType InferDtoType(IEnumerable<IDtoCapable> items, bool throwIfNotFound = true)
        {
            if (typeof(EntityCollection).IsAssignableFrom(items.GetType()))
            {
                var et = ((EntityCollection)items).EntityType;
                return GetDtoType(et, throwIfNotFound);
            }
            else
                return GetDtoType(items.GetType().GenericTypeArguments.First(), throwIfNotFound);
        }

        internal DtoMember GetMemberByJsonName(string jsonName, bool throwIfNotFound = true)
        {
            var field = this.GetFieldByJsonName(jsonName, false);
            if (field != null)
                return field;
            else
            {
                var link = this.GetLinkByJsonName(jsonName, false);
                if (link != null)
                    return link;
                else
                {
                    if (throwIfNotFound)
                        throw new InvalidOperationException(string.Format("A member with name '{0}' was not found.", jsonName));
                    else
                        return null;
                }
            }
        }
    }
}
