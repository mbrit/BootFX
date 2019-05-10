// BootFX - Application framework for .NET applications
// 
// File: MemberTarget.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;

namespace BootFX.Common.CodeGeneration
{
	/// <summary>
	/// Defines method targets.
	/// </summary>
	// mbr - 06-09-2007 - c7 - made public.	
	[Flags]
	public enum MemberTarget
	{
		Concrete = 1,
		Stub = 2,
		Interface = 4,
		WebMethod = 8
	}
}
