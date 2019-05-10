// BootFX - Application framework for .NET applications
// 
// File: DateTimeRange.cs
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

namespace BootFX.Common
{
    public class DateTimeRange
    {
        public DateTime From { get; private set; }
        public DateTime To { get; private set; }

        public DateTimeRange(DateTime from, DateTime to)
        {
            this.From = from;
            this.To = to;
        }

        public DateTime FromUtc
        {
            get
            {
                return this.From.ToUniversalTime();
            }
        }

        public DateTime ToUtc
        {
            get
            {
                return this.To.ToUniversalTime();
            }
        }

        public DateTime FromLocal
        {
            get
            {
                return this.From.ToLocalTime();
            }
        }

        public DateTime ToLocal
        {
            get
            {
                return this.To.ToLocalTime();
            }
        }

        public bool Contains(DateTime dt)
        {
            dt = dt.ToUniversalTime();
            return dt >= this.FromUtc && dt <= this.ToUtc;
        }
    }
}
