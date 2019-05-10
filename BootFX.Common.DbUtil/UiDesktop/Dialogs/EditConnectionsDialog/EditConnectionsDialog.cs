// BootFX - Application framework for .NET applications
// 
// File: EditConnectionsDialog.cs
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
using BootFX.Common.Data;

namespace BootFX.Common.UI.Desktop
{
	/// <summary>
	/// Summary description for EditConnectionsDialog.
	/// </summary>
	public class EditConnectionsDialog : BaseForm
	{
		private System.Windows.Forms.Button buttonOk;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.GroupBox groupBox1;
		private EntityListView listConnections;
		private System.Windows.Forms.Button buttonAdd;
		private System.Windows.Forms.Button buttonDelete;
		private System.Windows.Forms.Button buttonEdit;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public EditConnectionsDialog()
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
			this.buttonOk = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.listConnections = new BootFX.Common.UI.Desktop.EntityListView();
			this.buttonAdd = new System.Windows.Forms.Button();
			this.buttonDelete = new System.Windows.Forms.Button();
			this.buttonEdit = new System.Windows.Forms.Button();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// buttonOk
			// 
			this.buttonOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonOk.Location = new System.Drawing.Point(204, 340);
			this.buttonOk.Name = "buttonOk";
			this.buttonOk.TabIndex = 4;
			this.buttonOk.Text = "OK";
			this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
			// 
			// buttonCancel
			// 
			this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(288, 340);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.TabIndex = 3;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox1.Controls.Add(this.buttonEdit);
			this.groupBox1.Controls.Add(this.buttonDelete);
			this.groupBox1.Controls.Add(this.buttonAdd);
			this.groupBox1.Controls.Add(this.listConnections);
			this.groupBox1.Location = new System.Drawing.Point(8, 8);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(352, 324);
			this.groupBox1.TabIndex = 5;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "&Connections:";
			// 
			// listConnections
			// 
			this.listConnections.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.listConnections.DataSource = null;
			this.listConnections.Filter = null;
			this.listConnections.HideSelection = false;
			this.listConnections.Location = new System.Drawing.Point(12, 20);
			this.listConnections.Name = "listConnections";
			this.listConnections.Size = new System.Drawing.Size(248, 292);
			this.listConnections.TabIndex = 0;
			this.listConnections.View = System.Windows.Forms.View.Details;
			this.listConnections.DoubleClick += new System.EventHandler(this.listConnections_DoubleClick);
			// 
			// buttonAdd
			// 
			this.buttonAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonAdd.Location = new System.Drawing.Point(268, 20);
			this.buttonAdd.Name = "buttonAdd";
			this.buttonAdd.TabIndex = 1;
			this.buttonAdd.Text = "&Add";
			this.buttonAdd.Click += new System.EventHandler(this.buttonAdd_Click);
			// 
			// buttonDelete
			// 
			this.buttonDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonDelete.Location = new System.Drawing.Point(268, 76);
			this.buttonDelete.Name = "buttonDelete";
			this.buttonDelete.TabIndex = 2;
			this.buttonDelete.Text = "&Delete";
			this.buttonDelete.Click += new System.EventHandler(this.buttonDelete_Click);
			// 
			// buttonEdit
			// 
			this.buttonEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonEdit.Location = new System.Drawing.Point(268, 48);
			this.buttonEdit.Name = "buttonEdit";
			this.buttonEdit.TabIndex = 3;
			this.buttonEdit.Text = "&Edit";
			this.buttonEdit.Click += new System.EventHandler(this.buttonEdit_Click);
			// 
			// EditConnectionsDialog
			// 
			this.AcceptButton = this.buttonOk;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(368, 370);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.buttonOk);
			this.Controls.Add(this.buttonCancel);
			this.Name = "EditConnectionsDialog";
			this.Text = "Edit Connections";
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad (e);
			if(this.DesignMode)
				return;

			RefreshConnections();
		}

		/// <summary>
		/// Refreshes the connections.
		/// </summary>
		private void RefreshConnections()
		{
			this.listConnections.DataSource = null;
			this.listConnections.DataSource = ConnectionSettings.SavedSettings;
		}

		private void listConnections_DoubleClick(object sender, System.EventArgs e)
		{
			EditConnection();
		}

		private void buttonEdit_Click(object sender, System.EventArgs e)
		{
			EditConnection();
		}

		private void buttonDelete_Click(object sender, System.EventArgs e)
		{
			DeleteConnection();
		}

		private void buttonAdd_Click(object sender, System.EventArgs e)
		{
			AddConnection();
		}

		/// <summary>
		/// Gets teh current settings.
		/// </summary>
		private ConnectionSettings CurrentSettings
		{
			get
			{
				return (ConnectionSettings)this.listConnections.SelectedEntity;
			}
		}

		/// <summary>
		/// Edits the connection.
		/// </summary>
		private void EditConnection()
		{
			// TODO: This method has not been implemented.  Check whether it is a valid not implementated, or a code path that was not completed.
			throw new NotImplementedException(string.Format("Not implemented for '{0}'.", this.GetType()));
		}

		/// <summary>
		/// Edits the connection.
		/// </summary>
		private void AddConnection()
		{
			ConnectToDatabaseDialog dialog = new ConnectToDatabaseDialog();
			if(dialog.ShowDialog(this) == DialogResult.OK)
			{
				// add...
				ConnectionSettings.SavedSettings.Add(dialog.ConnectionSettings);

				// update...
				this.RefreshConnections();
			}
		}

		/// <summary>
		/// Edits the connection.
		/// </summary>
		private void DeleteConnection()
		{
			// TODO: This method has not been implemented.  Check whether it is a valid not implementated, or a code path that was not completed.
			throw new NotImplementedException(string.Format("Not implemented for '{0}'.", this.GetType()));
		}

		protected override bool DoApply(BootFX.Common.UI.Common.ApplyReason reason)
		{
			if(base.DoApply (reason) == false)
				return false;

			// save...
			ConnectionSettings.SaveSettings();

			// ok...
			return true;
		}

		private void buttonOk_Click(object sender, System.EventArgs e)
		{
			this.DialogOK();
		}

		private void buttonCancel_Click(object sender, System.EventArgs e)
		{
			this.DialogCancel();
		}
	}
}
