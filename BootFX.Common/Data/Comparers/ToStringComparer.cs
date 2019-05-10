// BootFX - Application framework for .NET applications
// 
// File: ToStringComparer.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace BootFX.Common.Data.Comparers
{
    internal class ToStringComparer : ComparerBase
    {
        internal ToStringComparer(CultureInfo culture)
            : base(culture)
        {
        }

        protected override int DoCompare(object x, object y)
        {
            string stringX = DoConvert(x);
            string stringY = DoConvert(y);
            return string.Compare(stringX, stringY, true, this.Culture);
        }

        private string DoConvert(object obj)
        {
            if (obj == null)
                return string.Empty;
            else
                return obj.ToString();
        }
    }
}
