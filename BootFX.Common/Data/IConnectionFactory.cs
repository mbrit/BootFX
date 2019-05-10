// BootFX - Application framework for .NET applications
// 
// File: IConnectionFactory.cs
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
	/// Defines an interface for obtaining connections.
	/// </summary>
	public interface IConnectionFactory
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="connectionString"></param>
		/// <returns></returns>
		IConnection CreateConnection(string connectionString);
	}
}
