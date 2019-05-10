// BootFX - Application framework for .NET applications
// 
// File: ILog.cs
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
	/// Interface for logging.
	/// </summary>
	public interface ILog : IIsLoggable
	{
		AppenderCollection Appenders
		{
			get;
		}

        //LogLevel Level
        //{
        //    get;
        //}

        bool IsTraceEnabled
        {
            get;
        }

        bool IsDebugEnabled
		{
			get;
		}

		bool IsInfoEnabled
		{
			get;
		}

		bool IsWarnEnabled
		{
			get;
		}

		bool IsErrorEnabled
		{
			get;
		}

		bool IsFatalEnabled
		{
			get;
		}

        void Trace(LogArgs args);
        void Debug(LogArgs args);
        void Info(LogArgs args);
        void Warn(LogArgs args);
        void Error(LogArgs args);
        void Fatal(LogArgs args);

        void Trace(string buf);
        void Debug(string buf);
		void Info(string buf);
		void Warn(string buf);
		void Error(string buf);
		void Fatal(string buf);

        void Trace(string buf, Exception exception);
        void Debug(string buf, Exception exception);
		void Info(string buf, Exception exception);
		void Warn(string buf, Exception exception);
		void Error(string buf, Exception exception);
		void Fatal(string buf, Exception exception);

        void TraceFormat(IFormatProvider culture, string format, params object[] args);
        void TraceFormat(string format, params object[] args);
        void TraceFormat(IFormatProvider culture, string format, Exception ex, params object[] args);
        void TraceFormat(string format, Exception ex, params object[] args);

		void DebugFormat(IFormatProvider culture, string format, params object[] args);
		void DebugFormat(string format, params object[] args);
		void DebugFormat(IFormatProvider culture, string format, Exception ex, params object[] args);
		void DebugFormat(string format, Exception ex, params object[] args);

		void InfoFormat(IFormatProvider culture, string format, params object[] args);
		void InfoFormat(string format, params object[] args);
		void InfoFormat(IFormatProvider culture, string format, Exception ex, params object[] args);
		void InfoFormat(string format, Exception ex, params object[] args);

		void WarnFormat(IFormatProvider culture, string format, params object[] args);
		void WarnFormat(string format, params object[] args);
		void WarnFormat(IFormatProvider culture, string format, Exception ex, params object[] args);
		void WarnFormat(string format, Exception ex, params object[] args);

		void ErrorFormat(IFormatProvider culture, string format, params object[] args);
		void ErrorFormat(string format, params object[] args);
		void ErrorFormat(IFormatProvider culture, string format, Exception ex, params object[] args);
		void ErrorFormat(string format, Exception ex, params object[] args);

		void FatalFormat(IFormatProvider culture, string format, params object[] args);
		void FatalFormat(string format, params object[] args);
		void FatalFormat(IFormatProvider culture, string format, Exception ex, params object[] args);
		void FatalFormat(string format, Exception ex, params object[] args);

		string Name
		{
			get;
		}
	}
}
