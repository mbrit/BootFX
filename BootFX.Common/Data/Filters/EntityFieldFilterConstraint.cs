// BootFX - Application framework for .NET applications
// 
// File: EntityFieldFilterConstraint.cs
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

namespace BootFX.Common.Data
{
    /// <summary>
    /// TODO: enter class description.  
    /// </summary>
    [Serializable]
    public class EntityFieldFilterConstraint : FilterConstraint
	{
		/// <summary>
		/// Private field to support <c>Field</c> property.
		/// </summary>
		private EntityField _field;

		/// <summary>
		/// Private field to support <c>SqlOperator</c> property.
		/// </summary>
		private SqlOperator _filterOperator;

		/// <summary>
		/// Private field to support <c>Value</c> property.
		/// </summary>
		private object _value;

		/// <summary>
		/// Constructor.
		/// </summary>
		internal EntityFieldFilterConstraint(SqlStatementCreator creator, EntityField field, SqlOperator filterOperator, object value) : base(creator)
		{
			// assertions...
			if(field == null)
				throw new ArgumentNullException("field");

			// check...
			if(IsOperatorValidForType(filterOperator, field.DBType) == false)
				throw new InvalidOperationException(string.Format(Cultures.Exceptions, "Operator '{0}' cannot be used with '{1}' fields.", filterOperator, field.DBType));

			// is the field encrypted?
			//if(field.IsEncrypted)
			//{
			//	// switch...
			//	if(filterOperator != SqlOperator.EqualTo)
			//		throw new NotSupportedException(string.Format("'{0}' is encrypted.  Encrypted fields cannot support the '{1}' operator.", 
			//			field, filterOperator));
			//}
			
			// set...
			_field = field;
			_filterOperator = filterOperator;
			_value = value;
		}

		/// <summary>
		/// Returns true if the operator can be used with the given type.
		/// </summary>
		/// <param name="filterOperator"></param>
		/// <param name="dbType"></param>
		/// <returns></returns>
		public static bool IsOperatorValidForType(SqlOperator filterOperator, DbType dbType)
		{
			switch(filterOperator)
			{
				case SqlOperator.EqualTo:
				case SqlOperator.NotEqualTo:
				case SqlOperator.GreaterThan:
				case SqlOperator.GreaterThanOrEqualTo:
				case SqlOperator.LessThan:
				case SqlOperator.LessThanOrEqualTo:
				case SqlOperator.Between:
				case SqlOperator.NotBetween:
				case SqlOperator.BitwiseAnd:
					return true;

				case SqlOperator.StartsWith:
				case SqlOperator.NotStartsWith:
				case SqlOperator.EndsWith:
				case SqlOperator.NotEndsWidth:
				case SqlOperator.Contains:
				case SqlOperator.NotContains:

					// only if it's a text operator...
				switch(dbType)
				{
					case DbType.String:
					case DbType.StringFixedLength:
					case DbType.AnsiString:
					case DbType.AnsiStringFixedLength:
						return true;

					default:
						return false;
				}

				default:
					throw ExceptionHelper.CreateCannotHandleException(filterOperator);
			}
		}

		/// <summary>
		/// Gets the field.
		/// </summary>
		public EntityField Field
		{
			get
			{
				return _field;
			}
		}

		/// <summary>
		/// Gets the filter operator.
		/// </summary>
		public SqlOperator FilterOperator
		{
			get
			{
				return _filterOperator;
			}
		}

		/// <summary>
		/// Gets the value.
		/// </summary>
		public object Value
		{
			get
			{
				return _value;
			}
		}

