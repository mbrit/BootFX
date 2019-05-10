// BootFX - Application framework for .NET applications
// 
// File: EntityPickerDialog.cs
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

namespace BootFX.Common.UI.Desktop
{
	/// <summary>
	/// Summary description for EntityPickerDialog.
	/// </summary>
	public class EntityPickerDialog : DataDialog
	{
		/// <summary>
		/// Private field to support <c>ChoiceRequired</c> property.
		/// </summary>
		private bool _choiceRequired = true;
		
		/// <summary>
		/// Private field to support <c>MultiSelect</c> property.
		/// </summary>
		private bool _multiSelect = false;
		
		private BootFX.Common.UI.Desktop.DialogBar button1;
		private System.Windows.Forms.GroupBox groupBox1;
		private BootFX.Common.UI.Desktop.EntityListView listItems;
		private BootFX.Common.UI.Desktop.EntityListViewFilterWidget filterWidget;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public EntityPickerDialog()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			this.SaveChangesOnApply = false;
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
			this.button1 = new BootFX.Common.UI.Desktop.DialogBar();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.filterWidget = new BootFX.Common.UI.Desktop.EntityListViewFilterWidget();
			this.listItems = new BootFX.Common.UI.Desktop.EntityListView();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// button1
			// 
			this.button1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.button1.Location = new System.Drawing.Point(0, 346);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(380, 32);
			this.button1.TabIndex = 7;
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox1.Controls.Add(this.filterWidget);
			this.groupBox1.Controls.Add(this.listItems);
			this.groupBox1.Location = new System.Drawing.Point(8, 8);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(364, 332);
			this.groupBox1.TabIndex = 8;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Items";
			// 
			// filterWidget
			// 
			this.filterWidget.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.filterWidget.EntityListView = this.listItems;
			this.filterWidget.InitializeCount = 0;
			this.filterWidget.Location = new System.Drawing.Point(16, 304);
			this.filterWidget.Name = "filterWidget";
			this.filterWidget.Size = new System.Drawing.Size(340, 20);
			this.filterWidget.TabIndex = 1;
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
			this.listItems.MultiSelect = false;
			this.listItems.Name = "listItems";
			this.listItems.Size = new System.Drawing.Size(344, 280);
			this.listItems.TabIndex = 0;
			this.listItems.View = System.Windows.Forms.View.Details;
			this.listItems.DoubleClick += new System.EventHandler(this.listItems_DoubleClick);
			// 
			// EntityPickerDialog
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(380, 378);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.button1);
			this.Name = "EntityPickerDialog";
			this.Text = "Select";
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		protected override void OnDataSourceChanged(EventArgs e)
		{
			base.OnDataSourceChanged (e);
			this.listItems.DataSource = this.DataSource;
		}

		/// <summary>
		/// Gets the selected entities.
		/// </summary>
		public IList SelectedEntities
		{
			get
			{
				return this.listItems.SelectedEntities;
			}
		}

		/// <summary>
		/// Gets the selected entity.
		/// </summary>
		public object SelectedEntity
		{
			get
			{
				return this.listItems.SelectedEntity;
			}
		}

		/// <summary>
		/// Gets or sets whether the picker supports multiple selections.
		/// </summary>
		[Browsable(false), Category("Behavior"), DefaultValue(false), Description("Gets or sets whether the picker supports multiple selections.")]
		public bool MultiSelect
		{
			get
			{
				return _multiSelect;
			}
			set
			{
				// check to see if the value has changed...
				if(value != _multiSelect)
				{
					// set the value...
					_multiSelect = value;
					this.listItems.MultiSelect = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets whether the user much make a selection in order to click the OK button.
		/// </summary>
		[Browsable(false), Category("Behavior"), DefaultValue(false), Description("Gets or sets whether the user much make a selection in order to click the OK button.")]
		public bool ChoiceRequired
		{
			get
			{
				return _choiceRequired;
			}
			set
			{
				// check to see if the value has changed...
				if(value != _choiceRequired)
				{
					// set the value...
					_choiceRequired = value;
				}
			}
		}

		protected override void OnApplying(CancelEventArgs e)
		{
			base.OnApplying (e);
			if(e.Cancel)
				return;

			// try...
			if(this.ChoiceRequired)
			{
				// selected...
				IList selected = this.SelectedEntities;
				if(selected == null)
					throw new InvalidOperationException("selected is null.");

				// count...
				if(selected.Count == 0)
				{
					if(this.MultiSelect)
						Alert.ShowWarning(this, "You must select at least one item.");
					else
						Alert.ShowWarning(this, "You must select an item.");

					// nope...
					e.Cancel = true;
				}
			}		
		}

		/// <summary>
		/// Shows the dialog and allows the user to pick entities.
		/// </summary>
		/// <param name="owner"></param>
		/// <param name="caption"></param>
		/// <returns></returns>
		public static IList PickEntities(Control owner, string caption, object dataSource, bool choiceRequired)
		{
			if(caption == null)
				throw new ArgumentNullException("caption");
			if(caption.Length == 0)
				throw new ArgumentOutOfRangeException("'caption' is zero-length.");
			if(dataSource == null)
				throw new ArgumentNullException("dataSource");

			// create...
			EntityPickerDialog dialog = new EntityPickerDialog();
			dialog.Text = caption;
			dialog.DataSource = dataSource;
			dialog.MultiSelect = true;
			dialog.ChoiceRequired = choiceRequired;
			if(dialog.ShowDialog(owner) == DialogResult.OK)
				return dialog.SelectedEntities;
			else
				return null;
		}

		/// <summary>
		/// Shows the dialog and allows the user to pick entities.
		/// </summary>
		/// <param name="owner"></param>
		/// <param name="caption"></param>
		/// <returns></returns>
		public static object PickEntity(Control owner, string caption, object dataSource, bool choiceRequired)
		{
			if(caption == null)
				throw new ArgumentNullException("caption");
			if(caption.Length == 0)
				throw new ArgumentOutOfRangeException("'caption' is zero-length.");
			if(dataSource == null)
				throw new ArgumentNullException("dataSource");

			// create...
			EntityPickerDialog dialog = new EntityPickerDialog();
			dialog.Text = caption;
			dialog.DataSource = dataSource;
			dialog.MultiSelect = false;
			dialog.ChoiceRequired = choiceRequired;
			if(dialog.ShowDialog(owner) == DialogResult.OK)
				return dialog.SelectedEntity;
			else
				return null;
		}

		private void listItems_DoubleClick(object sender, System.EventArgs e)
		{
			this.DialogOK();
		}
	}
}
