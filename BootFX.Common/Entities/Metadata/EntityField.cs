// BootFX - Application framework for .NET applications
// 
// File: EntityField.cs
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
using System.Globalization;
using System.ComponentModel;
using BootFX.Common.Data;
using BootFX.Common.Data.Schema;
using BootFX.Common.Data.Formatters;
using BootFX.Common.Data.Comparers;

namespace BootFX.Common.Entities
{
	/// <summary>
	/// Describes a field on an entity.
	/// </summary>
	/// <remarks>Typically, a field maps to a column on a relational database table.</remarks>
	public class EntityField : EntityMember
	{
		/// <summary>
		/// Private field to support <see cref="ExtendedPropertyDefinition"/> property.
		/// </summary>
		private ExtendedPropertyDefinition _extendedPropertyDefinition;
		
		/// <summary>
		/// Private field to support <see cref="DefaultExpression"/> property.
		/// </summary>
		private SqlDatabaseDefault _defaultExpression;
		
		/// <summary>
		/// Private field to support <c>DefaultFormatString</c> property.
		/// </summary>
		private string _defaultFormatString;
		
		///// <summary>
		///// Private field to support <c>EncryptionKeyName</c> property.
		///// </summary>
		//private string _encryptionKeyName;
		
		/// <summary>
		/// Private field to support <c>EnumerationType</c> property.
		/// </summary>
		private Type _enumerationType;
		
		/// <summary>
		/// Private field to support <c>DBNullEquivalent</c> property.
		/// </summary>
		private object _dbNullEquivalent = DBNull.Value;
		
		/// <summary>
		/// Private field to support <c>Default</c> property.
		/// </summary>
		private CommonDefault _default = CommonDefault.DBNull;
		
		/// <summary>
		/// Private field to support <c>Size</c> property.
		/// </summary>
		private long _size = 0;
		
		/// <summary>
		/// Private field to support <c>Type</c> property.
		/// </summary>
		private Type _type = null;
		
		/// <summary>
		/// Defines the field to use with <c>ExtendedProperties</c> when this field is used to create a <c>DataColumn</c>.
		/// </summary>
		public const string EntityFieldKey = "EntityField";

		/// <summary>
		/// Private field to support <c>DbType</c> property.
		/// </summary>
		private DbType _dbType;
		
		/// <summary>
		/// Private field to support <c>Flags</c> property.
		/// </summary>
		private EntityFieldFlags _flags;
		
