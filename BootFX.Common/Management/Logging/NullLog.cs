// BootFX - Application framework for .NET applications
// 
// File: NullLog.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;

namespace BootFX.Common.Management
{
	/// <summary>
	/// Defines an instance of <c>NullLog</c>.
	/// </summary>
	internal class NullLog : ILog
	{
		internal NullLog()
		{
		}

		/// <summary>
		/// Private field to support <c>Appenders</c> property.
		/// </summary>
		private AppenderCollection _appenders = new AppenderCollection();
		
		/// <summary>
		/// Gets a collection of Appender objects.
		/// </summary>
		public AppenderCollection Appenders
		{
			get
			{
				return _appenders;
			}
		}

		public BootFX.Common.Management.LogLevel Level
		{
			get
			{
				return LogLevel.Fatal;
			}
		}

		public bool IsDebugEnabled
		{
			get
			{
				// TODO:  Add NullLog.IsDebugEnabled getter implementation
				return false;
			}
		}

		public bool IsInfoEnabled
		{
			get
			{
				// TODO:  Add NullLog.IsInfoEnabled getter implementation
				return false;
			}
		}

		public bool IsWarnEnabled
		{
			get
			{
				// TODO:  Add NullLog.IsWarnEnabled getter implementation
				return false;
			}
		}

		public bool IsErrorEnabled
		{
			get
			{
				// TODO:  Add NullLog.IsErrorEnabled getter implementation
				return false;
			}
		}

		public bool IsFatalEnabled
		{
			get
			{
				// TODO:  Add NullLog.IsFatalEnabled getter implementation
				return false;
			}
		}

		public void Debug(string buf)
		{
			// TODO:  Add NullLog.Debug implementation
		}

		public void Info(string buf)
		{
			// TODO:  Add NullLog.Info implementation
		}

		public void Warn(string buf)
		{
			// TODO:  Add NullLog.Warn implementation
		}

		public void Error(string buf)
		{
			// TODO:  Add NullLog.Error implementation
		}

		public void Fatal(string buf)
		{
			// TODO:  Add NullLog.Fatal implementation
		}

        public void Debug(LogArgs args)
        {
            // TODO:  Add NullLog.Debug implementation
        }

        public void Info(LogArgs args)
        {
            // TODO:  Add NullLog.Info implementation
        }

        public void Warn(LogArgs args)
        {
            // TODO:  Add NullLog.Warn implementation
        }

        public void Error(LogArgs args)
        {
            // TODO:  Add NullLog.Error implementation
        }

        public void Fatal(LogArgs args)
        {
            // TODO:  Add NullLog.Fatal implementation
        }

		void BootFX.Common.Management.ILog.Debug(string buf, Exception exception)
		{
			// TODO:  Add NullLog.BootFX.Common.Management.ILog.Debug implementation
		}

		void BootFX.Common.Management.ILog.Info(string buf, Exception exception)
		{
			// TODO:  Add NullLog.BootFX.Common.Management.ILog.Info implementation
		}

		void BootFX.Common.Management.ILog.Warn(string buf, Exception exception)
		{
			// TODO:  Add NullLog.BootFX.Common.Management.ILog.Warn implementation
		}

		void BootFX.Common.Management.ILog.Error(string buf, Exception exception)
		{
			// TODO:  Add NullLog.BootFX.Common.Management.ILog.Error implementation
		}

		void BootFX.Common.Management.ILog.Fatal(string buf, Exception exception)
		{
			// TODO:  Add NullLog.BootFX.Common.Management.ILog.Fatal implementation
		}

		public void DebugFormat(IFormatProvider culture, string format, params object[] args)
		{
		}

		public void DebugFormat(string format, params object[] args)
		{
		}

		public void DebugFormat(IFormatProvider culture, string format, Exception ex, params object[] args)
		{
		}

		public void DebugFormat(string format, Exception ex, params object[] args)
		{
		}

		public void InfoFormat(IFormatProvider culture, string format, params object[] args)
		{
		}

		public void InfoFormat(string format, params object[] args)
		{
		}

		public void InfoFormat(IFormatProvider culture, string format, Exception ex, params object[] args)
		{
		}

		public void InfoFormat(string format, Exception ex, params object[] args)
		{
		}

		public void WarnFormat(IFormatProvider culture, string format, params object[] args)
		{
		}

		public void WarnFormat(string format, params object[] args)
		{
		}

		public void WarnFormat(IFormatProvider culture, string format, Exception ex, params object[] args)
		{
		}

		public void WarnFormat(string format, Exception ex, params object[] args)
		{
		}

		public void ErrorFormat(IFormatProvider culture, string format, params object[] args)
		{
		}

		public void ErrorFormat(string format, params object[] args)
		{
		}

		public void ErrorFormat(IFormatProvider culture, string format, Exception ex, params object[] args)
		{
		}

		public void ErrorFormat(string format, Exception ex, params object[] args)
		{
		}

		public void FatalFormat(IFormatProvider culture, string format, params object[] args)
		{
		}

		public void FatalFormat(string format, params object[] args)
		{
		}

		public void FatalFormat(IFormatProvider culture, string format, Exception ex, params object[] args)
		{
		}

		public void FatalFormat(string format, Exception ex, params object[] args)
		{
		}

		public string Name
		{
			get
			{
				// TODO:  Add NullLog.Name getter implementation
				return null;
			}
		}

        public bool IsTraceEnabled
        {
            get
            {
                return false;
            }
        }

        public void Trace(LogArgs args)
        {
        }

        public void Trace(string buf)
        {
        }

        public void Trace(string buf, Exception exception)
        {
        }

        public void TraceFormat(IFormatProvider culture, string format, params object[] args)
        {
        }

        public void TraceFormat(string format, params object[] args)
        {
        }

        public void TraceFormat(IFormatProvider culture, string format, Exception ex, params object[] args)
        {
        }

        public void TraceFormat(string format, Exception ex, params object[] args)
        {
        }
    }
}
