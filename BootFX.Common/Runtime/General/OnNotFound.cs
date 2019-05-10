// BootFX - Application framework for .NET applications
// 
// File: OnNotFound.cs
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
	///	 Common Boolean-like enumeration that describes common actions for searches the result in missing objects.
	/// </summary>
	public enum OnNotFound
	{
		/// <summary>
		/// Indicates that the method will return null if the object is not found.
		/// </summary>
		ReturnNull = 0,

		/// <summary>
		/// Indicates that the method will throw an exception if the object is not found.
		/// </summary>
		ThrowException = 1
	}
}
