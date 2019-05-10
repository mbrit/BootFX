// BootFX - Application framework for .NET applications
// 
// File: IDatabaseExtensibilityProviderFactory.cs
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
	/// Defines the database extensibility provider factory to use.
	/// </summary>
	public interface IDatabaseExtensibilityProviderFactory
	{
		/// <summary>
		/// Gets the provider to use.
		/// </summary>
		/// <returns></returns>
		DatabaseExtensibilityProvider GetProvider();
	}
}
