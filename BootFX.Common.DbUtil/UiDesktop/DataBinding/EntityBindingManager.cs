// BootFX - Application framework for .NET applications
// 
// File: EntityBindingManager.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Windows.Forms;
using System.Collections;
using BootFX.Common.UI.DataBinding;
using BootFX.Common.Entities;

namespace BootFX.Common.UI.Desktop.DataBinding	
{
	/// <summary>
	///	 Describes a class thet provides Windows Forms data binding extensions specifically for use with entities.
	/// </summary>
	public class EntityBindingManager
	{
		/// <summary>
		/// Private field to support <c>DataSource</c> property.
		/// </summary>
		private object _dataSource;
		
		/// <summary>
		/// Constructor.
		/// </summary>
		public EntityBindingManager(object dataSource)
		{
			_dataSource = dataSource;
		}

		/// <summary>
		/// Gets the datasource.
		/// </summary>
		public object DataSource
		{
			get
			{
				return _dataSource;
			}
		}

		/// <summary>
		/// Creates a binding for the given settings.
		/// </summary>
		/// <param name="dataSource"></param>
		/// <param name="control"></param>
		/// <param name="propertyName"></param>
		/// <param name="expression"></param>
		/// <returns></returns>
		public Binding CreateBinding(Control control, string propertyName, string expression)
		{
            if (propertyName == null)
                throw new ArgumentNullException("propertyName");
            if (propertyName.Length == 0)
                throw new ArgumentException("'propertyName' is zero-length.");
            if (expression == null)
                throw new ArgumentNullException("expression");
            if (expression.Length == 0)
                throw new ArgumentException("'expression' is zero-length.");

			if(DataSource == null)
				throw new ArgumentNullException("DataSource");

			// get...
			EntityType entityType = this.EntityType;
			if(entityType != null)
			{
				// work out what the expression is...
				EntityViewProperty property = EntityViewProperty.GetViewProperty(entityType, expression);
				if(property is EntityFieldViewProperty)
					return new EntityFieldBinding(((EntityFieldViewProperty)property).Field, this.DataSource, propertyName);
				if(property is EntityLinkViewProperty)
					return new EntityLinkBinding(((EntityLinkViewProperty)property).Link, this.DataSource, propertyName);
				if(property is EntityBindViewProperty)
					return new EntityPropertyBinding(((EntityBindViewProperty)property).Property,this.DataSource,propertyName);
				else
					throw new NotSupportedException(string.Format(Cultures.Exceptions, "Cannot handle '{0}'.", property));
			}

			// return a default binding...
			return this.CreateDefaultBinding(propertyName, expression);
		}

		/// <summary>
		/// Creates a default binding.
		/// </summary>
		/// <param name="control"></param>
		/// <param name="propertyName"></param>
		/// <param name="expression"></param>
		/// <returns></returns>
		private Binding CreateDefaultBinding(string propertyName, string expression)
		{
            if (propertyName == null)
                throw new ArgumentNullException("propertyName");
            if (propertyName.Length == 0)
                throw new ArgumentException("'propertyName' is zero-length.");
            if (expression == null)
                throw new ArgumentNullException("expression");
            if (expression.Length == 0)
                throw new ArgumentException("'expression' is zero-length.");
		
			// return...
			return new Binding(propertyName, this.DataSource, expression);
		}

		/// <summary>
		/// Gets the entity type.
		/// </summary>
		protected EntityType EntityType
		{
			get
			{
				if(DataSource == null)
					throw new ArgumentNullException("DataSource");
				if(this.DataSource is IEntityType)
					return ((IEntityType)this.DataSource).EntityType;
				else
					return null;
			}
		}

		/// <summary>
		/// Binds the controls in the given tree.
		/// </summary>
		/// <param name="control"></param>
		public void BindControl(Control control)
		{
			// tag?
			string tag = control.Tag as string;
			if(tag != null && tag.Length > 0)
			{
				// get the property...
				string property = GetPropertyToBind(control);
                if (property == null)
                    throw new InvalidOperationException("'property' is null.");
                if (property.Length == 0)
                    throw new InvalidOperationException("'property' is zero-length.");

				// create...
				control.DataBindings.Clear();
				if(this.DataSource != null)
				{
					// add...
					Binding binding = this.CreateBinding(control, property, tag);
					try
					{
						// add...
						control.DataBindings.Add(binding);

						// bindable?
						if(control is IDesktopBindableControl)
							((IDesktopBindableControl)control).Binding = binding;
					}
					catch(Exception ex)
					{
						throw new InvalidOperationException(string.Format(Cultures.Exceptions, "Failed to add a binding on property '{0}' to control '{1}' (name: {2}) for tag '{3}' on data source '{4}'.\r\nBinding - DataSource:{5}, Control:{6}, PropertyName:{7}, Info.Field:{8}, Info.Member:{9}, Info.Path:{10}", 
							property, control.GetType(), control.Name, tag, this.DataSource, binding.DataSource, binding.Control, binding.PropertyName, 
							binding.BindingMemberInfo.BindingField, binding.BindingMemberInfo.BindingMember, binding.BindingMemberInfo.BindingPath), ex);
					}
				}
			}

			// children...
			foreach(Control child in control.Controls)
				this.BindControl(child);
		}

		/// <summary>
		/// Gets the property to bind on the given control.
		/// </summary>
		/// <param name="control"></param>
		/// <returns></returns>
		public static string GetPropertyToBind(Control control)
		{
			if(control == null)
				throw new ArgumentNullException("control");
			
			// check...
			if(control is TextBoxBase)
				return "Text";
			if(control is ComboBox)
			{
				// if it's a drop down list, return the item, otherwise we want to use the text property...
				if(((ComboBox)control).DropDownStyle == ComboBoxStyle.DropDownList)
					return "SelectedItem";
				else
					return "Text";
			}
			if(control is DateTimePicker)
				return "Value";
			if(control is CheckBox)
				return "Checked";
			if(control is NumericUpDown)
				return "Value";
			if(control is IBindableControl)
				return ((IBindableControl)control).GetDefaultBindProperty();
			else
				throw new InvalidOperationException(string.Format(Cultures.Exceptions, "Cannot handle '{0}'.", control.GetType()));
		}
	}
}
