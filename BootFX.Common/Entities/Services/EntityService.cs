// BootFX - Application framework for .NET applications
// 
// File: EntityService.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using BootFX.Common.Management;
using System.Security;

namespace BootFX.Common.Entities
{
	/// <summary>
	/// Base class for entity services.
	/// </summary>
	public abstract class EntityService : LoggableMarshalByRefObject, IEntityType
	{
		/// <summary>
		/// Private field to support <c>EntityType</c> property.
		/// </summary>
		private EntityType _entityType;
		
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="entityType"></param>
		protected EntityService(EntityType entityType)
		{
			if(entityType == null)
				throw new ArgumentNullException("entityType");
			
			_entityType = entityType;
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

		/// <summary>
		/// Initializes the lifetime service so that this object never expires.
		/// </summary>
		/// <returns></returns>
        [SecurityCritical]
        public override object InitializeLifetimeService()
		{
			return null;
		}
	}
}
