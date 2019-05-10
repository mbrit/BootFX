// BootFX - Application framework for .NET applications
// 
// File: ConnectToDatabaseDialog.cs
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
using BootFX.Common.Data;

namespace BootFX.Common.UI.Desktop
{
	/// <summary>
	/// Defines a dialog that can configure a database connection.
	/// </summary>
	public class ConnectToDatabaseDialog : BaseForm
	{
		/// <summary>
		/// Private field to support <c>NameRequired</c> property.
		/// </summary>
		private bool _nameRequired = true;
		
		/// <summary>
		/// Private field to support <c>ConnectionSettings</c> property.
		/// </summary>
		private ConnectionSettings _connectionSettings;
		
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Button buttonOk;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ComboBox listConnectionTypes;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox textConnectionString;
		private System.Windows.Forms.Button buttonTest;
		private System.Windows.Forms.TextBox textName;
		private System.Windows.Forms.Button buttonBuild;
		private System.Windows.Forms.Label labelName;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public ConnectToDatabaseDialog()
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
			this.buttonBuild = new System.Windows.Forms.Button();
			this.textName = new System.Windows.Forms.TextBox();
			this.labelName = new System.Windows.Forms.Label();
			this.buttonTest = new System.Windows.Forms.Button();
			this.textConnectionString = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.listConnectionTypes = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.buttonOk = new System.Windows.Forms.Button();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox1.Controls.Add(this.buttonBuild);
			this.groupBox1.Controls.Add(this.textName);
			this.groupBox1.Controls.Add(this.labelName);
			this.groupBox1.Controls.Add(this.buttonTest);
			this.groupBox1.Controls.Add(this.textConnectionString);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Controls.Add(this.listConnectionTypes);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Location = new System.Drawing.Point(8, 8);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(316, 280);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Settings";
			// 
			// buttonBuild
			// 
			this.buttonBuild.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonBuild.Location = new System.Drawing.Point(152, 248);
			this.buttonBuild.Name = "buttonBuild";
			this.buttonBuild.TabIndex = 7;
			this.buttonBuild.Text = "&Build...";
			this.buttonBuild.Click += new System.EventHandler(this.buttonBuild_Click);
			// 
			// textName
			// 
			this.textName.Location = new System.Drawing.Point(12, 36);
			this.textName.Name = "textName";
			this.textName.Size = new System.Drawing.Size(292, 20);
			this.textName.TabIndex = 6;
			this.textName.Text = "";
			// 
			// labelName
			// 
			this.labelName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.labelName.Location = new System.Drawing.Point(12, 20);
			this.labelName.Name = "labelName";
			this.labelName.Size = new System.Drawing.Size(292, 20);
			this.labelName.TabIndex = 5;
			this.labelName.Text = "Connection &name:";
			// 
			// buttonTest
			// 
			this.buttonTest.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonTest.Location = new System.Drawing.Point(232, 248);
			this.buttonTest.Name = "buttonTest";
			this.buttonTest.TabIndex = 4;
			this.buttonTest.Text = "&Test";
			this.buttonTest.Click += new System.EventHandler(this.buttonTest_Click);
			// 
			// textConnectionString
			// 
			this.textConnectionString.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.textConnectionString.Location = new System.Drawing.Point(12, 128);
			this.textConnectionString.Multiline = true;
			this.textConnectionString.Name = "textConnectionString";
			this.textConnectionString.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.textConnectionString.Size = new System.Drawing.Size(292, 112);
			this.textConnectionString.TabIndex = 3;
			this.textConnectionString.Text = "";
			// 
			// label2
			// 
			this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.label2.Location = new System.Drawing.Point(12, 112);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(296, 20);
			this.label2.TabIndex = 2;
			this.label2.Text = "&Connection string:";
			// 
			// listConnectionTypes
			// 
			this.listConnectionTypes.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.listConnectionTypes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.listConnectionTypes.Location = new System.Drawing.Point(12, 80);
			this.listConnectionTypes.Name = "listConnectionTypes";
			this.listConnectionTypes.Size = new System.Drawing.Size(292, 21);
			this.listConnectionTypes.Sorted = true;
			this.listConnectionTypes.TabIndex = 1;
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.label1.Location = new System.Drawing.Point(12, 64);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(292, 20);
			this.label1.TabIndex = 0;
			this.label1.Text = "Connection &type:";
			// 
			// buttonCancel
			// 
			this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(248, 296);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.TabIndex = 1;
			this.buttonCancel.Text = "Cancel";
			// 
			// buttonOk
			// 
			this.buttonOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonOk.Location = new System.Drawing.Point(164, 296);
			this.buttonOk.Name = "buttonOk";
			this.buttonOk.TabIndex = 2;
			this.buttonOk.Text = "OK";
			this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
			// 
			// ConnectToDatabaseDialog
			// 
			this.AcceptButton = this.buttonOk;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(332, 326);
			this.Controls.Add(this.buttonOk);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.groupBox1);
			this.MinimizeBox = false;
			this.Name = "ConnectToDatabaseDialog";
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Connect to Database";
			this.Load += new System.EventHandler(this.ConnectToDatabaseDialog_Load);
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void buttonTest_Click(object sender, System.EventArgs e)
		{
			try
			{
				Test();
			}
			catch (Exception ex)
			{
				Alert.ShowWarning(this, "A connection to the database could not be made using the parameters you supplied.", ex);
			}
		}

		/// <summary>
		/// Tests the connection.
		/// </summary>
		private void Test()
		{
			// apply...
			if(this.Apply() == true)
			{
				// check...
				TestInternal();

				// ok...
				Alert.ShowInformation(this, "Connection OK.");
			}
		}

