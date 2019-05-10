// BootFX - Application framework for .NET applications
// 
// File: EntitySetDialog.cs
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
using BootFX.Common.Entities;

namespace BootFX.Common.UI.Desktop
{
	/// <summary>
	/// Summary description for EntitySetDialog.
	/// </summary>
	public class EntitySetDialog : BaseForm
	{
		/// <summary>
		/// Raised when the 'add' button is pressed.
		/// </summary>
		public event EventHandler AddClick;

		/// <summary>
		/// Raised when the 'edit' button is pressed.
		/// </summary>
		public event EntityEventHandler EditClick;

		/// <summary>
		/// Raised when the 'delete' button is pressed.
		/// </summary>
		public event EntityEventHandler DeleteClick;
		
		/// <summary>
		/// Raised when the <c>MasterEntity</c> property has changed.
		/// </summary>
		[Browsable(true), Category("Property Changed"), Description("Raised when the MasterEntity property has changed.")]
		public event EventHandler MasterEntityChanged;
		
		/// <summary>
		/// Private field to support <c>MasterEntity</c> property.
		/// </summary>
		private object _masterEntity;
		
		private System.Windows.Forms.GroupBox groupMaster;
		private BootFX.Common.UI.Desktop.EntityBox boxMaster;
		private BootFX.Common.UI.Desktop.DialogBar button1;
		private System.Windows.Forms.GroupBox groupItems;
		private System.Windows.Forms.Button buttonEdit;
		private System.Windows.Forms.Button buttonAdd;
		private BootFX.Common.UI.Desktop.EntityListView listItems;
		private EntityListViewFilterWidget widgetFilter;
		private System.Windows.Forms.Button buttonDelete;
		protected System.Windows.Forms.Panel panelOtherButtons;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public EntitySetDialog()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
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
			this.groupMaster = new System.Windows.Forms.GroupBox();
			this.boxMaster = new BootFX.Common.UI.Desktop.EntityBox();
			this.button1 = new BootFX.Common.UI.Desktop.DialogBar();
			this.groupItems = new System.Windows.Forms.GroupBox();
			this.buttonDelete = new System.Windows.Forms.Button();
			this.widgetFilter = new BootFX.Common.UI.Desktop.EntityListViewFilterWidget();
			this.listItems = new BootFX.Common.UI.Desktop.EntityListView();
			this.buttonEdit = new System.Windows.Forms.Button();
			this.buttonAdd = new System.Windows.Forms.Button();
			this.panelOtherButtons = new System.Windows.Forms.Panel();
			this.groupMaster.SuspendLayout();
			this.groupItems.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupMaster
			// 
			this.groupMaster.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.groupMaster.Controls.Add(this.boxMaster);
			this.groupMaster.Location = new System.Drawing.Point(8, 4);
			this.groupMaster.Name = "groupMaster";
			this.groupMaster.Size = new System.Drawing.Size(420, 48);
			this.groupMaster.TabIndex = 17;
			this.groupMaster.TabStop = false;
			this.groupMaster.Text = "Master";
			// 
			// boxMaster
			// 
			this.boxMaster.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.boxMaster.BackColor = System.Drawing.SystemColors.Window;
			this.boxMaster.DockPadding.Bottom = 1;
			this.boxMaster.DockPadding.Left = 4;
			this.boxMaster.DockPadding.Right = 4;
			this.boxMaster.DockPadding.Top = 1;
			this.boxMaster.Entity = null;
			this.boxMaster.Location = new System.Drawing.Point(12, 20);
			this.boxMaster.Name = "boxMaster";
			this.boxMaster.Size = new System.Drawing.Size(400, 20);
			this.boxMaster.TabIndex = 0;
			// 
			// button1
			// 
			this.button1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.button1.Location = new System.Drawing.Point(0, 390);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(436, 32);
			this.button1.TabIndex = 15;
			// 
			// groupItems
			// 
			this.groupItems.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.groupItems.Controls.Add(this.panelOtherButtons);
			this.groupItems.Controls.Add(this.buttonDelete);
			this.groupItems.Controls.Add(this.widgetFilter);
			this.groupItems.Controls.Add(this.buttonEdit);
			this.groupItems.Controls.Add(this.buttonAdd);
			this.groupItems.Controls.Add(this.listItems);
			this.groupItems.Location = new System.Drawing.Point(8, 60);
			this.groupItems.Name = "groupItems";
			this.groupItems.Size = new System.Drawing.Size(420, 324);
			this.groupItems.TabIndex = 16;
			this.groupItems.TabStop = false;
			this.groupItems.Text = "Items";
			// 
			// buttonDelete
			// 
			this.buttonDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonDelete.Location = new System.Drawing.Point(336, 76);
			this.buttonDelete.Name = "buttonDelete";
			this.buttonDelete.TabIndex = 4;
			this.buttonDelete.Text = "&Delete";
			this.buttonDelete.Click += new System.EventHandler(this.buttonDelete_Click);
			// 
			// widgetFilter
			// 
			this.widgetFilter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.widgetFilter.EntityListView = this.listItems;
			this.widgetFilter.InitializeCount = 0;
			this.widgetFilter.Location = new System.Drawing.Point(12, 292);
			this.widgetFilter.Name = "widgetFilter";
			this.widgetFilter.Size = new System.Drawing.Size(320, 20);
			this.widgetFilter.TabIndex = 3;
			// 
			// listItems
			// 
			this.listItems.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.listItems.DataSource = null;
			this.listItems.Filter = null;
			this.listItems.FullRowSelect = true;
			this.listItems.HideSelection = false;
			this.listItems.Location = new System.Drawing.Point(12, 20);
			this.listItems.Name = "listItems";
			this.listItems.Size = new System.Drawing.Size(320, 268);
			this.listItems.TabIndex = 0;
			this.listItems.View = System.Windows.Forms.View.Details;
			this.listItems.EntityDoubleClick += new BootFX.Common.Entities.EntityEventHandler(this.listItems_EntityDoubleClick);
			// 
			// buttonEdit
			// 
			this.buttonEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonEdit.Location = new System.Drawing.Point(336, 48);
			this.buttonEdit.Name = "buttonEdit";
			this.buttonEdit.TabIndex = 2;
			this.buttonEdit.Text = "&Edit";
			this.buttonEdit.Click += new System.EventHandler(this.buttonEdit_Click);
			// 
			// buttonAdd
			// 
			this.buttonAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonAdd.Location = new System.Drawing.Point(336, 20);
			this.buttonAdd.Name = "buttonAdd";
			this.buttonAdd.TabIndex = 1;
			this.buttonAdd.Text = "&Add";
			this.buttonAdd.Click += new System.EventHandler(this.buttonAdd_Click);
			// 
			// panelOtherButtons
			// 
			this.panelOtherButtons.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.panelOtherButtons.Location = new System.Drawing.Point(336, 104);
			this.panelOtherButtons.Name = "panelOtherButtons";
			this.panelOtherButtons.Size = new System.Drawing.Size(76, 212);
			this.panelOtherButtons.TabIndex = 5;
			// 
			// EntitySetDialog
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(436, 422);
			this.Controls.Add(this.groupMaster);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.groupItems);
			this.Name = "EntitySetDialog";
			this.Text = "EntitySetDialog";
			this.groupMaster.ResumeLayout(false);
			this.groupItems.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// Gets or sets the masterentity
		/// </summary>
		public object MasterEntity
		{
			get
			{
				return _masterEntity;
			}
			set
			{
				// check to see if the value has changed...
				if(value != _masterEntity)
				{
					// set the value...
					_masterEntity = value;
					this.boxMaster.Entity = this.MasterEntity;
					this.OnMasterEntityChanged();
				}
			}
		}

