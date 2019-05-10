// BootFX - Application framework for .NET applications
// 
// File: Log.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Linq;
using System.Text;
using System.Threading;

namespace BootFX.Common.Management
{
	/// <summary>
	/// Summary description for Log.
	/// </summary>
	public class Log : ILog
	{
		private const string OperationHeaderFooterSeparator = "**************************";

		/// <summary>
		/// Private field to support <c>Appenders</c> property.
		/// </summary>
		private AppenderCollection _appenders = new AppenderCollection();
		
		/// <summary>
		/// Private field to support <see cref="Name"/> property.
		/// </summary>
		private string _name;
		
        ///// <summary>
        ///// Private field to support <c>Level</c> property.
        ///// </summary>
        //private LogLevel _level;
		
		/// <summary>
		/// Constructor.
		/// </summary>
//		internal Log(LogLevel level, string name)
		internal Log(string name)
		{
			if(name == null)
				throw new ArgumentNullException("name");
			if(name.Length == 0)
				throw new ArgumentOutOfRangeException("'name' is zero-length.");
			
			//_level = level;
			_name = name;
		}

		/// <summary>
		/// Gets the name.
		/// </summary>
		public string Name
		{
			get
			{
				return _name;
			}
		}

        ///// <summary>
        ///// Gets the level.
        ///// </summary>
        //public LogLevel Level
        //{
        //    get
        //    {
        //        // returns the value...
        //        return _level;
        //    }
        //}

		/// <summary>
		/// Returns true if the given level is enabled.
		/// </summary>
		/// <param name="level"></param>
		/// <returns></returns>
		internal bool IsLevelEnabled(LogLevel level)
		{
            //return IsLevelEnabled(this.Level, level);
            return this.Appenders.Any(v => v.IsLevelEnabled(level));
		}

        ///// <summary>
        ///// Returns true if the given level is enabled.
        ///// </summary>
        ///// <param name="masterLevel"></param>
        ///// <param name="checkLevel"></param>
        ///// <returns></returns>
        //internal static bool IsLevelEnabled(LogLevel masterLevel, LogLevel checkLevel)
        //{
        //    // mbr - 2013-02-15 - trace?
        //    if (checkLevel == LogLevel.Trace && Runtime.IsStarted && Runtime.Current.InstallationSettings.IsTraceEnabled)
        //        return true;

        //    // next...
        //    if(checkLevel >= masterLevel)
        //        return true;
        //    else
        //        return false;
        //}

        public bool IsTraceEnabled
        {
            get
            {
                return this.IsLevelEnabled(LogLevel.Trace);
            }
        }

        public bool IsDebugEnabled
		{
			get
			{
				return this.IsLevelEnabled(LogLevel.Debug);
			}
		}

		public bool IsInfoEnabled
		{
			get
			{
				return this.IsLevelEnabled(LogLevel.Info);
			}
		}

		public bool IsWarnEnabled
		{
			get
			{
				return this.IsLevelEnabled(LogLevel.Warn);
			}
		}

		public bool IsErrorEnabled
		{
			get
			{
				return this.IsLevelEnabled(LogLevel.Error);
			}
		}

		public bool IsFatalEnabled
		{
			get
			{
				return this.IsLevelEnabled(LogLevel.Fatal);
			}
		}

        public void Trace(string buf, Exception exception)
        {
            this.LogMessage(LogLevel.Trace, buf, exception);
        }

        public void Debug(string buf, Exception exception)
		{
			this.LogMessage(LogLevel.Debug, buf, exception);
		}

		public void Info(string buf, Exception exception)
		{
			this.LogMessage(LogLevel.Info, buf, exception);
		}

		public void Warn(string buf, Exception exception)
		{
			this.LogMessage(LogLevel.Warn, buf, exception);
		}

		public void Error(string buf, Exception exception)
		{
			this.LogMessage(LogLevel.Error, buf, exception);
		}

