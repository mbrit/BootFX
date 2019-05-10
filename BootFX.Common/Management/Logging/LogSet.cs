// BootFX - Application framework for .NET applications
// 
// File: LogSet.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.IO;
using System.Threading;
using System.Collections;
using System.Collections.Specialized;

namespace BootFX.Common.Management
{
	/// <summary>
	/// Provides access to a set of named logs.
	/// </summary>
	public class LogSet : ILogSet
	{
		/// <summary>
		/// Private field to support <see cref="ContextId"/> property.
		/// </summary>
		private long _contextId;
		
		private static long _currentContextId = 0;

		private static Lookup _cleanupCount = null;
		private const int MaxLogFiles = 500;
		private static object _fileLogLock = new object();

		/// <summary>
		/// Private field to support <c>Logs</c> property.
		/// </summary>
		private HybridDictionary _logs = new HybridDictionary();
		
		/// <summary>
		/// Private field to support <c>Type</c> property.
		/// </summary>
		private Type _type;
		
		/// <summary>
		/// Private field to support <c>Default</c> property.
		/// </summary>
		private ILog _defaultLog;

		/// <summary>
		/// Private field to support <c>ContextSlot</c> property.
		/// </summary>
		private static LocalDataStoreSlot _contextSlot;
		
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="type"></param>
		public LogSet(Type type)
		{
			if(type == null)
				throw new ArgumentNullException("type");
			
			_type = type;
			_contextId = CurrentContextId;
		}

		static LogSet()
		{
			_contextSlot = Thread.AllocateDataSlot();

			// mbr - 27-06-2007 - log folder cleanup
			_cleanupCount = new Lookup();
			_cleanupCount.CreateItemValue += new CreateLookupItemEventHandler(_cleanupCount_CreateItemValue);
		}

		/// <summary>
		/// Gets the contextslot.
		/// </summary>
		private static LocalDataStoreSlot ContextSlot
		{
			get
			{
				// returns the value...
				return _contextSlot;
			}
		}

		/// <summary>
		/// Gets the default log.
		/// </summary>
		public ILog DefaultLog
		{
			get
			{
				if(_defaultLog == null)
					_defaultLog = GetLog(this.Type, null);
				return _defaultLog;
			}
		}

		/// <summary>
		/// Gets the log for the given activity.
		/// </summary>
		public ILog this[string activity]
		{
			get
			{
				if(activity == null || activity.Length == 0)
					return this.DefaultLog;

				// key...
				string key = activity.ToLower(Cultures.System);
				ILog log = (ILog)this.Logs[key];
				if(log == null)
				{
					// create and set...
					log = GetLog(this.Type, activity);
					this.Logs[key] = log;
				}

				// return...
				return log;
			}
		}

		/// <summary>
		/// Gets the type.
		/// </summary>
		public Type Type
		{
			get
			{
				return _type;
			}
		}

		/// <summary>
		/// Gets the logs.
		/// </summary>
		private HybridDictionary Logs
		{
			get
			{
				// returns the value...
				return _logs;
			}
		}

        public static ILog GetLog<T>(string activity = null)
        {
            return GetLog(typeof(T), activity);
        }

			
		/// <summary>
		/// Gets the log for the given type and activity.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="activity"></param>
		/// <returns></returns>
		public static ILog GetLog(Type type, string activity = null)
		{
			if(type == null)
				throw new ArgumentNullException("type");

			// mbr - 01-09-2005 - if we have a bound log, always return that...
			if(ContextBoundLog != null)
				return ContextBoundLog.CurrentLog;
			
			// get...
			if(activity == null || activity.Length == 0)
				return LogManager.GetLogger(type);
			else
				return LogManager.GetLogger(type, activity);
		}

		/// <summary>
		/// Gets the string represenation of an object for logging.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public static string ToString(object obj)
		{
			if(obj == null)
				return "(CLR null)";
			if(obj is DBNull)
				return "(DB null)";

			// string...
			if(obj is string)
			{
				string buf = (string)obj;
				if(buf.Length > 32)
					buf = buf.Substring(0, 32) + "...";
				return string.Format(Cultures.Log, "{0} (System.String, {1} char(s))", buf, buf.Length);
			}
			else
			{
				Type type = obj.GetType();
				string objAsString = obj.ToString();
				if(objAsString == type.FullName)
					return objAsString;
				else
					return string.Format(Cultures.Log, "{0} ({1})", objAsString, type);
			}
		}

