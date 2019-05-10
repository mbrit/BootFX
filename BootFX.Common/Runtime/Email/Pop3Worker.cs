// BootFX - Application framework for .NET applications
// 
// File: Pop3Worker.cs
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
using BootFX.Common.Management;

namespace BootFX.Common.Email
{
	/// <summary>
	/// Provides a class that can connect to a POP3 account and download messages.
	/// </summary>
	public class Pop3Worker : Loggable
	{
		/// <summary>
		/// Raised when a message is received.
		/// </summary>
		public event MailMessageEventHandler MessageReceived;
		
		/// <summary>
		/// Constructor.
		/// </summary>
		public Pop3Worker()
		{
		}

		/// <summary>
		/// Processes the messages.
		/// </summary>
		/// <param name="settings"></param>
		public void ProcessAllMessages(Pop3ConnectionSettings settings)
		{
			if(settings == null)
				throw new ArgumentNullException("settings");

			// connect...
			using(IPop3Provider provider = MailProviderFactory.GetPop3Provider())
			{
				// connect...
				provider.Connect(settings);

				// mbr - 15-11-2005 - info...				
				if(this.Log.IsInfoEnabled)
					this.Log.Info(string.Format("POP3 account '{0}' on '{1}' has '{2}' message(s).", settings.Username, settings.HostName, provider.MessageCount));

				// get them...
				ArrayList done = new ArrayList();
				try
				{
					for(int index = 0; index < provider.MessageCount; index++)
					{
						// add...
						done.Add(index);

						// get it...
						MailMessage message = provider.GetMessage(index);
						if(message == null)
							throw new InvalidOperationException("message is null.");

						// defer it for processing...
						this.ProcessMessage(message);
					}
				}
				finally
				{
					foreach(int index in done)
						provider.DeleteMessage(index);
				}
			}
		}

		/// <summary>
		/// Processes the given message.
		/// </summary>
		/// <param name="message"></param>
		private void ProcessMessage(MailMessage message)
		{
			if(message == null)
				throw new ArgumentNullException("message");
			
			// raise...
			this.OnMessageReceived(new MailMessageEventArgs(message));
		}

		/// <summary>
		/// Raises the <c>MessageReceived</c> event.
		/// </summary>
		protected virtual void OnMessageReceived(MailMessageEventArgs e)
		{
			// raise...
			if(MessageReceived != null)
				MessageReceived(this, e);
		}
	}
}
