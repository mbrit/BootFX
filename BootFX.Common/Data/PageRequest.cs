// BootFX - Application framework for .NET applications
// 
// File: PageRequest.cs
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
    public class PagedDataRequest : IPagedDataRequest, IPagedDataRequestSource
    {
        public int PageNumber { get; private set; }
        public int PageSize { get; private set; }

        public static PagedDataRequest NotPaged { get; private set; }

        public PagedDataRequest(int pageNumber, int pageSize)
        {
            if (pageNumber < 0)
                throw new ArgumentException("Page number must be zero or more.");

            this.PageNumber = pageNumber;
            this.PageSize = pageSize;
        }

        static PagedDataRequest()
        {
            NotPaged = new PagedDataRequest(0, 0);
        }

        public IPagedDataRequest Request
        {
            get
            {
                return this;
            }
        }
    }
}
