// BootFX - Application framework for .NET applications
// 
// File: SmtpConnectionSettings.cs
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
	/// Defines an instance of <c>SmtpConnectionSettings</c>.
	/// </summary>
	public class SmtpConnectionSettings : MailConnectionSettings
	{
        public bool EnableTls { get; private set; }

		public SmtpConnectionSettings()
		{
		}

		public SmtpConnectionSettings(string hostName, string username, string password)
			: base(hostName, username, password)
		{
		}

		public SmtpConnectionSettings(string hostName, string username, string password, int port, bool enableTls = false)
			: base(hostName, username, password, port)
		{
            this.EnableTls = enableTls;
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="connectionString"></param>
		public SmtpConnectionSettings(string connectionString) : base(connectionString)
		{
		}

		public static SmtpConnectionSettings GetDefaultSettings()
		{
			return new SmtpConnectionSettings("localhost", null, null);
		}

		public override int DefaultPort
		{
			get
			{
				return 25;
			}
		}

        /// <summary>
        /// Gets or sets whether to use the IIS delivery method.
        /// </summary>
        // mbr - 2009-02-28 - mbr added, jm diagnosed.
        public bool UseIisDeliveryMethod
        {
            get
            {
                return this.GetBooleanValue("UseIisDeliveryMethod", false, Cultures.System, OnNotFound.ReturnNull);
            }
            set
            {
                this.SetValue("UseIisDeliveryMethod", value);
            }
        }
	}
}
