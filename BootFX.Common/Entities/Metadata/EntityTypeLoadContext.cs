// BootFX - Application framework for .NET applications
// 
// File: EntityTypeLoadContext.cs
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

namespace BootFX.Common.Entities
{
    /// <summary>
    /// Defines a class that holds context information when loading entities.
    /// </summary>
    // mbr - 2009-09-21 - added in order to address problem of circular dependencies on load...
    internal class EntityTypeLoadContext : IDisposable
    {
        /// <summary>
        /// Private value to support the <see cref="TypeStack">TypeStack</see> property.
        /// </summary>
        private List<EntityType> _fixupList = new List<EntityType>();

        /// <summary>
		/// Private field to hold singleton instance.
		/// </summary>
        [ThreadStatic()]
		private static EntityTypeLoadContext _current = new EntityTypeLoadContext();
				
		/// <summary>
		/// Private constructor.
		/// </summary>
		private EntityTypeLoadContext()
		{
		}
						
		/// <summary>
		/// Gets the singleton instance of <see cref="EntityTypeLoadContext">EntityTypeLoadContext</see>.
		/// </summary>
		internal static EntityTypeLoadContext Current
		{
			get
			{
                // null is ok here - just means there is no context...
				return _current;
			}
		}

        internal static EntityTypeLoadContext Initialize()
        {
            _current = new EntityTypeLoadContext();
            return _current;
        }

        public void Dispose()
        {
            // if...
            if (_current == this)
            {
                // run...
                if (this.FixupList.Count > 0)
                    EntityType.HandleDeferredFixups(this.FixupList);

                // ok...
                _current = null;
            }

            // sup...
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Gets the TypeStack value.
        /// </summary>
        private List<EntityType> FixupList
        {
            get
            {
                return _fixupList;
            }
        }

        internal void AddToFixupList(EntityType et)
        {
            if (et == null)
                throw new ArgumentNullException("et");

            // add...
            this.FixupList.Add(et);
        }
    }
}
