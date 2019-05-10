// BootFX - Application framework for .NET applications
// 
// File: ExtendedPropertySettings.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Collections;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.IO;
using System.Text;
using System.Threading;
using System.Web;
using System.Xml;
using System.ComponentModel;
using BootFX.Common.Entities;
using BootFX.Common.Xml;
using BootFX.Common.Data;
using BootFX.Common.Data.Schema;
using BootFX.Common.Data.Formatters;

namespace BootFX.Common.Entities
{
	/// <summary>
	/// Describes a project.
	/// </summary>
	public sealed class ExtendedPropertySettings : ToXmlBase
	{
		/// <summary>
		/// Private field to support <c>Version</c> property.
		/// </summary>
		private int _version;

		/// <summary>
		/// Private field to support <see cref="Properties" /> property.
		/// </summary>
		private ExtendedPropertyDefinitionCollection _properties = null;
		
		/// <summary>
		/// Private field to support <see cref="Lookups" /> property.
		/// </summary>
		private ExtendedLookupDefinitionCollection _lookups = null;

		/// <summary>
		/// Constructor.
		/// </summary>
		public ExtendedPropertySettings()
		{
			_properties = new ExtendedPropertyDefinitionCollection();
			_lookups = new ExtendedLookupDefinitionCollection();
		}

		/// <summary>
		/// Gets the properties.
		/// </summary>
		public ExtendedPropertyDefinitionCollection Properties
		{
			get
			{
				return _properties;
			}
		}

		/// <summary>
		/// Gets the lookups.
		/// </summary>
		public ExtendedLookupDefinitionCollection Lookups
		{
			get
			{
				return _lookups;
			}
		}

		protected override void WriteXml(System.Xml.XmlWriter xml, WriteXmlContext context)
		{
			if(xml == null)
				throw new ArgumentNullException("xml");
			if(context == null)
				throw new ArgumentNullException("context");

			// mbr - 25-09-2007 - provider.			
			if(Database.ExtensibilityProvider == null)
				throw new InvalidOperationException("Database.ExtensibilityProvider is null.");

			// mbr - 25-09-2007 - this isn't lovely, but the default provider uses a defunct serialisation method.
			// it's easier to keep the old method (because it works, it's just not best practice), and if we
			// have a new provider, get the provider to do it.

			// add the schema version...
			xml.WriteAttributeString("providerType", string.Format("{0}, {1}", Database.ExtensibilityProvider.GetType().FullName,
				Database.ExtensibilityProvider.GetType().Assembly.GetName().Name));
			xml.WriteAttributeString("schemaVersion", FlatTableExtensibilityProvider.XmlSchemaVersion.ToString());

			xml.WriteStartElement("Lookups");
			foreach(ExtendedLookupDefinition lookup in Lookups.GetLookups())
				lookup.ToXml(xml);
			xml.WriteEndElement();

			// create...
			xml.WriteStartElement("ExtendedProperties");
			foreach(ExtendedPropertyDefinition property in Properties)
				property.ToXml(xml);
			xml.WriteEndElement();
		}

//		/// <summary>
//		/// Loads the extended properties from the given path.
//		/// </summary>
//		/// <param name="settingsXml"></param>
//		/// <returns></returns>
//		public static ExtendedPropertySettings LoadXml(string settingsXml)
//		{
//			if(settingsXml == null || settingsXml == string.Empty)
//				throw new ArgumentNullException("settingsXml");
//
//			XmlDocument document = new XmlDocument();
//			document.LoadXml(settingsXml);
//			// select the project node...
//			XmlElement extendedPropertiesElement = (XmlElement)document.SelectSingleNode("ExtendedPropertySettings");
//			if(extendedPropertiesElement == null)
//				throw new InvalidOperationException("ExtendedPropertySettings element not found.");
//
//			// create...
//			ExtendedPropertySettings newProject =FromXml(extendedPropertiesElement);
//
//			// return...
//			return newProject;
//		}

