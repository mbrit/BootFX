// BootFX - Application framework for .NET applications
// 
// File: IRssItem.cs
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
	/// Defines an interface that indicates an object is capable at transforming itself to an <see cref="RssItem"></see>.
	/// </summary>
	public interface IRssItem
	{
		/// <summary>
		/// Gets the RSS representation of the object.
		/// </summary>
		/// <returns></returns>
		RssItem ToRssItem();
	}
}
