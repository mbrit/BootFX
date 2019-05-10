// BootFX - Application framework for .NET applications
// 
// File: LogManager.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Linq;
using System.Reflection;
using System.Diagnostics;
using System.Text;

namespace BootFX.Common.Management
{
	/// <summary>
	/// Summary description for LogManager.
	/// </summary>
	internal class LogManager
	{
		/// <summary>
		/// Private field to support <c>EventLogSourceLookup</c> property.
		/// </summary>
		private static Lookup _eventLogSourceLookup;
		
		/// <summary>
		/// Private field to support <c>AssemblyLogs</c> property.
		/// </summary>
		private static Lookup _assemblyLogs;
		
		/// <summary>
		/// Constructor.
		/// </summary>
		private LogManager()
		{
		}

		/// <summary>
		/// Static constructor.
		/// </summary>
		static LogManager()
		{
			// setup the lookup...
			_assemblyLogs = new Lookup();
			_assemblyLogs.CreateItemValue += new CreateLookupItemEventHandler(_assemblyLogs_CreateItemValue);

			// mbr - 03-11-2005 - event log lookup...
			_eventLogSourceLookup = new Lookup();
			_eventLogSourceLookup.CreateItemValue += new CreateLookupItemEventHandler(_eventLogSourceLookup_CreateItemValue);
		}

        ///// <summary>
        ///// Gets the defaultloglevel.
        ///// </summary>
        //private static LogLevel DefaultLogLevel
        //{
        //    get
        //    {
        //        // returns the value...
        //        if(Debugger.IsAttached)
        //        {
        //            // if we have a debugger, we're info, unless we've specified debug...
        //            if(_defaultLogLevel != LogLevel.Debug)
        //                return LogLevel.Info;
        //            else
        //                return _defaultLogLevel;
        //        }
        //        else
        //            return _defaultLogLevel;
        //    }
        //}

		/// <summary>
		/// Gets a logger.
		/// </summary>
		/// <param name="asm"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		internal static ILog GetLogger(Type type)
		{
			return GetLogger(type, null);
		}

		/// <summary>
		/// Gets a logger.
		/// </summary>
		/// <param name="asm"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		internal static ILog GetLogger(Type type, string name)
		{
			if(type == null)
				throw new ArgumentNullException("type");

            // type...
            var typeName = type.Name;
            var args = type.GetGenericArguments();
            if (args.Any())
            {
                var index = typeName.IndexOf('`');
                if (index != -1)
                {
                    var builder = new StringBuilder();
                    builder.Append(typeName.Substring(0, index));

                    // then...
                    bool first = true;
                    foreach (var arg in args)
                    {
                        if (first)
                        {
                            builder.Append("-");
                            first = false;
                        }
                        else
                            builder.Append(",");
                        builder.Append(arg.Name);
                    }

                    // set...
                    typeName = builder.ToString();
                }
            }

			// name...
			if(name == null || name.Length == 0)
				name = typeName;
			else
				name = string.Format("{0}:{1}", typeName, name);

			// get the first one...
			Lookup asmLookup = (Lookup)AssemblyLogs[type.Assembly];
			if(asmLookup == null)
				throw new InvalidOperationException("asmLookup is null.");

			// get the next one...
			return (ILog)asmLookup[name];
		}

		/// <summary>
		/// Creates the default logger.
		/// </summary>
		/// <returns></returns>
		private static ILog CreateDefaultLog(string name)
		{
			Log log = new Log(name);
			log.Appenders.Add(new ConsoleAppender(log, new DefaultLogFormatter(), ConsoleAppender.DefaultLogLevel, LogLevel.Fatal));

			// return...
			return log;
		}

		/// <summary>
		/// Gets the assemblylogs.
		/// </summary>
		private static Lookup AssemblyLogs
		{
			get
			{
				// returns the value...
				return _assemblyLogs;
			}
		}

        internal static void Reset()
        {
            _assemblyLogs.Clear();
        }

		private static void _assemblyLogs_CreateItemValue(object sender, CreateLookupItemEventArgs e)
		{
			// create a new lookup...
			Lookup namesLookup = new Lookup();
			namesLookup.CreateItemValue += new CreateLookupItemEventHandler(namesLookup_CreateItemValue);

			// set...
			e.NewValue = namesLookup;
		}

