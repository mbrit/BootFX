// BootFX - Application framework for .NET applications
// 
// File: ISqlStatementCallback.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Text;

namespace BootFX.Common.Data
{
    public interface ISqlStatementCallback
    {
        /// <summary>
        /// Called when a statement needs new constraints (for example, mangle a query to limit visible data).
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="creator"></param>
        /// <returns></returns>
        string ReplaceContraints(SqlStatement sql, SqlStatementCreator creator, string constraints);
    }
}
