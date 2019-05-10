// BootFX - Application framework for .NET applications
// 
// File: SortedDataRequest.cs
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
    public class SortedDataRequest : ISortedDataRequest, ISortedDataRequestSource
    {
        public SortDirection Direction { get; set; }

        public string Expression { get; set; }

        public SortedDataRequest(string expression, SortDirection direction = SortDirection.Ascending)
        {
            this.Expression = expression;
            this.Direction = direction;
        }

        ISortedDataRequest ISortedDataRequestSource.Request
        {
            get
            {
                return this;
            }
        }

        public static SortedDataRequest NotSorted
        {
            get
            {
                return new SortedDataRequest(null);
            }
        }
    }
}
