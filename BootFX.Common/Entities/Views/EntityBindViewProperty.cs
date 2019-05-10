// BootFX - Application framework for .NET applications
// 
// File: EntityBindViewProperty.cs
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
using BootFX.Common.Data;

namespace BootFX.Common.Entities
{
	/// <summary>
	/// Provides access to a <see cref="BindPropertyDescriptor"></see> instance.
	/// </summary>
	public class EntityBindViewProperty : EntityViewProperty
	{
		/// <summary>
		/// Private field to support <c>Property</c> property.
		/// </summary>
		private PropertyInfo _propertyInfo;
		
		/// <summary>
		/// Private field to support <c>Property</c> property.
		/// </summary>
		private EntityProperty _property;

		/// <summary>
		/// Constructor.
		/// </summary>
		public EntityBindViewProperty(EntityType entityType, PropertyInfo propertyInfo) : base(entityType)
		{
			if(propertyInfo == null)
				throw new ArgumentNullException("propertyInfo");
			
			_propertyInfo = propertyInfo;
			_property = new EntityProperty(entityType.Type,propertyInfo);
		}

		/// <summary>
		/// Gets the property.
		/// </summary>
		public PropertyInfo PropertyInfo
		{
			get
			{
				return _propertyInfo;
			}
		}

		/// <summary>
		/// Gets the property.
		/// </summary>
		public EntityProperty Property
		{
			get
			{
				return _property;
			}
		}

		/// <summary>
		/// Gets the property descriptor for this property.
		/// </summary>
		/// <returns></returns>
		public override PropertyDescriptor GetPropertyDescriptor()
		{
			if(EntityType == null)
				throw new ArgumentNullException("EntityType");
			if(PropertyInfo == null)
				throw new ArgumentNullException("PropertyInfo");
			return new BindPropertyDescriptor(this.EntityType.Type, this.PropertyInfo, this.ResolvedDisplayName);
		}

		public override string DisplayName
		{
			get
			{
				if(this.PropertyInfo != null)
					return this.PropertyInfo.Name;
				else
					return "(No property)";
			}
		}
	}
}
