// BootFX - Application framework for .NET applications
// 
// File: SqlServerConnection.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using System.Collections;
using System.ComponentModel;
using System.Text;
using BootFX.Common.Data.Schema;
using BootFX.Common.Data.Formatters;
using BootFX.Common.Entities;

namespace BootFX.Common.Data
{
	/// <summary>
	/// Represents a connection to SQL server.
	/// </summary>
	[Description("SQL Server"), Connection(), ConnectionStringBuilder(typeof(SqlServerConnectionStringBuilder))]
	public class SqlServerConnection : Connection
	{
		private static SqlServerDialect _dialect = new SqlServerDialect();

		public SqlServerConnection(string connectionString) : base(connectionString)
		{
		}

		protected override System.Data.IDbConnection CreateNativeConnection()
		{
			return new SqlConnection(this.ConnectionString);
		}

		/// <summary>
		/// Creates an exception for the given command.
		/// </summary>
		/// <param name="command"></param>
		/// <param name="originalException"></param>
		/// <returns></returns>
		protected internal override ApplicationException CreateCommandException(string message, IDbCommand command, Exception originalException,
            SqlStatement sql = null)
		{
			if(command == null)
				throw new ArgumentNullException("command");
			if(originalException == null)
				throw new ArgumentNullException("originalException");

			// create the new exception...
			return SqlServerException.CreateException(message, command, originalException, sql);
		}

		protected override IDataAdapter CreateDataAdapter(IDbCommand command)
		{
			if(command == null)
				throw new ArgumentNullException("command");
			if(!(command is SqlCommand))
				throw new InvalidOperationException(string.Format(Cultures.Exceptions, "Command is '{0}', not SqlCommand.", command.GetType()));
			return new SqlDataAdapter((SqlCommand)command);
		}

		/// <summary>
		/// Gets the field specification for the given native type name.
		/// </summary>
		/// <param name="nativeTypeName"></param>
		/// <returns></returns>
		public override FieldSpecification GetFieldSpecificationForNativeTypeName(string nativeTypeName, bool throwIfNotFound)
		{
			if(nativeTypeName == null)
				throw new ArgumentNullException("nativeTypeName");
			if(nativeTypeName.Length == 0)
				throw new ArgumentOutOfRangeException("'nativeTypeName' is zero-length.");
			
			// get...
			DbType dbType = DbType.String;
			bool isLarge = false;
			switch(nativeTypeName.ToLower())
			{
				case "bit":
					dbType = DbType.Boolean;
					break;

				case "char":
					dbType = DbType.AnsiStringFixedLength;
					break;
				case "nchar":
					dbType = DbType.StringFixedLength;
					break;
				case "varchar":
					dbType = DbType.AnsiString;
					break;
				case "nvarchar":
				case "sysname":
					dbType = DbType.String;
					break;

				case "text":
					dbType = DbType.AnsiString;
					isLarge = true;
					break;
				case "ntext":
					dbType = DbType.String;
					isLarge = true;
					break;

				case "tinyint":
					dbType = DbType.Byte;
					break;
				case "smallint":
					dbType = DbType.Int16;
					break;
				case "int":
				case "integer":
					dbType = DbType.Int32;
					break;
				case "bigint":
					dbType = DbType.Int64;
					break;

					// mbr - 20-06-2006 - added numeric.					
				case "money": 
				case "smallmoney":
				case "decimal":
				case "numeric":
					dbType = DbType.Decimal;
					break;

				case "real":
					dbType = DbType.Single;
					break;
				case "float":
					dbType = DbType.Double;
					break;

				case "datetime":
				case "smalldatetime":
					dbType = DbType.DateTime;
					break;

				case "timestamp":
					dbType = DbType.Binary;
					break;

				case "varbinary":
				case "binary":
					dbType = DbType.Binary;
					break;

				case "image":
					dbType = DbType.Binary;
					isLarge = true;
					break;

				case "uniqueidentifier":
					dbType = DbType.Guid;
					break;

				default:
                    if (throwIfNotFound)
                        throw new NotSupportedException(string.Format("Cannot handle '{0}' ({1}).", nativeTypeName.ToLower(), nativeTypeName.ToLower().GetType()));
                    else
                        return null;
			}

			// return...
			return new FieldSpecification(dbType, isLarge);
		}

