// BootFX - Application framework for .NET applications
// 
// File: IDefaultDatabaseProvider.cs
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
	/// Summary description for IDefaultDatabaseProvider.
	/// </summary>
	// mbr - 05-02-2008 - added.
	public interface IDefaultDatabaseProvider
	{
		/// <summary>
		/// Creates the connection to use.
		/// </summary>
		/// <returns></returns>
		IConnection CreateConnection();

		/// <summary>
		/// Gets the current settings.
		/// </summary>
		/// <returns></returns>
		ConnectionSettings GetConnectionSettings();

		/// <summary>
		/// Sets the default settings.
		/// </summary>
		/// <param name="databaseType"></param>
		/// <param name="connectionString"></param>
		void SetConnectionSettings(Type connectionType, string connectionString);
	}
}
