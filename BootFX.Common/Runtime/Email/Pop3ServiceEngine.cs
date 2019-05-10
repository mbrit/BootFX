// BootFX - Application framework for .NET applications
// 
// File: Pop3ServiceEngine.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Collections;
using BootFX.Common.Email;

namespace BootFX.Common.Services
{
	/// <summary>
	/// Summary description for Pop3ServiceEngine.
	/// </summary>
	public abstract class Pop3ServiceEngine : ThreadedServiceEngine
	{
		/// <summary>
		/// Private field to support <c>Worker</c> property.
		/// </summary>
		private Pop3Worker _worker;
		
		/// <summary>
		/// Private field to support <see cref="Settings"/> property.
		/// </summary>
		private Pop3ConnectionSettings _settings;
				
		public Pop3ServiceEngine(Pop3ConnectionSettings settings) : base(new TimeSpan(0,0,5))
		{
			if(settings == null)
				throw new ArgumentNullException("settings");
			_settings = settings;

			// worker...
			_worker = new Pop3Worker();
			_worker.MessageReceived += new MailMessageEventHandler(_worker_MessageReceived);
		}

		public Pop3ServiceEngine(string hostName, string username, string password) : this(new Pop3ConnectionSettings(hostName, username, password))
		{
		}

		/// <summary>
		/// Gets the settings.
		/// </summary>
		internal Pop3ConnectionSettings Settings
		{
			get
			{
				return _settings;
			}
		}

		protected sealed override bool DoWork()
		{
			if(Worker == null)
				throw new InvalidOperationException("Worker is null.");
			if(Settings == null)
				throw new InvalidOperationException("Settings is null.");
			
			// do the lot...
			this.Worker.ProcessAllMessages(this.Settings);

			// ok...
			return true;
		}

		/// <summary>
		/// Gets the worker.
		/// </summary>
		private Pop3Worker Worker
		{
			get
			{
				// returns the value...
				return _worker;
			}
		}

		/// <summary>
		/// Processes the given message.
		/// </summary>
		/// <param name="message"></param>
		protected abstract void ProcessMessage(MailMessage message);

		protected override void OnStarted(EventArgs e)
		{
			base.OnStarted (e);
			this.EnsureThreadInitialized();
		}

		private void _worker_MessageReceived(object sender, MailMessageEventArgs e)
		{
			// defer...
			this.ProcessMessage(e.Message);
		}
	}
}
