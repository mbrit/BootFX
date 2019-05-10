// BootFX - Application framework for .NET applications
// 
// File: DefaultLogFormatter.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Text;
using System.Threading;

namespace BootFX.Common.Management
{
	/// <summary>
	/// Default log formatter.
	/// </summary>
	public class DefaultLogFormatter : ILogFormatter
	{
        private const char InterpunctSep = '·';

        public DefaultLogFormatter()
		{
		}

		/// <summary>
		/// Formats the message.
		/// </summary>
		/// <param name="log"></param>
		/// <param name="buf"></param>
		/// <returns></returns>
		public string Format(ILog log, LogLevel level, LogArgs args)
		{

            StringBuilder builder = new StringBuilder();
			AppendDateTime(builder);
			builder.Append(InterpunctSep);
			AppendLevelName(builder, level);
			builder.Append(InterpunctSep);
			AppendThreadName(builder);
			builder.Append(InterpunctSep);
			builder.Append(log.Name);

            builder.Append(" ");
            while (builder.Length % 4 != 0)
                builder.Append(" ");
			builder.Append(args.Message);

			// ex?
			if(args.HasException)
			{
				builder.Append("\r\n\t");
				builder.Append(args.Exception);
			}

			// return...
			return builder.ToString();
		}

		internal static void AppendDateTime(StringBuilder builder)
		{
			if(builder == null)
				throw new ArgumentNullException("builder");
			
			// append...
			DateTime now = DateTime.UtcNow;
			builder.Append(now.ToString("yyyyMMdd" + InterpunctSep + "HHmmss"));
		}

		internal static void AppendLevelName(StringBuilder builder, LogLevel level)
		{
			if(builder == null)
				throw new ArgumentNullException("builder");
			
			switch(level)
			{
                case LogLevel.Trace:
                    builder.Append("T");
                    break;

                case LogLevel.Debug:
					builder.Append("D");
					break;

				case LogLevel.Info:
					builder.Append("I");
					break;

				case LogLevel.Warn:
					builder.Append("W");
					break;

				case LogLevel.Error:
					builder.Append("E");
					break;

				case LogLevel.Fatal:
					builder.Append("F");
					break;

				default:
					throw new NotSupportedException(string.Format("Cannot handle '{0}' ({1}).", level, level.GetType()));
			}
		}

		internal static void AppendThreadName(StringBuilder builder)
		{
			if(builder == null)
				throw new ArgumentNullException("builder");
			
            //// append...
            //builder.Append(Runtime.GetCurrentThreadId().ToString("x"));
            //builder.Append(" ");

			// thread...
			Thread thread = Thread.CurrentThread;
			if(thread == null)
				throw new InvalidOperationException("thread is null.");

            // pool?
            if (thread.IsThreadPoolThread)
            {
                builder.Append("<P:");
                builder.Append(Environment.CurrentManagedThreadId);
                builder.Append(">");
            }
            else
            {
                string threadName = Thread.CurrentThread.Name;
                if (threadName == null || threadName.Length == 0)
                    threadName = "(No name)";
                builder.Append(threadName);
            }
		}
	}
}