		/// <summary>
		/// Get the valid types for extended fields
		/// </summary>
		/// <returns></returns>
		public ExtendedPropertyDataType[] GetExtendedPropertyDataTypes()
		{
			// basic CLR types.
			ArrayList typed = new ArrayList();
			typed.Add(new ClrExtendedPropertyDataType(typeof(Boolean)));
			typed.Add(new ClrExtendedPropertyDataType(typeof(DateTime)));
			typed.Add(new ClrExtendedPropertyDataType(typeof(Decimal)));
			typed.Add(new ClrExtendedPropertyDataType(typeof(Int64)));
			typed.Add(new ClrExtendedPropertyDataType(typeof(String)));

			// lookups...
			foreach(ExtendedLookupDefinition lookup in Lookups.GetLookups())
				typed.Add(new LookupExtendedPropertyDataType(lookup));

			// return...
			return (ExtendedPropertyDataType[]) typed.ToArray(typeof(ExtendedPropertyDataType));
		}

		public static ExtendedPropertySettings FromXml(string xml)
		{
			if(xml == null)
				throw new ArgumentNullException("xml");
			if(xml.Length == 0)
				throw new ArgumentOutOfRangeException("'xml' is zero-length.");
			
			// load...
			XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);

			// find...
			XmlElement root = (XmlElement)XmlHelper.GetRootElement(doc, true);
			if(root == null)
				throw new InvalidOperationException("root is null.");

			// return...
			return FromXml(root);
		}

		/// <summary>
		/// Restores ExtendedPropertySettings from the XML file.
		/// </summary>
		/// <param name="element"></param>
		/// <returns></returns>
		public static ExtendedPropertySettings FromXml(XmlElement element)
		{
			if(element == null)
				throw new ArgumentNullException("element");

			// what's the schema version...
			int version = XmlHelper.GetAttributeInt32(element, "schemaVersion", OnNotFound.ReturnNull);
			if(version < 3)
				return new ExtendedPropertySettings();
			if(version == 3)
				return FromXmlVersion3Schema(element);
			else
				throw new NotSupportedException(string.Format("Schema version '{0}' not supported.", version));
		}

		private static ExtendedPropertySettings FromXmlVersion3Schema(XmlElement element)
		{
			if(element == null)
				throw new ArgumentNullException("element");
			
			// load...
			ExtendedPropertySettings settings = new ExtendedPropertySettings();

			// load the extended properties
			foreach(XmlElement propertyElement in element.SelectNodes("ExtendedProperties/ExtendedProperty"))
				settings.Properties.Add(ExtendedPropertyDefinition.FromXml(propertyElement));

			// load the extended lookups
			foreach(XmlElement lookupElement in element.SelectNodes("Lookups/Lookup"))
				settings.Lookups.Add(ExtendedLookupDefinition.FromXml(lookupElement));

			// return...
			return settings;
		}

		// mbr - 25-09-2007 - moved to provider.		
//		/// <summary>
//		/// Gets the extended property column name used for storing and retrieving a value
//		/// </summary>
//		/// <param name="type"></param>
//		/// <returns></returns>
//		internal static NativeName GetColumnNameForDbType(DbType type)
//		{
//			string name;
//			switch(type)
//			{
//				case DbType.Boolean:
//				case DbType.Byte:
//				case DbType.SByte:
//				case DbType.Int16:
//				case DbType.Int32:
//				case DbType.Int64:
//				case DbType.UInt16:
//				case DbType.UInt32:
//				case DbType.UInt64:
//					name = IntegerColumnName;
//					break;
//
//				case DbType.Single:
//				case DbType.Decimal:
//				case DbType.Double:
//					name = DecimalColumnName;
//					break;
//
//				case DbType.DateTime:
//					name = DateTimeColumnName;
//					break;
//
//				case DbType.Object:
//					name = BinaryColumnName;
//					break;
//
//				case DbType.AnsiString:
//				case DbType.AnsiStringFixedLength:
//				case DbType.String:
//				case DbType.StringFixedLength:
//					name = StringColumnName;
//					break;
//
//				default:
//					throw ExceptionHelper.CreateCannotHandleException(type);
//			}
//
//			return new NativeName(name);
//		}

//		/// <summary>
//		/// Gets the suffixfor all the extended tables
//		/// </summary>
//		/// <returns></returns>
//		public static string GetExtendedTableSuffix()
//		{
//			return "Bfx";
//		}
//
//		/// <summary>
//		/// Get the ExtendedProperty table NativeName for an EntityType 
//		/// </summary>
//		/// <param name="type"></param>
//		/// <returns></returns>
//		public static NativeName GetExtendedNativeNameForEntityType(EntityType type)
//		{
//			return GetExtendedNativeNameForName(type.NativeName.Name);
//		}
//
//		/// <summary>
//		/// Get the ExtendedProperty table NativeName for a table name 
//		/// </summary>
//		/// <param name="tableName"></param>
//		/// <returns></returns>
//		private static NativeName GetExtendedNativeNameForName(string tableName)
//		{
//			return new NativeName(tableName + GetExtendedTableSuffix());
//		}

