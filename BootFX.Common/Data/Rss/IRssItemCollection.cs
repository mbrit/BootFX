// BootFX - Application framework for .NET applications
// 
// File: IRssItemCollection.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;

namespace BootFX.Common.Data.Rss
{
	/// <summary>
	/// Defines an interface for a collection that can transform itself into an <see cref="RssItemCollection"></see>.
	/// </summary>
	public interface IRssItemCollection
	{
		/// <summary>
		/// Gets the collection as a collection of RSS items.
		/// </summary>
		/// <returns></returns>
		RssItemCollection ToRssItemCollection();
	}
}
