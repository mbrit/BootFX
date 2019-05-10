// BootFX - Application framework for .NET applications
// 
// File: IWorkUnitProcessor.cs
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
	/// Interface describing a work unit processor.
	/// </summary>
	internal interface IWorkUnitProcessor
	{
		/// <summary>
		/// Processes work units.
		/// </summary>
		/// <param name="units"></param>
		void Process(WorkUnitCollection units);
	}
}
