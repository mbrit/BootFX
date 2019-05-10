// BootFX - Application framework for .NET applications
// 
// File: DtoRedirectedValue.cs
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
using System.Threading.Tasks;

namespace BootFX.Common.Dto
{
    internal class DtoRedirectedValue : DtoValue
    {
        private DtoAdHocField Field { get; set; }

        internal DtoRedirectedValue(DtoAdHocField field)
        {
            this.Field = field;
        }

        internal override object GetValue(IDtoBase dto)
        {
            return this.Field.DtoProperty.GetValue(dto);
        }

        internal override void SetValue(IDtoBase dto, object value)
        {
            // stage the values...
            var existing = this.GetValue(dto);
            value = this.Field.ConvertValueForSet(value);

            // check...
            if (HasChanged(existing, value))
            {
                this.Field.DtoProperty.SetValue(dto, value);
                this.NumChanges++;
            }

            // add...
            this.NumTouches++;
        }
    }
}
