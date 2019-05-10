// BootFX - Application framework for .NET applications
// 
// File: DisposeType.cs
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
	/// Defines disposable types.
	/// </summary>
	public enum DisposeType
	{
		/// <summary>
		/// Disposes was called explicitly.
		/// </summary>
		FromDispose = 0,

		/// <summary>
		/// Dispose was called as a last chance from the finalizer.
		/// </summary>
		FromFinalizer = 1
	}
}