		/// <summary>
		/// Gets the schema for the database.
		/// </summary>
		/// <returns></returns>
		// mbr - 02-10-2007 - added specification.		
		protected override SqlSchema CreateSchema(GetSchemaArgs args)
		{
			// base...
			SqlServerInformationSchemaFactory factory = new SqlServerInformationSchemaFactory(this);
			SqlSchema schema = factory.GetSchema(args);
			if(schema == null)
				throw new InvalidOperationException("schema is null.");

			// keys...
			this.SetupKeyColumns(schema);

			// indexes...
			this.AddIndexesToSchema(schema);

			// parents...
			this.AddParentLinks(schema);

			// return...
			return schema;
		}

		/// <summary>
		/// Sets up the key columns for the schema.
		/// </summary>
		/// <param name="schema"></param>
		private void SetupKeyColumns(SqlSchema schema)
		{
			if(schema == null)
				throw new ArgumentNullException("schema");

			//	declare	@TableName varchar(128)
			//	select @TableName = 'dbttmtourinformation'
			//	
			//	select 	c.COLUMN_NAME 
			//	from 	INFORMATION_SCHEMA.TABLE_CONSTRAINTS pk ,
			//		INFORMATION_SCHEMA.KEY_COLUMN_USAGE c
			//	where 	pk.TABLE_NAME = @TableName
			//	and	CONSTRAINT_TYPE = 'PRIMARY KEY'
			//	and	c.TABLE_NAME = pk.TABLE_NAME
			//	and	c.CONSTRAINT_NAME = pk.CONSTRAINT_NAME

			foreach(SqlTable table in schema.Tables)
			{
				// statement...
				SqlStatement statement = new SqlStatement(@"select c.COLUMN_NAME ColumnName from INFORMATION_SCHEMA.TABLE_CONSTRAINTS pk, 
						INFORMATION_SCHEMA.KEY_COLUMN_USAGE c where pk.TABLE_NAME = @p0 and CONSTRAINT_TYPE = 'PRIMARY KEY'
						and c.TABLE_NAME = pk.TABLE_NAME	and c.CONSTRAINT_NAME = pk.CONSTRAINT_NAME", CommandType.Text, 
					new object[] { table.NativeName }, this.Dialect);

				// get...
				DataTable columns = this.ExecuteDataTable(statement);
				foreach(DataRow column in columns.Rows)
				{
					// column...
					string columnName = (string)column["ColumnName"];
					SqlColumn schemaColumn = table.Columns[columnName];
					if(schemaColumn != null)
						schemaColumn.Flags |= EntityFieldFlags.Key;
				}

				// go again, this time with this to find the auto increment columns...
				//SELECT column_name, COLUMNPROPERTY(OBJECT_ID('actions'),column_name,'IsIdentity') as IsIdentity
				//	FROM information_schema.columns
				//	WHERE table_name='actions' 
				statement = new SqlStatement(@"SELECT column_name ColumnName, COLUMNPROPERTY(OBJECT_ID(@p0),column_name,'IsIdentity') as IsIdentity,
				ORDINAL_POSITION Ordinal FROM information_schema.columns	WHERE table_name=@p0", new object[] { table.NativeName }, this.Dialect);
				columns = this.ExecuteDataTable(statement);
				foreach(DataRow column in columns.Rows)
				{
					// get...
					string columnName = (string)column["ColumnName"];
					SqlColumn schemaColumn = table.Columns[columnName];

					int ordinal = Convert.ToInt32(column["Ordinal"]);
					bool isIdentity = Convert.ToBoolean(column["IsIdentity"]);
					if(schemaColumn != null && isIdentity)
						schemaColumn.Flags |= EntityFieldFlags.AutoIncrement;

					if(schemaColumn != null)
						schemaColumn.Ordinal = ordinal;
				}

				// Sort them
				table.Columns.SortByOrdinal();
			}
		}

		/// <summary>
		/// Adds links to parents in the schema.
		/// </summary>
		/// <param name="schema"></param>
		private void AddParentLinks(SqlSchema schema)
		{
			if(schema == null)
				throw new ArgumentNullException("schema");

			// get all constraints...
			DataTable table = this.ExecuteDataTable(new SqlStatement(@"select ctuparent.table_name as parent_table_name, unique_constraint_name as parent_constraint_name, 
			ctuchild.table_name as child_table_name, rc.constraint_name as child_constraint_name
				from information_schema.referential_constraints rc
					inner join information_schema.constraint_table_usage ctuparent on unique_constraint_name = ctuparent.constraint_name
					inner join information_schema.constraint_table_usage ctuchild on rc.constraint_name = ctuchild.constraint_name order by ctuparent.table_name, rc.constraint_name", 
                    this.Dialect));

			// walk each row...
			foreach(DataRow constraint in table.Rows)
			{
				// parent/child...
				SqlTable parent = schema.Tables[(string)constraint["parent_table_name"]];
				if(parent != null)
				{
					// child...
					SqlTable child = schema.Tables[(string)constraint["child_table_name"]];
					if(child != null)
					{
						// get the names...
						string parentConstraint = (string)constraint["parent_constraint_name"];
						string childConstraint = (string)constraint["child_constraint_name"];

						// find the columns...
						SqlColumn[] parentColumns = this.GetConstraintColumns(parent, parentConstraint);
						SqlColumn[] childColumns = this.GetConstraintColumns(child, childConstraint);

						// check...
						if(parentColumns == null)
							throw new InvalidOperationException("parentColumns is null.");
						if(childColumns == null)
							throw new InvalidOperationException("childColumns is null.");
						if(parentColumns.Length > 0 && parentColumns.Length == childColumns.Length)
						{
							// get the parent key columns...
							ArrayList parentKeyColumns = new ArrayList(parent.GetKeyColumns());
							if(parentKeyColumns.Count > 0)
							{
								// loop and remove...
								foreach(SqlColumn parentColumn in parentColumns)
									parentKeyColumns.Remove(parentColumn);

								// eliminated?
								if(parentKeyColumns.Count == 0)
								{
									// this is a legitimate match... create a link for that...
									SqlChildToParentLink link = new SqlChildToParentLink(childConstraint, parent);

									// add columns...
									link.Columns.AddRange(childColumns);	
									link.LinkFields.AddRange(parent.GetKeyColumns());

									// add...
									child.LinksToParents.Add(link);

									// mbr - 04-10-2007 - set the owner...
									link.SetSchema(child.Schema);
								}
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// Gets the columns in the given constraint.
		/// </summary>
		/// <param name="table"></param>
		/// <param name="constraintName"></param>
		/// <returns></returns>
		private SqlColumn[] GetConstraintColumns(SqlTable table, string constraintName)
		{
			if(table == null)
				throw new ArgumentNullException("table");
			if(constraintName == null)
				throw new ArgumentNullException("constraintName");
			if(constraintName.Length == 0)
				throw new ArgumentOutOfRangeException("'constraintName' is zero-length.");

			// defer...
			return this.GetColumns(table, this.GetConstraintColumnNames(table, constraintName));
		}

		/// <summary>
		/// Gets the columns in the given constraint.
		/// </summary>
		/// <param name="table"></param>
		/// <param name="constraintName"></param>
		/// <returns></returns>
		private string[] GetConstraintColumnNames(SqlTable table, string constraintName)
		{
			if(table == null)
				throw new ArgumentNullException("table");
			if(constraintName == null)
				throw new ArgumentNullException("constraintName");
			if(constraintName.Length == 0)
				throw new ArgumentOutOfRangeException("'constraintName' is zero-length.");
			
			// run...
			DataTable columns = this.ExecuteDataTable(new SqlStatement("select column_name from information_schema.constraint_column_usage where constraint_name=@p0", new object[] { constraintName }, 
                this.Dialect));
			string[] columnNames = new string[columns.Rows.Count];
			for(int index = 0; index < columns.Rows.Count; index++)
				columnNames[index] = (string)columns.Rows[index][0];

			// return...
			return columnNames;
		}

		/// <summary>
		/// Gets the columns with the given names.
		/// </summary>
		/// <param name="columnNames"></param>
		/// <returns></returns>
		private SqlColumn[] GetColumns(SqlTable table, string[] columnNames)
		{
			if(table == null)
				throw new ArgumentNullException("table");
			if(columnNames == null)
				throw new ArgumentNullException("columnNames");
			if(columnNames.Length == 0)
				throw new ArgumentOutOfRangeException("'columnNames' is zero-length.");
			
			// create...
			SqlColumn[] columns = new SqlColumn[columnNames.Length];
			for(int index = 0; index < columnNames.Length; index++)
			{
				columns[index] = table.Columns[columnNames[index]];
				if(columns[index] == null)
					throw new InvalidOperationException("columns[index] is null.");
			}

			// return...
			return columns;
		}

		/// <summary>
		/// Adds indexes to the schema.
		/// </summary>
		/// <param name="schema"></param>
		private void AddIndexesToSchema(SqlSchema schema)
		{
			if(schema == null)
				throw new ArgumentNullException("schema");

			foreach(SqlTable table in schema.Tables)
			{
				// create...
				SqlStatement statement = new SqlStatement("sp_MShelpindex", this.Dialect);
				statement.CommandType = CommandType.StoredProcedure;
				statement.Parameters.Add(new SqlStatementParameter("tablename", DbType.String, table.NativeName));
				statement.Parameters.Add(new SqlStatementParameter("indexname", DbType.String, DBNull.Value));
				statement.Parameters.Add(new SqlStatementParameter("flags", DbType.Int32, 1));

				// exec...
				DataTable indexes = this.ExecuteDataTable(statement);
				if(indexes == null)
					throw new InvalidOperationException("indexes is null.");

				// walk...
				foreach(DataRow indexRow in indexes.Rows)
				{
					// If we have a primary key
					if((((int)indexRow["status"]) & 2048) != 0)
						continue;

					// get the columns...
					ArrayList columnNames = new ArrayList();
					for(int index = 1; index <= 16; index++)
					{
						// name...
						string indexColumnName = "indcol" + index.ToString();
						if(indexRow.IsNull(indexColumnName) == false)
							columnNames.Add(indexRow[indexColumnName]);
					}

					// any?
					if(columnNames.Count > 0)
					{
						// create a new index...
						SqlIndex index = new SqlIndex((string)indexRow["name"]);
						table.Indexes.Add(index);

						// mbr - 04-10-2007 - set owner...
						index.SetSchema(schema);

						// unique values?
						int flags = (int)indexRow["status"];
						if((flags & 2) == 2)
							index.HasUniqueValues = true;

						// setup the columns...
						foreach(string columnName in columnNames)
						{
							// find...
							SqlColumn column = table.Columns[columnName];
							if(column == null)
								throw new InvalidOperationException(string.Format("Column '{0}' not found on table '{1}'.", column.NativeName, table.NativeName));

							// add...
							index.Columns.Add(column);
						}

                        // mbr - 2011-11-02 we also need to do included columns...
                        AddIncludedColumns(table, index);
					}
				}
			}
		}

        private void AddIncludedColumns(SqlTable table, SqlIndex index)
        {
            if (table == null)
                throw new ArgumentNullException("table");
            if (index == null)
                throw new ArgumentNullException("index");

            // go...
            DataTable dt = this.ExecuteDataTable(new SqlStatement(@"SELECT
                (CASE ic.key_ordinal WHEN 0 THEN CAST(1 AS tinyint) ELSE ic.key_ordinal END) AS [ID],
                clmns.name AS [Name],
                CAST(COLUMNPROPERTY(ic.object_id, clmns.name, N'IsComputed') AS bit) AS [IsComputed],
                ic.is_descending_key AS [Descending],
                ic.is_included_column AS [IsIncluded]
                FROM sys.tables AS tbl
                   INNER JOIN sys.indexes AS i 
                      ON (i.index_id > 0 AND i.is_hypothetical = 0) AND (i.object_id = tbl.object_id)
                   INNER JOIN sys.index_columns AS ic 
                      ON (ic.column_id > 0 AND (ic.key_ordinal > 0 OR ic.partition_ordinal = 0 OR ic.is_included_column != 0)) 
                         AND (ic.index_id = CAST(i.index_id AS int) AND ic.object_id = i.object_id)
                   INNER JOIN sys.columns AS clmns 
                   ON clmns.object_id = ic.object_id AND clmns.column_id = ic.column_id
                WHERE (i.name = @p0) AND (tbl.name = @p1)
                ORDER BY IsIncluded, [ID] ASC", new object[] { index.NativeName, table.NativeName }, this.Dialect));
            foreach (DataRow row in dt.Rows)
            {
                // add...
                string columnName = (string)row["name"];
                SqlColumn column = table.Columns[columnName];
                if (column == null)
                    throw new InvalidOperationException(string.Format("Column '{0}' not found on table '{1}'.", column.NativeName, table.NativeName));

                bool included = ConversionHelper.ToBoolean(row["isincluded"], Cultures.System);
                if(included)
                    index.IncludedColumns.Add(column);

                bool computed = ConversionHelper.ToBoolean(row["iscomputed"], Cultures.System);
                if (computed)
                    index.ComputedColumns.Add(column);
            }
        }

		/// <summary>
		/// Returns a boolean determining if the table you requested exists in the SQL Server database
		/// </summary>
		/// <param name="nativeName"></param>
		/// <returns></returns>
		protected override bool DoDoesTableExist(NativeName nativeName)
		{
			// create...
			// mbr - 11-10-2005 - wasn't passing in the dialect...
			SqlStatement statement = new SqlStatement(this.Dialect);
			StringBuilder builder = new StringBuilder();
			builder.Append(statement.Dialect.SelectKeyword);
			builder.Append(" count(*) ");
			builder.Append(statement.Dialect.FromKeyword);
			builder.Append(" ");
			builder.Append(statement.Dialect.FormatTableName("sysobjects"));
			builder.Append(" ");
			builder.Append(statement.Dialect.WhereKeyword);
			builder.Append(" ");
			builder.Append(statement.Dialect.FormatColumnName("id"));
			builder.Append(statement.Dialect.GetOperatorKeyword(SqlOperator.EqualTo, DbType.Int64));
			builder.Append("object_id(");
			builder.Append(statement.Dialect.FormatVariableNameForQueryText("nativeName"));
			builder.Append(")");

			statement.CommandText = builder.ToString();

			// params...
			statement.Parameters.Add(new SqlStatementParameter("nativeName", DbType.String, nativeName.Name));

			int count = (int) ExecuteScalar(statement);
			return count > 0;
		}

		public override BootFX.Common.Data.Formatters.SqlDialect Dialect
		{
			get
			{
				return _dialect;
			}
		}

        ///// <summary>
        ///// Copies a BLOB value to a stream.
        ///// </summary>
        ///// <param name="field"></param>
        ///// <param name="outStream"></param>
        //public override void CopyStreamToBlobValue(EntityField field, object[] keyValues, Stream inStream)
        //{
        //    if(field == null)
        //        throw new ArgumentNullException("field");
        //    if(keyValues == null)
        //        throw new ArgumentNullException("keyValues");
        //    if(keyValues.Length == 0)
        //        throw new ArgumentOutOfRangeException("'keyValues' is zero-length.");
        //    if(inStream == null)
        //        throw new ArgumentNullException("inStream");

        //    // fields...
        //    EntityType entityType = field.EntityType;
        //    if(entityType == null)
        //        throw new InvalidOperationException("entityType is null.");

        //    // ensure...
        //    this.EnsureConnectivity();

        //    // begin,...
        //    this.BeginTransaction();
        //    IDbCommand pointerCommand = null;
        //    IDbCommand updateCommand = null;
        //    try
        //    {
        //        // firstly, reset...
        //        this.ResetBlobField(entityType, field, keyValues);

        //        // statement...
        //        SqlStatement statement = new SqlStatement(this.Dialect);

        //        // secondly, get a test pointer...
        //        const string pointerParamName = "pointer";
        //        StringBuilder builder = new StringBuilder();
        //        builder.Append(this.Dialect.SelectKeyword);
        //        builder.Append(" ");
        //        builder.Append(this.Dialect.FormatVariableNameForQueryText(pointerParamName));
        //        builder.Append("=");
        //        builder.Append("TEXTPTR(");
        //        builder.Append(this.Dialect.FormatColumnName(field.NativeName.Name));
        //        builder.Append(")");
        //        this.AppendFromAndKeyValues(statement, builder, entityType, keyValues);
        //        statement.CommandText = builder.ToString();

        //        // create...
        //        pointerCommand = this.CreateCommandxxx(statement);
        //        if(pointerCommand == null)
        //            throw new InvalidOperationException("pointerCommand is null.");

        //        // add an addition argument...
        //        // mbr - 28-11-2005 - this needs to be moved to SqlServerConnection.				
        //        SqlParameter pointerParam = (SqlParameter)pointerCommand.CreateParameter();
        //        pointerCommand.Parameters.Add(pointerParam);
        //        pointerParam.ParameterName = "@" + pointerParamName;
        //        pointerParam.DbType = DbType.Binary;
        //        pointerParam.Direction = ParameterDirection.Output;
        //        pointerParam.Size = 16;

        //        // run that...
        //        pointerCommand.ExecuteNonQuery();

        //        // get the pointer...
        //        byte[] pointer = (byte[])pointerParam.Value;
        //        if(pointer == null)
        //            throw new InvalidOperationException("'pointer' is null.");
        //        if(pointer.Length == 0)
        //            throw new InvalidOperationException("'pointer' is zero-length.");

        //        // secondly, we need to make mutliple calls to UPDATE text...
        //        // http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpguide/html/cpconconservingresourceswhenwritingblobvaluestosqlserver.asp
        //        updateCommand  = this.CreateCommandxxx();
        //        if(updateCommand == null)
        //            throw new InvalidOperationException("updateCommand is null.");
        //        updateCommand.CommandText = string.Format("UPDATETEXT {0}.{1} {2} {3} 0 {4}", this.Dialect.FormatTableName(entityType.NativeName.Name),
        //            this.Dialect.FormatColumnName(field.NativeName.Name), this.Dialect.FormatVariableNameForQueryText(pointerParamName), this.Dialect.FormatVariableNameForQueryText("Offset"),
        //            this.Dialect.FormatVariableNameForQueryText("Bytes"));

        //        // create a buffer...
        //        const int bufLen = 10240;
        //        byte[] buf = new byte[bufLen];

        //        // set the pointer...
        //        pointerParam = (SqlParameter)updateCommand.CreateParameter();
        //        updateCommand.Parameters.Add(pointerParam);
        //        pointerParam.ParameterName = "@" + pointerParamName;
        //        pointerParam.DbType = DbType.Binary;
        //        pointerParam.Value = pointer;

        //        // bytes...
        //        IDataParameter bytesParam = updateCommand.CreateParameter();
        //        updateCommand.Parameters.Add(bytesParam);
        //        bytesParam.ParameterName = "@Bytes";
        //        bytesParam.DbType = DbType.Binary;
				
        //        // offset...
        //        IDataParameter offsetParam = updateCommand.CreateParameter();
        //        updateCommand.Parameters.Add(offsetParam);
        //        offsetParam.ParameterName = "@Offset";
        //        offsetParam.DbType = DbType.Int32;

        //        // while...
        //        int offset= 0;
        //        while(true)
        //        {
        //            int read = inStream.Read(buf, 0, bufLen);
        //            if(read == 0)
        //                break;

        //            // update...
        //            offsetParam.Value = offset;
        //            bytesParam.Value = buf;

        //            // run it...
        //            updateCommand.ExecuteNonQuery();

        //            // next...
        //            offset += read;
        //        }

        //        // commit...
        //        this.Commit();
        //    }
        //    catch(Exception ex)
        //    {
        //        // roll...
        //        this.Rollback();

        //        // throw...
        //        throw new InvalidOperationException("Failed to update database BLOB.", ex);
        //    }
        //    finally
        //    {
        //        // mbr - 2014-11-30 - caching means we never dispose commands per usage...
        //        //if(pointerCommand != null)
        //        //    pointerCommand.Dispose();
        //        //if(updateCommand != null)
        //        //    updateCommand.Dispose();
        //    }
        //}

        ///// <summary>
        ///// Resets the given BLOB field.
        ///// </summary>
        ///// <param name="entityType"></param>
        ///// <param name="field"></param>
        ///// <param name="keyValues"></param>
        //private void ResetBlobField(EntityType entityType, EntityField field, object[] keyValues)
        //{
        //    if(entityType == null)
        //        throw new ArgumentNullException("entityType");
        //    if(field == null)
        //        throw new ArgumentNullException("field");
        //    if(keyValues == null)
        //        throw new ArgumentNullException("keyValues");
        //    if(keyValues.Length == 0)
        //        throw new ArgumentOutOfRangeException("'keyValues' is zero-length.");
			
        //    // create...
        //    SqlStatement statement = new SqlStatement(this.Dialect);
			
        //    // first of all, set it to be a zero length field...
        //    StringBuilder builder = new StringBuilder();
        //    builder.Append(this.Dialect.UpdateKeyword);
        //    builder.Append(" ");
        //    builder.Append(this.Dialect.FormatTableName(entityType.NativeName.Name));
        //    builder.Append(" ");
        //    builder.Append(this.Dialect.SetKeyword);
        //    builder.Append(" ");
        //    builder.Append(this.Dialect.FormatColumnName(field.NativeName.Name));
        //    builder.Append("=");
        //    const string paramName = "val";
        //    builder.Append(this.Dialect.FormatVariableNameForQueryText(paramName));
        //    builder.Append(" ");
        //    this.AppendFromAndKeyValues(statement, builder, entityType, keyValues);

        //    // statement...
        //    statement.CommandText = builder.ToString();

        //    // set...
        //    statement.Parameters.Add(new SqlStatementParameter(paramName, DbType.Binary, new byte[] {}));

        //    // run...
        //    statement.CommandText = builder.ToString();
        //    this.ExecuteNonQuery(statement);
        //}

        public override DataTable ExecuteDataTable(ISqlStatementSource statement)
        {
            // mbr - 2014-11-30 - caching means we never dispose commands per usage...
            //using (IDbCommand command = this.CreateCommand(statement))
            SqlStatement realStatement = null;
            var command = this.CreateCommand(statement, ref realStatement);
            {
                try
                {
                    return this.CaptureProfile<DataTable>(realStatement, command, SqlMethod.DataSet, (profiling) =>
                    {
                        using (SqlDataAdapter adapter = new SqlDataAdapter())
                        {
                            adapter.SelectCommand = (SqlCommand)command;

                            // run...
                            DataTable table = new DataTable();
                            adapter.Fill(table);

                            // return...
                            return table;
                        }
                    });
                }
                catch (Exception ex)
                {
                    throw CreateCommandException("Failed to ExecuteDataTable.", command, ex);
                }
            }
        }
	}
}
