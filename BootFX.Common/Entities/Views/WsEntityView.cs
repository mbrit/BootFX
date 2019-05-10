// BootFX - Application framework for .NET applications
// 
// File: WsEntityView.cs
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

namespace BootFX.Common.Entities
{
	/// <summary>
	/// Summary description for WsEntityView.
	/// </summary>
	public class WsEntityView : ICustomTypeDescriptor
	{
		private object _wsEntity = null;

		/// <summary>
		/// Constructor for an web service entity view
		/// </summary>
		/// <param name="wsEntity"></param>
		public WsEntityView(object wsEntity)
		{
			_wsEntity = wsEntity;
		}

		#region ICustomTypeDescriptor Members

		public TypeConverter GetConverter()
		{
			// TODO:  Add WsEntityView.GetConverter implementation
			return null;
		}

		public EventDescriptorCollection GetEvents(Attribute[] attributes)
		{
			// TODO:  Add WsEntityView.GetEvents implementation
			return null;
		}

		EventDescriptorCollection System.ComponentModel.ICustomTypeDescriptor.GetEvents()
		{
			// TODO:  Add WsEntityView.System.ComponentModel.ICustomTypeDescriptor.GetEvents implementation
			return null;
		}

		public string GetComponentName()
		{
			// TODO:  Add WsEntityView.GetComponentName implementation
			return null;
		}

		public object GetPropertyOwner(PropertyDescriptor pd)
		{
			// TODO:  Add WsEntityView.GetPropertyOwner implementation
			return null;
		}

		public AttributeCollection GetAttributes()
		{
			// TODO:  Add WsEntityView.GetAttributes implementation
			return null;
		}

		public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
		{
			PropertyDescriptorCollection properties = new PropertyDescriptorCollection(null);
			FieldInfo[] fields = _wsEntity.GetType().GetFields();

			foreach(FieldInfo field in fields)
				properties.Add(new WsEntityField(field));

			return properties;
		}

		PropertyDescriptorCollection System.ComponentModel.ICustomTypeDescriptor.GetProperties()
		{
			// TODO:  Add WsEntityView.System.ComponentModel.ICustomTypeDescriptor.GetProperties implementation
			return GetProperties(null);
		}

		public object GetEditor(Type editorBaseType)
		{
			// TODO:  Add WsEntityView.GetEditor implementation
			return null;
		}

		public PropertyDescriptor GetDefaultProperty()
		{
			// TODO:  Add WsEntityView.GetDefaultProperty implementation
			return null;
		}

		public EventDescriptor GetDefaultEvent()
		{
			// TODO:  Add WsEntityView.GetDefaultEvent implementation
			return null;
		}

		public string GetClassName()
		{
			// TODO:  Add WsEntityView.GetClassName implementation
			return null;
		}

		#endregion

		/// <summary>
		/// Gets the entity used for binding
		/// </summary>
		public object WsEntity
		{
			get
			{
				return _wsEntity;
			}
		}

		/// <summary>
		/// Gets the views for all the objects.
		/// This view will make every public field on the entity visible as a property
		/// when databinding.
		/// </summary>
		/// <param name="wsEntities"></param>
		/// <returns></returns>
		public static WsEntityView[] GetViews(object[] wsEntities)
		{
			ArrayList views = new ArrayList();
			foreach(object wsEntity in wsEntities)
				views.Add(new WsEntityView(wsEntity));

			return (WsEntityView[]) views.ToArray(typeof (WsEntityView));
		}
	}
}
