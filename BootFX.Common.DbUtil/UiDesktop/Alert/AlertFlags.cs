// BootFX - Application framework for .NET applications
// 
// File: AlertFlags.cs
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
	/// Summary description for AlertFlags.
	/// </summary>
	[Flags()]
	public enum AlertFlags
	{
		Normal = 0,
		SuppressErrorReport = 1
	}
}
