// BootFX - Application framework for .NET applications
// 
// File: SqlBuilder.cs
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
    public class SqlBuilder : ISqlStatementSource
    {
        private StringBuilder Builder { get; set; }
        private SqlStatement Sql { get; set; }

        public SqlBuilder()
        {
            this.Builder = new StringBuilder();
            this.Sql = new SqlStatement();
        }

        public SqlStatement GetStatement()
        {
            this.Sql.CommandText = this.Builder.ToString();
            return this.Sql;
        }
    }
}
