// BootFX - Application framework for .NET applications
// 
// File: Pop3ConnectionSettings.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;

namespace BootFX.Common.Email
{
	/// <summary>
	/// Defines an instance of <c>Pop3ConnectionSettings</c>.
	/// </summary>
	public class Pop3ConnectionSettings : MailConnectionSettings
	{
		public Pop3ConnectionSettings()
		{
		}

		public Pop3ConnectionSettings(string hostName, string username, string password) 
			: base(hostName, username, password)
		{
		}

		public Pop3ConnectionSettings(string hostName, string username, string password, int port) 
			: base(hostName, username, password, port)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="connectionString"></param>
		public Pop3ConnectionSettings(string connectionString) : base(connectionString)
		{
		}

		public override int DefaultPort
		{
			get
			{
				return 110;
			}
		}

	}
}
