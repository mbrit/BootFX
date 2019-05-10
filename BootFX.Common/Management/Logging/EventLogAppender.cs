// BootFX - Application framework for .NET applications
// 
// File: EventLogAppender.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Collections.Generic;

namespace BootFX.Common.Management
{
	/// <summary>
	/// Summary description for EventLogAppender.
	/// </summary>
	public class EventLogAppender : Appender
	{
        private static long _messageNumber = 0;
        private static object _cleanupLock = new object();
        private const int MaxMessageFiles = 1000;
        private const int MessageExpiryDays = 30;

		/// <summary>
		/// Private field to support <c>SourceName</c> property.
		/// </summary>
		private string _sourceName;
		
		/// <summary>
		/// Constructor.
		/// </summary>
		public EventLogAppender(ILog log, ILogFormatter formatter, LogLevel minLevel, LogLevel maxLevel, string sourceName) 
            : base(log, formatter, minLevel, maxLevel)
		{
			if(sourceName == null)
				throw new ArgumentNullException("sourceName");
			if(sourceName.Length == 0)
				throw new ArgumentOutOfRangeException("'sourceName' is zero-length.");

			if(sourceName.Length > 8)
				_sourceName = sourceName.Substring(0, 8);
			else
				_sourceName = sourceName;
		}

		/// <summary>
		/// Gets the sourcename.
		/// </summary>
		private string SourceName
		{
			get
			{
				// returns the value...
				return _sourceName;
			}
		}

		protected internal override void DoAppend(LogData data)
		{
			if(data == null)
				throw new ArgumentNullException("data");
			
			// check...
			if(SourceName == null)
				throw new InvalidOperationException("'SourceName' is null.");
			if(SourceName.Length == 0)
				throw new InvalidOperationException("'SourceName' is zero-length.");

			// get the type...
			EventLogEntryType type;
			switch(data.Level)
			{
				case LogLevel.Debug:
				case LogLevel.Info:
					type = EventLogEntryType.Information;
					break;

				case LogLevel.Warn:
					type = EventLogEntryType.Warning;
					break;

				case LogLevel.Error:
				case LogLevel.Fatal:
					type = EventLogEntryType.Error;
					break;

				default:
					throw new NotSupportedException(string.Format("Cannot handle '{0}' ({1}).", data.Level, data.Level.GetType()));
			}

			// mbr - 15-11-2005 - get the body...
			StringBuilder builder = new StringBuilder();
			//data.GetExecutionMetricsAsCrlfSeparatedString(builder);
			//builder.Append("\r\n\r\n");
			builder.Append(data.FormattedBuf);
			string body = builder.ToString();

			// append...
			AppendInternal(SourceName, body, type, true);
		}

		/// <summary>
		/// Ensures the given event source exists.
		/// </summary>
		/// <param name="sourceName"></param>
		private static void EnsureSourceExists(string sourceName)
		{
			if(sourceName == null)
				throw new ArgumentNullException("sourceName");
			if(sourceName.Length == 0)
				throw new ArgumentOutOfRangeException("'sourceName' is zero-length.");
			
			// ensure...
			if(!(EventLog.SourceExists(sourceName)))
				EventLog.CreateEventSource(sourceName, sourceName);
		}

		/// <summary>
		/// Forces a message append, using the Runtime's default source name.
		/// </summary>
		/// <param name="sourceName"></param>
		/// <param name="message"></param>
		/// <param name="type"></param>
		/// <param name="throwOnError"></param>
		// mbr - 10-05-2006 - added.		
		public static void AppendSuccessAudit(string message, bool throwOnError)
		{
			Append(message, EventLogEntryType.SuccessAudit, throwOnError);
		}

		/// <summary>
		/// Forces a message append, using the Runtime's default source name.
		/// </summary>
		/// <param name="sourceName"></param>
		/// <param name="message"></param>
		/// <param name="type"></param>
		/// <param name="throwOnError"></param>
		// mbr - 10-05-2006 - added.		
		public static void AppendSuccessAudit(string sourceName, string message, bool throwOnError)
		{
			Append(sourceName, message, EventLogEntryType.SuccessAudit, throwOnError);
		}

