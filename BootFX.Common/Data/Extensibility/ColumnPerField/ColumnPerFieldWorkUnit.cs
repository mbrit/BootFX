// BootFX - Application framework for .NET applications
// 
// File: ColumnPerFieldWorkUnit.cs
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
	/// Summary description for CustomWorkUnit.
	/// </summary>
	// mbr - 29-11-2007 - for c7 - added IExtendedDataWorkUnit...
//	internal abstract class ColumnPerFieldWorkUnit : WorkUnit
	internal abstract class ColumnPerFieldWorkUnit : WorkUnit, IExtendedDataWorkUnit
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="entityType"></param>
		/// <param name="entity"></param>
		/// <param name="fields"></param>
		/// <param name="values"></param>
		internal ColumnPerFieldWorkUnit(EntityType entityType, object entity, EntityField[] fields, object[] values)
			: base(entityType, entity, fields, values)
		{
		}

		
		/// <summary>
		/// Returns true if the extension row for the entity exists.
		/// </summary>
		/// <returns></returns>
		protected bool DoesExtendedRowExist(WorkUnitProcessingContext context)
		{
			if(context == null)
				throw new ArgumentNullException("context");

			// values...
			EntityField[] keyFields = this.EntityType.GetKeyFields();
			this.AssertKeyFields(keyFields);

			// values...
			object[] values = this.EntityType.Storage.GetKeyValues(this.Entity);
			if(values == null)
				throw new InvalidOperationException("values is null.");
			if(keyFields.Length != values.Length)
				throw new InvalidOperationException(string.Format("Length mismatch for 'keyFields' and 'values': {0} cf {1}.", keyFields.Length, values.Length));

			// sql...
			SqlStatement sql = new SqlStatement();
			StringBuilder builder = new StringBuilder();

			// builder...
			builder.Append("SELECT ");
			for(int index = 0; index < keyFields.Length; index++)
			{
				if(index > 0)
					builder.Append(", ");
				builder.Append(sql.Dialect.FormatColumnName(
					ColumnPerFieldExtensibilityProvider.MangleIdColumnName(keyFields[index].NativeName)));
			}

			// from...
			builder.Append(" FROM ");
			builder.Append(sql.Dialect.FormatTableName(this.EntityType.NativeNameExtended));

			// values...
			this.AppendWhereClause(sql, builder, keyFields, values);

			// set...
			sql.CommandText = builder.ToString();

			// result...
			bool result = context.Connection.ExecuteExists(sql);
			return result;
		}

		protected void AssertKeyFields(EntityField[] keyFields)
		{
			if(keyFields == null)
				throw new ArgumentNullException("keyFields");
			
			// only support one...
			if(keyFields.Length != 1)
			{
				throw new InvalidOperationException(string.Format("Entity type '{0}' has '{1}' key fields, which is invalid.", 
					this.EntityType, keyFields.Length));
			}
		}

		/// <summary>
		/// Appends a WHERE clause to a statement.
		/// </summary>
		/// <param name="sql"></param>
		/// <param name="builder"></param>
		/// <param name="keyFields"></param>
		/// <param name="keyValues"></param>
		protected void AppendWhereClause(SqlStatement sql, StringBuilder builder, EntityField[] keyFields, object[] keyValues)
		{
			if(sql == null)
				throw new ArgumentNullException("sql");
			if(builder == null)
				throw new ArgumentNullException("builder");
			if(keyFields == null)
				throw new ArgumentNullException("keyFields");
			if(keyFields.Length == 0)
				throw new ArgumentOutOfRangeException("'keyFields' is zero-length.");
			if(keyValues == null)
				throw new ArgumentNullException("keyValues");
			if(keyFields.Length != keyValues.Length)
				throw new InvalidOperationException(string.Format("Length mismatch for 'keyFields' and 'keyValues': {0} cf {1}.", keyFields.Length, keyValues.Length));

			// where...
			builder.Append(" WHERE ");
			for(int index = 0; index < keyFields.Length; index++)
			{
				if(index > 0)
					builder.Append(" AND ");
				builder.Append(sql.Dialect.FormatColumnName(
					ColumnPerFieldExtensibilityProvider.MangleIdColumnName(keyFields[index].NativeName)));
				builder.Append("=");
                builder.Append(sql.Dialect.FormatVariableNameForQueryText(sql.Parameters.Add(keyFields[index].DBType, keyValues[index])));
			}
		}
	}
}
