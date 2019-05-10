// BootFX - Application framework for .NET applications
// 
// File: PageResult.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using BootFX.Common.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BootFX.Common.Data
{
    public class PagedDataResult<T>
        //where T : Entity
    {
        public IEnumerable<T> Items { get; private set; }
        public PageData PageData { get; private set; }

        public PagedDataResult(IEnumerable<T> items, PageData pageData)
        {
            this.Items = new List<T>(items);
            this.PageData = pageData;
        }
    }
}
