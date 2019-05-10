// BootFX - Application framework for .NET applications
// 
// File: EntityCancelEventArgs.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.ComponentModel;

namespace BootFX.Common.Entities
{
	/// <summary>
	/// Event handler for use with <see cref="EntityEventArgs"></see>.
	/// </summary>
	public delegate void EntityCancelEventHandler(object sender, EntityEventArgs e);

	/// <summary>
	/// Event args containing an entity.
	/// </summary>
	public class EntityCancelEventArgs : CancelEventArgs, IEntityType
	{
		/// <summary>
		/// Private field to support <see cref="Entity"/> property.
		/// </summary>
		private object _entity;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="entity"></param>
		public EntityCancelEventArgs(object entity)
		{
			if(entity == null)
				throw new ArgumentNullException("entity");
			
			// set...
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
		}

		/// <summary>
		/// Gets the entity type.
		/// </summary>
		public EntityType EntityType
		{
			get
			{
				if(Entity == null)
					throw new InvalidOperationException("Entity is null.");
				return EntityType.GetEntityType(this.Entity, OnNotFound.ThrowException);
			}
		}
	}
}
