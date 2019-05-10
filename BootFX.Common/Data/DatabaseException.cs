// BootFX - Application framework for .NET applications
// 
// File: DatabaseException.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Xml;
using System.Text;
using System.Data;
using System.Security;

namespace BootFX.Common.Data
{
	/// <summary>
	/// Raised when an exception occures on a SQL Server connection.
	/// </summary>
	[Serializable()]
	public class DatabaseException : ApplicationException
	{
		// mbr - 22-02-2008 - deprecated.
//		/// <summary>
//		/// Private field to support <c>CommandDump</c> property.
//		/// </summary>
//		private string _commandDump;
		
		/// <summary>
		/// Constructor.
		/// </summary>
		// TODO: Add a default message here.
		internal DatabaseException() 
			: base()
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		internal DatabaseException(string message) 
			: base(message)
		{
		}
		
		/// <summary>
		/// Constructor.
		/// </summary>
		internal DatabaseException(string message, Exception innerException) 
			: base(message, innerException)
		{
		}

		/// <summary> 
		/// Deserialization constructor.
		/// </summary> 
		protected DatabaseException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
			: base(info, context)
		{
			// mbr - 22-02-2008 - deprecated.			
//			_commandDump = info.GetString("_commandDump");
		}

		/// <summary> 
		/// Provides data to serialization.
		/// </summary> 
        [SecurityCritical]
        [System.Security.Permissions.SecurityPermissionAttribute(System.Security.Permissions.SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			// base...
			base.GetObjectData(info, context);

			// mbr - 22-02-2008 - deprecated.		
//			// add code here to stream extra state into 'info'.
//			// remember to update the deserializtion constructor.
//			info.AddValue("_commandDump", _commandDump);
		}

		// mbr - 22-02-2008 - deprecated.
//		/// <summary>
//		/// Gets the commanddump.
//		/// </summary>
//		public string CommandDump
//		{
//			get
//			{
//				return _commandDump;
//			}
//		}

		/// <summary>
		/// Updates the command dump.
		/// </summary>
		/// <param name="command"></param>
		internal static string CreateCommandDump(IDbCommand command)
		{
			if(command == null)
				throw new ArgumentNullException("command");
			
			// create it...
			StringBuilder builder = new StringBuilder();
			builder.Append(command.CommandText);
			builder.Append(" (");
			builder.Append(command.CommandType);
			builder.Append(")\r\n");

			// params...
			if(command.Parameters.Count > 0)
			{
				builder.Append(command.Parameters.Count);
				builder.Append(" parameter(s):");
				foreach(IDataParameter param in command.Parameters)
				{
					builder.Append("\r\n\t");
					builder.Append(param.ParameterName);
					builder.Append(": ");

					// format...
					FormatParameterValueForDump(param.Value, builder, true);

					// stuff...
					builder.Append(", ");
					builder.Append(param.DbType);
					builder.Append(", ");
					builder.Append(param.Direction);
				}
			}
			else
				builder.Append("(No parameters available.)");

			// return...
			return builder.ToString();
		}

		internal static void FormatParameterValueForDump(object value, StringBuilder builder, bool trimStrings)
		{
			if(value == null)
				builder.Append("(CLR null)");
			else if(value is DBNull)
				builder.Append("(DB null)");
			else
			{
				// string...
				if(value is string)
				{
					string valueAsString = (string)value;
					builder.Append("[");
					const int maxLen = 128;
					if(trimStrings && valueAsString.Length > maxLen)
					{
						builder.Append(valueAsString.Substring(0, maxLen));
						builder.Append("...");
					}
					else
						builder.Append(valueAsString);
					builder.Append("] System.String of ");
					builder.Append(valueAsString.Length);
					if(valueAsString.Length != 1)
						builder.Append(" chars");
					else
						builder.Append(" char");
				}
				else if(value is XmlNode)
				{
					XmlNode node = (XmlNode)value;
					builder.Append("[");
					builder.Append(node.OuterXml);
					builder.Append("] XML node, type=");
					builder.Append(node.NodeType);
				}
				else
				{
					builder.Append("[");
					builder.Append(value.ToString());
					builder.Append("]");
					builder.Append(" ");
					builder.Append(value.GetType());
				}
			}
		}

		// mbr - 22-02-2008 - deprecated.	
//		/// <summary>
//		/// Sets the command dump.
//		/// </summary>
//		/// <param name="commandDump"></param>
//		[FxCopNote("DesignRules.dll", "ConsiderReplacingMethodsWithProperties", "Done as a method in order to provide split access properties.")]
//		protected void SetCommandDump(string commandDump)
//		{
//			_commandDump = commandDump;
//		}

		// mbr - 22-02-2008 - no need for this - command dump had been deprecated for ages...
//		public override string ToString()
//		{
//			StringBuilder builder = new StringBuilder();
//			builder.Append(base.ToString());
//			builder.Append("\r\n=========================\r\n");
//			if(this.CommandDump != null && this.CommandDump.Length > 0)
//			{
//				builder.Append("Command dump:\r\n");
//				builder.Append(this.CommandDump);
//			}
//			else
//				builder.Append("Command dump not available.");
//
//			// return...
//			return builder.ToString();
//		}
	}
}
