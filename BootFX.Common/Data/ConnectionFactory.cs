// BootFX - Application framework for .NET applications
// 
// File: ConnectionFactory.cs
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
	/// Summary description for ConnectionFactory.
	/// </summary>
	public abstract class ConnectionFactory : IConnectionFactory
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		protected ConnectionFactory()
		{
		}

		/// <summary>
		/// Creates a connection for the given connection string.
		/// </summary>
		/// <param name="connectionString"></param>
		/// <returns></returns>
		public abstract IConnection CreateConnection(string connectionString);
	}
}
