// BootFX - Application framework for .NET applications
// 
// File: NullTimingBucket.cs
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
using BootFX.Common.Management;

namespace BootFX.Common
{
    public class NullTimingBucket : Loggable, ITimingBucket
    {
        public string Name { get; private set; }
        public int NumItems { get; set; }

        private NullTimer TheNullTimer { get; set; }

        public static NullTimingBucket Instance { get; set; }

        private NullTimingBucket()
        {
            this.TheNullTimer = new NullTimer();
        }

        static NullTimingBucket()
        {
            Instance = new NullTimingBucket();
        }

        public IDisposable GetTimer(string name)
        {
            return TheNullTimer;
        }

        public ITimingBucket GetChildBucket(string name, int numItems = 1)
        {
            return Instance;
        }

        public string Dump(string preamble)
        {
            if (this.Log.IsInfoEnabled)
                this.Log.InfoFormat("Null timing bucket used --> {0}", preamble);
            return string.Empty;
        }

        private class NullTimer : IDisposable
        {
            public void Dispose()
            {
                // no-op...
            }
        }

        public void Dispose()
        {
            // no-op...
        }

        public decimal Duration
        {
            get
            {
                return 0M;
            }
        }

        public void IncrementNumItems(int increment = 1)
        {
            this.NumItems += increment;
        }

        public void AddTag(string tag)
        {
        }

        public bool IsLogging
        {
            get
            {
                return false;
            }
        }
    }
}
