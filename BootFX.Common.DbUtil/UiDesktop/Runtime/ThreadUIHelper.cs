// BootFX - Application framework for .NET applications
// 
// File: ThreadUIHelper.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Windows.Forms;
using System.Reflection;
using System.Threading;
using BootFX.Common;
using BootFX.Common.Management;

namespace BootFX.Common.UI.Desktop
{
	/// <summary>
	/// Summary description for ThreadUIHelper.
	/// </summary>
	public class ThreadUIHelper : Loggable
	{
		/// <summary>
		/// Raised when the event succeeds.
		/// </summary>
		public event ResultEventHandler Succeeded;
		
		[ThreadStatic()]
		private static ThreadUIHelper _current = null;

		/// <summary>
		/// Private field to support <c>BoundLog</c> property.
		/// </summary>
		private ILog _boundLog;
		
		/// <summary>
		/// Raised before the ThreadFinished event is raised on the UI thread.
		/// </summary>
		public event EventHandler BeforeMarshalThreadFinished;
		
		/// <summary>fthreadfin
		/// Raised after the ThreadStarted event is raised on the UI thread.
		/// </summary>
		public event EventHandler AfterMarshalThreadStarted;
		
		/// <summary>
		/// Raised when the thread is finished.
		/// </summary>
		public event EventHandler ThreadFinished;
		
		/// <summary>
		/// Raised when the thread is finished.
		/// </summary>
		public event EventHandler ThreadStarted;
		
		/// <summary>
		/// Raised when the operation fails.
		/// </summary>
		public event ThreadExceptionEventHandler Failed;

		/// <summary>
		/// Private field to support <see cref="Operation"/> property.
		/// </summary>
		private IOperationItem _operation;
		
		/// <summary>
		/// Private field to support <c>StartObject</c> property.
		/// </summary>
		private object _startObject;

		/// <summary>
		/// Private field to support <see cref="StartMethod"/> property.
		/// </summary>
		private string _startMethod;

		/// <summary>
		/// Private field to support <see cref="StartArgs"/> property.
		/// </summary>
		private object[] _startArgs;
		
		/// <summary>
		/// Private field to support <see cref="Thread"/> property.
		/// </summary>
		private Thread _thread;
		
		/// <summary>
		/// Private field to support <c>Owner</c> property.
		/// </summary>
		private Control _owner;
		
		public ThreadUIHelper(Control owner)
			: this(owner, null)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="owner"></param>
		public ThreadUIHelper(Control owner, IOperationItem operation)
		{
			if(owner == null)
				throw new ArgumentNullException("owner");
			_owner = owner;
			_operation = operation;
		}

		/// <summary>
		/// Gets the owner.
		/// </summary>
		private Control Owner
		{
			get
			{
				// returns the value...
				return _owner;
			}
		}

		/// <summary>
		/// Gets the operation.
		/// </summary>
		private IOperationItem Operation
		{
			get
			{
				return _operation;
			}
		}

		/// <summary>
		/// Runs the given method on the given object in a thread.
		/// </summary>
		/// <param name="target"></param>
		/// <param name="method"></param>
		/// <param name="args"></param>
		public void RunAsync(object target, string method, params object[] args)
		{
			if(target == null)
				throw new ArgumentNullException("target");
			if(method == null)
				throw new ArgumentNullException("method");
			if(method.Length == 0)
				throw new ArgumentOutOfRangeException("'method' is zero-length.");
			
			// run...
			if(_thread != null)
				throw new InvalidOperationException("An async operation is already running.");

			// set...
			_startObject = target;
			_startMethod = method;
			_startArgs = args;

			// create...
			_thread = new Thread(new ThreadStart(ThreadEntryPoint));
			_thread.Name = string.Format("{0}: {1}.{2}", this.ToString(), target.GetType(), method);
			_thread.IsBackground = true;
			_thread.Start();
		}

		/// <summary>
		/// Gets the startargs.
		/// </summary>
		private object[] StartArgs
		{
			get
			{
				return _startArgs;
			}
		}
		
		/// <summary>
		/// Gets the startmethod.
		/// </summary>
		private string StartMethod
		{
			get
			{
				return _startMethod;
			}
		}
		
		/// <summary>
		/// Gets the startobject.
		/// </summary>
		private object StartObject
		{
			get
			{
				// returns the value...
				return _startObject;
			}
		}

		/// <summary>
		/// Gets the thread.
		/// </summary>
		private Thread Thread
		{
			get
			{
				return _thread;
			}
		}

