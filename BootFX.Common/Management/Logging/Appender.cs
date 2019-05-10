// BootFX - Application framework for .NET applications
// 
// File: Appender.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;

namespace BootFX.Common.Management
{
	/// <summary>
	/// Summary description for Appender.
	/// </summary>
	public abstract class Appender
	{
		/// <summary>
		/// Private field to support <c>LazyWrite</c> property.
		/// </summary>
        //private bool _lazyWrite;
		
		/// <summary>
		/// Private field to support <c>InternalLog</c> property.
		/// </summary>
		private ILog _internalLog;
		
		/// <summary>
		/// Private field to support <see cref="Level"/> property.
		/// </summary>
		//private LogLevel _level;
		
		/// <summary>
		/// Private field to support <c>Log</c> property.
		/// </summary>
		private ILog _log;
		
		/// <summary>
		/// Private field to support <see cref="Formatter"/> property.
		/// </summary>
		private ILogFormatter _formatter;

        public LogLevel MinLevel { get; private set; }
        public LogLevel MaxLevel { get; private set; }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="formatter"></param>
		protected Appender(ILog log, ILogFormatter formatter, LogLevel minLevel, LogLevel maxLevel = LogLevel.Fatal)
		{
			if(log == null)
				throw new ArgumentNullException("log");
			if(formatter == null)
				throw new ArgumentNullException("formatter");
			_log = log;
			_formatter = formatter;

            //_level = level;
            // mbr - 2013-02-20 - changed...
            this.MinLevel = minLevel;
            this.MaxLevel = maxLevel;

			// internal...
			_internalLog = new Log(log.Name + " [Internal]");
		}

		/// <summary>
		/// Gets the log.
		/// </summary>
		private ILog Log
		{
			get
			{
				// returns the value...
				return _log;
			}
		}

		/// <summary>
		/// Gets the formatter.
		/// </summary>
		private ILogFormatter Formatter
		{
			get
			{
				return _formatter;
			}
		}

		/// <summary>
		/// Returns true if the appender is enabled for this level.
		/// </summary>
		/// <param name="level"></param>
		/// <returns></returns>
        //private bool IsLevelEnabled(LogLevel level)
        //{
        //    return Management.Log.IsLevelEnabled(this.Level, level);
        //}

        public void Append(LogLevel level, string buf, Exception ex)
        {
            this.Append(level, new LogArgs(buf, ex));
        }

		/// <summary>
		/// Appends a message.
		/// </summary>
		/// <param name="buf"></param>
		public void Append(LogLevel level, LogArgs args)
		{
			if(Log == null)
				throw new InvalidOperationException("Log is null.");
			if(Formatter == null)
				throw new InvalidOperationException("Formatter is null.");

			// are we supported for this level?
			if(this.IsLevelEnabled(level))
			{
				// format it...
				string formatted = this.Formatter.Format(this.Log, level, args);

				// package it...
				LogData data = new LogData(level, args, formatted);

				// mbr - 07-11-2005 - if the appender supports lazy writing, do it in a background thread...				
                //if(this.LazyWrite && Runtime.IsStarted)
                //{
                //    // do we have the service?
                //    if(Runtime.Current.InternalManagementService == null)
                //        throw new InvalidOperationException("Runtime.Current.InternalManagementService is null.");
                //    if(Runtime.Current.InternalManagementService.LazyLoggingEngine == null)
                //        throw new InvalidOperationException("Runtime.Current.InternalManagementService.LazyLoggingEngine is null.");

                //    // queue it...
                //    Runtime.Current.InternalManagementService.LazyLoggingEngine.Enqueue((Appender)this, data);
                //}
                //else
				{
					// write it...
					this.DoAppend(data);
				}
			}
		}

		/// <summary>
		/// Appends the given message.
		/// </summary>
		/// <param name="buf"></param>
		protected internal abstract void DoAppend(LogData data);

        ///// <summary>
        ///// Gets the level.
        ///// </summary>
        //public LogLevel Level
        //{
        //    get
        //    {
        //        return _level;
        //    }
        //}

		/// <summary>
		/// Gets the log used for handling errors in the appender itself.
		/// </summary>
		protected ILog InternalLog
		{
			get
			{
				// returns the value...
				return _internalLog;
			}
		}

		/// <summary>
		/// Gets or sets whether messages should be written in a background thread.
		/// </summary>
        //public bool LazyWrite
        //{
        //    get
        //    {
        //        return _lazyWrite;
        //    }
        //    set
        //    {
        //        // check to see if the value has changed...
        //        if(value != _lazyWrite)
        //        {
        //            // set the value...
        //            _lazyWrite = value;
        //        }
        //    }
        //}

        internal bool IsLevelEnabled(LogLevel level)
        {
            return (int)level >= (int)this.MinLevel && (int)level <= (int)this.MaxLevel;
        }
    }
}
