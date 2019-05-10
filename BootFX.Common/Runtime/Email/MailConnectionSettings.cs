// BootFX - Application framework for .NET applications
// 
// File: MailConnectionSettings.cs
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
using BootFX.Common;

namespace BootFX.Common.Email
{
	/// <summary>
	/// Defines an instance of <c>MailConnectionSettings</c>.
	/// </summary>
	public abstract class MailConnectionSettings : NameValueStringPropertyBag
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		protected MailConnectionSettings()
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="bag"></param>
		protected MailConnectionSettings(string connectionString) : base(connectionString)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="hostName"></param>
		/// <param name="username"></param>
		/// <param name="password"></param>
		protected MailConnectionSettings(string hostName, string username, string password)
		{
			this.HostName = hostName;
			this.Username = username;
			this.Password = password;
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="hostName"></param>
		/// <param name="username"></param>
		/// <param name="password"></param>
		protected MailConnectionSettings(string hostName, string username, string password, int port)
			: this(hostName, username, password)
		{
			this.Port = port;
		}

		/// <summary>
		/// Gets or sets the password
		/// </summary>
		public string Password
		{
			get
			{
				return this.GetStringValue("Password", null, Cultures.System, OnNotFound.ReturnNull);
			}
			set
			{
				this.SetValue("Password", value);
			}
		}
		
		/// <summary>
		/// Gets or sets the Username
		/// </summary>
		public string Username
		{
			get
			{
				return this.GetStringValue("Username", null, Cultures.System, OnNotFound.ReturnNull);
			}
			set
			{
				this.SetValue("Username", value);
			}
		}
		
		/// <summary>
		/// Gets or sets the HostName
		/// </summary>
		public string HostName
		{
			get
			{
				return this.GetStringValue("HostName", null, Cultures.System, OnNotFound.ReturnNull);
			}
			set
			{
				this.SetValue("HostName", value);
			}
		}
		
		/// <summary>
		/// Gets or sets the HostName
		/// </summary>
		public int Port
		{
			get
			{
				return this.GetInt32Value("Port", DefaultPort, Cultures.System, OnNotFound.ReturnNull);
			}
			set
			{
				this.SetValue("Port", value);
			}
		}		

		/// <summary>
		/// Gets the default port.
		/// </summary>
		public abstract int DefaultPort
		{
			get;
		}

		public bool HasUsername
		{
			get
			{
				if(this.Username != null && this.Username.Length > 0)
					return true;
				else
					return false;
			}
		}
	}
}