		/// <summary>
		/// Gets the NativeName for the Column that stores the property name
		/// </summary>
		/// <returns></returns>
		public static NativeName GetExtendedNativeNameForNameColumn()
		{
			return new NativeName("Name");
		}

		// mbr - 25-09-2007 - moved to provider.		
//		/// <summary>
//		/// Returns the custom property sql table for an SqlTable
//		/// </summary>
//		/// <returns></returns>
//		private static SqlTable GetCustomPropertySqlTableForEntityType(EntityType entityType)
//		{
//			// mbr - 08-12-2005 - changed...
////			string name = GetExtendedNativeNameForEntityType(entityType).Name;
//			NativeName name = entityType.NativeNameExtended;
//			SqlTable entityTypeTable = new SqlTable(null,entityType);
//			
//			SqlTable table = new SqlTable(name.Name);
//
//			// add the standard columns...
//			SqlColumn[] columns = new SqlColumn[6];
//			columns[0] = new SqlColumn(NameColumnName, NameColumnName, DbType.String, MaxNativeNameLength, EntityFieldFlags.Key);
//			columns[1] = new SqlColumn(IntegerColumnName, IntegerColumnName, DbType.Int64, -1, EntityFieldFlags.Nullable);
//			columns[2] = new SqlColumn(DecimalColumnName, DecimalColumnName, DbType.Decimal, -1, EntityFieldFlags.Nullable);
//			columns[3] = new SqlColumn(DateTimeColumnName, DateTimeColumnName, DbType.DateTime, -1, EntityFieldFlags.Nullable);
//			columns[4] = new SqlColumn(StringColumnName, StringColumnName, DbType.String, MaxStringPropertyLength, EntityFieldFlags.Nullable);
//			columns[5] = new SqlColumn(BinaryColumnName, BinaryColumnName, DbType.Object, -1, EntityFieldFlags.Nullable | EntityFieldFlags.Large);
//			
//			// walk...
//			foreach(SqlColumn column in entityTypeTable.GetKeyColumns())
//			{
//				SqlColumn keyColumn = (SqlColumn) column.Clone();
//				if(keyColumn.IsAutoIncrement)
//					keyColumn.Flags ^= EntityFieldFlags.AutoIncrement;
//
//				// add...
//				table.Columns.Add(keyColumn);
//			}
//
//			// add...
//			table.Columns.AddRange(columns);
//			return table;
//		}

		// mbr - 25-09-2007 - moved to provider.		
//		/// <summary>
//		/// Creates the custom property table for a specified sql table
//		/// </summary>
//		/// <param name="entityType"></param>
//		private void CreateExtendedPropertiesTable(EntityType entityType)
//		{
//			if(entityType == null)
//				throw new ArgumentNullException("entityType");
//			
//			// get...
//			SqlTable customTable = ExtendedPropertySettings.GetCustomPropertySqlTableForEntityType(entityType);
//			if(customTable == null)
//				throw new InvalidOperationException("customTable is null.");
//
//			// scripts...
//			string[] createTableSql = Database.DefaultDialect.GetCreateTableScript(customTable, SqlTableScriptFlags.IgnoreExistenceErrors);
//			if(createTableSql == null)
//				throw new InvalidOperationException("'createTableSql' is null.");
//			if(createTableSql.Length == 0)
//				throw new InvalidOperationException("'createTableSql' is zero-length.");
//
//			// mbr - 08-12-2005 - use a separate conn...
//			IConnection connection = Connection.CreateConnection(entityType);
//			if(connection == null)
//				throw new InvalidOperationException("connection is null.");
//			try
//			{
//				connection.BeginTransaction();
//
//				// walk...
//				foreach(string statement in createTableSql)
//					connection.ExecuteNonQuery(new SqlStatement(statement));
//
//				// ok...
//				connection.Commit();
//			}
//			catch(Exception ex)
//			{
//				try
//				{
//					connection.Rollback();
//				}
//				catch
//				{
//					// no-op...
//				}
//
//				// throw...
//				throw new InvalidOperationException(string.Format("Failed to create table for '{0}'.", entityType.Name), ex);
//			}
//			finally
//			{			
//				if(connection != null)
//					connection.Dispose();
//			}
//		}

