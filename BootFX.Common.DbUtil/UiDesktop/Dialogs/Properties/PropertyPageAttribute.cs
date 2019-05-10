// BootFX - Application framework for .NET applications
// 
// File: PropertyPageAttribute.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;

namespace BootFX.Common.UI.Desktop
{
	/// <summary>
	/// Describes an entity for a property page.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class PropertyPageAttribute : Attribute
	{
		/// <summary>
		/// Stores the value for <see cref="EntityType" />
		/// </summary>
		private Type _entityType;

		/// <summary>
		/// Instantiates a new instance of <see cref="PropertyPageAttribute"/>.
		/// </summary>
		public PropertyPageAttribute(Type entityType)
		{
			if (entityType == null)
				throw new ArgumentNullException("entityType");

			// set...
			_entityType = entityType;
		}

		/// <summary>
		/// Gets the entityType.
		/// </summary>
		public Type EntityType
		{
			get
			{
				return _entityType;
			}
		}
	}
}
