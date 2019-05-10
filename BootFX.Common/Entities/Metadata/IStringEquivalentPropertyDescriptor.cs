// BootFX - Application framework for .NET applications
// 
// File: IStringEquivalentPropertyDescriptor.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.ComponentModel;

namespace BootFX.Common.Entities
{
	/// <summary>
	/// Summary description for <see cref="IStringEquivalentPropertyDescriptor"/>.
	/// </summary>
	public interface IStringEquivalentPropertyDescriptor
	{
		/// <summary>
		/// Returns true if the descriptor has a string equivalent.
		/// </summary>
		bool HasStringEquivalent
		{
			get;
		}

		/// <summary>
		/// Gets a descriptor that returns a string equivalent.
		/// </summary>
		/// <returns></returns>
		PropertyDescriptor GetStringEquivalentDescriptor(string defaultFormatString);
	}
}
