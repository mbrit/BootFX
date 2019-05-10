// BootFX - Application framework for .NET applications
// 
// File: FetchFlags.cs
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
	/// Defines fetch flags.
	/// </summary>
	[Flags()]
	public enum FetchFlags
	{
		/// <summary>
		/// Default option - simply fetch the value.
		/// </summary>
		None = 0,

		/// <summary>
		/// Keeps any encrypted value as an encrypted value type.
		/// </summary>
		KeepEncrypted = 1
	}
}
