// BootFX - Application framework for .NET applications
// 
// File: SystemSound.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;

namespace BootFX.Common.UI.Desktop
{
	/// <summary>
	/// Defines system sounds.
	/// </summary>
	public enum SystemSound
	{
		/// <summary>
		/// Windows system error sound.
		/// </summary>
		Error = 0,

		/// <summary>
		/// Windows system shutdown sound.
		/// </summary>
		SystemExit = 1,

		/// <summary>
		/// Windows system startup sound.
		/// </summary>
		SystemStart = 2,

		/// <summary>
		/// Default beep.
		/// </summary>
		Beep = 3,

		/// <summary>
		/// Windows warning sound.
		/// </summary>
		Warning = 4,

		/// <summary>
		/// Windows question sound.
		/// </summary>
		Question = 5
	}
}
