// BootFX - Application framework for .NET applications
// 
// File: SqlColumn.cs
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
using System.Xml;
using System.Data;
using BootFX.Common.Xml;
using BootFX.Common.Entities;
using BootFX.Common.Dto;

namespace BootFX.Common.Data.Schema
{
	/// <summary>
	/// Describes a column.
	/// </summary>
	public class SqlColumn : SqlMember, ICloneable
	{
		/// <summary>
		/// Private field to support <see cref="DefaultExpression"/> property.
		/// </summary>
		private SqlDatabaseDefault _defaultExpression;
		
		/// <summary>
		/// Raised when the <c>Flags</c> property has changed.
		/// </summary>
		[Browsable(true), Category("Property Changed"), Description("Raised when the Flags property has changed.")]
		public event EventHandler FlagsChanged;

		/// <summary>
		/// Raised when the <c>Modifiers</c> property has changed.
		/// </summary>
		[Browsable(true), Category("Property Changed"), Description("Raised when the Modifiers property has changed.")]
		public event EventHandler ModifiersChanged;

		/// <summary>
		/// Private field to support <c>Modifiers</c> property.
		/// </summary>
		private ColumnModifiers _modifiers = ColumnModifiers.Public;

		/// <summary>
		/// Private field to support <c>EnumerationTypeName</c> property.
		/// </summary>
		private string _enumerationTypeName;
		
		/// <summary>
		/// Private field to support <see cref="Table"/> property.
		/// </summary>
		private SqlTable _table;
		
		/// <summary>
		/// Private field to support <c>CamelName</c> property.
		/// </summary>
		private string _camelName;
		
		/// <summary>
		/// Private field to support <see cref="Type"/> property.
		/// </summary>
		private Type _type = null;
		
		/// <summary>
		/// Private field to support <c>Flags</c> property.
		/// </summary>
		private EntityFieldFlags _flags = EntityFieldFlags.Normal;
		
		/// <summary>
		/// Private field to support <see cref="DbType"/> property.
		/// </summary>
		private DbType _dbType;

		/// <summary>
		/// Private field to support <see cref="Length"/> property.
		/// </summary>
		private long _length;

        private bool _generateDtoField = true;
        private string _jsonName;

		/// <summary>
		/// Raised when the <c>EnumTypeName</c> property has changed.
		/// </summary>
		[Browsable(true), Category("Property Changed"), Description("Raised when the EnumerationTypeName property has changed.")]
		public event EventHandler EnumerationTypeNameChanged;

