// BootFX - Application framework for .NET applications
// 
// File: ProcessItemsResult.cs
// Build: 5.0.61009.900
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
    public class ProcessItemsResult<T>
    {
        public DateTime StartUtc { get; private set; }
        public DateTime EndUtc { get; internal set; }
        public Dictionary<T, decimal> Timings { get; private set; }

        public ProcessItemsResult(DateTime startUtc)
        {
            this.StartUtc = startUtc;
            this.Timings = new Dictionary<T, decimal>();
        }

        public decimal AverageSeconds
        {
            get
            {
                if (this.Timings.Count > 0)
                {
                    var total = 0M;
                    foreach (var duration in Timings.Values)
                        total += duration;

                    return total / (decimal)this.Timings.Count;
                }
                else
                    return 0;
            }
        }
    }
}
