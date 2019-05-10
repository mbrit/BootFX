// BootFX - Application framework for .NET applications
// 
// File: DtoBase.cs
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
using BootFX.Common.Data;
using System.Runtime.CompilerServices;

namespace BootFX.Common.Dto
{
    //[JsonConverter(typeof(DtoConverter))]
    public abstract class DtoBase<T> : IDtoBase
        where T : IDtoCapable
    {
        protected internal DtoType Type { get; private set; }

        private Dictionary<DtoField, DtoValue> Values { get; set; }
        private Dictionary<DtoLink, DtoLinkWrapper> Links { get; set; }

        protected DtoBase()
        {
            this.Type = DtoType.GetDtoType(this.GetType());

            // walk...
            this.Values = new Dictionary<DtoField, DtoValue>();
            foreach (var field in this.Type.Fields)
            {
                if (field is DtoEntityMemberField)
                    this.Values[field] = new DtoStoredValue(field.DefaultValue);
                else if (field is DtoAdHocField)
                    this.Values[field] = new DtoRedirectedValue((DtoAdHocField)field);
                else
                    throw new NotSupportedException(string.Format("Cannot handle '{0}'.", field.GetType()));
           }

            // also...
            this.Links = new Dictionary<DtoLink, DtoLinkWrapper>();
            foreach (var link in this.Type.Links)
                this.Links[link] = new DtoLinkWrapper();
        }

        public long Id
        {
            get
            {
                var field = this.Type.GetFieldByJsonName(DtoType.IdKey);
                return ConversionHelper.ToInt64(this[field]);
            }
        }

        public object this[DtoField field]
        {
            get
            {
                try
                {
                    var wrapper = this.Values[field];
                    return wrapper.GetValue(this);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException(string.Format("Failed to get value for '{0}'.", field.JsonName), ex);
                }
            }
            set
            {
                try
                {
                    // convert...
                    var type = field.DtoProperty.PropertyType;
                    var toSet = field.ConvertValueForSet(value);
                    //object toSet = null;
                    //if (value != null)
                    //{
                    //    if (type.IsEnum)
                    //    {
                    //        if (value is string)
                    //            toSet = Enum.Parse(field.DtoProperty.PropertyType, (string)value);
                    //        else
                    //            toSet = ConversionHelper.ChangeType(value, field.DtoProperty.PropertyType);
                    //    }
                    //    else
                    //        toSet = ConversionHelper.ChangeType(value, field.DtoProperty.PropertyType);
                    //}

                    // set...
                    var wrapper = this.Values[field];
                    wrapper.SetValue(this, toSet);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException(string.Format("Failed to set value for '{0}'.\r\nValue: {1}", field.Name, value), ex);
                }
            }
        }

        public object this[string jsonName]
        {
            get
            {
                var field = this.Type.GetFieldByName(jsonName);
                return this[field];
            }
            set
            {
                var field = this.Type.GetFieldByName(jsonName);
                this[field] = value;
            }
        }

        //public IEnumerable<string> GetNames()
        //{
        //    return this.Values.Keys;
        //}

        protected TT GetValue<TT>(DtoField field)
        {
            return ConversionHelper.ChangeType<TT>(this[field]);
        }

        protected TT GetValue<TT>([CallerMemberName] string name = null)
        {
            return ConversionHelper.ChangeType<TT>(this[name]);
        }

        protected void SetValue(object value, [CallerMemberName] string name = null)
        {
            this[name] = value;
        }

        public Dictionary<DtoField, object> GetChangedValues()
        {
            return this.GetChangedTouchedValuesInternal(true);
        }

        public Dictionary<DtoField, object> GetTouchedValues()
        {
            return this.GetChangedTouchedValuesInternal(false);
        }

        private Dictionary<DtoField, object> GetChangedTouchedValuesInternal(bool changed)
        {
            var type = this.Type;

            // walk...
            var results = new Dictionary<DtoField, object>();
            foreach (var field in this.Values.Keys)
            {
                // set...
                var wrapper = this.Values[field];
                if ((changed && wrapper.HasChangedValue) || (!(changed) && wrapper.HasTouchedValue))
                    results[field] = wrapper.GetValue(this);
            }

            // return...
            return results;
        }

        void IDtoBase.OnInitializingFromConcrete(IDtoCapable item)
        {
            this.OnInitializingFromConcrete((T)item);
        }

        protected virtual void OnInitializingFromConcrete(T item)
        {
        }

        void IDtoBase.OnInitializedFromConcrete(IDtoCapable item)
        {
            this.OnInitializedFromConcrete((T)item);
        }

        protected virtual void OnInitializedFromConcrete(T item)
        {
        }

        public IDtoBase GetLink(string name)
        {
            var link = this.Type.GetLinkByName(name);
            return GetLink(link);
        }

        public IDtoBase GetLink(DtoLink link)
        {
            if (!(this.Links[link].IsLoaded))
            {
                try
                {
                    // get...
                    var value = this.GetValue<long>(link.ReferenceField);
                    if (value != 0)
                    {
                        var persistence = link.Link.ParentEntityType.Persistence;
                        var parent = (IDtoCapable)persistence.GetById(new object[] { value }, OnNotFound.ReturnNull);

                        // set...
                        if (parent != null)
                            this.Links[link].Dto = parent.ToDto();
                    }
                }
                finally
                {
                    this.Links[link].IsLoaded = true;
                }
            }

            // return...
            return this.Links[link].Dto;
        }

        protected TT GetLink<TT>([CallerMemberName] string name = null)
        {
            return (TT)this.GetLink(name);
        }

        public void SetLink(DtoLink link, IDtoBase dto)
        {
            this.Links[link].Dto = dto;
        }

        protected void SetLink(IDtoBase dto, [CallerMemberName] string name = null)
        {
            var link = this.Type.GetLinkByName(name);
            this.SetLink(link, dto);
        }

        public bool IsLoaded(DtoLink link)
        {
            return this.Links[link].IsLoaded;
        }

        DtoType IDtoBase.Type
        {
            get
            {
                return this.Type;
            }
        }
    }
}
