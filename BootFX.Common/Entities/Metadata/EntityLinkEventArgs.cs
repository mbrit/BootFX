// BootFX - Application framework for .NET applications
// 
// File: EntityLinkEventArgs.cs
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
	/// Delegate for use with <c>EntityLinkEventArgs</c>.
	/// </summary>
	public delegate void EntityLinkEventHandler(object sender, EntityLinkEventArgs e);

	/// <summary>
	/// EventArgs for use with <c>EntityLink</c> objects.
	/// </summary>
	public class EntityLinkEventArgs : EventArgs
	{
		/// <summary>
		/// Private Link to support <c>EntityLink</c> property.
		/// </summary>
		private EntityLink _entityLink;
	
		/// <summary>
		/// Constructor.
		/// </summary>
		public EntityLinkEventArgs(EntityLink entityLink)
		{
			if(entityLink == null)
				throw new ArgumentNullException("entityLink");
			_entityLink = entityLink;
		}
		
		/// <summary>
		/// Gets the Link.
		/// </summary>
		public EntityLink EntityLink
		{
			get
			{
				return _entityLink;
			}
		}
	}
}
