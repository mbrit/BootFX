// BootFX - Application framework for .NET applications
// 
// File: OperationContext.cs
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

namespace BootFX.Common.Management
{
	/// <summary>
	/// Defines an instance of <c>OperationContext</c>.
	/// </summary>
	// mbr - 28-02-2008 - added implementation of IUserMessages.
	public class OperationContext : Loggable, IOperationItem, IUserMessages
	{
		private const string DefaultLogMessagePrefix = "[CONTEXT] ";

		/// <summary>
		/// Private field to support <see cref="IsBound"/> property.
		/// </summary>
		private bool _isBound = false;
		
		/// <summary>
		/// Private field to support <c>LogMessagePrefix</c> property.
		/// </summary>
		private string _logMessagePrefix = DefaultLogMessagePrefix;
		
		/// <summary>
		/// Private field to support <see cref="EchoLevel"/> property.
		/// </summary>
		private LogLevel _echoLevel = LogLevel.Warn;
		
		public event System.EventHandler Finished;

		public event System.EventHandler Cancelled;

		public event System.EventHandler Error;

		/// <summary>
		/// Private field to support <see cref="innerOperation"/> property.
		/// </summary>
		private IOperationItem _innerOperation;
		
		/// <summary>
		/// Private field to support <c>LogItems</c> property.
		/// </summary>
		private OperationLogItemCollection _logItems = new OperationLogItemCollection();
		
		/// <summary>
		/// Private field to support <see cref="InnerLog"/> property.
		/// </summary>
		private ILog _innerLog;

		private bool _defaultInnerOperationUsed;
		
		/// <summary>
		/// Creates a context.
		/// </summary>
		/// <param name="log"></param>
		public OperationContext(ILog log, IOperationItem innerOperation)
		{
			if(log == null)
				log = new NullLog();
			_innerLog = log;

			// check...
			if(innerOperation == null)
			{
				innerOperation = new OperationItem();
				_defaultInnerOperationUsed = true;
			}
			_innerOperation = innerOperation;

			// setup...
			_logItems.ItemAdded += new OperationLogItemEventHandler(_logItems_ItemAdded);
		}

		public bool DefaultInnerOperationUsed
		{
			get
			{
				return _defaultInnerOperationUsed;
			}
		}

		/// <summary>
		/// Binds the context to the current thread.
		/// </summary>
		public void Bind()
		{
			if(this.IsBound)
				throw new InvalidOperationException("Context is already bound.");

			if(InnerLog == null)
				throw new InvalidOperationException("InnerLog is null.");
			
			// bind, if we have a proper log...
			if(!(this.InnerLog is NullLog))
				LogSet.BindToContext(this.InnerLog);
			_isBound = true;
		}

		/// <summary>
		/// Unbinds the context from the current thread.
		/// </summary>
		public void Unbind()
		{
			if(InnerLog == null)
				throw new InvalidOperationException("InnerLog is null.");

			// unbind?
			if(this.IsBound)
			{
				_isBound = false;

				// unbind, if we have a proper log...
				if(!(this.InnerLog is NullLog))
					LogSet.UnbindFromContext(this.InnerLog);
			}
		}

		/// <summary>
		/// Gets the innerlog.
		/// </summary>
		public ILog InnerLog
		{
			get
			{
				return _innerLog;
			}
		}

		public void FatalUser(string message)
		{
			this.FatalUser(message, null);
		}

		public void FatalUser(string message, Exception ex)
		{
			// mbr - 28-02-2008 - changed to defer to format...
			this.LogItems.Add(LogLevel.Fatal, this.FormatMessage(LogLevel.Fatal, message, ex), ex);
		}

		public void DebugUser(string message)
		{
			this.DebugUser(message, null);
		}

		public void DebugUser(string message, Exception ex)
		{
			// mbr - 28-02-2008 - changed to defer to format...
			this.LogItems.Add(LogLevel.Debug, this.FormatMessage(LogLevel.Debug, message, ex), ex);
		}

		public void InfoUser(string message)
		{
			this.InfoUser(message, null);
		}

