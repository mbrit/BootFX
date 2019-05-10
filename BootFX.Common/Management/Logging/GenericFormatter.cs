// BootFX - Application framework for .NET applications
// 
// File: GenericFormatter.cs
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
	/// Defines a formatter that returns the unformatted message as the formatted message.
	/// </summary>
	public class GenericFormatter : ILogFormatter
	{
		public GenericFormatter()
		{
		}

		public string Format(ILog log, BootFX.Common.Management.LogLevel level, LogArgs args)
		{
			// mbr - 22-05-2008 - changed this to add date/time, because that's always handy.

//			// mbr - 20-08-2006 - wasn't handling exceptions...
//			if(ex == null)
//				return buf;
//			else
//				return string.Format("{0}\r\n\t{1}", buf, ex);

			// builder...
			StringBuilder builder = new StringBuilder();
			DefaultLogFormatter.AppendDateTime(builder);
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
