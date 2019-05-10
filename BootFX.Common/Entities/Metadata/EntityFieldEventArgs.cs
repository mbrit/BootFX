// BootFX - Application framework for .NET applications
// 
// File: EntityFieldEventArgs.cs
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
	/// Delegate for use with <c>EntityFieldEventArgs</c>.
	/// </summary>
	public delegate void EntityFieldEventHandler(object sender, EntityFieldEventArgs e);

	/// <summary>
	/// EventArgs for use with <c>EntityField</c> objects.
	/// </summary>
	public class EntityFieldEventArgs : EventArgs
	{
		/// <summary>
		/// Private field to support <c>EntityField</c> property.
		/// </summary>
		private EntityField _entityField;
	
		/// <summary>
		/// Constructor.
		/// </summary>
		public EntityFieldEventArgs(EntityField entityField)
		{
			if(entityField == null)
				throw new ArgumentNullException("entityField");
			_entityField = entityField;
		}
		
		/// <summary>
		/// Gets the field.
		/// </summary>
		public EntityField EntityField
		{
			get
			{
				return _entityField;
			}
		}
	}
}
