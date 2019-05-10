// BootFX - Application framework for .NET applications
// 
// File: ISmtpProvider.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;

namespace BootFX.Common.Email
{
	/// <summary>
	/// Describes an interface for sending e-mail over an SMTP server.
	/// </summary>
	public interface ISmtpProvider
	{
		/// <summary>
		/// Gets or sets the connection settings.
		/// </summary>
		SmtpConnectionSettings Settings	
		{
			get;
			set;
		}

		/// <summary>
		/// Sends a message.
		/// </summary>
		/// <param name="message"></param>
		void Send(IMailMessage message);
	}
}
