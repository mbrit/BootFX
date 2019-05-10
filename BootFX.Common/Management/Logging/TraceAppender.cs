// BootFX - Application framework for .NET applications
// 
// File: TraceAppender.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Diagnostics;

namespace BootFX.Common.Management
{
	/// <summary>
	/// Describes a class that appends to <see cref="Console"></see>.
	/// </summary>
	public class ConsoleAppender : Appender
	{
        public static ConsoleLogMode Mode { get; internal set; }
        public static LogLevel DefaultLogLevel { get; set; }

		/// <summary>
		/// Constructor.
		/// </summary>
		public ConsoleAppender(ILog log, ILogFormatter formatter, LogLevel minLevel, LogLevel maxLevel) 
            : base(log, formatter, minLevel, maxLevel)
		{
		}

        static ConsoleAppender()
        {
            Mode = ConsoleLogMode.Console;
            DefaultLogLevel = LogLevel.Debug;
        }

        protected internal override void DoAppend(LogData data)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            if (Mode == ConsoleLogMode.Console)
            {
                var oldColor = ConsoleColor.Gray;
                if (data.HasColor)
                {
                    oldColor = Console.ForegroundColor;
                    Console.ForegroundColor = data.Color;
                }
                try
                {
                    Console.WriteLine(data.FormattedBuf);
                }
                finally
                {
                    if (data.HasColor)
                        Console.ForegroundColor = oldColor;
                }
            }
            else if (Mode == ConsoleLogMode.Debug)
                Debug.WriteLine(data.FormattedBuf);
            else if (Mode == ConsoleLogMode.Trace)
                Trace.WriteLine(data.FormattedBuf);
            else if (Mode == ConsoleLogMode.Off)
            {
                // no-op...
            }
            else
                throw new NotSupportedException(string.Format("Cannot handle '{0}'.", Mode));
        }

        public static ILogFormatter DefaultFormatter
		{
			get
			{
				return new DefaultLogFormatter();
			}
		}
    }
}
