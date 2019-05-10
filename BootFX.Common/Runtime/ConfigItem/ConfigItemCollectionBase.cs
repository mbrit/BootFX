// BootFX - Application framework for .NET applications
// 
// File: ConfigItemCollectionBase.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Data;
using System.Collections;
using System.Collections.Specialized;
using BootFX.Common;
using BootFX.Common.Data;
using BootFX.Common.Entities;
using BootFX.Common.Entities.Attributes;

namespace BootFX.Common
{
    /// <summary>
    /// Defines the base collection for entities of type <see cref="ConfigItem"/>.
    /// </summary>
    [Serializable()]
    public abstract class ConfigItemCollectionBase : BootFX.Common.Entities.EntityCollection
    {
        
        /// <summary>
        /// Constructor.
        /// </summary>
        protected ConfigItemCollectionBase() : 
                base(typeof(ConfigItem))
        {
        }
        
        /// <summary>
        /// Deserialization constructor.
        /// </summary>
        protected ConfigItemCollectionBase(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : 
                base(info, context)
        {
        }
        
        /// <summary>
        /// Gets or sets the item with the given index.
        /// </summary>
        public ConfigItem this[int index]
        {
            get
            {
                return ((ConfigItem)(this.GetItem(index)));
            }
            set
            {
                this.SetItem(index, value);
            }
        }
        
        /// <summary>
        /// Adds a <see cref="ConfigItem"/> instance to the collection.
        /// </summary>
        public int Add(ConfigItem item)
        {
            return base.Add(item);
        }
        
        /// <summary>
        /// Adds a range of <see cref="ConfigItem"/> instances to the collection.
        /// </summary>
        public void AddRange(ConfigItem[] items)
        {
            base.AddRange(items);
        }
        
        /// <summary>
        /// Adds a range of <see cref="ConfigItem"/> instances to the collection.
        /// </summary>
        public void AddRange(ConfigItemCollection items)
        {
            base.AddRange(items);
        }
    }
}
