// BootFX - Application framework for .NET applications
// 
// File: FloatingOperationDialogBar.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace BootFX.Common.UI.Desktop
{
	/// <summary>
	/// Summary description for FloatingOperationDialogBar.
	/// </summary>
	internal class FloatingOperationDialogBar : System.Windows.Forms.UserControl
	{
		/// <summary>
		/// Private field to support <see cref="Item"/> property.
		/// </summary>
		private IOperationItem _item;
		
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.ProgressBar progress;
		private System.Windows.Forms.Panel panelCancel;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Panel panel3;
		private System.Windows.Forms.Label labelStatus;
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public FloatingOperationDialogBar()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitializeComponent call

		}
 
		public FloatingOperationDialogBar(IOperationItem item) : this()
		{
			if(item == null)
				throw new ArgumentNullException("item");
			
			// set...
			_item = item;
			this.RefreshView();
		}

		/// <summary>
		/// Gets the item.
		/// </summary>
		public IOperationItem Item
		{
			get
			{
				return _item;
			}
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

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.panel1 = new System.Windows.Forms.Panel();
			this.panel2 = new System.Windows.Forms.Panel();
			this.progress = new System.Windows.Forms.ProgressBar();
			this.panelCancel = new System.Windows.Forms.Panel();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.panel3 = new System.Windows.Forms.Panel();
			this.labelStatus = new System.Windows.Forms.Label();
			this.panel1.SuspendLayout();
			this.panel2.SuspendLayout();
			this.panelCancel.SuspendLayout();
			this.panel3.SuspendLayout();
			this.SuspendLayout();
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.panel2);
			this.panel1.Controls.Add(this.panelCancel);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panel1.Location = new System.Drawing.Point(0, 20);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(312, 24);
			this.panel1.TabIndex = 0;
			// 
			// panel2
			// 
			this.panel2.Controls.Add(this.progress);
			this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel2.DockPadding.All = 3;
			this.panel2.Location = new System.Drawing.Point(0, 0);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(236, 24);
			this.panel2.TabIndex = 0;
			// 
			// progress
			// 
			this.progress.Dock = System.Windows.Forms.DockStyle.Fill;
			this.progress.Location = new System.Drawing.Point(3, 3);
			this.progress.Name = "progress";
			this.progress.Size = new System.Drawing.Size(230, 18);
			this.progress.TabIndex = 0;
			// 
			// panelCancel
			// 
			this.panelCancel.Controls.Add(this.buttonCancel);
			this.panelCancel.Dock = System.Windows.Forms.DockStyle.Right;
			this.panelCancel.DockPadding.Left = 3;
			this.panelCancel.Location = new System.Drawing.Point(236, 0);
			this.panelCancel.Name = "panelCancel";
			this.panelCancel.Size = new System.Drawing.Size(76, 24);
			this.panelCancel.TabIndex = 1;
			// 
			// buttonCancel
			// 
			this.buttonCancel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.buttonCancel.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.buttonCancel.Location = new System.Drawing.Point(3, 0);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(73, 24);
			this.buttonCancel.TabIndex = 0;
			this.buttonCancel.Text = "Cancel";
			// 
			// panel3
			// 
			this.panel3.Controls.Add(this.labelStatus);
			this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel3.Location = new System.Drawing.Point(0, 0);
			this.panel3.Name = "panel3";
			this.panel3.Size = new System.Drawing.Size(312, 20);
			this.panel3.TabIndex = 1;
			// 
			// labelStatus
			// 
			this.labelStatus.Dock = System.Windows.Forms.DockStyle.Fill;
			this.labelStatus.Location = new System.Drawing.Point(0, 0);
			this.labelStatus.Name = "labelStatus";
			this.labelStatus.Size = new System.Drawing.Size(312, 20);
			this.labelStatus.TabIndex = 0;
			this.labelStatus.Text = "Working, please wait...";
			this.labelStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// FloatingOperationDialogBar
			// 
			this.Controls.Add(this.panel3);
			this.Controls.Add(this.panel1);
			this.Name = "FloatingOperationDialogBar";
			this.Size = new System.Drawing.Size(312, 44);
			this.panel1.ResumeLayout(false);
			this.panel2.ResumeLayout(false);
			this.panelCancel.ResumeLayout(false);
			this.panel3.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// Updates the view.
		/// </summary>
		internal void RefreshView()
		{
			if(this.Item != null)
			{
				this.labelStatus.Text = this.Item.Status;
				this.progress.Maximum = this.Item.ProgressMaximum;
				this.progress.Minimum = this.Item.ProgressMinimum;
				this.progress.Value  = this.Item.ProgressValue;
				this.progress.Enabled = true;
			}
			else
			{
				this.labelStatus.Text = string.Empty;
				this.progress.Enabled = false;
			}
		}

		/// <summary>
		/// Gets or sets the status.
		/// </summary>
		public string Status
		{
			get
			{
				return this.labelStatus.Text;
			}
			set
			{
				this.labelStatus.Text = value;
			}
		}
	}
}
