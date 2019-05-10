// BootFX - Application framework for .NET applications
// 
// File: EntityLookup.cs
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
using System.Text;
using System.Threading.Tasks;

namespace BootFX.Common.Entities
{
    public class EntityLookup<T> : IEntityType
    {
        private BfxLookup<long, T> Items { get; set; }

        public EntityLookup()
        {
            this.Items = new BfxLookup<long, T>();
            this.Items.CreateItemValue += Items_CreateItemValue;
        }

        void Items_CreateItemValue(object sender, CreateLookupItemEventArgs<long, T> e)
        {
            e.NewValue = (T)this.EntityType.Persistence.GetById(new object[] { e.Key });
        }

        public EntityType EntityType
        {
            get
            {
                return typeof(T).ToEntityType();
            }
        }

        public T this[int id]
        {
            get
            {
                return this.Items[id];
            }
        }

        public T this[long id]
        {
            get
            {
                return this.Items[id];
            }
        }

        public IEnumerable<T> GetItems()
        {
            return this.Items.Values;
        }

        public void PreloadItems(IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                var id = ConversionHelper.ToInt64(this.EntityType.Storage.GetKeyValues(item).First());
                this.Items[id] = item;
            }
        }
    }
}
