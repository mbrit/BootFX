// BootFX - Application framework for .NET applications
// 
// File: FlatTableExtensibilityProvider.cs
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
using BootFX.Common;
using BootFX.Common.Data.Schema;
using BootFX.Common.Data.Formatters;
using BootFX.Common.Entities;

namespace BootFX.Common.Data
{
	/// <summary>
	/// Defines the class for doing the basic "Bfx" database provider.
	/// </summary>
	internal class FlatTableExtensibilityProvider : DatabaseExtensibilityProvider
	{
		internal const string NameColumnName = "Name";
		internal const string StringColumnName = "String";
		internal const string DateTimeColumnName = "DateTime";
		internal const string BinaryColumnName = "Binary";
		internal const string DecimalColumnName = "Decimal";
		internal const string IntegerColumnName = "Int64";

		/// <summary>
		/// Defines the XML serialization version.
		/// </summary>
		internal const int XmlSchemaVersion = 3;

		/// <summary>
		/// Defines the maximum length of a string property.
		/// </summary>
		internal const int MaxStringPropertyLength = 2048;
		
		/// <summary>
		/// Constructor.
		/// </summary>
		internal FlatTableExtensibilityProvider()
		{
		}

		/// <summary>
		/// Ensures that the extended properties table has been created.
		/// </summary>
		/// <param name="entityType"></param>
		public override void EnsureExtendedTableUpToDate(EntityType entityType)
		{
			if(entityType == null)
				throw new ArgumentNullException("entityType");
			
			// check...
			if(!(IsExtendedPropertiesTableCreated(entityType)))
				this.CreateExtendedPropertiesTable(entityType);
		}

		/// <summary>
		/// Returns true if the table for the extended properties exist
		/// </summary>
		/// <param name="entityType"></param>
		/// <returns></returns>
		private bool IsExtendedPropertiesTableCreated(EntityType entityType)
		{
			if(!Database.HasConnectionSettings)
				return false;

			// return...
			return Database.DoesTableExist(entityType.NativeNameExtended);
		}

		/// <summary>
		/// Gets the extended table name.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public override string GetExtendedTableName(NativeName name)
		{
			if(name == null)
				throw new ArgumentNullException("name");
			
			// return...
			return name.Name + "Bfx";
		}

		/// <summary>
		/// Creates the custom property table for a specified sql table
		/// </summary>
		/// <param name="entityType"></param>
		public void CreateExtendedPropertiesTable(EntityType entityType)
		{
			if(entityType == null)
				throw new ArgumentNullException("entityType");
			
			// get...
			SqlTable customTable = GetCustomPropertySqlTableForEntityType(entityType);
			if(customTable == null)
				throw new InvalidOperationException("customTable is null.");

			// scripts...
			string[] createTableSql = Database.DefaultDialect.GetCreateTableScript(customTable, SqlTableScriptFlags.IgnoreExistenceErrors);
			if(createTableSql == null)
				throw new InvalidOperationException("'createTableSql' is null.");
			if(createTableSql.Length == 0)
				throw new InvalidOperationException("'createTableSql' is zero-length.");

			// mbr - 08-12-2005 - use a separate conn...
			IConnection connection = Connection.CreateConnection(entityType);
			if(connection == null)
				throw new InvalidOperationException("connection is null.");
			try
			{
				connection.BeginTransaction();

				// walk...
				foreach(string statement in createTableSql)
					connection.ExecuteNonQuery(new SqlStatement(statement));

				// ok...
				connection.Commit();
			}
			catch(Exception ex)
			{
				try
				{
					connection.Rollback();
				}
				catch
				{
					// no-op...
				}

				// throw...
				throw new InvalidOperationException(string.Format("Failed to create table for '{0}'.", entityType.Name), ex);
			}
			finally
			{			
				if(connection != null)
					connection.Dispose();
			}
		}

