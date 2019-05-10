// BootFX - Application framework for .NET applications
// 
// File: ColumnPerFieldUpdateWorkUnit.cs
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
using BootFX.Common;
using BootFX.Common.Entities;

namespace BootFX.Common.Data
{
	/// <summary>
	/// Summary description for CustomUpdateWorkUnit.
	/// </summary>
	internal class ColumnPerFieldUpdateWorkUnit : ColumnPerFieldInsertWorkUnit
	{
		internal ColumnPerFieldUpdateWorkUnit(EntityType entityType, object entity, EntityField[] fields, object[] values)
			: base(entityType, entity, fields, values)
		{
		}

		public override SqlStatement[] GetStatements(WorkUnitProcessingContext context)
		{
			// job one - find out if we have an item...
			bool exists = this.DoesExtendedRowExist(context);
			if(exists)
				return this.GetUpdateStatements(context);
			else
				return this.GetInsertStatements(context);
		}

		/// <summary>
		/// Gets the statements to update data in the related table.
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		private SqlStatement[] GetUpdateStatements(WorkUnitProcessingContext context)
		{
			if(context == null)
				throw new ArgumentNullException("context");
			
			// key fields...
			EntityField[] keyFields = this.EntityType.GetKeyFields();
			this.AssertKeyFields(keyFields);

			// key values...
			object[] keyValues = this.EntityType.Storage.GetKeyValues(this.Entity);
			if(keyValues == null)
				throw new InvalidOperationException("keyValues is null.");
			if(keyFields.Length != keyValues.Length)
				throw new InvalidOperationException(string.Format("Length mismatch for 'keyFields' and 'keyValues': {0} cf {1}.", keyFields.Length, keyValues.Length));

			// fields...
			EntityField[] fields = this.GetFields();
			if(fields == null)
				throw new InvalidOperationException("'fields' is null.");
			if(fields.Length == 0)
				throw new InvalidOperationException("'fields' is zero-length.");

			// values...
			object[] values = this.GetValues();
			if(values == null)
				throw new InvalidOperationException("values is null.");
			if(fields.Length != values.Length)
				throw new InvalidOperationException(string.Format("Length mismatch for 'fields' and 'values': {0} cf {1}.", fields.Length, values.Length));

			// sql...
			SqlStatement sql = new SqlStatement();
			StringBuilder builder = new StringBuilder();
			
			// update...
			builder.Append("UPDATE ");
			builder.Append(sql.Dialect.FormatTableName(this.EntityType.NativeNameExtended));
			builder.Append(" SET ");

			// values...
			for(int index = 0; index < fields.Length; index++)
			{
				if(index > 0)
					builder.Append(", ");

				// add...
				builder.Append(sql.Dialect.FormatColumnName(fields[index].NativeName));
				builder.Append("=");

				// value...
				object value = values[index];
				if(value == null)
					value = DBNull.Value;

				// add...
                builder.Append(sql.Dialect.FormatVariableNameForQueryText(sql.Parameters.Add(fields[index].DBType, value)));
			}

			// where...
			this.AppendWhereClause(sql, builder, keyFields, keyValues);

			// set...
			sql.CommandText = builder.ToString();

			// run...
			return new SqlStatement[] { sql };
		}
	}
}
