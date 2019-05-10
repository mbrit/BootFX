// BootFX - Application framework for .NET applications
// 
// File: MailMessageEventArgs.cs
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
	public delegate void MailMessageEventHandler(object sender, MailMessageEventArgs e);

	/// <summary>
	/// Defines an instance of <c>MailMessageEventArgs</c>.
	/// </summary>
	public class MailMessageEventArgs : EventArgs
	{
		/// <summary>
		/// Private field to support <see cref="Message"/> property.
		/// </summary>
		private MailMessage _message;
		
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="message"></param>
		internal MailMessageEventArgs(MailMessage message)
		{
			if(message == null)
				throw new ArgumentNullException("message");
			_message = message;
		}

		/// <summary>
		/// Gets the message.
		/// </summary>
		public MailMessage Message
		{
			get
			{
				return _message;
			}
		}
	}
}