		private static void namesLookup_CreateItemValue(object sender, CreateLookupItemEventArgs e)
		{
			// create...
			ILog log = CreateDefaultLog((string)e.Key);
			if(log == null)
				throw new InvalidOperationException("log is null.");

			// raise...
			if(Runtime.IsStarted)
			{
				// mbr - 03-11-2005 - we can do some standard config here...
				ConfigureLogFromInstallationSettings(log);

				// defer...
				Runtime.Current.OnLogCreated(new ILogEventArgs(log));
			}

			// set...
			e.NewValue = log;
		}

		/// <summary>
		/// Configures a log object from installation settings.
		/// </summary>
		private static void ConfigureLogFromInstallationSettings(ILog log)
		{
			if(log == null)
				throw new ArgumentNullException("log");
			
			// mbr - 03-11-2005 - check in the installation settings - do we want to do event log and e-mail error reporting?
			InstallationSettings settings = Runtime.Current.InstallationSettings;
			if(settings == null)
				throw new InvalidOperationException("settings is null.");

			// event log...?
			if(settings.DoEventLogReporting)
			{
				// name...
				string sourceName = settings.EventLogSourceName;
				if(sourceName == null)
					throw new InvalidOperationException("'sourceName' is null.");
				if(sourceName.Length == 0)
					throw new InvalidOperationException("'sourceName' is zero-length.");

				// have we tried to create this already?
				bool ok = (bool)EventLogSourceLookup[sourceName];
				if(ok)
					log.Appenders.Add(new EventLogAppender(log, new EventLogFormatter(), 
                        settings.EventLogLevel, LogLevel.Fatal, sourceName));
			}

			// mbr - 10-08-2006 - deprecated.			
//			// how about e-mail reporting...?
//			if(settings.DoEmailReporting && settings.IsEmailReportingProperlyConfigured)
//			{
//				// set...
//				log.Appenders.Add(new EmailAppender(log, new EmailFormatter(), settings.EmailReportingLevel, settings.EmailReportingHostName, settings.EmailReportingUsername, 
//					settings.EmailReportingPassword, settings.EmailReportingFrom, settings.EmailReportingTo));
//			}

			// mbr - 07-11-2005 - finally, database...
            // mbr - 2009-12-18 - deprecated!
            //if(settings.DoDatabaseLogging)
            //{
            //    log.Appenders.Add(new DatabaseAppender(log, DatabaseAppender.DefaultFormatter, settings.DatabaseLoggingLevel, settings.GetConnectionSettingsForLogging(), 
            //        settings.DatabaseLoggingTableNativeName));
            //}

			// mbr - 15-05-2007 - removed.			
//			if(settings.DoPhoneHomeLogging)
//				log.Appenders.Add(new PhoneHomeAppender(log, PhoneHomeAppender.DefaultFormatter, LogLevel.Error, settings.PhoneHomeUrl, settings.PhoneHomeKey));
		}

		/// <summary>
		/// Gets the eventlogsourcelookup.
		/// </summary>
		private static Lookup EventLogSourceLookup
		{
			get
			{
				// returns the value...
				return _eventLogSourceLookup;
			}
		}

		/// <summary>
		/// Raised when there is an error with the logging system itself.
		/// </summary>
		/// <param name="message"></param>
		/// <param name="ex"></param>
		internal static void InternalError(string message, Exception ex)
		{
			string debugMessage = string.Format("Internal logging error: {0}\r\n\t{1}", message, ex);

			// mjr - 15-06-2005 - turned off internal errors...
			
//			// output...
//			ILog log = GetLogger(typeof(LogManager));
//			if(log != null && log.IsErrorEnabled)
//				log.Error(debugMessage);
			
			// "always on"...
			Debug.WriteLine(debugMessage);
		}

		private static void _eventLogSourceLookup_CreateItemValue(object sender, CreateLookupItemEventArgs e)
		{
			string name = (string)e.Key;

			// try it...
			try
			{
				if(EventLog.SourceExists(name))
					e.NewValue = true;
				else
				{
					// create...
					EventLog.CreateEventSource(name, name);

					// set...  if we get here we can do it...
					e.NewValue = true;
				}
			}
			catch(Exception ex)
			{
				// no-op...
				Console.WriteLine(ex);

				// nope...
				e.NewValue = false;
			}
		}
	}
}
