// BootFX - Application framework for .NET applications
// 
// File: RssEntity.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using BootFX.Common.Data.Rss;

namespace BootFX.Common.Entities
{
	/// <summary>
	/// Defines an entity that supports RSS.
	/// </summary>
	public abstract class RssEntity : Entity, IRssItem
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		protected RssEntity()
		{
		}

		/// <summary>
		/// Gets the RSS representation of the entity.
		/// </summary>
		/// <returns></returns>
		public abstract RssItem ToRssItem();
	}
}
