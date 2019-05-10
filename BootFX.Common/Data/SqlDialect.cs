// BootFX - Application framework for .NET applications
// 
// File: SqlDialect.cs
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
using System.Collections.Specialized;
using BootFX.Common.Data.Schema;
using BootFX.Common.Entities;
using System.Collections.Generic;
using System.Linq;

namespace BootFX.Common.Data.Formatters
{
	/// <summary>
	/// Describes a class capable of formatting items in SQL strings.
	/// </summary>
	public abstract class SqlDialect
	{
        /// <summary>
        /// Constructor.
        /// </summary>
        protected SqlDialect()
		{
		}

		/// <summary>
		/// Supports getting the last id from the database
		/// </summary>
		internal virtual LastInsertedIdMode LastInsertedIdMode
		{
			get
			{
				return LastInsertedIdMode.Scalar;
			}
		}

		/// <summary>
		/// Gets the all wildcard.
		/// </summary>
		public virtual string AllWildcard
		{
			get
			{
				return "%";
			}
		}

		/// <summary>
		/// Gets the single char wildcard.
		/// </summary>
		public virtual string SingleWildcard
		{
			get
			{
				return "$";
			}
		}

		/// <summary>
		/// Gets the variable prefix.
		/// </summary>
		public virtual string FromKeyword
		{
			get
			{
				return "FROM";
			}
		}

		/// <summary>
		/// Gets the variable prefix.
		/// </summary>
		public abstract string VariablePrefix
		{
			get;
		}

		/// <summary>
		/// Gets the variable prefix.
		/// </summary>
		public abstract string IdentifierPrefix
		{
			get;
		}

		/// <summary>
		/// Gets the variable prefix.
		/// </summary>
		public abstract string IdentifierSuffix
		{
			get;
		}

		/// <summary>
		/// Gets the select keyword.
		/// </summary>
		public virtual string NullKeyword
		{
			get
			{
				return "NULL";
			}
		}

		/// <summary>
		/// Gets the select keyword.
		/// </summary>
		public virtual string IsKeyword
		{
			get
			{
				return "IS";
			}
		}

		/// <summary>
		/// Gets the select keyword.
		/// </summary>
		public virtual string InKeyword
		{
			get
			{
				return "IN";
			}
		}

		/// <summary>
		/// Gets the select keyword.
		/// </summary>
		public virtual string NotKeyword
		{
			get
			{
				return "NOT";
			}
		}

		/// <summary>
		/// Gets the select keyword.
		/// </summary>
		public virtual string BetweenKeyword
		{
			get
			{
				return "BETWEEN";
			}
		}

        internal string FormatColumnNamesList(IEnumerable<string> columnNames)
        {
            StringBuilder builder = new StringBuilder();
            var first = true;
            foreach (var name in columnNames)
            {
                if (first)
                    first = false;
                else
                    builder.Append(", ");
                builder.Append(this.FormatColumnName(name));
            }
            return builder.ToString();
        }

        /// <summary>
        /// Gets the select keyword.
        /// </summary>
        public virtual string LikeKeyword
		{
			get
			{
				return "LIKE";
			}
		}

		/// <summary>
		/// Gets the select keyword.
		/// </summary>
		public virtual string SelectKeyword
		{
			get
			{
				return "SELECT";
			}
		}

        /// <summary>
        /// Gets the select keyword.
        /// </summary>
        // mbr - 2010-03-03 - added...
        public virtual string DistinctKeyword
        {
            get
            {
                return "DISTINCT";
            }
        }

        /// <summary>
		/// Gets the select keyword.
		/// </summary>
		public virtual string UpdateKeyword
		{
			get
			{
				return "UPDATE";
			}
		}

		/// <summary>
		/// Gets the select keyword.
		/// </summary>
		public virtual string DeleteFromKeyword
		{
			get
			{
				return "DELETE FROM";
			}
		}

		/// <summary>
		/// Gets the select keyword.
		/// </summary>
		public virtual string InsertIntoKeyword
		{
			get
			{
				return "INSERT INTO";
			}
		}

		/// <summary>
		/// Gets the select keyword.
		/// </summary>
		public virtual string ValuesKeyword
		{
			get
			{
				return "VALUES";
			}
		}

		/// <summary>
		/// Gets the select keyword.
		/// </summary>
		public virtual string IdentifierSeparator
		{
			get
			{
				return ", ";
			}
		}

		/// <summary>
		/// Gets the select keyword.
		/// </summary>
		public virtual string StatementBlockSeparator
		{
			get
			{
				return "GO";
			}
		}

		/// <summary>
		/// Gets the select keyword.
		/// </summary>
		public abstract string StatementSeparator
		{
			get;
		}

		/// <summary>
		/// Gets the select keyword.
		/// </summary>
		public virtual string SetKeyword
		{
			get
			{
				return "SET";
			}
		}

		/// <summary>
		/// Gets the select keyword.
		/// </summary>
		public virtual string WhereKeyword
		{
			get
			{
				return "WHERE";
			}
		}

		/// <summary>
		/// Gets the select keyword.
		/// </summary>
		public virtual string OrKeyword
		{
			get
			{
				return "OR";
			}
		}

		/// <summary>
		/// Gets the select keyword.
		/// </summary>
		public virtual string DescendingKeyword
		{
			get
			{
				return "DESC";
			}
		}

		/// <summary>
		/// Gets the select keyword.
		/// </summary>
		public virtual string AscendingKeyword
		{
			get
			{
				return "ASC";
			}
		}

		/// <summary>
		/// Gets the select keyword.
		/// </summary>
		public virtual string AndKeyword
		{
			get
			{
				return "AND";
			}
		}

