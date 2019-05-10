// BootFX - Application framework for .NET applications
// 
// File: ChangeNotificationContext.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using BootFX.Common.Data;

namespace BootFX.Common.Entities
{
	/// <summary>
	/// Defines an instance of <c>ChangeNotificationContext</c>.
	/// </summary>
	public class ChangeNotificationContext : IEntityType
	{
		/// <summary>
		/// Private field to support <see cref="EntityType"/> property.
		/// </summary>
		private EntityType _entityType;

		/// <summary>
		/// Private field to support <see cref="Entity"/> property.
		/// </summary>
		private object _entity;

		/// <summary>
		/// Private field to support <see cref="Unit"/> property.
		/// </summary>
		private WorkUnit _unit;
		
		internal ChangeNotificationContext(EntityType entityType, object entity, WorkUnit unit)
		{
			if(entityType == null)
				throw new ArgumentNullException("entityType");
			if(entity == null)
				throw new ArgumentNullException("entity");
			if(unit == null)
				throw new ArgumentNullException("unit");

			// set...
			_entityType = entityType;
			_entity = entity;
			_unit = unit;
		}

		/// <summary>
		/// Gets the unit.
		/// </summary>
		public WorkUnit Unit
		{
			get
			{
				return _unit;
			}
		}

		private WorkUnitType UnitType
		{
			get
			{
				if(Unit == null)
					throw new InvalidOperationException("Unit is null.");
				return this.Unit.WorkUnitType;
			}
		}

		public bool IsUpdate
		{
			get
			{
				if(this.UnitType == WorkUnitType.Update)
					return true;
				else
					return false;
			}
		}

		public bool IsInsert
		{
			get
			{
				if(this.UnitType == WorkUnitType.Insert)
					return true;
				else
					return false;
			}
		}

		public bool IsDelete
		{
			get
			{
				if(this.UnitType == WorkUnitType.Delete)
					return true;
				else
					return false;
			}
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
		/// Gets the entitytype.
		/// </summary>
		public EntityType EntityType
		{
			get
			{
				return _entityType;
			}
		}
	}
}
