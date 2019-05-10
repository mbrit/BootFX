// BootFX - Application framework for .NET applications
// 
// File: AccurateTimer.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Threading;

namespace BootFX.Common
{
	/// <summary>
	/// Summary description for AccurateTimer.
	/// </summary>
	public class AccurateTimer
	{
        private IAccurateTimer InnerTimer { get; set;  }

        private static bool IsUnix { get; set; }

        public AccurateTimer()
        {
            if(IsUnix)
                InnerTimer = new UnixAccurateTimer();
            else
                InnerTimer = new WindowsAccurateTimer();
        }

        static AccurateTimer()
        {
            IsUnix = Environment.OSVersion.Platform == PlatformID.Unix;
        }

        public void Start()
        {
            this.InnerTimer.Start();
        }

        public void Stop()
        {
            this.InnerTimer.Stop();
        }

        public double Duration
        {
            get
            {
                return this.InnerTimer.Duration;
            }
        }

        public decimal DurationAsDecimal
        {
            get
            {
                return this.InnerTimer.DurationAsDecimal;
            }
        }
	}
}
