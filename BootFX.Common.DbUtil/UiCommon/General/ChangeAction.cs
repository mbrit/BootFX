// BootFX - Application framework for .NET applications
// 
// File: ChangeAction.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;

namespace BootFX.Common.UI
{
	/// <summary>
	/// Defines different change actions.
	/// </summary>
	[Flags()]
	public enum ChangeAction
	{
		None = 0,
		Insert = 1,
		Update = 2,
		Delete = 4,
		All = Insert | Update | Delete,
	}
}