		// mbr - 25-09-2007 - moved to provider.		
//		/// <summary>
//		/// Returns true if the table for the extended properties exist
//		/// </summary>
//		/// <param name="entityType"></param>
//		/// <returns></returns>
//		private bool IsExtendedPropertiesTableCreated(EntityType entityType)
//		{
//			if(!Database.HasConnectionSettings)
//				return false;
//
//			// return...
//			// mbr - 08-12-2005 - changed...
////			return Database.DoesTableExist(GetExtendedNativeNameForEntityType(entityType).Name);
//			return Database.DoesTableExist(entityType.NativeNameExtended);
//		}

		// mbr - 25-09-2007 - moved to provider.	
//		/// <summary>
//		/// Creates the extended properties table.
//		/// </summary>
//		/// <param name="entityType"></param>
//		private void EnsureExtendedPropertiesTableCreated(EntityType entityType)
//		{
//			if(entityType == null)
//				throw new ArgumentNullException("entityType");
//			
//			// check...
//			if(!(IsExtendedPropertiesTableCreated(entityType)))
//				this.CreateExtendedPropertiesTable(entityType);
//		}

		/// <summary>	
		/// Gets the extended fields for the entity type
		/// </summary>
		/// <param name="entityType"></param>
		/// <returns></returns>
		public EntityField[] GetExtendedPropertiesForEntityType(EntityType entityType)
		{
			if(entityType == null)
				throw new ArgumentNullException("entityType");

			// mbr - 25-09-2007 - the original meaning of this was flawed.  basically if the table was missing,
			// no fields would be returned.  for c7 (and others?), if the table is missing there's actually a 
			// problem with the app so return the fields and let something later on complain.

//			// check...
//			if(!(IsExtendedPropertiesTableCreated(entityType)))
//				return new EntityField[]{};
//			else
//			{
				// walk...
				ArrayList fields = new ArrayList();
				foreach(ExtendedPropertyDefinition property in Properties.GetPropertiesForEntityType(entityType))
					fields.Add(property.GetEntityField());

				// return...
				return (EntityField[]) fields.ToArray(typeof(EntityField));
//			}
		}
		
		/// <summary>
		/// Gets or sets the version
		/// </summary>
		internal int Version
		{
			get
			{
				return _version;
			}
			set
			{
				// check to see if the value has changed...
				if(value != _version)
				{
					// set the value...
					_version = value;
				}
			}
		}

		public void Clear()
		{
			this.Properties.Clear();
			this.Lookups.Clear();
		}

