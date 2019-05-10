// BootFX - Application framework for .NET applications
// 
// File: AboutDialog.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Text;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace BootFX.Common.UI.Desktop
{
	/// <summary>
	/// Defines the built-in About box.
	/// </summary>
	public class AboutDialog : BaseForm
	{
		private System.Windows.Forms.Button buttonSystemInformation;
		private System.Windows.Forms.Button buttonClose;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label labelName;
		private System.Windows.Forms.Label labelCompany;
		private System.Windows.Forms.Label labelVersion;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textCopyright;
		private System.Windows.Forms.PictureBox pictureIcon;
		private System.Windows.Forms.LinkLabel linkSupportUrl;
		private System.Windows.Forms.Button buttonViewLogs;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public AboutDialog()
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
			this.buttonSystemInformation = new System.Windows.Forms.Button();
			this.buttonClose = new System.Windows.Forms.Button();
			this.labelName = new System.Windows.Forms.Label();
			this.labelCompany = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.linkSupportUrl = new System.Windows.Forms.LinkLabel();
			this.pictureIcon = new System.Windows.Forms.PictureBox();
			this.textCopyright = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.labelVersion = new System.Windows.Forms.Label();
			this.buttonViewLogs = new System.Windows.Forms.Button();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// buttonSystemInformation
			// 
			this.buttonSystemInformation.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonSystemInformation.Location = new System.Drawing.Point(8, 312);
			this.buttonSystemInformation.Name = "buttonSystemInformation";
			this.buttonSystemInformation.Size = new System.Drawing.Size(148, 23);
			this.buttonSystemInformation.TabIndex = 0;
			this.buttonSystemInformation.Text = "&System Information...";
			this.buttonSystemInformation.Click += new System.EventHandler(this.buttonSystemInformation_Click);
			// 
			// buttonClose
			// 
			this.buttonClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonClose.Location = new System.Drawing.Point(380, 312);
			this.buttonClose.Name = "buttonClose";
			this.buttonClose.TabIndex = 1;
			this.buttonClose.Text = "Close";
			// 
			// labelName
			// 
			this.labelName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.labelName.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold);
			this.labelName.Location = new System.Drawing.Point(48, 20);
			this.labelName.Name = "labelName";
			this.labelName.Size = new System.Drawing.Size(392, 28);
			this.labelName.TabIndex = 2;
			this.labelName.Text = "Lorem Ipsum";
			// 
			// labelCompany
			// 
			this.labelCompany.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.labelCompany.Location = new System.Drawing.Point(48, 44);
			this.labelCompany.Name = "labelCompany";
			this.labelCompany.Size = new System.Drawing.Size(392, 20);
			this.labelCompany.TabIndex = 0;
			this.labelCompany.Text = "Lorem Ipsum";
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox1.Controls.Add(this.linkSupportUrl);
			this.groupBox1.Controls.Add(this.pictureIcon);
			this.groupBox1.Controls.Add(this.textCopyright);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Controls.Add(this.labelVersion);
			this.groupBox1.Controls.Add(this.labelCompany);
			this.groupBox1.Controls.Add(this.labelName);
			this.groupBox1.Location = new System.Drawing.Point(8, 8);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(448, 296);
			this.groupBox1.TabIndex = 3;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Product information";
			// 
			// linkSupportUrl
			// 
			this.linkSupportUrl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.linkSupportUrl.Location = new System.Drawing.Point(48, 80);
			this.linkSupportUrl.Name = "linkSupportUrl";
			this.linkSupportUrl.Size = new System.Drawing.Size(392, 20);
			this.linkSupportUrl.TabIndex = 7;
			this.linkSupportUrl.TabStop = true;
			this.linkSupportUrl.Text = "http://www.mbrit.com/";
			this.linkSupportUrl.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkSupportUrl_LinkClicked);
			// 
			// pictureIcon
			// 
			this.pictureIcon.Location = new System.Drawing.Point(12, 20);
			this.pictureIcon.Name = "pictureIcon";
			this.pictureIcon.Size = new System.Drawing.Size(32, 32);
			this.pictureIcon.TabIndex = 6;
			this.pictureIcon.TabStop = false;
			this.pictureIcon.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureIcon_Paint);
			// 
			// textCopyright
			// 
			this.textCopyright.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.textCopyright.Location = new System.Drawing.Point(12, 120);
			this.textCopyright.Multiline = true;
			this.textCopyright.Name = "textCopyright";
			this.textCopyright.ReadOnly = true;
			this.textCopyright.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.textCopyright.Size = new System.Drawing.Size(428, 168);
			this.textCopyright.TabIndex = 5;
			this.textCopyright.Text = "";
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.label1.Location = new System.Drawing.Point(12, 104);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(428, 20);
			this.label1.TabIndex = 4;
			this.label1.Text = "&Notices: (scroll down to view complete notice)";
			// 
			// labelVersion
			// 
			this.labelVersion.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.labelVersion.Location = new System.Drawing.Point(48, 60);
			this.labelVersion.Name = "labelVersion";
			this.labelVersion.Size = new System.Drawing.Size(392, 20);
			this.labelVersion.TabIndex = 3;
			this.labelVersion.Text = "Version 1.0, build 100";
			// 
			// buttonViewLogs
			// 
			this.buttonViewLogs.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonViewLogs.Location = new System.Drawing.Point(164, 312);
			this.buttonViewLogs.Name = "buttonViewLogs";
			this.buttonViewLogs.Size = new System.Drawing.Size(148, 23);
			this.buttonViewLogs.TabIndex = 4;
			this.buttonViewLogs.Text = "&View Application Log Files...";
			this.buttonViewLogs.Click += new System.EventHandler(this.buttonViewLogs_Click);
			// 
			// AboutDialog
			// 
			this.AcceptButton = this.buttonClose;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.buttonClose;
			this.ClientSize = new System.Drawing.Size(462, 344);
			this.Controls.Add(this.buttonViewLogs);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.buttonClose);
			this.Controls.Add(this.buttonSystemInformation);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "AboutDialog";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "About";
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void buttonSystemInformation_Click(object sender, System.EventArgs e)
		{
            throw new NotImplementedException("This operation has not been implemented.");
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad (e);
			if(this.DesignMode)
				return;

			// set...
			if(Runtime.IsStarted)
			{
				// app...
				MbrApplication app = Runtime.Current.Application;
				this.labelName.Text = app.ProductName;
				this.labelCompany.Text = app.ProductCompany;
				this.textCopyright.Text = app.CopyrightNotice;

				// mbr - 14-04-2008 - changed this...
//				StringBuilder builder = new StringBuilder();
//				builder.Append("Version ");
//				builder.Append(app.ProductVersion.Major);
//				builder.Append(".");
//				builder.Append(app.ProductVersion.Minor);
//				builder.Append(", build ");
//				builder.Append(app.ProductVersion.Build);
//				if(app.ProductVersion.Revision != 0)
//				{
//					builder.Append(" revision ");
//					builder.Append(app.ProductVersion.Revision);
//				}
//				this.labelVersion.Text = builder.ToString();
				this.labelVersion.Text = app.ProductVersion.ToString();

				// url...
				if(app.ProductSupportUrl != null && app.ProductSupportUrl.Length > 0)
					this.linkSupportUrl.Text = app.ProductSupportUrl;
				else
					this.linkSupportUrl.Text = string.Empty;
			}
		}

		private void pictureIcon_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			e.Graphics.DrawIconUnstretched(DesktopRuntime.Current.DefaultFormIcon, this.pictureIcon.ClientRectangle);
		}

		private void linkSupportUrl_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
		{
			string url = this.linkSupportUrl.Text;
			if(url != null && url.Length > 0)
				System.Diagnostics.Process.Start(url);
		}

		private void buttonViewLogs_Click(object sender, System.EventArgs e)
		{
			try
			{
				System.Diagnostics.Process.Start(Runtime.Current.LogsFolderPath);
			}
			catch(Exception ex)
			{
				Alert.ShowWarning(this, "An error occurred when opening the logs folder.", ex);
			}
		}
	}
}