		/// <summary>
		/// Formats a length into bytes.
		/// </summary>
		/// <param name="length"></param>
		/// <param name="alwaysKb">If <c>true</c>, the value will always be expressed as KB.  If <c>false</c>,
		/// the value will be - for example - 1.23MB, 20.6MB, 10,000MB, etc.</param>
		/// <returns></returns>
		public static string FormatBytes(long length, bool alwaysKb)
		{
			if(alwaysKb)
			{
				if(length <= 1024)
					return "1KB";
				else
					return (length / 1024).ToString("n0") + "KB";
			}
			else
			{
				if(length < 1024)
					return length + " bytes";
				else if(length < 1024 * 1024)
					return (length / 1024).ToString("n0") + "KB";
				else if(length < (1024 * 1024) * 10)
					return (length / (1024 * 1024)).ToString("n2") + "MB";
				else if(length < (1024 * 1024) * 100)
					return (length / (1024 * 1024)).ToString("n1") + "MB";
				else
					return (length / (1024 * 1024)).ToString("n0") + "MB";
			}
		}

		/// <summary>
		/// Creates a file logger with the given name.
		/// </summary>
		/// <param name="name"></param>
		public static FileLog CreateFileLogger(string name, FileLoggerFlags flags)
		{
			// defer...
			return CreateFileLogger(name, flags, null);
		}

		/// <summary>
		/// Creates a file logger with the given name.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="formatter">Specify the formatter, or <c>null</c> to use the default formatter.</param>
		public static FileLog CreateFileLogger(string name, FileLoggerFlags flags, ILogFormatter formatter)
		{
			return CreateFileLogger(name, flags, formatter, LogLevel.Debug, LogLevel.Fatal);
		}

		/// <summary>
		/// Creates a file logger with the given name.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="formatter">Specify the formatter, or <c>null</c> to use the default formatter.</param>
		public static FileLog CreateFileLogger(string name, FileLoggerFlags flags, ILogFormatter formatter, 
            LogLevel minLevel, LogLevel maxLevel)
		{
			if(name == null || name.Length == 0)
				name = "Log";

			// mbr - 27-06-2007 - missed the lock...
			lock(_fileLogLock)
			{
				// get the file path...
				string path = GetFileLoggerPath(name, flags);
				if(path == null)
					throw new InvalidOperationException("'path' is null.");
				if(path.Length == 0)
					throw new InvalidOperationException("'path' is zero-length.");

				// folder...
				string folderPath = Path.GetDirectoryName(path);
				if(!(Directory.Exists(folderPath)))
					Directory.CreateDirectory(folderPath);

				// mbr - 28-07-2006 - create new,.,.
				if((flags & FileLoggerFlags.EnsureNewFile) != 0)
				{
					path = Runtime.Current.GetUniqueFilePathInFolder(path);
					if(path == null)
						throw new InvalidOperationException("'path' is null.");
					if(path.Length == 0)
						throw new InvalidOperationException("'path' is zero-length.");
				}
				else
				{
					// replace?
					if((flags & FileLoggerFlags.ReplaceExisting) != 0 && File.Exists(path))
					{
						try
						{
							File.Delete(path);
						}
						catch
						{
							// no-op the exception...
						}
					}
				}

				// formatter...
				if(formatter == null)
					formatter = FileLog.DefaultFormatter;

				// mbr - 27-06-2007 - cleanup?  only on first use of the log, or if we have used it a lot.
				int count = (int)CleanupCount[name];
				if(count % (MaxLogFiles * 2) == 0)
					CleanupLogFolder(name, path);

				// increment...
				CleanupCount[name] = count + 1;

				// create...
				FileLog log = new FileLog(name, path);
				log.Appenders.Add(new FileAppender(log, formatter, minLevel, maxLevel, path));

				// mbr - 28-11-2006 - screen?
				if((int)(flags & FileLoggerFlags.EchoToDebug) != 0)
                    log.Appenders.Add(new ConsoleAppender(log, formatter, minLevel, maxLevel));

				// return...
				return log;
			}
		}

		/// <summary>
		/// Cleans up obsolete logs.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="path"></param>
		private static void CleanupLogFolder(string name, string path)
		{
			if(name == null)
				throw new ArgumentNullException("name");
			if(name.Length == 0)
				throw new ArgumentOutOfRangeException("'name' is zero-length.");
			if(path == null)
				throw new ArgumentNullException("path");
			if(path.Length == 0)
				throw new ArgumentOutOfRangeException("'path' is zero-length.");
			
			// don't fail this method...
			try
			{
				// get the folder...
				string folderPath = Path.GetDirectoryName(path);
				if(folderPath == null)
					throw new InvalidOperationException("'folderPath' is null.");
				if(folderPath.Length == 0)
					throw new InvalidOperationException("'folderPath' is zero-length.");

				// select...
				ArrayList existingFiles = new ArrayList(new DirectoryInfo(folderPath).GetFiles(name + "*.log"));

				// sort...
				existingFiles.Sort(new FileInfoDateTimeComparer());

				// walk...
				while(existingFiles.Count >= MaxLogFiles)
				{
					// delete the one at the end...
					FileInfo file = (FileInfo)existingFiles[0];
					Runtime.Current.SafeDelete(file.FullName);

					// remove...
					existingFiles.RemoveAt(0);
				}
			}
			catch
			{
				// no-op...
			}
		}

