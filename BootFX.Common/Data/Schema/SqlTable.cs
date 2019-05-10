// BootFX - Application framework for .NET applications
// 
// File: SqlTable.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.CodeDom;
using System.ComponentModel;
using System.Reflection;
using System.Xml;
using System.Data;
using System.Collections;
using BootFX.Common.Xml;
using BootFX.Common.Entities;
using BootFX.Common.Data.Formatters;

namespace BootFX.Common.Data.Schema
{
	/// <summary>
	/// Defines a table.
	/// </summary>
	public class SqlTable : SqlMemberWithColumns, ISqlProgrammable
	{
		/// <summary>
		/// Defines the stored procedure prefix.
		/// </summary>
		internal const string SprocPrefix = "bfxsp";

		/// <summary>
		/// Private field to support <c>AssociatedLinks</c> property.
		/// </summary>
		private SqlChildToParentLinkCollection _associatedLinks = new SqlChildToParentLinkCollection();
		
		/// <summary>
		/// Private field to support <c>AssociatedLinks</c> property.
		/// </summary>
		private SqlChildToParentLinkCollection _linksToParents;
		
		/// <summary>
		/// Private field to support <c>Indexes</c> property.
		/// </summary>
		private SqlIndexCollection _indexes = new SqlIndexCollection();

		/// <summary>
		/// Private field to support <c>EntityType</c> property.
		/// </summary>
		private EntityType _entityType = null;

		/// <summary>
		/// Raised when the <c>Modifiers</c> property has changed.
		/// </summary>
		[Browsable(true), Category("Property Changed"), Description("Raised when the Modifiers property has changed.")]
		public event EventHandler ModifiersChanged;

		/// <summary>
		/// Private field to support <c>Modifiers</c> property.
		/// </summary>
		private TableModifiers _modifiers = TableModifiers.Public;

        private bool _generateDto;

		/// <summary>
		/// Constructor.
		/// </summary>
		public SqlTable(string nativeName) 
			: base(nativeName)
		{
			this.Initialize();
		}

