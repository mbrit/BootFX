// BootFX - Application framework for .NET applications
// 
// File: AuthenticateUserEventArgs.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;

namespace BootFX.Common.UI
{
	/// <summary>
	/// Delegate for use with <see cref="AuthenticateUserEventArgs"></see>.
	/// </summary>
	public delegate void AuthenticateUserEventHandler(object sender, AuthenticateUserEventArgs e);

	/// <summary>
	/// Summary description for ValidateLogonEventArgs.
	/// </summary>
	public class AuthenticateUserEventArgs
	{
		/// <summary>
		/// Private field to support <see cref="Username"/> property.
		/// </summary>
		private string _username;

		/// <summary>
		/// Private field to support <see cref="Password"/> property.
		/// </summary>
		private string _password;
		
		/// <summary>
		/// Private field to support <c>IsAuthenticated</c> property.
		/// </summary>
		private bool _isAuthenticated = false;
		
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="username"></param>
		/// <param name="rawPassword"></param>
		public AuthenticateUserEventArgs(string username, string password)
		{
			_username = username;
			_password = password;
		}

		/// <summary>
		/// Gets or sets the isAuthenticated
		/// </summary>
		public bool IsAuthenticated
		{
			get
			{
				return _isAuthenticated;
			}
			set
			{
				// check to see if the value has changed...
				if(value != _isAuthenticated)
				{
					// set the value...
					_isAuthenticated = value;
				}
			}
		}

		/// <summary>
		/// Gets the password.
		/// </summary>
		public string Password
		{
			get
			{
				return _password;
			}
		}
		
		/// <summary>
		/// Gets the username.
		/// </summary>
		public string Username
		{
			get
			{
				return _username;
			}
		}
	}
}
