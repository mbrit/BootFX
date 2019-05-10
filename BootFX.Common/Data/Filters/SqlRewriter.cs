// BootFX - Application framework for .NET applications
// 
// File: SqlRewriter.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using BootFX.Common.Data.Formatters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BootFX.Common.Data.Formatters
{
    public abstract class SqlRewriter
    {
        private static Dictionary<Type, SqlRewriter> Rewriters { get; set; }
        private static bool HasRewriters { get; set; }
        //private static SqlRewriter NullRewriter;

        static SqlRewriter()
        {
            Rewriters = new Dictionary<Type, SqlRewriter>();
        }

        public static void SetRewriter<T>(SqlRewriter rewriter)
            where T : SqlDialect
        {
            Rewriters[typeof(T)] = rewriter;
            HasRewriters = true;
        }

        internal static SqlRewriter GetRewriter(SqlDialect dialect)
        {
            if (HasRewriters)
            {
                var type = dialect.GetType();
                if (Rewriters.ContainsKey(type))
                    return Rewriters[type];
            }

            return null;
        }

        public abstract SqlStatement Rewrite(SqlStatement sql);
    }
}
