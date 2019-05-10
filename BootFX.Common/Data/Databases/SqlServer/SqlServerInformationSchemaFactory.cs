// BootFX - Application framework for .NET applications
// 
// File: SqlServerInformationSchemaFactory.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;

namespace BootFX.Common.Data.Schema
{
	/// <summary>
	/// Defines an instance of <c>SqlServerInformationSchemaFactory</c>.
	/// </summary>
	internal class SqlServerInformationSchemaFactory : InformationSchemaFactory
	{
		/// <summary>
		/// Private field to support <see cref="HeaderRegex"/> property.
		/// </summary>
		private static Regex _headerRegex = new Regex(@"create\s+procedure\s+(?<name>[\[\]\w\s\._-]+)", RegexOptions.IgnoreCase | RegexOptions.Multiline);

		/// <summary>
		/// Private field to support <see cref="AsRegex"/> property.
		/// </summary>
		private static Regex _asRegex = new Regex(@"\bas\b", RegexOptions.IgnoreCase | RegexOptions.Multiline);
		
		/// <summary>
		/// Private field to support <see cref="ParamsRegex"/> property.
		/// </summary>
		private static Regex _paramsRegex = new Regex(@"@(?<name>[\[\]\w\._-]+)\s+(?<type>\w+)\s*(\((?<dimension>[\d\,]+)\))?\s*(=\s*(?<default>null|'[^']*'))?\s*(?<direction>out(put)?)?", RegexOptions.IgnoreCase | RegexOptions.Multiline);

		/// <summary>
		/// Private field to support <c>LiteralStringDefaultRegex</c> property.
		/// </summary>
		private Regex _integerRegex = new Regex(@"^[0-9\.]+$", RegexOptions.Singleline | RegexOptions.IgnoreCase);
		
		internal SqlServerInformationSchemaFactory(SqlServerConnection connection) : base(connection)
		{
		}

		public override SqlSchema GetSchema(GetSchemaArgs args)
		{
			SqlSchema schema = base.GetSchema(args);
			if(schema == null)
				throw new InvalidOperationException("schema is null.");

			// mbr - 02-10-2007 - for c7 - if we have a search spec, don't do sprocs...
			if(args.ConstrainTableNames.Count == 0)
			{
				// mbr - 15-06-2006 - now do procedures...
				DataTable table = this.Connection.ExecuteDataTable(new SqlStatement("select specific_name, routine_definition from information_schema.routines where routine_type='PROCEDURE' order by specific_name",
                    this.Dialect));
				foreach(DataRow row in table.Rows)
				{
					try
					{
						// create...
						SqlProcedure proc = this.GetSchemaProcedure(schema, row);
						schema.Procedures.Add(proc);
					}
					catch(Exception ex)
					{
						throw new InvalidOperationException(string.Format("Failed to handle stored procedure '{0}'.\r\nBody: {1}", row["specific_name"], row["routine_definition"]), ex);
					}
				}
			}

			// return...
			return schema;
		}

		protected override void TableDiscovered(SqlTable schemaTable)
		{
			base.TableDiscovered (schemaTable);

			// run...
			if(Connection == null)
				throw new InvalidOperationException("Connection is null.");
			// sp_MShelpcolumns N'dbo.SSActivities', null, 'id'
			SqlStatement statement = new SqlStatement("sp_MShelpcolumns", this.Dialect);
			statement.CommandType = CommandType.StoredProcedure;
			statement.Parameters.Add(new SqlStatementParameter("tablename", DbType.String, schemaTable.NativeName));
			statement.Parameters.Add(new SqlStatementParameter("flags", DbType.Int32, 0));
			statement.Parameters.Add(new SqlStatementParameter("tablename", DbType.String, "id"));

			// run...
			DataTable table = this.Connection.ExecuteDataTable(statement);
			if(table == null)
				throw new InvalidOperationException("table is null.");

			// walk each row...
			foreach(DataRow row in table.Rows)
			{
				// find the column...
				string name = (string)row["col_name"];
				SqlColumn column = schemaTable.Columns[name];
				if(column == null)
					throw new InvalidOperationException(string.Format("A column with name '{0}' was not found.", name));

				// fix...
				this.FixupColumnFromMsHelpColumns(column, row);
			}
		}

