// BootFX - Application framework for .NET applications
// 
// File: SkipDatabaseUpdateAttribute.cs
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
	/// Defines an instance of <c>SkipDatabaseUpdateAttribute</c>.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class SkipDatabaseUpdateAttribute : Attribute
	{
		public SkipDatabaseUpdateAttribute()
		{
		}
	}
}