		/// <summary>
		/// Gets the select keyword.
		/// </summary>
		public virtual string OrderByKeyword
		{
			get
			{
				return "ORDER BY";
			}
		}

		/// <summary>
		/// Gets the select keyword.
		/// </summary>
		public virtual string DropProcedureKeyword
		{
			get
			{
				return "DROP PROCEDURE";
			}
		}

		/// <summary>
		/// Gets the select keyword.
		/// </summary>
		public virtual string CreateProcedureKeyword
		{
			get
			{
				return "CREATE PROCEDURE";
			}
		}

		/// <summary>
		/// Gets the select keyword.
		/// </summary>
		public virtual string CreateTableKeyword
		{
			get
			{
				return "CREATE TABLE";
			}
		}

		/// <summary>
		/// Gets the create index keyword.
		/// </summary>
		public virtual string CreateIndexKeyword
		{
			get
			{
				return "CREATE INDEX";
			}
		}

		/// <summary>
		/// Gets the drop index keyword.
		/// </summary>
		public virtual string DropIndexKeyword
		{
			get
			{
				return "DROP INDEX";
			}
		}

		/// <summary>
		/// Gets the create unique index keyword.
		/// </summary>
		public virtual string CreateUniqueIndexKeyword
		{
			get
			{
				return "CREATE UNIQUE INDEX";
			}
		}

		/// <summary>
		/// Gets the casting keyword.
		/// </summary>
		public virtual string CastKeyword
		{
			get
			{
				return "CAST";
			}
		}

		/// <summary>
		/// Gets the as keyword.
		/// </summary>
		public virtual string AsKeyword
		{
			get
			{
				return "AS";
			}
		}

		/// <summary>
		/// Gets the select keyword.
		/// </summary>
		public virtual string BitwiseAndKeyword
		{
			get
			{
				return "&";
			}
		}

		/// <summary>
		/// Gets the keyword for the given combine.
		/// </summary>
		/// <param name="combine"></param>
		/// <returns></returns>
		public virtual string GetOperatorKeyword(SqlOperator sqlOperator, DbType dataType)
		{
			switch(sqlOperator)
			{
				case SqlOperator.EqualTo:
					return "=";
				case SqlOperator.NotEqualTo:
					return "<>";
				case SqlOperator.GreaterThan:
					return ">";
				case SqlOperator.GreaterThanOrEqualTo:
					return ">=";
				case SqlOperator.LessThan:
					return "<";
				case SqlOperator.LessThanOrEqualTo:
					return "<=";

				default:
					throw ExceptionHelper.CreateCannotHandleException(sqlOperator);
			}
		}

		/// <summary>
		/// Gets the keyword for the given combine.
		/// </summary>
		/// <param name="combine"></param>
		/// <returns></returns>
		public string GetCombineKeyword(SqlCombine combine)
		{
			switch(combine)
			{
				case SqlCombine.And:
					return this.AndKeyword;
				case SqlCombine.Or:
					return this.OrKeyword;

				default:
					throw ExceptionHelper.CreateCannotHandleException(combine);
			}
		}

		/// <summary>
		/// Gets the keyword for the given combine.
		/// </summary>
		/// <param name="combine"></param>
		/// <returns></returns>
		public string GetSortDirectionKeyword(SortDirection direction)
		{
			switch(direction)
			{
				case SortDirection.Ascending:
					return this.AscendingKeyword;
				case SortDirection.Descending:
					return this.DescendingKeyword;

				default:
					throw ExceptionHelper.CreateCannotHandleException(direction);
			}
		}

        public virtual string FormatVariableNameForQueryText(SqlStatementParameter parameter)
        {
            return this.FormatVariableNameForQueryText(parameter.Name);
        }

		/// <summary>
		/// Formats the variable name.
		/// </summary>
		/// <param name="variableName"></param>
		/// <returns></returns>
		public virtual string FormatVariableNameForQueryText(string variableName)
		{
			if(variableName == null)
				throw new ArgumentNullException("variableName");
			if(variableName.Length == 0)
				throw new ArgumentOutOfRangeException("'variableName' is zero-length.");
			
			if(VariablePrefix == null)
				throw new InvalidOperationException("VariablePrefix is null.");
			return this.VariablePrefix + variableName;
		}

		public virtual string FormatVariableNameForCommandParameter(CommandType type, string variableName)
		{
            return this.FormatVariableNameForQueryText(variableName);
		}

		/// <summary>
		/// Gets the drop table keyword.
		/// </summary>
		public virtual string DropTableKeyword
		{
			get
			{
				return "DROP TABLE";
			}
		}

		/// <summary>
		/// Gets the snippet of code for creating a column.
		/// </summary>
		/// <param name="column"></param>
		/// <returns></returns>
		protected string GetColumnSnippet(SqlColumn column)
		{
			if(column == null)
				throw new ArgumentNullException("column");

			// defer...
			return this.GetColumnSnippet(column.Table, column.NativeName, column.DbType, column.Length, column.IsLarge, column.IsNullable, 
				column.IsAutoIncrement, column.DefaultExpression);
		}

		/// <summary>
		/// Gets the snippet of code for creating a column.
		/// </summary>
		/// <param name="column"></param>
		/// <returns></returns>
		protected virtual string GetColumnSnippet(SqlTable table, string nativeName, DbType type, long length, bool isLarge, bool isNullable, 
			bool isAutoIncrement, SqlDatabaseDefault defaultExpression)
		{
			if(table == null)
				throw new ArgumentNullException("table");
			if(nativeName == null)
				throw new ArgumentNullException("nativeName");
			if(nativeName.Length == 0)
				throw new ArgumentOutOfRangeException("'nativeName' is zero-length.");

			// name...
			StringBuilder builder = new StringBuilder();
			builder.Append(this.FormatColumnName(nativeName));
			builder.Append(" ");
			builder.Append(this.GetColumnTypeNativeName(type, isLarge, length));

			// unll?
			builder.Append(" ");
			if(isNullable)
				builder.Append(this.NullKeyword);	
			else
				builder.Append(NotNullKeyword);

			// auto?
			if(isAutoIncrement)
			{
                //builder.Append(" IDENTITY(1,1)");
                builder.Append(" ");
                builder.Append(this.AutoIncrementKeyword);
			}

			// return...
			return builder.ToString();
		}

