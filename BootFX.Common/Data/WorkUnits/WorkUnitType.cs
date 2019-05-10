// BootFX - Application framework for .NET applications
// 
// File: WorkUnitType.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;

namespace BootFX.Common.Data
{
	/// <summary>
	/// Defines types of work units.
	/// </summary>
	public enum WorkUnitType
	{
		/// <summary>
		/// A work unit that inserts items into the underlying store.
		/// </summary>
		Insert = 0,

		/// <summary>
		/// A work unit that updates items in the underlying store.
		/// </summary>
		Update = 1,

		/// <summary>
		/// A work unit that deletes items from the underlying store.
		/// </summary>
		Delete = 2,

		/// <summary>
		/// A work unit that does schema changes.
		/// </summary>
		// mbr - 31-10-2005 - added...		
		Schema = 3
	}
}
