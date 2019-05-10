// BootFX - Application framework for .NET applications
// 
// File: SqlServerDialect.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Collections;
using System.Text;
using BootFX.Common.Data.Schema;
using BootFX.Common.Entities;
using System.Collections.Generic;

namespace BootFX.Common.Data.Formatters
{
	/// <summary>
	/// Describes specific dialect constructs for Microsoft SQL Server.
	/// </summary>
	public class SqlServerDialect : SqlDialect
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		public SqlServerDialect()
		{
		}

		public override string IdentifierPrefix
		{
			get
			{
				return "[";
			}
		}

		public override string IdentifierSuffix
		{
			get
			{
				return "]";
			}
		}

		public override string StatementSeparator
		{
			get
			{
				return ";";
			}
		}

		public override string VariablePrefix
		{
			get
			{
				return "@";
			}
		}

		public override string LastInsertedIdVariableName
		{
			get
			{
				return "scope_identity()";
			}
		}

		/// <summary>
		/// Gets the statement for creating a primary key constraint for the given table.
		/// </summary>
		/// <param name="table"></param>
		protected virtual string GetCreatePrimaryKeyConstraintStatement(SqlTable table)
		{
			if(table == null)
				throw new ArgumentNullException("table");

			// alter table authors add constraint pk_foobar primary key (au_id)
			return this.GetCreatePrimaryKeyConstraintStatement(table, "PK_" + table.NativeName);
		}
		
		/// <summary>
		/// Gets the statement for creating a primary key constraint for the given table.
		/// </summary>
		/// <param name="table"></param>
		private string GetCreatePrimaryKeyConstraintStatement(SqlTable table, string constraintName)
		{
			StringBuilder builder = new StringBuilder();

			// mangle...
			string sqlName = this.FormatConstraintName(constraintName);
			
			// alter table authors add constraint pk_foobar primary key (au_id)
			builder.Append(this.AlterTableKeyword);
			builder.Append(" ");
			builder.Append(this.FormatTableName(table.NativeName));
			builder.Append(" ");
			builder.Append(this.AddConstraintKeyword);
			builder.Append(" ");
			builder.Append(sqlName);
			builder.Append(" PRIMARY KEY (");
			bool first = true;
			foreach(SqlColumn column in table.Columns)
			{
				if(column.IsKey)
				{
					if(first)
						first = false;
					else
						builder.Append(", ");
					builder.Append(this.FormatColumnName(column.NativeName));
				}
			}
			if(first)
				throw new InvalidOperationException(string.Format("Table '{0}' ('{1}') does not have a primary key defined.", table.Name, table.NativeName));
			builder.Append(")");
			builder.Append(this.StatementSeparator);

			// return...
			return builder.ToString();
		}
		
		/// <summary>
		/// Gets the statement that can drop the given table.
		/// </summary>
		/// <param name="table"></param>
		/// <returns></returns>
		public override string[] GetDropTableStatement(BootFX.Common.Data.Schema.SqlTable table)
		{
			if(table == null)
				throw new ArgumentNullException("table");
			
			return new string[] {string.Format("{0} {1}{2}", this.DropTableKeyword, this.FormatTableName(table.NativeName), this.StatementSeparator)};
		}

		/// <summary>
		/// Gets the statement that can create the given table.
		/// </summary>
		/// <param name="table"></param>
		/// <returns></returns>
		public override string[] GetCreateTableStatement(SqlTable table)
		{
			if(table == null)
				throw new ArgumentNullException("table");

			StringBuilder builder = new StringBuilder();

			builder.Append(this.CreateTableKeyword);
			builder.Append(" ");
			builder.Append(this.FormatTableName(table.NativeName));
			builder.Append(" (");
			for(int index = 0; index < table.Columns.Count; index++)
			{
				if(index > 0)
					builder.Append(", ");
				builder.Append("\r\n\t");

				// column...
				SqlColumn column = table.Columns[index];
				builder.Append(GetColumnSnippet(column));
			}
            builder.Append("\r\n\t");
            builder.Append(this.GetInlinePrimaryKeyClause(table));
            builder.Append(")");
			builder.Append(this.StatementSeparator);

			// append...
			builder.Append("\r\n");
			builder.Append(this.GetCreatePrimaryKeyConstraintStatement(table));
			
			// return...
			return new string[]{builder.ToString()};
		}

