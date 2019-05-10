// BootFX - Application framework for .NET applications
// 
// File: AddArrayParameterUdtsUpdateStep.cs
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
    internal class AddArrayParameterUdtsUpdateStep : DatabaseUpdateStep
    {
        public override void Execute(DatabaseUpdateContext context)
        {
            ArrayParameterUdtsHelper.CheckUdtParameters();
        }
    }

    public static class ArrayParameterUdtsHelper
    {
        public static void CheckUdtParameters()
        {
            try
            {
                //Database.ExecuteNonQuery(@"CREATE TYPE [dbo].[__bfxIdsGuid] AS TABLE(
                // [id] [uniqueidentifier] NULL
                //)");

                //Database.ExecuteNonQuery(@"CREATE TYPE [dbo].[__bfxIdsInt32] AS TABLE(
                // [id] [int] NULL
                //)");

                //Database.ExecuteNonQuery(@"CREATE TYPE [dbo].[__bfxIdsInt64] AS TABLE(
                // [id] [bigint] NULL
                //)");

                //Database.ExecuteNonQuery(@"CREATE TYPE [dbo].[__bfxIdsString] AS TABLE(
                // [id] [nvarchar](max) NULL
                //)");

                var sqls = GetStatements();
                foreach (var sql in sqls)
                    Database.ExecuteNonQuery(sql);
            }
            catch
            {
                // fail this silently...
            }
        }

        public static void CheckUdtParameters(IConnection conn)
        {
            try
            {
                //Database.ExecuteNonQuery(@"CREATE TYPE [dbo].[__bfxIdsGuid] AS TABLE(
                // [id] [uniqueidentifier] NULL
                //)");

                //Database.ExecuteNonQuery(@"CREATE TYPE [dbo].[__bfxIdsInt32] AS TABLE(
                // [id] [int] NULL
                //)");

                //Database.ExecuteNonQuery(@"CREATE TYPE [dbo].[__bfxIdsInt64] AS TABLE(
                // [id] [bigint] NULL
                //)");

                //Database.ExecuteNonQuery(@"CREATE TYPE [dbo].[__bfxIdsString] AS TABLE(
                // [id] [nvarchar](max) NULL
                //)");

                var sqls = GetStatements();
                foreach (var sql in sqls)
                    conn.ExecuteNonQuery(new SqlStatement(sql));
            }
            catch
            {
                // fail this silently...
            }
        }

        private static IEnumerable<string> GetStatements()
        {
            return new List<string>() { @"CREATE TYPE [dbo].[__bfxIdsGuid] AS TABLE(
	                [id] [uniqueidentifier] NULL
                )", @"CREATE TYPE [dbo].[__bfxIdsInt32] AS TABLE(
	                [id] [int] NULL
                )", @"CREATE TYPE [dbo].[__bfxIdsInt64] AS TABLE(
	                [id] [bigint] NULL
                )", @"CREATE TYPE [dbo].[__bfxIdsString] AS TABLE(
	                [id] [nvarchar](max) NULL
                )" };
        }
    }
}