		public void InfoUser(string message, Exception ex)
		{
			// mbr - 28-02-2008 - changed to defer to format...
			this.LogItems.Add(LogLevel.Info, this.FormatMessage(LogLevel.Info, message, ex), ex);
		}

		public void ErrorUser(string message)
		{
			this.ErrorUser(message, null);
		}

		public void ErrorUser(string message, Exception ex)
		{
			// mbr - 28-02-2008 - changed to defer to format...
			this.LogItems.Add(LogLevel.Error, this.FormatMessage(LogLevel.Error, message, ex), ex);
		}

		public void WarnUser(string message)
		{
			this.WarnUser(message, null);
		}

		public void WarnUser(string message, Exception ex)
		{
			// mbr - 28-02-2008 - changed to defer to format...
//			this.LogItems.Add(LogLevel.Warn, message, ex);
			this.LogItems.Add(LogLevel.Warn, this.FormatMessage(LogLevel.Warn, message, ex), ex);
		}

		/// <summary>
		/// Provides an opportunity to alter the message based on other data in the context.
		/// </summary>
		/// <param name="level"></param>
		/// <param name="message"></param>
		/// <param name="ex"></param>
		/// <returns>Default operation is to return the value in the <c>message</c> argument.</returns>
		// mbr - 28-02-2008 - added.
		protected virtual string FormatMessage(LogLevel level, string message, Exception ex)
		{
			return message;
		}

		/// <summary>
		/// Gets the echolevel.
		/// </summary>
		public LogLevel EchoLevel
		{
			get
			{
				return _echoLevel;
			}
		}

		/// <summary>
		/// Gets a collection of OperationLogItem objects.
		/// </summary>
		public OperationLogItemCollection LogItems
		{
			get
			{
				return _logItems;
			}
		}

		/// <summary>
		/// Gets the inneroperation.
		/// </summary>
		public IOperationItem InnerOperation
		{
			get
			{
				return _innerOperation;
			}
			set
			{
				_innerOperation = value;
				_defaultInnerOperationUsed = false;
			}
		}

		public string Status
		{
			get
			{
				if(InnerOperation == null)
					throw new InvalidOperationException("InnerOperation is null.");
				return this.InnerOperation.Status;
			}
			set
			{
				if(InnerOperation == null)
					throw new InvalidOperationException("InnerOperation is null.");
				this.InnerOperation.Status = value;
			}
		}

		public int ProgressMaximum
		{
			get
			{
				if(InnerOperation == null)
					throw new InvalidOperationException("InnerOperation is null.");
				return this.InnerOperation.ProgressMaximum;
			}
			set
			{
				if(InnerOperation == null)
					throw new InvalidOperationException("InnerOperation is null.");
				this.InnerOperation.ProgressMaximum = value;
			}
		}

		public int ProgressMinimum
		{
			get
			{
				if(InnerOperation == null)
					throw new InvalidOperationException("InnerOperation is null.");
				return this.InnerOperation.ProgressMinimum;
			}
			set
			{
				if(InnerOperation == null)
					throw new InvalidOperationException("InnerOperation is null.");
				this.InnerOperation.ProgressMinimum = value;
			}
		}

		public int ProgressValue
		{
			get
			{
				if(InnerOperation == null)
					throw new InvalidOperationException("InnerOperation is null.");
				return this.InnerOperation.ProgressValue;
			}
			set
			{
				if(InnerOperation == null)
					throw new InvalidOperationException("InnerOperation is null.");
				this.InnerOperation.ProgressValue = value;
			}
		}

		public Exception LastError
		{
			get
			{
				if(InnerOperation == null)
					throw new InvalidOperationException("InnerOperation is null.");
				return this.InnerOperation.LastError;
			}
		}

		void IOperationItem.SetLastError(string status, Exception error)
		{
			if(InnerOperation == null)
				throw new InvalidOperationException("InnerOperation is null.");
			this.InnerOperation.SetLastError(status, error);
		}

		public void Reset()
		{
			if(InnerOperation == null)
				throw new InvalidOperationException("InnerOperation is null.");
			this.InnerOperation.Reset();
		}

