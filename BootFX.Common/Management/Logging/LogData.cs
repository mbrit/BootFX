// BootFX - Application framework for .NET applications
// 
// File: LogData.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Web;
using System.Text;
using System.Security.Principal;
using System.Reflection;
using System.Threading;

namespace BootFX.Common.Management
{
	/// <summary>
	/// Summary description for LogData.
	/// </summary>
	public class LogData
	{
		///// <summary>
		///// Private field to support <c>WindowsUsername</c> property.
		///// </summary>
		//private string _windowsUsername;
		
		/// <summary>
		/// Private field to support <c>DateTime</c> property.
		/// </summary>
		private DateTime _dateTime;
		
		///// <summary>
		///// Private field to support <c>IsHttpRequest</c> property.
		///// </summary>
		//private bool _isHttpRequest;
		
		///// <summary>
		///// Private field to support <see cref="HttpUrl"/> property.
		///// </summary>
		//private string _httpUrl;

		///// <summary>
		///// Private field to support <see cref="HttpQueryString"/> property.
		///// </summary>
		//private string _httpQueryString;
		
		/// <summary>
		/// Private field to support <see cref="ThreadName"/> property.
		/// </summary>
		private string _threadName;

		/// <summary>
		/// Private field to support <see cref="IsThreadPoolThread"/> property.
		/// </summary>
		private bool _isThreadPoolThread;

		///// <summary>
		///// Private field to support <c>EntryAssembly</c> property.
		///// </summary>
		//private Assembly _entryAssembly;
		
		/// <summary>
		/// Private field to support <c>Level</c> property.
		/// </summary>
		private LogLevel _level;

		/// <summary>
		/// Private field to support <see cref="UnformattedBuf"/> property.
		/// </summary>
		private string _unformattedBuf;

		/// <summary>
		/// Private field to support <see cref="Exception"/> property.
		/// </summary>
		private Exception _exception;

		/// <summary>
		/// Private field to support <see cref="FormattedBuf"/> property.
		/// </summary>
		private string _formattedBuf;
		
		/// <summary>
		/// Private field to support <see cref="ThreadId"/> property.
		/// </summary>
		private int _threadId;

        public string System { get; private set; }
        public string Category { get; private set; }

