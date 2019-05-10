// BootFX - Application framework for .NET applications
// 
// File: ConnectionSettingsListItem.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using BootFX.Common.Data;

namespace BootFX.Common.UI.Desktop
{
	/// <summary>
	/// List item for connection settings.
	/// </summary>
	internal class ConnectionSettingsListItem
	{
		/// <summary>
		/// Private field to support <c>Settings</c> property.
		/// </summary>
		private ConnectionSettings _settings;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="settings"></param>
		public ConnectionSettingsListItem(ConnectionSettings settings)
		{
			if(settings == null)
				throw new ArgumentNullException("settings");
			_settings = settings;
		}

		/// <summary>
		/// Gets the settings.
		/// </summary>
		public ConnectionSettings Settings
		{
			get
			{
				// returns the value...
				return _settings;
			}
		}

		public override string ToString()
		{
			if(Settings == null)
				throw new InvalidOperationException("Settings is null.");
			return this.Settings.ToString();
		}
	}
}
