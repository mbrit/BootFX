// BootFX - Application framework for .NET applications
// 
// File: IEntityType.cs
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
	/// Describes an interface that can provide the type of entity that object represents, maps to, or holds a list of.
	/// </summary>
	public interface IEntityType
	{
		/// <summary>
		/// Gets the entity type for the given object.
		/// </summary>
		EntityType EntityType
		{
			get;
		}
	}
}
