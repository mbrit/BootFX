// BootFX - Application framework for .NET applications
// 
// File: SafeCDataStrategy.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Xml;

namespace BootFX.Common
{
	/// <summary>
	/// Defines methods for handing CDATA in output.
	/// </summary>
	/// <remarks>Typically, when using <see cref="XmlTextWriter.WriteCData"></see>, if the string you are writing already contains an <c>]]&gt;</c> 'end CDATA marker', 
	/// the method will crash.  To do this safely, the best method is to base-64 encode the value, but this makes using XSLT on the output document much more difficult.  An
	/// alternative is just to replace the end marker with, say, a URL encoded version.</remarks>
	public enum SafeCDataStrategy
	{
		/// <summary>
		/// Base-64 encode the entire string if it contains an 'end CDATA' marker.
		/// </summary>
		Base64Encode = 0,

		/// <summary>
		/// URL encode (or otherwise replace) just the 'end CDATA' marker if one exists.
		/// </summary>
		ReplaceEndMarker = 1
	}
}
