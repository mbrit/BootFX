// BootFX - Application framework for .NET applications
// 
// File: EntityFieldPropertyDescriptor.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.ComponentModel;
using BootFX.Common.Management;

namespace BootFX.Common.Entities
{
	/// <summary>
	/// Describes a property descriptor that access data via an <see cref="EntityField"></see>.
	/// </summary>
	public class EntityFieldPropertyDescriptor : PropertyDescriptorBase, IStringEquivalentPropertyDescriptor
	{
		/// <summary>
		/// Private field to support <c>Field</c> property.
		/// </summary>
		private EntityField _field;

		/// <summary>
		/// Private field to support <c>Format</c> property.
		/// </summary>
		private string _format = string.Empty;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="field"></param>
		public EntityFieldPropertyDescriptor(EntityField field) : base(field.Name, new Attribute[] {})
		{
			if(field == null)
				throw new ArgumentNullException("field");
			
			// set...
			_field = field;
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="field"></param>
		public EntityFieldPropertyDescriptor(EntityField field, string format) : this(field)
		{
			_format = format;
		}

		/// <summary>
		/// Gets the field.
		/// </summary>
		public EntityField Field
		{
			get
			{
				return _field;
			}
		}

		/// <summary>
		/// Gets the format.
		/// </summary>
		public string Format
		{
			get
			{
				return _format;
			}
		}

		public override bool CanResetValue(object component)
		{
			return false;
		}

		/// <summary>
		/// Gets the component type for the descriptor.
		/// </summary>
		public override Type ComponentType
		{
			get
			{
				if(EntityType == null)
					throw new ArgumentNullException("EntityType");
				
				if(Format != string.Empty)
					return typeof(string);

				return this.EntityType.Type;
			}
		}

		/// <summary>
		/// Gets the value for the entity.
		/// </summary>
		/// <param name="component"></param>
		/// <returns></returns>
		public override object GetValue(object component)
		{
			if(component == null)
				throw new ArgumentNullException("component");

			// check...
			if(EntityType == null)
				throw new ArgumentNullException("EntityType");

			// do we have an entity view?
			object result = null;
			if(!(component is EntityView))
			{
				this.EntityType.AssertIsOfType(component);

				// anything?
				if(Field == null)
					throw new ArgumentNullException("Field");
				if(Storage == null)
					throw new ArgumentNullException("Storage");
			
				// get the value...
				result = Storage.GetValue(component, this.Field);
			}
			else
				result = ((EntityView)component).GetValue(this.Field);

			if(Format != string.Empty && result is IFormattable)
				return ((IFormattable)result).ToString(Format,null);

			// return...
			return result;
		}

		/// <summary>
		/// Returns true if the field is read-only.
		/// </summary>
		public override bool IsReadOnly
		{
			get
			{
				if(Field == null)
					throw new ArgumentNullException("Field");
				return Field.IsKey();
			}
		}

		/// <summary>
		/// Gets the type of the property.
		/// </summary>
		public override Type PropertyType
		{
			get
			{
				if(Field == null)
					throw new ArgumentNullException("Field");
				return this.Field.Type;
			}
		}

		/// <summary>
		/// Sets the value for the entity.
		/// </summary>
		/// <param name="component"></param>
		/// <param name="value"></param>
		public override void SetValue(object component, object value)
		{
			if(component == null)
				throw new ArgumentNullException("component");

			// anything?
			if(Field == null)
				throw new ArgumentNullException("Field");

			// do we have a view?
			if(!(component is EntityView))
			{
				if(Storage == null)
					throw new ArgumentNullException("Storage");
			
				// get the value...
				Storage.SetValue(component, this.Field, value, SetValueReason.UserSet);
			}
			else
				((EntityView)component).SetValue(this.Field, value);
		}

		public override bool ShouldSerializeValue(object component)
		{
			return false;
		}

		public override void ResetValue(object component)
		{
			
		}

		/// <summary>
		/// Gets the entitytype.
		/// </summary>
		private EntityType EntityType
		{
			get
			{
				if(Field == null)
					throw new ArgumentNullException("Field");
				return this.Field.EntityType;
			}
		}

		/// <summary>
		/// Gets the storage for the entity.
		/// </summary>
		private IEntityStorage Storage
		{
			get
			{
				if(EntityType == null)
					throw new ArgumentNullException("EntityType");
				return this.EntityType.Storage;
			}
		}

		public bool HasStringEquivalent
		{
			get
			{
				if(this.PropertyType == typeof(string))
					return false;
				else
					return true;
			}
		}

		public PropertyDescriptor GetStringEquivalentDescriptor(string defaultFormatString)
		{
			return new StringFormatterPropertyDescriptor(this, defaultFormatString);
		}
	}
}
