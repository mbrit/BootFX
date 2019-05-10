// BootFX - Application framework for .NET applications
// 
// File: FlatTableInsertWorkUnit.cs
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
using System.Runtime.Serialization;
using BootFX.Common.Entities;

namespace BootFX.Common.Data
{
	/// <summary>
	/// Defines a work unit that performs a SQL <c>DELETE</c> statement.
	/// </summary>
	// mbr - 25-09-2007 - renamed.	
//	public class SqlExtendedPropertyInsertWorkUnit : SqlExtendedPropertyWorkUnit
	internal class FlatTableInsertWorkUnit : FlatTableWorkUnit
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="entityType"></param>
		/// <param name="entity"></param>
		/// <param name="fields"></param>
		/// <param name="values"></param>
		internal FlatTableInsertWorkUnit(EntityType entityType, object entity, EntityField[] fields, object[] values) : base(entityType, entity, fields, values)
		{
		}

		/// <summary>
		/// Creates an Delete statement.
		/// </summary>
		/// <returns></returns>
		public override SqlStatement[] GetStatements(WorkUnitProcessingContext context)
		{
			if(EntityType == null)
				throw new ArgumentNullException("EntityType");
			if(Dialect == null)
				throw new InvalidOperationException("Dialect is null.");

			// create...
			SqlStatement statement = new SqlStatement(this.EntityType, this.Dialect);

			// sql...
			StringBuilder builder = new StringBuilder();
			EntityField field = null;
			field = GetExtendedFields()[0]; // This is the field being updated

			AppendInsertIntoExtendedTable(context, field, builder, statement);
			
			builder.Append(")");

			statement.CommandText = builder.ToString();
			return new SqlStatement[] { statement };
		}

		public override WorkUnitType WorkUnitType
		{
			get
			{
				return WorkUnitType.Update;
			}
		}
	}
}
