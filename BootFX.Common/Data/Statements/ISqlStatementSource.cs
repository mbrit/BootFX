// BootFX - Application framework for .NET applications
// 
// File: ISqlStatementSource.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using BootFX.Common.Data.Formatters;

namespace BootFX.Common.Data
{
	/// <summary>
	/// Interface for objects that expose a SQL statement.
	/// </summary>
	public interface ISqlStatementSource
	{
		/// <summary>
		/// Gets the statement.
		/// </summary>
		/// <returns></returns>
		SqlStatement GetStatement();
	}
}