		/// <summary>
		/// Gets the snippet of code for creating a column.
		/// </summary>
		/// <param name="column"></param>
		/// <returns></returns>
		protected string GetColumnConstraintSnippet(SqlColumn column, SqlDatabaseDefault defaultExpression)
		{
			if(column == null)
				throw new ArgumentNullException("column");

			// defer...
			return this.GetColumnConstraintSnippet(column.Table, column.NativeName, column.DbType, column.Length, column.IsLarge, column.IsNullable, 
				column.IsAutoIncrement, defaultExpression);
		}

		/// <summary>
		/// Gets the snippet of code for creating a column constraint.
		/// </summary>
		/// <returns></returns>
		protected string GetColumnConstraintSnippet(SqlTable table, string nativeName, DbType type, long length, bool isLarge, bool isNullable, 
			bool isAutoIncrement, SqlDatabaseDefault defaultExpression)
		{
			if(table == null)
				throw new ArgumentNullException("table");
			if(nativeName == null)
				throw new ArgumentNullException("nativeName");
			if(nativeName.Length == 0)
				throw new ArgumentOutOfRangeException("'nativeName' is zero-length.");

			// name...
			StringBuilder builder = new StringBuilder();
			builder.Append(ConstraintKeyword);
			builder.Append(" ");
			string constraintName = string.Format("DF_{0}_{1}", table.NativeName, nativeName);
			builder.Append(this.FormatTableName(constraintName));
			builder.Append(" ");
			builder.Append(DefaultKeyword);
			builder.Append(" ");

			// add...
			string expression = this.GetDefaultExpressionSnippet(defaultExpression);
			if(expression == null)
				throw new InvalidOperationException("'expression' is null.");
			if(expression.Length == 0)
				throw new InvalidOperationException("'expression' is zero-length.");
			builder.Append(expression);

			// return...
			return builder.ToString();
		}

        protected virtual string GetDefaultExpressionSnippet(SqlDatabaseDefault expression)
		{
			if(expression == null)
				throw new ArgumentNullException("expression");

			// do the expression...
			switch(expression.Type)
			{
				case SqlDatabaseDefaultType.Primitive:

					// check...
					if(expression.Value == null)
						throw new InvalidOperationException("expression.Value is null.");

					// type code...
					TypeCode code = Type.GetTypeCode(expression.Value.GetType());
				    switch(code)
				    {
					    case TypeCode.String:
						    return "(N'" + ((string)expression.Value).Replace("'", "''") + "')";

					    case TypeCode.Int16:
					    case TypeCode.Int32:
					    case TypeCode.Int64:
                        case TypeCode.Double:
                        case TypeCode.Single:
                        case TypeCode.Decimal:
                            return string.Format("({0})", ConversionHelper.ToString(expression.Value, Cultures.System));

					    default:
						    throw new NotSupportedException(string.Format("Cannot handle '{0}' ({1}).", code, code.GetType()));
				    }

				case SqlDatabaseDefaultType.CurrentDateTime:
					return "(getdate())";

				case SqlDatabaseDefaultType.Literal:
					if(expression.Value == null)
						throw new InvalidOperationException("expression.Value is null.");
					if(expression.Value is string)
						return (string)expression.Value;
					else
						throw new NotSupportedException(string.Format("Cannot handle literal expression of type '{0}'.", expression.Value.GetType()));

				default:
					throw new NotSupportedException(string.Format("Cannot handle '{0}' ({1}).", expression.Type, expression.Type.GetType()));
			}		
		}

		public string ConstraintKeyword
		{
			get
			{
				return "CONSTRAINT";
			}
		}

		public string DefaultKeyword
		{
			get
			{
				return "DEFAULT";
			}
		}

		public string WithValuesKeyword
		{
			get
			{
				return "WITH VALUES";
			}
		}

		/// <summary>
		/// Gets the auto-increment keyword.
		/// </summary>
		public virtual string AutoIncrementKeyword
		{
			get
			{
				return "IDENTITY(1,1)";
			}
		}

		/// <summary>
		/// Gets the keyword for specifying 'not null-ness'.
		/// </summary>
		private string NotNullKeyword
		{
			get
			{
				return "NOT NULL";
			}
		}

		/// <summary>
		/// Gets the type name for the given column type.
		/// </summary>
		/// <param name="dbType"></param>
		/// <param name="isLarge"></param>
		/// <param name="length"></param>
		/// <returns></returns>
		public string GetColumnTypeNativeName(SqlColumn column)
		{
			if(column == null)
				throw new ArgumentNullException("column");
			
			// defer...
			return this.GetColumnTypeNativeName(column.DbType, column.IsLarge, column.Length);
		}

