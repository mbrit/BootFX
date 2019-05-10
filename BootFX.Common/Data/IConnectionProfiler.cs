// BootFX - Application framework for .NET applications
// 
// File: IConnectionProfiler.cs
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
using System.Data;

namespace BootFX.Common.Data
{
    public interface IConnectionProfiler
    {
        void Profile(SqlStatement sql, IDbCommand command, decimal duration, SqlMethod method, Exception ex);
    }
}
