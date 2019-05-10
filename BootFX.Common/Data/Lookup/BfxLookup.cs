// BootFX - Application framework for .NET applications
// 
// File: BfxLookup.cs
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

namespace BootFX.Common
{
    public class BfxLookup<TKey, TValue>
    {
        private Lookup InnerLookup { get; set; }

        public event EventHandler<CreateLookupItemEventArgs<TKey, TValue>> CreateItemValue;

        public BfxLookup()
        {
            this.InnerLookup = new Lookup();
            this.InnerLookup.CreateItemValue += InnerLookup_CreateItemValue;
        }

        void InnerLookup_CreateItemValue(object sender, CreateLookupItemEventArgs e)
        {
            var newE = new CreateLookupItemEventArgs<TKey, TValue>((TKey)e.Key);
            if (this.CreateItemValue != null)
                this.CreateItemValue(this, newE);

            e.NewValue = newE.NewValue;
        }

        public TValue this[TKey key]
        {
            get
            {
                return (TValue)this.InnerLookup[key];
            }
            set
            {
                this.InnerLookup[key] = value;
            }
        }

        public void Clear()
        {
            this.InnerLookup.Clear();
        }

        public IEnumerable<TKey> Keys
        {
            get
            {
                var results = new List<TKey>();
                foreach (TKey key in this.InnerLookup.Keys)
                    results.Add(key);
                return results;
            }
        }

        public IEnumerable<TValue> Values
        {
            get
            {
                var results = new List<TValue>();
                foreach (TValue value in this.InnerLookup.Values)
                    results.Add(value);
                return results;
            }
        }

        public TimeSpan ExpirationPeriod
        {
            get
            {
                return this.InnerLookup.ExpirationPeriod;
            }
            set
            {
                this.InnerLookup.ExpirationPeriod = value;
            }
        }
    }
}
