// BootFX - Application framework for .NET applications
// 
// File: ComparerReverser.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Collections;
using System.Text;

namespace BootFX.Common.Data
{
    internal class ComparerReverser : IComparer
    {
        private IComparer _innerComparer;

        internal ComparerReverser(IComparer innerComparer)
        {
            if (innerComparer == null)
                throw new ArgumentNullException("innerComparer");
            _innerComparer = innerComparer;
        }

        public int Compare(object x, object y)
        {
            if (_innerComparer == null)
                throw new InvalidOperationException("'_innerComparer' is null.");
            return 0 - _innerComparer.Compare(x, y);
        }
    }

}
