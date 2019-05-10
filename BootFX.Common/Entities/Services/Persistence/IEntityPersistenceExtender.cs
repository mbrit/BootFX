// BootFX - Application framework for .NET applications
// 
// File: IEntityPersistenceExtender.cs
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
    public static class IEntityPersistenceExtender
    {
        private static IEnumerable<object> CastIds(IEnumerable<int> set)
        {
            var s = new List<object>();
            foreach (var id in set)
                s.Add(id);
            return s;
        }

        private static IEnumerable<object> CastIds(IEnumerable<long> set)
        {
            var s = new List<object>();
            foreach (var id in set)
                s.Add(id);
            return s;
        }

        public static IEnumerable<T> GetByIds<T>(this IEntityPersistence service, IEnumerable<long> ids)
            where T : Entity
        {
            return service.GetByIds<T>(ids);
        }

        public static IEnumerable<T> GetByIds<T>(this IEntityPersistence service, IEnumerable<int> ids)
            where T : Entity
        {
            return service.GetByIds<T>(ids.ToInt64s());
        }

        public static Dictionary<int, T> GetByIdsAsDictionary<T>(this IEntityPersistence service, IEnumerable<int> ids)
            where T : Entity
        {
            var results = new Dictionary<int, T>();
            var items = service.GetByIds<T>(ids);
            foreach (var item in items)
            {
                var id = ConversionHelper.ToInt32(service.EntityType.Storage.GetKeyValues(item)[0]);
                results[id] = item;
            }
            return results;
        }

        public static Dictionary<long, T> GetByIdsAsDictionary<T>(this IEntityPersistence service, IEnumerable<long> ids)
            where T : Entity
        {
            var results = new Dictionary<long, T>();
            var items = service.GetByIds<T>(ids);
            foreach (var item in items)
            {
                var id = ConversionHelper.ToInt64(service.EntityType.Storage.GetKeyValues(item)[0]);
                results[id] = item;
            }
            return results;
        }

        public static PagedDataResult<T> GetByIdsPaged<T>(this IEntityPersistence service, IEnumerable<long> ids, IPagedDataRequestSource source)
            where T : Entity
        {
            //var asObjects = CastIds(set);
            return service.GetByIdsPaged<T>(ids, source);
        }

        public static PagedDataResult<T> GetByIdsPaged<T>(this IEntityPersistence service, IEnumerable<int> ids, IPagedDataRequestSource source)
            where T : Entity
        {
            return service.GetByIdsPaged<T>(ids.ToInt64s(), source);
        }
    }
}
