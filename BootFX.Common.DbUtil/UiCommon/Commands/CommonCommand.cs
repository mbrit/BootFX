// BootFX - Application framework for .NET applications
// 
// File: CommonCommand.cs
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
	/// Defines an instance of <c>CommonCommand</c>.
	/// </summary>
	[Flags()]
	public enum CommonCommand
	{
		None = 0,
		Open = 1,
		Properties = 2,
		Delete = 4
	}
}
