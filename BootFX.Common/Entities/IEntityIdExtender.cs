// BootFX - Application framework for .NET applications
// 
// File: IEntityIdExtender.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using BootFX.Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BootFX.Common.Entities
{
    public static class IEntityIdExtender
    {
        public static IEnumerable<long> GetIdsInt64(this IEnumerable<IEntityId> items)
        {
            var results = new List<long>();
            foreach (var item in items)
                results.Add(item.Id);
            return results;
        }

        public static IEnumerable<int> GetIdsInt32(this IEnumerable<IEntityId> items)
        {
            var results = new List<int>();
            foreach (var item in items)
                results.Add(ConversionHelper.ToInt32(item.Id));
            return results;
        }
    }
}
