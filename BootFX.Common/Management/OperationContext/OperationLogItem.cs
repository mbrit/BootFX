// BootFX - Application framework for .NET applications
// 
// File: OperationLogItem.cs
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
	/// Defines an instance of <c>OperationWarning</c>.
	/// </summary>
	public class OperationLogItem
	{
		public const OperationLogItemFormatFlags DefaultFormatFlags = OperationLogItemFormatFlags.IncludeExceptionMessage;

		/// <summary>
		/// Private field to support <see cref="Message"/> property.
		/// </summary>
		private string _message;

		/// <summary>
		/// Private field to support <see cref="Exception"/> property.
		/// </summary>
		private Exception _exception;
		
		/// <summary>
		/// Private field to support <see cref="Level"/> property.
		/// </summary>
		private LogLevel _level;
		
		public OperationLogItem(LogLevel level, string message, Exception ex)
		{
			_level = level;
			_message = message;
			_exception = ex;
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
		/// Gets the message.
		/// </summary>
		public string Message
		{
			get
			{
				return _message;
			}
		}

		/// <summary>
		/// Gets the level.
		/// </summary>
		public LogLevel Level
		{
			get
			{
				return _level;
			}
		}

		public override string ToString()
		{
			return this.ToString(DefaultFormatFlags);
		}

		public string ToString(OperationLogItemFormatFlags flags)
		{
			StringBuilder builder = new StringBuilder();
			if((int)(flags & OperationLogItemFormatFlags.IncludeLogLevel) != 0)
			{
				builder.Append(this.Level.ToString());
				builder.Append(": ");
			}

			// add...
			builder.Append(this.Message);

			// ex?
			if(this.Exception != null)
			{
				if((int)(flags & OperationLogItemFormatFlags.UseBrTagSeparator) != 0)
					builder.Append("<BR />&nbsp;&nbsp;&nbsp;&nbsp;");
				else
					builder.Append("\r\n\t");

				// now what...
				if((int)(flags & OperationLogItemFormatFlags.IncludeExceptionMessage) != 0)
					builder.Append(this.Exception.Message);
				else if((int)(flags & OperationLogItemFormatFlags.IncludeFullException) != 0)
				{
					// mbr - 08-02-2007 - added br formatting...
					string exAsString = this.Exception.ToString();
					if((int)(flags & OperationLogItemFormatFlags.UseBrTagSeparator) != 0)
						exAsString = exAsString.Replace("\n", "<BR />");

					// append...
					builder.Append(exAsString);
				}
			}

			// return...
			return builder.ToString();
		}
	}
}