		/// <summary>
		/// Constructor.
		/// </summary>
		public EntityField(string name, string nativeName, DbType dbType, EntityFieldFlags flags) : base(name, nativeName)
		{
			// do we need a length?
			long size = 0;
			if((flags & EntityFieldFlags.Large) == 0)
				size = ConversionHelper.GetDefaultDBTypeSize(dbType);
			else
				size = uint.MaxValue;

			// init...
			this.Initialize(dbType, flags, size);
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		public EntityField(string name, string nativeName, DbType dbType, EntityFieldFlags flags, long size) : base(name, nativeName)
		{
			// are we allowed to have a size?
			if(ConversionHelper.DoesDBTypeHaveFixedSize(dbType) == true)
				throw new InvalidOperationException(string.Format(Cultures.Exceptions, "Cannot specify a size for '{0}'.", dbType));

			// set...
			this.Initialize(dbType, flags, size);
		}

		/// <summary>
		/// Initializes the field.
		/// </summary>
		/// <param name="dbType"></param>
		/// <param name="flags"></param>
		/// <param name="size"></param>
		private void Initialize(DbType dbType, EntityFieldFlags flags, long size)
		{
			// large...
			bool isLarge = ((flags & EntityFieldFlags.Large) == EntityFieldFlags.Large);
			bool supportsLarge = ConversionHelper.DoesDBTypeSupportLarge(dbType);
			if(isLarge == true && supportsLarge == false)
				throw new InvalidOperationException(string.Format(Cultures.Exceptions, "Database type '{0}' does not support large values.", dbType));

			// basics...
			_dbType = dbType;
			_flags = flags;
			_size = size;

			// default...
			_default = GetDefaultDefault(dbType, flags);
		}

		/// <summary>
		/// Gets the common default for this field.
		/// </summary>
		/// <remarks>To get a specific default value, use <see cref="GetDefaultValue"></see>.</remarks>
		public CommonDefault Default
		{
			get
			{
				// returns the value...
				return _default;
			}
			set
			{
				if(value != _default)
					_default = value;
			}
		}

		/// <summary>
		/// Gets the physical default value at this moment.
		/// </summary>
		/// <returns></returns>
		public object GetDefaultValue()
		{
			switch(Default)
			{
				case CommonDefault.DBNull:
					return DBNull.Value;

				case CommonDefault.ClrNullEquivalent:
					return ConversionHelper.GetClrLegalDBNullEquivalent(this.DBType);

				case CommonDefault.UtcNow:
					return DateTime.UtcNow;

				case CommonDefault.NewGuid:
					return Guid.NewGuid();

				default:
					throw ExceptionHelper.CreateCannotHandleException(Default);
			}
		}

		/// <summary>
		/// Gets the flags.
		/// </summary>
		public EntityFieldFlags Flags
		{
			get
			{
				return _flags;
			}
		}

		/// <summary>
		/// Gets the friendly name for the field
		/// </summary>
		public string FriendlyName
		{
			get
			{
				return string.Format("{0} ({1})",Name,Type.Name);
			}
		}

		/// <summary>
		/// Gets the nullable.
		/// </summary>
		public bool IsCommon()
		{
			if(this.IsKey() == true || (Flags & EntityFieldFlags.Common) != 0)
				return true;
			else
				return false;
		}

		/// <summary>
		/// Returns true if the field is large.
		/// </summary>
		public bool IsLarge()
		{
			if((Flags & EntityFieldFlags.Large) != 0)
				return true;
			else
				return false;
		}

		/// <summary>
		/// Gets the nullable.
		/// </summary>
		public bool PersistAsCData()
		{
			if((Flags & EntityFieldFlags.PersistAsCData) != 0)
				return true;
			else
				return false;
		}

		/// <summary>
		/// Gets the nullable.
		/// </summary>
		public bool XmlIgnore()
		{
			if((Flags & EntityFieldFlags.XmlIgnore) != 0)
				return true;
			else
				return false;
		}

		/// <summary>
		/// Gets the nullable.
		/// </summary>
		public bool PersistAsBase64()
		{
			if((Flags & EntityFieldFlags.PersistAsBase64) != 0)
				return true;
			else
				return false;
		}

		/// <summary>
		/// Gets the nullable.
		/// </summary>
		public bool IsNullable()
		{
			if((Flags & EntityFieldFlags.Nullable) != 0)
				return true;
			else
				return false;
		}

		// mbr - 04-07-2007 - made public, changed to property.		
		/// <summary>
		/// Gets the nullable.
		/// </summary>
		public bool IsAutoIncrement
		{
			get
			{
				if((Flags & EntityFieldFlags.AutoIncrement) != 0)
					return true;
				else
					return false;
			}
		}

		/// <summary>
		/// Gets the nullable.
		/// </summary>
		public bool IsKey()
		{
			if((Flags & EntityFieldFlags.Key) != 0)
				return true;
			else
				return false;
		}

		/// <summary>
		/// Gets the is extended.
		/// </summary>
		public bool IsExtendedProperty
		{
			get
			{
				// mbr - 12-12-2005 - rejigged...
				//			if((Flags & EntityFieldFlags.ExtendedProperty) != 0)
				//				return true;
				//			else
				//				return false;
				if(this.ExtendedPropertyDefinition == null)
					return false;
				else
					return true;
			}
		}

		/// <summary>
		/// Gets the is lookup.
		/// </summary>
		public bool IsLookupProperty
		{
			get
			{
				// mbr - 12-12-2005 - rejigged...
				//			if((Flags & EntityFieldFlags.LookupProperty) != 0)
				//		return true;
				//		else
				//		return false;
				if(this.ExtendedPropertyDefinition != null && this.ExtendedPropertyDefinition.DataType is LookupExtendedPropertyDataType)
					return true;
				else
					return false;
			}
		}

		/// <summary>
		/// Gets the dbtype.
		/// </summary>
		public DbType DBType
		{
			get
			{
				return _dbType;
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
					_type = ConversionHelper.GetClrTypeForDBType(this.DBType);
				return _type;
			}
		}

		/// <summary>
		/// Creates a data column.
		/// </summary>
		/// <returns></returns>
		public DataColumn CreateDataColumn()
		{
			// create...
			DataColumn column = new DataColumn(this.NativeName.ToString(), ConversionHelper.GetClrTypeForDBType(this.DBType));
			column.ExtendedProperties.Add(EntityFieldKey, this);

			// return...
			return column;
		}

		/// <summary>
		/// Gets or sets the size
		/// </summary>
		public long Size
		{
			get
			{
				return _size;
			}
			set
			{
				// check to see if the value has changed...
				if(value != _size)
				{
					// set the value...
					_size = value;
				}
			}
		}

		/// <summary>
		/// Gets encoding for the field.
		/// </summary>
		public Encoding Encoding
		{
			get
			{
				return Encoding.Unicode;
			}
		}

		/// <summary>
		/// Returns true if the given DB type supports encryption.
		/// </summary>
		/// <param name="dbType"></param>
		/// <returns></returns>
		//private static bool IsEncryptionSupported(DbType dbType)
		//{
		//	switch(dbType)
		//	{
		//		case DbType.String:
		//		case DbType.StringFixedLength:
		//		case DbType.AnsiString:
		//		case DbType.AnsiStringFixedLength:
		//			return true;

		//		default:
		//			return false;
		//	}
		//}

		/// <summary>
		/// Gets the default default.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		internal static CommonDefault GetDefaultDefault(DbType dbType, EntityFieldFlags flags)
		{
			// set the default default...
			if((flags & EntityFieldFlags.Nullable) != 0)
				return CommonDefault.DBNull;
			else
			{
				// if we're date time or a guid use a flashy version...
				if(dbType == DbType.DateTime || dbType == DbType.Date)
					return CommonDefault.UtcNow;
				else if(dbType == DbType.Guid)
					return CommonDefault.NewGuid;
				else
					return CommonDefault.ClrNullEquivalent;
			}
		}

		/// <summary>
		/// Gets a property descriptor for this field.
		/// </summary>
		/// <returns></returns>
		public override PropertyDescriptor GetPropertyDescriptor()
		{
			return new EntityFieldPropertyDescriptor(this);
		}

		/// <summary>
		/// Gets a property descriptor for this field with a format
		/// </summary>
		/// <returns></returns>
		public PropertyDescriptor GetPropertyDescriptor(string format)
		{
			return new EntityFieldPropertyDescriptor(this,format);
		}

		/// <summary>
		/// Gets the comparer for the field.
		/// </summary>
		/// <returns></returns>
		public IComparer GetComparerStrategy(CultureInfo culture)
		{
			return ComparerBase.GetComparer(this.DBType, culture);
		}

		/// <summary>
		/// Gets the comparer for the field.
		/// </summary>
		/// <returns></returns>
		public override IComparer GetComparer(CultureInfo culture)
		{
			IComparer strategy = this.GetComparerStrategy(culture);
			return new EntityMemberComparer(strategy, this);
		}

		/// <summary>
		/// Gets the view property for the field.
		/// </summary>
		/// <returns></returns>
		public override EntityViewProperty GetViewProperty()
		{
			return new EntityFieldViewProperty(this);
		}

		/// <summary>
		/// Gets the value from the object specified
		/// </summary>
		/// <param name="entity"></param>
		/// <returns></returns>
		public override object GetValue(object entity)
		{
			Entity entityValue = (Entity) entity;
			return entityValue.GetValue(this);
		}

        public override void SetValue(object entity, object value, SetValueReason reason)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");
            ((Entity)entity).SetValue(this, value, reason);
        }