        protected virtual string GetInlinePrimaryKeyClause(SqlTable table)
        {
            return string.Empty;
        }

        public override string[] GetDropIndexStatement(SqlTable table, SqlIndex index)
		{
			//DROP INDEX CompaniesSiteIdActive ON dbo.SSCompanies
			if(index == null)
				throw new ArgumentNullException("table");

			StringBuilder builder = new StringBuilder();

			builder.Append(this.DropIndexKeyword);
			builder.Append(" ");

			// mbr - 14-03-2006 - changed this to be [table].[column].  not sure on SQL 2000 support.			
			builder.Append(FormatTableName(table.NativeName));
			builder.Append(".");
			builder.Append(this.FormatIndexName(index.NativeName));

			// return...
			return new string[]{builder.ToString()};
		}

		/// <summary>
		/// Gets the statement that can create the given table.
		/// </summary>
		/// <param name="table"></param>
		/// <returns></returns>
		public override string[] GetCreateIndexStatement(SqlTable table, SqlIndex index)
		{
			if(index == null)
				throw new ArgumentNullException("table");

			StringBuilder builder = new StringBuilder();

			if(!index.HasUniqueValues)
				builder.Append(this.CreateIndexKeyword);
			else
				builder.Append(this.CreateUniqueIndexKeyword);

			builder.Append(" ");
			builder.Append(this.FormatIndexName(index.NativeName));
			builder.Append(" ON ");
			builder.Append(FormatTableName(table.NativeName));

            // mbr -- which columns to use? mysql doesn't support included columns...
            var toUse = new List<SqlColumn>(index.Columns.ToArray());
            if (!(this.SupportsIncludedColumns))
                toUse.AddRange(index.IncludedColumns.ToArray());

			builder.Append(" (");
			
			for(int column = 0; column < toUse.Count; column++)
			{
				if(column > 0)
					builder.Append(", ");
				builder.Append("\r\n\t");

				// column...
				SqlColumn indexColumn = toUse[column];
				builder.Append(FormatIndexName(indexColumn.NativeName));
			}
			builder.Append("\r\n\t)");

            // mbr  - 2011-11-02 - include...
            if (this.SupportsIncludedColumns && index.IncludedColumns.Count > 0)
            {
                builder.Append(" INCLUDE ");
                builder.Append("(");
                for (int column = 0; column < index.IncludedColumns.Count; column++)
                {
                    if (column > 0)
                        builder.Append(", ");
                    SqlColumn indexColumn = index.IncludedColumns[column];
                    builder.Append(FormatIndexName(indexColumn.NativeName));
                }
                builder.Append(")");
            }

            // mbr - 2011-11-02 - calculated...
            if (index.ComputedColumns.Count > 0)
                throw new NotSupportedException("Computed columns are not supported.");
			
			// return...
			return new string[]{builder.ToString()};
		}

		/// <summary>
		/// Gets the statement that can create a foreign key.
		/// </summary>
		/// <returns></returns>
		public override string[] GetCreateForeignKeyStatement(SqlChildToParentLink foreignKey)
		{
			if(foreignKey == null)
				throw new ArgumentNullException("foreignKey");

			StringBuilder builder = new StringBuilder();

			// alter table authors add constraint pk_foobar primary key (au_id)
			builder.Append(this.AlterTableKeyword);
			builder.Append(" ");
			builder.Append(this.FormatTableName(foreignKey.Table.NativeName));
			builder.Append(" ");
			builder.Append(this.AddConstraintKeyword);
			builder.Append(" ");
			builder.Append(FormatForeignKeyName(foreignKey.NativeName));
			builder.Append(" FOREIGN KEY (");
			
			bool first = true;
			foreach(SqlColumn column in foreignKey.LinkFields)
			{
				if(first)
					first = false;
				else
					builder.Append(", ");
				builder.Append(this.FormatColumnName(column.NativeName));
			}
			builder.Append(") REFERENCES ");
			builder.Append(this.FormatTableName(foreignKey.ParentTable.NativeName));
			builder.Append(" (");

			first = true;
			foreach(SqlColumn column in foreignKey.ParentTable.GetKeyColumns())
			{
				if(first)
					first = false;
				else
					builder.Append(", ");
				builder.Append(this.FormatColumnName(column.NativeName));
			}
			builder.Append(")");

			builder.Append(this.StatementSeparator);

			// return...
			return new string[]{builder.ToString()};
		}

