// BootFX - Application framework for .NET applications
// 
// File: FilterConstraint.cs
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
using BootFX.Common.Entities;
using System.Collections.Generic;

namespace BootFX.Common.Data
{
    /// <summary>
    /// TODO: enter class description.  
    /// </summary>
    [Serializable]
    public class FilterConstraint
	{
		/// <summary>
		/// Private field to support <c>Filter</c> property.
		/// </summary>
		private SqlStatementCreator _creator;
		
		/// <summary>
		/// Private field to support <c>ChildContraints</c> property.
		/// </summary>
		private FilterConstraintCollection _childConstraints;
		
		/// <summary>
		/// Private field to support <c>CombineWithFirstChild</c> property.
		/// </summary>
		private SqlCombine _combineWithFirstChild = SqlCombine.And;		

		/// <summary>
		/// Private field to support <c>CombineWithNext</c> property.
		/// </summary>
		private SqlCombine _combineWithNext = SqlCombine.And;

		/// <summary>
		/// Constructor.
		/// </summary>
		internal FilterConstraint(SqlStatementCreator creator)
		{
			// assertions...
			if(creator == null)
				throw new ArgumentNullException("creator");
			
			// set...
			_creator = creator;
		}

		/// <summary>
		/// Gets the filter.
		/// </summary>
		public SqlStatementCreator Creator
		{
			get
			{
				return _creator;
			}
		}
		/// <summary>
		/// Gets or sets the combinewithnext
		/// </summary>
		public SqlCombine CombineWithNext
		{
			get
			{
				return _combineWithNext;
			}
			set
			{
				// check to see if the value has changed...
				if(value != _combineWithNext)
				{
					// set the value...
					_combineWithNext = value;
				}
			}
		}

		/// <summary>
		/// Gets a collection of FilterConstraint objects.
		/// </summary>
		public FilterConstraintCollection ChildConstraints
		{
			get
			{
				if(_childConstraints == null)
				{
					if(Creator == null)
						throw new ArgumentNullException("Creator");
					_childConstraints = new FilterConstraintCollection(this.Creator);
				}
				return _childConstraints;
			}
		}

		/// <summary>
		/// Gets or sets the combinewithfirstchild
		/// </summary>
		public SqlCombine CombineWithFirstChild
		{
			get
			{
				return _combineWithFirstChild;
			}
			set
			{
				// check to see if the value has changed...
				if(value != _combineWithFirstChild)
				{
					// set the value...
					_combineWithFirstChild = value;
				}
			}
		}

		/// <summary>
		/// Appends data to the filter.
		/// </summary>
		/// <param name="sql"></param>
		public virtual void Append(FilterConstraintAppendContext context)
		{
		}

		
        //public static string AppendSqlOperator(FilterConstraintAppendContext context, SqlFilter filter, string fieldName, SqlOperator sqlOperator, DbType dbType, object value)
        //{
        //    StringBuilder sql = new StringBuilder(); 
        //    SqlStatement statement = filter.GetStatement();
        //    AppendSqlOperator(sqlOperator,new EntityField(fieldName,fieldName,dbType,EntityFieldFlags.Normal,4000), sql,value,statement,context);

        //    foreach(SqlStatementParameter parameter in statement.Parameters)
        //        filter.ExtraParameters.Add(parameter);

        //    return sql.ToString();
        //}