		/// <summary>
		/// Forces a message append, using the Runtime's default source name.
		/// </summary>
		/// <param name="sourceName"></param>
		/// <param name="message"></param>
		/// <param name="type"></param>
		/// <param name="throwOnError"></param>
		// mbr - 10-05-2006 - added.		
		public static void AppendFailureAudit(string message, bool throwOnError)
		{
			Append(message, EventLogEntryType.FailureAudit, throwOnError);
		}

		/// <summary>
		/// Forces a message append, using the Runtime's default source name.
		/// </summary>
		/// <param name="sourceName"></param>
		/// <param name="message"></param>
		/// <param name="type"></param>
		/// <param name="throwOnError"></param>
		// mbr - 10-05-2006 - added.		
		public static void AppendFailureAudit(string sourceName, string message, bool throwOnError)
		{
			Append(sourceName, message, EventLogEntryType.FailureAudit, throwOnError);
		}

		/// <summary>
		/// Forces a message append, using the Runtime's default source name.
		/// </summary>
		/// <param name="sourceName"></param>
		/// <param name="message"></param>
		/// <param name="type"></param>
		/// <param name="throwOnError"></param>
		// mbr - 10-05-2006 - added.		
		public static void AppendError(string message, bool throwOnError)
		{
			Append(message, EventLogEntryType.Error, throwOnError);
		}

		/// <summary>
		/// Forces a message append, using the Runtime's default source name.
		/// </summary>
		/// <param name="sourceName"></param>
		/// <param name="message"></param>
		/// <param name="type"></param>
		/// <param name="throwOnError"></param>
		// mbr - 10-05-2006 - added.		
		public static void AppendError(string sourceName, string message, bool throwOnError)
		{
			Append(sourceName, message, EventLogEntryType.Error, throwOnError);
		}

		/// <summary>
		/// Forces a message append, using the Runtime's default source name.
		/// </summary>
		/// <param name="sourceName"></param>
		/// <param name="message"></param>
		/// <param name="type"></param>
		/// <param name="throwOnError"></param>
		// mbr - 10-05-2006 - added.		
		public static void AppendWarn(string message, bool throwOnError)
		{
			Append(message, EventLogEntryType.Warning, throwOnError);
		}

		/// <summary>
		/// Forces a message append, using the Runtime's default source name.
		/// </summary>
		/// <param name="sourceName"></param>
		/// <param name="message"></param>
		/// <param name="type"></param>
		/// <param name="throwOnError"></param>
		// mbr - 10-05-2006 - added.		
		public static void AppendWarn(string sourceName, string message, bool throwOnError)
		{
			Append(sourceName, message, EventLogEntryType.Warning, throwOnError);
		}

		/// <summary>
		/// Forces a message append, using the Runtime's default source name.
		/// </summary>
		/// <param name="sourceName"></param>
		/// <param name="message"></param>
		/// <param name="type"></param>
		/// <param name="throwOnError"></param>
		// mbr - 10-05-2006 - added.		
		public static void AppendInfo(string message, bool throwOnError)
		{
			Append(message, EventLogEntryType.Information, throwOnError);
		}

		/// <summary>
		/// Forces a message append, using the Runtime's default source name.
		/// </summary>
		/// <param name="sourceName"></param>
		/// <param name="message"></param>
		/// <param name="type"></param>
		/// <param name="throwOnError"></param>
		// mbr - 10-05-2006 - added.		
		public static void AppendInfo(string sourceName, string message, bool throwOnError)
		{
			Append(sourceName, message, EventLogEntryType.Information, throwOnError);
		}

		/// <summary>
		/// Forces a message append, using the Runtime's default source name.
		/// </summary>
		/// <param name="sourceName"></param>
		/// <param name="message"></param>
		/// <param name="type"></param>
		/// <param name="throwOnError"></param>
		// mbr - 10-05-2006 - added.		
		public static void Append(string message, EventLogEntryType type, bool throwOnError)
		{
			AppendInternal(null, message, type, throwOnError);
		}

		public static void Append(string sourceName, string message, EventLogEntryType type, bool throwOnError)
		{
			// mbr - 10-05-2006 - moved error handling.			
//			try
//			{
				AppendInternal(sourceName, message, type, throwOnError);
//			}
//			catch(Exception ex)
//			{
//				// no-op...
//				if(throwOnError)
//					throw new InvalidOperationException(string.Format("Failed to write '{0}' message to '{1}'.", type, sourceName), ex);
//			}
		}