		/// <summary>
		/// Physically tests the connection against the settings in <see cref="ConnectionSettings"></see>.
		/// </summary>
		private void TestInternal()
		{
			if(ConnectionSettings == null)
				throw new InvalidOperationException("ConnectionSettings is null.");

			// try...
			Connection.TestConnection(this.ConnectionSettings);
		}

		/// <summary>
		/// Applies dialog changes.
		/// </summary>
		/// <param name="reason"></param>
		/// <returns></returns>
		protected override bool DoApply(ApplyReason reason)
		{
			if(base.DoApply(reason) == false)
				return false;

			// name...
			string name = this.textName.Text;
			if(name == null || name.Length == 0)
			{
				Alert.ShowWarning(this, "You must supply a connection name.");
				return false;
			}

			// check...
			ConnectionType connectionType = this.SelectedConnectionType;
			if(connectionType == null)
			{
				Alert.ShowWarning(this, "You must select a connection type.");
				return false;
			}

			// string...
			string connectionString = this.ConnectionString;
			if(connectionString == null || connectionString.Length == 0)
			{
				Alert.ShowWarning(this, "You must enter a connection string.");
				return false;
			}

			// set...
			this.ConnectionSettings = new ConnectionSettings(name, connectionType.Type, connectionString);

			// test...
			if(reason == ApplyReason.OKPressed)
				this.TestInternal();

			// ok...
			return true;
		}

		/// <summary>
		/// Gets or sets the connection string.
		/// </summary>
		private string ConnectionString
		{
			get
			{
				string connectionString = this.textConnectionString.Text.Trim();
				if(connectionString.Length > 0)
					return connectionString;
				else
					return null;
			}
			set
			{
				this.textConnectionString.Text = value;
			}
		}

		/// <summary>
		/// Gets or sets the connectionsettings
		/// </summary>
		public ConnectionSettings ConnectionSettings
		{
			get
			{
				return _connectionSettings;
			}
			set
			{
				// check to see if the value has changed...
				if(value != _connectionSettings)
				{
					// set the value...
					_connectionSettings = value;

					// update...
					if(this.ConnectionSettings != null)
						this.ConnectionString = this.ConnectionSettings.ConnectionString;
					else
						this.ConnectionString = null;
				}
			}
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad (e);

			// set...
			// TODO: use typefinder...
//			this.listConnectionTypes.Items.Add(new ConnectionType(typeof(SqlServerConnection), "SQL Server"));
//			this.listConnectionTypes.Items.Add(new ConnectionType(typeof(OracleConnection), "Oracle"));
//			this.listConnectionTypes.Items.Add(new ConnectionType(typeof(MySqlConnection), "MySQL"));

			// mbr - 2008-08-31 - added typefinder for the others...
			TypeFinder finder = new TypeFinder(typeof(Connection));
			finder.AddAttributeSpecification(typeof(ConnectionAttribute), false);
			foreach(Type type in finder.GetTypes())
				this.listConnectionTypes.Items.Add(new ConnectionType(type, Runtime.Current.GetDescription(type)));

			// set...
			for(int index = 0; index < this.listConnectionTypes.Items.Count; index++)
			{
				if(((ConnectionType)this.listConnectionTypes.Items[index]).Type == typeof(SqlServerConnection))
				{
					this.listConnectionTypes.SelectedIndex = index;
					break;
				}
			}
		}

		private void buttonOk_Click(object sender, System.EventArgs e)
		{
			this.DialogOK();
		}

		private void ConnectToDatabaseDialog_Load(object sender, System.EventArgs e)
		{
		
		}

		private void buttonBuild_Click(object sender, System.EventArgs e)
		{
			try
			{
				BuildConnectionString();
			}
			catch (Exception ex)
			{
				Alert.ShowWarning(this, "Failed to build connection string.", ex);
			}
		}

		/// <summary>
		/// Builds a connection string.
		/// </summary>
		private void BuildConnectionString()
		{
			if(this.SelectedConnectionType == null)
			{
				Alert.ShowWarning(this, "You must select a connection type.");
				return;
			}
			
			// check...
			if(SelectedConnectionType.Type == null)
				throw new InvalidOperationException("SelectedConnectionType.Type is null.");

			// show...
			ConnectionStringDialog dialog = new ConnectionStringDialog();
			dialog.ConnectionType = this.SelectedConnectionType.Type;
			if(dialog.ShowDialog(this) == DialogResult.OK)
				this.textConnectionString.Text = dialog.GetConnectionString();
		}

		/// <summary>
		/// Gets the selected connection type.
		/// </summary>
		private ConnectionType SelectedConnectionType
		{
			get
			{
				return (ConnectionType)this.listConnectionTypes.SelectedItem;
			}
		}

		/// <summary>
		/// Gets or sets whether the database name is required.
		/// </summary>
		[Browsable(true), DefaultValue(true), Category("Behavior"), Description("Gets or sets whether the database name is required.")]
		public bool NameRequired
		{
			get
			{
				return _nameRequired;
			}
			set
			{
				// check to see if the value has changed...
				if(value != _nameRequired)
				{
					// set the value...
					_nameRequired = value;

					// which...
					if(!(value))
					{
						this.textName.Text = "Database";
						this.textName.Enabled = false;
						this.labelName.Enabled = false;
					}
					else
					{
						this.textName.Enabled = true;
						this.labelName.Enabled = true;
					}
				}
			}
		}
	}
}
