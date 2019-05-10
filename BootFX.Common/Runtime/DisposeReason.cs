// BootFX - Application framework for .NET applications
// 
// File: DisposeReason.cs
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
	/// Defines object disposal reasons.
	/// </summary>
	public enum DisposeReason
	{
		/// <summary>
		/// Dispose was called explictly.
		/// </summary>
		ExplicitCall = 0,

		/// <summary>
		/// Dispose was called from the object's finalizer.
		/// </summary>
		FromFinalizer = 1
	}
}
