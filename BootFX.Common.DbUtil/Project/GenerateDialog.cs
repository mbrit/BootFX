// BootFX - Application framework for .NET applications
// 
// File: GenerateDialog.cs
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
using BootFX.Common.UI.Desktop;

namespace BootFX.Common.DbUtil
{
	/// <summary>
	/// Summary description for GenerateDialog.
	/// </summary>
	public class GenerateDialog : DataDialog
	{
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label label1;
		private DialogBar button1;
		private System.Windows.Forms.TextBox textEntitiesPath;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox textServicesPath;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public GenerateDialog()
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
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.textServicesPath = new System.Windows.Forms.TextBox();
			this.textEntitiesPath = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.button1 = new BootFX.Common.UI.Desktop.DialogBar();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox1.Controls.Add(this.textServicesPath);
			this.groupBox1.Controls.Add(this.textEntitiesPath);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Location = new System.Drawing.Point(8, 8);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(404, 72);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Settings";
			// 
			// textServicesPath
			// 
			this.textServicesPath.Font = new System.Drawing.Font("Tahoma", 8F);
			this.textServicesPath.Location = new System.Drawing.Point(116, 44);
			this.textServicesPath.Name = "textServicesPath";
			this.textServicesPath.ReadOnly = true;
			this.textServicesPath.Size = new System.Drawing.Size(280, 20);
			this.textServicesPath.TabIndex = 3;
			this.textServicesPath.Tag = "servicesfolderpath";
			this.textServicesPath.Text = "";
			// 
			// textEntitiesPath
			// 
			this.textEntitiesPath.Location = new System.Drawing.Point(116, 20);
			this.textEntitiesPath.Name = "textEntitiesPath";
			this.textEntitiesPath.ReadOnly = true;
			this.textEntitiesPath.Size = new System.Drawing.Size(280, 20);
			this.textEntitiesPath.TabIndex = 2;
			this.textEntitiesPath.Tag = "folderpath";
			this.textEntitiesPath.Text = "";
			// 
			// label2
			// 
			this.label2.Font = new System.Drawing.Font("Tahoma", 8F);
			this.label2.Location = new System.Drawing.Point(12, 44);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(100, 20);
			this.label2.TabIndex = 1;
			this.label2.Text = "&Web Services path:";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(12, 20);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(80, 20);
			this.label1.TabIndex = 0;
			this.label1.Text = "&Entities path:";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// button1
			// 
			this.button1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.button1.Location = new System.Drawing.Point(0, 84);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(420, 32);
			this.button1.TabIndex = 1;
			// 
			// GenerateDialog
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(420, 116);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.groupBox1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "GenerateDialog";
			this.Text = "Generate Code";
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion
	}
}
