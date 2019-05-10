// BootFX - Application framework for .NET applications
// 
// File: CommandCreatedEventArgs.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BootFX.Common.Data
{
    public class CommandCreatedEventArgs : EventArgs
    {
        public IConnection Connection { get; private set; }
        public SqlStatement Statement { get; set; }
        private IDbCommand _command;
        public bool CommandChanged { get; set; }

        internal CommandCreatedEventArgs(IConnection connection, SqlStatement statement, IDbCommand command)
        {
            this.Connection = connection;
            this.Statement = statement;
            this.Command = command;
        }

        public IDbCommand Command
        {
            get
            {
                return _command;
            }
            set
            {
                _command = value;
                this.CommandChanged = true;
            }
        }
    }
}
