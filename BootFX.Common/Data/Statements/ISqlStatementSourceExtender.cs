// BootFX - Application framework for .NET applications
// 
// File: ISqlStatementSourceExtender.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BootFX.Common.Entities;

namespace BootFX.Common.Data
{
    public static class ISqlStatementSourceExtender
    {
        public static int ExecuteNonQuery(this ISqlStatementSource sql)
        {
            return Database.ExecuteNonQuery(sql);
        }

        public static object ExecuteEntity(this ISqlStatementSource sql)
        {
            return Database.ExecuteEntity(sql);
        }

        public static IList ExecuteEntityCollection(this ISqlStatementSource sql)
        {
            return Database.ExecuteEntityCollection(sql);
        }

        public static IEnumerable<T> ExecuteEntityCollection<T>(this ISqlStatementSource sql)
        {
            return Database.ExecuteValues<T>(sql);
        }
    }
}
