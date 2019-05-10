// BootFX - Application framework for .NET applications
// 
// File: IFormattableToString.cs
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
	/// Describes an interface that provides an alternative <c>ToString</c> that takes an <see cref="System.IFormatProvider"/> instance.
	/// </summary>
	public interface IFormattableToString
	{
		/// <summary>
		/// Gets the string representation of the object using the given format provider.
		/// </summary>
		/// <param name="formatProvider"></param>
		/// <returns></returns>
		string ToString(IFormatProvider formatProvider);
	}
}