		/// <summary>
		/// Gets the type name for the given column type.
		/// </summary>
		/// <param name="dbType"></param>
		/// <param name="isLarge"></param>
		/// <param name="length"></param>
		/// <returns></returns>
		public virtual string GetColumnTypeNativeName(DbType dbType, bool isLarge, long length)
		{
			switch(dbType)
			{
				case DbType.Int16:
					return "smallint";
				case DbType.Int32:
					return "int";
				case DbType.Int64:
					return "bigint";
				case DbType.Byte:
					return "tinyint";

				case DbType.Boolean:
					return "bit";

				case DbType.AnsiString:
                    if (isLarge)
                        return "text";
                    else
                    {
                        // mbr - 2010-04-28 - max?
                        if (length != -1)
                            return string.Format("varchar({0})", length);
                        else
                            return "varchar(max)";
                    }

				case DbType.Date:
				case DbType.Time:
				case DbType.DateTime:
					return "datetime";

				case DbType.AnsiStringFixedLength:
					return string.Format("char({0})", length);

				case DbType.String:
                    if (isLarge)
                        return "ntext";
                    else
                    {
                        // mbr - 2010-04-28 - max?
                        if (length != -1)
                            return string.Format("nvarchar({0})", length);
                        else
                            return "nvarchar(max)";
                    }

				case DbType.StringFixedLength:
					return string.Format("nchar({0})", length);

				case DbType.Double:
					return "float";
				case DbType.Single:
					return "real";

				case DbType.Decimal:
					return "decimal(18, 5)";

				case DbType.Binary:
				case DbType.Object:
                    if (isLarge)
                        return "image";
                    else if (length == -1)
                        return "timestamp";
                    else
                        return string.Format("binary({0})", length);

				case DbType.Guid:
					return "uniqueidentifier";

                case DbType.Xml:
                    return "xml";

				default:
					throw new NotSupportedException(string.Format("Cannot handle '{0}' ({1}).", dbType, dbType.GetType()));
			}
		}

//		/// <summary>
//		/// Formats a column name.
//		/// </summary>
//		/// <param name="columnName"></param>
//		/// <returns></returns>
//		// mbr - 25-09-2007 - added.		
//		internal string FormatColumnName(NativeName name, SqlAlias alias)
//		{
//			if(name == null)
//				throw new ArgumentNullException("name");
//			
//			// alias...
//			if(alias != null)
//				return string.Format("{0}.{1}", this.FormatTableName(alias.Alias), this.FormatColumnName(name));
//			else
//				return this.FormatColumnName(name);
//		}

		/// <summary>
		/// Formats a column name.
		/// </summary>
		/// <param name="columnName"></param>
		/// <returns></returns>
		// mbr - 10-10-2007 - added.		
		public string FormatColumnName(EntityField field)
		{
			return this.FormatColumnName(field, false);
		}

        public string FormatColumnName(EntityField field, string alias)
        {
            return this.FormatTableName(alias) + "." + this.FormatColumnName(field);
        }

        public string FormatColumnName(string field, string alias)
        {
            return this.FormatTableName(alias) + "." + this.FormatColumnName(field);
        }

        public string FormatColumnName(EntityField field, SqlJoin join)
        {
            if (join.UseAlias)
                return this.FormatColumnName(field, join.Alias);
            else
                return this.FormatColumnName(field, true);
        }

        /// <summary>
        /// Formats a column name.
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns></returns>
        // mbr - 10-10-2007 - added.		
        public string FormatColumnName(EntityField field, bool includeTableName)
		{
			if(field == null)
				throw new ArgumentNullException("field");
			
			// format...
			if(includeTableName)
				return string.Concat(this.FormatTableName(field.EntityType.NativeName.Name), ".", this.FormatColumnName(field.NativeName.Name));
			else
				return this.FormatColumnName(field.NativeName.Name);
		}

		/// <summary>
		/// Formats a column name.
		/// </summary>
		/// <param name="columnName"></param>
		/// <returns></returns>
		public string FormatColumnName(NativeName name)
		{
			if(name == null)
				throw new ArgumentNullException("name");			
			return this.FormatColumnName(name.Name);
		}

		/// <summary>
		/// Formats a column name.
		/// </summary>
		/// <param name="columnName"></param>
		/// <returns></returns>
		public string FormatColumnName(SqlColumn column)
		{
			if(column == null)
				throw new ArgumentNullException("column");
			return this.FormatColumnName(column.NativeName);
		}

		/// <summary>
		/// Formats a column name.
		/// </summary>
		/// <param name="columnName"></param>
		/// <returns></returns>
		public virtual string FormatColumnName(string columnName)
		{
			if(columnName == null)
				throw new ArgumentNullException("columnName");
			if(columnName.Length == 0)
				throw new ArgumentOutOfRangeException("'columnName' is zero-length.");
			return this.IdentifierPrefix + columnName + this.IdentifierSuffix;
		}

		private void AppendIfObjectExistsScript(StringBuilder builder, string objectName, SqlObjectType type, string trueCode, string falseCode)
		{
			if(builder == null)
				throw new ArgumentNullException("builder");
			if(objectName == null)
				throw new ArgumentNullException("objectName");
			if(objectName.Length == 0)
				throw new ArgumentOutOfRangeException("'objectName' is zero-length.");
			if((trueCode == null || trueCode.Length == 0) && (falseCode == null || falseCode.Length == 0))
				throw new InvalidOperationException("No true or false code was supplied.");

			// if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[bfxspISSActivities]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)

			// append...
			builder.Append("IF EXISTS (SELECT * FROM DBO.SYSOBJECTS WHERE ID=OBJECT_ID(N'");
			builder.Append(objectName);
			builder.Append("') AND OBJECTPROPERTY(id, N'");
			switch(type)
			{
				case SqlObjectType.Sproc:
					builder.Append("IsProcedure");
					break;

				default:
					throw new NotSupportedException(string.Format("Cannot handle '{0}' ({1}).", type, type.GetType()));
			}
			builder.Append("')=1)\r\nBEGIN\r\n");
			builder.Append(trueCode);
			builder.Append("\r\n");
			if(falseCode != null && falseCode.Length > 0)
			{
				builder.Append("\r\nELSE");
				builder.Append(falseCode);
				builder.Append("\r\n");
			}
			builder.Append("END\r\n");
		}

