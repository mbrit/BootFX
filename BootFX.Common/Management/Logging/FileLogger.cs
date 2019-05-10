// BootFX - Application framework for .NET applications
// 
// File: FileLogger.cs
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
	/// Defines a logger that logs to a file.
	/// </summary>
	public class FileLog : Log
	{
		/// <summary>
		/// Private field to support <see cref="Path"/> property.
		/// </summary>
		private string _path;
		
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="path"></param>
		internal FileLog(string name, string path) 
            : base(name)
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
		public string Path
		{
			get
			{
				return _path;
			}
		}

		/// <summary>
		/// Reads the contents of the file log.
		/// </summary>
		/// <returns></returns>
		public string ReadToEnd()
		{
			if(Path == null)
				throw new InvalidOperationException("'Path' is null.");
			if(Path.Length == 0)
				throw new InvalidOperationException("'Path' is zero-length.");

			// exists?
			if(File.Exists(this.Path))
			{
				using(StreamReader reader = new StreamReader(this.Path))
					return reader.ReadToEnd();
			}
			else
				return null;
		}

		public static ILogFormatter DefaultFormatter
		{
			get
			{
				return new FileLogFormatter();
			}
		}
	}
}
