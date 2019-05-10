// BootFX - Application framework for .NET applications
// 
// File: EvaluateFlags.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;

namespace BootFX.Common
{
	/// <summary>
	/// Summary description for EvaluateFlags.
	/// </summary>
	public enum EvaluateFlags
	{
		Normal = 0,
		ConvertResultToString = 1,

        /// <summary>
        /// Ignores values that are missing.
        /// </summary>
        // mbr - 2009-04-24 - added.
        IgnoreMissingValues = 2
	}
}
