// BootFX - Application framework for .NET applications
// 
// File: DatabaseUpdateThread.cs
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
using BootFX.Common;
using BootFX.Common.Data;

namespace BootFX.Common.UI.Desktop
{
	/// <summary>
	/// Defines an instance of <c>DatabaseUpdateThread</c>.
	/// </summary>
	internal class DatabaseUpdateUIWorker
	{
		/// <summary>
		/// Raised when the update has been finished.
		/// </summary>
		public event EventHandler UpdateComplete;
		
		/// <summary>
		/// Raised when a check operation is complete.
		/// </summary>
		public event DatabaseUpdateCheckResultsEventHandler CheckComplete;

		/// <summary>
		/// Private field to support <see cref="Operation"/> property.
		/// </summary>
		private IOperationItem _operation;
		
		/// <summary>
		/// Private field to support <see cref="Settings"/> property.
		/// </summary>
		private ConnectionSettings _settings;

		/// <summary>
		/// Private field to support <see cref="Type"/> property.
		/// </summary>
		private DatabaseUpdateOperation _type;
		
		internal DatabaseUpdateUIWorker(ConnectionSettings settings, DatabaseUpdateOperation type, IOperationItem operation) 
		{
			if(settings == null)
				throw new ArgumentNullException("settings");
			
			// set...
			_settings = settings;
			_type = type;
			_operation = operation;
		}

		internal void DoWork()
		{
			if(Operation == null)
				throw new InvalidOperationException("Operation is null.");

			// run...
			DatabaseUpdate update = DatabaseUpdate.Current;
			if(update == null)
				throw new InvalidOperationException("update is null.");

			// mbr - 28-09-2007 - added args...
			DatabaseUpdateArgs args = new DatabaseUpdateArgs();
			switch(Type)
			{
				case DatabaseUpdateOperation.Check:
					DatabaseUpdateCheckResults results = update.Check(this.Operation, args);
					if(results == null)
						throw new InvalidOperationException("results is null.");
					this.OnCheckComplete(new DatabaseUpdateCheckResultsEventArgs(results));
					break;

				case DatabaseUpdateOperation.Update:
					update.Update(this.Operation, args	);
					this.OnUpdateComplete();
					break;

				default:
					throw new NotSupportedException(string.Format("Cannot handle '{0}' ({1}).", Type, Type.GetType()));
			}
		}

		/// <summary>
		/// Gets the operation.
		/// </summary>
		internal DatabaseUpdateOperation Type
		{
			get
			{
				return _type;
			}
		}
		
		/// <summary>
		/// Gets the settings.
		/// </summary>
		internal ConnectionSettings Settings
		{
			get
			{
				return _settings;
			}
		}

		/// <summary>
		/// Gets the operation.
		/// </summary>
		internal IOperationItem Operation
		{
			get
			{
				return _operation;
			}
		}

		/// <summary>
		/// Raises the <c>CheckComplete</c> event.
		/// </summary>
		protected virtual void OnCheckComplete(DatabaseUpdateCheckResultsEventArgs e)
		{
			// raise...
			if(CheckComplete != null)
				CheckComplete(this, e);
		}

		/// <summary>
		/// Raises the <c>UpdateComplete</c> event.
		/// </summary>
		private void OnUpdateComplete()
		{
			OnUpdateComplete(EventArgs.Empty);
		}
		
		/// <summary>
		/// Raises the <c>UpdateComplete</c> event.
		/// </summary>
		protected virtual void OnUpdateComplete(EventArgs e)
		{
			// raise...
			if(UpdateComplete != null)
				UpdateComplete(this, e);
		}
	}
}
