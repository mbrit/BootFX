// BootFX - Application framework for .NET applications
// 
// File: WorkUnitProcessorTransactionManager.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Text;
using System.Collections;
using BootFX.Common;
using BootFX.Common.Management;

namespace BootFX.Common.Data
{
	/// <summary>
	/// Defines a class that can manage transactions within a work unit process.
	/// </summary>
	// mbr - 26-11-2007 - added.
    internal class WorkUnitProcessorTransactionManager : Loggable, IDisposable, IWorkUnitTransactionManager
	{
		/// <summary>
		/// Private field to support <see cref="NamedConnections"/> property.
		/// </summary>
		private Lookup _namedConnections;
		
		/// <summary>
		/// Private field to support <see cref="MainBound"/> property.
		/// </summary>
		private bool _mainBound = false;
		
		/// <summary>
		/// Private field to support <c>FailureExceptions</c> property.
		/// </summary>
		private ArrayList _failureExceptions = new ArrayList();
		
		/// <summary>
		/// Private field to support <see cref="CompletionState"/> property.
		/// </summary>
		private CompletionState _completionState = CompletionState.NotSet;
		
		/// <summary>
		/// Private field to support <see cref="Usage"/> property.
		/// </summary>
		private int _usage = 0;

		/// <summary>
		/// Private field to hold the singleton instance.
		/// </summary>
		[ThreadStatic()]
		private static WorkUnitProcessorTransactionManager _current = null;

		/// <summary>
		/// Constructor.
		/// </summary>
		private WorkUnitProcessorTransactionManager()
		{
			_namedConnections = new Lookup(false);
			_namedConnections.CreateItemValue += new CreateLookupItemEventHandler(_namedConnections_CreateItemValue);
		}
		
		/// <summary>
		/// Gets the singleton instance.
		/// </summary>
		internal static WorkUnitProcessorTransactionManager Current
		{
			get
			{
				if(_current == null)
					_current = new WorkUnitProcessorTransactionManager();
				return _current;
			}
		}

		/// <summary>
		/// Sets up the object.
		/// </summary>
		public void Initialize()
		{
			_usage++;

			// log...
            //if(this.Log.IsDebugEnabled)
            //{
            //    this.Log.Debug(string.Format("Transaction manager '{0}' initialized - usage is now {1}...", 
            //        this.GetHashCode(), this.Usage));
            //}
		}

		/// <summary>
		/// Bind the main database.
		/// </summary>
		private void StartMain()
		{
            //if(this.Log.IsDebugEnabled)
            //    this.Log.Debug("Binding main database...");

			// are we?
			if(this.MainBound)
				throw new InvalidOperationException("The main database has already been bound.");
			_mainBound = true;

			// setup...
			Database.BeginTransactionInternal();
		}

		/// <summary>
		/// Unbinds the main database.
		/// </summary>
		private void FinishMain(bool commit, Exception[] failureExceptions)
		{
			if(failureExceptions == null)
				throw new InvalidOperationException("failureExceptions is null.");

			// finished...
            //if(this.Log.IsDebugEnabled)
            //    this.Log.Debug(string.Format("Unbinding main database --> commit: {0}...", commit));

			// are we?
			if(!(this.MainBound))
				throw new InvalidOperationException("The main database has not been bound.");
			_mainBound = false;

			// now what...
			if(commit)
			{
				Database.CommitInternal();
			}
			else
			{
				Exception failureException = null;
				if(failureExceptions.Length == 1)
					failureException = failureExceptions[0];
				else
				{
					StringBuilder builder = new StringBuilder();
					builder.AppendFormat("'{0}' failure exception(s) occurred:", failureExceptions.Length);
					foreach(Exception ex in failureExceptions)
					{
						builder.Append("\r\n\t");
						builder.Append(ex);
					}

					// create...
					failureException = new InvalidOperationException(builder.ToString());
				}

				// rollback...
				Database.RollbackInternal(failureException);
			}
		}

		/// <summary>
		/// Finishes the named connections.
		/// </summary>
		/// <param name="ex"></param>
		// mbr - 09-12-2007 - now do named...
		private void FinishNamedConnections(bool commit)
		{
			try
			{
				// walk...
				foreach(IConnection conn in this.NamedConnections.Values)
				{
					// mbr - 05-05-2008 - had missed dispose...
					bool result = false;
					try
					{
						if(commit)
							result = conn.Commit();
						else
							result = conn.Rollback();
					}
					finally
					{
						if(result)
							conn.Dispose();
					}
				}
			}
			finally
			{
				// clear...
				this.NamedConnections.Clear();
			}
		}

		/// <summary>
		/// Gets the mainbound.
		/// </summary>
		private bool MainBound
		{
			get
			{
				return _mainBound;
			}
		}

