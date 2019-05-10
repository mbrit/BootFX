// BootFX - Application framework for .NET applications
// 
// File: FlatTableUpdateWorkUnit.cs
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
//	public class SqlExtendedPropertyUpdateWorkUnit : SqlExtendedPropertyWorkUnit
	internal class FlatTableUpdateWorkUnit : FlatTableWorkUnit
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="entityType"></param>
		/// <param name="entity"></param>
		/// <param name="fields"></param>
		/// <param name="values"></param>
		internal FlatTableUpdateWorkUnit(EntityType entityType, object entity, EntityField[] fields, object[] values) : base(entityType, entity, fields, values)
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
						
			// create...
			SqlStatement statement = new SqlStatement(this.EntityType, this.Dialect);

			// sql...
			StringBuilder builder = new StringBuilder();
			EntityField field = GetExtendedFields()[0]; // This is the field being updated

			// If we are doing an insert or update we must check to see if the field already exists
			builder.Append("IF ((");
			builder.Append(statement.Dialect.SelectKeyword);
			builder.Append(" COUNT(*) ");
			builder.Append(statement.Dialect.FromKeyword);
			builder.Append(" ");
			// mbr - 08-12-2005 - changed...
//			builder.Append(statement.Dialect.FormatNativeName(ExtendedPropertySettings.GetExtendedNativeNameForEntityType(EntityType)));
			builder.Append(statement.Dialect.FormatNativeName(this.EntityType.NativeNameExtended));
			builder.Append(" ");
			builder.Append(statement.Dialect.WhereKeyword);
			builder.Append(" ");

			AppendKeyFieldConstraints(builder,statement);
			AppendPropertyNameConstraint(field,builder,statement);

			builder.Append(" ) = 0)\r\n");

			base.AppendInsertIntoExtendedTable(context, field,builder,statement);

			builder.Append(")\r\n");
			builder.Append("ELSE\r\n");

			AppendUpdateExtendedTable(field,builder,statement);

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
