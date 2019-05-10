// BootFX - Application framework for .NET applications
// 
// File: ElapsingServiceEngine.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using BootFX.Common.Management;
using System;

namespace BootFX.Common.Services
{
	/// <summary>
	/// Summary description for TimedServiceEngine.
	/// </summary>
	public abstract class ElapsingServiceEngine : ThreadedServiceEngine
	{
		/// <summary>
		/// Private field to support <c>flags</c> property.
		/// </summary>
		private ElapsingServiceEngineFlags _flags;
		
		/// <summary>
		/// Raised when an elapse has finished.
		/// </summary>
		// mbr - 20-04-2006 - added.					
		public event EventHandler Finished;
		
		/// <summary>
		/// Private field to support <c>DefaultElapsePeriod</c> property.
		/// </summary>
		private TimeSpan _defaultElapsePeriod;
		
		/// <summary>
		/// Raised when the service elapses.
		/// </summary>
		public event EventHandler Elapsed;
		
		/// <summary>
		/// Private field to support <c>Elaspses</c> property.
		/// </summary>
		private DateTime _elapses;

        protected ElapsingServiceEngine()
            : this(ElapsingServiceEngineFlags.UseLocalTime)
        {
        }

        protected ElapsingServiceEngine(ElapsingServiceEngineFlags flags)
            : this(new TimeSpan(0, 0, 30), flags)
        {
            _flags = flags;
        }

