// BootFX - Application framework for .NET applications
// 
// File: SettingsDialog.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.IO;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using BootFX.Common.UI.Desktop;
using BootFX.Common.UI.Common;

namespace BootFX.Common.DbUtil
{
	/// <summary>
	/// Summary description for SettingsDialog.
	/// </summary>
	internal class SettingsDialog : BaseForm
	{
		/// <summary>
		/// Private field to support <c>Settings</c> property.
		/// </summary>
		private Settings _settings;

		/// <summary>
		/// Private field to support <c>LocalSettings</c> property.
		/// </summary>
		private LocalSettings _localSettings;
		
		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage tabPage2;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Button buttonOk;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textFolderPath;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button buttonBrowse;
		private System.Windows.Forms.Button buttonOpen;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.RadioButton radioDefaultDatabase;
		private System.Windows.Forms.RadioButton radioSpecificDatabase;
		private System.Windows.Forms.TextBox textDatabaseName;
		private System.Windows.Forms.Button buttonOpenServices;
		private System.Windows.Forms.Button buttonBrowseServices;
		private System.Windows.Forms.TextBox textServicesFolderPath;
		private System.Windows.Forms.TextBox textNamespace;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.RadioButton radioDotNetV2;
		private System.Windows.Forms.RadioButton radioDotNetV1;
		private System.Windows.Forms.TextBox textBoxEntityBaseType;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox textBoxServicesBaseType;
		private System.Windows.Forms.Label labelServicesBaseType;
        private System.Windows.Forms.Label labelServicesFolderPath;
		private System.Windows.Forms.TabPage tabPage1;
		private System.Windows.Forms.GroupBox groupBox4;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox textCodeGenerationTypeName;
		private System.Windows.Forms.Label label6;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public SettingsDialog()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.textDatabaseName = new System.Windows.Forms.TextBox();
            this.radioSpecificDatabase = new System.Windows.Forms.RadioButton();
            this.radioDefaultDatabase = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.textBoxServicesBaseType = new System.Windows.Forms.TextBox();
            this.labelServicesBaseType = new System.Windows.Forms.Label();
            this.textBoxEntityBaseType = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.buttonBrowseServices = new System.Windows.Forms.Button();
            this.buttonOpenServices = new System.Windows.Forms.Button();
            this.textServicesFolderPath = new System.Windows.Forms.TextBox();
            this.labelServicesFolderPath = new System.Windows.Forms.Label();
            this.buttonOpen = new System.Windows.Forms.Button();
            this.buttonBrowse = new System.Windows.Forms.Button();
            this.textFolderPath = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioDotNetV1 = new System.Windows.Forms.RadioButton();
            this.radioDotNetV2 = new System.Windows.Forms.RadioButton();
            this.label4 = new System.Windows.Forms.Label();
            this.textNamespace = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.label6 = new System.Windows.Forms.Label();
            this.textCodeGenerationTypeName = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonOk = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Location = new System.Drawing.Point(8, 8);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(368, 447);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.groupBox3);
            this.tabPage2.Controls.Add(this.groupBox2);
            this.tabPage2.Controls.Add(this.groupBox1);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Size = new System.Drawing.Size(360, 421);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "General";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.textDatabaseName);
            this.groupBox3.Controls.Add(this.radioSpecificDatabase);
            this.groupBox3.Controls.Add(this.radioDefaultDatabase);
            this.groupBox3.Location = new System.Drawing.Point(8, 345);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(344, 68);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Database";
            // 
            // textDatabaseName
            // 
            this.textDatabaseName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textDatabaseName.Enabled = false;
            this.textDatabaseName.Location = new System.Drawing.Point(128, 40);
            this.textDatabaseName.Name = "textDatabaseName";
            this.textDatabaseName.Size = new System.Drawing.Size(208, 20);
            this.textDatabaseName.TabIndex = 2;
            this.textDatabaseName.Tag = "";
            // 
            // radioSpecificDatabase
            // 
            this.radioSpecificDatabase.Location = new System.Drawing.Point(12, 40);
            this.radioSpecificDatabase.Name = "radioSpecificDatabase";
            this.radioSpecificDatabase.Size = new System.Drawing.Size(116, 20);
            this.radioSpecificDatabase.TabIndex = 4;
            this.radioSpecificDatabase.Text = "&Specific database:";
            this.radioSpecificDatabase.CheckedChanged += new System.EventHandler(this.radioSpecificDatabase_CheckedChanged);
            // 
            // radioDefaultDatabase
            // 
            this.radioDefaultDatabase.Checked = true;
            this.radioDefaultDatabase.Location = new System.Drawing.Point(12, 20);
            this.radioDefaultDatabase.Name = "radioDefaultDatabase";
            this.radioDefaultDatabase.Size = new System.Drawing.Size(108, 20);
            this.radioDefaultDatabase.TabIndex = 3;
            this.radioDefaultDatabase.TabStop = true;
            this.radioDefaultDatabase.Text = "&Default database";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.textBoxServicesBaseType);
            this.groupBox2.Controls.Add(this.labelServicesBaseType);
            this.groupBox2.Controls.Add(this.textBoxEntityBaseType);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.buttonBrowseServices);
            this.groupBox2.Controls.Add(this.buttonOpenServices);
            this.groupBox2.Controls.Add(this.textServicesFolderPath);
            this.groupBox2.Controls.Add(this.labelServicesFolderPath);
            this.groupBox2.Controls.Add(this.buttonOpen);
            this.groupBox2.Controls.Add(this.buttonBrowse);
            this.groupBox2.Controls.Add(this.textFolderPath);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Location = new System.Drawing.Point(8, 92);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(344, 247);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Generation";
            // 
            // textBoxServicesBaseType
            // 
            this.textBoxServicesBaseType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxServicesBaseType.Location = new System.Drawing.Point(12, 189);
            this.textBoxServicesBaseType.Name = "textBoxServicesBaseType";
            this.textBoxServicesBaseType.Size = new System.Drawing.Size(324, 20);
            this.textBoxServicesBaseType.TabIndex = 11;
            this.textBoxServicesBaseType.Tag = "ServiceBaseType";
            // 
            // labelServicesBaseType
            // 
            this.labelServicesBaseType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelServicesBaseType.Location = new System.Drawing.Point(11, 172);
            this.labelServicesBaseType.Name = "labelServicesBaseType";
            this.labelServicesBaseType.Size = new System.Drawing.Size(324, 16);
            this.labelServicesBaseType.TabIndex = 10;
            this.labelServicesBaseType.Text = "DTO base type name:";
            // 
            // textBoxEntityBaseType
            // 
            this.textBoxEntityBaseType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxEntityBaseType.Location = new System.Drawing.Point(12, 72);
            this.textBoxEntityBaseType.Name = "textBoxEntityBaseType";
            this.textBoxEntityBaseType.Size = new System.Drawing.Size(324, 20);
            this.textBoxEntityBaseType.TabIndex = 9;
            this.textBoxEntityBaseType.Tag = "BaseType";
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label5.Location = new System.Drawing.Point(12, 56);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(324, 16);
            this.label5.TabIndex = 8;
            this.label5.Text = "Entity base type name:";
            // 
            // buttonBrowseServices
            // 
            this.buttonBrowseServices.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonBrowseServices.Location = new System.Drawing.Point(260, 215);
            this.buttonBrowseServices.Name = "buttonBrowseServices";
            this.buttonBrowseServices.Size = new System.Drawing.Size(76, 23);
            this.buttonBrowseServices.TabIndex = 7;
            this.buttonBrowseServices.Text = "&Browse >>";
            this.buttonBrowseServices.Click += new System.EventHandler(this.buttonBrowseServices_Click);
            // 
            // buttonOpenServices
            // 
            this.buttonOpenServices.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOpenServices.Location = new System.Drawing.Point(176, 215);
            this.buttonOpenServices.Name = "buttonOpenServices";
            this.buttonOpenServices.Size = new System.Drawing.Size(76, 23);
            this.buttonOpenServices.TabIndex = 6;
            this.buttonOpenServices.Text = "O&pen >>";
            this.buttonOpenServices.Click += new System.EventHandler(this.buttonOpenServices_Click);
            // 
            // textServicesFolderPath
            // 
            this.textServicesFolderPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textServicesFolderPath.Location = new System.Drawing.Point(12, 147);
            this.textServicesFolderPath.Name = "textServicesFolderPath";
            this.textServicesFolderPath.Size = new System.Drawing.Size(324, 20);
            this.textServicesFolderPath.TabIndex = 5;
            this.textServicesFolderPath.Tag = "ServicesFolderPath";
            // 
            // labelServicesFolderPath
            // 
            this.labelServicesFolderPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelServicesFolderPath.Location = new System.Drawing.Point(9, 128);
            this.labelServicesFolderPath.Name = "labelServicesFolderPath";
            this.labelServicesFolderPath.Size = new System.Drawing.Size(324, 16);
            this.labelServicesFolderPath.TabIndex = 4;
            this.labelServicesFolderPath.Text = "&Output DTOs to this folder:";
            // 
            // buttonOpen
            // 
            this.buttonOpen.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOpen.Location = new System.Drawing.Point(176, 96);
            this.buttonOpen.Name = "buttonOpen";
            this.buttonOpen.Size = new System.Drawing.Size(76, 23);
            this.buttonOpen.TabIndex = 3;
            this.buttonOpen.Text = "O&pen >>";
            this.buttonOpen.Click += new System.EventHandler(this.buttonOpen_Click);
            // 
            // buttonBrowse
            // 
            this.buttonBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonBrowse.Location = new System.Drawing.Point(260, 96);
            this.buttonBrowse.Name = "buttonBrowse";
            this.buttonBrowse.Size = new System.Drawing.Size(76, 23);
            this.buttonBrowse.TabIndex = 2;
            this.buttonBrowse.Text = "&Browse >>";
            this.buttonBrowse.Click += new System.EventHandler(this.buttonBrowse_Click);
            // 
            // textFolderPath
            // 
            this.textFolderPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textFolderPath.Location = new System.Drawing.Point(12, 32);
            this.textFolderPath.Name = "textFolderPath";
            this.textFolderPath.Size = new System.Drawing.Size(324, 20);
            this.textFolderPath.TabIndex = 1;
            this.textFolderPath.Tag = "FolderPath";
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.Location = new System.Drawing.Point(12, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(324, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "&Output entities to this folder:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioDotNetV1);
            this.groupBox1.Controls.Add(this.radioDotNetV2);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.textNamespace);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Location = new System.Drawing.Point(8, 8);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(344, 68);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Code";
            // 
            // radioDotNetV1
            // 
            this.radioDotNetV1.Location = new System.Drawing.Point(88, 40);
            this.radioDotNetV1.Name = "radioDotNetV1";
            this.radioDotNetV1.Size = new System.Drawing.Size(82, 20);
            this.radioDotNetV1.TabIndex = 6;
            this.radioDotNetV1.Text = ".NET 1.0/1.1";
            // 
            // radioDotNetV2
            // 
            this.radioDotNetV2.Enabled = false;
            this.radioDotNetV2.Location = new System.Drawing.Point(176, 40);
            this.radioDotNetV2.Name = "radioDotNetV2";
            this.radioDotNetV2.Size = new System.Drawing.Size(92, 20);
            this.radioDotNetV2.TabIndex = 5;
            this.radioDotNetV2.Text = ".NET 2.0";
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(12, 40);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(72, 20);
            this.label4.TabIndex = 3;
            this.label4.Text = ".NET version:";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // textNamespace
            // 
            this.textNamespace.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textNamespace.Location = new System.Drawing.Point(88, 16);
            this.textNamespace.Name = "textNamespace";
            this.textNamespace.Size = new System.Drawing.Size(248, 20);
            this.textNamespace.TabIndex = 2;
            this.textNamespace.Tag = "namespacename";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(12, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(72, 20);
            this.label2.TabIndex = 1;
            this.label2.Text = "&Namespace:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.groupBox4);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Size = new System.Drawing.Size(360, 421);
            this.tabPage1.TabIndex = 2;
            this.tabPage1.Text = "Advanced";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.label6);
            this.groupBox4.Controls.Add(this.textCodeGenerationTypeName);
            this.groupBox4.Controls.Add(this.label3);
            this.groupBox4.Location = new System.Drawing.Point(8, 8);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(344, 100);
            this.groupBox4.TabIndex = 0;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Generation";
            // 
            // label6
            // 
            this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label6.Location = new System.Drawing.Point(148, 60);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(188, 28);
            this.label6.TabIndex = 2;
            this.label6.Text = "Leave this as an empty entry to use the default code generation type.";
            // 
            // textCodeGenerationTypeName
            // 
            this.textCodeGenerationTypeName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textCodeGenerationTypeName.Location = new System.Drawing.Point(12, 36);
            this.textCodeGenerationTypeName.Name = "textCodeGenerationTypeName";
            this.textCodeGenerationTypeName.Size = new System.Drawing.Size(324, 20);
            this.textCodeGenerationTypeName.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.Location = new System.Drawing.Point(12, 20);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(324, 16);
            this.label3.TabIndex = 0;
            this.label3.Text = "&Code generation type:";
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(300, 463);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 1;
            this.buttonCancel.Text = "Cancel";
            // 
            // buttonOk
            // 
            this.buttonOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOk.Location = new System.Drawing.Point(216, 463);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(75, 23);
            this.buttonOk.TabIndex = 2;
            this.buttonOk.Text = "OK";
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // SettingsDialog
            // 
            this.AcceptButton = this.buttonOk;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(380, 493);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.tabControl1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SettingsDialog";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Settings";
            this.tabControl1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabPage1.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);

		}
		#endregion

		private void buttonOk_Click(object sender, System.EventArgs e)
		{
			this.DialogOK();
		}

		private void buttonBrowse_Click(object sender, System.EventArgs e)
		{
			string path = Browse();
			if(path != null && path.Length > 0)
				this.textFolderPath.Text = path;
		}

		/// <summary>
		/// Browse for the folder.
		/// </summary>
		private string Browse()
		{
			FolderBrowserDialog dialog = new FolderBrowserDialog();
			dialog.Description = "Choose the default folder for code generation output.";
			dialog.SelectedPath = this.textFolderPath.Text;
			if(dialog.ShowDialog(this) == DialogResult.OK)
				return dialog.SelectedPath;
			else
				return null;
		}

		private void buttonOpen_Click(object sender, System.EventArgs e)
		{
			Open(this.textFolderPath.Text);
		}

		/// <summary>
		/// Opens the dialog.
		/// </summary>
		private void Open(string path)
		{
			if(path == null)
				throw new ArgumentNullException("path");
			if(path.Length == 0)
				throw new ArgumentOutOfRangeException("'path' is zero-length.");

			// open...
			path = path.Trim();
			try
			{
				if(path.Length == 0)
				{
					Alert.ShowWarning(this, "You must enter a path.");
					return;
				}
				
				// exists?
				if(Directory.Exists(path) == false)
				{
					Alert.ShowWarning(this, "The folder does not exist.");
					return;
				}

				// show...
				System.Diagnostics.Process.Start("explorer.exe", path);
			}
			catch(Exception ex)
			{
				Alert.ShowWarning(this, string.Format("The folder '{0}' could not be opened.", path), ex);
			}
		}

		private void radioSpecificDatabase_CheckedChanged(object sender, System.EventArgs e)
		{
			this.textDatabaseName.Enabled = this.radioSpecificDatabase.Checked;
		}

		/// <summary>
		/// Gets or sets the localsettings
		/// </summary>
		internal LocalSettings LocalSettings
		{
			get
			{
				return _localSettings;
			}
			set
			{
				// check to see if the value has changed...
				if(value != _localSettings)
				{
					// set the value...
					_localSettings = value;

					// set...
					if(value != null)
					{
						this.textFolderPath.Text = value.EntitiesFolderPath;
						this.textServicesFolderPath.Text = value.DtoFolderPath;
					}
				}
			}
		}
		
		/// <summary>
		/// Gets or sets the settings
		/// </summary>
		internal Settings Settings
		{
			get
			{
				return _settings;
			}
			set
			{
				// check to see if the value has changed...
				if(value != _settings)
				{
					// set the value...
					_settings = value;
		
					// set...
					if(value != null)
					{
						if(value.DatabaseName == null || value.DatabaseName.Length == 0)
							this.radioDefaultDatabase.Checked = true;
						else
						{
							this.radioSpecificDatabase.Checked = true;
							this.textDatabaseName.Text = value.DatabaseName;
						}

						// namespace...
						this.textNamespace.Text = value.NamespaceName;
						this.textBoxEntityBaseType.Text = value.BaseType;
						this.textBoxServicesBaseType.Text = value.DtoBaseType;

						// radio...
						switch(value.TargetVersion)
						{
							case DotNetVersion.V1:
								this.radioDotNetV1.Checked = true;
								break;

							case DotNetVersion.V2:
								this.radioDotNetV2.Checked = true;
								break;

							default:
								throw new NotSupportedException(string.Format("Cannot handle '{0}' ({1}).", value.TargetVersion, value.TargetVersion.GetType()));
						}

					}
				}
			}
		}

		protected override bool DoApply(ApplyReason reason)
		{
			if(base.DoApply (reason) == false)
				return false;

			if(Settings == null)
				throw new InvalidOperationException("Settings is null.");
			if(LocalSettings == null)
				throw new InvalidOperationException("LocalSettings is null.");

			// set...
			if(this.radioDefaultDatabase.Checked)
				this.Settings.DatabaseName = null;
			else
				this.Settings.DatabaseName = this.textDatabaseName.Text;
			this.Settings.NamespaceName = this.textNamespace.Text;

			// radio...
			if(this.radioDotNetV1.Checked)
				this.Settings.TargetVersion = DotNetVersion.V1;
			else
				this.Settings.TargetVersion = DotNetVersion.V2;

			// entities...
			this.LocalSettings.EntitiesFolderPath = this.textFolderPath.Text;
			this.Settings.BaseType = this.textBoxEntityBaseType.Text;

			// services...
			this.LocalSettings.DtoFolderPath = this.textServicesFolderPath.Text;
			this.Settings.DtoBaseType = this.textBoxServicesBaseType.Text;

			// ok...
			return true;
		}

		private void buttonOpenServices_Click(object sender, System.EventArgs e)
		{
			this.Open(this.textServicesFolderPath.Text);
		}

		private void buttonBrowseServices_Click(object sender, System.EventArgs e)
		{
			string path = Browse();
			if(path != null && path.Length > 0)
				this.textServicesFolderPath.Text = path;
		}
	}
}
