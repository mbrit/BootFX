// BootFX - Application framework for .NET applications
// 
// File: DtoValue.cs
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
    internal abstract class DtoValue
    {
        protected int NumChanges { get; set; }
        protected int NumTouches { get; set; }

        internal DtoValue()
        {
        }

        internal abstract object GetValue(IDtoBase dto);
        internal abstract void SetValue(IDtoBase dto, object value);

        protected static bool HasChanged(object a, object b)
        {
            if (a == null && b == null)
                return false;
            if (a == null && b != null)
                return true;
            if (a != null && b == null)
                return true;

            if(a.GetType() == b.GetType())
            {
                var tc = Type.GetTypeCode(a.GetType());
                if(tc != TypeCode.Object)
                    return !(object.Equals(a, b));
                else
                    return true;
            }
            else
                return true;
        }

        internal bool HasChangedValue
        {
            get
            {
                return this.NumChanges > 0;
            }
        }

        internal bool HasTouchedValue
        {
            get
            {
                return this.NumTouches > 0;
            }
        }
    }
}
