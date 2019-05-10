// BootFX - Application framework for .NET applications
// 
// File: FlatTableWorkUnit.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Collections;
using System.Data;
using System.Runtime.Serialization;
using System.Text;
using BootFX.Common.Entities;

namespace BootFX.Common.Data
{
	/// <summary>
	/// Summary description for SqlExtendedPropertyWorkUnit.
	/// </summary>
	// mbr - 29-11-2007 - for c7 - added IExtendedDataWorkUnit...
//	internal abstract class FlatTableWorkUnit : WorkUnit
	internal abstract class FlatTableWorkUnit : WorkUnit, IExtendedDataWorkUnit
	{
		/// <summary>
		/// Field used to store whether the extended property table exists
		/// </summary>
//		private static Hashtable _extendedTableExists = new Hashtable();
		private static Lookup _extendedTableExists = null;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="entityType"></param>
		protected FlatTableWorkUnit(EntityType entityType, object entity) 
			: base(entityType, entity)
		{
		}

		protected FlatTableWorkUnit(EntityType entityType, object entity, EntityField[] fields, object[] values) 
			: base(entityType, entity, fields, values)
		{
		}

		static FlatTableWorkUnit()
		{
			// reset...
			ResetLookup();

			// sub...
			Database.DefaultDatabaseChanged += new EventHandler(Database_DefaultDatabaseChanged);
		}

		private static void ResetLookup()
		{
			// create...
			_extendedTableExists = new Lookup();
			_extendedTableExists.CreateItemValue += new CreateLookupItemEventHandler(_extendedTableExists_CreateItemValue);
		}

		/// <summary>
		/// Add a named parameter for the extended property name
		/// </summary>
		/// <param name="field"></param>
		/// <param name="statement"></param>
		/// <returns></returns>
		private string AddPropertyNameParameter(EntityField field,SqlStatement statement)
		{
			SqlStatementParameter propertyNameField = CreateParameterForField(field);
			if(statement.Parameters.Contains(propertyNameField.Name))
				return propertyNameField.Name;

			// Add the property name parameter so we can make sure we are accessing the correct property name.
			statement.Parameters.Add(propertyNameField);

			return propertyNameField.Name;
		}

		/// <summary>
		/// Append the constraint for the extended table property name and key fields
		/// </summary>
		/// <param name="field"></param>
		/// <param name="builder"></param>
		/// <param name="statement"></param>
		protected void AppendPropertyNameConstraint(EntityField field, StringBuilder builder, SqlStatement statement)
		{
			string parameterName = FlatTableExtensibilityProvider.AddNameParameter(field,statement);
			if(parameterName == null)
				throw new InvalidOperationException("'parameterName' is null.");
			if(parameterName.Length == 0)
				throw new InvalidOperationException("'parameterName' is zero-length.");

			// param...
			builder.Append(" ");
			builder.Append(statement.Dialect.AndKeyword);
			builder.Append(" ");
			builder.Append(statement.Dialect.FormatNativeName(new NativeName("Name")));
			builder.Append("=");
			builder.Append(statement.Dialect.FormatVariableNameForQueryText(parameterName));
		}

		/// <summary>
		/// Append the update extended table
		/// </summary>
		/// <param name="extendedField"></param>
		/// <param name="builder"></param>
		/// <param name="statement"></param>
		protected void AppendUpdateExtendedTable(EntityField extendedField,StringBuilder builder,SqlStatement statement)
		{
			string propertyName = AddPropertyNameParameter(extendedField,statement);

			// Append the inssert statements
			builder.Append(statement.Dialect.UpdateKeyword);
			builder.Append(" ");
			builder.Append(statement.Dialect.FormatNativeName(this.EntityType.NativeNameExtended));
			builder.Append(" ");
			builder.Append(statement.Dialect.SetKeyword);
			builder.Append(" ");
			builder.Append(statement.Dialect.FormatNativeName(FlatTableExtensibilityProvider.GetColumnNameForDbType(extendedField.DBType)));
			builder.Append("=");
			builder.Append(statement.Dialect.FormatVariableNameForQueryText(propertyName));
			builder.Append(" ");
			builder.Append(statement.Dialect.WhereKeyword);
			builder.Append(" ");

			AppendKeyFieldConstraints(builder,statement);
			AppendPropertyNameConstraint(extendedField,builder,statement);
		}

