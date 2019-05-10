// BootFX - Application framework for .NET applications
// 
// File: DtoStoredValue.cs
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
using System.Text;

namespace BootFX.Common.Dto
{
    internal class DtoStoredValue : DtoValue
    {
        private object _initialValue;
        private object _value;

        internal DtoStoredValue(object initialValue)
        {
            _initialValue = initialValue;
            _value = initialValue;
        }

        internal override object GetValue(IDtoBase dto)
        {
            return _value;
        }

        internal override void SetValue(IDtoBase dto, object value)
        {
            if (HasChanged(_value, value))
            {
                _value = value;
                this.NumChanges++;
            }

            // record...
            this.NumTouches++;
        }
    }
}
