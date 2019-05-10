// BootFX - Application framework for .NET applications
// 
// File: SqlMode.cs
// Build: 5.2.10321.2307
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

namespace BootFX.Common.Data
{
	/// <summary>
	/// Defines whether statements should be created adhoc or whether stored procedures should be used where appropriate.
	/// </summary>
	public enum SqlMode
	{
		AdHoc = 0,

		// mbr - 10-10-2007 - removed...		
//		Sprocs = 1
	}
}