		// mbr - 25-09-2007 - moved to provider.		
//		/// <summary>
//		/// Returns true if the property is being used.
//		/// </summary>
//		/// <param name="entityType"></param>
//		/// <param name="name"></param>
//		/// <returns></returns>
//		private bool IsPropertyInUse(EntityType entityType, string name)
//		{
//			 if(entityType == null)
//			 	throw new ArgumentNullException("entityType");
//			 if(name == null)
//			 	throw new ArgumentNullException("name");
//			 if(name.Length == 0)
//			 	throw new ArgumentOutOfRangeException("'name' is zero-length.");
//			 
//			// table?
//			if(IsExtendedPropertiesTableCreated(entityType))
//			{
//				// dialect...
//				SqlDialect dialect = entityType.Dialect;
//				if(dialect == null)
//					throw new InvalidOperationException("dialect is null.");
//
//				// find it...
//				StringBuilder builder = new StringBuilder();
//				builder.Append(dialect.SelectKeyword);
//				builder.Append(" TOP 1 ");
//				builder.Append(dialect.FormatColumnName("Name"));
//				builder.Append(" ");
//				builder.Append(dialect.FromKeyword);
//				builder.Append(" ");
//				builder.Append(dialect.FormatNativeName(entityType.NativeNameExtended));
//				builder.Append(" ");
//				builder.Append(dialect.WhereKeyword);
//				builder.Append(" ");
//				builder.Append(dialect.FormatColumnName("Name"));
//				builder.Append("=");
//				const string paramName = "name";
//				builder.Append(dialect.FormatVariableNameForQueryText(paramName));
//
//				// create...
//				SqlStatement statement = new SqlStatement(builder.ToString());
//				statement.Parameters.Add(new SqlStatementParameter(paramName, DbType.String, name));
//
//				// run...
//				object result = Database.ExecuteScalar(statement);
//				if(result is string)
//					return true;
//				else
//					return false;
//			}
//			else
//				return false;
//		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="entityType"></param>
		/// <param name="nativeName"></param>
		public void DeleteProperty(EntityType entityType, string nativeName)
		{
			if(entityType == null)
				throw new ArgumentNullException("entityType");
			if(nativeName == null)
				throw new ArgumentNullException("name");
			if(nativeName.Length == 0)
				throw new ArgumentOutOfRangeException("'name' is zero-length.");
			
			// check...
			if(Database.ExtensibilityProvider == null)
				throw new InvalidOperationException("Database.ExtensibilityProvider is null.");

			// mbr - 25-09-2007 - changed to provider			
//			if(this.IsPropertyInUse(entityType, nativeName))
			if(Database.ExtensibilityProvider.IsPropertyInUse(entityType, nativeName))
				throw new InvalidOperationException(string.Format("The property '{0}' on '{1} ({2})' is in use.", nativeName, entityType.Name, entityType.NativeName));

			// delete...
			ExtendedPropertyDefinition prop = this.Properties[entityType.Id, nativeName];
			if(prop == null)
				throw new InvalidOperationException("prop is null.");

			// delete it...
			this.Properties.Remove(prop);
		}

		/// <summary>
		/// Suggests a native name for a property.
		/// </summary>
		/// <param name="entityType"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		public string SuggestNativeName(EntityType entityType, string name)
		{
			if(entityType == null)
				throw new ArgumentNullException("entityType");
			if(name == null)
				throw new ArgumentNullException("name");
			if(name.Length == 0)
				throw new ArgumentOutOfRangeException("'name' is zero-length.");
			
			// strip out the illegal chars...
			StringBuilder builder = new StringBuilder();
			foreach(char c in name)
			{
				if(EntityType.IsLegalForIdenitifer(c))
					builder.Append(c);
			}

			// anything?
			string nativeName = null;
			if(builder.Length > 0)
			{
				// get...
				nativeName = builder.ToString();

				// legal...
				char first = char.ToLower(name[0]);
				if((first < 'a' || first > 'z') && first != '_')
					nativeName = "_" + nativeName;
			}
			else
				nativeName = "Property";

			// has is already been used?
			int index = 1;
			string entityTypeId = entityType.Id;
			while(true)
			{
				string useName = nativeName;
				if(index > 1)
					useName += index.ToString();
				ExtendedPropertyDefinition existing = this.Properties[entityTypeId, useName];
				if(existing == null)
				{
					nativeName = useName;
					break;
				}			
	
				// else...
				index++;
			}

			// return...
			return nativeName;
		}

		public ExtendedPropertyDefinition CreateProperty(EntityType entityType, string name, ExtendedPropertyDataType dataType, long size, 
			bool multiValue)
		{
			if(entityType == null)
				throw new ArgumentNullException("entityType");
			if(name == null)
				throw new ArgumentNullException("name");
			if(name.Length == 0)
				throw new ArgumentOutOfRangeException("'name' is zero-length.");
			
			// get...
			string nativeName = SuggestNativeName(entityType, name);
			if(nativeName == null)
				throw new InvalidOperationException("'nativeName' is null.");
			if(nativeName.Length == 0)
				throw new InvalidOperationException("'nativeName' is zero-length.");
			
			// defer...
			return this.CreateProperty(entityType, name, nativeName, dataType, size, multiValue);
		}