		public void Fatal(string buf, Exception exception)
		{
			this.LogMessage(LogLevel.Fatal, buf, exception);
		}

        public void Trace(string buf)
        {
            this.LogMessage(LogLevel.Trace, buf);
        }

        public void Debug(string buf)
		{
			this.LogMessage(LogLevel.Debug, buf);
		}

		public void Info(string buf)
		{
			this.LogMessage(LogLevel.Info, buf);
		}

		public void Warn(string buf)
		{
			this.LogMessage(LogLevel.Warn, buf);
		}

		public void Error(string buf)
		{
			this.LogMessage(LogLevel.Error, buf);
		}

		public void Fatal(string buf)
		{
			this.LogMessage(LogLevel.Fatal, buf);
		}

        public void Trace(LogArgs args)
        {
            this.LogMessage(LogLevel.Trace, args);
        }

        public void Debug(LogArgs args)
        {
            this.LogMessage(LogLevel.Debug, args);
        }

        public void Info(LogArgs args)
        {
            this.LogMessage(LogLevel.Info, args);
        }

        public void Warn(LogArgs args)
        {
            this.LogMessage(LogLevel.Warn, args);
        }

        public void Error(LogArgs args)
        {
            this.LogMessage(LogLevel.Error, args);
        }

        public void Fatal(LogArgs args)
        {
            this.LogMessage(LogLevel.Fatal, args);
        }

        /// <summary>
		/// Logs the given message.
		/// </summary>
		/// <param name="level"></param>
		/// <param name="buf"></param>
		/// <returns></returns>
        internal void LogMessage(LogLevel level, string buf, Exception exception = null, Action<LogArgs> options = null)
        {
            var args = new LogArgs(buf, exception);
            if (options != null)
                options(args);
            this.LogMessage(level, args);
        }

        private void LogMessage(LogLevel level, LogArgs args)
		{
            if(this.IsLevelEnabled(level) == false)
				return;

			// walk the appenders...
			foreach(Appender appender in this.Appenders)
			{
				try
				{
					appender.Append(level, args);
				}
				catch(Exception ex)
				{
					LogManager.InternalError(string.Format("Failed to append to '{0}'.", appender), ex);
				}
			}
		}

		internal static void LogMessage(ILog log, LogLevel level, string buf, Exception ex)
		{
			if(log == null)
				throw new ArgumentNullException("log");
			
			// check...
			switch(level)
			{
                case LogLevel.Trace:
                    if (log.IsTraceEnabled)
                        log.Trace(buf, ex);
                    break;

                case LogLevel.Debug:
					if(log.IsDebugEnabled)
						log.Debug(buf, ex);
					break;

				case LogLevel.Info:
					if(log.IsInfoEnabled)
						log.Info(buf, ex);
					break;

				case LogLevel.Warn:
					if(log.IsWarnEnabled)
						log.Warn(buf, ex);
					break;

				case LogLevel.Error:
					if(log.IsErrorEnabled)
						log.Error(buf, ex);
					break;

				case LogLevel.Fatal:
					if(log.IsFatalEnabled)
						log.Fatal(buf, ex);
					break;

				default:
					throw new NotSupportedException(string.Format("Cannot handle '{0}' ({1}).", level, level.GetType()));
			}
		}

		/// <summary>
		/// Gets a collection of IAppender objects.
		/// </summary>
		public AppenderCollection Appenders
		{
			get
			{
				return _appenders;
			}
		}

		public void WriteOperationHeader(string caption)
		{
			Info(OperationHeaderFooterSeparator);
			Info("{Started} " + caption);
			Info("\tMachine name: " + Environment.MachineName);
			Info(string.Format("\tUsername: {0}\\{1}", Environment.UserDomainName, Environment.UserName));
			Info(OperationHeaderFooterSeparator);
		}

		public void WriteOperationFooter(string caption)
		{
			Info(OperationHeaderFooterSeparator);
			Info("{Finished} " + caption);
			Info(OperationHeaderFooterSeparator);
		}

