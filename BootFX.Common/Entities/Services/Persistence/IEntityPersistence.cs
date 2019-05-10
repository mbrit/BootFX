// BootFX - Application framework for .NET applications
// 
// File: IEntityPersistence.cs
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
using System.Collections.Generic;

namespace BootFX.Common.Entities
{
	/// <summary>
	///	 Common interface for entity persistence.
	/// </summary>
	/// <seealso cref="EntityPersistence"></seealso>
	public interface IEntityPersistence : IEntityType
	{
		/// <summary>
		/// Saves changes to the given entity.
		/// </summary>
		/// <param name="entity"></param>
		void SaveChanges(object entity, ITimingBucket timings = null);
        void SaveChangesOutsideTransaction(object entity, ITimingBucket timings = null);

		/// <summary>
		/// Demand loads data for the given field.
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="fieldNames"></param>
		void DemandLoad(object entity, EntityField[] fields);

		/// <summary>
		/// Gets the parent for a given item.
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="link"></param>
		/// <returns></returns>
		object GetParent(object entity, string linkName, string[] parentFieldNames);

		/// <summary>
		/// Gets the work units for the given entity.
		/// </summary>
		/// <param name="entity"></param>
		/// <returns></returns>
		WorkUnitCollection GetWorkUnits(object entity);

		/// <summary>
		/// Reconciles the results of a work unit process.
		/// </summary>
		/// <param name="units"></param>
		void ReconcileWorkUnitProcessorResults(WorkUnitCollection units);

		/// <summary>
		/// Gets the entity with the given ID.
		/// </summary>
		/// <param name="keyValues"></param>
		/// <returns></returns>
		object GetById(object[] keyValues, OnNotFound onNotFound = OnNotFound.ReturnNull);

		/// <summary>
		/// Gets all entities.
		/// </summary>
		/// <returns></returns>
		IList GetAll();

        // get the entities with the given ids...
        IEnumerable<T> GetByIds<T>(IEnumerable<long> set)
            where T : Entity;

        // get the entities with the given ids using a paged request...
        PagedDataResult<T> GetByIdsPaged<T>(IEnumerable<long> set, IPagedDataRequestSource source)
            where T : Entity;

        void Delete(IEnumerable<long> ids, bool inTransaction = true);
    }
}