		private class FileInfoDateTimeComparer : IComparer
		{
			internal FileInfoDateTimeComparer()
			{
			}

			public int Compare(object x, object y)
			{
				FileInfo f1 = (FileInfo)x;
				FileInfo f2 = (FileInfo)y;

				// if..
				if(f1.LastWriteTimeUtc < f2.LastWriteTimeUtc)
					return -1;
				if(f1.LastWriteTimeUtc > f2.LastWriteTimeUtc)
					return 1;
				else
					return 0;
			}
		}

		/// <summary>
		/// Gets the logger path for this file.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="flags"></param>
		/// <returns></returns>
		private static string GetFileLoggerPath(string name, FileLoggerFlags flags)
		{
			if(name == null)
				throw new ArgumentNullException("name");
			if(name.Length == 0)
				throw new ArgumentOutOfRangeException("'name' is zero-length.");
			
			// get the path...
			string path = Runtime.Current.LogsFolderPath;

			// mbr - 2007-03-19 - support own folder?
			if((int)(flags & FileLoggerFlags.OwnFolder) != 0)
				path = Path.Combine(path, name);

			// mbr - 11-10-2005 - add the date...
			if((flags & FileLoggerFlags.AddDateToFileName) != 0)
				name = name + " - " + DateTime.Now.ToString("yyyyMMdd");

			// add...
			path = Path.Combine(path, name + ".log");

			// return...
			return path;
		}

		/// <summary>
		/// Binds a log object to the thread context.
		/// </summary>
		/// <param name="log"></param>
		public static void BindToContext(ILog log)
		{
			if(log == null)
				throw new ArgumentNullException("log");
			
			// get the existing context...
			if(ContextBoundLog == null)
				Thread.SetData(ContextSlot, new ContextBoundLog());

			// bind...
			if(ContextBoundLog == null)
				throw new InvalidOperationException("ContextBoundLog is null.");
			ContextBoundLog.Bind(log);

			// mbr - 11-10-2005 - reset the context id.  (to cause all logs to be recreated)			
			IncrementContextId();
		}

		internal static ContextBoundLog ContextBoundLog
		{
			get
			{
				if(ContextSlot == null)
					throw new InvalidOperationException("ContextSlot is null.");
				return (ContextBoundLog)Thread.GetData(ContextSlot);
			}
		}

		/// <summary>
		/// Unbinds a log object from the thread context.
		/// </summary>
		/// <param name="log"></param>
		public static void UnbindFromContext(ILog log)
		{
			if(log == null)
				throw new ArgumentNullException("log");
			
			// context?
			if(ContextBoundLog == null)
				throw new InvalidOperationException("No logs are bound to context.");

			// remove...
			if(ContextBoundLog.Unbind(log))
			{
				if(ContextSlot == null)
					throw new InvalidOperationException("ContextSlot is null.");
				Thread.SetData(ContextSlot, null);
			}

			// mbr - 11-10-2005 - reset the context id.  (to cause all logs to be recreated)			
			IncrementContextId();
		}

		/// <summary>
		/// Increments the context it.
		/// </summary>
		internal static void IncrementContextId()
		{
			Interlocked.Increment(ref _currentContextId);
		}

		/// <summary>
		/// Gets the context ID.
		/// </summary>
		public static long CurrentContextId
		{
			get
			{
				return _currentContextId;
			}
		}

		/// <summary>
		/// Gets the contextid.
		/// </summary>
		public long ContextId
		{
			get
			{
				return _contextId;
			}
		}

		/// <summary>
		/// Gets the cleanupcount.
		/// </summary>
		internal static Lookup CleanupCount
		{
			get
			{
				return _cleanupCount;
			}
		}

		private static void _cleanupCount_CreateItemValue(object sender, CreateLookupItemEventArgs e)
		{
			e.NewValue = 0;
		}

		/// <summary>
		/// Gets the log levels that relate to audits.
		/// </summary>
		/// <returns></returns>
		public static LogLevel[] GetAuditLogLevels()
		{
			return new LogLevel[] { LogLevel.AuditPending, LogLevel.AuditSuccess, 
						LogLevel.AuditFailed};
		}

		/// <summary>
		/// Gets the log levels that do not relate to audits.
		/// </summary>
		/// <returns></returns>
		public static LogLevel[] GetNonAuditLogLevels()
		{
			return new LogLevel[] { LogLevel.Debug, LogLevel.Info, LogLevel.Warn, LogLevel.Error, 
						LogLevel.Fatal, LogLevel.PhoneHome};
		}
	}
}