		/// <summary>
		/// Gets the statement that can create a foreign key.
		/// </summary>
		/// <returns></returns>
		public override string[] GetDropForeignKeyStatement(SqlChildToParentLink foreignKey)
		{
			if(foreignKey == null)
				throw new ArgumentNullException("foreignKey");

			StringBuilder builder = new StringBuilder();
			builder = new StringBuilder();
			builder.Append(this.AlterTableKeyword);
			builder.Append(" ");
			builder.Append(this.FormatTableName(foreignKey.Table.NativeName));
			builder.Append(" ");
			builder.Append(" DROP CONSTRAINT ");
			builder.Append(" ");
			builder.Append(this.FormatConstraintName(foreignKey.NativeName));

			return new string[] {builder.ToString()};
		}


		public override string[] GetDropConstraintStatement(SqlColumn column, SqlDatabaseDefault databaseDefault)
		{
			if(column == null)
				throw new ArgumentNullException("column");
			if(databaseDefault == null)
				throw new ArgumentNullException("databaseDefault");

			StringBuilder builder = new StringBuilder();
			builder = new StringBuilder();
			builder.Append(this.AlterTableKeyword);
			builder.Append(" ");
			builder.Append(this.FormatTableName(column.Table.NativeName));
			builder.Append(" ");
			builder.Append(" DROP CONSTRAINT ");
			builder.Append(" ");
			builder.Append(this.FormatConstraintName(databaseDefault.Name));

			return new string[] {builder.ToString()};
		}

		public override string[] GetCreateDefaultConstraintStatement(SqlColumn column, SqlDatabaseDefault databaseDefault)
		{
			if(column == null)
				throw new ArgumentNullException("column");
			if(databaseDefault == null)
				throw new ArgumentNullException("databaseDefault");

			StringBuilder builder = new StringBuilder();

			builder.Append(this.AlterTableKeyword);
			builder.Append(" ");
			builder.Append(this.FormatTableName(column.Table.NativeName));
			builder.Append(" ");
			builder.Append(" ADD ");
			builder.Append(" ");
			builder.Append(GetColumnConstraintSnippet(column,databaseDefault));
			builder.Append(" FOR ");
			builder.Append(this.FormatColumnName(column.NativeName));

			return new string[] {builder.ToString()};
		}


		public override string[] GetAddColumnStatement(SqlColumn column, bool add)
		{
			if(column == null)
				throw new ArgumentNullException("column");
			ArrayList statements = new ArrayList();

			StringBuilder builder = new StringBuilder();
			builder.Append(this.AlterTableKeyword);
			builder.Append(" ");
			builder.Append(this.FormatTableName(column.Table.NativeName));
			builder.Append(" ");
            if (add)
                builder.Append("ADD");
            else
            {
                //builder.Append("ALTER COLUMN");
                builder.Append(" ");
                builder.Append(this.AlterColumnKeyword);
                builder.Append(" ");
            }
            builder.Append(" ");
			builder.Append(GetColumnSnippet(column));

			if(column.HasDefaultExpression && add)
			{
				builder.Append(" ");
				builder.Append(GetColumnConstraintSnippet(column,column.DefaultExpression));
				//statements.AddRange(GetCreateConstraintStatement(column,column.DefaultExpression));
			}

			statements.Add(builder.ToString());

			// return...
			return (string[]) statements.ToArray(typeof(string));
		}

		public override string SetKeyword
		{
			get
			{
				return base.SetKeyword;
			}
		}

		protected override void SetupPopulateDataScript(SqlTable table, StringBuilder builder)
		{
			base.SetupPopulateDataScript (table, builder);

			// insert...
			builder.Append("SET IDENTITY_INSERT ");
			builder.Append(this.FormatTableName(table));
			builder.Append(" ON");
			builder.Append(this.StatementSeparator);
			builder.Append("\r\n");
		}

		protected override void TeardownPopulateDataScript(SqlTable table, StringBuilder builder)
		{
			// insert...
			builder.Append("SET IDENTITY_INSERT ");
			builder.Append(this.FormatTableName(table));
			builder.Append(" OFF");
			builder.Append(this.StatementSeparator);
			builder.Append("\r\n");

			// base...
			base.TeardownPopulateDataScript (table, builder);
		}

        protected virtual bool SupportsIncludedColumns
        {
            get
            {
                return true;
            }
        }
	}
}
