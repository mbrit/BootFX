// BootFX - Application framework for .NET applications
// 
// File: PickCatalogDialog.cs
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
	/// Summary description for PickCatalogDialog.
	/// </summary>
	public class PickCatalogDialog : BaseForm
	{
		/// <summary>
		/// Private field to support <c>ConnectionType</c> property.
		/// </summary>
		private Type _connectionType;

		/// <summary>
		/// Private field to support <c>ConnectionString</c> property.
		/// </summary>
		private string _connectionString;
		
		private DialogBar button1;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.ListBox listCatalogs;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public PickCatalogDialog()
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
			this.button1 = new BootFX.Common.UI.Desktop.DialogBar();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.listCatalogs = new System.Windows.Forms.ListBox();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// button1
			// 
			this.button1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.button1.Location = new System.Drawing.Point(0, 286);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(328, 32);
			this.button1.TabIndex = 0;
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox1.Controls.Add(this.listCatalogs);
			this.groupBox1.Location = new System.Drawing.Point(8, 8);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(314, 274);
			this.groupBox1.TabIndex = 1;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "&Databases:";
			// 
			// listCatalogs
			// 
			this.listCatalogs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.listCatalogs.IntegralHeight = false;
			this.listCatalogs.Location = new System.Drawing.Point(12, 20);
			this.listCatalogs.Name = "listCatalogs";
			this.listCatalogs.Size = new System.Drawing.Size(294, 246);
			this.listCatalogs.Sorted = true;
			this.listCatalogs.TabIndex = 0;
			this.listCatalogs.DoubleClick += new System.EventHandler(this.listCatalogs_DoubleClick);
			// 
			// PickCatalogDialog
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(328, 318);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.button1);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "PickCatalogDialog";
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Browse for Database";
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad (e);

			// check...
			if(ConnectionType == null)
				throw new InvalidOperationException("ConnectionType is null.");
			if(ConnectionString == null)
				throw new InvalidOperationException("'ConnectionString' is null.");
			if(ConnectionString.Length == 0)
				throw new InvalidOperationException("'ConnectionString' is zero-length.");

			Cursor old = this.Cursor;
			this.Cursor = Cursors.WaitCursor;
			try
			{
				// get...
				this.listCatalogs.Items.Clear();
				using(IConnection conn = Connection.CreateConnection(this.ConnectionType, this.ConnectionString))
				{
					// get...
					foreach(string catalog in conn.GetCatalogNames())
						this.listCatalogs.Items.Add(catalog);
				}
			}
			finally
			{
				this.Cursor = old;
			}
		}

		private void listCatalogs_DoubleClick(object sender, System.EventArgs e)
		{
			this.DialogOK();
		}

		/// <summary>
		/// Gets or sets the connectionstring
		/// </summary>
		internal string ConnectionString
		{
			get
			{
				return _connectionString;
			}
			set
			{
				// check to see if the value has changed...
				if(value != _connectionString)
				{
					// set the value...
					_connectionString = value;
				}
			}
		}
		
		/// <summary>
		/// Gets or sets the connectiontype
		/// </summary>
		internal Type ConnectionType
		{
			get
			{
				return _connectionType;
			}
			set
			{
				// check to see if the value has changed...
				if(value != _connectionType)
				{
					// set the value...
					_connectionType = value;
				}
			}
		}

		internal string CatalogName
		{
			get
			{
				if(this.listCatalogs.SelectedIndex != -1)
					return (string)this.listCatalogs.Items[this.listCatalogs.SelectedIndex];
				else
					return null;
			}
		}

		protected override bool DoApply(BootFX.Common.UI.Common.ApplyReason reason)
		{
			if(!(base.DoApply (reason)))
				return false;

			// check...
			if(this.CatalogName == null || this.CatalogName.Length == 0)
			{
				Alert.ShowWarning(this, "You must choose a database.");
				return false;
			}
			else
				return true;
		}
	}
}
