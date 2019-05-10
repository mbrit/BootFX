// BootFX - Application framework for .NET applications
// 
// File: FileLoggerFlags.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using BootFX.Common;
using BootFX.Common.Data;

namespace BootFX.Common.Management
{
	/// <summary>
	/// Summary description for FileLoggerFlags.
	/// </summary>
	[Flags()]
	public enum FileLoggerFlags
	{
		/// <summary>
		/// Defines a file logger than appends an existing file.
		/// </summary>
		AppendExisting = 0,

		/// <summary>
		/// Defines a logger that replaced an existing file.
		/// </summary>
		ReplaceExisting = 1,

		/// <summary>
		/// Adds the date in yyyyMMdd format to the file.
		/// </summary>
		AddDateToFileName = 2,

		/// <summary>
		/// Creates a new file each time.
		/// </summary>
		// mbr - 28-07-2006 - added.		
		EnsureNewFile = 4,

		/// <summary>
		/// Echos log output to the debugger.
		/// </summary>
		EchoToDebug = 8,

		/// <summary>
		/// Puts the log files into their own folders.
		/// </summary>
		OwnFolder = 16,

		// mbr - 04-07-2007 - added.
		/// <summary>
		/// Creates default file log options.
		/// </summary>
		Default = AddDateToFileName | EnsureNewFile | OwnFolder | EchoToDebug
	}
}
