// BootFX - Application framework for .NET applications
// 
// File: BfxLookupTT.cs
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
    public class BfxLookup<TKey1, TKey2, TValue>
    {
        private BfxLookup<TKey1, BfxLookup<TKey2, TValue>> InnerLookup { get; set; }

        public event EventHandler<CreateLookupItemEventArgs<TKey1, TKey2, TValue>> CreateItemValue;

        public BfxLookup()
        {
            this.InnerLookup = new BfxLookup<TKey1, BfxLookup<TKey2, TValue>>();
            this.InnerLookup.CreateItemValue += InnerLookup_CreateItemValue;
        }

        private void InnerLookup_CreateItemValue(object sender, CreateLookupItemEventArgs<TKey1, BfxLookup<TKey2, TValue>> e)
        {
            e.NewValue = new BfxLookup<TKey2, TValue>();
            e.NewValue.CreateItemValue += (theSender, theE) =>
            {
                var newE = new CreateLookupItemEventArgs<TKey1, TKey2, TValue>(e.Key, theE.Key);
                this.OnCreateItemValue(newE);

                // pass...
                theE.NewValue = newE.NewValue;
            };
        }

        private void OnCreateItemValue(CreateLookupItemEventArgs<TKey1, TKey2, TValue> e)
        {
            if (this.CreateItemValue != null)
                this.CreateItemValue(this, e);
        }

        public TValue this[TKey1 key1, TKey2 key2]
        {
            get
            {
                var inner = this.InnerLookup[key1];
                return inner[key2];
            }
        }
    }
}
