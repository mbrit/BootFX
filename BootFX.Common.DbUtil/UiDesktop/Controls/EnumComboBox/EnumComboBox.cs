// BootFX - Application framework for .NET applications
// 
// File: EnumComboBox.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Windows.Forms;
using System.Reflection;
using System.ComponentModel;
using BootFX.Common.UI.DataBinding;

namespace BootFX.Common.UI.Desktop
{
	/// <summary>
	/// Describes a combo box that can hold an enumeration.
	/// </summary>
	[DefaultEvent("ValueChanged")]
	public class EnumComboBox : UserControl, IBindableControl
	{
		/// <summary>
		/// Private field to support <c>InitializeCount</c> property.
		/// </summary>
		private int _initializeCount;
		
		/// <summary>
		/// Raised when the <c>Value</c> property has changed.
		/// </summary>
		[Browsable(true), Category("Property Changed"), Description("Raised when the Value property has changed.")]
		public event EventHandler ValueChanged;
		
		/// <summary>
		/// Private field to support <c>InnerComboBox</c> property.
		/// </summary>
		private ComboBox _innerComboBox = new ComboBox();
		
		/// <summary>
		/// Private field to support <c>EnumType</c> property.
		/// </summary>
		private Type _enumType;
		
		/// <summary>
		/// Constructor.
		/// </summary>
		public EnumComboBox()
		{
			this.InnerComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
			this.InnerComboBox.Dock = DockStyle.Fill;
			this.InnerComboBox.SelectedIndexChanged += new EventHandler(InnerComboBox_SelectedIndexChanged);
			this.Controls.Add(this.InnerComboBox);

			// set...
			this.Height = this.InnerComboBox.Height;
		}

		protected override void Dispose(bool disposing)
		{
			// clear...
			if(_innerComboBox != null)
			{
				_innerComboBox.Dispose();
				this.Controls.Clear();
				_innerComboBox = null;
			}

			base.Dispose (disposing);
		}

		/// <summary>
		/// Gets the innercombobox.
		/// </summary>
		private ComboBox InnerComboBox
		{
			get
			{
				// returns the value...
				return _innerComboBox;
			}
		}

		/// <summary>
		/// Gets or sets the enumtype
		/// </summary>
		public Type EnumType
		{
			get
			{
				return _enumType;
			}
			set
			{
				// check...
				if(value != null && value.IsEnum == false)
					throw new InvalidOperationException(string.Format("Type '{0}' is not an enumeration type.", value));

				// check to see if the value has changed...
				if(value != _enumType)
				{
					// set the value...
					_enumType = value;
					this.RefreshView();
				}
			}
		}

		/// <summary>
		/// Refreshes the view.
		/// </summary>
		private void RefreshView()
		{
			if(this.InnerComboBox == null)
				return;

			this._initializeCount++;
			try
			{
				// reset...
				this.InnerComboBox.Items.Clear();
				if(this.EnumType == null)
					return;

				// get...
				string[] names = Enum.GetNames(this.EnumType);
				foreach(string name in names)
				{
					// find a field with that name...
					FieldInfo field = this.EnumType.GetField(name, BindingFlags.IgnoreCase | BindingFlags.Static | BindingFlags.Public);
					if(field == null)
						throw new InvalidOperationException("field is null.");

					// get...
					object value = field.GetValue(null);
					this.InnerComboBox.Items.Add(new EnumComboBoxListItem(value));
				}

				// select...
				if(this.InnerComboBox.Items.Count > 0)
					this.InnerComboBox.SelectedIndex = 0;
			}
			finally
			{
				this._initializeCount--;
			}
		}

		public string GetDefaultBindProperty()
		{
			return "Value";
		}

		/// <summary>
		/// Gets or sets the value
		/// </summary>
		public object Value
		{
			get
			{
				if(this.InnerComboBox.SelectedItem != null)
					return ((EnumComboBoxListItem)this.InnerComboBox.SelectedItem).Value;
				else
					return 0;
			}
			set
			{
				// check to see if the value has changed...
				this.InnerComboBox.SelectedIndex = this.IndexOf(value);
			}
		}

		/// <summary>
		/// Raises the <c>ValueChanged</c> event.
		/// </summary>
		private void OnValueChanged()
		{
			OnValueChanged(EventArgs.Empty);
		}
		
		/// <summary>
		/// Raises the <c>ValueChanged</c> event.
		/// </summary>
		protected virtual void OnValueChanged(EventArgs e)
		{
			if(ValueChanged != null)
				ValueChanged(this, e);
		}

		/// <summary>
		/// Finds the index of the item with the given value.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		private int IndexOf(object value)
		{
			// walk...
			for(int index = 0; index < this.InnerComboBox.Items.Count; index++)
			{
				EnumComboBoxListItem item = (EnumComboBoxListItem)this.InnerComboBox.Items[index];
				if((int)item.Value == (int)value)
					return index;
			}

			// nope...
			return -1;
		}

		/// <summary>
		/// Gets the initializecount.
		/// </summary>
		private int InitializeCount
		{
			get
			{
				// returns the value...
				return _initializeCount;
			}
		}

		private void InnerComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			if(this.InitializeCount == 0)
				this.OnValueChanged();
		}
	}
}
