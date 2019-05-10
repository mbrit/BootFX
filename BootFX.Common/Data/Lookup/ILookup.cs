// BootFX - Application framework for .NET applications
// 
// File: ILookup.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;

namespace BootFX.Common
{
	/// <summary>
	/// Interface for lookup classes.
	/// </summary>
	public interface ILookup
	{
		/// <summary>
		/// Gets an item.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		object this[object key]
		{
			get;
		}

		/// <summary>
		/// Gets the number of items.
		/// </summary>
		int Count
		{
			get;
		}

		/// <summary>
		/// Clears the contents.
		/// </summary>
		void Clear();

		/// <summary>
		/// Defines an event for item creation.
		/// </summary>
		event CreateLookupItemEventHandler CreateItemValue;
	}
}
