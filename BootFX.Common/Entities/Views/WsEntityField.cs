// BootFX - Application framework for .NET applications
// 
// File: WsEntityField.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.ComponentModel;
using System.Reflection;

namespace BootFX.Common.Entities
{
	/// <summary>
	/// Summary description for WsEntityField.
	/// </summary>
	public class WsEntityField : PropertyDescriptor
	{
		/// <summary>
		/// Private field to support the property descriptor
		/// </summary>
		private FieldInfo _field = null;

		public WsEntityField(FieldInfo field): base(field.Name,null)
		{
			_field = field;
		}

		/// <summary>
		/// Gets the value of the field from the component
		/// </summary>
		/// <param name="component">WsEntityView</param>
		/// <returns></returns>
		public override object GetValue(object component)
		{
			return _field.GetValue(((WsEntityView)component).WsEntity);
		}

		/// <summary>
		/// Sets a value on the field for the component
		/// </summary>
		/// <param name="component"></param>
		/// <param name="value"></param>
		public override void SetValue(object component, object value)
		{
			_field.SetValue(((WsEntityView)component).WsEntity,value);
		}

		/// <summary>
		/// Resets the value. Unsupported
		/// </summary>
		/// <param name="component"></param>
		public override void ResetValue(object component)
		{
			return;
		}

		/// <summary>
		/// Is readonly
		/// </summary>
		public override bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// Gets the property type of the field
		/// </summary>
		public override Type PropertyType
		{
			get
			{
				return _field.FieldType;
			}
		}

		/// <summary>
		/// Gets whether the field can be reset
		/// </summary>
		/// <param name="component"></param>
		/// <returns></returns>
		public override bool CanResetValue(object component)
		{
			return false;
		}

		/// <summary>
		/// Returns whether the value can be serialized. false
		/// </summary>
		/// <param name="component"></param>
		/// <returns></returns>
		public override bool ShouldSerializeValue(object component)
		{
			return false;
		}

		/// <summary>
		/// Returns the component type of the field
		/// </summary>
		public override Type ComponentType
		{
			get
			{
				return _field.DeclaringType;
			}
		}
	}
}