		/// <summary>
		/// Append insert into extended table
		/// </summary>
		/// <param name="context"></param>
		/// <param name="extendedField"></param>
		/// <param name="builder"></param>
		/// <param name="statement"></param>
		protected void AppendInsertIntoExtendedTable(WorkUnitProcessingContext context, EntityField extendedField,StringBuilder builder,SqlStatement statement)
		{
			if(context == null)
				throw new ArgumentNullException("context");

			EntityField[] keyFields = GetKeyFields();

			string propertyName = AddPropertyNameParameter(extendedField,statement);
			string propertyNameParameter = FlatTableExtensibilityProvider.AddNameParameter(extendedField,statement);

			// Append the inssert statements
			builder.Append(statement.Dialect.InsertIntoKeyword);
			builder.Append(" ");
			// mbr - 08-12-2005 - changed...
//			builder.Append(statement.Dialect.FormatNativeName(ExtendedPropertySettings.GetExtendedNativeNameForEntityType(EntityType)));
			builder.Append(statement.Dialect.FormatNativeName(this.EntityType.NativeNameExtended));
			builder.Append(" (");
			builder.Append(statement.Dialect.FormatNativeName(FlatTableExtensibilityProvider.GetColumnNameForDbType(extendedField.DBType)));
			builder.Append(statement.Dialect.IdentifierSeparator);
			builder.Append(statement.Dialect.FormatNativeName(ExtendedPropertySettings.GetExtendedNativeNameForNameColumn()));

			for(int index = 0; index < keyFields.Length; index++)
			{
				builder.Append(statement.Dialect.IdentifierSeparator);
				builder.Append(statement.Dialect.FormatNativeName(keyFields[index].NativeName));
			}

			builder.Append(") ");
			builder.Append(statement.Dialect.ValuesKeyword);
			builder.Append(" (");

			// create the param...

			builder.Append(statement.Dialect.FormatVariableNameForQueryText(propertyName));
			builder.Append(statement.Dialect.IdentifierSeparator);
			builder.Append(statement.Dialect.FormatVariableNameForQueryText(propertyNameParameter));

			for(int index = 0; index < keyFields.Length; index++)
			{
				// create the param...
				SqlStatementParameter param = null;
				if(!statement.Parameters.Contains(keyFields[index].NativeName.Name))
					AddPropertyNameParameter(keyFields[index],statement);

				// Get the parameter
				param = statement.Parameters[keyFields[index].Name];
	
				// We need to get the id of the field if it is auto generated from the context
				if(context.Bag.LastCreatedId != null)
					param.Value = context.Bag.LastCreatedId;

				// add...
				builder.Append(statement.Dialect.IdentifierSeparator);
				builder.Append(statement.Dialect.FormatVariableNameForQueryText(param.Name));
			}
		}

		/// <summary>
		/// Append the constraint for the extended table property name and key fields, used for updating and deleting
		/// </summary>
		/// <param name="builder"></param>
		/// <param name="statement"></param>
		protected void AppendKeyFieldConstraints(StringBuilder builder,SqlStatement statement)
		{
			EntityField[] keyFields = GetKeyFields();
			if(keyFields.Length == 0)
				throw new InvalidOperationException("Key fields are zero-length.");
	
			// Walk each key field
			for(int index = 0; index < keyFields.Length; index++)
			{
				// param...
				SqlStatementParameter parameter = this.CreateParameterForField(keyFields[index]);
				statement.Parameters.Add(parameter);
	
				// add...
				if(index > 0)
				{
					builder.Append(" ");
					builder.Append(statement.Dialect.AndKeyword);
					builder.Append(" ");
				}
				builder.Append(statement.Dialect.FormatNativeName(keyFields[index].NativeName));
				builder.Append("=");
                builder.Append(statement.Dialect.FormatVariableNameForQueryText(parameter.Name));
			}
		}

		/// <summary>
		/// Returns a boolean true if the extended table for the EntityType exists
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		// mbr - 25-09-2007 - should refactor to a lookup...		
		internal static bool DoesExtendedTableExist(EntityType type)
		{
			if(type == null)
				throw new ArgumentNullException("type");

			// mbr - 25-09-2007 - changed to lookup.
//			// already?
//			if(_extendedTableExists.ContainsKey(type))
//				return (bool) _extendedTableExists[type];
//
//			// get the connection...
//			using(IConnection connection = Database.CreateConnection(type.DatabaseName))
//			{
//				// check...
//				// mbr - 08-12-2005 - changed...
//				bool tableExists = connection.DoesTableExist(type.NativeNameExtended);
//
//				// add...
//				_extendedTableExists[type] = tableExists;
//
//				// return...
//				return tableExists;
//			}

			// return...
			return (bool)_extendedTableExists[type];
		}

		private static void _extendedTableExists_CreateItemValue(object sender, CreateLookupItemEventArgs e)
		{
			EntityType type = (EntityType)e.Key;

			// get the connection...
			using(IConnection connection = Database.CreateConnection(type.DatabaseName))
				e.NewValue = connection.DoesTableExist(type.NativeNameExtended);
		}

		private static void Database_DefaultDatabaseChanged(object sender, EventArgs e)
		{
			ResetLookup();
		}
		}
}
