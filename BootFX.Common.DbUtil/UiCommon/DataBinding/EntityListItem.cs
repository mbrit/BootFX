// BootFX - Application framework for .NET applications
// 
// File: EntityListItem.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using BootFX.Common.Entities;

namespace BootFX.Common.UI
{
	/// <summary>
	/// Defines an entity list item.
	/// </summary>
	public class EntityListItem : IEntityType
	{
		/// <summary>
		/// Private field to support <see cref="Entity"/> property.
		/// </summary>
		private object _entity;
		
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="entity"></param>
		public EntityListItem(object entity)
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
				IEntityType defer = this.Entity as IEntityType;
				if(defer != null)
					return defer.EntityType;
				else
					return null;
			}
		}

		public override string ToString()
		{
			if(Entity == null)
				throw new InvalidOperationException("Entity is null.");
			try
			{
				return this.Entity.ToString();
			}
			catch(Exception ex)
			{
				return "[" + ex.Message + "]";
			}
		}
	}
}
