// BootFX - Application framework for .NET applications
// 
// File: EntityPersistenceFactory.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using BootFX.Common.Management;

namespace BootFX.Common.Entities
{
	/// <summary>
	/// Describes a class that can provide entity persistence objects.
	/// </summary>
	internal class EntityPersistenceFactory : LoggableMarshalByRefObject, IEntityPersistenceFactory
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		public EntityPersistenceFactory()
		{
		}

		/// <summary>
		/// Gets persistence for the given type.
		/// </summary>
		/// <param name="typeName"></param>
		/// <returns></returns>
		public IEntityPersistence GetPersistence(string typeName)
		{
			if(typeName == null)
				throw new ArgumentNullException("typeName");
			if(typeName.Length == 0)
				throw new ArgumentOutOfRangeException("'typeName' is zero-length.");

			// load...
			Type type = Type.GetType(typeName, true, false);
			if(type == null)
				throw new InvalidOperationException("type is null.");

			// return...
			EntityType entityType = EntityType.GetEntityType(type, OnNotFound.ThrowException);
			if(entityType == null)
				throw new InvalidOperationException("entityType is null.");

			// return...
			return new SqlEntityPersistence(entityType);
		}
	}
}