		/// <summary>
		/// Gets the script to add a column.
		/// </summary>
		/// <param name="column"></param>
		/// <param name="flags"></param>
		/// <returns></returns>
		private string GetAlterColumnScript(SqlTable table, string nativeName, DbType type, long length, bool isLarge, bool isNullable, SqlDatabaseDefault defaultExpression,bool add)
		{
			if(table == null)
				throw new ArgumentNullException("table");
			if(nativeName == null)
				throw new ArgumentNullException("nativeName");
			if(nativeName.Length == 0)
				throw new ArgumentOutOfRangeException("'nativeName' is zero-length.");
			
			// create...
			StringBuilder builder = new StringBuilder();
			builder.Append(this.AlterTableKeyword);
			builder.Append(" ");
			builder.Append(this.FormatTableName(table));
            if (add)
                builder.Append(" ADD ");
            else
            {
                //builder.Append(" ALTER COLUMN ");
                builder.Append(" ");
                builder.Append(this.AlterColumnKeyword);
                builder.Append(" ");
            }
            builder.Append(this.GetColumnSnippet(table, nativeName, type, length, isLarge, isNullable, false, defaultExpression));
			builder.Append(this.StatementSeparator);
			builder.Append("\r\n");

			// return...
			return builder.ToString();
		}

        ///// <summary>
        ///// Gets the sproc for creating an insert into the given table.
        ///// </summary>
        ///// <param name="table"></param>
        ///// <returns></returns>
        //public string GetCreateInsertSprocScript(SqlTable table, SqlTableScriptFlags flags)
        //{
        //    if(table == null)
        //        throw new ArgumentNullException("table");
			
        //    // header...
        //    StringBuilder builder = new StringBuilder();
        //    builder.Append(this.CommentPrefix);
        //    builder.Append(" Insert sproc for ");
        //    builder.Append(table.NativeName);
        //    builder.Append("\r\n");

        //    // what's the name...
        //    string name = this.FormatSprocName(table.InsertSprocName);
        //    if(name == null)
        //        throw new InvalidOperationException("'name' is null.");
        //    if(name.Length == 0)
        //        throw new InvalidOperationException("'name' is zero-length.");

        //    // drop...
        //    if((flags & SqlTableScriptFlags.IncludeDropObject) != 0)
        //    {
        //        this.AppendIfObjectExistsScript(builder, name, SqlObjectType.Sproc, DropProcedureKeyword + " " + name, null);
        //        builder.Append(this.StatementBlockSeparator);
        //        builder.Append("\r\n");
        //    }

        //    // create...
        //    builder.Append(CreateProcedureKeyword);
        //    builder.Append(" ");
        //    builder.Append(name);
        //    builder.Append(" (");
        //    IDictionary paramNames = new HybridDictionary();
        //    bool first = true;
        //    int index = 0;
        //    IList autoIncrementColumns = new ArrayList();
        //    foreach(SqlColumn column in table.Columns)
        //    {
        //        // name...
        //        string paramName = column.NativeName;
        //        paramNames[column] = paramName;

        //        if(first)
        //            first = false;
        //        else
        //            builder.Append(", ");
        //        builder.Append("\r\n");
        //        builder.Append(this.FormatVariableNameForQueryText(paramName, DbType.String));
        //        builder.Append(" ");
        //        builder.Append(this.GetColumnTypeNativeName(column));

        //        // auto...
        //        if(column.IsAutoIncrement)
        //        {
        //            // out...
        //            builder.Append(" OUT");

        //            // flag...
        //            autoIncrementColumns.Add(column);
        //        }
        //    }

        //    // create the body...
        //    builder.Append(") AS\r\n");
        //    builder.Append(InsertIntoKeyword);
        //    builder.Append(" ");
        //    builder.Append(this.FormatTableName(table));
        //    builder.Append(" (");
        //    first = true;
        //    index = 0;
        //    foreach(SqlColumn column in table.Columns)
        //    {
        //        if(!(column.IsAutoIncrement))
        //        {
        //            if(first)
        //                first = false;
        //            else
        //                builder.Append(", ");
        //            if(index != 0 && index % 8 == 0)
        //                builder.Append("\r\n");
        //            builder.Append(this.FormatColumnName(column));

        //            index++;
        //        }
        //    }
        //    builder.Append(")\r\n    ");
        //    builder.Append(this.ValuesKeyword);
        //    builder.Append(" (");
        //    first = true;
        //    index = 0;
        //    foreach(SqlColumn column in table.Columns)
        //    {
        //        if(!(column.IsAutoIncrement))
        //        {
        //            if(first)
        //                first = false;
        //            else
        //                builder.Append(", ");
        //            if(index != 0 && index % 8 == 0)
        //                builder.Append("\r\n");
        //            builder.Append(this.FormatVariableNameForQueryText((string)paramNames[column]));

        //            index++;
        //        }
        //    }
        //    builder.Append(")");
        //    builder.Append(this.StatementSeparator);
        //    builder.Append("\r\n");

        //    // return...
        //    if(autoIncrementColumns.Count == 1)
        //    {
        //        SqlColumn column = (SqlColumn)autoIncrementColumns[0];

        //        // scope...
        //        builder.Append(this.SelectKeyword);
        //        builder.Append(" ");
        //        builder.Append(this.FormatVariableNameForQueryText((string)paramNames[column]));
        //        builder.Append("=");
        //        builder.Append(this.LastInsertedIdVariableName);
        //        builder.Append(this.StatementSeparator);
        //        builder.Append("\r\n");
        //    }
        //    else if(autoIncrementColumns.Count > 1)
        //    {
        //        builder.Append(this.CommentPrefix);
        //        builder.AppendFormat("Table has '{0}' auto-increment columns and only single auto-increment columns are supported.", autoIncrementColumns.Count);
        //    }