		/// <summary>
		/// Constructor - specifically for use of creating extended tables.
		/// </summary>
		/// <param name="entityType"></param>
		/// <param name="nativeName"></param>
		// mbr - 02-10-2007 - for c7 - added.		
		internal SqlTable(EntityType entityType, string nativeName)
			: this(nativeName)
		{
			if(entityType == null)
				throw new ArgumentNullException("entityType");
			
			// set...
			_entityType = entityType;
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		public SqlTable(string nativeName, string name) 
			: base(nativeName, name)
		{
			this.Initialize();
		}

		/// <summary>
		/// Creates a SQL table from an entity type.
		/// </summary>
		/// <param name="entityType"></param>
		public SqlTable(SqlSchema schema, EntityType entityType)  
			: base(entityType.NativeName.Name, entityType.Name)
		{
			// store the entity type for later use
			_entityType = entityType;

			// init...
			this.Initialize();

			// mbr - 04-10-2007 - set the schema...
			this.SetSchema(schema);
			
			// columns...
			foreach(EntityField field in entityType.Fields)
			{
				// mbr - 07-12-2005 - only non-extended...
				if(!(field.IsExtendedProperty))
				{
					// mbr - 04-10-2007 - set schema...					
					SqlColumn column = new SqlColumn(field);
					column.SetSchema(schema);
					this.Columns.Add(column);
				}
			}

			// indexes...
			foreach(EntityIndex index in entityType.Indexes)
			{
				// mbr - 04-10-2007 - set schema...				
				SqlIndex sqlIndex = new SqlIndex(this, index);
				sqlIndex.SetSchema(schema);
				this.Indexes.Add(sqlIndex);
			}

			// relationships...
			foreach(ChildToParentEntityLink relationship in entityType.Links)
				this.LinksToParents.Add(new SqlChildToParentLink(schema, relationship));
		}

		/// <summary>
		/// Sets up the object.
		/// </summary>
		private void Initialize()
		{
			this.Generate = false;
			_linksToParents = new SqlChildToParentLinkCollection(this);
		}

		/// <summary>
		/// Gets the size of the table.
		/// </summary>
		/// <remarks>This is the minimum size because this size does not include the size of any large columns.  (See <see cref="HasLargeColumns"></see>.)</remarks>
		public long MinimumSize
		{
			get
			{
				long size = 0;
				foreach(SqlColumn column in this.Columns)
				{
					// get...
					long length = column.ActualLength;
					if(length > 0)
						size += length;
				}

				// return...
				return size;
			}
		}

		/// <summary>
		/// Returns true if the table has large columns defined.
		/// </summary>
		public bool HasLargeColumns
		{
			get
			{
				if(this.NumLargeColumns == 0)
					return false;
				else
					return true;
			}
		}

		/// <summary>
		/// Gets the number of large columns.
		/// </summary>
		public int NumLargeColumns
		{
			get
			{
				int num = 0;
				foreach(SqlColumn column in this.Columns)
				{
					if(column.IsLarge)
						num++;
				}

				// return...
				return num;
			}
		}

		/// <summary>
		/// Suggests a singlular name for the given name.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public static string SuggestSingularName(string name)
		{
			if(name == null)
				throw new ArgumentNullException("name");
			if(name.Length == 0)
				throw new ArgumentOutOfRangeException("'name' is zero-length.");
			
			// check...
			// this is fairly ugly - would make sense to change this to a dictionary lookup...
			string checkName = name.ToLower();
			if(ManglePluralName(ref name, checkName, "ies", "y"))
				return name;
			if(ManglePluralName(ref name, checkName, "values", "value"))
				return name;
			if(ManglePluralName(ref name, checkName, "bes", "be"))
				return name;
			if(ManglePluralName(ref name, checkName, "ces", "ce"))
				return name;
			if(ManglePluralName(ref name, checkName, "des", "de"))
				return name;
			if(ManglePluralName(ref name, checkName, "fes", "fe"))
				return name;
			if(ManglePluralName(ref name, checkName, "ges", "ge"))
				return name;
			if(ManglePluralName(ref name, checkName, "jes", "je"))
				return name;
			if(ManglePluralName(ref name, checkName, "kes", "ke"))
				return name;
			if(ManglePluralName(ref name, checkName, "les", "le"))
				return name;
			if(ManglePluralName(ref name, checkName, "mes", "me"))
				return name;
			if(ManglePluralName(ref name, checkName, "nes", "me"))
				return name;
			if(ManglePluralName(ref name, checkName, "pes", "pe"))
				return name;
			if(ManglePluralName(ref name, checkName, "qes", "qe"))
				return name;
			if(ManglePluralName(ref name, checkName, "res", "re"))
				return name;
			if(ManglePluralName(ref name, checkName, "tes", "te"))
				return name;
			if(ManglePluralName(ref name, checkName, "ves", "ve"))
				return name;
			if(ManglePluralName(ref name, checkName, "wes", "we"))
				return name;
			if(ManglePluralName(ref name, checkName, "xes", "xe"))
				return name;
			if(ManglePluralName(ref name, checkName, "yes", "ye"))
				return name;
			if(ManglePluralName(ref name, checkName, "zes", "ze"))
				return name;
			if(ManglePluralName(ref name, checkName, "es", string.Empty))
				return name;
			if(ManglePluralName(ref name, checkName, "s", string.Empty))
				return name;

			// nope...
			return name;
		}

		/// <summary>
		/// Mangles the given plural name.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="lowerName"></param>
		/// <param name="checkFor"></param>
		/// <param name="replace"></param>
		/// <returns></returns>
		private static bool ManglePluralName(ref string name, string lowerName, string checkFor, string replace)
		{
			try
			{
				int index = lowerName.LastIndexOf(checkFor);
				if(index != -1 && index == name.Length - checkFor.Length)
				{
					// set...
					name = name.Substring(0, index) + replace;

					// return...
					return true;
				}
				else
					return false;
			}
			catch(Exception ex)
			{
				throw new InvalidOperationException(string.Format("Failed to mangle plural name.  Name: {0}, check for: {1}, replace: {2}.", name, checkFor, replace), ex);
			}
		}

		/// <summary>
		/// Gets the columns defined as key columns.
		/// </summary>
		/// <returns></returns>
		public SqlColumn[] GetKeyColumns()
		{
			// defer...
			return this.GetColumnsWithFlags(EntityFieldFlags.Key);
		}

		/// <summary>
		/// Gets the columns defined as key columns.
		/// </summary>
		/// <returns></returns>
		private SqlColumn[] GetColumnsWithFlags(EntityFieldFlags flags)
		{
			return this.Columns.GetColumnsWithFlags(flags);
		}

		/// <summary>
		/// Gets a collection of SqlIndex objects.
		/// </summary>
		public SqlIndexCollection Indexes
		{
			get
			{
				return _indexes;
			}
		}

		protected override void WriteXml(System.Xml.XmlWriter xml, WriteXmlContext context)
		{
			base.WriteXml(xml, context);

			// indexes...
			xml.WriteStartElement("Indexes");
			foreach(SqlIndex index in this.Indexes)
				index.ToXml(xml, context.Encoding);
			xml.WriteEndElement();

			// links...
			xml.WriteStartElement("ChildToParentLinks");
			foreach(SqlChildToParentLink link in this.LinksToParents)
				link.ToXml(xml, context.Encoding);
			xml.WriteEndElement();
			xml.WriteElementString("Modifiers", this.Modifiers.ToString());

            xml.WriteElementString("GenerateDto", this.GenerateDto.ToString());
		}

		/// <summary>
		/// Fixes up the object.
		/// </summary>
		internal override void Fixup()
		{
			// base...
			base.Fixup ();

			// make sure we don't have columns with the same name as the table...
			foreach(SqlColumn column in this.Columns)
			{
				if(string.Compare(column.Name, Name, true, System.Globalization.CultureInfo.InvariantCulture) == 0)
					column.Name += "Value";
			}

			// TODO: make sure we don't have duplicate items...

			// fixup columns...
			for(int index = 0; index < this.Columns.Count; index++)
			{
				this.Columns[index].Fixup();
				this.Columns[index].Ordinal = index;
			}

			// mangle index names...
			SqlSchema.MangleDuplicateNames(this.Indexes.ToArray());

			// fixup indexes...
			for(int index = 0; index < this.Indexes.Count; index++)
			{
				// fix...
				this.Indexes[index].Fixup();
				this.Indexes[index].Ordinal = index;
			}

			// copy...
			foreach(SqlChildToParentLink parentLink in this.LinksToParents)
			{
				if(parentLink.Name == null || parentLink.Name.Length == 0 || parentLink.Name == parentLink.NativeName)
					parentLink.Name = parentLink.ParentTable.Name;
			}

			// mangle link names...
			SqlSchema.MangleDuplicateNames(this.LinksToParents.ToArray());
		}

		/// <summary>
		/// Gets a collection of SqlChildToParentLink objects.
		/// </summary>
		public SqlChildToParentLinkCollection LinksToParents
		{
			get
			{
				return _linksToParents;
			}
		}

		/// <summary>
		/// Gets a collection of SqlChildToParentLink objects.
		/// </summary>
		public SqlChildToParentLinkCollection AssociatedLinks
		{
			get
			{
				return _associatedLinks;
			}
		}

		internal override void Merge(XmlElement element, bool createIfNotFound)
		{
			base.Merge (element, createIfNotFound);

			// walk tables...
			foreach(XmlElement columnElement in element.SelectNodes("Columns/SqlColumn"))
			{
				// get the name...
				string nativeName = XmlHelper.GetElementString(columnElement, "NativeName", OnNotFound.ThrowException);
				if(nativeName == null)
					throw new InvalidOperationException("'nativeName' is null.");
				if(nativeName.Length == 0)
					throw new InvalidOperationException("'nativeName' is zero-length.");

				// get...
				SqlColumn column = this.Columns[nativeName];
				if(column == null && createIfNotFound)
				{
					column = new SqlColumn(nativeName, DbType.Int32, -1, EntityFieldFlags.Normal);
					this.Columns.Add(column);
				}

				// add...
				if(column != null)
					column.Merge(columnElement, createIfNotFound);
			}

			// walk the links...
			foreach(XmlElement linkElement in element.SelectNodes("ChildToParentLinks/SqlChildToParentLink"))
			{
				// get...
				string nativeName = XmlHelper.GetElementString(linkElement, "NativeName", OnNotFound.ReturnNull);
				if(nativeName == null)
					throw new InvalidOperationException("'nativeName' is null.");
				if(nativeName.Length == 0)
					throw new InvalidOperationException("'nativeName' is zero-length.");

				// find it...
				SqlChildToParentLink link = this.LinksToParents[nativeName];
				if(link == null && createIfNotFound)
				{
					link = new SqlChildToParentLink(nativeName, this);
					this.LinksToParents.Add(link);
				}

				// add....
				if(link != null)
					link.Merge(linkElement, createIfNotFound);
			}

			// mbr - 21-09-2007 - do we actually have modifiers?
			string modifiersAsString = XmlHelper.GetElementString(element, "Modifiers", OnNotFound.ReturnNull);
			if(modifiersAsString != null && modifiersAsString.Length > 0)
				this.Modifiers = (TableModifiers)Enum.Parse(typeof(TableModifiers), modifiersAsString, true);

            // xml...
            var asString = element.GetElementString("GenerateDto", OnNotFound.ReturnNull);
            if (!(string.IsNullOrEmpty(asString)))
                this.GenerateDto = ConversionHelper.ToBoolean(asString);
            else
                this.GenerateDto = false;
		}

		/// <summary>
		/// Gets the schema structure for the given entity type.
		/// </summary>
		/// <param name="entityType"></param>
		/// <returns></returns>
		public static SqlTable GetTable(SqlSchema schema, EntityType entityType)
		{
			if(entityType == null)
				throw new ArgumentNullException("entityType");
			
			return new SqlTable(schema, entityType);
		}

		/// <summary>
		/// Gets the name of the insert sproc.
		/// </summary>
		internal string InsertSprocName
		{
			get
			{
				return SprocPrefix + "I" + this.NativeName;
			}
		}

		/// <summary>
		/// Gets the schema work unit specific to this table.
		/// </summary>
		/// <returns></returns>
		internal WorkUnitCollection GetCreateTableWorkUnit()
		{
			if(_entityType == null)
				throw new InvalidOperationException("_entityType");

			WorkUnitCollection units = new WorkUnitCollection();

			units.Add(new CreateTableSchemaWorkUnit(_entityType,this));

			// check foreign keys...
			foreach(SqlChildToParentLink foreignKey in LinksToParents)
				units.Add(new CreateForeignKeySchemaWorkUnit(_entityType, foreignKey));

			return units;
		}

		/// <summary>
		/// Gets the schema work units specific to this table.
		/// </summary>
		/// <param name="existingTable"></param>
		/// <returns></returns>
		internal WorkUnitCollection GetSchemaWorkUnits(SqlTable existingTable)
		{
			if(_entityType == null)
				throw new ArgumentNullException("entityType");

			if(existingTable == null)
				throw new ArgumentNullException("existingTable");
			
			// results...
			WorkUnitCollection results = new WorkUnitCollection();

			// Removed indexes.
			SqlIndexCollection removedIndexes = new SqlIndexCollection();

			// Removed indexes.
			SqlChildToParentLinkCollection removedForeignKeys = new SqlChildToParentLinkCollection();

			// find missing columns...
			foreach(SqlColumn column in this.Columns)
			{
				SqlColumn existingColumn = null;
				foreach(SqlColumn scanColumn in existingTable.Columns)
				{
					if(string.Compare(column.NativeName, scanColumn.NativeName, true, System.Globalization.CultureInfo.InvariantCulture) == 0)
					{
						existingColumn = scanColumn;
						break;
					}
				}

				// found?
				if(existingColumn == null)
					results.Add(new AddColumnSchemaWorkUnit(_entityType,column));
				else
				{
					// get the new column metrics.  this is tricky, because if the length changes to be less than what it was,
					// we don't want to change the length, but we might want to change other metrics.
					bool changed = false;
					string reason = null;

					// check...
					long newLength = existingColumn.Length;
					if(column.Length > newLength && newLength != -1)
					{
						newLength = column.Length;

						// mbr - 24-07-2007 - case 321 - added reason...						
						changed = true;
						reason = string.Format("Length changed from {0} to {1}", existingColumn.Length, column.Length);
					}

					// flags?
					bool newIsNullable = false;
					if(!(existingColumn.IsKey))
					{
						newIsNullable = existingColumn.IsNullable;
						if(column.IsNullable != newIsNullable)
						{
							newIsNullable = column.IsNullable;

							// mbr - 24-07-2007 - case 321 - added reason...							
							changed = true;
							reason = string.Format("Nullability changed to '{0}'", column.IsNullable);
						}
					}
					else
						newIsNullable = false;

					// type...
					DbType newType = existingColumn.DbType;
					bool newIsLarge = existingColumn.IsLarge;
					if(column.DbType != newType || column.IsLarge != newIsLarge)
					{
						// mbr - 24-07-2007 - case 308 - this didn't do anything, hence have changed the code 
						// to the original intention - i.e. if something was an int and it's now a string, it will try and
						// convert.  the original meaning of CanUpgradeType was to make sure the fx knew how 
						// to go from 

						//						// are the compatible?
						//						bool ok = false;
						//						try
						//						{
						//							this.CanUpgradeType(newType, newIsLarge, column.DbType, column.IsLarge);
						//						}
						//						catch(Exception ex)
						//						{
						//							throw new InvalidOperationException(string.Format("Failed to check upgradability of '{0}' on '{1}'.", column, column.Table), ex);
						//						}

						// can we?
						//if(ok)
					{
						newType = column.DbType;
						newIsLarge = column.IsLarge;

						// mbr - 24-07-2007 - case 321 - added reason...						
						changed = true;
						string fromAsString = existingColumn.DbType.ToString();
						if(existingColumn.IsLarge)
							fromAsString += " (large)";
						string toAsString = column.DbType.ToString();
						if(column.IsLarge)
							toAsString += " (large)";
						reason = string.Format("Type changed from '{0}' to '{1}'", fromAsString, toAsString);
					}
					}

					// now check the default default...
					SqlDatabaseDefault existingDefault = existingColumn.DefaultExpression;
					SqlDatabaseDefault newDefault = column.DefaultExpression;

					// alter...
					if(changed && !(column.IsLarge))
					{
						// Now we can check the indexes are on this column as they need to be removed
						foreach(SqlIndex index in existingTable.Indexes)
						{
							if(index.Columns.Contains(column.NativeName))
							{
								removedIndexes.Add(index);
								results.Add(new DropIndexSchemaWorkUnit(_entityType,existingTable,index));
							}
						}

						// Now we can check the foreign keys are on this column as they need to be removed
						foreach(SqlChildToParentLink foreignKey in existingTable.LinksToParents)
						{
							if(foreignKey.Columns.Contains(column.NativeName))
							{
								removedForeignKeys.Add(foreignKey);
								results.Add(new DropForeignKeySchemaWorkUnit(_entityType,foreignKey));
							}
						}
                        
						// mbr - 24-07-2007 - case 321 - added reason.						
						results.Add(new AlterColumnSchemaWorkUnit(_entityType,column, newType, newLength, 
							newIsLarge, newIsNullable, newDefault, reason));
					}

					if(((existingDefault != null && newDefault == null) || (existingDefault == null && newDefault != null)) ||
						(existingDefault != null && newDefault != null && !(existingDefault.Equals(newDefault))))
					{
						if(existingDefault != null)
							results.Add(new DropConstraintSchemaWorkUnit(_entityType,column,existingDefault));
						
						if(newDefault != null)
							results.Add(new AddConstraintSchemaWorkUnit(_entityType,column, newDefault));
					}
				}
			}


			// Check existing indexes
			foreach(SqlIndex index in Indexes)
			{
                SqlIndex existingIndex = null;
                foreach (SqlIndex scanIndex in existingTable.Indexes)
                {
                    //
                    if (string.Compare(scanIndex.NativeName, index.NativeName, true, System.Globalization.CultureInfo.InvariantCulture) == 0)
                    {
                        // mbr - 2011-11-02 - do we need to drop it?
                        if (!(scanIndex.DoesMatch(index)))
                        {
                            // drop...
                            results.Add(new DropIndexSchemaWorkUnit(_entityType, this, index));
                        }
                        else
                        {
                            // create...
                            existingIndex = scanIndex;
                        }

                        // stop...
                        break;
                    }
                }

                // found?
                if (existingIndex == null || removedIndexes[existingIndex.Name] != null)
                    results.Add(new CreateIndexSchemaWorkUnit(_entityType, this, index));
            }

            // Check existing foreign keys
            foreach (SqlChildToParentLink foreignKey in LinksToParents)
			{
				SqlChildToParentLink existingForeignKey = null;
				foreach(SqlChildToParentLink scanForeignKey in existingTable.LinksToParents)
				{
					//
					if(string.Compare(scanForeignKey.NativeName, foreignKey.NativeName, true, System.Globalization.CultureInfo.InvariantCulture) == 0)
					{
						existingForeignKey = scanForeignKey;
						break;
					}
				}

				// found?
				if(existingForeignKey == null || removedIndexes[existingForeignKey.Name] != null)
				{
					// mbr - 04-10-2007 - case 825 - only do this if the parent is referenced in the schema...
					if(Schema == null)
						throw new InvalidOperationException("Schema is null.");
					bool ok = foreignKey.IsSupported(this.Schema);
					
					// ok?
					if(ok)
						results.Add(new CreateForeignKeySchemaWorkUnit(_entityType, foreignKey));
				}
			}

			// return...
			return results;
		}

		/// <summary>
		/// Gets or sets the modifiers for this member
		/// </summary>
		/// <remarks>This should ideally be the full type name, e.g. <c>Foo.Bar.Widget</c>.</remarks>
		public TableModifiers Modifiers
		{
			get
			{
				return _modifiers;
			}
			set
			{
				if (value != _modifiers)
				{
					// set the value...
					_modifiers = value;
					OnModifiersChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Raises the <c>ModifiersChanged</c> event.
		/// </summary>
		/// <param name="e"></param>
		protected virtual void OnModifiersChanged(EventArgs e)
		{
			if (ModifiersChanged != null)
				ModifiersChanged(this, e);
		}

		// mbr - 24-07-2007 - case 308 - removed.		
		//		/// <summary>
		//		/// Returns true if the given type upgrade should be done, returns false if it should not be done or throws an exception is invalid (e.g. string to int).
		//		/// </summary>
		//		/// <param name="existingType"></param>
		//		/// <param name="existingIsLarge"></param>
		//		/// <param name="newType"></param>
		//		/// <param name="newIsLarge"></param>
		//		/// <returns></returns>
		//		private bool CanUpgradeType(DbType existingType, bool existingIsLarge, DbType newType, bool newIsLarge)
		//		{
		//			// mbr - 11-07-2007 - before, this was reasonably sophisticated, but perhaps a better plan
		//			// is just to allow the database to reject invalid changes?  (the problem is that there is no
		//			// reconciliation steps if this returns 'false'.)
		//
		////			switch(existingType)
		////			{
		////				case DbType.Boolean:
		////					switch(newType)
		////					{
		////						case DbType.Boolean:
		////							return false;
		////
		////						case DbType.Int16:
		////						case DbType.Int32:
		////						case DbType.Int64:
		////							return true;
		////
		////						default:
		////							throw new InvalidOperationException(string.Format("Cannot convert '{0}' to '{1}'.", existingType, newType));
		////					}
		////
		////				case DbType.Int32:
		////					switch(newType)
		////					{
		////						case DbType.Int16:
		////						case DbType.Int32:
		////						case DbType.Boolean:
		////							return false;
		////
		////						case DbType.Int64:
		////						case DbType.Single:
		////						case DbType.Double:
		////						case DbType.Decimal:
		////							return true;
		////
		////						default:
		////							throw new InvalidOperationException(string.Format("Cannot convert '{0}' to '{1}'.", existingType, newType));
		////					}
		////
		////				case DbType.Int64:
		////					switch(newType)
		////					{
		////						case DbType.Int16:
		////						case DbType.Int32:
		////						case DbType.Int64:
		////							return false;
		////						default:
		////							throw new InvalidOperationException(string.Format("Cannot convert '{0}' to '{1}'.", existingType, newType));
		////					}
		////
		////				case DbType.Double:
		////					switch(newType)
		////					{
		////						case DbType.Double:
		////							return false;
		////
		////						case DbType.Single:
		////							return true;
		////
		////						default:
		////							throw new InvalidOperationException(string.Format("Cannot convert '{0}' to '{1}'.", existingType, newType));
		////					}
		////
		////				case DbType.AnsiString:
		////				case DbType.AnsiStringFixedLength:
		////					switch(newType)
		////					{
		////						case DbType.AnsiString:
		////						case DbType.AnsiStringFixedLength:
		////
		////							// only do this if we're going large...
		////							if(!(existingIsLarge) && newIsLarge)
		////								return true;
		////							else
		////								return false;
		////
		////						case DbType.String:
		////						case DbType.StringFixedLength:
		////							return true;
		////
		////						default:
		////							throw new InvalidOperationException(string.Format("Cannot convert '{0}' to '{1}'.", existingType, newType));
		////					}
		////
		////				case DbType.String:
		////				case DbType.StringFixedLength:
		////					switch(newType)
		////					{
		////						case DbType.String:
		////						case DbType.StringFixedLength:
		////
		////							// only do this if we're going large...
		////							if(!(existingIsLarge) && newIsLarge)
		////								return true;
		////							else
		////								return false;
		////
		////						case DbType.AnsiString:
		////						case DbType.AnsiStringFixedLength:
		////							return false;
		////
		////						default:
		////							throw new InvalidOperationException(string.Format("Cannot convert '{0}' to '{1}'.", existingType, newType));
		////					}
		////
		////				default:
		////					throw new NotSupportedException(string.Format("Cannot handle '{0}' ({1}).   (New type: {2}, is large: {3})", 
		////						existingType, existingType.GetType(), newType, newIsLarge));
		////			}
		//
		//			// ok...
		//			return true;
		//		}

        public bool GenerateDto
        {
            get
            {
                return _generateDto;
            }
            set
            {
                _generateDto = value;
                this.OnPropertyChanged(new PropertyChangedEventArgs("GenerateDto"));
            }
        }
	}
}
