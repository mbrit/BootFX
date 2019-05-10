// BootFX - Application framework for .NET applications
// 
// File: WorkUnitProcessorNullTransactionManager.cs
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
    // mbr - 2014-11-30 - the improvment here is this no longer creates transactions, if we're running
    // just one statement, assuming any higher level transaction will catch it...
    internal class WorkUnitProcessorNullTransactionManager : Loggable, IDisposable, IWorkUnitTransactionManager
    {
        private BfxLookup<string, IConnection> NamedConnections { get; set; }

        [ThreadStatic]
        private static WorkUnitProcessorNullTransactionManager _current;

        private const string DefaultKey = "*";

        private WorkUnitProcessorNullTransactionManager()
        {
            NamedConnections = new BfxLookup<string, IConnection>();
            NamedConnections.CreateItemValue += NamedConnections_CreateItemValue;
        }

        static WorkUnitProcessorNullTransactionManager()
        {
        }

        public static WorkUnitProcessorNullTransactionManager Current
        {
            get
            {
                if (_current == null)
                    _current = new WorkUnitProcessorNullTransactionManager();
                return _current;
            }
        }

        public void Initialize()
        {
        }

        private void NamedConnections_CreateItemValue(object sender, CreateLookupItemEventArgs<string, IConnection> e)
        {
            if (e.Key == DefaultKey)
                e.NewValue = Database.CreateConnection();
            else
                e.NewValue = Database.CreateConnection((string)e.Key);
        }

        public IConnection GetConnection(IWorkUnit unit)
        {
            string name = unit.EntityType.DatabaseName;
            if (name == null || name.Length == 0)
                return NamedConnections[DefaultKey];
            else
                return NamedConnections[name];
        }

        public void Commit()
        {
        }

        public void Rollback(Exception ex)
        {
        }

        public void Dispose()
        {
            try
            {
                foreach (var conn in this.NamedConnections.Values)
                {
                    try
                    {
                        conn.Dispose();
                    }
                    catch
                    {
                        // no-op...
                    }
                }
            }
            finally
            {
                this.NamedConnections.Clear();
            }
        }
    }
}