		public override void Append(FilterConstraintAppendContext context)
		{
			if(context == null)
				throw new ArgumentNullException("context");
			
			// get...
			SqlStatement statement = context.Statement;
			if(statement == null)
				throw new ArgumentNullException("statement");
			StringBuilder sql = context.Sql;
			if(sql == null)
				throw new ArgumentNullException("sql");
			if(Creator == null)
				throw new ArgumentNullException("Creator");

			// do the column name...
			if(this.Field.IsExtendedProperty)
			{
				// mbr - 25-09-2007 - moved to provider.				
				if(Database.ExtensibilityProvider == null)
					throw new InvalidOperationException("Database.ExtensibilityProvider is null.");
				Database.ExtensibilityProvider.AppendColumnNameForEntityFieldFilterConstraint(context, this, statement,
					sql, this.Field);
			}
			else
			{
				AppendBitwiseOperators(sql,statement);

                // mbr - 2011-08-30 - join support - are we needing to do qualified names?  support wasn't included here before.  the second
                // clause seems strange, but is legacy...
                if (context.Creator.UseFullyQualifiedNames)
                    sql.Append(statement.Dialect.FormatColumnName(this.Field, true));
                else
                    sql.Append(statement.Dialect.FormatNativeName(this.Field.NativeName));
			}

			// get the value to use...
			object value = this.Value;
			if(value is Array)
			{
				// only support arrays on between...
				// mbr - 21-04-2006 - DateTime[] for example is not an object[].  Use Array instead.
				//object[] values = (object[])value;
				Array values = (Array)value;
				if(this.FilterOperator == SqlOperator.Between || this.FilterOperator == SqlOperator.NotBetween)
				{
					// check...
					if(values.Length != 2)
						throw new InvalidOperationException(string.Format(Cultures.Exceptions, "Array length for 'between' operators must be 2, not '{0}'.", values.Length));

					// check...
					if(values.GetValue(0) == null)
						throw new InvalidOperationException("First 'between' value cannot be CLR null.");
					if(values.GetValue(0) is DBNull)
						throw new InvalidOperationException("First 'between' value cannot be DB null");
					if(values.GetValue(1) == null)
						throw new InvalidOperationException("Second 'between' value cannot be null.");
					if(values.GetValue(1) is DBNull)
						throw new InvalidOperationException("Second 'between' value cannot be DB null.");
				}
				else
					throw new InvalidOperationException(string.Format(Cultures.Exceptions, "Array values are only supported with 'between' operators."));
			}
			else if(value is DBNull)
			{
				// check...
				if(SqlFilter.IsDBNullValidForOperator(this.FilterOperator) == false)
					throw new InvalidOperationException(string.Format(Cultures.Exceptions, "Operator '{0}' cannot be used with DB null.", value));
			}

			// Appends the filter to the sql builder
			var added = AppendSqlOperator(FilterOperator, Field, sql, value, statement, context);
            foreach (var param in added)
                param.OriginalConstraint = this;

			// mbr - 25-09-2007 - remove this weird code about the extended properties...			
//			if(this.Field.IsExtendedProperty || this.FilterOperator == SqlOperator.BitwiseAnd)
			if((this.Field.IsExtendedProperty && Database.ExtensibilityProvider is FlatTableExtensibilityProvider) || 
				this.FilterOperator == SqlOperator.BitwiseAnd)
			{
				sql.Append(")");
			}
		}

		/// <summary>
		/// Append bitwise operators to the SQL string if required
		/// </summary>
		/// <param name="sql"></param>
		/// <param name="statement"></param>
		internal void AppendBitwiseOperators(StringBuilder sql, SqlStatement statement)
		{
			if(sql == null)
				throw new ArgumentNullException("sql");
			if(statement == null)
				throw new ArgumentNullException("statement");		

			// Check if we are a bitwise filter, if so we must be processed first
			if(this.FilterOperator == SqlOperator.BitwiseAnd)
			{
				if(Value == null) 
					throw new InvalidOperationException(string.Format(Cultures.Exceptions, "Cannot do a '{0}' operation with a CLR null value.", this.FilterOperator));
				if(!(Value is int))
					throw new InvalidOperationException(string.Format(Cultures.Exceptions, "Bitwise operations work only in int32 values, not '{0}'.", Value.GetType()));
			
				sql.Append("(");
				sql.Append(statement.Dialect.CastKeyword);
				sql.Append("(");
			}
		}
	}
}
