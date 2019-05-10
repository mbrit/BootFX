// BootFX - Application framework for .NET applications
// 
// File: EntityPersistenceHelper.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Collections;
using BootFX.Common.Data;

namespace BootFX.Common.Entities
{
	/// <summary>
	/// Class that provides a mechanism to save a list of entities.
	/// </summary>
	public class EntityPersistenceHelper
	{
		/// <summary>
		/// Private field to hold the singleton instance.
		/// </summary>
		private static EntityPersistenceHelper _current = new EntityPersistenceHelper();
		
		/// <summary>
		/// Private constructor.
		/// </summary>
		private EntityPersistenceHelper()
		{
		}
		
		/// <summary>
		/// Gets the singleton instance.
		/// </summary>
		public static EntityPersistenceHelper Current
		{
			get
			{
				return _current;
			}
		}

		/// <summary>
		/// Saves changes to the given entity.
		/// </summary>
		/// <param name="entity"></param>
		public void SaveChanges(object entity)
		{
			if(entity == null)
				throw new ArgumentNullException("entity");

			// list?
			if(entity is IList)
			{
				SaveChanges((IList)entity);
				return;
			}
			
			// get it...
			EntityType entityType = EntityType.GetEntityType(entity, OnNotFound.ThrowException);
			if(entityType == null)
				throw new ArgumentNullException("entityType");

			// save...
			IEntityPersistence persistence = entityType.Persistence;
			if(persistence == null)
				throw new ArgumentNullException("persistence");

			// save...
			persistence.SaveChanges(entity);
		}

		/// <summary>
		/// Saves changes to the given list.
		/// </summary>
		/// <param name="list"></param>
		public void SaveChanges(IList list)
		{
			if(list == null)
				throw new ArgumentNullException("list");
			
			// strong?
			if(list is IEntityType)
			{
				// get the entity type...
				EntityType entityType = ((IEntityType)list).EntityType;
				if(entityType != null)
				{
					SaveChangesToStrongCollection(list);
					return;
				}
			}

			// save changes to a weak list...
			SaveChangesToWeakCollection(list);
		}

		/// <summary>
		/// Saves changes to a weak collection.
		/// </summary>
		/// <param name="list"></param>
		private void SaveChangesToWeakCollection(IList list)
		{
			if(list == null)
				throw new ArgumentNullException("list");

            throw new NotImplementedException("This operation has not been implemented.");
		}

		/// <summary>
		/// Saves changes to a strong collection.
		/// </summary>
		/// <param name="list"></param>
		private void SaveChangesToStrongCollection(IList list)
		{
			if(list == null)
				throw new ArgumentNullException("list");

			if(!(list is IEntityType))
				throw new InvalidOperationException("List does not support IEntityType.");
			EntityType entityType = ((IEntityType)list).EntityType;
			if(entityType == null)
				throw new ArgumentNullException("entityType");

			// persistence...
			IEntityPersistence persistence = entityType.Persistence;
			if(persistence == null)
				throw new ArgumentNullException("persistence");

			// get all work...
			WorkUnitCollection units = new WorkUnitCollection();
			foreach(object entity in list)
				units.AddRange(persistence.GetWorkUnits(entity));

			// anything to do?
			if(units.Count == 0)
				return;

			// process that lot...
            IWorkUnitProcessor processor = new WorkUnitProcessor();
			if(processor == null)
				throw new InvalidOperationException("processor is null.");
			processor.Process(units);

			// reconcile...
			persistence.ReconcileWorkUnitProcessorResults(units);
		}
	}
}
