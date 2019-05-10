// BootFX - Application framework for .NET applications
// 
// File: LineBasedDataWriterFlags.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;

namespace BootFX.Common.Data.Text
{
	/// <summary>
	/// Defines an instance of <c>LineBasedDataWriterFlags</c>.
	/// </summary>
	[Flags()]
	public enum LineBasedDataWriterFlags
	{
		Normal = 0,
		ExcludeHeader = 1
	}
}