		private void ThreadEntryPoint()
		{
			try
			{
				// context...
				_current = this;

				// bound...
				if(this.BoundLog != null)
					LogSet.BindToContext(this.BoundLog);

				// events...
				this.OnThreadStarted(EventArgs.Empty);
				this.OnAfterMarshalThreadStarted();

				// check...
				if(StartObject == null)
					throw new InvalidOperationException("StartObject is null.");
				if(StartMethod == null)
					throw new InvalidOperationException("'StartMethod' is null.");
				if(StartMethod.Length == 0)
					throw new InvalidOperationException("'StartMethod' is zero-length.");

				// log...
				if(this.Log.IsInfoEnabled)
					this.Log.Info(string.Format("Running '{0}' on '{1}'...", this.StartMethod, this.StartObject.GetType()));

				// invoke...
				object result = this.StartObject.GetType().InvokeMember(this.StartMethod, BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.NonPublic, 
					null, this.StartObject, this.StartArgs);

				// finish...
				this.HandleSucceeded(result);
			}
			catch(Exception ex)
			{
				if(this.Log.IsErrorEnabled)
					this.Log.Error(string.Format("Failed to invoke '{0}' on '{1}'.", this.StartMethod, this.StartObject.GetType()), ex);

				// failed...
				this.HandleFailed(ex);
			}
			finally
			{
				// reset...
				_thread = null;

				// raise...
				this.OnBeforeMarshalThreadFinished();
				this.OnThreadFinished(EventArgs.Empty);

				// unbind?
				if(this.BoundLog != null)
					LogSet.UnbindFromContext(this.BoundLog);

				// reset... 
				_current = null;
			}
		}

		private void HandleSucceeded(object result)
		{
			// raise an event...
			this.OnSucceeded(new ResultEventArgs(result));
		}

		/// <summary>
		/// Raises the <c>Succeeded</c> event.
		/// </summary>
		protected virtual void OnSucceeded(ResultEventArgs e)
		{
			// raise...
			if(Succeeded != null)
				Succeeded(this, e);
		}

		private void HandleFailed(Exception ex)
		{
			if(ex == null)
				throw new ArgumentNullException("ex");
			
			// check...
			if(this.Operation != null)
				this.Operation.SetLastError("The operation failed.", ex);

			// raise an event...
			this.OnFailed(new ThreadExceptionEventArgs(ex));
		}

		private delegate void OnFailedDelegate(ThreadExceptionEventArgs e);

		/// <summary>
		/// Raises the <c>Failed</c> event.
		/// </summary>
		protected virtual void OnFailed(ThreadExceptionEventArgs e)
		{
			if(Owner == null)
				throw new InvalidOperationException("Owner is null.");
			if(this.Owner.InvokeRequired)
			{
				if(this.Log.IsInfoEnabled)
					this.Log.Info("Marshalling control to UI thread to raise 'Failed' event...");

				// raise...
				OnFailedDelegate d = new OnFailedDelegate(this.OnFailed);
				this.Owner.Invoke(d, new object[] { e });
				return;
			}

			// raise...
			if(Failed != null)
				Failed(this, e);
			else
				Alert.ShowWarning(this.Owner, "An error occurred whilst processing your request.", e.Exception);
		}

		public bool IsRunning
		{
			get
			{
				if(this.Thread != null)
					return true;
				else
					return false;
			}
		}

		private delegate void OnThreadStartedDelegate(EventArgs e);
		private delegate void OnThreadFinishedDelegate(EventArgs e);

		/// <summary>
		/// Raises the <c>ThreadFinished</c> event.
		/// </summary>
		protected virtual void OnThreadFinished(EventArgs e)
		{
			if(this.ThreadFinished == null)
				return;

			// flip...
			if(Owner == null)
				throw new InvalidOperationException("Owner is null.");
			if(this.Owner.InvokeRequired)
			{
				if(this.Log.IsInfoEnabled)
					this.Log.Info("Marshalling control to UI thread to raise 'ThreadFinished' event...");

				// raise...
				OnThreadFinishedDelegate d = new OnThreadFinishedDelegate(this.OnThreadFinished);
				this.Owner.Invoke(d, new object[] { e });
				return;
			}

			// raise...
			ThreadFinished(this, e);
		}

		/// <summary>
		/// Raises the <c>ThreadStarted</c> event.
		/// </summary>
		protected virtual void OnThreadStarted(EventArgs e)
		{
			// stop?
			if(this.ThreadStarted == null)
				return;

			if(Owner == null)
				throw new InvalidOperationException("Owner is null.");
			if(this.Owner.InvokeRequired)
			{
				if(this.Log.IsInfoEnabled)
					this.Log.Info("Marshalling control to UI thread to raise 'ThreadStarted' event...");

				// raise...
				OnThreadStartedDelegate d = new OnThreadStartedDelegate(this.OnThreadStarted);
				this.Owner.Invoke(d, new object[] { e });
				return;
			}

			// raise...
			ThreadStarted(this, e);
		}

		/// <summary>
		/// Raises the <c>AfterMarshalThreadStarted</c> event.
		/// </summary>
		private void OnAfterMarshalThreadStarted()
		{
			OnAfterMarshalThreadStarted(EventArgs.Empty);
		}
		
		/// <summary>
		/// Raises the <c>AfterMarshalThreadStarted</c> event.
		/// </summary>
		protected virtual void OnAfterMarshalThreadStarted(EventArgs e)
		{
			// raise...
			if(AfterMarshalThreadStarted != null)
				AfterMarshalThreadStarted(this, e);
		}

		/// <summary>
		/// Raises the <c>BeforeMarshalThreadFinished</c> event.
		/// </summary>
		private void OnBeforeMarshalThreadFinished()
		{
			OnBeforeMarshalThreadFinished(EventArgs.Empty);
		}
		
