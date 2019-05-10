// BootFX - Application framework for .NET applications
// 
// File: RssEntityCollection.cs
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
	/// Describes an entity collection that holds RSS items.
	/// </summary>
	public abstract class RssEntityCollection : EntityCollection, IRssItemCollection
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="entityType"></param>
		protected RssEntityCollection(Type entityType) : this(EntityType.GetEntityType(entityType, OnNotFound.ThrowException)) 
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="entityType"></param>
		protected RssEntityCollection(EntityType entityType) : base(entityType)
		{
			if(entityType == null)
				throw new ArgumentNullException("entityType");
			
			// check...
			if(typeof(IRssItem).IsAssignableFrom(entityType.Type) == false)
				throw new InvalidOperationException(string.Format(Cultures.Exceptions, "Entity type '{0}' does not map to an type that supports IRssItem.", entityType));
		}

		/// <summary>
		/// Gets the collection as a collection of RSS items.
		/// </summary>
		/// <returns></returns>
		public RssItemCollection ToRssItemCollection()
		{
			RssItemCollection items = new RssItemCollection();
			foreach(object entity in this.InnerList)
				items.Add(((IRssItem)entity).ToRssItem());

			// return...
			return items;
		}
	}
}