		/// <summary>
		/// Raises the <c>MasterEntityChanged</c> event.
		/// </summary>
		private void OnMasterEntityChanged()
		{
			OnMasterEntityChanged(EventArgs.Empty);
		}

		private void buttonAdd_Click(object sender, System.EventArgs e)
		{
			try
			{
				this.Add();
			}
			catch(Exception ex)
			{
				Alert.ShowWarning(this, "Failed to add a new item.", ex);
			}
		}

		/// <summary>
		/// Called when the 'add' button is pressed.
		/// </summary>
		private void Add()
		{
			this.OnAddClick();
		}

		private void buttonEdit_Click(object sender, System.EventArgs e)
		{
			try
			{
				this.Edit();
			}
			catch(Exception ex)
			{
				Alert.ShowWarning(this, "Failed to edit an item.", ex);
			}
		}

		/// <summary>
		/// Called when the 'edit' button is pressed.
		/// </summary>
		private void Edit()
		{
			if(this.CurrentEntity != null)
				this.OnEditClick(new EntityEventArgs(this.CurrentEntity));
			else
				Alert.ShowWarning(this, "You must select an item.");
		}

		/// <summary>
		/// Gets the current entity.
		/// </summary>
		protected object CurrentEntity
		{
			get
			{
				return this.listItems.SelectedEntity;
			}
		}

		private void buttonDelete_Click(object sender, System.EventArgs e)
		{
			try
			{
				Delete();
			}
			catch(Exception ex)
			{
				Alert.ShowWarning(this, "Failed to delete an item.", ex);
			}
		}

		/// <summary>
		/// Deletes the selected entity.
		/// </summary>
		private void Delete()
		{
			if(this.CurrentEntity != null)
				this.OnDeleteClick(new EntityEventArgs(this.CurrentEntity));
			else
				Alert.ShowWarning(this, "You must select an item.");
		}