		/// <summary>
		/// Returns the custom property sql table for an SqlTable
		/// </summary>
		/// <returns></returns>
		private static SqlTable GetCustomPropertySqlTableForEntityType(EntityType entityType)
		{
			// mbr - 08-12-2005 - changed...
//			string name = GetExtendedNativeNameForEntityType(entityType).Name;
			NativeName name = entityType.NativeNameExtended;
			SqlTable entityTypeTable = new SqlTable(null,entityType);
			
			SqlTable table = new SqlTable(name.Name);

			// add the standard columns...
			SqlColumn[] columns = new SqlColumn[6];
			columns[0] = new SqlColumn(NameColumnName, NameColumnName, DbType.String, MaxNativeNameLength, EntityFieldFlags.Key);
			columns[1] = new SqlColumn(IntegerColumnName, IntegerColumnName, DbType.Int64, -1, EntityFieldFlags.Nullable);
			columns[2] = new SqlColumn(DecimalColumnName, DecimalColumnName, DbType.Decimal, -1, EntityFieldFlags.Nullable);
			columns[3] = new SqlColumn(DateTimeColumnName, DateTimeColumnName, DbType.DateTime, -1, EntityFieldFlags.Nullable);
			columns[4] = new SqlColumn(StringColumnName, StringColumnName, DbType.String, MaxStringPropertyLength, EntityFieldFlags.Nullable);
			columns[5] = new SqlColumn(BinaryColumnName, BinaryColumnName, DbType.Object, -1, EntityFieldFlags.Nullable | EntityFieldFlags.Large);
			
			// walk...
			foreach(SqlColumn column in entityTypeTable.GetKeyColumns())
			{
				SqlColumn keyColumn = (SqlColumn) column.Clone();
				if(keyColumn.IsAutoIncrement)
					keyColumn.Flags ^= EntityFieldFlags.AutoIncrement;

				// add...
				table.Columns.Add(keyColumn);
			}

			// add...
			table.Columns.AddRange(columns);
			return table;
		}

		/// <summary>
		/// Gets the extended property column name used for storing and retrieving a value
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		internal static NativeName GetColumnNameForDbType(DbType type)
		{
			string name;
			switch(type)
			{
				case DbType.Boolean:
				case DbType.Byte:
				case DbType.SByte:
				case DbType.Int16:
				case DbType.Int32:
				case DbType.Int64:
				case DbType.UInt16:
				case DbType.UInt32:
				case DbType.UInt64:
					name = IntegerColumnName;
					break;

				case DbType.Single:
				case DbType.Decimal:
				case DbType.Double:
					name = DecimalColumnName;
					break;

				case DbType.DateTime:
					name = DateTimeColumnName;
					break;

				case DbType.Object:
					name = BinaryColumnName;
					break;

				case DbType.AnsiString:
				case DbType.AnsiStringFixedLength:
				case DbType.String:
				case DbType.StringFixedLength:
					name = StringColumnName;
					break;

				default:
					throw ExceptionHelper.CreateCannotHandleException(type);
			}

			return new NativeName(name);
		}

		/// <summary>
		/// Append SQL to select the extended properties from the extended table for the entity
		/// </summary>
		/// <param name="statement"></param>
		/// <param name="builder"></param>
		/// <param name="field"></param>
		public override void AddExtendedPropertyToSelectStatement(SqlStatementCreator creator, SqlStatement statement, StringBuilder builder, 
			EntityField field)
		{
			if(creator == null)
				throw new ArgumentNullException("creator");
			if(statement == null)
				throw new ArgumentNullException("statement");
			if(builder == null)
				throw new ArgumentNullException("builder");
			if(field == null)
				throw new ArgumentNullException("field");
			
			// check...
			if(creator.EntityType == null)
				throw new InvalidOperationException("creator.EntityType is null.");

			// add...
			builder.Append("(");
			builder.Append(statement.Dialect.SelectKeyword);
			builder.Append(" ");
			builder.Append(statement.Dialect.FormatNativeName(GetColumnNameForDbType(field.DBType)));
			builder.Append(" ");
			builder.Append(statement.Dialect.FromKeyword);
			builder.Append(" ");
			builder.Append(statement.Dialect.FormatNativeName(creator.EntityType.NativeNameExtended));
			builder.Append(" ");
			builder.Append(statement.Dialect.WhereKeyword);
			builder.Append(" ");

			// where...
			creator.AppendConstraints(statement, builder);

			// create the param...
			SqlStatementParameter parameter = new SqlStatementParameter(string.Format("Extended{0}", field.NativeName), DbType.String,
				field.NativeName.Name);

			// param...
			builder.Append(" ");
			builder.Append(statement.Dialect.AndKeyword);
			builder.Append(" ");
			builder.Append(statement.Dialect.FormatNativeName(new NativeName("Name")));
			builder.Append("=");
			builder.Append(statement.Dialect.FormatVariableNameForQueryText(parameter.Name));

			// details...
			builder.Append(") AS ");
			builder.Append(statement.Dialect.FormatNativeName(field.NativeName));

			// add the parameter...
			statement.Parameters.Add(parameter);
		}

