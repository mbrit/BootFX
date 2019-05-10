// BootFX - Application framework for .NET applications
// 
// File: SqlDatabaseDefaultType.cs
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
	/// Defines an instance of <c>SqlDatabaseDefault</c>.
	/// </summary>
	public enum SqlDatabaseDefaultType
	{
		Literal = 0,
		Primitive = 1,
		CurrentDateTime = 2,
	}
}