		/// <summary>
		/// Constructor.
		/// </summary>
		public SqlColumn(string nativeName, DbType dbType, long length, EntityFieldFlags flags) : base(nativeName)
		{
			this.Initialize(dbType, length, flags);
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		public SqlColumn(string nativeName, string name, DbType dbType, long length, EntityFieldFlags flags) 
			: base(nativeName, name)
		{
			this.Initialize(dbType, length, flags);
		}

		/// <summary>
		/// Creates a SQL column from an entity field.
		/// </summary>
		/// <param name="field"></param>
		internal SqlColumn(EntityField field) 
            : base(field.NativeName.Name, field.Name)
		{
			this.Initialize(field.DBType, field.Size, field.Flags);

			// mbr - 01-11-2005 - expression...			
			this.DefaultExpression = field.DefaultExpression;
		}

		/// <summary>
		/// Initializes the object.
		/// </summary>
		/// <param name="dbType"></param>
		/// <param name="isNullable"></param>
		/// <param name="length"></param>
		private void Initialize(DbType dbType, long length, EntityFieldFlags flags)
		{
			_dbType = dbType;
			_length = length;
			_flags = flags;

            // json...
            this.JsonName = this.DefaultJsonName;
		}

		/// <summary>
		/// Gets the length.
		/// </summary>
		public long Length
		{
			get
			{
				return _length;
			}
			set
			{
				_length = value;
			}
		}
		
		/// <summary>
		/// Gets the dbtype.
		/// </summary>
		public DbType DbType
		{
			get
			{
				return _dbType;
			}
			set
			{
				_dbType = value;
			}
		}

		protected override void WriteXml(System.Xml.XmlWriter xml, BootFX.Common.Xml.WriteXmlContext context)
		{
			base.WriteXml(xml, context);

			// others...
			xml.WriteElementString("DbType", this.DbType.ToString());
			xml.WriteElementString("Flags", this.Flags.ToString());
			xml.WriteElementString("Length", this.Length.ToString());
			xml.WriteElementString("EnumerationTypeName", this.EnumerationTypeName);
			xml.WriteElementString("Modifiers", this.Modifiers.ToString());
            xml.WriteElementString("GenerateDtoField", this.GenerateDtoField.ToString());
            xml.WriteElementString("JsonName", this.JsonName);
        }

		/// <summary>
		/// Gets or sets the flags
		/// </summary>
		public EntityFieldFlags Flags
		{
			get
			{
				return _flags;
			}
			set
			{
				// check to see if the value has changed...
				if(value != _flags)
				{
					// set the value...
					_flags = value;
				}
			}
		}

		/// <summary>
		/// Gets whether the field is auto-increment.
		/// </summary>
		internal bool IsAutoIncrement
		{
			get
			{
				return this.GetEntityFieldFlags(EntityFieldFlags.AutoIncrement);
			}
		}

		/// <summary>
		/// Gets whether the field is a key field.
		/// </summary>
		public bool IsKey
		{
			get
			{
				return this.GetEntityFieldFlags(EntityFieldFlags.Key);
			}
		}

		/// <summary>
		/// Gets whether the field is nullable.
		/// </summary>
		public bool IsNullable
		{
			get
			{
				return this.GetEntityFieldFlags(EntityFieldFlags.Nullable);
			}
			set
			{
				this.SetEntityFieldFlags(EntityFieldFlags.Nullable, value);
			}
		}

		/// <summary>
		/// Gets or sets whether the field is nullable.
		/// </summary>
		public bool IsLarge
		{
			get
			{
				return this.GetEntityFieldFlags(EntityFieldFlags.Large);
			}
			set
			{
				this.SetEntityFieldFlags(EntityFieldFlags.Large, value);
			}
		}

		/// <summary>
		/// Returns true if the given flag is set.
		/// </summary>
		/// <param name="flags"></param>
		/// <returns></returns>
		private void SetEntityFieldFlags(EntityFieldFlags flags, bool value)
		{
			_flags = _flags | flags;
			if(!(value))
				_flags = _flags ^ flags;

			// set...
			this.OnFlagsChanged();
		}

		/// <summary>
		/// Returns true if the given flag is set.
		/// </summary>
		/// <param name="flags"></param>
		/// <returns></returns>
		public bool GetEntityFieldFlags(EntityFieldFlags flags)
		{
			if((this.Flags & flags) == flags)
				return true;
			else
				return false;
		}

		/// <summary>
		/// Returns true if the column has a fixed length.
		/// </summary>
		public bool IsFixedLength
		{
			get
			{
				if(this.IsLarge == false)
					return !(ConversionHelper.DoesDBTypeHaveFixedSize(this.DbType));
				else
					return false;
			}
		}

		/// <summary>
		/// Gets the actual length of the column.
		/// </summary>
		/// <remarks>Returns the actual length of the column, or <c>-1</c> if the column length is not defined (e.g. for BLOB fields).</remarks>
		public long ActualLength
		{
			get
			{
				if(this.IsLarge)
					return -1;
				if(this.IsFixedLength)
					return this.Length;
				else
					return ConversionHelper.GetDefaultDBTypeSize(this.DbType);
			}
		}

		/// <summary>
		/// Gets the type.
		/// </summary>
		public Type Type
		{
			get
			{
				if(_type == null)
					_type = ConversionHelper.GetClrTypeForDBType(this.DbType);
				return _type;
			}
		}

		/// <summary>
		/// Gets the camel case version of the name.
		/// </summary>
		public string CamelName
		{
			get
			{
				EnsureCamelNameCreated();
				return _camelName;
			}
		}
		
		/// <summary>
		/// Returns  CamelName.
		/// </summary>
		private bool IsCamelNameCreated()
		{
			if(_camelName == null)
				return false;
			else
				return true;
		}
		
		/// <summary>
		/// Ensures that CamelName has been created.
		/// </summary>
		private void EnsureCamelNameCreated()
		{
			if(IsCamelNameCreated() == false)
				_camelName = CreateCamelName();
		}
		
		/// <summary>
		/// Creates an instance of CamelName.
		/// </summary>
		/// <remarks>This does not assign the instance to the _camelName field</remarks>
		private string CreateCamelName()
		{
			return CodeDomHelper.GetCamelName(this.Name);
		}

		protected override void OnNameChanged(EventArgs e)
		{
			base.OnNameChanged (e);
			_camelName = null;
		}

		protected virtual void OnEnumerationTypeNameChanged(EventArgs e)
		{
			if(EnumerationTypeNameChanged != null)
				EnumerationTypeNameChanged(this,e);
		}

		/// <summary>
		/// Gets the table.
		/// </summary>
		public SqlTable Table
		{
			get
			{
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
				throw new InvalidOperationException(string.Format("Column '{0}' is already assigned to table '{1}'.", this.NativeName, table.NativeName));
			_table = table;
		}

		/// <summary>
		/// Gets or sets the type name of an enumeration that overloads this member.
		/// </summary>
		/// <remarks>This should ideally be the full type name, e.g. <c>Foo.Bar.Widget</c>.</remarks>
		public string EnumerationTypeName
		{
			get
			{
				return _enumerationTypeName;
			}
			set
			{
				// check to see if the value has changed...
				if(value != null && value.Length == 0)
					value = null;
				if(value != _enumerationTypeName)
				{
					// set the value...
					_enumerationTypeName = value;
					OnEnumerationTypeNameChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Gets or sets the modifiers for this member
		/// </summary>
		/// <remarks>This should ideally be the full type name, e.g. <c>Foo.Bar.Widget</c>.</remarks>
		public ColumnModifiers Modifiers
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

		/// <summary>
		/// Raises the <c>FlagsChanged</c> event.
		/// </summary>
		private void OnFlagsChanged()
		{
			OnFlagsChanged(EventArgs.Empty);
		}
		
		/// <summary>
		/// Raises the <c>FlagsChanged</c> event.
		/// </summary>
		protected virtual void OnFlagsChanged(EventArgs e)
		{
			if(FlagsChanged != null)
				FlagsChanged(this, e);
		}

		/// <summary>
		/// Returns true if the column has an enumeration type name.
		/// </summary>
		public bool HasEnumerationTypeName
		{
			get
			{
				if(this.EnumerationTypeName == null)
					return false;
				else
					return true;
			}
		}

		internal override void Merge(System.Xml.XmlElement element, bool createIfNotFound)
		{
			base.Merge (element, createIfNotFound);

			// load the values...
			this.EnumerationTypeName = XmlHelper.GetElementString(element, "EnumerationTypeName", OnNotFound.ReturnNull);
			_length = XmlHelper.GetElementInt64(element, "Length", OnNotFound.ReturnNull);
			_dbType = (DbType)XmlHelper.GetElementEnumerationValue(element, "DbType", typeof(DbType), OnNotFound.ReturnNull);

			// we don't merge in all field values - basically just those that we can set.  (e.g. if the DB changes from Nullable to NotNullable, 
			// we don't want to replace that flag.)
			// mbr - 13-11-2005 - added...
			EntityFieldFlags loadedFlags = (EntityFieldFlags)XmlHelper.GetElementEnumerationValue(element, "Flags", typeof(EntityFieldFlags), OnNotFound.ReturnNull);

			// mbr - 21-09-2007 - do we actually have modifiers?
			string modifiersAsString = XmlHelper.GetElementString(element, "Modifiers", OnNotFound.ReturnNull);
			if(modifiersAsString != null && modifiersAsString.Length > 0)
				this.Modifiers = (ColumnModifiers)Enum.Parse(typeof(ColumnModifiers), modifiersAsString, true);

            //  dto...
            var jsonName = element.GetElementString("JsonName", OnNotFound.ReturnNull);
            if (!(string.IsNullOrEmpty(jsonName)))
                this.JsonName = jsonName;
            else
                this.JsonName = DefaultJsonName;

            var asString = element.GetElementString("GenerateDtoField", OnNotFound.ReturnNull);
            if (!(string.IsNullOrEmpty(asString)))
                this.GenerateDtoField = ConversionHelper.ToBoolean(asString);
            else
            {
                if (!(this.IsLarge))
                    this.GenerateDtoField = this.Table.GenerateDto;
                else
                    this.GenerateDtoField = false;
            }
		}

		private void MergeFlags(EntityFieldFlags loadedFlags, EntityFieldFlags toTest)
		{
			_flags = _flags | toTest;
			if((loadedFlags & toTest) == 0)
				_flags = _flags ^ toTest;
		}

		/// <summary>
		/// Gets the column for the field.
		/// </summary>
		/// <param name="field"></param>
		/// <returns></returns>
		internal static SqlColumn GetColumn(EntityField field)
		{
			if(field == null)
				throw new ArgumentNullException("field");
			
			return new SqlColumn(field.NativeName.Name, field.Name, field.DBType, field.Size, field.Flags);
		}

		public object Clone()
		{
			SqlColumn column = new SqlColumn(this.NativeName,this.Name,this.DbType,this.Length,this.Flags);
			column.DefaultExpression = this.DefaultExpression;
			return column;
		}

		/// <summary>
		/// Gets whether the Column has a default expression.
		/// </summary>
		public bool HasDefaultExpression
		{
			get
			{
				return DefaultExpression != null;
			}
		}

		/// <summary>
		/// Gets the defaultexpression.
		/// </summary>
		public SqlDatabaseDefault DefaultExpression
		{
			get
			{
				return _defaultExpression;
			}
			set
			{
				_defaultExpression = value;
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
                _generateDtoField = value;
                this.OnPropertyChanged(new PropertyChangedEventArgs("GenerateDtoField"));
            }
        }

        public string JsonName
        {
            get
            {
                if (this.IsKey)
                    return DtoType.IdJsonKey;
                else
                    return _jsonName;
            }
            set
            {
                _jsonName = value;
                this.OnPropertyChanged(new PropertyChangedEventArgs("JsonName"));
            }
        }

        private string DefaultJsonName
        {
            get
            {
                if (this.IsKey)
                    return "id";
                else
                    return CodeDomHelper.GetCamelName(this.Name);
            }
        }
	}
}
