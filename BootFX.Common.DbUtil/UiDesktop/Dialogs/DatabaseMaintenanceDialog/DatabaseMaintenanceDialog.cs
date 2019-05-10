// BootFX - Application framework for .NET applications
// 
// File: DatabaseMaintenanceDialog.cs
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
	/// Summary description for DatabaseMaintenanceDialog.
	/// </summary>
	public class DatabaseMaintenanceDialog : BaseForm
	{
		/// <summary>
		/// Private field to support <c>IsDirty</c> property.
		/// </summary>
		private bool _isDirty;
		
		private System.Windows.Forms.Button buttonClose;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textCurrent;
		private System.Windows.Forms.Button buttonChange;
		private System.Windows.Forms.LinkLabel linkDatabaseUpdate;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public DatabaseMaintenanceDialog()
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
			this.buttonClose = new System.Windows.Forms.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.linkDatabaseUpdate = new System.Windows.Forms.LinkLabel();
			this.buttonChange = new System.Windows.Forms.Button();
			this.textCurrent = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// buttonClose
			// 
			this.buttonClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonClose.Location = new System.Drawing.Point(348, 78);
			this.buttonClose.Name = "buttonClose";
			this.buttonClose.TabIndex = 1;
			this.buttonClose.Text = "Close";
			this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox1.Controls.Add(this.linkDatabaseUpdate);
			this.groupBox1.Controls.Add(this.buttonChange);
			this.groupBox1.Controls.Add(this.textCurrent);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Location = new System.Drawing.Point(8, 8);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(416, 64);
			this.groupBox1.TabIndex = 2;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Current database";
			// 
			// linkDatabaseUpdate
			// 
			this.linkDatabaseUpdate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.linkDatabaseUpdate.Location = new System.Drawing.Point(60, 40);
			this.linkDatabaseUpdate.Name = "linkDatabaseUpdate";
			this.linkDatabaseUpdate.Size = new System.Drawing.Size(264, 16);
			this.linkDatabaseUpdate.TabIndex = 3;
			this.linkDatabaseUpdate.TabStop = true;
			this.linkDatabaseUpdate.Text = "Run Database Update...";
			this.linkDatabaseUpdate.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkDatabaseUpdate_LinkClicked);
			// 
			// buttonChange
			// 
			this.buttonChange.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonChange.Location = new System.Drawing.Point(332, 16);
			this.buttonChange.Name = "buttonChange";
			this.buttonChange.TabIndex = 2;
			this.buttonChange.Text = "&Change";
			this.buttonChange.Click += new System.EventHandler(this.buttonChange_Click);
			// 
			// textCurrent
			// 
			this.textCurrent.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.textCurrent.Location = new System.Drawing.Point(60, 16);
			this.textCurrent.Name = "textCurrent";
			this.textCurrent.ReadOnly = true;
			this.textCurrent.Size = new System.Drawing.Size(264, 20);
			this.textCurrent.TabIndex = 1;
			this.textCurrent.Text = "";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(12, 16);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(48, 23);
			this.label1.TabIndex = 0;
			this.label1.Text = "&Details:";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// DatabaseMaintenanceDialog
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(428, 108);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.buttonClose);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "DatabaseMaintenanceDialog";
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Database Maintenance";
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void buttonClose_Click(object sender, System.EventArgs e)
		{
			this.Close();
		}

		private void buttonChange_Click(object sender, System.EventArgs e)
		{
			ConnectToDatabaseDialog dialog = new ConnectToDatabaseDialog();
			dialog.NameRequired = false;
			dialog.ConnectionSettings = Database.DefaultConnectionSettings;
			if(dialog.ShowDialog(this) == DialogResult.OK)
				SetDatabase(dialog.ConnectionSettings, true);
		}

		/// <summary>
		/// Gets the isdirty.
		/// </summary>
		private bool IsDirty
		{
			get
			{
				// returns the value...
				return _isDirty;
			}
		}

		private void SetDatabase(ConnectionSettings settings, bool makeDirty)
		{
			if(settings == null)
				throw new ArgumentNullException("settings");
			
			// set...
			this.textCurrent.Text = settings.FriendlyName;

			// commit...
			Database.SetDefaultDatabase(settings);

			// set...
			if(makeDirty)
				_isDirty = true;
		}

		private void linkDatabaseUpdate_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
		{
			try
			{
				using(DatabaseUpdateDialog dialog = new DatabaseUpdateDialog())
					dialog.ShowDialog(this);
			}
			catch(Exception ex)
			{
				Alert.ShowWarning(this, "Failed to run database update.", ex);
			}
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad (e);

			// set...
			if(Database.HasDefaultDatabaseSettings())
				this.SetDatabase(Database.DefaultConnectionSettings, false);
		}

		protected override void OnClosed(EventArgs e)
		{
			base.OnClosed (e);

			// dirty?
			if(this.IsDirty)
			{
				try
				{
					// reset..
					_isDirty = false;

					// current...
					ConnectionSettings current = Database.DefaultConnectionSettings;
					if(current == null)
						throw new InvalidOperationException("current is null.");

					// save...
					Runtime.Current.InstallationSettings.ConnectionType = current.ConnectionType;
					Runtime.Current.InstallationSettings.ConnectionString = current.ConnectionString;
					Runtime.Current.InstallationSettings.Save();
				}
				catch(Exception ex)
				{
					Alert.ShowWarning(this, "The connection settings could not be saved.", ex);
				}
			}
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			base.OnClosing (e);

			// check...
			if(!(Database.HasDefaultDatabaseSettings()))
			{
				if(Alert.AskYesNoQuestion(this, "You do not have database settings configured.  Do you want to continue?") != DialogResult.Yes)
				{
					e.Cancel = true;
					return;
				}
				else
				{
					_isDirty = false;
					this.DialogResult = DialogResult.Cancel;
				}
			}
			else
				this.DialogResult = DialogResult.OK;
		}
	}
}
