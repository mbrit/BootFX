// BootFX - Application framework for .NET applications
// 
// File: CommonDefault.cs
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
	/// Defines field defaults.
	/// </summary>
	public enum CommonDefault
	{
		/// <summary>
		/// Use DB null.
		/// </summary>
		/// <remarks>This is the default if the field is flagged as being nullable.</remarks>
		DBNull = 0,

		/// <summary>
		/// Uses the default CLR null, e.g. '0' for int32, 'null' for string, etc.
		/// </summary>
		/// <remarks>This is the default if teh field is not flagged as being nullable.</remarks>
		ClrNullEquivalent = 1,

		/// <summary>
		/// Uses the actual UTC now value used at the time of insertion.
		/// </summary>
		UtcNow = 2,

		/// <summary>
		/// Uses a custom literal value.
		/// </summary>
		CustomLiteral = 3,

		/// <summary>
		/// Uses a delegate to get the value.
		/// </summary>
		Custom = 4,

		/// <summary>
		/// Creates a new GUID.
		/// </summary>
		NewGuid = 5
	}
}
