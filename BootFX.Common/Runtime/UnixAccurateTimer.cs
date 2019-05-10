// BootFX - Application framework for .NET applications
// 
// File: UnixAccurateTimer.cs
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

namespace BootFX.Common
{
    internal class UnixAccurateTimer : IAccurateTimer
    {
        private long _start = 0;
        private long _stop = 0;

        public long Start()
        {
            _start = Environment.TickCount;
            return _start;
        }

        public long Stop()
        {
            _stop = Environment.TickCount;
            return _stop;
        }

        public double Duration
        {
            get
            {
                return ((double)_stop - (double)_start) / 1000;
            }
        }

        public decimal DurationAsDecimal
        {
            get
            {
                return ((decimal)_stop - (decimal)_start) / 1000;
            }
        }
    }
}
