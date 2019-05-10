// BootFX - Application framework for .NET applications
// 
// File: IWorkUnit.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Collections;
using BootFX.Common.Entities;

namespace BootFX.Common.Data
{
	/// <summary>
	///	 Defines an interface that declares required functionality for a work unit.
	/// </summary>
	public interface IWorkUnit : IEntityType
	{
		/// <summary>
		/// Gets the entity.
		/// </summary>
		object Entity
		{
			get;
		}

		/// <summary>
		/// Gets the results.
		/// </summary>
		WorkUnitResultsBag ResultsBag
		{
			get;
		}

		/// <summary>
		/// Gets the work unit type.
		/// </summary>
		WorkUnitType WorkUnitType
		{
			get;
		}

		/// <summary>
		/// Gets the statement(s) that have to be executed to complete this work unit.
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		SqlStatement[] GetStatements(WorkUnitProcessingContext context);

        bool NeedsTransaction
        {
            get;
        }

		/// <summary>
		/// Processes the work unit.
		/// </summary>
		/// <param name="context"></param>
		void Process(WorkUnitProcessingContext context, ITimingBucket timings);

		/// <summary>
		/// Sets the bag for the results
		/// </summary>
		/// <param name="resultsBag"></param>
		void SetResultsBag(WorkUnitResultsBag resultsBag);

		/// <summary>
		/// Gets the values touched as part of this operation.
		/// </summary>
		/// <returns></returns>
		// mbr - 2007-04-02 - added.
		TouchedValueWrapper GetTouchedValues();
	}
}
