// BootFX - Application framework for .NET applications
// 
// File: SqlTableScriptFlags.cs
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
	/// Flags for use with table script creation.
	/// </summary>
	[Flags()]
	public enum SqlTableScriptFlags
	{
		/// <summary>
		/// No defined flags.
		/// </summary>
		None = 0,

		/// <summary>
		/// When set, a 'drop table' statement will be inserted.
		/// </summary>
		IncludeDropObject = 1,

		/// <summary>
		/// Indicates that errors should not be thrown is objects to not exists.
		/// </summary>
		IgnoreExistenceErrors = 2,
	}
}
