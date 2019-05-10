// BootFX - Application framework for .NET applications
// 
// File: EntityProperty.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Collections;
using System.ComponentModel;
using System.Reflection;
using System.Globalization;
using BootFX.Common.Data;
using BootFX.Common.Data.Comparers;
using BootFX.Common.Entities.Attributes;

namespace BootFX.Common.Entities
{
	/// <summary>
	/// Defines a field on a property.
	/// </summary>
	public class EntityProperty : EntityMember
	{
		/// <summary>
		/// Private field to support <see cref="PropertyInfo"/> property.
		/// </summary>
		private PropertyInfo _propertyInfo;
		
		/// <summary>
		/// Private field to support <see cref="Type"/> property.
		/// </summary>
		private Type _type;
		
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="name"></param>
		internal EntityProperty(Type type, PropertyInfo propertyInfo) : base(propertyInfo.Name, propertyInfo.Name)
		{
			if(type == null)
				throw new ArgumentNullException("type");
			if(propertyInfo == null)
				throw new ArgumentNullException("propertyInfo");

			_type = type;
			_propertyInfo = propertyInfo;
		}

		/// <summary>
		/// Gets the type.
		/// </summary>
		public Type Type
		{
			get
			{
				return _type;
			}
		}

		/// <summary>
		/// Gets the propertyinfo.
		/// </summary>
		public PropertyInfo PropertyInfo
		{
			get
			{
				return _propertyInfo;
			}
		}

		/// <summary>
		/// Returns true if the property is read-only.
		/// </summary>
		public bool IsReadOnly
		{
			get
			{
				if(PropertyInfo == null)
					throw new InvalidOperationException("PropertyInfo is null.");
				return !(this.PropertyInfo.CanWrite);
			}
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
		/// Gets a property descriptor that can bind to the property.
		/// </summary>
		/// <returns></returns>
		public override System.ComponentModel.PropertyDescriptor GetPropertyDescriptor()
		{
			// create...
			return new BindPropertyDescriptor(this.Type, this.PropertyInfo, null);
		}

		public System.Data.DbType DBType
		{
			get
			{
				return ConversionHelper.GetDBTypeForClrType(this.Type);
			}
		}

		public long Size
		{
			get
			{
				return -1;
			}
		}

		public bool IsLookupProperty
		{
			get
			{
				return false;
			}
		}

		public bool IsMultiValue
		{
			get
			{
				return false;
			}
		}

		public bool IsLarge
		{
			get
			{
				return false;
			}
		}

		public override object GetValue(object entity)
		{
			if(PropertyInfo == null)
				throw new InvalidOperationException("PropertyInfo is null.");
			return this.PropertyInfo.GetValue(entity, null);
		}

        public override void SetValue(object entity, object value, BootFX.Common.Entities.SetValueReason reason)
		{
			if(PropertyInfo == null)
				throw new InvalidOperationException("PropertyInfo is null.");
			this.PropertyInfo.SetValue(entity, value, null);
		}

		public bool IsRequired
		{
			get
			{
				return false;
			}
		}

        /// <summary>
        /// Gets the comparer for the field.
        /// </summary>
        /// <returns></returns>
        public IComparer GetComparerStrategy(CultureInfo culture)
        {
            // mbr - 2011-02-17 - this is wrong... it should use the type of the property...
            //return ComparerBase.GetComparer(this.DBType, culture);
            if (PropertyInfo == null)
                throw new InvalidOperationException("'PropertyInfo' is null.");
            return ComparerBase.GetComparer(this.PropertyInfo.PropertyType, culture);
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
    }
}