        public bool HasColor { get; private set; }
        public ConsoleColor Color { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="level"></param>
        /// <param name="unformattedBuf"></param>
        /// <param name="exception"></param>
        /// <param name="formattedBuf"></param>
        public LogData(LogLevel level, LogArgs args, string formattedBuf)
		{
			_level = level;
			_unformattedBuf = args.Message;
			_exception = args.Exception;
            this.System = args.System;
            this.Category = args.Category;
			_formattedBuf = formattedBuf;
            this.Color = args.Color;
            this.HasColor = args.HasColor;

			// mbr - 07-11-2005 - other stuff...
			_dateTime = DateTime.UtcNow;
			//_entryAssembly = Assembly.GetEntryAssembly();

			// mbr - 08-02-2008 - changed to use low-trust and shared thread method...		
			if(Runtime.IsStarted && !(Runtime.Current.IsLowTrust))
				_threadId = Runtime.GetCurrentThreadId();
			else
				_threadId = 0;

			// set...
			_isThreadPoolThread = Thread.CurrentThread.IsThreadPoolThread;
			_threadName = GetThreadName();

			//// user...
			//IPrincipal principal = Thread.CurrentPrincipal;
			//if(principal != null && principal.Identity != null && principal.Identity.IsAuthenticated)
			//	_windowsUsername = principal.Identity.Name;

			//// mbr - 07-11-2005 - http...
			//HttpContext context = HttpContext.Current;
			//if(context != null && context.Request != null)
			//{
			//	_isHttpRequest = true;
			//	_httpUrl = context.Request.Url.LocalPath;
			//	_httpQueryString = context.Request.Url.Query;
			//}
		}

		internal static string GetThreadName()
		{
			string threadName = null;
			if(Thread.CurrentThread.IsThreadPoolThread)
				threadName = "<Pool thread>";
			else
			{
				threadName = Thread.CurrentThread.Name;
				if(threadName == null || threadName.Length == 0)
					threadName = "<No name>";
			}

			// return...
			return threadName;
		}

		//public bool IsAuthenticated
		//{
		//	get
		//	{
		//		if(this.WindowsUsername != null && this.WindowsUsername.Length > 0)
		//			return true;
		//		else
		//			return false;
		//	}
		//}

		///// <summary>
		///// Gets the windowsusername.
		///// </summary>
		//public string WindowsUsername
		//{
		//	get
		//	{
		//		// returns the value...
		//		return _windowsUsername;
		//	}
		//}

		/// <summary>
		/// Gets the datetime.
		/// </summary>
		public DateTime DateTime
		{
			get
			{
				// returns the value...
				return _dateTime;
			}
		}

		/// <summary>
		/// Gets the threadid.
		/// </summary>
		public int ThreadId
		{
			get
			{
				return _threadId;
			}
		}

		///// <summary>
		///// Gets the httpquerystring.
		///// </summary>
		//public string HttpQueryString
		//{
		//	get
		//	{
		//		return _httpQueryString;
		//	}
		//}
		
		///// <summary>
		///// Gets the httpurl.
		///// </summary>
		//public string HttpUrl
		//{
		//	get
		//	{
		//		return _httpUrl;
		//	}
		//}

		///// <summary>
		///// Gets the ishttprequest.
		///// </summary>
		//public bool IsHttpRequest
		//{
		//	get
		//	{
		//		// returns the value...
		//		return _isHttpRequest;
		//	}
		//}

		///// <summary>
		///// Gets the entryassembly.
		///// </summary>
		//public Assembly EntryAssembly
		//{
		//	get
		//	{
		//		// returns the value...
		//		return _entryAssembly;
		//	}
		//}

		///// <summary>
		///// Gets the entry assembly version.
		///// </summary>
		//public Version EntryAssemblyVersion
		//{
		//	get
		//	{
		//		if(this.EntryAssembly != null)
		//			return this.EntryAssembly.GetName().Version;
		//		else
		//			return new Version(0,0,0,0);
		//	}
		//}
		
		/// <summary>
		/// Gets the isthreadpoolthread.
		/// </summary>
		public bool IsThreadPoolThread
		{
			get
			{
				return _isThreadPoolThread;
			}
		}
		
		/// <summary>
		/// Gets the threadname.
		/// </summary>
		public string ThreadName
		{
			get
			{
				return _threadName;
			}
		}

		/// <summary>
		/// Gets the formattedbuf.
		/// </summary>
		public string FormattedBuf
		{
			get
			{
				return _formattedBuf;
			}
		}
		
		/// <summary>
		/// Gets the exception.
		/// </summary>
		public Exception Exception
		{
			get
			{
				return _exception;
			}
		}
		
		/// <summary>
		/// Gets the unformattedbuf.
		/// </summary>
		public string UnformattedBuf
		{
			get
			{
				return _unformattedBuf;
			}
		}
		
		/// <summary>
		/// Gets the level.
		/// </summary>
		public LogLevel Level
		{
			get
			{
				// returns the value...
				return _level;
			}
		}

        /// <summary>
        /// Gets the execution metrics as a CRLF separated string.
        /// </summary>
        //public void GetExecutionMetricsAsCrlfSeparatedString(StringBuilder builder)
        //{
        //	if(builder == null)
        //		throw new ArgumentNullException("builder");

        //	try
        //	{
        //		 stuff...
        //		if(Runtime.IsStarted)
        //		{
        //			builder.Append("Application: ");
        //			builder.Append(Runtime.Current.Application.ProductName);
        //			builder.Append("\r\nModule: ");
        //			builder.Append(Runtime.Current.Application.ProductModule);
        //                  builder.Append("\r\nVersion: ");
        //			builder.Append(Runtime.Current.Application.ProductVersion);
        //		}
        //		else
        //			builder.Append("(Runtime not started)");

        //		 other...
        //		builder.Append("\r\nEntry: ");
        //		if(this.EntryAssembly != null)
        //		{
        //			AssemblyName name = this.EntryAssembly.GetName();
        //			builder.Append(name.Name);
        //			builder.Append(" (");
        //			builder.Append(name.Version);
        //			builder.Append(")");
        //		}
        //		else
        //			builder.Append("(Unknown)");
        //		builder.Append("\r\nMachine: ");
        //		builder.Append(Environment.MachineName);
        //		builder.Append("\r\nThread: ");
        //		builder.Append(this.ThreadName);
        //		builder.Append("\r\nIdentity: ");
        //		builder.Append(this.WindowsUsername);
        //		if(this.IsHttpRequest)
        //		{
        //			builder.Append("\r\nURL: ");
        //			builder.Append(this.HttpUrl);
        //			builder.Append("\r\nQuery string: ");
        //			builder.Append(this.HttpQueryString);
        //		}
        //	}
        //	catch(Exception ex)
        //	{
        //		builder.AppendFormat("(Failed to build string: {0} ({1})", ex.Message, ex.GetType());
        //	}		
        //}

        /// <summary>
        /// Gets the execution metrics as a CRLF separated string.
        /// </summary>
        //public string GetExecutionMetricsAsCrlfSeparatedString()
        //{
        //	StringBuilder builder = new StringBuilder();
        //	this.GetExecutionMetricsAsCrlfSeparatedString(builder);

        //	 return...
        //	return builder.ToString();
        //}
    }
}