		/// <summary>
		/// Gets the usage.
		/// </summary>
		private int Usage
		{
			get
			{
				return _usage;
			}
		}

		/// <summary>
		/// Disposes the object.
		/// </summary>
		public void Dispose()
		{
			this.FinishAll();

			// sup...
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Complete work in this instance.
		/// </summary>
		private void FinishAll()
		{
			// usage...
			_usage--;
			if(_usage < 0)
				throw new InvalidOperationException("Usage has dropped below zero.");

			// log...
            //if(this.Log.IsDebugEnabled)
            //{
            //    this.Log.Debug(string.Format("Transaction manager '{0}' disposed - usage is now {1}...", 
            //        this.GetHashCode(), this.Usage));
            //}

			// what now?
			if(_usage == 0)
			{
				// commit?
				bool commit = false;
				switch(CompletionState)
				{
					case CompletionState.NotSet:
						throw new InvalidOperationException("No 'Commit' or 'Rollback' calls were made.");

					case CompletionState.Commit:
						commit = true;
						break;

					case CompletionState.Rollback:
						commit = false;
						break;

					default:
						throw new NotSupportedException(string.Format("Cannot handle '{0}' ({1}).", CompletionState, CompletionState.GetType()));
				}

				// failures...
				Exception[] failureExceptions = (Exception[])this.FailureExceptions.ToArray(typeof(Exception));

				// do we need to unbind main?
				if(this.MainBound)
					this.FinishMain(commit, failureExceptions);

				// mbr - 09-12-2007 - now do named...
				this.FinishNamedConnections(commit);

				// reset...
				_current = null;
			}
		}

		/// <summary>
		/// Commits the transaction.
		/// </summary>
		public void Commit()
		{
			this.FinishOne(true, null);
		}

		/// <summary>
		/// Rolls back the transaction.
		/// </summary>
		public void Rollback(Exception ex)
		{
			this.FinishOne(false, ex);
		}

		/// <summary>
		/// Flags a finish.
		/// </summary>
		/// <param name="commit"></param>
		/// <param name="failureException"></param>
		private void FinishOne(bool commit, Exception failureException)
		{
            //if(this.Log.IsDebugEnabled)
            //    this.Log.Debug(string.Format("Finished with commit: {0}", commit));

			// what now?
			if(this.CompletionState == CompletionState.NotSet)
			{
				if(commit)
					_completionState = CompletionState.Commit;
				else
					_completionState = CompletionState.Rollback;
			}
			else
			{
				// if we commit, keep as commit if we were commit...
				if(commit)
				{
					// no-op... because we don't want to overwrite a rollback with a commit...
				}
				else
				{
					// if we're not committing, force to rollback...
					_completionState = CompletionState.Rollback;
				}
			}

			// state?
            //if(this.Log.IsDebugEnabled)
            //    this.Log.Debug(string.Format("\tCompletion state: {0}", this.CompletionState));

			// failure?
			if(!(commit) && failureException != null)
			{
				// set...
                //if(this.Log.IsDebugEnabled)
                //    this.Log.Debug(string.Format("\tAdding failure exception: {0} ({1})", failureException.Message, failureException.GetType()));

				// add...
				FailureExceptions.Add(failureException);
			}
		}

		/// <summary>
		/// Gets the failureexceptions.
		/// </summary>
		private ArrayList FailureExceptions
		{
			get
			{
				// returns the value...
				return _failureExceptions;
			}
		}

		/// <summary>
		/// Gets the completionstate.
		/// </summary>
		private CompletionState CompletionState
		{
			get
			{
				return _completionState;
			}
		}

		/// <summary>
		/// Gets a connection for a work unit.
		/// </summary>
		/// <param name="unit"></param>
		/// <returns></returns>
		public IConnection GetConnection(IWorkUnit unit)
		{
			if(unit == null)
				throw new ArgumentNullException("unit");

			// name...
			string name = unit.EntityType.DatabaseName;
			if(name == null || name.Length == 0)
			{
				// main?
				if(!(this.MainBound))
					this.StartMain();

				// return...
				return Database.CreateConnection(name);
			}
			else
			{
				return (IConnection)NamedConnections[name];
			}
		}

		/// <summary>
		/// Gets the namedconnections.
		/// </summary>
		private Lookup NamedConnections
		{
			get
			{
				return _namedConnections;
			}
		}

		private void _namedConnections_CreateItemValue(object sender, CreateLookupItemEventArgs e)
		{
			// create one...
			IConnection connection = Database.CreateConnection((string)e.Key);
			if(connection == null)
				throw new InvalidOperationException("connection is null.");

			// txn...
			connection.BeginTransaction();

			// set...
			e.NewValue = connection;
		}
    }
}
