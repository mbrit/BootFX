// BootFX - Application framework for .NET applications
// 
// File: StandardNamedDatabaseProvider.cs
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
	/// Summary description for StandardNamedDatabaseProvider.
	/// </summary>
	internal class StandardNamedDatabaseProvider : INamedDatabaseProvider
	{
		internal StandardNamedDatabaseProvider()
		{
		}

		public NamedDatabase GetNamedDatabase(string name)
		{
			if(name == null)
				throw new ArgumentNullException("name");
			if(name.Length == 0)
				throw new ArgumentOutOfRangeException("'name' is zero-length.");
			
			// return...
			return Database.Databases.GetDatabase(name, OnNotFound.ThrowException);
		}
	}
}
