// BootFX - Application framework for .NET applications
// 
// File: FileAppender.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.IO;

namespace BootFX.Common.Management
{
	/// <summary>
	/// Summary description for FileAppender.
	/// </summary>
	internal class FileAppender : Appender
	{
		/// <summary>
		/// Private field to support <see cref="Path"/> property.
		/// </summary>
		private string _path;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="path"></param>
		internal FileAppender(ILog log, ILogFormatter formatter, LogLevel minLevel, LogLevel maxLevel, string path) 
            : base(log, formatter, minLevel, maxLevel)
		{
			if(path == null)
				throw new ArgumentNullException("path");
			if(path.Length == 0)
				throw new ArgumentOutOfRangeException("'path' is zero-length.");
			_path = path;
		}

		/// <summary>
		/// Gets the path.
		/// </summary>
		internal string Path
		{
			get
			{
				return _path;
			}
		}

		protected internal override void DoAppend(LogData data)
		{
			if(data == null)
				throw new ArgumentNullException("data");
			
			// open...
			using(FileStream stream = new FileStream(this.Path, FileMode.Append, FileAccess.Write))
			{
				StreamWriter writer = new StreamWriter(stream);
				writer.WriteLine(data.FormattedBuf);
				writer.Flush();
			}
		}
	}
}