        //    // block...
        //    builder.Append(this.StatementBlockSeparator);

        //    // return...
        //    return builder.ToString();
        //}

		/// <summary>
		/// Gets the complete script for building a table.
		/// </summary>
		/// <param name="table"></param>
		/// <returns></returns>
		public string[] GetCreateTableScript(SqlTable table, SqlTableScriptFlags flags)
		{
			if(table == null)
				throw new ArgumentNullException("table");

			ArrayList statements = new ArrayList();
			// drop...
			if((flags & SqlTableScriptFlags.IncludeDropObject) != 0)
				statements.AddRange(this.GetDropTableStatement(table));

			// return the basic create table statement...
			statements.AddRange(this.GetCreateTableStatement(table));
			
			// Add the constraints
			foreach(SqlColumn column in table.Columns)
				if(column.HasDefaultExpression)
					statements.AddRange(GetCreateDefaultConstraintStatement(column,column.DefaultExpression));

            // Get create index statements
            foreach (SqlIndex index in table.Indexes)
            {
                if(index.Name.Length <= this.MaximumIndexNameLength)
                    statements.AddRange(this.GetCreateIndexStatement(table, index));
            }

			// return...
			return (string[]) statements.ToArray(typeof(string));
		}

		protected string AddConstraintKeyword
		{
			get
			{
				return "ADD CONSTRAINT";
			}
		}

		protected string AlterTableKeyword	
		{
			get
			{
				return "ALTER TABLE";
			}
		}

		protected string FormatConstraintName(string name)
		{
			return this.FormatColumnName(name);
		}

		public string FormatIndexName(string name)
		{
			return this.FormatColumnName(name);
		}

		protected string FormatForeignKeyName(string name)
		{
			return this.FormatColumnName(name);
		}

		/// <summary>
		/// Formats a column name for use in a select statement.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		// mbr - 2008-09-11 - added for MySQL support.
		public virtual string FormatColumnNameForSelect(EntityField field, bool useFullyQualifiedNames)
		{
			return this.FormatColumnName(field, useFullyQualifiedNames);
		}

		private string DropConstraintKeyword
		{
			get
			{
				return "DROP CONSTRAINT";
			}
		}

		/// <summary>
		/// Formats the given table name.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public string FormatSprocName(string name)
		{
			if(name == null)
				throw new ArgumentNullException("name");
			if(name.Length == 0)
				throw new ArgumentOutOfRangeException("'name' is zero-length.");

			// defer...
			return this.FormatTableName(name);
		}

        public string FormatTableName(NativeName name, string alias)
        {
            return this.FormatTableName(name) + " " + this.FormatTableName(alias);
        }

        public string FormatTableName(NativeName name)
		{
			if(name == null)
				throw new ArgumentNullException("name");
			
			// if...
			StringBuilder builder = new StringBuilder();			
			if(name.HasCatalogName)
			{
				builder.Append(this.FormatTableName(name.CatalogName));
				builder.Append(".");
				if(name.HasUserName)
					builder.Append(this.FormatTableName(name.UserName));
				builder.Append(".");
				builder.Append(this.FormatTableName(name.Name));
			}
			else if(name.HasUserName)
			{
				builder.Append(this.FormatTableName(name.UserName));
				builder.Append(".");
				builder.Append(this.FormatTableName(name.Name));
			}
			else
				builder.Append(this.FormatTableName(name.Name));

			// return...
			return builder.ToString();
		}

		/// <summary>
		/// Formats the given table name.
		/// </summary>
		/// <param name="table"></param>
		/// <returns></returns>
		public string FormatTableName(SqlTable table)
		{
			if(table == null)
				throw new ArgumentNullException("table");			
			return this.FormatTableName(table.NativeName);
		}

        public string FormatTableName(string name, string alias)
        {
            return this.FormatTableName(name) + " " + this.FormatTableName(alias);
        }

        /// <summary>
        /// Formats the given table name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string FormatTableName(string name)
		{
			if(name == null)
				throw new ArgumentNullException("name");
			if(name.Length == 0)
				throw new ArgumentOutOfRangeException("'name' is zero-length.");
			return this.IdentifierPrefix + name + this.IdentifierSuffix;
		}

		/// <summary>
		/// Gets the name of the variable that can be queried to get the last auto-increment ID.
		/// </summary>
		public abstract string LastInsertedIdVariableName
		{
			get;
		}

		private void AppendScriptHeader(StringBuilder builder, SqlTableScriptFlags flags)
		{
			if(builder == null)
				throw new ArgumentNullException("builder");

			builder.Append(this.CommentPrefix);
			builder.Append("Database creation script for ");
			builder.Append(this.ToString());
			builder.Append("\r\n");
			builder.Append(this.CommentPrefix);
			builder.Append("Flags: ");
			builder.Append(flags);
			builder.Append("\r\n");
			builder.Append(this.CommentPrefix);
			builder.Append("BootFX DBUtil Tools v");
			builder.Append(this.GetType().Assembly.GetName().Version);
			builder.Append("\r\n");
			builder.Append(this.CommentPrefix);
			builder.Append(DateTime.Now.ToString());
			builder.Append("\r\n\r\n");
		}

		/// <summary>
		/// Gets the script to create this database.
		/// </summary>
		/// <param name="tableFlags"></param>
		/// <returns></returns>
        //public string GetRecreateSprocsScript(SqlSchema schema, SqlTableScriptFlags tableFlags)
        //{
        //    if(schema == null)
        //        throw new ArgumentNullException("schema");

