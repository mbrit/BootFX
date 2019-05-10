// BootFX - Application framework for .NET applications
// 
// File: OutsideTransactionWorkUnitProcessorTransactionManager.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BootFX.Common.Data
{
    internal class OutsideTransactionWorkUnitProcessorTransactionManager : IWorkUnitTransactionManager
    {
        private IConnection Connection { get; set; }

        public void Initialize()
        {
            this.Connection = Database.CreateNewConnection();
            this.Connection.BeginTransaction();
        }

        public IConnection GetConnection(IWorkUnit workUnit)
        {
            if (workUnit.EntityType.HasDatabaseName)
                throw new InvalidOperationException("Custom databases are not supported with this transaction manager.");
            return this.Connection;
        }

        public void Commit()
        {
            this.Connection.Commit();
        }

        public void Rollback(Exception ex)
        {
            this.Connection.Rollback();
        }

        public void Dispose()
        {
            if (this.Connection != null)
            {
                try
                {
                    this.Connection.Dispose();
                }
                finally
                {
                    this.Connection = null;
                }
            }
        }
    }
}
