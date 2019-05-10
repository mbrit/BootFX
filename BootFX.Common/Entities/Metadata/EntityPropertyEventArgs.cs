// BootFX - Application framework for .NET applications
// 
// File: EntityPropertyEventArgs.cs
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
	/// Delegate for use with <c>EntityPropertyEventArgs</c>.
	/// </summary>
	public delegate void EntityPropertyEventHandler(object sender, EntityPropertyEventArgs e);

	/// <summary>
	/// EventArgs for use with <c>EntityProperty</c> objects.
	/// </summary>
	public class EntityPropertyEventArgs : EventArgs
	{
		/// <summary>
		/// Private Property to support <c>EntityProperty</c> property.
		/// </summary>
		private EntityProperty _entityProperty;
	
		/// <summary>
		/// Constructor.
		/// </summary>
		public EntityPropertyEventArgs(EntityProperty entityProperty)
		{
			if(entityProperty == null)
				throw new ArgumentNullException("entityProperty");
			_entityProperty = entityProperty;
		}
		
		/// <summary>
		/// Gets the Property.
		/// </summary>
		public EntityProperty EntityProperty
		{
			get
			{
				return _entityProperty;
			}
		}
	}
}
