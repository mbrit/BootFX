// BootFX - Application framework for .NET applications
// 
// File: ConnectionAttribute.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;

namespace BootFX.Common.Data
{
	/// <summary>
	/// Marks a class as being a connection type.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class ConnectionAttribute : Attribute
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		public ConnectionAttribute()
		{
		}
	}
}
