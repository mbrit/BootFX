// BootFX - Application framework for .NET applications
// 
// File: CompletionState.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;

namespace BootFX.Common.Data
{
	/// <summary>
	/// Summary description for CompletionState.
	/// </summary>
	internal enum CompletionState
	{
		NotSet = 0,
		Commit = 1,
		Rollback = 2
	}
}
