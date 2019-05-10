// BootFX - Application framework for .NET applications
// 
// File: PageData.cs
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
using System.Threading.Tasks;

namespace BootFX.Common.Data
{
    public class PageData
    {
        public int Page { get; private set; }
        public int PageSize { get; private set; }
        public int NumPages { get; private set; }
        public int NumResults { get; private set; }
        public bool MaxResultsReached { get; private set; }

        public PageData(int page, int pageSize, int numPages, int numResults, bool maxResultsReached)
        {
            this.Page = page;
            this.PageSize = pageSize;
            this.NumPages = numPages;
            this.NumResults = numResults;
            this.MaxResultsReached = maxResultsReached;
        }

        public static PageData GetUnpagedData(int count)
        {
            return new PageData(0, count, 1, count, false);
        }
    }
}
