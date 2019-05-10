// BootFX - Application framework for .NET applications
// 
// File: ElapsingServiceEngineFlags.cs
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
	/// Summary description for ElapsingServiceEngineFlags.
	/// </summary>
	public enum ElapsingServiceEngineFlags
	{
		Normal = 0,

		/// <summary>
		/// Times are based on local times, not UTC time.  
		/// </summary>
		/// <remarks>Be careful this option.  It is for customer requirements whereby something needs to 
		/// happen at the same time each day regardless of BST.</remarks>
		UseLocalTime = 1
	}
}