		public static IEnumerable<SqlStatementParameter> AppendSqlOperator(SqlOperator sqlOperator, EntityField field, StringBuilder sql, object value, SqlStatement statement, FilterConstraintAppendContext context)
		{
            var results = new List<SqlStatementParameter>();

			// any mangling?
			bool addParameter = true;
			sql.Append(" ");
			switch(sqlOperator)
			{
				case SqlOperator.EqualTo:
				case SqlOperator.NotEqualTo:
				case SqlOperator.GreaterThan:
				case SqlOperator.GreaterThanOrEqualTo:
				case SqlOperator.LessThan:
				case SqlOperator.LessThanOrEqualTo:
					if(!(value is DBNull))
						sql.Append(statement.Dialect.GetOperatorKeyword(sqlOperator, field.DBType));
					else
					{
						sql.Append(statement.Dialect.IsKeyword);
						sql.Append(" ");
						if(sqlOperator == SqlOperator.NotEqualTo)
						{
							sql.Append(statement.Dialect.NotKeyword);
							sql.Append(" ");
						}
						sql.Append(statement.Dialect.NullKeyword);

						// no need for param...
						addParameter = false;
					}
					break;

				case SqlOperator.StartsWith:
				case SqlOperator.NotStartsWith:
				case SqlOperator.EndsWith:
				case SqlOperator.NotEndsWidth:
				case SqlOperator.Contains:
				case SqlOperator.NotContains:

					// add...
				    switch(sqlOperator)
				    {
					    case SqlOperator.NotStartsWith:
					    case SqlOperator.NotEndsWidth:
					    case SqlOperator.NotContains:
						    sql.Append(statement.Dialect.NotKeyword);
						    sql.Append(" ");
						    break;
    				}
					sql.Append(statement.Dialect.LikeKeyword);

					// mangle...
					if(value is DBNull)
						throw new InvalidOperationException(string.Format(Cultures.Exceptions, "Cannot do a '{0}' query with a null value.", sqlOperator));
					value = ConversionHelper.ToString(value, Cultures.System);

					// append...
				switch(sqlOperator)
				{
					case SqlOperator.StartsWith:
					case SqlOperator.NotStartsWith:
						value = (string)value + "%";
						break;

					case SqlOperator.EndsWith:
					case SqlOperator.NotEndsWidth:
						value = "%" + (string)value;
						break;

					case SqlOperator.Contains:
					case SqlOperator.NotContains:
						value = "%" + (string)value + "%";
						break;

					default:
						throw ExceptionHelper.CreateCannotHandleException(sqlOperator);
				}

					// done...
					break;

				case SqlOperator.Between:
				case SqlOperator.NotBetween:

					// values...
					// mbr - 21-04-2006 - datetime[] is not an object[]...
					//object[] values = (object[])value;
					Array values = (Array)value;

					// create...
					if(sqlOperator == SqlOperator.NotBetween)
					{
						sql.Append(statement.Dialect.NotKeyword);
						sql.Append(" ");
					}
					sql.Append(statement.Dialect.BetweenKeyword);
					sql.Append(" ");

					// first... 
					SqlStatementParameter parameter = statement.OriginalCreator.CreateParameter(field, values.GetValue(0), context.CreateConstraintName());
					statement.Parameters.Add(parameter);
                    results.Add(parameter);
					sql.Append(statement.Dialect.FormatVariableNameForQueryText(parameter.Name));

					// and...
					sql.Append(" ");
					sql.Append(statement.Dialect.AndKeyword);
					sql.Append(" ");

					// second...
					parameter = statement.OriginalCreator.CreateParameter(field, values.GetValue(1), context.CreateConstraintName());
					statement.Parameters.Add(parameter);
                    results.Add(parameter);
                    sql.Append(statement.Dialect.FormatVariableNameForQueryText(parameter.Name));

					// don't do default...
					addParameter = false;
					break;

				case SqlOperator.BitwiseAnd:
					
					// add...
					SqlStatementParameter bitwiseParameter = statement.OriginalCreator.CreateParameter(field, value, context.CreateConstraintName());
					statement.Parameters.Add(bitwiseParameter); // Check if the parameter has already been added
                    results.Add(bitwiseParameter);

					sql.Append(" ");
					sql.Append(statement.Dialect.AsKeyword);
					sql.Append(" ");
					sql.Append(statement.Dialect.GetColumnTypeNativeName(DbType.Int32,false,0));
					sql.Append(")");
					sql.Append(" ");
					sql.Append(statement.Dialect.BitwiseAndKeyword);
					sql.Append(" ");
					sql.Append(statement.Dialect.FormatVariableNameForQueryText(bitwiseParameter.Name));
					sql.Append(")");
					sql.Append(statement.Dialect.GetOperatorKeyword(SqlOperator.EqualTo, field.DBType));
					sql.Append(statement.Dialect.FormatVariableNameForQueryText(bitwiseParameter.Name));

					addParameter = false;

					break;

				default:
					throw ExceptionHelper.CreateCannotHandleException(sqlOperator);
			}
	
			// value...
			if(addParameter == true)
			{
				// space...
				sql.Append(" ");

				// add...
				SqlStatementParameter parameter = statement.OriginalCreator.CreateParameter(field, value, context.CreateConstraintName());
                statement.Parameters.Add(parameter); // Check if the parameter has already been added
                results.Add(parameter);

				sql.Append(statement.Dialect.FormatVariableNameForQueryText(parameter));
			}

            return results;
		}
	}
}
