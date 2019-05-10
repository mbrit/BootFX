// BootFX - Application framework for .NET applications
// 
// File: SimpleXmlSaveMode.cs
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
	/// Summary description for SimpleXmlSaveMode.
	/// </summary>
	[Flags()]
	public enum SimpleXmlSaveMode
	{
		/// <summary>
		/// Clears all the child elements before starting.
		/// </summary>
		ClearAllChildren = 0,

		/// <summary>
		/// Replaces any node of the same name.
		/// </summary>
		ReplaceExisting = 1,

		/// <summary>
		/// Duplicates any existing value.
		/// </summary>
		Duplicate = 2,

		/// <summary>
		/// Uses attributes rather than elements.
		/// </summary>
		// mbr - 22-09-2006 - added.		
		UseAttributes = 4
	}
}
