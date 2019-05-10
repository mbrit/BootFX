// BootFX - Application framework for .NET applications
// 
// File: EntityMemberType.cs
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
	/// Defines types of entity members.
	/// </summary>
	[Flags()]
	public enum EntityMemberType
	{
		/// <summary>
		/// Refers to <see cref="EntityField"></see>.
		/// </summary>
		Field = 1,

		/// <summary>
		/// Refers to <see cref="EntityLink"></see>.
		/// </summary>
		Link = 2,

		/// <summary>
		/// Refers to <see cref="EntityProperty"></see>.
		/// </summary>
		Property = 4,

		/// <summary>
		/// Includes string versions of non-string descriptors.
		/// </summary>
		IncludeStringEquivalents = 8,

		/// <summary>
		/// Refers to all members.
		/// </summary>
		All = Field | Link | Property | IncludeStringEquivalents
	}
}