		public DateTime LastUpdateTime
		{
			get
			{
				if(InnerOperation == null)
					throw new InvalidOperationException("InnerOperation is null.");
				return this.InnerOperation.LastUpdateTime;
			}
		}

		public void Cancel()
		{
			if(InnerOperation == null)
				throw new InvalidOperationException("InnerOperation is null.");
			this.InnerOperation.Cancel();
		}

		public bool IsCancelled
		{
			get
			{
				if(InnerOperation == null)
					throw new InvalidOperationException("InnerOperation is null.");
				return this.InnerOperation.IsCancelled;
			}
		}

		public void IncrementProgress()
		{
			if(InnerOperation == null)
				throw new InvalidOperationException("InnerOperation is null.");
			this.InnerOperation.IncrementProgress();
		}

		private void _innerOperation_Finished(object sender, EventArgs e)
		{
			this.OnFinished(e);
		}

		protected virtual void OnFinished(EventArgs e)
		{
			if(this.Finished != null)
				this.Finished(this, e);
		}

		private void _innerOperation_Cancelled(object sender, EventArgs e)
		{
			this.OnCancelled(e);
		}

		protected virtual void OnCancelled(EventArgs e)
		{
			if(this.Cancelled != null)
				this.Cancelled(this, e);
		}

		private void _innerOperation_Error(object sender, EventArgs e)
		{
			this.OnError(e);
		}

		protected virtual void OnError(EventArgs e)
		{
			if(this.Error != null)
				this.Error(this, e);
		}

		public OperationLogItemCollection GetItems()
		{
			return this.GetDebugItems(true);
		}

		public OperationLogItemCollection GetDebugItems(bool andAbove)
		{
			return this.LogItems.GetDebugItems(andAbove);
		}

		public OperationLogItemCollection GetInfoItems(bool andAbove)
		{
			return this.LogItems.GetInfoItems(andAbove);
		}

		public OperationLogItemCollection GetWarningItems(bool andAbove)
		{
			return this.LogItems.GetWarningItems(andAbove);
		}

		public OperationLogItemCollection GetErrorItems(bool andAbove)
		{
			return this.LogItems.GetErrorItems(andAbove);
		}

		public OperationLogItemCollection GetFatalItems(bool andAbove)
		{
			return this.LogItems.GetFatalItems(andAbove);
		}

		/// <summary>
		/// Gets or sets the logmessageprefix
		/// </summary>
		public string LogMessagePrefix
		{
			get
			{
				return _logMessagePrefix;
			}
			set
			{
				// check to see if the value has changed...
				if(value != _logMessagePrefix)
				{
					// set the value...
					_logMessagePrefix = value;
				}
			}
		}

		private void _logItems_ItemAdded(object sender, OperationLogItemEventArgs e)
		{
			// write it to the context bound log...
			BootFX.Common.Management.Log.LogMessage(this.Log, e.Item.Level, 
				string.Format("{0}{1}", this.LogMessagePrefix, e.Item.Message), e.Item.Exception);

			// log to the status...
			if((int)e.Item.Level >= (int)this.EchoLevel)
				this.Status = e.Item.ToString();

			// if we have an error or fatal, defer to SetLastError (this will raise an event)...
			if(e.Item.Level == LogLevel.Error || e.Item.Level == LogLevel.Fatal)
			{
				if(InnerOperation == null)
					throw new InvalidOperationException("InnerOperation is null.");
				this.InnerOperation.SetLastError(e.Item.Message, e.Item.Exception);
			}
		}

		/// <summary>
		/// Gets the isbound.
		/// </summary>
		internal bool IsBound
		{
			get
			{
				return _isBound;
			}
		}

		/// <summary>
		/// Returns true if a log is available on the object.
		/// </summary>
		public bool HasInnerLog
		{
			get
			{
				if(this.InnerLog == null || this.InnerLog is NullLog)
					return false;
				else
					return true;
			}
		}

		/// <summary>
		/// Gets user messages.
		/// </summary>
		/// <returns></returns>
		// mbr - 28-02-2008 - added implementation of IUserMessages.
		public string[] GetUserMessages()
		{
			return this.LogItems.GetUserMessages();
		}
	}
}
