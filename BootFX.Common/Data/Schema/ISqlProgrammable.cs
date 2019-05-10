// BootFX - Application framework for .NET applications
// 
// File: ISqlProgrammable.cs
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

namespace BootFX.Common.Data.Schema
{
	/// <summary>
	/// Defines a class that can be programmed against.
	/// </summary>
	public interface ISqlProgrammable
	{
		string Name
		{
			get;
		}

		string NativeName
		{
			get;
		}

		bool Generate
		{
			get;
		}
	}
}
