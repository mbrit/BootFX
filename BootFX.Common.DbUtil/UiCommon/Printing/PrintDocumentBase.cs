// BootFX - Application framework for .NET applications
// 
// File: PrintDocumentBase.cs
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
using System.Drawing.Printing;
using System.Threading;
using System.Collections;
using BootFX.Common.Management;

namespace BootFX.Common.UI
{
	/// <summary>
	/// Defines an instance of <c>PrintDocumentBase</c>.
	/// </summary>
	public abstract class PrintDocumentBase : PrintDocument, ILoggable
	{
		/// <summary>
		/// Private field to support <c>Logs</c> property.
		/// </summary>
		private ILogSet _logs = null;
		
		/// <summary>
		/// Raised when there is a problem printing the page.
		/// </summary>
		public event ThreadExceptionEventHandler PrintPageError;
		
		protected PrintDocumentBase()
		{
		}

		protected override sealed void OnPrintPage(PrintPageEventArgs e)
		{
			base.OnPrintPage (e);
			try
			{
				this.DoPrintPage(e);
			}
			catch(Exception ex)
			{
				// stop has more pages...
				e.HasMorePages = false;

				// log...
				if(this.Log.IsErrorEnabled)
					this.Log.Error("An error occurred when printing the page.", ex);
				this.OnPrintPageError(new ThreadExceptionEventArgs(ex));
			}
		}

		protected abstract void DoPrintPage(PrintPageEventArgs e);

		/// <summary>
		/// Raises the <c>PrintPageError</c> event.
		/// </summary>
		protected virtual void OnPrintPageError(ThreadExceptionEventArgs e)
		{
			// raise...
			if(PrintPageError != null)
				PrintPageError(this, e);
		}

		ILog ILoggable.Log
		{
			get
			{
				return this.Log;
			}
		}

		/// <summary>
		/// Gets the default log.
		/// </summary>
		protected ILog Log
		{
			get
			{
				return this.Logs.DefaultLog;
			}
		}

		ILogSet ILoggable.Logs
		{
			get
			{
				return this.Logs;
			}
		}

		/// <summary>
		/// Gets the set of logs for other activities.
		/// </summary>
		protected ILogSet Logs
		{
			get
			{
				// mbr - 11-10-2005 - provided an ability to invalidate logs if the context changes...				
				if(_logs != null && _logs.ContextId != LogSet.CurrentContextId)
					_logs = null;
				if(_logs == null)
					_logs = new LogSet(this.GetType());
				return _logs;
			}
		}
	}
}
