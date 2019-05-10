// BootFX - Application framework for .NET applications
// 
// File: ConversionFlags.cs
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
	/// Defines conversion flags.
	/// </summary>
	[Flags()]
	public enum ConversionFlags
	{
		/// <summary>
		/// Default setting - no extra processing is performed.
		/// </summary>
		None = 0,

		/// <summary>
		/// Any values that are CLR null (i.e. "null") are converted to an equivalent value using <see cref="ConversionHelper.GetClrNullLegalEquivalent"></see>.
		/// </summary>
		ClrNullToLegal = 1,

		/// <summary>
		/// Any values that are DB null (i.e. <see cref="DBNull.Value"></see>) are converted to CLR null (i.e. "null").
		/// </summary>
		DBNullToClrNull = 2,

		/// <summary>
		/// Any values that are DB null (i.e. <see cref="DBNull.Value"></see>) are converted to an equivalent value using <see cref="ConversionHelper.GetClrNullLegalEquivalent"></see>.
		/// </summary>
		/// <remarks>This is a combination of <see cref="ClrNullToLegal"></see> and <see cref="DBNullToClrNull"></see>.</remarks>
		DBNullToLegal = ClrNullToLegal | DBNullToClrNull,

		/// <summary>
		/// Combines the action of <see cref="ClrNullToLegal"></see> and <see cref="DBNullToLegal"></see>.
		/// </summary>
		Safe = ClrNullToLegal | DBNullToLegal
	}
}
