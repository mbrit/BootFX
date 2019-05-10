// BootFX - Application framework for .NET applications
// 
// File: EntityPropertyAttribute.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;

namespace BootFX.Common.Entities.Attributes
{
	/// <summary>
	/// Marks a property as being exposed as an <see cref="EntityProperty"></see>.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
	public class EntityPropertyAttribute : Attribute
	{
		public EntityPropertyAttribute()
		{
		}
	}
}