		private void FixupColumnFromMsHelpColumns(SqlColumn column, DataRow row)
		{
			if(column == null)
				throw new ArgumentNullException("column");
			if(row == null)
				throw new ArgumentNullException("row");
			
			// get the default expression name...
			string expressionName = null;
			if(!(row.IsNull("col_dridefname")))
				expressionName = (string)row["col_dridefname"];

			// got one?
			column.DefaultExpression = null;
			if(expressionName != null && expressionName.Length > 0)
			{
				// gets the expression...
				string asString = this.ExecuteHelpText(expressionName);
				if(asString != null)
				{
					string rawExpression = asString;

					// as...
					asString = asString.Trim();
					if(asString.StartsWith("((") && asString.EndsWith("))"))
						asString = asString.Substring(2, asString.Length - 4);
					if(asString.StartsWith("(") && asString.EndsWith(")"))
						asString = asString.Substring(1, asString.Length - 2);
					if(asString.Length > 0)
					{
						// does it look like a literal string?  does it start ' and end ', or start N' and end '.
						if((asString.StartsWith("N'") || asString.StartsWith("'")) && asString.EndsWith("'"))
						{
							// mangle...
							if(asString.StartsWith("N"))
								asString = asString.Substring(2);
							else
								asString = asString.Substring(1);
							asString = asString.Substring(0, asString.Length - 1);
							
							// remove the initial paren...
							if(asString.StartsWith("('"))
								asString = asString.Substring(2);
							if(asString.EndsWith("')"))
								asString = asString.Substring(0, asString.Length - 2);

							// mangle it...
							asString = asString.Replace("''", "'");

							// must be a string...
							column.DefaultExpression = new SqlDatabaseDefault(SqlDatabaseDefaultType.Primitive, asString, (string)row[14]);
						}
						else
						{
							// int?
							Match match = this.IntegerRegex.Match(asString);
							if(match.Success)
							{
								// mbr - 14-02-2007 - changed this to support requirement of having decimal defaults.								
								object value = null;
								if(asString.IndexOf(".") != -1)
									value = ConversionHelper.ToDecimal(asString, Cultures.System);
								else
									value = ConversionHelper.ToInt64(asString, Cultures.System);

								// set...
								column.DefaultExpression = new SqlDatabaseDefault(SqlDatabaseDefaultType.Primitive, value, (string)row[14]);
							}
							else if(asString.ToLower() == "getdate()")
								column.DefaultExpression = new SqlDatabaseDefault(SqlDatabaseDefaultType.CurrentDateTime, null,(string)row[14]);
							else
							{
								// if we get here, store it as a literal...
								column.DefaultExpression = new SqlDatabaseDefault(SqlDatabaseDefaultType.Literal, rawExpression, (string)row[14]);
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// Gets the literalstringdefaultregex.
		/// </summary>
		private Regex IntegerRegex
		{
			get
			{
				// returns the value...
				return _integerRegex;
			}
		}

		/// <summary>
		/// Executes the sp_helptext sproc against the given object.
		/// </summary>
		/// <param name="objectName"></param>
		/// <returns></returns>
		private string ExecuteHelpText(string objectName)
		{
			if(objectName == null)
				throw new ArgumentNullException("objectName");
			if(objectName.Length == 0)
				throw new ArgumentOutOfRangeException("'objectName' is zero-length.");
			
			// run...
			SqlStatement statement = new SqlStatement("sp_helptext", this.Dialect);
			statement.CommandType = CommandType.StoredProcedure;
			statement.Parameters.Add(new SqlStatementParameter("objname", DbType.String, objectName));

			// run it...
			object asObject = this.Connection.ExecuteScalar(statement);
			if(asObject != null && !(asObject is DBNull))
				return ConversionHelper.ToString(asObject, Cultures.System);
			else
				return null;
		}
	
		private SqlProcedure GetSchemaProcedure(SqlSchema schema, DataRow row)
		{
			if(schema == null)
				throw new ArgumentNullException("schema");			
			if(row == null)
				throw new ArgumentNullException("row");
			
			// create...
			SqlProcedure proc = new SqlProcedure((string)row["specific_name"]);
			proc.Body = ConversionHelper.ToString(row["routine_definition"], Cultures.System);

			// handle...
			this.ParseProcedureBody(schema, proc);

			// return...
			return proc;
		}

		private void ParseProcedureBody(SqlSchema schema, SqlProcedure proc)
		{
			if(schema == null)
				throw new ArgumentNullException("schema");
			if(proc == null)
				throw new ArgumentNullException("proc");	
			
			// reset...
			proc.Parameters.Clear();
			if(proc.Body == null || proc.Body.Length == 0)
				return;

			// add the params...
			Match asMatch = AsRegex.Match(proc.Body);
			if(asMatch.Success)
			{
				// header...
				string header = proc.Body.Substring(0, asMatch.Index);

				// get...
				foreach(Match paramMatch in ParamsRegex.Matches(header))
				{
					string name = paramMatch.Groups["name"].Value;
					if(name == null)
						throw new InvalidOperationException("'name' is null.");
					if(name.Length == 0)
						throw new InvalidOperationException("'name' is zero-length.");

					try
					{
						// type...
						string typeName = paramMatch.Groups["type"].Value;
						if(typeName == null)
							throw new InvalidOperationException("'typeName' is null.");
						if(typeName.Length == 0)
							throw new InvalidOperationException("'typeName' is zero-length.");

						// mbr - 26-02-2007 - is the type name parameterized?
						typeName = typeName.ToLower();

						// are we output?
						const string outputDirective = " output";
						if(typeName.EndsWith(outputDirective))
							typeName = typeName.Substring(typeName.Length - outputDirective.Length).TrimEnd();

						// field...
						FieldSpecification spec = this.Connection.GetFieldSpecificationForNativeTypeName(typeName, false);
                        if (spec != null)
                        {
                            DbType dbType = spec.DbType;

                            // dimension et al...
                            string dimensionAsString = paramMatch.Groups["dimension"].Value;
                            string defaultValueAsString = paramMatch.Groups["default"].Value;

                            // direction...
                            string directionAsString = paramMatch.Groups["direction"].Value;
                            ParameterDirection direction = ParameterDirection.Input;
                            if (directionAsString != null && directionAsString.Length > 0)
                            {
                                directionAsString = directionAsString.ToLower();
                                if (directionAsString.StartsWith("out"))
                                    direction = ParameterDirection.Output;
                            }

                            // value...
                            object defaultValue = DBNull.Value;

                            // create...
                            proc.Parameters.Add(new SqlStatementParameter(name, dbType, defaultValue, direction));
                        }
                        else
                            proc.Parameters.Add(new SqlStatementParameter(name, DbType.Object, DBNull.Value));
					}
					catch(Exception ex)
					{
						throw new InvalidOperationException(string.Format("Failed when processing parameter '{0}'\r\nText: {1}", name, paramMatch.Value), ex);
					}
				}
			}
		}

		private new SqlServerConnection Connection
		{
			get
			{
				return (SqlServerConnection)base.Connection;
			}
		}
		
		/// <summary>
		/// Gets the paramsregex.
		/// </summary>
		private static Regex ParamsRegex
		{
			get
			{
				return _paramsRegex;
			}
		}
		
		/// <summary>
		/// Gets the asregex.
		/// </summary>
		private static Regex AsRegex
		{
			get
			{
				return _asRegex;
			}
		}
		
		/// <summary>
		/// Gets the headerregex.
		/// </summary>
		private static Regex HeaderRegex
		{
			get
			{
				return _headerRegex;
			}
		}
	}
}
