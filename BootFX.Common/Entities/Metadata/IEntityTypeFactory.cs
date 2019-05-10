// BootFX - Application framework for .NET applications
// 
// File: IEntityTypeFactory.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using BootFX.Common.Data;

namespace BootFX.Common.Entities
{
	/// <summary>
	/// Summary description for IEntityTypeFactory.
	/// </summary>
	// mbr - 06-09-2007 - case 668 for c7 - added.
	public interface IEntityTypeFactory
	{
		/// <summary>
		/// Creates an entity type.
		/// </summary>
		/// <returns></returns>
		EntityType CreateEntityType(Type type, Type collectionType, NativeName nativeName, string databaseName, bool isSystemTable);
	}
}
