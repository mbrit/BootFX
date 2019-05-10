// BootFX - Application framework for .NET applications
// 
// File: SqlCombine.cs
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
	/// Decribes how clauses are combined in SQL statements.
	/// </summary>
	public enum SqlCombine
	{
		/// <summary>
		/// Combine with an "AND" directive.
		/// </summary>
		And = 0,

		/// <summary>
		/// Summary with an "OR" directive.
		/// </summary>
		Or = 1
	}
}
