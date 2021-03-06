// BootFX - Application framework for .NET applications
// 
// File: IExistenceChecker.cs
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
using System.Threading.Tasks;

namespace BootFX.Common.Data
{
    public interface IExistenceChecker
    {
        bool DoesTableExist(Connection connection, NativeName nativeName);
        bool DoesColumnExist(Connection connection, string tableName, string columnName);
        bool DoesIndexExist(Connection connection, string tableName, string indexName);
    }
}
