// BootFX - Application framework for .NET applications
// 
// File: CommandFlags.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;

namespace BootFX.Common.UI
{
	/// <summary>
	/// Defines an instance of <c>CommandFlags</c>.
	/// </summary>
	[Flags()]
	public enum CommandFlags
	{
		Normal = 0,
		HasSeparator = 1,
		SingleOnly = 2
	}
}
