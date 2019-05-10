// BootFX - Application framework for .NET applications
// 
// File: ExecuteType.cs
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
	/// Defines an instance of <c>ExecuteType</c>.
	/// </summary>
	internal enum ExecuteType
	{
		Entity = 0,
		EntityCollection = 1,
		NonQuery = 2,
		Scalar = 3,
		DataTable = 4,
		DataSet = 5,
		Values = 6,
		ValuesVertical = 7
	}
}
