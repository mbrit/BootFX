// BootFX - Application framework for .NET applications
// 
// File: SqlFilterT.cs
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
using BootFX.Common.Entities;

namespace BootFX.Common.Data
{
    [Serializable]
    public class SqlFilter<T> : SqlFilter
        where T : Entity
    {
        public SqlFilter()
            : base(typeof(T))
        {
        }

        public IEnumerable<T> ExecuteEnumerable()
        {
            return (IEnumerable<T>)this.ExecuteEntityCollection();
        }

        public T ExecuteFirstOrDefault()
        {
            return (T)this.ExecuteEntity();
        }

        protected virtual IEnumerable<long> GetKeyValuesForExecutePage()
        {
            var ids = this.ExecuteKeyValues<long>();
            return ids;
        }

        public PagedDataResult<T> ExecutePage(IPagedDataRequestSource source)
        {
            // get all of the ids...
            var ids = this.GetKeyValuesForExecutePage();
            return this.EntityType.Persistence.GetByIdsPaged<T>(ids, source);
        }
    }
}
