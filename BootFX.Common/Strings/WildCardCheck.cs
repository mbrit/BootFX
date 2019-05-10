// BootFX - Application framework for .NET applications
// 
// File: WildCardCheck.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Text.RegularExpressions;

namespace BootFX.Common.Strings
{
	/// <summary>
	/// Summary description for WildCardCheck.
	/// </summary>
	public class WildCardCheck
	{
		Regex regex;

		public WildCardCheck(string pattern)
		{
			pattern = "^" + Regex.Escape(pattern).
				Replace("\\*", ".*").
				Replace("\\?", ".") + "$";

			regex = new Regex(pattern, 
				System.Text.RegularExpressions.RegexOptions.IgnoreCase); 
		}

		public bool Check(string s)
		{
			return regex.IsMatch(s);
		}
	}
}
