// BootFX - Application framework for .NET applications
// 
// File: MailProviderFactory.cs
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
	/// Summary description for MailProviderFactory.
	/// </summary>
	public sealed class MailProviderFactory
	{
		private MailProviderFactory()
		{
		}

		/// <summary>
		/// Gets the SMTP provider.
		/// </summary>
		/// <returns></returns>
		public static ISmtpProvider GetSmtpProvider()
		{
			return GetSmtpProvider(null);
		}

		/// <summary>
		/// Gets the SMTP provider.
		/// </summary>
		/// <returns></returns>
		public static ISmtpProvider GetSmtpProvider(SmtpConnectionSettings settings)
		{
            return new NetSmtpMailProvider(settings);
		}

		/// <summary>
		/// Gets the POP3 provider.
		/// </summary>
		/// <returns></returns>
		public static IPop3Provider GetPop3Provider()
		{
            throw new NotImplementedException("This operation has not been implemented.");
		}

		/// <summary>
		/// Configures and sends a message via SMTP.
		/// </summary>
		/// <param name="from"></param>
		/// <param name="to"></param>
		/// <param name="cc"></param>
		/// <param name="bcc"></param>
		/// <param name="subject"></param>
		/// <param name="body"></param>
		/// <param name="format"></param>
		public static void SendViaSmtp(string from, string toAsString, string ccAsString, string bccAsString, string subject, string body, MailFormat format,
			SmtpConnectionSettings settings)
		{
			if(settings == null)
				throw new ArgumentNullException("settings");
			
			// add...
			MailMessageRecipientCollection to = new MailMessageRecipientCollection();
			if(toAsString != null && toAsString.Length > 0)
				to.AddRange(toAsString);
			MailMessageRecipientCollection cc = new MailMessageRecipientCollection();
			if(ccAsString != null && ccAsString.Length > 0)
				cc.AddRange(ccAsString);
			MailMessageRecipientCollection bcc = new MailMessageRecipientCollection();
			if(bccAsString != null && bccAsString.Length > 0)
				bcc.AddRange(bccAsString);

			// defer...
			SendViaSmtp(new MailMessageRecipient(from), to, cc, bcc, subject, body, format, settings);
		}

		/// <summary>
		/// Configures and sends a message via SMTP.
		/// </summary>
		/// <param name="from"></param>
		/// <param name="to"></param>
		/// <param name="cc"></param>
		/// <param name="bcc"></param>
		/// <param name="subject"></param>
		/// <param name="body"></param>
		/// <param name="format"></param>
		public static void SendViaSmtp(string from, string toAsString, string ccAsString, string bccAsString, string subject, string body, MailFormat format,
			string hostName, string username, string password)
		{
			// add...
			MailMessageRecipientCollection to = new MailMessageRecipientCollection();
			if(toAsString != null && toAsString.Length > 0)
				to.AddRange(toAsString);
			MailMessageRecipientCollection cc = new MailMessageRecipientCollection();
			if(ccAsString != null && ccAsString.Length > 0)
				cc.AddRange(ccAsString);
			MailMessageRecipientCollection bcc = new MailMessageRecipientCollection();
			if(bccAsString != null && bccAsString.Length > 0)
				bcc.AddRange(bccAsString);

			// defer...
			SendViaSmtp(new MailMessageRecipient(from), to, cc, bcc, subject, body, format, hostName, username, password);
		}

		/// <summary>
		/// Configures and sends a message via SMTP.
		/// </summary>
		/// <param name="from"></param>
		/// <param name="to"></param>
		/// <param name="cc"></param>
		/// <param name="bcc"></param>
		/// <param name="subject"></param>
		/// <param name="body"></param>
		/// <param name="format"></param>
		public static void SendViaSmtp(MailMessageRecipient from, MailMessageRecipientCollection to, MailMessageRecipientCollection cc, MailMessageRecipientCollection bcc,
			string subject, string body, MailFormat format, SmtpConnectionSettings settings)
		{
			if(from == null)
				throw new ArgumentNullException("from");			
			
			// anyone?
			int total = 0;
			if(to != null)
				total += to.Count;
			if(cc != null)
				total += cc.Count;
			if(bcc != null)
				total += bcc.Count;

			// throw?
			if(total == 0)
				throw new InvalidOperationException("You must specify at least one person to send the message to.");

			// create...
			MailMessage message = new MailMessage();
			message.From = from;
			message.To.Clear();
			if(to != null)
				message.To.AddRange(to);
			message.Cc.Clear();
			if(cc != null)
				message.Cc.AddRange(cc);
			message.Bcc.Clear();
			if(bcc != null)
				message.Bcc.AddRange(bcc);
			message.Subject = subject;
			message.Body = body;
			message.MailFormat = format;

			// send...
			ISmtpProvider provider = GetSmtpProvider();
			if(provider == null)
				throw new InvalidOperationException("provider is null.");

			// set...
			provider.Settings = settings;

			// run...
			provider.Send(message);
		}

		/// <summary>
		/// Configures and sends a message via SMTP.
		/// </summary>
		/// <param name="from"></param>
		/// <param name="to"></param>
		/// <param name="cc"></param>
		/// <param name="bcc"></param>
		/// <param name="subject"></param>
		/// <param name="body"></param>
		/// <param name="format"></param>
		public static void SendViaSmtp(MailMessageRecipient from, MailMessageRecipientCollection to, MailMessageRecipientCollection cc, MailMessageRecipientCollection bcc,
			string subject, string body, MailFormat format, string hostName, string username, string password)
		{
			SendViaSmtp(from, to, cc, bcc, subject, body, format, new SmtpConnectionSettings(hostName, username, password));
		}
	}
}
