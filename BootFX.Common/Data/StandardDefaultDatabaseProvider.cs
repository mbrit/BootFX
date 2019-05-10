// BootFX - Application framework for .NET applications
// 
// File: StandardDefaultDatabaseProvider.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.IO;

namespace BootFX.Common.Data
{
	/// <summary>
	/// Summary description for StandardDefaultDatabaseProvider.
	/// </summary>
	public class StandardDefaultDatabaseProvider : IDefaultDatabaseProvider
	{
		/// <summary>
		/// Private field to support <c>DatabaseType</c> property.
		/// </summary>
		private static Type _defaultConnectionType = null;

		/// <summary>
		/// private static field to support <c>DefaultConnectionString</c> property.
		/// </summary>
		private static string _defaultConnectionString = null;

		internal StandardDefaultDatabaseProvider()
		{
		}

		public IConnection CreateConnection()
		{
			// what type?
			if(DefaultConnectionType == null)
			{
				// check...
				string message = null;
				string path = Runtime.Current.InstallationSettingsFilePath;
				if(path == null || path.Length == 0)
					message = "The installation settings file path is not defined.";
				else
				{
					if(File.Exists(path))
						message = string.Format("An installation settings file path '{0}' DOES exist.", path);
					else
						message = string.Format("An installation settings file path '{0}' DOES NOT exist.", path);
				}

				// mbr - 05-02-2008 - changed to use message...
//				throw new InvalidOperationException(string.Format("'DefaultConnectionType' is null.  ({0})", path));
				throw new InvalidOperationException(string.Format("'DefaultConnectionType' is null.  ({0})", message));
			}

			// return...
			IConnection conn = Connection.CreateConnection(DefaultConnectionType, DefaultConnectionString);
			if(conn == null)
				throw new InvalidOperationException("conn is null.");

			// set the mode...
			conn.SqlMode = Database.DefaultSqlMode;	

			// return...
			return conn;
		}

		public ConnectionSettings GetConnectionSettings()
		{
			// do we have any?
			if(this.DefaultConnectionType != null && this.DefaultConnectionString != null && this.DefaultConnectionString.Length > 0)
				return new ConnectionSettings(null, this.DefaultConnectionType, this.DefaultConnectionString);
			else
				return null;
		}

		public void SetConnectionSettings(Type connectionType, string connectionString)
		{
			if(connectionType == null)
				throw new ArgumentNullException("connectionType");
			if(connectionString == null)
				throw new ArgumentNullException("connectionString");
			if(connectionString.Length == 0)
				throw new ArgumentOutOfRangeException("'connectionString' is zero-length.");
			
			// set...
			_defaultConnectionType = connectionType;
			_defaultConnectionString = connectionString;
		}

		private Type DefaultConnectionType
		{
			get
			{
				return _defaultConnectionType;
			}
		}

		private string DefaultConnectionString
		{
			get
			{
				return _defaultConnectionString;
			}
		}
	}
}
