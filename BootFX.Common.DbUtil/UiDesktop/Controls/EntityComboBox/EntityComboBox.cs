// BootFX - Application framework for .NET applications
// 
// File: EntityComboBox.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using BootFX.Common.UI.DataBinding;
using BootFX.Common.UI.Desktop.DataBinding;
using BootFX.Common.Data;
using BootFX.Common.Entities;
using ComponentModel_DesignerSerializationVisibility = System.ComponentModel.DesignerSerializationVisibility;

namespace BootFX.Common.UI.Desktop
{
	/// <summary>
	/// Summary description for EntityComboBox.
	/// </summary>
	[DefaultEvent("SelectedEntityChanged")]
	public class EntityComboBox : UserControl, IEntityType, IDesktopBindableControl
	{
		/// <summary>
		/// Private field to support <c>Binding</c> property.
		/// </summary>
		private Binding _binding;
		
		/// <summary>
		/// Private field to support <c>InitializeCount</c> property.
		/// </summary>
		private int _initializeCount;
		
		/// <summary>
		/// Raised when the <c>AllowNullSelection</c> property has changed.
		/// </summary>
		[Browsable(true), Category("Property Changed"), Description("Raised when the AllowNullSelection property has changed.")]
		public event EventHandler AllowNullSelectionChanged;
		
		/// <summary>
		/// Raised when the <c>SelectedEntity</c> property has changed.
		/// </summary>
		[Browsable(true), Category("Property Changed"), Description("Raised when the SelectedEntity property has changed.")]
		public event EventHandler SelectedEntityChanged;
		
		/// <summary>
		/// Defines the test for a null selection.
		/// </summary>
		public const string NullSelectionText = "(None)";

		/// <summary>
		/// Private field to support <c>AllowNullSelection</c> property.
		/// </summary>
		private bool _allowNullSelection;
		
		/// <summary>
		/// Private field to support <c>InnerComboBox</c> property.
		/// </summary>
		private ComboBox _innerComboBox;
		
		/// <summary>
		/// Private field to support <c>DataSource</c> property.
		/// </summary>
		private IList _dataSource;
		
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public EntityComboBox()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			// set...
			_innerComboBox = new ComboBox();
			this.InnerComboBox.Dock = DockStyle.Fill;
			this.InnerComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
			this.InnerComboBox.SelectedIndexChanged += new EventHandler(InnerComboBox_SelectedIndexChanged);
			this.Controls.Add(this.InnerComboBox);
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
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.Size = new System.Drawing.Size(300,300);
			this.Text = "EntityComboBox";
		}
		#endregion

		/// <summary>
		/// Gets or sets the datasource
		/// </summary>
		public IList DataSource
		{
			get
			{
				return _dataSource;
			}
			set
			{
				// check to see if the value has changed...
				if(value != _dataSource)
				{
					// set the value...
					_dataSource = value;

					// refresh view...
					RefreshView();
				}
			}
		}

		/// <summary>
		/// Refreshes the view.
		/// </summary>
		private void RefreshView()
		{
			this._initializeCount++;
			try
			{
				this.InnerComboBox.Items.Clear();

				// none?
				if(this.AllowNullSelection)
					this.InnerComboBox.Items.Add(NullSelectionText);

				// return...
				if(this.DataSource == null)
				{
					if(this.InnerComboBox.Items.Count > 0)
						this.SelectedIndex = 0;
					return;
				}

				// create items...
				foreach(object entity in this.DataSource)
					this.InnerComboBox.Items.Add(new EntityListItem(entity));

				// select the item...
				int index = this.IndexOf(this.SelectedEntity, OnNotFound.ReturnNull);
				this.SelectedIndex = index;
			}
			finally
			{
				_initializeCount--;
			}
		}

		/// <summary>
		/// Gets the entity type.
		/// </summary>
		public EntityType EntityType
		{
			get
			{
				IEntityType entityType = this.DataSource as IEntityType;
				if(entityType != null)
					return entityType.EntityType;
				else
					return null;
			}
		}

		/// <summary>
		/// Sets the datasource from the given entity type.
		/// </summary>
		/// <param name="entityType"></param>
		public void SetDataSourceFromType(Type entityType)
		{
			if(entityType == null)
				throw new ArgumentNullException("entityType");
			
			// defer...
			this.SetDataSourceFromType(EntityType.GetEntityType(entityType, OnNotFound.ThrowException));
		}

		/// <summary>
		/// Sets the datasource from the given entity type.
		/// </summary>
		/// <param name="entityType"></param>
		public void SetDataSourceFromType(EntityType entityType)
		{
			if(entityType == null)
				throw new ArgumentNullException("entityType");
			
			// set...
			IEntityPersistence persistence = entityType.Persistence;
			if(persistence == null)
				throw new InvalidOperationException("persistence is null.");
			this.DataSource = persistence.GetAll();
		}

