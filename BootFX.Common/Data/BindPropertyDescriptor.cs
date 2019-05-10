// BootFX - Application framework for .NET applications
// 
// File: BindPropertyDescriptor.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Reflection;
using System.ComponentModel;
using BootFX.Common.Entities;

namespace BootFX.Common.Data
{
	/// <summary>
	///	 Describes a property descriptor that provides generic property binding through reflection.
	/// </summary>
	internal class BindPropertyDescriptor : PropertyDescriptorBase
	{
		/// <summary>
		/// Private field to support <c>Type</c> property.
		/// </summary>
		private Type _type;

		/// <summary>
		/// Private field to support <c>PropertyInfo</c> property.
		/// </summary>
		private PropertyInfo _propertyInfo;
		
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="property"></param>
		public BindPropertyDescriptor(Type type, PropertyInfo propertyInfo, string displayName) : base(propertyInfo.Name, new Attribute[] {})
		{
			if(type == null)
				throw new ArgumentNullException("type");
			if(propertyInfo == null)
				throw new ArgumentNullException("propertyInfo");
			
			_type = type;
			_propertyInfo = propertyInfo;

			// display name...
			if(displayName != null && displayName.Length > 0)
				this.CustomDisplayName = displayName;
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
		/// Gets the type.
		/// </summary>
		public Type Type
		{
			get
			{
				return _type;
			}
		}

		public override bool CanResetValue(object component)
		{
			return false;
		}

		public override Type ComponentType
		{
			get
			{
				if(Type == null)
					throw new ArgumentNullException("Type");
				return this.Type;
			}
		}

		public override object GetValue(object component)
		{
			if(PropertyInfo == null)
				throw new ArgumentNullException("PropertyInfo");
			object entity = EntityView.Unwrap(component);
			return this.PropertyInfo.GetValue(entity, new object[] {});
		}

		public override bool IsReadOnly
		{
			get
			{
				if(PropertyInfo == null)
					throw new ArgumentNullException("PropertyInfo");
				if(this.PropertyInfo.GetSetMethod() == null)
					return true;
				else
					return false;
			}
		}

		public override Type PropertyType
		{
			get
			{
				if(PropertyInfo == null)
					throw new ArgumentNullException("PropertyInfo");
				return this.PropertyInfo.PropertyType;
			}
		}

		public override void ResetValue(object component)
		{
			throw new NotImplementedException("The operation has not been implemented.");
		}

		public override void SetValue(object component, object value)
		{
			if(PropertyInfo == null)
				throw new ArgumentNullException("PropertyInfo");
			this.PropertyInfo.SetValue(component, value, new object[] {});
		}

		public override bool ShouldSerializeValue(object component)
		{
			return false;
		}
	}
}
