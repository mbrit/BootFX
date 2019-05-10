// BootFX - Application framework for .NET applications
// 
// File: SlotFlags.cs
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
	///	 Defines the state of storage slots on <see cref="Entity"></see>.
	/// </summary>
	[Flags()]
	internal enum SlotFlags
	{
		/// <summary>
		/// Defines a normal, default state.
		/// </summary>
		Normal = 0,

		/// <summary>
		/// Defines that the value has been loaded.
		/// </summary>
		Loaded = 1,

		/// <summary>
		/// Defines that the value has been modified.
		/// </summary>
		Modified = 2
	}
}
