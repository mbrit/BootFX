// BootFX - Application framework for .NET applications
// 
// File: ConnectionStringBuilder.cs
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
	/// Defines a class that can be used to build a connection string.
	/// </summary>
	public abstract class ConnectionStringBuilder
	{
		/// <summary>
		/// Creates a new instance of <see cref="ConnectionStringBuilder"/>.
		/// </summary>
		protected ConnectionStringBuilder()
		{
		}

		/// <summary>
		/// Gets a connection string with the given parameters.
		/// </summary>
		/// <param name="serverName"></param>
		/// <param name="databaseName"></param>
		/// <param name="useIntegratedSecurity"></param>
		/// <param name="username"></param>
		/// <param name="password"></param>
		/// <returns></returns>
		public abstract string GetConnectionString(string serverName, string databaseName, bool useIntegratedSecurity,
			string username, string password);

		/// <summary>
		/// Returns true if the database supports databases with individual names.
		/// </summary>
		public virtual bool SupportsNamedDatabases
		{
			get
			{
				return true;
			}
		}
	}
}
