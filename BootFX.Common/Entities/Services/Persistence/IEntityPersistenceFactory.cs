// BootFX - Application framework for .NET applications
// 
// File: IEntityPersistenceFactory.cs
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
	/// Interface for obtaining entity persistence objects.
	/// </summary>
	public interface IEntityPersistenceFactory
	{
		/// <summary>
		/// Gets entity persistence.
		/// </summary>
		/// <param name="entityType"></param>
		/// <returns></returns>
		IEntityPersistence GetPersistence(string typeName);
	}
}