		string IBindableControl.GetDefaultBindProperty()
		{
			return "SelectedEntity";
		}

		/// <summary>
		/// Gets or sets the selectedentity
		/// </summary>
		public object SelectedEntity
		{
			get
			{
				object selectedItem = this.InnerComboBox.SelectedItem;
				if(selectedItem == null || selectedItem is string)
					return null;
				if(selectedItem is EntityListItem)
					return ((EntityListItem)selectedItem).Entity;
				else
					throw new NotSupportedException(string.Format("Cannot handle '{0}'.", selectedItem.GetType()));
			}
			set
			{
				// db null?
				if(value is DBNull)
					value = null;

				// check to see if the value has changed...
				if(value != this.SelectedEntity)
				{
					// set the value...
					int index = this.IndexOf(value, OnNotFound.ReturnNull);
					if(index == -1)
					{
						// create a new item... why not?  perhaps create a property to prevent this...
						if(this.DataSource != null)
						{
							if(EntityType == null)
								throw new InvalidOperationException("EntityType is null.");
							this.EntityType.AssertIsOfType(value);
						}

						// create...
						EntityListItem item = new EntityListItem(value);
						index = this.InnerComboBox.Items.Add(item);

						// set...
						this.SelectedIndex = index;
					}
				}
			}
		}

		/// <summary>
		/// Gets or sets the selected index.
		/// </summary>
		public int SelectedIndex
		{
			get
			{
				return this.InnerComboBox.SelectedIndex;
			}
			set
			{
				this.InnerComboBox.SelectedIndex = value;
			}
		}

		/// <summary>
		/// Raises the <c>SelectedEntityChanged</c> event.
		/// </summary>
		private void OnSelectedEntityChanged()
		{
			OnSelectedEntityChanged(EventArgs.Empty);
		}
		
		/// <summary>
		/// Raises the <c>SelectedEntityChanged</c> event.
		/// </summary>
		protected virtual void OnSelectedEntityChanged(EventArgs e)
		{
			if(SelectedEntityChanged != null)
				SelectedEntityChanged(this, e);
		}

		/// <summary>
		/// Gets or sets whether to allow 'none' to be selected from the list.
		/// </summary>
		[Browsable(true), Category("Behavior"), Description("Gets or sets whether to allow 'none' to be selected from the list."), DefaultValue(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public bool AllowNullSelection
		{
			get
			{
				return _allowNullSelection;
			}
			set
			{
				// check to see if the value has changed...
				if(value != _allowNullSelection)
				{
					// set the value...
					_allowNullSelection = value;
					this.RefreshView();
					this.OnAllowNullSelectionChanged();
				}
			}
		}

		/// <summary>
		/// Raises the <c>AllowNullSelectionChanged</c> event.
		/// </summary>
		private void OnAllowNullSelectionChanged()
		{
			OnAllowNullSelectionChanged(EventArgs.Empty);
		}
		
		/// <summary>
		/// Raises the <c>AllowNullSelectionChanged</c> event.
		/// </summary>
		protected virtual void OnAllowNullSelectionChanged(EventArgs e)
		{
			if(AllowNullSelectionChanged != null)
				AllowNullSelectionChanged(this, e);
		}

		/// <summary>
		/// Gets the index of the given entity.
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="onNotFound"></param>
		/// <returns></returns>
		private int IndexOf(object entity, OnNotFound onNotFound)
		{
			if(entity == null)
			{
				// walk...
				for(int index = 0; index < this.InnerComboBox.Items.Count; index++)
				{
					if(this.InnerComboBox.Items[index] is string)
						return index;
				}
			}
			else
			{
				// find...
				for(int index = 0; index < this.InnerComboBox.Items.Count; index++)
				{
					object item = this.InnerComboBox.Items[index];
					if(item is EntityListItem)
					{
						if(Entity.Equals(((EntityListItem)item), entity))
							return index;
					}
				}
			}

			// nope...
			switch(onNotFound)
			{
				case OnNotFound.ReturnNull:
					return -1;

				case OnNotFound.ThrowException:
					if(entity != null)
						throw new InvalidOperationException(string.Format("The item '{0}' was not found in the collection.", entity));
					else
						throw new InvalidOperationException("The null item was not found in the collection.");

				default:
					throw new NotSupportedException(string.Format("Cannot handle '{0}' ({1}).", onNotFound, onNotFound.GetType()));
			}
		}

		private void InnerComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			if(this.InitializeCount == 0)
				this.OnSelectedEntityChanged();
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

		/// <summary>
		/// Gets or sets the binding
		/// </summary>
		Binding IDesktopBindableControl.Binding
		{
			get
			{
				return _binding;
			}
			set
			{
				// check to see if the value has changed...
				if(value != _binding)
				{
					// set the value...
					_binding = value;
				}
			}
		}
	}
}
