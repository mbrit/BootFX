// BootFX - Application framework for .NET applications
// 
// File: ISqlFilterSink.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Collections;
using System.Linq;
using System.Text;

namespace BootFX.Common.Data
{
    public interface ISqlFilterSink
    {
        IList BeforeExecuteEntityCollection(SqlFilter filter, ref object cacheTag);
        void AfterExecuteEntityCollection(SqlFilter sqlFilter, object cacheTag, IList results);

        void FilterCreated(SqlFilter sqlFilter);

        object BeforeExecuteEntity(SqlFilter sqlFilter, ref object cacheTag);

        void AfterExecuteEntity(SqlFilter sqlFilter, object cacheTag, object result);
    }
}
