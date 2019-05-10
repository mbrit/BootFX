// BootFX - Application framework for .NET applications
// 
// File: ServiceControllerAction.cs
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

namespace BootFX.Common
{
	/// <summary>
	/// Defines an instance of <c>ServiceControllerAction</c>.
	/// </summary>
	public enum ServiceControllerAction
	{
		Interactive = 0,
		Start = 1,
		Stop = 2,
		Pause = 3,
		Restart = 4
	}
}
