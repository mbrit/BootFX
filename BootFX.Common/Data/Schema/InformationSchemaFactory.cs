// BootFX - Application framework for .NET applications
// 
// File: InformationSchemaFactory.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Text;
using System.Data;
using System.Collections;
using BootFX.Common.Data;
using BootFX.Common.Entities;
using BootFX.Common.Management;
using BootFX.Common.Data.Formatters;

namespace BootFX.Common.Data.Schema
{
	/// <summary>
	/// Creates a schema from an information schema factory.
	/// </summary>
	internal class InformationSchemaFactory : Loggable
	{
		/// <summary>
		/// Private field to support <see cref="Connection"/> property.
		/// </summary>
		private Connection _connection;

		/// <summary>
		/// Defines the size in bytes of the common field threshold. 
		/// </summary>
		private const int CommonThreshold = 2048;
		
		/// <summary>
		/// Constructor.
		/// </summary>
		public InformationSchemaFactory(Connection connection)
		{
			if(connection == null)
				throw new ArgumentNullException("connection");
			
			// set...
			_connection = connection;
		}

		/// <summary>
		/// Gets the connection.
		/// </summary>
		public Connection Connection
		{
			get
			{
				return _connection;
			}
		}

		/// <summary>
		/// Creates the schema.
		/// </summary>
		/// <returns></returns>
		// mbr - 02-10-2007 - added optional specification.		
		public virtual SqlSchema GetSchema(GetSchemaArgs args)
		{
			// create...
			SqlSchema schema = new SqlSchema();

			// gets a datatable of information_schema...
			if(Connection == null)
				throw new InvalidOperationException("Connection is null.");

			// sql...
			SqlStatement sql = new SqlStatement(this.Dialect);

			// mbr - 02-10-2007 - build the sql, now including the search specification...
			StringBuilder builder = new StringBuilder();
			builder.Append("SELECT DISTINCT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE='BASE TABLE'");
			if(args.ConstrainTableNames.Count > 0)
			{
				builder.Append(" AND (");
				this.AppendTableNameConstraint(sql, builder, args.ConstrainTableNames);
				builder.Append(")");
			}
			if(args.ConstrainSchemaNames.Count > 0)
			{
				builder.Append(" AND (");
				this.AppendSchemaNameConstraint(sql, builder, args.ConstrainSchemaNames);
				builder.Append(")");
			}
			builder.Append(" ORDER BY TABLE_NAME");

			// get...
			sql.CommandText = builder.ToString();
			DataTable table = this.Connection.ExecuteDataTable(sql);

			// walk...
			foreach(DataRow row in table.Rows)
			{
				// create a new schema table...
				SqlTable schemaTable = this.GetSchemaTable(row, schema);
				schema.Tables.Add(schemaTable);

				// mbr - 04-10-2007 - set owner...				
				schemaTable.SetSchema(schema);
			}

			// return...
			return schema;
		}

		/// <summary>
		/// Appends table name constraints.
		/// </summary>
		/// <param name="sql"></param>
		/// <param name="builder"></param>
		/// <param name="tableNames"></param>
		protected void AppendTableNameConstraint(SqlStatement sql, StringBuilder builder, IList tableNames)
		{
			if(sql == null)
				throw new ArgumentNullException("sql");
			if(builder == null)
				throw new ArgumentNullException("builder");
			if(tableNames == null)
				throw new ArgumentNullException("tableNames");
			
			// build...
			builder.Append("TABLE_NAME IN (");
			for(int index = 0; index < tableNames.Count; index++)
			{
				string tableName = (string)tableNames[index];

				// first?
				if(index > 0)
					builder.Append(", ");
				builder.Append(sql.Dialect.FormatVariableNameForQueryText(sql.Parameters.Add(DbType.String, tableName)));
			}
			builder.Append(")");
		}

		/// <summary>
		/// Appends table name constraints.
		/// </summary>
		/// <param name="sql"></param>
		/// <param name="builder"></param>
		/// <param name="tableNames"></param>
		protected void AppendSchemaNameConstraint(SqlStatement sql, StringBuilder builder, IList tableNames)
		{
			if(sql == null)
				throw new ArgumentNullException("sql");
			if(builder == null)
				throw new ArgumentNullException("builder");
			if(tableNames == null)
				throw new ArgumentNullException("tableNames");
			
			// build...
			builder.Append("TABLE_SCHEMA IN (");
			for(int index = 0; index < tableNames.Count; index++)
			{
				string tableName = (string)tableNames[index];

				// first?
				if(index > 0)
					builder.Append(", ");
                builder.Append(sql.Dialect.FormatVariableNameForQueryText(sql.Parameters.Add(DbType.String, tableName)));
			}
			builder.Append(")");
		}

