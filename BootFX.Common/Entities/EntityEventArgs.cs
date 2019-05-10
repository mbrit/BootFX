// BootFX - Application framework for .NET applications
// 
// File: EntityEventArgs.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;

namespace BootFX.Common.Entities
{
	/// <summary>
	/// Event handler for use with <see cref="EntityEventArgs"></see>.
	/// </summary>
	public delegate void EntityEventHandler(object sender, EntityEventArgs e);

	/// <summary>
	/// Event args containing an entity.
	/// </summary>
	public class EntityEventArgs : EventArgs, IEntityType
	{
		/// <summary>
		/// Private field to support <see cref="Entity"/> property.
		/// </summary>
		private object _entity;
        public bool HasChanged { get; private set; }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="entity"></param>
		public EntityEventArgs(object entity)
		{
			_entity = entity;
		}

		/// <summary>
		/// Gets the entity.
		/// </summary>
		public object Entity
		{
			get
			{
				return _entity;
			}
            set
            {
                _entity = value;

                if (_entity != value)
                    this.HasChanged = true;
            }
		}

		/// <summary>
		/// Gets the entity type.
		/// </summary>
		public EntityType EntityType
		{
			get
			{
                if (this.Entity != null)
                    return EntityType.GetEntityType(this.Entity, OnNotFound.ThrowException);
                else
                    return null;
			}
		}
	}
}
