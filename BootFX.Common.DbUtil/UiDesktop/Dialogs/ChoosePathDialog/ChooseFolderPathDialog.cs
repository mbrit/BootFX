// BootFX - Application framework for .NET applications
// 
// File: ChooseFolderPathDialog.cs
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

namespace BootFX.Common.UI.Desktop
{
	/// <summary>
	/// Summary description for ChoosePathDialog.
	/// </summary>
	public class ChooseFolderPathDialog : BaseForm
	{
		/// <summary>
		/// Private field to support <c>CreateIfNotFound</c> property.
		/// </summary>
		private bool _createIfNotFound = true;
		
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Button buttonOk;
		private System.Windows.Forms.TextBox textFolderPath;
		private System.Windows.Forms.Label labelCaption;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.Button buttonBrowse;
		private System.Windows.Forms.Button buttonOpen;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public ChooseFolderPathDialog()
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
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.textFolderPath = new System.Windows.Forms.TextBox();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.buttonOk = new System.Windows.Forms.Button();
			this.labelCaption = new System.Windows.Forms.Label();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.buttonBrowse = new System.Windows.Forms.Button();
			this.buttonOpen = new System.Windows.Forms.Button();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox1.Controls.Add(this.buttonOpen);
			this.groupBox1.Controls.Add(this.buttonBrowse);
			this.groupBox1.Controls.Add(this.textFolderPath);
			this.groupBox1.Location = new System.Drawing.Point(8, 48);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(400, 80);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "&Folder:";
			// 
			// textFolderPath
			// 
			this.textFolderPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.textFolderPath.Location = new System.Drawing.Point(12, 20);
			this.textFolderPath.Name = "textFolderPath";
			this.textFolderPath.Size = new System.Drawing.Size(380, 20);
			this.textFolderPath.TabIndex = 0;
			this.textFolderPath.Text = "";
			// 
			// buttonCancel
			// 
			this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(330, 134);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(78, 23);
			this.buttonCancel.TabIndex = 1;
			this.buttonCancel.Text = "Cancel";
			// 
			// buttonOk
			// 
			this.buttonOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonOk.Location = new System.Drawing.Point(246, 134);
			this.buttonOk.Name = "buttonOk";
			this.buttonOk.Size = new System.Drawing.Size(78, 23);
			this.buttonOk.TabIndex = 2;
			this.buttonOk.Text = "OK";
			this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
			// 
			// labelCaption
			// 
			this.labelCaption.Location = new System.Drawing.Point(48, 8);
			this.labelCaption.Name = "labelCaption";
			this.labelCaption.Size = new System.Drawing.Size(360, 32);
			this.labelCaption.TabIndex = 3;
			this.labelCaption.Text = "Please choose the folder that you want to use.";
			this.labelCaption.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// pictureBox1
			// 
			this.pictureBox1.Location = new System.Drawing.Point(8, 8);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(32, 32);
			this.pictureBox1.TabIndex = 4;
			this.pictureBox1.TabStop = false;
			this.pictureBox1.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBox1_Paint);
			// 
			// buttonBrowse
			// 
			this.buttonBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonBrowse.Location = new System.Drawing.Point(316, 48);
			this.buttonBrowse.Name = "buttonBrowse";
			this.buttonBrowse.Size = new System.Drawing.Size(76, 23);
			this.buttonBrowse.TabIndex = 1;
			this.buttonBrowse.Text = "&Browse >>";
			this.buttonBrowse.Click += new System.EventHandler(this.buttonBrowse_Click);
			// 
			// buttonOpen
			// 
			this.buttonOpen.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonOpen.Location = new System.Drawing.Point(232, 48);
			this.buttonOpen.Name = "buttonOpen";
			this.buttonOpen.Size = new System.Drawing.Size(76, 23);
			this.buttonOpen.TabIndex = 2;
			this.buttonOpen.Text = "&Open >>";
			this.buttonOpen.Visible = false;
			this.buttonOpen.Click += new System.EventHandler(this.buttonOpen_Click);
			// 
			// ChooseFolderPathDialog
			// 
			this.AcceptButton = this.buttonOk;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(414, 164);
			this.Controls.Add(this.pictureBox1);
			this.Controls.Add(this.labelCaption);
			this.Controls.Add(this.buttonOk);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.groupBox1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "ChooseFolderPathDialog";
			this.Text = "Choose Path";
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void buttonOk_Click(object sender, System.EventArgs e)
		{
			this.DialogOK();
		}

