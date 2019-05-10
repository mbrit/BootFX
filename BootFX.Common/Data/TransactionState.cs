// BootFX - Application framework for .NET applications
// 
// File: TransactionState.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Data;
using System.Reflection;
using System.Collections.Generic;
using System.Text;
using BootFX.Common.Management;

namespace BootFX.Common.Data
{
    public class TransactionState : Loggable, IDisposable
    {
        /// <summary>
        /// Private value to support the <see cref="CommitCalled">CommitCalled</see> property.
        /// </summary>
        private bool _commitCalled;

        /// <summary>
        /// Private value to support the <see cref="RollbackCalled">RollbackCalled</see> property.
        /// </summary>
        private bool _rollbackCalled;

        private string CallStack { get; set; }

        internal TransactionState()
        {
            this.InitializeCallStack();
            try
            {
                Database.BeginTransactionInternal();
            }
            catch(Exception ex)
            {
                HandleSetupFailure(ex);
            }
        }

        internal TransactionState(IsolationLevel level)
        {
            this.InitializeCallStack();
            try
            {
                Database.BeginTransactionInternal(level);
            }
            catch (Exception ex)
            {
                HandleSetupFailure(ex);
            }
        }

        ~TransactionState()
        {
            throw new BadTransactionException("Finalizer was called on transaction state.  Check that all results from CreateTransaction are wrapped in a 'using' construct.");
        }

        private void HandleSetupFailure(Exception ex)
        {
            if (this.Log.IsErrorEnabled)
                this.Log.Error("Transaction setup failed.", ex);

            // suppress the GC, otherwise we'll end up in the finaliser exception...
            GC.SuppressFinalize(this);
        }

        private void InitializeCallStack()
        {
            // mbr - 2011-10-27 - removed...
            //this.CallStack = Environment.StackTrace;
        }

        public void Dispose()
        {
            try
            {
                if (!(this.CommitCalled) && !(this.RollbackCalled))
                {
                    Database.RollbackInternal(null);
                    throw new BadTransactionException("The transaction was not explicitly closed.  A rollback has been forced.");
                }
            }
            finally
            {
                GC.SuppressFinalize(this);
            }
        }

        public void Commit()
        {
            // have we?
            if (this.RollbackCalled)
                throw new InvalidOperationException("Rollback has already been called.");

            _commitCalled = true;
            Database.CommitInternal();
        }

        public void Rollback(Exception ex)
        {
            // have we?
            if (this.CommitCalled)
                throw new InvalidOperationException("Commit has already been called.");

            _rollbackCalled = true;
            Database.RollbackInternal(ex);
        }

        /// <summary>
        /// Gets the RollbackCalled value.
        /// </summary>
        private bool RollbackCalled
        {
            get
            {
                return _rollbackCalled;
            }
        }

        /// <summary>
        /// Gets the CommitCalled value.
        /// </summary>
        private bool CommitCalled
        {
            get
            {
                return _commitCalled;
            }
        }

        public IDbCommand CreateNativeCommand()
        {
            var conn = Database.CreateConnection();
            return ((Connection)conn).CreateCommand();
        }
    }
}
