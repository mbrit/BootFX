// BootFX - Application framework for .NET applications
// 
// File: ConnectionStringDialog.cs
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
	/// Summary description for GenericDatabaseOptionsDialog.
	/// </summary>
	internal class ConnectionStringDialog : BaseForm
	{
		/// <summary>
		/// Private field to support <c>ConnectionStringBuilder</c> property.
		/// </summary>
		private ConnectionStringBuilder _connectionStringBuilder;
		
		/// <summary>
		/// Private field to support <c>ConnectionType</c> property.
		/// </summary>
		private Type _connectionType;
		private DialogBar panel1;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textServerName;
		private System.Windows.Forms.TextBox textDatabaseName;
		private System.Windows.Forms.CheckBox checkIntegratedSecurity;
		private System.Windows.Forms.TextBox textUsername;
		private System.Windows.Forms.Label labelUsername;
		private System.Windows.Forms.TextBox textPassword;
		private System.Windows.Forms.Button buttonTest;
		private System.Windows.Forms.Label labelPassword;
		private System.Windows.Forms.Button buttonBrowse;
		private System.Windows.Forms.Label labelDatabaseName;
		
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public ConnectionStringDialog()
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
			this.panel1 = new BootFX.Common.UI.Desktop.DialogBar();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.buttonBrowse = new System.Windows.Forms.Button();
			this.buttonTest = new System.Windows.Forms.Button();
			this.textPassword = new System.Windows.Forms.TextBox();
			this.labelPassword = new System.Windows.Forms.Label();
			this.textUsername = new System.Windows.Forms.TextBox();
			this.labelUsername = new System.Windows.Forms.Label();
			this.checkIntegratedSecurity = new System.Windows.Forms.CheckBox();
			this.textDatabaseName = new System.Windows.Forms.TextBox();
			this.labelDatabaseName = new System.Windows.Forms.Label();
			this.textServerName = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// panel1
			// 
			this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panel1.Location = new System.Drawing.Point(0, 184);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(348, 32);
			this.panel1.TabIndex = 0;
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox1.Controls.Add(this.buttonBrowse);
			this.groupBox1.Controls.Add(this.buttonTest);
			this.groupBox1.Controls.Add(this.textPassword);
			this.groupBox1.Controls.Add(this.labelPassword);
			this.groupBox1.Controls.Add(this.textUsername);
			this.groupBox1.Controls.Add(this.labelUsername);
			this.groupBox1.Controls.Add(this.checkIntegratedSecurity);
			this.groupBox1.Controls.Add(this.textDatabaseName);
			this.groupBox1.Controls.Add(this.labelDatabaseName);
			this.groupBox1.Controls.Add(this.textServerName);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Location = new System.Drawing.Point(8, 8);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(332, 170);
			this.groupBox1.TabIndex = 1;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Settings:";
			// 
			// buttonBrowse
			// 
			this.buttonBrowse.Location = new System.Drawing.Point(300, 112);
			this.buttonBrowse.Name = "buttonBrowse";
			this.buttonBrowse.Size = new System.Drawing.Size(24, 20);
			this.buttonBrowse.TabIndex = 10;
			this.buttonBrowse.Text = "&...";
			this.buttonBrowse.Click += new System.EventHandler(this.buttonBrowse_Click);
			// 
			// buttonTest
			// 
			this.buttonTest.Location = new System.Drawing.Point(248, 140);
			this.buttonTest.Name = "buttonTest";
			this.buttonTest.Size = new System.Drawing.Size(76, 23);
			this.buttonTest.TabIndex = 9;
			this.buttonTest.Text = "&Test";
			this.buttonTest.Click += new System.EventHandler(this.buttonTest_Click);
			// 
			// textPassword
			// 
			this.textPassword.Enabled = false;
			this.textPassword.Location = new System.Drawing.Point(104, 88);
			this.textPassword.Name = "textPassword";
			this.textPassword.PasswordChar = '*';
			this.textPassword.Size = new System.Drawing.Size(220, 20);
			this.textPassword.TabIndex = 8;
			this.textPassword.Text = "";
			// 
			// labelPassword
			// 
			this.labelPassword.Enabled = false;
			this.labelPassword.Location = new System.Drawing.Point(12, 88);
			this.labelPassword.Name = "labelPassword";
			this.labelPassword.Size = new System.Drawing.Size(96, 20);
			this.labelPassword.TabIndex = 7;
			this.labelPassword.Text = "&Password:";
			this.labelPassword.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textUsername
			// 
			this.textUsername.Enabled = false;
			this.textUsername.Location = new System.Drawing.Point(104, 64);
			this.textUsername.Name = "textUsername";
			this.textUsername.Size = new System.Drawing.Size(220, 20);
			this.textUsername.TabIndex = 6;
			this.textUsername.Text = "";
			// 
			// labelUsername
			// 
			this.labelUsername.Enabled = false;
			this.labelUsername.Location = new System.Drawing.Point(12, 64);
			this.labelUsername.Name = "labelUsername";
			this.labelUsername.Size = new System.Drawing.Size(96, 20);
			this.labelUsername.TabIndex = 5;
			this.labelUsername.Text = "U&sername:";
			this.labelUsername.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// checkIntegratedSecurity
			// 
			this.checkIntegratedSecurity.Checked = true;
			this.checkIntegratedSecurity.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkIntegratedSecurity.Location = new System.Drawing.Point(12, 44);
			this.checkIntegratedSecurity.Name = "checkIntegratedSecurity";
			this.checkIntegratedSecurity.Size = new System.Drawing.Size(220, 20);
			this.checkIntegratedSecurity.TabIndex = 4;
			this.checkIntegratedSecurity.Text = "&Use integrated security";
			this.checkIntegratedSecurity.CheckedChanged += new System.EventHandler(this.checkIntegratedSecurity_CheckedChanged);
			// 
			// textDatabaseName
			// 
			this.textDatabaseName.Location = new System.Drawing.Point(104, 112);
			this.textDatabaseName.Name = "textDatabaseName";
			this.textDatabaseName.Size = new System.Drawing.Size(192, 20);
			this.textDatabaseName.TabIndex = 3;
			this.textDatabaseName.Text = "";
			// 
			// labelDatabaseName
			// 
			this.labelDatabaseName.Location = new System.Drawing.Point(12, 112);
			this.labelDatabaseName.Name = "labelDatabaseName";
			this.labelDatabaseName.Size = new System.Drawing.Size(96, 20);
			this.labelDatabaseName.TabIndex = 2;
			this.labelDatabaseName.Text = "&Database name:";
			this.labelDatabaseName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textServerName
			// 
			this.textServerName.Location = new System.Drawing.Point(104, 20);
			this.textServerName.Name = "textServerName";
			this.textServerName.Size = new System.Drawing.Size(220, 20);
			this.textServerName.TabIndex = 1;
			this.textServerName.Text = "";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(12, 20);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(96, 20);
			this.label1.TabIndex = 0;
			this.label1.Text = "&Server name:";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// ConnectionStringDialog
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(348, 216);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.panel1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ConnectionStringDialog";
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Build Connection String";
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void checkIntegratedSecurity_CheckedChanged(object sender, System.EventArgs e)
		{
			this.labelUsername.Enabled = !(this.checkIntegratedSecurity.Checked);
			this.textUsername.Enabled = !(this.checkIntegratedSecurity.Checked);
			this.labelPassword.Enabled = !(this.checkIntegratedSecurity.Checked);
			this.textPassword.Enabled = !(this.checkIntegratedSecurity.Checked);
		}

		private void buttonTest_Click(object sender, System.EventArgs e)
		{
			try
			{
				Test();
			}
			catch (Exception ex)
			{
				Alert.ShowWarning(this, "Failed to build connection string and/or test connection.", ex);
			}
		}

		/// <summary>
		/// Tests the connection string.
		/// </summary>
		private void Test()
		{
			string connectionString = this.GetConnectionString();
			if(connectionString == null)
				throw new InvalidOperationException("'connectionString' is null.");
			if(connectionString.Length == 0)
				throw new InvalidOperationException("'connectionString' is zero-length.");
			this.Test(connectionString, false);
		}

		/// <summary>
		/// Tests the connection string.
		/// </summary>
		private void Test(string connString, bool quiet)
		{
			if(connString == null)
				throw new ArgumentNullException("connString");
			if(connString.Length == 0)
				throw new ArgumentOutOfRangeException("'connString' is zero-length.");

			// try...
			Cursor old = this.Cursor;
			this.Cursor = Cursors.WaitCursor;
			try
			{
				Connection.TestConnection(new ConnectionSettings("Database", this.ConnectionType, connString));
			}
			finally
			{
				this.Cursor = old;
			}

			// ok...
			if(!(quiet))
				Alert.ShowInformation(this, "The connection was OK.");
		}

		/// <summary>
		/// Gets the connection string.
		/// </summary>
		/// <returns></returns>
		internal string GetConnectionString()
		{
			return this.GetConnectionString(false);
		}

		/// <summary>
		/// Gets the connection string.
		/// </summary>
		/// <returns></returns>
		private string GetConnectionString(bool master)
		{
			if(ConnectionStringBuilder == null)
				throw new InvalidOperationException("ConnectionStringBuilder is null.");
			
			// try...
			string name = this.textDatabaseName.Text.Trim();
			if(master)
				name = "master";
			return this.ConnectionStringBuilder.GetConnectionString(this.textServerName.Text.Trim(), name, 
				this.checkIntegratedSecurity.Checked, this.textUsername.Text.Trim(), this.textPassword.Text.Trim());
		}

		private void buttonBrowse_Click(object sender, System.EventArgs e)
		{
			string connString = this.GetConnectionString(true);
			if(connString == null)
				throw new InvalidOperationException("'connString' is null.");
			if(connString.Length == 0)
				throw new InvalidOperationException("'connString' is zero-length.");

			// test...
			try
			{
				this.Test(connString, true);
			}
			catch(Exception ex)
			{
				Alert.ShowWarning(this, "A connection to the database could not be made.", ex);
				return;
			}

			// ok...
			using(PickCatalogDialog dialog = new PickCatalogDialog())
			{
				dialog.ConnectionType = this.ConnectionType;
				dialog.ConnectionString = connString;

				// show...
				if(dialog.ShowDialog(this) == DialogResult.OK)
					this.textDatabaseName.Text = dialog.CatalogName;
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

					// check...
					if(value != null)
					{
						ConnectionStringBuilderAttribute[] attrs = (ConnectionStringBuilderAttribute[])ConnectionType.GetCustomAttributes(typeof(ConnectionStringBuilderAttribute), false);
						if(attrs.Length == 0)
							throw new InvalidOperationException(string.Format("Connection type '{0}' does not have an available connection string builder.", _connectionType));

						// set...
						_connectionStringBuilder = (ConnectionStringBuilder)Activator.CreateInstance(attrs[0].Type);
					}
					else
						_connectionStringBuilder = null;
				}
			}
		}

		/// <summary>
		/// Gets the connectionstringbuilder.
		/// </summary>
		private ConnectionStringBuilder ConnectionStringBuilder
		{
			get
			{
				// returns the value...
				return _connectionStringBuilder;
			}
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad (e);
			try
			{
				bool enabled = true;
				if(!(this.ConnectionStringBuilder.SupportsNamedDatabases))
					enabled = false;

				// set...
				this.labelDatabaseName.Enabled = enabled;
				this.textDatabaseName.Enabled = enabled;
				this.buttonBrowse.Enabled = enabled;
			}
			catch(Exception ex)
			{
				Alert.ShowWarning(this, "The view failed to initialize.", ex);
			}
		}
	}
}
