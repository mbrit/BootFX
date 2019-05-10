// BootFX - Application framework for .NET applications
// 
// File: OnStringNotFound.cs
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
	/// Defines actions used for missing strings.
	/// </summary>
	public enum OnStringNotFound
	{
		/// <summary>
		/// Indicates that <c>null</c> be returned if the string is missing.
		/// </summary>
		ReturnNull = 0,

		/// <summary>
		/// Indicates that an exception should be thrown if the string is missing.
		/// </summary>
		ThrowException = 1,

		/// <summary>
		/// Returns the name of the string that was requested.
		/// </summary>
		/// <remarks>The purpose of this is for exception handling.  If the string cannot be found, it's better to return the name
		/// of the string that's missing from the assembly rather than masking the original exception.  If a further exception occurs
		/// when this directive is specifies, the partial exception message will be wrapped in the result the exception published through 
		/// logging.</remarks>
		ReturnName = 2
	}
}
