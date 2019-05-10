// BootFX - Application framework for .NET applications
// 
// File: CheckoutDialog.cs
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

namespace BootFX.Common.DbUtil
{
	/// <summary>
	/// Summary description for CheckoutDialog.
	/// </summary>
	internal class CheckoutDialog : BaseForm
	{
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ListBox listFiles;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Button buttonRetry;
		private System.Windows.Forms.Button buttonDiff;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		internal CheckoutDialog()
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
			this.label1 = new System.Windows.Forms.Label();
			this.listFiles = new System.Windows.Forms.ListBox();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.buttonRetry = new System.Windows.Forms.Button();
			this.buttonDiff = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.label1.Location = new System.Drawing.Point(4, 8);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(804, 16);
			this.label1.TabIndex = 0;
			this.label1.Text = "These &files need to be checked out so that they can be replaced:";
			// 
			// listFiles
			// 
			this.listFiles.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.listFiles.IntegralHeight = false;
			this.listFiles.Location = new System.Drawing.Point(8, 24);
			this.listFiles.Name = "listFiles";
			this.listFiles.Size = new System.Drawing.Size(788, 348);
			this.listFiles.TabIndex = 1;
			this.listFiles.DoubleClick += new System.EventHandler(this.listFiles_DoubleClick);
			// 
			// buttonCancel
			// 
			this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(724, 380);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.TabIndex = 2;
			this.buttonCancel.Text = "Cancel";
			// 
			// buttonRetry
			// 
			this.buttonRetry.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonRetry.Location = new System.Drawing.Point(644, 380);
			this.buttonRetry.Name = "buttonRetry";
			this.buttonRetry.TabIndex = 3;
			this.buttonRetry.Text = "&Retry";
			this.buttonRetry.Click += new System.EventHandler(this.buttonRetry_Click);
			// 
			// buttonDiff
			// 
			this.buttonDiff.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonDiff.Location = new System.Drawing.Point(8, 380);
			this.buttonDiff.Name = "buttonDiff";
			this.buttonDiff.TabIndex = 4;
			this.buttonDiff.Text = "&Diff...";
			this.buttonDiff.Click += new System.EventHandler(this.buttonDiff_Click);
			// 
			// CheckoutDialog
			// 
			this.AcceptButton = this.buttonRetry;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(808, 410);
			this.Controls.Add(this.buttonDiff);
			this.Controls.Add(this.buttonRetry);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.listFiles);
			this.Controls.Add(this.label1);
			this.MinimizeBox = false;
			this.Name = "CheckoutDialog";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Checkout Required";
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// Sets the files.
		/// </summary>
		/// <param name="files"></param>
		internal void SetFiles(string[][] files)
		{
			if(files == null)
				throw new ArgumentNullException("files");
			
			// walk...
			this.listFiles.Items.Clear();
			foreach(string[] file in files)
				this.listFiles.Items.Add(new CheckoutFileListItem(file));
		}

		private void buttonRetry_Click(object sender, System.EventArgs e)
		{
			this.DialogResult = DialogResult.Retry;
		}

		private void buttonDiff_Click(object sender, System.EventArgs e)
		{
			this.ShowDiff();
		}

		private void ShowDiff()
		{
			// selected...
			CheckoutFileListItem selected = this.SelectedFile;
			if(selected == null)
			{
				Alert.ShowWarning(this, "You must select a file.");
				return;
			}

			// get...
			string path = Settings.SgdmPath;
			if(File.Exists(path))
			{
				// run...
				System.Diagnostics.Process.Start(path, string.Format("\"{0}\" \"{1}\"", selected.TempPath, selected.TargetPath));
			}
			else
			{
				Alert.ShowWarning(this, string.Format("SourceGear DiffMerge was not found at '{0}'.  This must be installed to use the 'Diff' functionality.", path));
			}
		}

		private void listFiles_DoubleClick(object sender, System.EventArgs e)
		{
			ShowDiff();
		}

		private CheckoutFileListItem SelectedFile
		{
			get
			{
				return (CheckoutFileListItem)this.listFiles.SelectedItem;
			}
		}
	}
}