		/// <summary>
		/// Returns true if the property is being used.
		/// </summary>
		/// <param name="entityType"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		public override bool IsPropertyInUse(EntityType entityType, string name)
		{
			if(entityType == null)
				throw new ArgumentNullException("entityType");
			if(name == null)
				throw new ArgumentNullException("name");
			if(name.Length == 0)
				throw new ArgumentOutOfRangeException("'name' is zero-length.");
			 
			// table?
			if(IsExtendedPropertiesTableCreated(entityType))
			{
				// dialect...
				SqlDialect dialect = entityType.Dialect;
				if(dialect == null)
					throw new InvalidOperationException("dialect is null.");

				// find it...
				StringBuilder builder = new StringBuilder();
				builder.Append(dialect.SelectKeyword);
				builder.Append(" TOP 1 ");
				builder.Append(dialect.FormatColumnName("Name"));
				builder.Append(" ");
				builder.Append(dialect.FromKeyword);
				builder.Append(" ");
				builder.Append(dialect.FormatNativeName(entityType.NativeNameExtended));
				builder.Append(" ");
				builder.Append(dialect.WhereKeyword);
				builder.Append(" ");
				builder.Append(dialect.FormatColumnName("Name"));
				builder.Append("=");
				const string paramName = "name";
				builder.Append(dialect.FormatVariableNameForQueryText(paramName));

				// create...
				SqlStatement statement = new SqlStatement(builder.ToString());
				statement.Parameters.Add(new SqlStatementParameter(paramName, DbType.String, name));

				// run...
				object result = Database.ExecuteScalar(statement);
				if(result is string)
					return true;
				else
					return false;
			}
			else
				return false;
		}

		/// <summary>
		/// Gets the insert work units.
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="et"></param>
		/// <param name="modifiedFields"></param>
		/// <returns></returns>
		public override IWorkUnit[] GetInsertWorkUnits(object entity, EntityType et, EntityField[] modifiedFields)
		{
			// results...
			WorkUnitCollection units = new WorkUnitCollection();

			// check...
			if(FlatTableWorkUnit.DoesExtendedTableExist(et))
			{
				// keys...
				EntityField[] keyFields = et.GetKeyFields();
				if(keyFields == null)
					throw new InvalidOperationException("keyFields is null.");

				// create...
				foreach(EntityField field in modifiedFields)
				{
					ArrayList unitFields = new ArrayList(keyFields);
					unitFields.Add(field);

					EntityField[] workUnitFields = (EntityField[]) unitFields.ToArray(typeof(EntityField));
					object[] values = et.Storage.GetValues(entity, workUnitFields);

					// create...
					units.Add(new FlatTableInsertWorkUnit(et, entity,workUnitFields, values));
				}
			}

			// return...
			return units.ToArray();
		}

		/// <summary>
		/// Gets the work units to use with an update.
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="et"></param>
		/// <param name="modifiedFields"></param>
		/// <returns></returns>
		public override IWorkUnit[] GetUpdateWorkUnits(object entity, EntityType et, EntityField[] modifiedFields)
		{
			if(entity == null)
				throw new ArgumentNullException("entity");
			if(et == null)
				throw new ArgumentNullException("et");
			if(modifiedFields == null)
				throw new ArgumentNullException("modifiedFields");

			// key fields...
			EntityField[] keyFields = et.GetKeyFields();
			if(keyFields == null)
				throw new InvalidOperationException("'keyFields' is null.");
			if(keyFields.Length == 0)
				throw new InvalidOperationException("'keyFields' is zero-length.");

			// get...
			WorkUnitCollection units = new WorkUnitCollection();
			if(FlatTableWorkUnit.DoesExtendedTableExist(et))
			{
				// create...
				foreach(EntityField field in modifiedFields)
				{
					// mbr - 17-03-2006 - check that we don't actually have null...  if we do, we actually need to delete the row...
					if(et.Storage.IsDBNull(entity, field))
					{
						// add...
						object[] keyValues = et.Storage.GetValues(entity, keyFields);
						units.Add(new FlatTableDeleteOneWorkUnit(et, entity, keyFields, keyValues, field.NativeName.Name));;
					}
					else
					{
						ArrayList unitFields = new ArrayList(keyFields);
						unitFields.Add(field);

						EntityField[] workUnitFields = (EntityField[]) unitFields.ToArray(typeof(EntityField));
						object[] values = et.Storage.GetValues(entity, workUnitFields);

						// create...
						units.Add(new FlatTableUpdateWorkUnit(et, entity, workUnitFields, values));
					}
				}
			}
	
			// return...
			return units.ToArray();
		}

