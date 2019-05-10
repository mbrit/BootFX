// BootFX - Application framework for .NET applications
// 
// File: ColumnPerFieldInsertWorkUnit.cs
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
using BootFX.Common.Entities;

namespace BootFX.Common.Data
{
	/// <summary>
	/// Summary description for CustomInsertWorkUnit.
	/// </summary>
	internal class ColumnPerFieldInsertWorkUnit : ColumnPerFieldWorkUnit
	{
		internal ColumnPerFieldInsertWorkUnit(EntityType entityType, object entity, EntityField[] fields, object[] values)
			: base(entityType, entity, fields, values)
		{
		}

		public override SqlStatement[] GetStatements(WorkUnitProcessingContext context)
		{
			return this.GetInsertStatements(context);
		}

		/// <summary>
		/// Gets the statements to do an insert.
		/// </summary>
		/// <returns></returns>
		protected SqlStatement[] GetInsertStatements(WorkUnitProcessingContext context)
		{
			if(context == null)
				throw new ArgumentNullException("context");
			
			// check...
			if(EntityType == null)
				throw new InvalidOperationException("EntityType is null.");

			// keys...
			EntityField[] keyFields = this.EntityType.GetKeyFields();
			AssertKeyFields(keyFields);

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

			// fields...
			ArrayList allFields = new ArrayList();
			allFields.AddRange(keyFields);
			allFields.AddRange(fields);

			// values...
			ArrayList allValues = new ArrayList();
			allValues.AddRange(keyValues);
			allValues.AddRange(values);

			// setup...
			SqlStatement sql = new SqlStatement();
			StringBuilder builder = new StringBuilder();
			
			// get...
			builder.Append(sql.Dialect.InsertIntoKeyword);
			builder.Append(" ");
			builder.Append(sql.Dialect.FormatTableName(this.EntityType.NativeNameExtended));
			builder.Append(" (");
			for(int index = 0; index < allFields.Count; index++)
			{
				if(index > 0)
					builder.Append(", ");

				// field...
				EntityField field = (EntityField)allFields[index];

				// key?
				if(field.IsKey())
					builder.Append(sql.Dialect.FormatColumnName(ColumnPerFieldExtensibilityProvider.MangleIdColumnName(field.NativeName)));
				else
					builder.Append(sql.Dialect.FormatColumnName(field.NativeName));
			}
			builder.Append(") ");
			builder.Append(sql.Dialect.ValuesKeyword);
			builder.Append(" (");
			for(int index = 0; index < allFields.Count; index++)
			{
				if(index > 0)
					builder.Append(", ");

				// value...
				object value = allValues[index];

				// mbr - 10-10-2007 - for c7 - before we wouldn't have had a reconciled value here, but now that the 
				// reconciliation step has moved, we do.
				EntityField field = (EntityField)allFields[index];
//				if(field.IsKey())
//					value = context.Bag.LastCreatedId;
//				else if(value == null)
//					value = DBNull.Value;
				if(value == null)
					value = DBNull.Value;

				// add...
				builder.Append(sql.Dialect.FormatVariableNameForQueryText(sql.Parameters.Add(field.DBType, value)));
			}
			builder.Append(")");

			// return...
			sql.CommandText = builder.ToString();
			return new SqlStatement[] { sql };
		}

		public override WorkUnitType WorkUnitType
		{
			get
			{
				// this is always an *update* because this short circuits the reconciliation
				// routine at the end.
				return WorkUnitType.Update;
			}
		}
	}
}
