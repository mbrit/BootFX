// BootFX - Application framework for .NET applications
// 
// File: ColumnPerFieldDeleteWorkUnit.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Text;
using BootFX.Common;
using BootFX.Common.Entities;

namespace BootFX.Common.Data
{
	/// <summary>
	/// Summary description for CustomDeleteWorkUnit.
	/// </summary>
	internal class ColumnPerFieldDeleteWorkUnit : ColumnPerFieldWorkUnit
	{
		internal ColumnPerFieldDeleteWorkUnit(EntityType et, object entity)
			: base(et, entity, new EntityField[] {}, new object[] {})
		{
		}

		public override SqlStatement[] GetStatements(WorkUnitProcessingContext context)
		{
			if(context == null)
				throw new ArgumentNullException("context");
			
			// get...
			EntityField[] keyFields = this.EntityType.GetKeyFields();
			this.AssertKeyFields(keyFields);

			// key values...
			object[] keyValues = this.EntityType.Storage.GetKeyValues(this.Entity);
			if(keyValues == null)
				throw new InvalidOperationException("keyValues is null.");
			if(keyFields.Length != keyValues.Length)
				throw new InvalidOperationException(string.Format("Length mismatch for 'keyFields' and 'keyValues': {0} cf {1}.", keyFields.Length, keyValues.Length));

			// delete...
			SqlStatement sql = new SqlStatement();
			StringBuilder builder = new StringBuilder();

			// create...
			builder.Append("DELETE FROM ");
			builder.Append(sql.Dialect.FormatTableName(this.EntityType.NativeNameExtended));
			this.AppendWhereClause(sql, builder, keyFields, keyValues);

			// set...
			sql.CommandText = builder.ToString();

			// return...
			return new SqlStatement[] { sql };
		}

		public override WorkUnitType WorkUnitType
		{
			get
			{
				return WorkUnitType.Delete;
			}
		}

	}
}