		/// <summary>
		/// Gets the converter for this field.
		/// </summary>
		/// <returns></returns>
		public override TypeConverter GetConverter()
		{
			return TypeDescriptor.GetConverter(this.Type);
		}

		/// <summary>
		/// Gets or sets a value to use as a DB null equivalent.
		/// </summary>
		/// <remarks></remarks>
		public object DBNullEquivalent
		{
			get
			{
				return _dbNullEquivalent;
			}
			set
			{
				// check to see if the value has changed...
				if(value != _dbNullEquivalent)
				{
					// mbr - 17-10-2007 - case 898 - box the right type...  this code actually
					// seems a bit worth of thedailywtf.com - and I'm sure it's not the first time...
					switch(DBType)
					{
						case DbType.Byte:
							if(value is short && (short)value == 0)
								value = (byte)0;
							else if(value is int && (int)value == 0)
								value = (byte)0;
							if(value is long && (long)value == 0)
								value = (byte)0;
							break;

						case DbType.Int16:
							if(value is byte && (byte)value == 0)
								value = (short)0;
							else if(value is int && (int)value == 0)
								value = (short)0;
							else if(value is long && (long)value == 0)
								value = (short)0;
							break;

						case DbType.Int32:
							if(value is byte && (byte)value == 0)
								value = (int)0;
							else if(value is short && (short)value == 0)
								value = (int)0;
							else if(value is long && (long)value == 0)
								value = (int)0;
							break;

						case DbType.Int64:
							if(value is byte && (byte)value == 0)
								value = (long)0;
							else if(value is short && (short)value == 0)
								value = (long)0;
							else if(value is int && (int)value == 0)
								value = (long)0;
							break;

							// mbr - 2008-09-02 - added.
						case DbType.Decimal:
							if(value is byte && (byte)value == 0)
								value = 0M;
							else if(value is short && (short)value == 0)
								value = 0M;
							else if(value is int && (int)value == 0)
								value = 0M;
							break;

						default:
							throw new NotSupportedException(string.Format("Cannot handle '{0}' ({1}).", DBType, DBType.GetType()));
					}

					// set the value...
					_dbNullEquivalent = value;
				}
			}
		}