        protected ElapsingServiceEngine(TimeSpan defaultElapsePeriod) 
            : this(defaultElapsePeriod, ElapsingServiceEngineFlags.UseLocalTime)
		{
			_defaultElapsePeriod = defaultElapsePeriod;
			ResetElapses();
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		protected ElapsingServiceEngine(TimeSpan defaultElapsePeriod, ElapsingServiceEngineFlags flags) 
//            : base(TimeSpan.FromMilliseconds(100))
            : base(TimeSpan.FromMilliseconds(200))
		{
			_defaultElapsePeriod = defaultElapsePeriod;
			ResetElapses();
		}

		/// <summary>
		/// Gets the flags.
		/// </summary>
		private ElapsingServiceEngineFlags Flags
		{
			get
			{
				// returns the value...
				return _flags;
			}
		}

		/// <summary>
		/// Gets the defaultelapseperiod.
		/// </summary>
		private TimeSpan DefaultElapsePeriod
		{
			get
			{
				// returns the value...
				return _defaultElapsePeriod;
			}
		}

		/// <summary>
		/// Checks the timer.
		/// </summary>
		protected sealed override bool DoWork()
		{
			// have we elapsed?
			if(this.IsElapsed)
			{
				try
				{
                    this.Status = ServiceEngineStatus.Active;

                    // set...
                    this.OnWorkStarted(EventArgs.Empty);

                    // monitor...
                    this.OnElapsed();
                }
                finally
				{
                    try
                    {
                        this.OnWorkFinished(EventArgs.Empty);
                    }
                    catch
                    {
                        // no-op...
                    }

                    try
                    {
                        // reset...
                        this.ResetElapses();

                        // mbr - 20-04-2006 - added.					
                        this.OnFinished();
                    }
                    finally
                    {
                        this.Status = ServiceEngineStatus.Idle;
                    }
				}
			}

			// ok...
			return true;
		}

		/// <summary>
		/// Gets the elaspses.
		/// </summary>
		private DateTime Elapses
		{
			get
			{
				// returns the value...
				return _elapses;
			}
			set
			{
				if(value == DateTime.MinValue)
					throw new ArgumentNullException("value");

				// set...
				_elapses = value;
			}
		}

		/// <summary>
		/// Returns true if the service is elapsed.
		/// </summary>
		private bool IsElapsed
		{
			get
			{
				if(this.Elapses != DateTime.MinValue)
				{
					if(CurrentTime >= this.Elapses)
						return true;
					else
						return false;
				}
				else
					return false;
			}
		}

		/// <summary>
		/// Gets the time that the engine is basing time calculations on.
		/// </summary>
		protected DateTime CurrentTime
		{
			get
			{
                //this.LogDebug(() => string.Format("Getting time for '{0}'...", this.GetType().Name));

				if((int)(this.Flags & ElapsingServiceEngineFlags.UseLocalTime) != 0)
					return DateTime.Now;
				else
					return DateTime.UtcNow;
			}
		}

		/// <summary>
		/// Gets the next time to elapse.
		/// </summary>
		/// <returns>The next elapse time, as a UTC time.</returns>
		/// <remarks>By default, this happens in 30 seconds from the current UTC time.</remarks>
		protected virtual DateTime GetNextTimeToElapse()
		{
			return CurrentTime.AddSeconds(30);
		}

		/// <summary>
		/// Resets when the service next elapses.
		/// </summary>
		private void ResetElapses()
		{
			this.Elapses = this.GetNextTimeToElapse();
		}

		/// <summary>
		/// Raises the <c>Elapsed</c> event.
		/// </summary>
		private void OnElapsed()
		{
			OnElapsed(EventArgs.Empty);
		}
		
		/// <summary>
		/// Raises the <c>Elapsed</c> event.
		/// </summary>
		protected virtual void OnElapsed(EventArgs e)
		{
			// raise...
			if(Elapsed != null)
				Elapsed(this, e);
		}

		/// <summary>
		/// Adjusts the start time to the next hour.
		/// </summary>
		/// <param name="date"></param>
		/// <returns></returns>
		protected DateTime AdjustToNextTime(int hour, int minute, int second)
		{
			return AdjustToNextTime(CurrentTime, hour, minute, second);
		}

		/// <summary>
		/// Adjusts the start time to the next hour.
		/// </summary>
		/// <param name="date"></param>
		/// <returns></returns>
		protected DateTime AdjustToNextTime(DateTime utcFrom, int hour, int minute, int second)
		{
			if(hour < 0 || hour > 23)
				throw new ArgumentOutOfRangeException("hour", hour, "Hour must be between 0 and 23.");
			if(minute < 0 || minute > 60)
				throw new ArgumentOutOfRangeException("minute", minute, "Minute must be between 0 and 60.");
			if(second < 0 || second > 60)
				throw new ArgumentOutOfRangeException("second", second, "Second must be between 0 and 60.");

			// create...
			DateTime date = new DateTime(utcFrom.Year, utcFrom.Month, utcFrom.Day, hour, minute, second);

			// make sure it's in the future...
			while(date < CurrentTime)
				date = date.AddDays(1);

			// return...
			return date;
		}

		/// <summary>
		/// Sets the engine to elapse now.
		/// </summary>
		public void ElapseNow()
		{
			// set...
			this.Elapses = CurrentTime;
			
			// run...
			this.Tick();
		}

		protected override void OnStarted(EventArgs e)
		{
			base.OnStarted (e);

			// mbr - 12-11-2005 - start the thread...			
			this.EnsureThreadInitialized();
		}

		/// <summary>
		/// Raises the <c>Finished</c> event.
		/// </summary>
		private void OnFinished()
		{
			OnFinished(EventArgs.Empty);
		}
		
		/// <summary>
		/// Raises the <c>Finished</c> event.
		/// </summary>
		protected virtual void OnFinished(EventArgs e)
		{
			// raise...
			if(Finished != null)
				Finished(this, e);
		}

        /// <summary>
        /// Set the engine to elapse in the given number of seconds.
        /// </summary>
        /// <param name="seconds"></param>
        public void ElapseIn(int seconds)
        {
            this.ElapseIn(new TimeSpan(0, 0, seconds));
        }

        /// <summary>
        /// Set the engine to elapse in the given number of seconds.
        /// </summary>
        /// <param name="seconds"></param>
        public void ElapseIn(TimeSpan period)
        {
            // set...
            this.Elapses = this.CurrentTime.Add(period);
        }
	}
}