        //    // wakl...
        //    StringBuilder builder = new StringBuilder();
        //    this.AppendScriptHeader(builder, tableFlags);

        //    // get...
        //    foreach(SqlTable table in schema.GetTablesForGeneration())
        //    {
        //        builder.Append("\r\n");
        //        builder.Append(this.GetCreateInsertSprocScript(table, tableFlags));
        //    }		

        //    // return...
        //    return builder.ToString();
        //}

		/// <summary>
		/// Gets the script to create this database.
		/// </summary>
		/// <param name="tableFlags"></param>
		/// <returns></returns>
		public string GetCreateDatabaseScript(SqlSchema schema, SqlTableScriptFlags tableFlags)
		{
			if(schema == null)
				throw new ArgumentNullException("schema");

			// create...
			StringBuilder builder = new StringBuilder();
			this.AppendScriptHeader(builder, tableFlags);

			// tables...
			SqlTable[] tables = schema.GetTablesForGeneration();
			builder.Append(this.CommentPrefix);
			builder.Append("Includes tables:");
			foreach(SqlTable table in tables)
			{
				builder.Append("\r\n");
				builder.Append(this.CommentPrefix);
				builder.Append("   ");
				builder.Append(table);
			}

			// walk...
			foreach(SqlTable table in schema.GetTablesForGeneration())
			{
				foreach(string snippet in this.GetCreateTableScript(table, tableFlags))
				{
					builder.Append("\r\n");
					builder.Append(snippet);
				}
			}

			// return...
			return builder.ToString();
		}

		/// <summary>
		/// Gets the prefix for comments.
		/// </summary>
		private string CommentPrefix
		{
			get
			{
				return "-- ";
			}
		}

		/// <summary>
		/// Gets the statement to create the given table.
		/// </summary>
		/// <param name="table"></param>
		/// <returns></returns>
		public abstract string[] GetCreateTableStatement(SqlTable table);

		/// <summary>
		/// Gets the statement to create the given table.
		/// </summary>
		/// <param name="table"></param>
		/// <returns></returns>
		public abstract string[] GetDropTableStatement(SqlTable table);

		/// <summary>
		/// Gets the statement to add or alter a column.
		/// </summary>
		/// <param name="column"></param>
		/// <returns></returns>
		public abstract string[] GetAddColumnStatement(SqlColumn column, bool add);

		/// <summary>
		/// Gets the statement to add an index
		/// </summary>
		/// <param name="table"></param>
		/// <param name="index"></param>
		/// <returns></returns>
		public abstract string[] GetCreateIndexStatement(SqlTable table, SqlIndex index);

		/// <summary>
		/// Gets the statement to drop an index
		/// </summary>
		/// <param name="table"></param>
		/// <param name="index"></param>
		/// <returns></returns>
		public abstract string[] GetDropIndexStatement(SqlTable table, SqlIndex index);

		/// <summary>
		/// Gets the statement to drop a constraint
		/// </summary>
		/// <param name="column"></param>
		/// <param name="databaseDefault"></param>
		/// <returns></returns>
		public abstract string[] GetDropConstraintStatement(SqlColumn column, SqlDatabaseDefault databaseDefault);
		
		/// <summary>
		/// Gets the statement to create a constraint
		/// </summary>
		/// <param name="column"></param>
		/// <param name="databaseDefault"></param>
		/// <returns></returns>
		public abstract string[] GetCreateDefaultConstraintStatement(SqlColumn column, SqlDatabaseDefault databaseDefault);

		/// <summary>
		/// Gets the statement to add a foreign key.
		/// </summary>
		/// <param name="foreignKey"></param>
		/// <returns></returns>
		public abstract string[] GetCreateForeignKeyStatement(SqlChildToParentLink foreignKey);

		/// <summary>
		/// Gets the statement to drop a foreign key.
		/// </summary>
		/// <param name="foreignKey"></param>
		/// <returns></returns>
		public abstract string[] GetDropForeignKeyStatement(SqlChildToParentLink foreignKey);

		protected virtual void SetupPopulateDataScript(SqlTable table, StringBuilder builder)
		{
			builder.Append(this.BeginTransactionKeyword);
			builder.Append(this.StatementSeparator);
			builder.Append("\r\n");
		}

		protected virtual void TeardownPopulateDataScript(SqlTable table, StringBuilder builder)
		{
			builder.Append(this.CommitKeyword);
			builder.Append(this.StatementSeparator);
			builder.Append("\r\n");
		}

		/// <summary>
		/// Runs a 'select *' query against the given table and generates SQL statements for each insert.
		/// </summary>
		/// <param name="table"></param>
		public string GetPopulateDataScript(SqlTable table)
		{
			if(table == null)
				throw new ArgumentNullException("table");

			StringBuilder builder = new StringBuilder();
			builder.Append(this.CommentPrefix);
			builder.Append("Contents of ");
			builder.Append(table);
			builder.Append("\r\n");
			builder.Append(this.CommentPrefix);
			builder.Append("Dialect: ");
			builder.Append(this.ToString());
			builder.Append("\r\n");
			
			// begin..
			this.SetupPopulateDataScript(table, builder);

			// create...
			SqlStatement statement = new SqlStatement(string.Format("select * from {0}", this.FormatTableName(table)));
			using(IConnection conn = Database.CreateConnection(statement))
			{
                // mbr - 2014-11-30 - caching means we no longer dispose here...
                var command = conn.CreateCommand(statement);
				{
					conn.EnsureOpen();
					using(IDataReader reader = command.ExecuteReader())
					{
                        var first = true;
                        var per = 0;
						while(reader.Read())
						{
                            // create...
                            if (first)
                            {
                                builder.Append(this.InsertIntoKeyword);
                                builder.Append(" ");
                                builder.Append(this.FormatTableName(table));
                                builder.Append(" ");
                                builder.Append("(");
                                for (int index = 0; index < table.Columns.Count; index++)
                                {
                                    if (index > 0)
                                        builder.Append(", ");
                                    builder.Append(this.FormatColumnName(table.Columns[index]));
                                }
                                builder.Append(") ");
                                builder.Append(this.ValuesKeyword);

                                first = false;
                            }
                            else
                                builder.Append(", ");

							builder.Append(" (");
							for(int index = 0; index < table.Columns.Count; index++)
							{
								if(index > 0)
									builder.Append(", ");

								// value...
								object value = reader[index];
								string valueAsString = this.FormatValueForDirectSql(value);
								builder.Append(valueAsString);
							}
							builder.Append(")");
							builder.Append("\r\n");

                            per++;
                            if (per == 1000)
                            {
                                builder.Append(this.StatementSeparator);
                                builder.Append("\r\n");

                                first = true;
                                per = 0;
                            }
						}
                    }
                }
			}

			// commit...
			this.TeardownPopulateDataScript(table, builder);

			// return...
			return builder.ToString();
		}

