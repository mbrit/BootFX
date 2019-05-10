// BootFX - Application framework for .NET applications
// 
// File: MissingItem.cs
// Build: 5.0.61009.900
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
	/// Describes an item missing from a property bag.
	/// </summary>
	[Serializable()]
	public struct MissingItem
	{
		/// <summary>
		/// Gets a singleton instance of <see cref="MissingItem"/>
		/// </summary>
		public static MissingItem Value = new MissingItem();
	}
}
