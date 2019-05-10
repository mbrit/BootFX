// BootFX - Application framework for .NET applications
// 
// File: DtoField.cs
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
using System.Reflection;
using System.Web;
using BootFX.Common.Data;
using BootFX.Common.Entities;

namespace BootFX.Common.Dto
{
    public abstract class DtoField : DtoMember
    {
        private static BfxLookup<string, object> EnumValuesLookup { get; set; }

        internal DtoField(string name, string jsonName, PropertyInfo dtoProp)
            : base(name, jsonName, dtoProp)
        {
        }

        public object ConvertValueForSet(object value)
        {
            if (value == null)
                return ConversionHelper.GetClrNullLegalEquivalent(this.ValueType);
            else
            {
                if (this.ValueType.IsEnum)
                    return ConversionHelper.ConvertToNativeEnumValue(value, this.ValueType);
                else
                    return ConversionHelper.ChangeType(value, this.ValueType);
            }
        }

        internal virtual bool CanGetValueFromConcreteItem
        {
            get
            {
                return false;
            }
        }

        internal virtual object GetValueFromConcreteItem(IDtoCapable item)
        {
            throw new NotImplementedException("This operation has not been implemented.");
        }
    }
}
