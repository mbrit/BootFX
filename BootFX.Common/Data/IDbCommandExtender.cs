// BootFX - Application framework for .NET applications
// 
// File: IDbCommandExtender.cs
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
    public static class IDbCommandExtender
    {
        public static IDbDataParameter CreateParameter(this IDbCommand command, string name, DbType dbtype, object value = null)
        {
            if (value == null)
                value = DBNull.Value;

            var dialect = Database.DefaultDialect;

            var param = command.CreateParameter();
            param.ParameterName = dialect.FormatVariableNameForCommandParameter(command.CommandType, name);
            param.DbType = dbtype;
            param.Value = value;

            command.Parameters.Add(param);
            return param;
        }
    }
}
