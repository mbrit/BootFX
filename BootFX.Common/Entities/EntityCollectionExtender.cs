// BootFX - Application framework for .NET applications
// 
// File: EntityCollectionExtender.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using BootFX.Common.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BootFX.Common.Entities
{
    public static class EntityCollectionExtender
    {
        //public static IEnumerable<int> GetIdsAsInt32(this IEnumerable<Entity> items)
        //{
        //    var et = items.GetType().GenericTypeArguments.First().ToEntityType();
        //    var field = et.GetKeyFields().First();

        //    var results = new List<int>();
        //    foreach (var item in items)
        //        results.Add(ConversionHelper.ToInt32(et.Storage.GetValue(item, field)));

        //    return results;
        //}

        //public static IEnumerable<long> GetIdsAsInt64(this IEnumerable<Entity> items)
        //{
        //    var et = items.GetType().GenericTypeArguments.First().ToEntityType();
        //    var field = et.GetKeyFields().First();

        //    var results = new List<long>();
        //    foreach (var item in items)
        //        results.Add(ConversionHelper.ToInt64(et.Storage.GetValue(item, field)));

        //    return results;
        //}

        public static Dictionary<int, T> ToByIdMap<T>(this IEnumerable<T> items)
            where T : Entity
        {
            var et = typeof(T).ToEntityType();

            var results = new Dictionary<int, T>();
            foreach (var item in items)
            {
                var id = ConversionHelper.ToInt32(et.Storage.GetKeyValues(item).First());
                results[id] = item;
            }
            return results;
        }

        public static void ReorderEntities(this IList items, IEnumerable<long> orderedIds)
        {
            var lookup = new Dictionary<long, Entity>();
            foreach (Entity item in items)
                lookup[((IEntityId)item).Id] = item;

            // clear...
            items.Clear();
            foreach (var id in orderedIds)
            {
                // this ignores missing ids because it's intended to be used with database selections...
                if (lookup.ContainsKey(id))
                    items.Add(lookup[id]);
            }
        }
    }
}
