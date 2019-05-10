// BootFX - Application framework for .NET applications
// 
// File: EntityCodeFileType.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;

namespace BootFX.Common.CodeGeneration
{
	/// <summary>
	/// Defines code file types.
	/// </summary>
	public enum EntityCodeFileType
	{
		EntityBase = 0,
		Entity = 1,
		CollectionBase = 2,
		Collection = 3,
		DtoBase = 4,
		Dto = 5,
	}
}
