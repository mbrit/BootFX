// BootFX - Application framework for .NET applications
// 
// File: FlatTableDeleteOneWorkUnit.cs
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
//	public class SqlExtendedPropertyDeleteOneWorkUnit : SqlExtendedPropertyWorkUnit
	internal class FlatTableDeleteOneWorkUnit : FlatTableWorkUnit
	{
		/// <summary>
		/// Private field to support <see cref="FieldName"/> property.
		/// </summary>
		private string _fieldName;
		
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="entityType"></param>
		/// <param name="entity"></param>
		/// <param name="fields"></param>
		/// <param name="values"></param>
		internal FlatTableDeleteOneWorkUnit(EntityType entityType, object entity, EntityField[] fields, object[] values, string fieldName) : base(entityType, entity, fields, values)
		{
			if(fieldName == null)
				throw new ArgumentNullException("fieldName");
			if(fieldName.Length == 0)
				throw new ArgumentOutOfRangeException("'fieldName' is zero-length.");
			_fieldName = fieldName;
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
			StringBuilder builder = new StringBuilder();

			// DELETE FROM ActivitesBFX WHERE 
			builder.Append(statement.Dialect.DeleteFromKeyword); 
			builder.Append(" ");
			// mbr - 08-12-2005 - changed...
			//			builder.Append(statement.Dialect.FormatNativeName(ExtendedPropertySettings.GetExtendedNativeNameForEntityType(EntityType)));
			builder.Append(statement.Dialect.FormatNativeName(this.EntityType.NativeNameExtended));
			builder.Append(" ");
			builder.Append(statement.Dialect.WhereKeyword);
			builder.Append(" ");	
			AppendKeyFieldConstraints(builder,statement);

			// field name...
			if(FieldName == null)
				throw new InvalidOperationException("'FieldName' is null.");
			if(FieldName.Length == 0)
				throw new InvalidOperationException("'FieldName' is zero-length.");
			builder.Append(" AND ");
			builder.Append(statement.Dialect.FormatColumnName("Name"));
			builder.Append("=");
			builder.Append(statement.Dialect.FormatVariableNameForQueryText(statement.Parameters.Add(DbType.String, this.FieldName)));

			// set..
			statement.CommandText = builder.ToString();
			return new SqlStatement[] { statement };
	}

	public override WorkUnitType WorkUnitType
		{
			get
			{
				return WorkUnitType.Delete;
			}
		}

		/// <summary>
		/// Gets the fieldname.
		/// </summary>
		private string FieldName
		{
			get
			{
				return _fieldName;
			}
		}
	}
}