		// mbr - 10-05-2006 - added throwOnError.
		private static void AppendInternal(string sourceName, string message, EventLogEntryType type, bool throwOnError)
		{
			try
			{
				// mbr - 10-05-2006 - sourceName now optional.
//				if(sourceName == null)
//					throw new ArgumentNullException("sourceName");
//				if(sourceName.Length == 0)
//					throw new ArgumentOutOfRangeException("'sourceName' is zero-length.");
				if(message == null)
					throw new ArgumentNullException("message");
				if(message.Length == 0)
					throw new ArgumentOutOfRangeException("'message' is zero-length.");

				// mbr - 10-05-2006 - added default.
				if(sourceName == null || sourceName.Length == 0)
				{
					sourceName = Runtime.Current.InstallationSettings.EventLogSourceName;
					if(sourceName == null)
						throw new InvalidOperationException("'sourceName' is null.");
					if(sourceName.Length == 0)
						throw new InvalidOperationException("'sourceName' is zero-length.");
				}

				// ensure...
				EnsureSourceExists(sourceName);

				// add...
				EventLog.WriteEntry(sourceName, message, type);
			}
			catch(Exception ex)
			{
                SecondChance(sourceName, message, type, ex);
			}
		}

        private static void SecondChance(string sourceName, string message, EventLogEntryType type, Exception writeError)
        {
            try
            {
                if (!(Runtime.IsStarted))
                    return;

                // message...
                long number = Interlocked.Increment(ref _messageNumber);

                // where...
                string path = Runtime.Current.InstallationSettingsFolderPath;
                if (!(Directory.Exists(path)))
                    Directory.CreateDirectory(path);

                // check...
                if(number - 1 % 1000 == 0)
                    CheckExpiredMessages(path);

                // create a filename...
                string filename = string.Format("Event - {0} - {1} - {2} - {3}.txt", DateTime.UtcNow.ToString("yyyyMMdd HHmmss"), sourceName, type, 
                    number);
                path = Path.Combine(path, filename);

                // create...
                using (StreamWriter writer = new StreamWriter(path))
                {
                    writer.WriteLine(message);

                    // if...
                    if (writeError != null)
                    {
                        writer.WriteLine("-----------------");
                        writer.WriteLine(writeError);
                    }
                }
            }
            catch 
            {
                // no-op...
            }
        }

        private static void CheckExpiredMessages(string path)
        {
            if (path == null)
                throw new ArgumentNullException("path");
            if (path.Length == 0)
                throw new ArgumentException("'path' is zero-length.");

            lock (_cleanupLock)
            {
                Debug.WriteLine("EVENT FILE CLEANUP: Starting...");

                // get them...
                List<FileInfo> files = new List<FileInfo>(new DirectoryInfo(path).GetFiles("Event - *.txt"));
                Debug.WriteLine(string.Format("EVENT FILE CLEANUP: {0} file(s) found.", files.Count));
                files.Sort(new ExpiredComparer());

                // delete...
                int index = 0;
                int originalCount = files.Count;
                while(files.Count > MaxMessageFiles)
                {
                    files[0].Delete();
                    files.RemoveAt(0);

                    index++;
                    if (index % 500 == 0)
                        Debug.WriteLine(string.Format("EVENT FILE CLEANUP: Deleted {0} of {1} 'anytime' messages...", index + 1, originalCount));
                }

                // old?
                DateTime threshhold = DateTime.UtcNow.AddDays(0 - MessageExpiryDays);
                index = 0;
                foreach(FileInfo file in files)
                {
                    if (file.LastWriteTimeUtc < threshhold)
                        file.Delete();

                    // next..
                    index++;

                    if(index % 500 == 0)
                        Debug.WriteLine(string.Format("EVENT FILE CLEANUP: Deleted {0} 'expired' messages...", index + 1));
                }

                Debug.WriteLine(string.Format("FILE CLEANUP: Finished. {0} expired file(s) deleted.", index));
            }
        }

        private class ExpiredComparer : IComparer<FileInfo>
        {
            public int Compare(FileInfo x, FileInfo y)
            {
                if (x.LastWriteTimeUtc < y.LastWriteTimeUtc)
                    return -1;
                else if (x.LastWriteTimeUtc > y.LastWriteTimeUtc)
                    return 1;
                else
                    return 0;
            }
        }
	}
}
