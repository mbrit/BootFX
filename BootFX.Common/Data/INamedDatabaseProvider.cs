// BootFX - Application framework for .NET applications
// 
// File: INamedDatabaseProvider.cs
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
	/// Defines an interface that returns named databases.
	/// </summary>
	// mbr - 18-06-2008 - added.	
	public interface INamedDatabaseProvider
	{
		/// <summary>
		/// Gets the name of the database.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		NamedDatabase GetNamedDatabase(string name);
	}
}
