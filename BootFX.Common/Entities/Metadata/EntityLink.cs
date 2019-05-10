// BootFX - Application framework for .NET applications
// 
// File: EntityLink.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Collections;
using System.Globalization;
using BFX = BootFX.Common.Data.Comparers;

namespace BootFX.Common.Entities
{
	/// <summary>
	/// Defines a link from one entity to another, or from one field on an entity back to itself.
	/// </summary>
	public abstract class EntityLink : EntityMember
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="nativeName"></param>
		protected EntityLink(string name, string nativeName) : base(name, nativeName)
		{
		}

		/// <summary>
		/// Gets the view property for this link.
		/// </summary>
		/// <returns></returns>
		public override EntityViewProperty GetViewProperty()
		{
			return new EntityLinkViewProperty(this);
		}
	}
}
