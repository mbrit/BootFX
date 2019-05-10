// BootFX - Application framework for .NET applications
// 
// File: LookupValueCollectionBase.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;

namespace BootFX.Common.Data
{
    using System;
    using System.IO;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Data;
    using System.Collections;
    using System.Collections.Specialized;
    using BootFX.Common;
    using BootFX.Common.Data;
    using BootFX.Common.Entities;
    using BootFX.Common.Entities.Attributes;
    
    
    /// <summary>
    /// Defines the base collection for entities of type <see cref="LookupValue"/>.
    /// </summary>
    [Serializable()]
    public abstract class LookupValueCollectionBase : BootFX.Common.Entities.EntityCollection
    {
        
        /// <summary>
        /// Constructor.
        /// </summary>
        protected LookupValueCollectionBase() : 
                base(typeof(LookupValue))
        {
        }
        
        /// <summary>
        /// Deserialization constructor.
        /// </summary>
        protected LookupValueCollectionBase(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : 
                base(info, context)
        {
        }
        
        /// <summary>
        /// Gets or sets the item with the given index.
        /// </summary>
        public LookupValue this[int index]
        {
            get
            {
                return ((LookupValue)(this.GetItem(index)));
            }
            set
            {
                this.SetItem(index, value);
            }
        }
        
        /// <summary>
        /// Adds a <see cref="LookupValue"/> instance to the collection.
        /// </summary>
        public int Add(LookupValue item)
        {
            return base.Add(item);
        }
        
        /// <summary>
        /// Adds a range of <see cref="LookupValue"/> instances to the collection.
        /// </summary>
        public void AddRange(LookupValue[] items)
        {
            base.AddRange(items);
        }
        
        /// <summary>
        /// Adds a range of <see cref="LookupValue"/> instances to the collection.
        /// </summary>
        public void AddRange(LookupValueCollection items)
        {
            base.AddRange(items);
        }
    }
}
