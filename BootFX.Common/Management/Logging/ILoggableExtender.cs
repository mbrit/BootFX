// BootFX - Application framework for .NET applications
// 
// File: ILoggableExtender.cs
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
using System.Threading.Tasks;

namespace BootFX.Common.Management
{
    public static class IIsLoggableExtender
    {
        public static ILog GetLog(this IIsLoggable loggable)
        {
            return LogManager.GetLogger(loggable.GetType());
        }

        public static void LogTrace(this IIsLoggable loggable, Func<string> callback, Exception ex = null, Action<LogArgs> options = null)
        {
            loggable.WriteEntry(LogLevel.Trace, callback, ex, options);
        }

        public static void LogDebug(this IIsLoggable loggable, Func<string> callback, Exception ex = null, Action<LogArgs> options = null)
        {
            loggable.WriteEntry(LogLevel.Debug, callback, ex, options);
        }

        public static void LogInfo(this IIsLoggable loggable, Func<string> callback, Exception ex = null, Action<LogArgs> options = null)
        {
            loggable.WriteEntry(LogLevel.Info, callback, ex, options);
        }

        public static void LogWarn(this IIsLoggable loggable, Func<string> callback, Exception ex = null, Action<LogArgs> options = null)
        {
            loggable.WriteEntry(LogLevel.Warn, callback, ex, options);
        }

        public static void LogError(this IIsLoggable loggable, Func<string> callback, Exception ex = null, Action<LogArgs> options = null)
        {
            loggable.WriteEntry(LogLevel.Error, callback, ex, options);
        }

        public static void LogFatal(this IIsLoggable loggable, Func<string> callback, Exception ex = null, Action<LogArgs> options = null)
        {
            loggable.WriteEntry(LogLevel.Fatal, callback, ex, options);
        }

        private static void WriteEntry(this IIsLoggable loggable, LogLevel level, Func<string> callback, Exception ex = null, Action<LogArgs> options = null)
        {
            var log = (Log)loggable.GetLog();
            if (log.IsLevelEnabled(level))
            {
                var buf = callback();
                log.LogMessage(level, buf, ex, options);
            }
        }

        public static IDisposable StartAuditing()
        {
            //if (Runtime.Current.AuditingEnabled)
            //{
            //}
            //else
                return new NullDisposer();
        }

        private class NullDisposer : IDisposable
        {
            public void Dispose()
            {
                throw new NotImplementedException();
            }
        }
    }
}