		public ExtendedPropertyDefinition CreateProperty(EntityType entityType, string name, string nativeName, ExtendedPropertyDataType dataType, 
			long size, bool multiValue)
		{
			if(entityType == null)
				throw new ArgumentNullException("entityType");
			if(name == null)
				throw new ArgumentNullException("name");
			if(name.Length == 0)
				throw new ArgumentOutOfRangeException("'name' is zero-length.");
			if(nativeName == null)
				throw new ArgumentNullException("nativeName");
			if(nativeName.Length == 0)
				throw new ArgumentOutOfRangeException("'nativeName' is zero-length.");
			if(nativeName.Length > DatabaseExtensibilityProvider.MaxNativeNameLength)
			{
				throw new ArgumentException(string.Format("Native name '{0}' is {1} characters in length and the maximum is {2} characters.", nativeName, 
					nativeName.Length, DatabaseExtensibilityProvider.MaxNativeNameLength));
			}
			if(dataType == null)
				throw new ArgumentNullException("dataType");

			// mbr - 25-09-2007 - check with provider...
			if(Database.ExtensibilityProvider == null)
				throw new InvalidOperationException("Database.ExtensibilityProvider is null.");
			Database.ExtensibilityProvider.AssertDefinition(entityType, name, nativeName, dataType, size, multiValue);
			
			// is it a legal name?
			EntityType.AssertIsLegalIdentifierName(nativeName);

			// do we already have it?
			ExtendedPropertyDefinition existing = this.Properties[entityType.Id, nativeName];
			if(existing != null)
				throw new InvalidOperationException(string.Format("'{0}' already has a property called '{1}' (name).", entityType.Name, nativeName));

			// mbr - 12-12-2005 - other check for the display name...
			existing = this.Properties.GetByName(entityType.Id, name, false);
			if(existing != null)
				throw new InvalidOperationException(string.Format("'{0}' already has a property called '{1}' (display name).", entityType.Name, name));

			// create it...
			ExtendedPropertyDefinition prop = new ExtendedPropertyDefinition(name, nativeName, dataType, entityType.Id);
			prop.Size = size;

			// check...
			if(dataType.SupportsMultiValue && multiValue)
				prop.MultiValue = true;
			else
				prop.MultiValue = false;

			// add...
			this.Properties.Add(prop);

			// mbr - 02-10-2007 - for c7 - we don't want to do this here because it will frankly take too
			// long.  do it later...
//			EnsureExtendedTableUpToDate(entityType);

			// return...
			return prop;
		}

		/// <summary>
		/// Ensures that the extended props table is up-to-date.
		/// </summary>
		// mbr - 25-09-2007 - for c7 - added.		
		internal void EnsureExtendedTableUpToDate(EntityType et)
		{
			if(Database.ExtensibilityProvider == null)
				throw new InvalidOperationException("Database.ExtensibilityProvider is null.");
			Database.ExtensibilityProvider.EnsureExtendedTableUpToDate(et);
		}

		/// <summary>
		/// Deletes a lookup.
		/// </summary>
		/// <param name="name"></param>
		public void DeleteLookup(string name)
		{
			if(name == null)
				throw new ArgumentNullException("name");
			
			// walk...
			foreach(ExtendedPropertyDefinition prop in this.Properties)
			{
				if(prop.DataType is LookupExtendedPropertyDataType && 
					string.Compare(((LookupExtendedPropertyDataType)prop.DataType).LookupName, name, true, System.Globalization.CultureInfo.InvariantCulture) == 0)
				{	
					throw new InvalidOperationException(string.Format("The lookup '{0}' is currently used by property '{1}' on '{2}'.", name, prop.NativeName, 
						prop.EntityTypeId));
				}
			}

			// remove it...
			ExtendedLookupDefinition lookup = this.Lookups[name];
			if(lookup != null)
				this.Lookups.Remove(lookup);
		}

		/// <summary>
		/// Gets the properites in the settings grouped by entity type.
		/// </summary>
		/// <returns>A dictionary grouping the items today.  The keys are entity types, and values are an IList of 
		/// <c>ExtendedPropertyDefinition</c> instances.</returns>
		public IDictionary GetExtendedPropertiesByEntityType()
		{
			IDictionary results = new HybridDictionary();

			// walk...
			foreach(EntityType et in EntityType.GetEntityTypes())
			{
				// create...
				ArrayList perType = new ArrayList();
				results[et] = perType;

				// props...
				foreach(ExtendedPropertyDefinition prop in this.Properties)
				{
					if(string.Compare(prop.EntityTypeId, et.Id, true, Cultures.System) == 0)
						perType.Add(prop);
				}
			}

			// return...
			return results;
		}
	}
}
