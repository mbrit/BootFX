// BootFX - Application framework for .NET applications
// 
// File: LogonDialog.cs
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
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using BootFX.Common;
using BootFX.Common.UI;
using BootFX.Common.UI.Desktop;
using BootFX.Common.Data;

namespace BootFX.Common.UI.Desktop
{
	/// <summary>
	/// Summary description for LogonDialog.
	/// </summary>
	public class LogonDialog : System.Windows.Forms.Form
	{
		/// <summary>
		/// Delegate for <see cref="EditConnections"></see>.
		/// </summary>
		private delegate void EditConnectionsDelegate();

		/// <summary>
		/// Private field to support <c>LazyTimer</c> property.
		/// </summary>
		private System.Timers.Timer _lazyTimer;
		
		/// <summary>
		/// Raised when the system should create the first connection.
		/// </summary>
		public event EventHandler CreateFirstConnection;
		
		/// <summary>
		/// Private field to support <c>UsernameRequired</c> property.
		/// </summary>
		private bool _usernameRequired = true;
		
		/// <summary>
		/// Private field to support <c>PasswordRequired</c> property.
		/// </summary>
		private bool _passwordRequired = true;
		
		/// <summary>
		/// Raised when the user attempts to logon the user.
		/// </summary>
		public event AuthenticateUserEventHandler AuthenticateUser;
		
		private System.Windows.Forms.PictureBox pictureSplash;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textUsername;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.TextBox textPassword;
		private System.Windows.Forms.Timer timerFade;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.ComboBox listConnections;
		private System.ComponentModel.IContainer components;

