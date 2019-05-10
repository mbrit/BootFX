// BootFX - Application framework for .NET applications
// 
// File: SqlChildToParentLink.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using BootFX.Common.Entities;
using BootFX.Common.Xml;
using System.ComponentModel;

namespace BootFX.Common.Data.Schema
{
	/// <summary>
	/// Defines a link from child to parent.
	/// </summary>
	public class SqlChildToParentLink : SqlMemberWithColumns
	{
		private bool _generateParentAccessMethods = false;

        private bool _generateDtoField;

        private string _jsonName;

		/// <summary>
		/// Private field to support <c>ParentColumns</c> property.
		/// </summary>
		private SqlColumnCollection _linkFields;

		/// <summary>
		/// Private field to support <see cref="ParentTable"/> property.
		/// </summary>
		private SqlTable _parentTable;
		
		/// <summary>
		/// Private field to support <c>Table</c> property.
		/// </summary>
		private SqlTable _table;

		/// <summary>
		/// Private field to support <c>ChildToParentEntityLink</c> property.
		/// </summary>
		private ChildToParentEntityLink _childToParentEntityLink = null;
		
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nativeName"></param>
		public SqlChildToParentLink(string nativeName, SqlTable parentTable) : base(nativeName)
		{
			if(parentTable == null)
				throw new ArgumentNullException("parentTable");
			
			// set...
			_parentTable = parentTable;
			_linkFields = new SqlColumnCollection(this);
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		internal SqlChildToParentLink(SqlSchema schema, ChildToParentEntityLink entityLink) 
			: base(entityLink.NativeName.Name)
		{
			// mbr - 04-10-2007 - force the schema...
			//			_schema = schema;
			this.SetSchema(schema);

			_childToParentEntityLink = entityLink;
		}

		/// <summary>
		/// Gets the parenttable.
		/// </summary>
		public SqlTable ParentTable
		{
			get
			{		
				// we must lazy load as we rely on the parent
				if(_parentTable == null && ChildToParentEntityLink != null)
				{
					string name = ChildToParentEntityLink.ParentEntityType.NativeName.Name;
					_parentTable = Schema.Tables[name];
					
					// mbr - 04-10-2007 - added assertion...
					if(_parentTable == null)
						throw new InvalidOperationException(string.Format("A table with name '{0}' was not found.", name));
				}

				return _parentTable;
			}
		}

		/// <summary>
		/// Returns true if the link is supported within the given schema.
		/// </summary>
		/// <param name="schema"></param>
		/// <returns></returns>
		/// <remarks>Essentially, this will check that the parent table is referenced in the schema.</remarks>
		internal bool IsSupported(SqlSchema schema)
		{
			if(this.ChildToParentEntityLink != null)
			{
				// check...
				EntityType et = this.ChildToParentEntityLink.ParentEntityType;
				if(et == null)
					throw new InvalidOperationException("et is null.");

				// walk...
				foreach(SqlTable table in schema.Tables)
				{
					if(string.Compare(table.Name, et.NativeName.Name, true, Cultures.System) == 0)
						return true;
				}

				// nope...
				return false;
			}
			else
			{
				// if we don't have a child to parent link, we're ok...
				return true;
			}
		}
		
		/// <summary>
		/// Gets the ChildToParentEntityLink.
		/// </summary>
		private ChildToParentEntityLink ChildToParentEntityLink
		{
			get
			{		
				return _childToParentEntityLink;
			}
		}

		/// <summary>
		/// Gets a collection of SqlColumn objects.
		/// </summary>
		public SqlColumnCollection LinkFields
		{
			get
			{
				// If we have a parent columns but we were loaded from a link
				// we must lazy load as we rely on the parent
				if(_linkFields == null && ChildToParentEntityLink != null)
				{
					_linkFields = new SqlColumnCollection(this);
					foreach(EntityField field in ChildToParentEntityLink.GetLinkFields())
					{
						SqlColumn column = Table.Columns[field.NativeName.Name];
						if(column == null)
							throw new InvalidOperationException(string.Format("Failed to find column with native name '{0}' (field name '{1}')", field.NativeName, field.Name));
			
						// add...
						_linkFields.Add(column);
					}
				}
				return _linkFields;
			}
		}

		/// <summary>
		/// Gets the table.
		/// </summary>
		public SqlTable Table
		{
			get
			{
				// returns the value...
				return _table;
			}
		}

		/// <summary>
		/// Sets the table.
		/// </summary>
		/// <param name="table"></param>
		internal void SetTable(SqlTable table)
		{
			if(table == null)
				throw new ArgumentNullException("table");
			if(this.Table != null)
				throw new InvalidOperationException(string.Format("Link is already assigned to '{0}'.", this.Table.NativeName));
			_table = table;
		}
		
		/// <summary>
		/// Gets or sets the generateparentaccessmethods
		/// </summary>
		public bool GenerateParentAccessMethods
		{
			get
			{
				return _generateParentAccessMethods;
			}
			set
			{
				// check to see if the value has changed...
				if(value != _generateParentAccessMethods)
				{
					// set the value...
					_generateParentAccessMethods = value;
                    this.OnPropertyChanged(new PropertyChangedEventArgs("GenerateParentAccessMethods"));
				}
			}
		}

        public bool GenerateDtoField
        {
            get
            {
                return _generateDtoField;
            }
            set
            {
                // check to see if the value has changed...
                if (value != _generateDtoField)
                {
                    // set the value...
                    _generateDtoField = value;
                    this.OnPropertyChanged(new PropertyChangedEventArgs("GenerateDtoField"));
                }
            }
        }

		protected override void WriteXml(System.Xml.XmlWriter xml, BootFX.Common.Xml.WriteXmlContext context)
		{
			base.WriteXml(xml, context);

			// add...
			xml.WriteElementString("GenerateParentAccessMethods", this.GenerateParentAccessMethods.ToString());
            xml.WriteElementString("GenerateDtoField", this.GenerateDtoField.ToString());
            xml.WriteElementString("JsonName", _jsonName);
        }

		internal override void Merge(System.Xml.XmlElement element, bool createIfNotFound)
		{
			base.Merge (element, createIfNotFound);

			// add...
			this.GenerateParentAccessMethods = XmlHelper.GetElementBoolean(element, "GenerateParentAccessMethods", OnNotFound.ReturnNull);
            this.GenerateDtoField = XmlHelper.GetElementBoolean(element, "GenerateDtoField", OnNotFound.ReturnNull);
            _jsonName = XmlHelper.GetElementString(element, "JsonName", OnNotFound.ReturnNull);
        }

        public string JsonName
        {
            get
            {
                if (string.IsNullOrEmpty(_jsonName))
                    return CodeDomHelper.GetCamelName(this.Name);
                else
                    return _jsonName;
            }
            set
            {
                _jsonName = value;
                this.OnPropertyChanged(new PropertyChangedEventArgs("JsonName"));
            }
        }
	}
}
