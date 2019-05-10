// BootFX - Application framework for .NET applications
// 
// File: SqlStatementHash.cs
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
    public struct SqlStatementHash
    {
        public string CommandHash { get; private set; }
        public string ValueHash { get; private set; }

        internal SqlStatementHash(string commandHash, string valueHash)
            : this()
        {
            CommandHash = commandHash;
            ValueHash = valueHash; 
        }
    }
}