		/// <summary>
		/// Raises the <c>DeleteClick</c> event.
		/// </summary>
		protected virtual void OnDeleteClick(EntityEventArgs e)
		{
			if(e == null)
				throw new InvalidOperationException("e is null.");
			if(e.Entity == null)
				throw new InvalidOperationException("e.Entity is null.");
			if(this.DeleteEntity(e.Entity))
				this.RefreshView();

			// raise...
			if(DeleteClick != null)
				DeleteClick(this, e);
		}
		
		/// <summary>
		/// Raises the <c>EditClick</c> event.
		/// </summary>
		protected virtual void OnEditClick(EntityEventArgs e)
		{
			// by default, edit the entity...
			if(e == null)
				throw new InvalidOperationException("e is null.");
			if(e.Entity == null)
				throw new InvalidOperationException("e.Entity is null.");
			if(this.EditEntity(e.Entity))
				this.RefreshView();

			// raise...
			if(EditClick != null)
				EditClick(this, e);
		}
		
		/// <summary>
		/// Raises the <c>AddClick</c> event.
		/// </summary>
		private void OnAddClick()
		{
			OnAddClick(EventArgs.Empty);
		}
		
		/// <summary>
		/// Raises the <c>AddClick</c> event.
		/// </summary>
		protected virtual void OnAddClick(EventArgs e)
		{
			// handle...
			object newEntity = this.CreateNewEntity();
			if(newEntity != null)
			{
				if(this.EditEntity(newEntity))
					this.RefreshView();
			}

			// raise...
			if(AddClick != null)
				AddClick(this, e);
		}

		/// <summary>
		/// Deletes the given entity.
		/// </summary>
		/// <param name="entity"></param>
		/// <returns></returns>
		private bool DeleteEntity(object entity)
		{
			if(entity == null)
				throw new ArgumentNullException("entity");
			
			// check...
//			return DesktopCommands.DeleteEntity(this, entity);
			// TODO: This method has not been implemented.  Check whether it is a valid not implementated, or a code path that was not completed.
			throw new NotImplementedException(string.Format("Not implemented for '{0}'.", this.GetType()));
		}

		/// <summary>
		/// Edits the given entity.
		/// </summary>
		/// <param name="entity"></param>
		/// <returns></returns>
		private bool EditEntity(object entity)
		{
			if(entity == null)
				throw new ArgumentNullException("entity");
			
			// check...
			//return DesktopCommands.EditEntity(this, entity);
			// TODO: This method has not been implemented.  Check whether it is a valid not implementated, or a code path that was not completed.
			throw new NotImplementedException(string.Format("Not implemented for '{0}'.", this.GetType()));
		}

		/// <summary>
		/// Creates a new entity.
		/// </summary>
		/// <returns></returns>
		protected virtual object CreateNewEntity()
		{
			return null;
		}
		
		/// <summary>
		/// Raises the <c>MasterEntityChanged</c> event.
		/// </summary>
		protected virtual void OnMasterEntityChanged(EventArgs e)
		{
			this.RefreshView();

			// raise the event...
			if(MasterEntityChanged != null)
				MasterEntityChanged(this, e);
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		protected virtual void RefreshView()
		{
			// get items...
			if(this.MasterEntity != null)
				this.listItems.DataSource = this.GetItemsForList();
			else
				this.listItems.DataSource = null;
		}

		/// <summary>
		/// Called when items for the list are required.
		/// </summary>
		/// <returns></returns>
		protected virtual IList GetItemsForList()
		{
			return null;
		}

		private void listItems_EntityDoubleClick(object sender, BootFX.Common.Entities.EntityEventArgs e)
		{
			this.Edit();
		}

		/// <summary>
		/// Gets or sets the itemscaption
		/// </summary>
		[Browsable(true), Description("Gets or sets the 'items' caption."), Category("Appearance")]
		public string ItemsCaption
		{
			get
			{
				return this.groupItems.Text;
			}
			set
			{
				this.groupItems.Text = value;
			}
		}
		
		/// <summary>
		/// Gets or sets the mastercaption
		/// </summary>
		[Browsable(true), Description("Gets or sets the 'master' caption."), Category("Appearance")]
		public string MasterCaption
		{
			get
			{
				return this.groupMaster.Text;
			}
			set
			{
				this.groupMaster.Text = value;
			}
		}

		/// <summary>
		/// Gets or sets the multiselect
		/// </summary>
		[Browsable(true), Description("Gets or sets whether multiple selections are allowed."), Category("Behavior")]
		public bool MultiSelect
		{
			get
			{
				return this.listItems.MultiSelect;
			}
			set
			{
				this.listItems.MultiSelect = value;
			}
		}
	}
}
