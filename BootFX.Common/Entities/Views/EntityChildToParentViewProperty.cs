// BootFX - Application framework for .NET applications
// 
// File: EntityChildToParentViewProperty.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.ComponentModel;

namespace BootFX.Common.Entities
{
	/// <summary>
	/// Provides access to a property descriptor for an <see cref="EntityLink"></see>.
	/// </summary>
	public class EntityLinkViewProperty : EntityViewProperty
	{
		/// <summary>
		/// Private field to support <c>Link</c> property.
		/// </summary>
		private EntityLink _link;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="link"></param>
		public EntityLinkViewProperty(EntityLink link) : base(link.EntityType)
		{
			if(link == null)
				throw new ArgumentNullException("link");
			
			_link = link;
		}

		/// <summary>
		/// Gets the link.
		/// </summary>
		public EntityLink Link
		{
			get
			{
				return _link;
			}
		}

		/// <summary>
		/// Creates the property descriptor.
		/// </summary>
		/// <returns></returns>
		public override System.ComponentModel.PropertyDescriptor GetPropertyDescriptor()
		{
			if(Link == null)
				throw new ArgumentNullException("Link");

			// mbr - 28-03-2006 - added display name...			
			PropertyDescriptor prop = this.Link.GetPropertyDescriptor();
			if(prop == null)
				throw new InvalidOperationException("prop is null.");

			// set...
			if(prop is PropertyDescriptorBase)
				((PropertyDescriptorBase)prop).CustomDisplayName = this.ResolvedDisplayName;
			
			// return...
			return prop;
		}

		public override string DisplayName
		{
			get
			{
				if(this.Link != null)
					return this.Link.Name;
				else
					return "(No link)";
			}
		}
	}
}
