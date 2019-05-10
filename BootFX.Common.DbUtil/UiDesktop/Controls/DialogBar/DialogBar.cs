// BootFX - Application framework for .NET applications
// 
// File: DialogBar.cs
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
using BootFX.Common.UI.Common;

namespace BootFX.Common.UI.Desktop
{
	/// <summary>
	/// Summary description for DialogBar.
	/// </summary>
	public class DialogBar : UserControl
	{
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Button buttonOK;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public DialogBar()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			this.Dock = DockStyle.Bottom;
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
			this.buttonCancel = new System.Windows.Forms.Button();
			this.buttonOK = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// buttonCancel
			// 
			this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonCancel.Location = new System.Drawing.Point(404, 4);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.TabIndex = 0;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
			// 
			// buttonOK
			// 
			this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonOK.Location = new System.Drawing.Point(324, 4);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.TabIndex = 1;
			this.buttonOK.Text = "OK";
			this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
			// 
			// DialogBar
			// 
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.buttonCancel);
			this.Name = "DialogBar";
			this.Size = new System.Drawing.Size(484, 40);
			this.ResumeLayout(false);

		}
		#endregion

		private void buttonCancel_Click(object sender, System.EventArgs e)
		{
			if(Dialog == null)
				throw new InvalidOperationException("Dialog is null.");
			this.Dialog.DialogCancel();
		}

		private void buttonOK_Click(object sender, System.EventArgs e)
		{
			if(Dialog == null)
				throw new InvalidOperationException("Dialog is null.");

			try
			{
				this.Dialog.DialogOK();
			}
			catch(Exception ex)
			{
				Alert.ShowWarning(this, "Failed to apply changes.", ex);
			}
		}

		/// <summary>
		/// Gets the dialog.
		/// </summary>
		private IDialog Dialog
		{
			get
			{
				Control parent = this.Parent;
				while(parent != null)
				{
					if(parent is IDialog)
						return (IDialog)parent;
					parent = parent.Parent;
				}

				// nope...
				throw new InvalidOperationException("Failed to find dialog.");
			}
		}

		/// <summary>
		/// Gets the owner dialog as a form.
		/// </summary>
		private Form DialogAsForm
		{
			get
			{
				return this.Dialog as Form;
			}
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad (e);
			if(this.DesignMode)
				return;

			// assign...
			if(this.DialogAsForm != null)
			{
				this.DialogAsForm.AcceptButton = this.buttonOK;
				this.DialogAsForm.CancelButton = this.buttonCancel;
			}
		}
	}
}
