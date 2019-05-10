// BootFX - Application framework for .NET applications
// 
// File: EntityFlags.cs
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
	///	 Defines flags for an entity.
	/// </summary>
	internal enum EntityFlags
	{
		/// <summary>
		/// Defines a default, normal state.
		/// </summary>
		Normal = 0,

		/// <summary>
		/// Defines that the entity should be deleted then next time work units are requested.
		/// </summary>
		DeletePending = 1,

		/// <summary>
		/// Defines that the entity has been deleted from the underlying store.
		/// </summary>
		Deleted = 2,

		/// <summary>
		/// Defines that the entity is read-only.
		/// </summary>
		ReadOnly = 4,
	}
}
