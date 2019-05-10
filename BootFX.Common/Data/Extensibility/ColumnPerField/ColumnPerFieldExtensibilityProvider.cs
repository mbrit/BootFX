// BootFX - Application framework for .NET applications
// 
// File: ColumnPerFieldExtensibilityProvider.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Text;
using System.Collections;
using BootFX.Common.Data.Schema;
using BootFX.Common.Data.Formatters;
using BootFX.Common.Entities;

namespace BootFX.Common.Data
{
	/// <summary>
	/// Summary description for CustomExtensibilityProvider.
	/// </summary>
	public class ColumnPerFieldExtensibilityProvider : DatabaseExtensibilityProvider
	{
		private const string IdColumnSuffix = "Ex";

		/// <summary>
		/// Constructor.
		/// </summary>
		public ColumnPerFieldExtensibilityProvider()
		{
		}

		public override void AddExtendedPropertyToSelectStatement(SqlStatementCreator creator, SqlStatement statement, 
			System.Text.StringBuilder builder, BootFX.Common.Entities.EntityField field)
		{
			// add it...
			builder.Append(statement.Dialect.FormatTableName(field.EntityType.NativeNameExtended));
			builder.Append(".");
			builder.Append(statement.Dialect.FormatColumnName(field.Name));
		}

		public override void EnsureExtendedTableUpToDate(BootFX.Common.Entities.EntityType entityType)
		{
			// create a new database update args...
			DatabaseUpdateArgs args = new DatabaseUpdateArgs();
			args.LimitEntityTypes.Add(entityType);

			// update...  this will magically create or update the associated table.
			DatabaseUpdate.Current.Update(null, args);
		}

		public override IWorkUnit[] GetInsertWorkUnits(object entity, BootFX.Common.Entities.EntityType et, BootFX.Common.Entities.EntityField[] modifiedFields)
		{
			if(entity == null)
				throw new ArgumentNullException("entity");			
			if(et == null)
				throw new ArgumentNullException("et");
			
			// get...
			object[] values = et.Storage.GetValues(entity, modifiedFields);
			return new WorkUnit[] { new ColumnPerFieldInsertWorkUnit(et, entity, modifiedFields, values) };
		}

		public override IWorkUnit[] GetUpdateWorkUnits(object entity, BootFX.Common.Entities.EntityType et, BootFX.Common.Entities.EntityField[] modifiedFields)
		{
			if(entity == null)
				throw new ArgumentNullException("entity");			
			if(et == null)
				throw new ArgumentNullException("et");
			
			// get...
			object[] values = et.Storage.GetValues(entity, modifiedFields);
			return new WorkUnit[] { new ColumnPerFieldUpdateWorkUnit(et, entity, modifiedFields, values) };
		}

		public override IWorkUnit[] GetDeleteWorkUnits(object entity, BootFX.Common.Entities.EntityType et, BootFX.Common.Entities.EntityField[] modifiedFields)
		{
			return new WorkUnit[] { new ColumnPerFieldDeleteWorkUnit(et, entity) };
		}

		public override bool IsPropertyInUse(BootFX.Common.Entities.EntityType entityType, string name)
		{
			return false;
		}

		public override string GetExtendedTableName(NativeName name)
		{
			if(name == null)
				throw new ArgumentNullException("name");
			
			// return...
			return name.Name + "Ex";
		}

		public override void AppendJoinsToSelectStatement(SqlStatement statement, System.Text.StringBuilder builder)
		{
			if(statement == null)
				throw new ArgumentNullException("statement");
			if(builder == null)
				throw new ArgumentNullException("builder");

			// et...
			EntityType et = statement.EntityType;
			if(et == null)
				throw new InvalidOperationException("et is null.");

			// dialect...
			SqlDialect dialect = statement.Dialect;
			if(dialect == null)
				throw new InvalidOperationException("dialect is null.");

			// names...
			string masterName = dialect.FormatTableName(et.NativeName);
			if(masterName == null)
				throw new InvalidOperationException("'masterName' is null.");
			if(masterName.Length == 0)
				throw new InvalidOperationException("'masterName' is zero-length.");
			string extensionName = dialect.FormatTableName(et.NativeNameExtended);
			if(extensionName == null)
				throw new InvalidOperationException("'extensionName' is null.");
			if(extensionName.Length == 0)
				throw new InvalidOperationException("'extensionName' is zero-length.");

			// dialect...
			builder.Append(" LEFT OUTER JOIN ");
			builder.Append(extensionName);
			builder.Append(" ON ");

			// keys...
			EntityField[] keyFields = et.GetKeyFields();
			if(keyFields == null)
				throw new InvalidOperationException("'keyFields' is null.");
			if(keyFields.Length == 0)
				throw new InvalidOperationException("'keyFields' is zero-length.");

			for(int index = 0; index < keyFields.Length; index++)
			{
				if(index > 0)
					builder.Append(", ");

				// add...
				builder.Append(dialect.FormatColumnName(keyFields[index].Name));
				builder.Append("=");
				builder.Append(dialect.FormatColumnName(MangleIdColumnName(keyFields[index].NativeName)));
			}
		}

		/// <summary>
		/// Mangles the ID column name for use in SQL.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		internal static string MangleIdColumnName(NativeName name)
		{
			if(name == null)
				throw new ArgumentNullException("name");
			
			// return...
			return MangleIdColumnName(name.Name);
		}

		/// <summary>
		/// Mangles the ID column name for use in SQL.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		internal static string MangleIdColumnName(string name)
		{
			if(name == null)
				throw new ArgumentNullException("name");
			if(name.Length == 0)
				throw new ArgumentOutOfRangeException("'name' is zero-length.");

			// mbr - 25-09-2007 - the rule here is that it has to be different to the name in the master
			// table otherwise we can't handle the ambiguity.
			return name + IdColumnSuffix;
		}

		public override void AddSchemaTables(EntityType et, Type type, BootFX.Common.Data.Schema.SqlTable coreTable, 
			BootFX.Common.Data.Schema.SqlSchema entitySchema)
		{
			// base...
			base.AddSchemaTables (et, type, coreTable, entitySchema);

			// have we done anything?
			if(!(et.HasExtendedProperties))
				return;

			// create a table...
			SqlTable table = new SqlTable(et, et.NativeNameExtended.Name);

			// key...
			EntityField[] keyFields = et.GetKeyFields();
			if(keyFields == null)
				throw new InvalidOperationException("keyFields is null.");
			ArrayList extendedKeyColumns = new ArrayList();
			foreach(EntityField keyField in keyFields)
			{
				SqlColumn column = new SqlColumn(MangleIdColumnName(keyField.NativeName), 
					keyField.DBType, keyField.Size, EntityFieldFlags.Key);

				// add...
				table.Columns.Add(column);
				extendedKeyColumns.Add(column);
			}

			// columns...
			foreach(EntityField field in et.GetExtendedProperties())
				table.Columns.Add(new SqlColumn(field));

			// relationship to parent...
			SqlChildToParentLink link = new SqlChildToParentLink(string.Format("FK_{0}_{1}", et.NativeName.Name, 
				et.NativeNameExtended.Name), coreTable);
			link.LinkFields.AddRange((SqlColumn[])extendedKeyColumns.ToArray(typeof(SqlColumn)));
			link.Columns.AddRange(coreTable.GetKeyColumns());
			table.LinksToParents.Add(link);

			// add...
			entitySchema.Tables.Add(table);
		}
	}
}