        public void TraceFormat(IFormatProvider culture, string format, params object[] args)
        {
            this.TraceFormat(culture, format, null, args);
        }

        public void TraceFormat(string format, params object[] args)
        {
            this.TraceFormat(Cultures.Log, format, args);
        }

        public void TraceFormat(IFormatProvider culture, string format, Exception ex, params object[] args)
        {
            this.LogMessageFormat(LogLevel.Trace, culture, format, ex, args);
        }

        public void TraceFormat(string format, Exception ex, params object[] args)
        {
            this.TraceFormat(Cultures.Log, format, ex, args);
        }

        public void DebugFormat(IFormatProvider culture, string format, params object[] args)
		{
			this.DebugFormat(culture, format, null, args);
		}

		public void DebugFormat(string format, params object[] args)
		{
			this.DebugFormat(Cultures.Log, format, args);
		}

		public void DebugFormat(IFormatProvider culture, string format, Exception ex, params object[] args)
		{
			this.LogMessageFormat(LogLevel.Debug, culture, format, ex, args);
		}

		public void DebugFormat(string format, Exception ex, params object[] args)
		{
			this.DebugFormat(Cultures.Log, format, ex, args);
		}

		public void InfoFormat(IFormatProvider culture, string format, params object[] args)
		{
			this.InfoFormat(culture, format, null, args);
		}

		public void InfoFormat(string format, params object[] args)
		{
			this.InfoFormat(Cultures.Log, format, args);
		}

		public void InfoFormat(IFormatProvider culture, string format, Exception ex, params object[] args)
		{
			this.LogMessageFormat(LogLevel.Info, culture, format, ex, args);
		}

		public void InfoFormat(string format, Exception ex, params object[] args)
		{
			this.InfoFormat(Cultures.Log, format, ex, args);
		}

		public void WarnFormat(IFormatProvider culture, string format, params object[] args)
		{
			this.WarnFormat(culture, format, null, args);
		}

		public void WarnFormat(string format, params object[] args)
		{
			this.WarnFormat(Cultures.Log, format, args);
		}

		public void WarnFormat(IFormatProvider culture, string format, Exception ex, params object[] args)
		{
			this.LogMessageFormat(LogLevel.Warn, culture, format, ex, args);
		}

		public void WarnFormat(string format, Exception ex, params object[] args)
		{
			this.WarnFormat(Cultures.Log, format, ex, args);
		}

		public void ErrorFormat(IFormatProvider culture, string format, params object[] args)
		{
			this.ErrorFormat(culture, format, null, args);
		}

		public void ErrorFormat(string format, params object[] args)
		{
			this.ErrorFormat(Cultures.Log, format, args);
		}

		public void ErrorFormat(IFormatProvider culture, string format, Exception ex, params object[] args)
		{
			this.LogMessageFormat(LogLevel.Error, culture, format, ex, args);
		}

		public void ErrorFormat(string format, Exception ex, params object[] args)
		{
			this.ErrorFormat(Cultures.Log, format, ex, args);
		}

		public void FatalFormat(IFormatProvider culture, string format, params object[] args)
		{
			this.FatalFormat(culture, format, null, args);
		}

		public void FatalFormat(string format, params object[] args)
		{
			this.FatalFormat(Cultures.Log, format, args);
		}

		public void FatalFormat(IFormatProvider culture, string format, Exception ex, params object[] args)
		{
			this.LogMessageFormat(LogLevel.Fatal, culture, format, ex, args);
		}

		public void FatalFormat(string format, Exception ex, params object[] args)
		{
			this.FatalFormat(Cultures.Log, format, ex, args);
		}
 
		private void LogMessageFormat(LogLevel level, IFormatProvider culture, string format, Exception exception, object[] args)
		{
			// format...
			string message = string.Format(culture, format, args);

			// defer...
			this.LogMessage(level, message, exception);
		}
	}
}