		/// <summary>
		/// Creates a table from the given row.
		/// </summary>
		/// <param name="row"></param>
		/// <returns></returns>
		private SqlTable GetSchemaTable(DataRow row, SqlSchema schema)
		{
			if(row == null)
				throw new ArgumentNullException("row");
			if(schema == null)
				throw new ArgumentNullException("schema");			

			// get the name...
			string nativeName = (string)row["table_name"];
			if(nativeName == null)
				throw new ArgumentNullException("nativeName");
			if(nativeName.Length == 0)
				throw new ArgumentOutOfRangeException("'nativeName' is zero-length.");
			
			try
			{
				// name...
				string name = SqlTable.SuggestSingularName(nativeName);
				name = CodeDomHelper.GetPascalName(name);
			
				// create...
				SqlTable schemaTable = new SqlTable(nativeName, name);

				// mbr - 04-10-2007 - set schema...
				schemaTable.SetSchema(schema);

				// get the columns...
				if(Connection == null)
					throw new InvalidOperationException("Connection is null.");
				DataTable table = this.Connection.ExecuteDataTable(new SqlStatement("select column_name, is_nullable, data_type, character_maximum_length from information_schema.columns where table_name=@p0 order by ordinal_position", 
					new object[] { nativeName }, this.Dialect));
				foreach(DataRow columnRow in table.Rows)
				{
					// create...
					SqlColumn schemaColumn = this.GetSchemaColumn(columnRow);
					schemaTable.Columns.Add(schemaColumn);

					// mbr - 04-10-2007 - set owner...
					schemaColumn.SetSchema(schemaTable.Schema);

					// mbr - 01-11-2005 - added SQL Server specific stuff...			
					this.ColumnDiscovered(schemaColumn);
				}

				// fix...
				FixupCommonFlags(schemaTable);

				// mbr - 01-11-2005 - added opportunity to fixup...
				TableDiscovered(schemaTable);

				// return...
				return schemaTable;
			}
			catch(Exception ex)
			{
				throw new InvalidOperationException(string.Format("Failed when processing table '{0}'.", nativeName), ex);
			}
		}

		// mbr - 24-07-2008 - moved into separate method for Oracle support.		
		internal static void FixupCommonFlags(SqlTable schemaTable)
		{
			if(schemaTable == null)
				throw new ArgumentNullException("schemaTable");
			
			// work out the physical size of the entity...
			long totalLength = 0;
			foreach(SqlColumn schemaColumn in schemaTable.Columns)
			{
				// get...
				long actualLength = schemaColumn.ActualLength;
				if(actualLength > 0)
				{
					totalLength += actualLength;

					// should we be common...
					if(totalLength < CommonThreshold && schemaColumn.IsKey == false)
					{
						// set...
						schemaColumn.Flags |= EntityFieldFlags.Common;
					}
				}
			}
		}

		/// <summary>
		/// Called when a column is discovered.
		/// </summary>
		/// <param name="column"></param>
		protected virtual void TableDiscovered(SqlTable table)
		{
		}

		/// <summary>
		/// Called when a column is discovered.
		/// </summary>
		/// <param name="column"></param>
		protected virtual void ColumnDiscovered(SqlColumn column)
		{
		}

		/// <summary>
		/// Creates a column from the given row.
		/// </summary>
		/// <param name="row"></param>
		/// <returns></returns>
		private SqlColumn GetSchemaColumn(DataRow row)
		{
			if(row == null)
				throw new ArgumentNullException("row");
			
			// get...
			string nativeName = (string)row["column_name"];
			string isNullableAsString = (string)row["is_nullable"];
			string dataTypeAsString = (string)row["data_type"];

			// mbr - 2008-08-31 - MySQL implementation means this int has to be a long?
			//int length = -1;
			long length = -1;
			if(row.IsNull("character_maximum_length") == false)
			{
				//length = ConversionHelper.ToInt32(row["character_maximum_length"], Cultures.System);
				length = ConversionHelper.ToInt64(row["character_maximum_length"], Cultures.System);
			}

			// get...
			if(Connection == null)
				throw new InvalidOperationException("Connection is null.");

			// nullable?
			EntityFieldFlags flags = EntityFieldFlags.Normal;
			if(string.Compare(isNullableAsString, "yes", true, System.Globalization.CultureInfo.InvariantCulture) == 0)
				flags |= EntityFieldFlags.Nullable;

			// large?
			FieldSpecification specification = this.Connection.GetFieldSpecificationForNativeTypeName(dataTypeAsString);
			if(specification == null)
				throw new InvalidOperationException("specification is null.");
			if(specification.IsLarge)
				flags |= EntityFieldFlags.Large;

			// return...
			return new SqlColumn(nativeName, specification.DbType, length, flags);
		}

        protected SqlDialect Dialect
        {
            get
            {
                return this.Connection.Dialect;
            }
        }
	}
}