		public LogonDialog()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			// setup...
			if(DesktopRuntime.IsStarted)
				this.Icon = DesktopRuntime.Current.DefaultFormIcon;
			else
				this.Icon = SystemIcons.Application;
			this.pictureSplash.Image = this.CreateSplashImage();
			this.textPassword.PasswordChar = DesktopRuntime.XpPasswordChar;
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			this.DisposeLazyTimer();
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
			this.components = new System.ComponentModel.Container();
			this.pictureSplash = new System.Windows.Forms.PictureBox();
			this.label1 = new System.Windows.Forms.Label();
			this.textUsername = new System.Windows.Forms.TextBox();
			this.textPassword = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.buttonOK = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.timerFade = new System.Windows.Forms.Timer(this.components);
			this.label3 = new System.Windows.Forms.Label();
			this.listConnections = new System.Windows.Forms.ComboBox();
			this.SuspendLayout();
			// 
			// pictureSplash
			// 
			this.pictureSplash.Location = new System.Drawing.Point(1, 1);
			this.pictureSplash.Name = "pictureSplash";
			this.pictureSplash.Size = new System.Drawing.Size(400, 100);
			this.pictureSplash.TabIndex = 0;
			this.pictureSplash.TabStop = false;
			this.pictureSplash.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureSplash_Paint);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(52, 148);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(80, 20);
			this.label1.TabIndex = 1;
			this.label1.Text = "&Username:";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textUsername
			// 
			this.textUsername.Location = new System.Drawing.Point(124, 148);
			this.textUsername.Name = "textUsername";
			this.textUsername.Size = new System.Drawing.Size(216, 20);
			this.textUsername.TabIndex = 2;
			this.textUsername.Text = "";
			// 
			// textPassword
			// 
			this.textPassword.Location = new System.Drawing.Point(124, 172);
			this.textPassword.Name = "textPassword";
			this.textPassword.PasswordChar = '*';
			this.textPassword.Size = new System.Drawing.Size(216, 20);
			this.textPassword.TabIndex = 4;
			this.textPassword.Text = "";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(52, 172);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(80, 20);
			this.label2.TabIndex = 3;
			this.label2.Text = "&Password:";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// buttonOK
			// 
			this.buttonOK.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.buttonOK.Location = new System.Drawing.Point(124, 200);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.TabIndex = 5;
			this.buttonOK.Text = "OK";
			this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
			// 
			// buttonCancel
			// 
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.buttonCancel.Location = new System.Drawing.Point(204, 200);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.TabIndex = 6;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
			// 
			// timerFade
			// 
			this.timerFade.Enabled = true;
			this.timerFade.Interval = 25;
			this.timerFade.Tick += new System.EventHandler(this.timerFade_Tick);
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(52, 124);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(80, 20);
			this.label3.TabIndex = 7;
			this.label3.Text = "&Connection:";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// listConnections
			// 
			this.listConnections.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.listConnections.Location = new System.Drawing.Point(124, 124);
			this.listConnections.Name = "listConnections";
			this.listConnections.Size = new System.Drawing.Size(216, 21);
			this.listConnections.TabIndex = 8;
			this.listConnections.SelectedIndexChanged += new System.EventHandler(this.listConnections_SelectedIndexChanged);
			// 
			// LogonDialog
			// 
			this.AcceptButton = this.buttonOK;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.BackColor = System.Drawing.SystemColors.Window;
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(402, 236);
			this.Controls.Add(this.listConnections);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.textPassword);
			this.Controls.Add(this.textUsername);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.pictureSplash);
			this.Font = new System.Drawing.Font("Tahoma", 8F);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Name = "LogonDialog";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "User Logon";
			this.Load += new System.EventHandler(this.LogonDialog_Load);
			this.ResumeLayout(false);

		}
		#endregion

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint (e);

			// us...
			e.Graphics.DrawLine(Pens.Black, this.ClientRectangle.Left,  this.ClientRectangle.Top,  this.ClientRectangle.Right,  this.ClientRectangle.Top);
			e.Graphics.DrawLine(Pens.Black, this.ClientRectangle.Left,  this.ClientRectangle.Top,  this.ClientRectangle.Left,  this.ClientRectangle.Bottom);
			e.Graphics.DrawLine(Pens.Black, this.ClientRectangle.Right - 1,  this.ClientRectangle.Top,  this.ClientRectangle.Right - 1,  this.ClientRectangle.Bottom);
			e.Graphics.DrawLine(Pens.Black, this.ClientRectangle.Left,  this.ClientRectangle.Bottom - 1,  this.ClientRectangle.Right,  this.ClientRectangle.Bottom - 1);
			e.Graphics.DrawLine(Pens.Black, this.ClientRectangle.Left,  this.pictureSplash.Bottom,  this.ClientRectangle.Right,  this.pictureSplash.Bottom);
		}

		private void pictureSplash_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			Version version = Runtime.Current.Application.ProductVersion; // typeof(Company).Assembly.GetName().Version;
			StringFormat format = new StringFormat();
			format.Alignment = StringAlignment.Far;
			e.Graphics.DrawString(version.ToString(), this.Font, Brushes.Black, 390, 10, format);
		}

		private void buttonCancel_Click(object sender, System.EventArgs e)
		{
			this.DialogResult = DialogResult.Cancel;
		}

		private void buttonOK_Click(object sender, System.EventArgs e)
		{
			string username = this.textUsername.Text;
			string password = this.textPassword.Text;

			// check...
			if(this.CurrentConnectionSettings == null)
			{
				Alert.ShowWarning(this, "You must select a connection.");
				return;
			}

			// set...
			Database.SetDefaultDatabase(this.CurrentConnectionSettings);

			// check...
			if(this.UsernameRequired && (username == null || username.Length == 0))
			{
				Alert.ShowWarning(this, "You must enter your username.");
				return;
			}
			if(this.PasswordRequired && (password == null || password.Length == 0))
			{
				Alert.ShowWarning(this, "You must enter your password.");
				return;
			}

			// validate...
			AuthenticateUserEventArgs validateArgs = new AuthenticateUserEventArgs(username, password);
			try
			{
				this.AuthenticateUser(this, validateArgs);
			}
			catch(Exception ex)
			{
				Alert.ShowWarning(this, "Failed to logon user.", ex);
				return;
			}

			// what now?
			if(validateArgs.IsAuthenticated)
			{
				// remember...
				Runtime.Current.UserSettings["LastUsername"] = username;
				Runtime.Current.UserSettings["LastConnection"] = this.CurrentConnectionSettings.Name;

				// ok...
				this.DialogResult = DialogResult.OK;
			}
		}

		private void LogonDialog_Load(object sender, System.EventArgs e)
		{
			this.Opacity = 0;
			this.BringToFront();
			this.Activate();

			// refresh...
			this.RefreshConnections();

			// select...
			string lastConnectionName = (string)Runtime.Current.UserSettings["LastConnection"];
			if(lastConnectionName != null && lastConnectionName.Length > 0)
			{
				// walk...
				foreach(ConnectionSettingsListItem item in this.listConnections.Items)
				{
					if(string.Compare(item.Settings.Name, lastConnectionName, true, System.Globalization.CultureInfo.InvariantCulture) == 0)
					{
						this.listConnections.SelectedItem = item;
						break;
					}
				}
			}

			// get...
			this.textUsername.Text = (string)Runtime.Current.UserSettings["LastUsername"];

			// show...
			if(this.textUsername.Text == null || this.textUsername.Text.Length == 0)
				this.textUsername.Focus();
			else
				this.textPassword.Focus();
		}

		private void timerFade_Tick(object sender, System.EventArgs e)
		{
			if(this.Opacity < 1)
				this.Opacity += .05;
			else
				this.timerFade.Enabled = false;
		}

		/// <summary>
		/// Gets the current connection settings.
		/// </summary>
		private ConnectionSettings CurrentConnectionSettings
		{
			get
			{
				ConnectionSettingsListItem item = this.listConnections.SelectedItem as ConnectionSettingsListItem;
				if(item != null)
					return item.Settings;
				else
					return null;
			}
		}

		/// <summary>
		/// Raises the <c>CreateFirstConnection</c> event.
		/// </summary>
		private void OnCreateFirstConnection()
		{
			OnCreateFirstConnection(EventArgs.Empty);
		}
		
		/// <summary>
		/// Raises the <c>CreateFirstConnection</c> event.
		/// </summary>
		protected virtual void OnCreateFirstConnection(EventArgs e)
		{
			// raise...
			if(CreateFirstConnection != null)
				CreateFirstConnection(this, e);
		}

		/// <summary>
		/// Refreshes the connections.
		/// </summary>
		private void RefreshConnections()
		{
			// walk...
			if(ConnectionSettings.SavedSettings == null)
				throw new InvalidOperationException("ConnectionSettings.SavedSettings is null.");

			// do we have the default?
			if(ConnectionSettings.SavedSettings.Count == 0)
			{
				// create..
				this.OnCreateFirstConnection();

				// save...
				if(ConnectionSettings.SavedSettings.Count > 0)
					ConnectionSettings.SaveSettings();
			}

			// update...
			this.listConnections.Items.Clear();
			foreach(ConnectionSettings settings in ConnectionSettings.SavedSettings)
				this.listConnections.Items.Add(new ConnectionSettingsListItem(settings));

			// add...
			this.listConnections.Items.Add("Edit connections...");

			// select...
			if(this.listConnections.Items.Count > 1)
				this.listConnections.SelectedIndex = 0;
		}

		private void listConnections_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if(this.listConnections.SelectedItem is string)
				LazyEditConnections();
		}

		/// <summary>
		/// Lazily edits connections.
		/// </summary>
		private void LazyEditConnections()
		{
			this.DisposeLazyTimer();

			// create...
			_lazyTimer = new System.Timers.Timer(250);
			_lazyTimer.Elapsed += new System.Timers.ElapsedEventHandler(_lazyTimer_Elapsed);
			_lazyTimer.Enabled = true;
		}

		/// <summary>
		/// Disposes the lazy timer.
		/// </summary>
		private void DisposeLazyTimer()
		{
			if(_lazyTimer != null)
			{
				_lazyTimer.Dispose();
				_lazyTimer = null;
			}
		}

		/// <summary>
		/// Gets the lazytimer.
		/// </summary>
		private System.Timers.Timer LazyTimer
		{
			get
			{
				// returns the value...
				return _lazyTimer;
			}
		}

		/// <summary>
		/// Edits the connections.
		/// </summary>
		private void EditConnections()
		{
			if(this.IsHandleCreated == false)
				return;

			// flip?
			if(this.InvokeRequired)
			{
				EditConnectionsDelegate d = new EditConnectionsDelegate(this.EditConnections);
				this.Invoke(d);
				return;
			}

			// show...
			EditConnectionsDialog dialog = new EditConnectionsDialog();
			if(dialog.ShowDialog(this) == DialogResult.OK)
				this.RefreshConnections();

			// select none...
			if(this.listConnections.Items.Count <= 1)
				this.listConnections.SelectedIndex = -1;
			else
				this.listConnections.SelectedIndex = 0;
		}

		/// <summary>
		/// Raises the <c>AuthenticateUser</c> event.
		/// </summary>
		protected virtual void OnAuthenticateUser(AuthenticateUserEventArgs e)
		{
			// raise...
			if(AuthenticateUser != null)
				AuthenticateUser(this, e);
		}

		/// <summary>
		/// Gets or sets the usernamerequired
		/// </summary>
		[Category("Behavior"), Description("Gets or sets whether the username is required."), DefaultValue(true)]
		public bool UsernameRequired
		{
			get
			{
				return _usernameRequired;
			}
			set
			{
				// check to see if the value has changed...
				if(value != _usernameRequired)
				{
					// set the value...
					_usernameRequired = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the passwordrequired
		/// </summary>
		[Category("Behavior"), Description("Gets or sets whether the username is required."), DefaultValue(true)]
		public bool PasswordRequired
		{
			get
			{
				return _passwordRequired;
			}
			set
			{
				// check to see if the value has changed...
				if(value != _passwordRequired)
				{
					// set the value...
					_passwordRequired = value;
				}
			}
		}

		/// <summary>
		/// Creates the splash image.
		/// </summary>
		/// <returns></returns>
		private Image CreateSplashImage()
		{
			Bitmap bitmap = new Bitmap(this.pictureSplash.Width, this.pictureSplash.Height, PixelFormat.Format32bppPArgb);
			try
			{
				// image...
				using(Graphics graphics = Graphics.FromImage(bitmap))
				{
					graphics.SmoothingMode = SmoothingMode.HighQuality;

					// render...
					Color from = Color.FromArgb(231, 236, 253); // #E7ECFD
					Color to = Color.FromArgb(151, 173, 255);  // #97AFFF
					Rectangle rect = new Rectangle(0,0, bitmap.Width, bitmap.Height);
					using(Brush brush = new LinearGradientBrush(rect, from, to, 0.0))
						graphics.FillRectangle(brush, rect);

					// text...
					Rectangle textRect = new Rectangle(rect.Location, rect.Size);
					textRect.Inflate(-5, -5);
					StringFormat format = new StringFormat(StringFormatFlags.NoWrap);
					format.LineAlignment = StringAlignment.Center;
					using(Font font = new Font("Verdana", 26, FontStyle.Bold, GraphicsUnit.Point))
						graphics.DrawString(Runtime.Current.Application.ProductName, font, Brushes.Black, textRect, format);

					// powered by...
					format.LineAlignment = StringAlignment.Far;
					format.Alignment = StringAlignment.Far;
					using(Font font = new Font("Tahoma", 8, FontStyle.Italic, GraphicsUnit.Point))
						graphics.DrawString("Powered by IridiFX�", font, Brushes.Black, textRect, format);
				}
			}
			catch(Exception ex)
			{
				bitmap.Dispose();
				throw new InvalidOperationException("Failed to create splash bitmap.", ex);
			}

			// return...
			return bitmap;
		}

		private void _lazyTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			this.DisposeLazyTimer();
			this.EditConnections();
		}
	}
}
