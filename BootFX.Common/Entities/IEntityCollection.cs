// BootFX - Application framework for .NET applications
// 
// File: IEntityCollection.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Collections;

namespace BootFX.Common.Entities
{
	/// <summary>
	///	 Defines an interface common to all entity collections.
	/// </summary>
	public interface IEntityCollection : IList, IEntityType
	{ 
		/// <summary>
		/// Returns true if the collection is a strong collection.
		/// </summary>
		bool IsStrongCollection
		{
			get;
		}

		/// <summary>
		/// Gets the items in the collection as an array.
		/// </summary>
		/// <returns></returns>
		object[] ToArray();
	}
}