		/// <summary>
		/// Gets or sets the createifnotfound
		/// </summary>
		public bool CreateIfNotFound
		{
			get
			{
				return _createIfNotFound;
			}
			set
			{
				// check to see if the value has changed...
				if(value != _createIfNotFound)
				{
					// set the value...
					_createIfNotFound = value;
				}
			}
		}

		protected override bool DoApply(BootFX.Common.UI.Common.ApplyReason reason)
		{
			if(base.DoApply (reason) == false)
				return false;

			// check...
			if(this.FolderPath == null || this.FolderPath.Length == 0)
			{
				Alert.ShowWarning(this, "You must choose a folder.");
				return false;
			}

			// exists?
			if(Directory.Exists(this.FolderPath) == false)
			{
				if(this.CreateIfNotFound)
				{
					// ask...
					if(Alert.AskYesNoQuestion(this, "The folder does not exist.  Do you want to create it?") != DialogResult.Yes)
						return false;

					// create...
					try
					{
						Directory.CreateDirectory(this.FolderPath);
					}
					catch(Exception ex)
					{
						throw new InvalidOperationException(string.Format("The folder '{0}' could not be created.", this.FolderPath), ex);
					}
				}
				else
				{
					Alert.ShowWarning(this, "The folder does not exist.");
					return false;
				}
			}

			// ok...
			return true;
		}

		private void pictureBox1_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			e.Graphics.DrawIconUnstretched(SystemIcons.Question, this.pictureBox1.ClientRectangle);
		}

		private void buttonOpen_Click(object sender, System.EventArgs e)
		{
			Open();
		}

		/// <summary>
		/// Opens the selected folder.
		/// </summary>
		private void Open()
		{
			if(this.FolderPath == null || this.FolderPath.Length == 0)
			{
				Alert.ShowWarning(this, "You must select a folder.");
				return;
			}

			// exists?
			if(Directory.Exists(this.FolderPath) == false)
			{
				if(this.CreateIfNotFound)
					Alert.ShowInformation(this, "The folder does not exist.  You will have an opportunity to create it when you close the dialog.");
				else
					Alert.ShowWarning(this, "The folder does not exist.");

				// return...
				return;
			}

			// open...
			try
			{
				// show...
				System.Diagnostics.Process.Start("explorer.exe", this.FolderPath);
			}
			catch(Exception ex)
			{
				Alert.ShowWarning(this, string.Format("The folder '{0}' could not be opened.", this.FolderPath), ex);
			}
		}

		private void buttonBrowse_Click(object sender, System.EventArgs e)
		{
			Browse();
		}

		/// <summary>
		/// Browses for the folder.
		/// </summary>
		private void Browse()
		{
			FolderBrowserDialog dialog = new FolderBrowserDialog();
			dialog.Description = this.Caption;
			dialog.SelectedPath = this.textFolderPath.Text;
			if(dialog.ShowDialog(this) == DialogResult.OK)
				this.textFolderPath.Text = dialog.SelectedPath;
		}

		/// <summary>
		/// Gets or sets the folderpath
		/// </summary>
		public string FolderPath
		{
			get
			{
				return this.textFolderPath.Text.Trim();
			}
			set
			{
				// check to see if the value has changed...
				if(value != null)
					value = value.Trim();
				this.textFolderPath.Text = value;
			}
		}

		/// <summary>
		/// Gets or sets the Caption
		/// </summary>
		public string Caption
		{
			get
			{
				return this.labelCaption.Text;
			}
			set
			{
				// check to see if the value has changed...
				this.labelCaption.Text = value;
			}
		}

		/// <summary>
		/// Gets or sets whether to show the <c>Open</c> button.
		/// </summary>
		public bool OpenButtonVisible
		{
			get
			{
				return this.buttonOpen.Visible;
			}
			set
			{
				this.buttonOpen.Visible = value;
			}
		}
	}
}
