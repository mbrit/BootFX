// BootFX - Application framework for .NET applications
// 
// File: DtoMember.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using BootFX.Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BootFX.Common.Dto
{
    public abstract class DtoMember
    {
        public string Name { get; private set; }
        public string JsonName { get; private set; }
        internal PropertyInfo DtoProperty { get; private set; }

        public DtoMember(string name, string jsonName, PropertyInfo dtoProp)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (name.Length == 0)
                throw new ArgumentException("'name' is zero-length.");
            if (jsonName == null)
                throw new ArgumentNullException("jsonName");
            if (jsonName.Length == 0)
                throw new ArgumentException("'jsonName' is zero-length.");
            if (dtoProp == null)
                throw new ArgumentNullException("dtoProp");

            this.Name = name;
            this.JsonName = jsonName;
            this.DtoProperty = dtoProp;
        }

        internal object DefaultValue
        {
            get
            {
                if (this.DtoProperty.PropertyType.IsEnum)
                    return Enum.GetValues(this.DtoProperty.PropertyType).GetValue(0);
                else
                    return ConversionHelper.ChangeType(null, this.DtoProperty.PropertyType);
            }
        }

        public Type ValueType
        {
            get
            {
                return this.DtoProperty.PropertyType;
            }
        }

        public bool CanWrite
        {
            get
            {
                return this.DtoProperty.CanWrite;
            }
        }
    }
}
