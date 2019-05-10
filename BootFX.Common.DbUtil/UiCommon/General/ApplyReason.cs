// BootFX - Application framework for .NET applications
// 
// File: ApplyReason.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;

namespace BootFX.Common.UI.Common
{
	/// <summary>
	/// Defines 'apply' reasons for use with dialogs.
	/// </summary>
	public enum ApplyReason
	{
		/// <summary>
		/// The OK button was pressed.
		/// </summary>
		OKPressed = 0,

		/// <summary>
		/// The Apply button was pressed.
		/// </summary>
		ApplyPressed = 1,

		/// <summary>
		/// The <c>Apply</c> method was called directly.
		/// </summary>
		ManuallyInvoked = 2
	}
}
