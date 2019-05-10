// BootFX - Application framework for .NET applications
// 
// File: SqlServerException.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Data;
using System.Data.SqlClient;
using System.Security;

namespace BootFX.Common.Data
{
	/// <summary>
	/// Raised when an exception occures on a SQL Server connection.
	/// </summary>
	[Serializable()]
    public class SqlServerException : DatabaseException
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		// TODO: Add a default message here.
		internal SqlServerException() 
			: base()
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		internal SqlServerException(string message) 
			: base(message)
		{
		}
		
		/// <summary>
		/// Constructor.
		/// </summary>
		internal SqlServerException(string message, Exception innerException) 
			: base(message, innerException)
		{
		}

		public static SqlServerException CreateException(string message, IDbCommand command, Exception innerException, SqlStatement sql)
		{
			StringBuilder builder = new StringBuilder();
			builder.Append(message);
			builder.Append("\r\n");
			if(command == null)
				builder.Append("(No command provided)");
			else
			{
				// base...
                if (sql != null && command is SqlCommand)
                    builder.Append(sql.GetTsqlDump());
                else
                    builder.Append(CreateCommandDump(command));

                builder.Append("\r\n-------------------------\r\n");

                // other error messages...
                if (innerException is SqlException)
				{
					SqlException sqlException = (SqlException)innerException;
					builder.Append("\r\n-------------------------\r\n");
					builder.Append(sqlException.Errors.Count);
					builder.Append(" error(s) occurred:\r\n");
					foreach(SqlError error in sqlException.Errors)
					{
						// append...
						builder.Append("\t");
						builder.Append(error);
						builder.Append("\r\n");
					}
					builder.Append("\r\n-------------------------\r\n");

					builder.Append("Connection string:\r\n\t");
					if(command.Connection != null)
						builder.Append(Connection.GetSafeConnectionStringForDebug(command.Connection));
					else
						builder.Append("(No connection)");
				}
			}

			// return...
			return new SqlServerException(builder.ToString(), innerException);
		}

		/// <summary> 
		/// Deserialization constructor.
		/// </summary> 
		protected SqlServerException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
			: base(info, context)
		{
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

			// add code here to stream extra state into 'info'.
			// remember to update the deserializtion constructor.
		}
	}
}