		/// <summary>
		/// Gets the delete work units.
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="et"></param>
		/// <param name="modifiedFields"></param>
		/// <returns></returns>
		public override IWorkUnit[] GetDeleteWorkUnits(object entity, EntityType et, EntityField[] modifiedFields)
		{
			if(entity == null)
				throw new ArgumentNullException("entity");
			if(et == null)
				throw new ArgumentNullException("et");
			if(modifiedFields == null)
				throw new ArgumentNullException("modifiedFields");

			// create...
			WorkUnitCollection units = new WorkUnitCollection();
			if(FlatTableWorkUnit.DoesExtendedTableExist(et))
			{
				// Create a delete work unit for extended properties, we may not have any field changes
				// get all of the fields...
				EntityField[] keyFields = et.GetKeyFields();
				object[] values = et.Storage.GetValues(entity, keyFields);

				// add...
				units.Add(new FlatTableDeleteAllWorkUnit(et, entity, keyFields, values));
			}

			// return...
			return units.ToArray();
		}

		public override void AppendJoinsToSelectStatement(SqlStatement statement, StringBuilder builder)
		{
			// no-op...
		}

		public override void AppendColumnNameForEntityFieldFilterConstraint(FilterConstraintAppendContext context, 
			EntityFieldFilterConstraint constraint, SqlStatement statement, StringBuilder builder, EntityField field)
		{
			if(context == null)
				throw new ArgumentNullException("context");
			if(constraint == null)
				throw new ArgumentNullException("constraint");			
			if(statement == null)
				throw new ArgumentNullException("statement");
			if(field == null)
				throw new ArgumentNullException("field");

			// Add the name paramter for the extended field
			string nameField = AddNameParameter(field, statement);
			if(nameField == null)
				throw new InvalidOperationException("'nameField' is null.");
			if(nameField.Length == 0)
				throw new InvalidOperationException("'nameField' is zero-length.");

			// This could cause a problem, it assumes the first key field is the primary key and that there is only one
			builder.Append(statement.Dialect.FormatNativeName(context.Creator.EntityType.GetKeyFields()[0].NativeName));
			builder.Append(" IN (");
			builder.Append(statement.Dialect.SelectKeyword);
			builder.Append(" ");
			builder.Append(statement.Dialect.FormatNativeName(context.Creator.EntityType.GetKeyFields()[0].NativeName));
			builder.Append(" ");
			builder.Append(statement.Dialect.FromKeyword);
			builder.Append(" ");
			builder.Append(statement.Dialect.FormatNativeName(context.Creator.EntityType.NativeNameExtended));
			builder.Append(" ");
			builder.Append(statement.Dialect.WhereKeyword);
			builder.Append(" ");
			builder.Append(statement.Dialect.FormatNativeName(ExtendedPropertySettings.GetExtendedNativeNameForNameColumn()));
			builder.Append(" ");
			builder.Append(statement.Dialect.GetOperatorKeyword(SqlOperator.EqualTo, field.DBType));
			builder.Append(" ");
			builder.Append(statement.Dialect.FormatVariableNameForQueryText(nameField));
			builder.Append(" ");
			builder.Append(statement.Dialect.AndKeyword);
			builder.Append(" ");

			// do the bitwise...
			constraint.AppendBitwiseOperators(builder,statement);
			builder.Append(statement.Dialect.FormatNativeName(GetColumnNameForDbType(field.DBType)));
		}
		
		/// <summary>
		/// Add a named parameter for the extended fields
		/// </summary>
		/// <param name="field"></param>
		/// <param name="statement"></param>
		/// <returns></returns>
		internal static string AddNameParameter(EntityField field, SqlStatement statement)
		{
			string parameterName = string.Format("Extended{0}", field.NativeName);
			if(statement.Parameters.Contains(parameterName))
				return parameterName;

			// Add the property name parameter so we can make sure we are accessing the correct property name.
			SqlStatementParameter propertyNameParameter = new SqlStatementParameter(parameterName, DbType.String, field.NativeName.Name);
			statement.Parameters.Add(propertyNameParameter);

			return parameterName;
		}
	}
}
