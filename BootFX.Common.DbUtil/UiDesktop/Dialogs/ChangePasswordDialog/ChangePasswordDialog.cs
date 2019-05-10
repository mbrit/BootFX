// BootFX - Application framework for .NET applications
// 
// File: ChangePasswordDialog.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Text;
using System.Security.Cryptography;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace BootFX.Common.UI.Desktop
{
	/// <summary>
	/// Summary description for ChangePasswordDialog.
	/// </summary>
	public class ChangePasswordDialog : BaseForm
	{
		/// <summary>
		/// Private field to support <c>ExistingPasswordHash</c> property.
		/// </summary>
		private string _existingPasswordHash;
		
		private BootFX.Common.UI.Desktop.DialogBar button1;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textCurrent;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox textNew;
		private System.Windows.Forms.TextBox textConfirm;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public ChangePasswordDialog()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			this.textCurrent.PasswordChar = DesktopRuntime.XpPasswordChar;
			this.textNew.PasswordChar = DesktopRuntime.XpPasswordChar;
			this.textConfirm.PasswordChar = DesktopRuntime.XpPasswordChar;
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
			this.label1 = new System.Windows.Forms.Label();
			this.textCurrent = new System.Windows.Forms.TextBox();
			this.textNew = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.textConfirm = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// button1
			// 
			this.button1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.button1.Location = new System.Drawing.Point(0, 108);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(334, 28);
			this.button1.TabIndex = 7;
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox1.Controls.Add(this.textConfirm);
			this.groupBox1.Controls.Add(this.label3);
			this.groupBox1.Controls.Add(this.textNew);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Controls.Add(this.textCurrent);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Location = new System.Drawing.Point(8, 8);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(322, 98);
			this.groupBox1.TabIndex = 8;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Password";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(12, 20);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(100, 20);
			this.label1.TabIndex = 0;
			this.label1.Text = "&Current password:";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textCurrent
			// 
			this.textCurrent.Location = new System.Drawing.Point(112, 20);
			this.textCurrent.Name = "textCurrent";
			this.textCurrent.PasswordChar = '*';
			this.textCurrent.Size = new System.Drawing.Size(200, 20);
			this.textCurrent.TabIndex = 1;
			this.textCurrent.Text = "";
			// 
			// textNew
			// 
			this.textNew.Location = new System.Drawing.Point(112, 44);
			this.textNew.Name = "textNew";
			this.textNew.PasswordChar = '*';
			this.textNew.Size = new System.Drawing.Size(200, 20);
			this.textNew.TabIndex = 3;
			this.textNew.Text = "";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(12, 44);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(100, 20);
			this.label2.TabIndex = 2;
			this.label2.Text = "&New password:";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textConfirm
			// 
			this.textConfirm.Location = new System.Drawing.Point(112, 68);
			this.textConfirm.Name = "textConfirm";
			this.textConfirm.PasswordChar = '*';
			this.textConfirm.Size = new System.Drawing.Size(200, 20);
			this.textConfirm.TabIndex = 5;
			this.textConfirm.Text = "";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(12, 68);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(100, 20);
			this.label3.TabIndex = 4;
			this.label3.Text = "Confirm &password:";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// ChangePasswordDialog
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(334, 136);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.button1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "ChangePasswordDialog";
			this.Text = "Change Password";
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// Gets or sets the existingpasswordhash
		/// </summary>
		public string ExistingPasswordHash
		{
			get
			{
				return _existingPasswordHash;
			}
			set
			{
				// check to see if the value has changed...
				if(value != _existingPasswordHash)
				{
					// set the value...
					_existingPasswordHash = value;
				}
			}
		}

		/// <summary>
		/// Gets the hash of the new password.
		/// </summary>
		public string NewPasswordHash
		{
			get
			{	
				if(this.textNew.Text != null && this.textNew.Text.Length > 0 && this.textConfirm.Text == this.textNew.Text)
					return this.GetHash(this.textNew.Text);
				else
					return null;
			}
		}

		/// <summary>
		/// Gets the hash of the given password.
		/// </summary>
		/// <param name="password"></param>
		/// <returns></returns>
		private string GetHash(string password)
		{
			if(password == null)
				password = string.Empty;

			// return...
			return Convert.ToBase64String(new MD5CryptoServiceProvider().ComputeHash(Encoding.Unicode.GetBytes(password)));
		}

		protected override bool DoApply(BootFX.Common.UI.Common.ApplyReason reason)
		{
			if(base.DoApply (reason) == false)
				return false;

			// check...
			if(this.GetHash(this.textCurrent.Text) != this.ExistingPasswordHash)
			{
				Alert.ShowWarning(this, "The current password you entered is invalid.");
				return false;
			}

			// anything?
			if(this.textNew.Text == null || this.textNew.Text.Length == 0)
			{
				Alert.ShowWarning(this, "You must enter a password.");
				return false;
			}

			// confirm...
			if(this.textNew.Text != this.textConfirm.Text)
			{
				Alert.ShowWarning(this, "The passwords you entered do not match.");
				return false;
			}

			// ok...
			return true;
		}
	}
}
