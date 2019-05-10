// BootFX - Application framework for .NET applications
// 
// File: XdrType.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;

namespace BootFX.Common.Xml
{
	/// <summary>
	/// Summary description for XdrType.
	/// </summary>
	public enum XdrType
	{
		String = 0,
		DateTime = 1,
		Boolean = 2,
		Int16 = 3,
		Int32 = 4,
		Int64 = 5,
		Guid = 6,
		DateTimeTz = 7,
		BinBase64 = 8,

		// mv...
		MvString = 9,
		MvDateTimeTz = 10,

		// mbr - 21-09-2006 - added.
		Float = 11,
		Double = 11,

		// mbr - 09-03-2007 - added.		
		Byte = 12,
		Single = 13
	}
}
