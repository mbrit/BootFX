// BootFX - Application framework for .NET applications
// 
// File: NamedDatabase.cs
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
	/// Describes a names database.
	/// </summary>
	public class NamedDatabase
	{
		/// <summary>
		/// Private field to support <see cref="Name"/> property.
		/// </summary>
		private string _name;

		/// <summary>
		/// Private field to support <see cref="Settings"/> property.
		/// </summary>
		private ConnectionSettings _settings;
		
		/// <summary>
		/// Constructor.
		/// </summary>
		public NamedDatabase(string name, ConnectionSettings settings)
		{
			if(name == null)
				throw new ArgumentNullException("name");
			if(name.Length == 0)
				throw new ArgumentOutOfRangeException("'name' is zero-length.");
			if(settings == null)
				throw new ArgumentNullException("settings");

			_name = name;
			_settings = settings;
		}

		/// <summary>
		/// Gets the settings.
		/// </summary>
		public ConnectionSettings Settings
		{
			get
			{
				return _settings;
			}
		}
		
		/// <summary>
		/// Gets the name.
		/// </summary>
		public string Name
		{
			get
			{
				return _name;
			}
		}
	}
}