		/// <summary>
		/// Raises the <c>BeforeMarshalThreadFinished</c> event.
		/// </summary>
		protected virtual void OnBeforeMarshalThreadFinished(EventArgs e)
		{
			// raise...
			if(BeforeMarshalThreadFinished != null)
				BeforeMarshalThreadFinished(this, e);
		}

		/// <summary>
		/// Gets or sets the boundlog
		/// </summary>
		public ILog BoundLog
		{
			get
			{
				return _boundLog;
			}
			set
			{
				if(this.IsRunning)
					throw new InvalidOperationException("The property cannot be changed when the thread is running.");

				// check to see if the value has changed...
				if(value != _boundLog)
				{
					// set the value...
					_boundLog = value;
				}
			}
		}

		/// <summary>
		/// Gets the thread UI helper for the current thread.
		/// </summary>
		public static ThreadUIHelper Current
		{
			get
			{
				if(_current == null)
					throw new InvalidOperationException("This thread is not bound to a UI helper instance.");
				return _current;
			}
		}

		/// <summary>
		/// Shows the information box, marshalling control to the main thread.
		/// </summary>
		/// <param name="message"></param>
		/// <returns></returns>
		public DialogResult ShowInformation(string message)
		{
			return this.ShowAlert(message, null, MessageBoxIcon.Information, MessageBoxButtons.OK);
		}

		/// <summary>
		/// Shows the information box, marshalling control to the main thread.
		/// </summary>
		/// <param name="message"></param>
		/// <returns></returns>
		public DialogResult ShowWarning(string message)
		{
			return this.ShowWarning(message, null);
		}

		/// <summary>
		/// Shows the information box, marshalling control to the main thread.
		/// </summary>
		/// <param name="message"></param>
		/// <returns></returns>
		public DialogResult ShowWarning(string message, Exception ex)
		{
			return this.ShowAlert(message, ex, MessageBoxIcon.Warning, MessageBoxButtons.OK);
		}

		/// <summary>
		/// Shows the information box, marshalling control to the main thread.
		/// </summary>
		/// <param name="message"></param>
		/// <returns></returns>
		public DialogResult AskYesNoQuestion(string message)
		{
			return this.ShowAlert(message, null, MessageBoxIcon.Question, MessageBoxButtons.YesNo);
		}

		private delegate DialogResult ShowAlertDelegate(string message, Exception ex, MessageBoxIcon icon, MessageBoxButtons buttons);

		/// <summary>
		/// Shows the alert.
		/// </summary>
		/// <param name="message"></param>
		/// <param name="icon"></param>
		/// <returns></returns>
		private DialogResult ShowAlert(string message, Exception ex, MessageBoxIcon icon, MessageBoxButtons buttons)
		{
			// marshal?
			if(Owner == null)
				throw new InvalidOperationException("Owner is null.");
			if(this.Owner.InvokeRequired)
			{
				if(this.Log.IsInfoEnabled)
					this.Log.Info("Marshalling control to UI thread to display alert message...");

				// return...
				ShowAlertDelegate d = new ShowAlertDelegate(this.ShowAlert);
				return (DialogResult)this.Owner.Invoke(d, new object[] { message, ex, icon, buttons });
			}

			// log...
			if(this.Log.IsInfoEnabled)
			{
				string exceptionAsString = "(None)";
				if(ex != null)
					exceptionAsString = ex.ToString();
				this.Log.Info(string.Format("About to display message (icons: {0}, buttons: {1}).  Message:\r\n{2}\r\nException: {3}", icon,
					buttons, message, exceptionAsString));
			}

			// show...
			switch(icon)
			{
				case MessageBoxIcon.Information:
					return Alert.ShowInformation(this.Owner, message);

				case MessageBoxIcon.Warning:

					// show...
					if(ex == null)
						Alert.ShowWarning(this.Owner, message);
					else
						Alert.ShowWarning(this.Owner, message, ex);

					// return...
					return DialogResult.OK;

				case MessageBoxIcon.Question:
					return Alert.AskYesNoQuestion(this.Owner, message);

				default:
					throw new NotSupportedException(string.Format("Cannot handle '{0}' ({1}).", icon, icon.GetType()));
			}
		}

		public object Invoke(string methodName)
		{
			return this.Invoke(methodName, null);
		}

		private delegate object InvokeDelegate(object targetObject, string methodName, params object[] args);

		public object Invoke(object targetObject, string methodName, params object[] args)
		{
			if(methodName == null)
				throw new ArgumentNullException("methodName");
			if(methodName.Length == 0)
				throw new ArgumentOutOfRangeException("'methodName' is zero-length.");
			
			// do we need to invoke?
			if(Owner == null)
				throw new InvalidOperationException("Owner is null.");
			if(this.Owner.InvokeRequired)
			{
				InvokeDelegate d = new InvokeDelegate(this.Invoke);
				return this.Owner.Invoke(d, new object[] { targetObject, methodName, args });
			}

			// do something!
			Alert.ShowInformation(this.Owner, "Hello!");
			return null;
		}
	}
}
