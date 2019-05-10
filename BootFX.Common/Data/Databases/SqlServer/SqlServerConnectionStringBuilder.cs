// BootFX - Application framework for .NET applications
// 
// File: SqlServerConnectionStringBuilder.cs
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
	/// Summary description for <see cref="MySqlConnectionStringBuilder"/>.
	/// </summary>
	public class SqlServerConnectionStringBuilder : ConnectionStringBuilder
	{
		/// <summary>
		/// Creates a new instance of <see cref="MySqlConnectionStringBuilder"/>.
		/// </summary>
		public SqlServerConnectionStringBuilder()
		{
		}

		/// <summary>
		/// Gets the connection string.
		/// </summary>
		/// <param name="serverName"></param>
		/// <param name="databaseName"></param>
		/// <param name="useIntegratedSecurity"></param>
		/// <param name="username"></param>
		/// <param name="password"></param>
		/// <returns></returns>
		public override string GetConnectionString(string serverName, string databaseName, bool useIntegratedSecurity, string username, string password)
		{
			if(serverName == null)
				throw new ArgumentNullException("serverName");
			if(serverName.Length == 0)
				throw new ArgumentOutOfRangeException("'serverName' is zero-length.");
			if(databaseName == null)
				throw new ArgumentNullException("databaseName");
			if(databaseName.Length == 0)
				throw new ArgumentOutOfRangeException("'databaseName' is zero-length.");

			// integrated?
			if(useIntegratedSecurity)
				return string.Format("integrated security=sspi;data source={0};initial catalog={1}", serverName, databaseName);
			else
			{
				if(username == null)
					throw new InvalidOperationException("'username' is null.");
				if(username.Length == 0)
					throw new InvalidOperationException("'username' is zero-length.");

				// return...
				return string.Format("data source={0};initial catalog={1};uid={2};pwd={3}", serverName, databaseName, username, password);
			}
		}
	}
}
