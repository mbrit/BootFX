// BootFX - Application framework for .NET applications
// 
// File: WindowsAccurateTimer.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace BootFX.Common
{
    internal class WindowsAccurateTimer : IAccurateTimer
    {
		[DllImport("Kernel32.dll")]
		internal static extern bool QueryPerformanceCounter(out long lpPerformanceCount);
		[DllImport("Kernel32.dll")]
		internal static extern bool QueryPerformanceFrequency(out long lpFrequency); 

		// fields...
		private long startTime;
		private long stopTime;
		private long freq;

		/// <summary>
		/// Constructor.
		/// </summary>
        [SecurityCritical]
		public WindowsAccurateTimer()
		{
			startTime = 0;
			stopTime = 0;
			freq = 0;
			if (QueryPerformanceFrequency(out freq) == false)
			{ 
				throw new Win32Exception(); // timer not supported
			}
		}  

		/// <summary>
		/// Start the timer
		/// </summary>
		/// <returns>long - tick count</returns>
		public long Start()
		{
			QueryPerformanceCounter(out startTime);
			return startTime;
		}
		/// <summary>
		/// Stop timer 
		/// </summary>
		/// <returns>long - tick count</returns>
		public long Stop()
		{
			stopTime = this.Peek();
			return stopTime;
		} 

		/// <summary>
		/// Peeks the timer value.
		/// </summary>
		/// <returns></returns>
		public long Peek()
		{
			long result = 0;
			QueryPerformanceCounter(out result);

			// return...
			return result;
		}

		/// <summary>
		/// Return the duration of the timer (in seconds)
		/// </summary>
		/// <returns>double - duration</returns>
		public double Duration
		{
			get
			{
				return (double)(stopTime - startTime) / (double) freq;
			}
		}
		/// <summary>
		/// Frequency of timer (no counts in one second on this machine)
		/// </summary>
		///<returns>long - Frequency</returns>
		public long Frequency 
		{
			get
			{
				QueryPerformanceFrequency(out freq);
				return freq;
			}
		}

        public decimal DurationAsDecimal
        {
            get
            {
                return (decimal)this.Duration;
            }
        }
    }
}
