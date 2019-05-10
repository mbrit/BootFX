// BootFX - Application framework for .NET applications
// 
// File: FolderViewDialog.cs
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
using BootFX.Common;
using BootFX.Common.UI.Desktop;

namespace BootFX.Common.DbUtil
{
	/// <summary>
	/// Summary description for FolderViewDialog.
	/// </summary>
	public class FolderViewDialog : System.Windows.Forms.Form
	{
		/// <summary>
		/// Private field to support <c>FolderPath</c> property.
		/// </summary>
		private string _folderPath;

		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Splitter splitter1;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.ListBox listFiles;
		private System.Windows.Forms.RichTextBox textFile;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public FolderViewDialog()
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
			this.panel1 = new System.Windows.Forms.Panel();
			this.splitter1 = new System.Windows.Forms.Splitter();
			this.panel2 = new System.Windows.Forms.Panel();
			this.listFiles = new System.Windows.Forms.ListBox();
			this.textFile = new System.Windows.Forms.RichTextBox();
			this.panel1.SuspendLayout();
			this.panel2.SuspendLayout();
			this.SuspendLayout();
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.listFiles);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
			this.panel1.DockPadding.Bottom = 4;
			this.panel1.DockPadding.Left = 4;
			this.panel1.DockPadding.Top = 4;
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(200, 410);
			this.panel1.TabIndex = 0;
			// 
			// splitter1
			// 
			this.splitter1.Location = new System.Drawing.Point(200, 0);
			this.splitter1.Name = "splitter1";
			this.splitter1.Size = new System.Drawing.Size(4, 410);
			this.splitter1.TabIndex = 1;
			this.splitter1.TabStop = false;
			// 
			// panel2
			// 
			this.panel2.Controls.Add(this.textFile);
			this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel2.DockPadding.Bottom = 4;
			this.panel2.DockPadding.Right = 4;
			this.panel2.DockPadding.Top = 4;
			this.panel2.Location = new System.Drawing.Point(204, 0);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(484, 410);
			this.panel2.TabIndex = 2;
			// 
			// listFiles
			// 
			this.listFiles.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listFiles.IntegralHeight = false;
			this.listFiles.Location = new System.Drawing.Point(4, 4);
			this.listFiles.Name = "listFiles";
			this.listFiles.Size = new System.Drawing.Size(196, 402);
			this.listFiles.TabIndex = 0;
			this.listFiles.SelectedIndexChanged += new System.EventHandler(this.listFiles_SelectedIndexChanged);
			// 
			// textFile
			// 
			this.textFile.Dock = System.Windows.Forms.DockStyle.Fill;
			this.textFile.Location = new System.Drawing.Point(0, 4);
			this.textFile.Name = "textFile";
			this.textFile.Size = new System.Drawing.Size(480, 402);
			this.textFile.TabIndex = 0;
			this.textFile.Text = "";
			// 
			// FolderViewDialog
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(688, 410);
			this.Controls.Add(this.panel2);
			this.Controls.Add(this.splitter1);
			this.Controls.Add(this.panel1);
			this.Name = "FolderViewDialog";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Folder";
			this.panel1.ResumeLayout(false);
			this.panel2.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void listFiles_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			try
			{
				// get
				string path = this.SelectedFilePath;
				if(path != null && path.Length > 0)
				{
					using(StreamReader reader = new StreamReader(path))
						this.textFile.Text = reader.ReadToEnd();
				}
				else
					this.textFile.Text = string.Empty;
			}
			catch (Exception ex)
			{
				Alert.ShowWarning(this, "Failed to handle selection change.", ex);
			}
		}
		
		/// <summary>
		/// Gets or sets the folderpath
		/// </summary>
		internal string FolderPath
		{
			get
			{
				return _folderPath;
			}
			set
			{
				if(value == null)
					throw new ArgumentNullException("value");
				if(value.Length == 0)
					throw new ArgumentOutOfRangeException("'value' is zero-length.");

				// check to see if the value has changed...
				if(value != _folderPath)
				{
					// set the value...
					_folderPath = value;

					// set...
					this.Text = "Folder - " + value;

					// clear...
					this.listFiles.Items.Clear();
					foreach(FileInfo file in new DirectoryInfo(value).GetFiles())
						this.listFiles.Items.Add(file.Name);
				}
			}
		}

		private string SelectedFilePath
		{
			get
			{
				if(this.listFiles.SelectedIndex != -1)
					return Path.Combine(this.FolderPath, (string)this.listFiles.Items[this.listFiles.SelectedIndex]);
				else
					return null;
			}
		}
	}
}
