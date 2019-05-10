// BootFX - Application framework for .NET applications
// 
// File: CreateLookupItemEventArgsTT.cs
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

namespace BootFX.Common
{
    public class CreateLookupItemEventArgs<TKey1, TKey2, TValue> : EventArgs
    {
        public TKey1 Key1 { get; private set; }
        public TKey2 Key2 { get; private set; }
        public TValue NewValue { get; set; }

        public CreateLookupItemEventArgs(TKey1 key1, TKey2 key2)
        {
            this.Key1 = key1;
            this.Key2 = key2;
        }
    }
}
