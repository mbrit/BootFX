// BootFX - Application framework for .NET applications
// 
// File: HttpResponseStatus.cs
// Build: 5.2.10321.2307
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
	/// Indicates what type of response was returned from a call.
	/// </summary>
	public enum HttpResponseStatus
	{
		/// <summary>
		/// An exception was returned, and no response was returned.
		/// </summary>
		NoResponse = 0,

		/// <summary>
		/// An exception was returned and the response was not an <c>HttpWebResponse</c>.
		/// </summary>
		ResponseIsNotHttp = 1,

		/// <summary>
		/// A response was returned an an HTTP status code was returned.
		/// </summary>
		ResponseOk = 2
	}
}