		private string BeginTransactionKeyword
		{
			get
			{
				return "BEGIN TRANSACTION";
			}
		}

		private string CommitKeyword
		{
			get
			{
				return "COMMIT";
			}
		}

		/// <summary>
		/// Formats a value for use in a SQL statement.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		/// <remarks>Use this method *only* for scripting.  All other access should be done through parameters.</remarks>
		private string FormatValueForDirectSql(object value)
		{
			if(value == null || value is DBNull)
				return this.NullKeyword;
			else if(value is string)
				return "'" + ((string)value).Replace("'", "''") + "'";
			else if(value is Guid)
				return "'" + ((Guid)value).ToString() + "'";
			else if(value is bool)
			{
				if((bool)value)
					return "1";
				else
					return "0";
			}
			else if(value is DateTime)
				return "'" + ((DateTime)value).ToString(DateTimeFormat) + "'";
			else if(value is byte[])
			{
				StringBuilder builder = new StringBuilder();
				builder.Append("0x");
				foreach(byte b in (byte[])value)
					builder.Append(b.ToString("x2"));
				return builder.ToString();
			}
			else
				return Convert.ToString(value);
		}

		private string DateTimeFormat
		{
			get
			{
				return "yyyy-MM-dd HH:mm:ss.fff";
			}
		}

		/// <summary>
		/// Formats the given table name.
		/// </summary>
		/// <param name="nativeName"></param>
		/// <returns></returns>
		public virtual string FormatNativeName(NativeName nativeName)
		{
			if(nativeName == null)
				throw new ArgumentNullException("nativeName");

			// format it...
			if(nativeName.CatalogName == null && nativeName.UserName == null)
				return this.IdentifierPrefix + nativeName.Name + this.IdentifierSuffix;

			// do we have a catalogname?
			if(nativeName.CatalogName != null)
			{
				if(nativeName.UserName != null)
					return string.Format("{0}{2}{1}.{0}{3}{1}.{0}{4}{1}", this.IdentifierPrefix, this.IdentifierSuffix, 
						nativeName.CatalogName, nativeName.UserName, nativeName.Name);
				else
					return string.Format("{0}{2}{1}..{0}{3}{1}", this.IdentifierPrefix, this.IdentifierSuffix, 
						nativeName.CatalogName, nativeName.Name);
			}

			// username?
			return string.Format("{0}{2}{1}.{0}{3}{1}", this.IdentifierPrefix, this.IdentifierSuffix, 
				nativeName.UserName, nativeName.Name);
		}

		public virtual string FormatTop(int top)
		{
			if(top < 1)
				throw new ArgumentOutOfRangeException("top", top, "Top must be >= 1.");

			// return...
			return "TOP " + top.ToString();
		}

        public virtual bool TopAtTop
        {
            get
            {
                return true;
            }
        }

//		/// <summary>
//		/// Format the given alias.
//		/// </summary>
//		/// <param name="alias"></param>
//		/// <returns></returns>
//		internal string FormatAlias(SqlAlias alias)
//		{
//			if(alias == null)
//				throw new ArgumentNullException("alias");
//
//			// return...
//			return string.Format("{0} {1} {2}", alias.ResolvedName, AsKeyword, alias.Alias);
//		}

        public virtual string AppendLastInsertedIdParameterAccess(EntityField[] autoIncrementFields)
        {
            throw new NotImplementedException();
        }

        public virtual SqlStatementParameter GetLastInsertedIdParameter()
        {
            throw new NotImplementedException();
        }

        public virtual bool SupportsMultipleInsertValues
        {
            get
            {
                return true;
            }
        }

        public virtual string GetForceIndexDirective(string name)
        {
            return string.Format("WITH(INDEX({0}))", this.FormatIndexName(name));
        }

        public virtual int MaximumIndexNameLength
        {
            get
            {
                return 1024;
            }
        }

        public virtual string AlterColumnKeyword
        {
            get
            {
                return "ALTER COLUMN";
            }
        }

        protected internal virtual SqlStatement RewriteStatement(SqlStatement sql)
        {
            return sql;
        }

        public string FormatTableName<T>()
            where T : Entity
        {
            return this.FormatTableName(typeof(T).ToEntityType());
        }

        public string FormatTableName(EntityType et)
        {
            return this.FormatTableName(et.NativeName);
        }

        public string FormatKeyColumnName<T>()
            where T : Entity
        {
            return this.FormatKeyColumnName(typeof(T).ToEntityType());
        }

        public string FormatKeyColumnName(EntityType et)
        {
            return this.FormatColumnName(et.GetKeyFields().First());
        }
    }
}
