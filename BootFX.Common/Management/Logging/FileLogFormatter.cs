// BootFX - Application framework for .NET applications
// 
// File: FileLogFormatter.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;

namespace BootFX.Common.Management
{
	/// <summary>
	/// Defines an instance of <c>FileLogFormatter</c>.
	/// </summary>
	internal class FileLogFormatter : ILogFormatter
	{
		internal FileLogFormatter()
		{
		}

		public string Format(ILog log, BootFX.Common.Management.LogLevel level, LogArgs args)
		{
			StringBuilder builder = new StringBuilder();
			DefaultLogFormatter.AppendDateTime(builder);
			builder.Append(" | ");
			DefaultLogFormatter.AppendLevelName(builder, level);
			builder.Append(" | ");
			DefaultLogFormatter.AppendThreadName(builder);
			builder.Append(" | ");
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
	}
}
