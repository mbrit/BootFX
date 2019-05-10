// BootFX - Application framework for .NET applications
// 
// File: TimingBucketTimer.cs
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
    internal class TimingBucketTimer : ITimingElement
    {
        public string Name { get; private set; }
        private AccurateTimer Timer { get; set; }
        public decimal Duration { get; set; }

        public TimingBucketTimer(string name)
        {
            this.Name = name;
            this.Timer = new AccurateTimer();
        }

        public void Dispose()
        {
            this.Timer.Stop();
            this.Duration += this.Timer.DurationAsDecimal;
        }

        internal void Start()
        {
            this.Timer.Start();
        }
    }
}