		/// <summary>
		/// Returns true if the object has a  null equivalent.
		/// </summary>
		public bool HasDBNullEquivalent
		{
			get
			{
				if(this.DBNullEquivalent == DBNull.Value)
					return false;
				else
					return true;
			}
		}

		/// <summary>
		/// Gets or sets the enumerationtype
		/// </summary>
		public Type EnumerationType
		{
			get
			{
				return _enumerationType;
			}
			set
			{
				// check to see if the value has changed...
				if(value != _enumerationType)
				{
					// set the value...
					_enumerationType = value;
				}
			}
		}

		/// <summary>
		/// Returns true if the field has an enumeration.
		/// </summary>
		public bool HasEnumerationType
		{
			get
			{
				if(this.EnumerationType == null)
					return false;
				else
					return true;
			}
		}

		/// <summary>
		/// Gets or sets the encryptionkeyname
		/// </summary>
		//public string EncryptionKeyName
		//{
		//	get
		//	{
		//		return _encryptionKeyName;
		//	}
		//	set
		//	{
		//		if(value != null && value.Length == 0)
		//			value = null;

		//		// check...
		//		if(value != null)
		//		{
		//			// has to be a string field...
		//			if(IsEncryptionSupported(this.DBType) == false)
		//				throw new InvalidOperationException(string.Format("Field '{0}' ({1}) is not of a valid type to support encryption.", 
		//					this, this.DBType));
		//		}

		//		// check to see if the value has changed...
		//		if(value != _encryptionKeyName)
		//		{
		//			// set the value...
		//			_encryptionKeyName = value;
		//		}
		//	}
		//}

		/// <summary>
		/// Returns true if the object is encrypted.
		/// </summary>
		//public bool IsEncrypted
		//{
		//	get
		//	{
		//		if(this.EncryptionKeyName != null && this.EncryptionKeyName.Length > 0)
		//			return true;
		//		else
		//			return false;
		//	}
		//}

		/// <summary>
		/// Gets or sets the defaultformatstring
		/// </summary>
		internal string DefaultFormatString
		{
			get
			{
				return _defaultFormatString;
			}
			set
			{
				// check to see if the value has changed...
				if(value != _defaultFormatString)
				{
					// set the value...
					_defaultFormatString = value;
				}
			}
		}

		/// <summary>
		/// Returns true if the field has a default format string.
		/// </summary>
		internal bool HasDefaultFormatString
		{
			get
			{
				if(this.DefaultFormatString != null && this.DefaultFormatString.Length > 0)
					return true;
				else
					return false;
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

		/// <summary>
		/// Gets the extendedpropertydefinition.
		/// </summary>
		public ExtendedPropertyDefinition ExtendedPropertyDefinition
		{
			get
			{
				return _extendedPropertyDefinition;
			}
		}

		internal void SetExtendedPropertyDefinition(ExtendedPropertyDefinition prop)
		{
			if(prop == null)
				throw new ArgumentNullException("prop");
			_extendedPropertyDefinition = prop;
		}

		public bool IsMultiValue
		{
			get
			{
				if(this.IsExtendedProperty)
					return this.ExtendedPropertyDefinition.MultiValue;
				else
					return false;
			}
		}

		internal bool NeedsRounding
		{
			get
			{
				return ConversionHelper.DoesDbTypeNeedRounding(this.DBType);
			}
		}

		internal int Scale
		{
			get
			{
				if(this.NeedsRounding)
					return 5;
				else
					return 0;
			}
		}

		/// <summary>
		/// Gets the link from this field to the parent.
		/// </summary>
		/// <returns></returns>
		public ChildToParentEntityLink GetChildToParentLink()
		{
			// walk...
			foreach(ChildToParentEntityLink link in this.EntityType.Links)
			{
				EntityField[] fields = link.GetLinkFields();
				if(fields == null)
					throw new InvalidOperationException("fields is null.");

				// does it?
				if(fields.Length == 1 && fields[0] == this)
					return link;
			}

			// nope...
			return null;
		}

        public bool IsStringType
        {
            get
            {
                return ConversionHelper.IsStringType(this.DBType);
            }
        }
	}
}
