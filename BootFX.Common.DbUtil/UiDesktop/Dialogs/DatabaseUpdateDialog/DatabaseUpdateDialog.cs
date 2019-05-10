// BootFX - Application framework for .NET applications
// 
// File: DatabaseUpdateDialog.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Text;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using BootFX.Common;
using BootFX.Common.Data;
using BootFX.Common.Data.Schema;

namespace BootFX.Common.UI.Desktop
{
	/// <summary>
	/// Summary description for DatabaseUpdateDialog.
	/// </summary>
	public class DatabaseUpdateDialog : BaseForm
	{
		/// <summary>
		/// Private field to support <see cref="CompletedOk"/> property.
		/// </summary>
		private bool _completedOk;
		
		/// <summary>
		/// Private field to support <c>Thread</c> property.
		/// </summary>
		private ThreadUIHelper _thread;
		
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textConnectionString;
		private System.Windows.Forms.GroupBox groupBox2;
		private OperationBox operation;
		private System.Windows.Forms.Button buttonCheck;
		private System.Windows.Forms.Button buttonUpdate;
		private System.Windows.Forms.Button buttonClose;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public DatabaseUpdateDialog()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			this.RefreshButtons();
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
			this.textConnectionString = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.buttonUpdate = new System.Windows.Forms.Button();
			this.buttonCheck = new System.Windows.Forms.Button();
			this.operation = new BootFX.Common.UI.Desktop.OperationBox();
			this.buttonClose = new System.Windows.Forms.Button();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox1.Controls.Add(this.textConnectionString);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Location = new System.Drawing.Point(8, 8);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(328, 84);
			this.groupBox1.TabIndex = 1;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Settings";
			// 
			// textConnectionString
			// 
			this.textConnectionString.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.textConnectionString.Location = new System.Drawing.Point(80, 20);
			this.textConnectionString.Multiline = true;
			this.textConnectionString.Name = "textConnectionString";
			this.textConnectionString.ReadOnly = true;
			this.textConnectionString.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.textConnectionString.Size = new System.Drawing.Size(240, 56);
			this.textConnectionString.TabIndex = 1;
			this.textConnectionString.Text = "";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(12, 20);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(64, 20);
			this.label1.TabIndex = 0;
			this.label1.Text = "&Connection:";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// groupBox2
			// 
			this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox2.Controls.Add(this.buttonUpdate);
			this.groupBox2.Controls.Add(this.buttonCheck);
			this.groupBox2.Controls.Add(this.operation);
			this.groupBox2.Location = new System.Drawing.Point(8, 96);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(328, 92);
			this.groupBox2.TabIndex = 0;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Update";
			// 
			// buttonUpdate
			// 
			this.buttonUpdate.Location = new System.Drawing.Point(168, 20);
			this.buttonUpdate.Name = "buttonUpdate";
			this.buttonUpdate.Size = new System.Drawing.Size(148, 23);
			this.buttonUpdate.TabIndex = 1;
			this.buttonUpdate.Text = "&Run Database Update";
			this.buttonUpdate.Click += new System.EventHandler(this.buttonUpdate_Click);
			// 
			// buttonCheck
			// 
			this.buttonCheck.Location = new System.Drawing.Point(12, 20);
			this.buttonCheck.Name = "buttonCheck";
			this.buttonCheck.Size = new System.Drawing.Size(148, 23);
			this.buttonCheck.TabIndex = 0;
			this.buttonCheck.Text = "Check &Status";
			this.buttonCheck.Click += new System.EventHandler(this.buttonCheck_Click);
			// 
			// operation
			// 
			this.operation.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.operation.CopyStatusChangesToLog = true;
			this.operation.Location = new System.Drawing.Point(12, 52);
			this.operation.Name = "operation";
			this.operation.ProgressMaximum = 0;
			this.operation.ProgressMinimum = 0;
			this.operation.ProgressValue = 0;
			this.operation.Size = new System.Drawing.Size(308, 32);
			this.operation.Status = "Working, please wait...";
			this.operation.TabIndex = 2;
			// 
			// buttonClose
			// 
			this.buttonClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonClose.Location = new System.Drawing.Point(258, 196);
			this.buttonClose.Name = "buttonClose";
			this.buttonClose.TabIndex = 2;
			this.buttonClose.Text = "Close";
			this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
			// 
			// DatabaseUpdateDialog
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(342, 226);
			this.Controls.Add(this.buttonClose);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "DatabaseUpdateDialog";
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Database Update";
			this.groupBox1.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void buttonCheck_Click(object sender, System.EventArgs e)
		{
			this.Run(DatabaseUpdateOperation.Check);
		}

		private bool IsRunning
		{
			get
			{
				if(_thread == null)
					return false;
				else
					return true;
			}
		}

		private void RefreshButtons()
		{
			this.buttonClose.Enabled = true;
			if(this.IsRunning)
			{
				// set...
				this.buttonCheck.Enabled = false;
				this.buttonUpdate.Enabled = false;
				this.buttonClose.Text = "Cancel";
			}
			else
			{
				// reset...
				this.buttonCheck.Enabled = true;
				this.buttonUpdate.Enabled = true;
				this.buttonClose.Text = "Close";
				
				// reset...
				this.operation.Reset();
				this.operation.Status = "Idle";
			}
		}

		private void Run(DatabaseUpdateOperation type)
		{
			// check...
			if(_thread != null)
				throw new InvalidOperationException("A database update operation is already running.");

			// create...
			ConnectionSettings settings = Database.DefaultConnectionSettings;
			if(settings == null)
				throw new InvalidOperationException("settings is null.");

			// create...
			_thread = new ThreadUIHelper(this, this.operation);
			_thread.ThreadStarted += new EventHandler(_thread_ThreadStarted);
			_thread.ThreadFinished += new EventHandler(_thread_ThreadFinished);
			_thread.Failed += new System.Threading.ThreadExceptionEventHandler(_thread_Failed);
			_thread.RunAsync(this, "RunInternal", type);

			// reset...
			this.RefreshButtons();
		}

		private void RunInternal(DatabaseUpdateOperation type)
		{
			DatabaseUpdateUIWorker worker = new DatabaseUpdateUIWorker(Database.DefaultConnectionSettings, type, 
				this.operation);
			worker.CheckComplete += new DatabaseUpdateCheckResultsEventHandler(worker_CheckComplete);
			worker.UpdateComplete += new EventHandler(worker_UpdateComplete);
			worker.DoWork();
		}

		private void buttonUpdate_Click(object sender, System.EventArgs e)
		{
			this.Run(DatabaseUpdateOperation.Update);
		}

		private void buttonClose_Click(object sender, System.EventArgs e)
		{
			if(this.IsRunning)
			{
				this.operation.Cancel();
				this.buttonClose.Enabled = false;
				this.buttonClose.Text = "Wait...";
			}
			else
			{
				// stop...
				if(this.CompletedOk)
					this.DialogResult = DialogResult.OK;
				else
					this.DialogResult = DialogResult.Cancel;
			}
		}

		/// <summary>
		/// Gets the thread.
		/// </summary>
		private ThreadUIHelper Thread
		{
			get
			{
				// returns the value...
				return _thread;
			}
		}

		private void _thread_UnhandledError(object sender, System.Threading.ThreadExceptionEventArgs e)
		{	
			Alert.ShowWarning(this, "An unhandled error occurred when running the update operation.", e.Exception);
		}

		private void _thread_ThreadFinished(object sender, EventArgs e)
		{
			// reset....
			_thread = null;

			// update...
			this.RefreshButtons();
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad (e);

			// check...
			if(!(Database.HasDefaultDatabaseSettings()))
				throw new InvalidOperationException("Database settings have not been configured.");

			// set...
			this.textConnectionString.Text = Database.DefaultConnectionString;
		}

		private void _thread_CheckComplete(object sender, DatabaseUpdateCheckResultsEventArgs e)
		{
			if(e == null)
				throw new ArgumentNullException("e");
			
			// flip...
			if(this.InvokeRequired)
			{
				DatabaseUpdateCheckResultsEventHandler d = new DatabaseUpdateCheckResultsEventHandler(this._thread_CheckComplete);
				this.Invoke(d, new object[] { sender, e });
				return;
			}

			// check...
			if(e.Results == null)
				throw new InvalidOperationException("e.Results is null.");

			// update...
			if(e.Results.IsUpToDate)
				Alert.ShowInformation(this, "The database is up-to-date.");
			else
			{
				string[] messages = e.Results.GetFriendlyUnitWorkMessages();
				if(messages == null)
					throw new InvalidOperationException("'messages' is null.");
				if(messages.Length == 0)
					throw new InvalidOperationException("'messages' is zero-length.");

				StringBuilder builder = new StringBuilder();
				builder.Append("Changes need to be made to this database:\r\n");
				for(int index = 0; index < messages.Length; index++)
				{
					builder.Append("\r\n   - ");
					if(index == 16)
					{
						builder.Append("(more...)");
						break;
					}
					else
						builder.Append(messages[index]);
				}

				// show...
				Alert.ShowWarning(this, builder.ToString());
			}
		}

		private void _thread_ThreadStarted(object sender, EventArgs e)
		{
			this.RefreshButtons();
		}

		private void _thread_Failed(object sender, System.Threading.ThreadExceptionEventArgs e)
		{
			_completedOk = false;
			Alert.ShowWarning(this, "The database update operation failed.", e.Exception);
		}

		private void worker_CheckComplete(object sender, DatabaseUpdateCheckResultsEventArgs e)
		{
			StringBuilder builder = new StringBuilder();
			if(e.Results.IsUpToDate)
				builder.Append("The database is up-to-date.");
			else
			{
				builder.Append("Changed need to be made to the database:\r\n");
				int index = 0;
				foreach(string message in e.Results.GetFriendlyUnitWorkMessages())
				{
					const string prefix = "\r\n   - ";
					builder.Append(prefix);
					builder.Append(message);

					// mbr - 24-07-2007 - case 321 - the new dialog lets us show loads...					
//					if(index == 20)
//					{
//						builder.Append(prefix);
//						builder.Append("(more...)");
//						break;
//					}

					// next...
					index++;
				}
			}

			// show...
			_completedOk = true;

			// mbr - 24-07-2007 - case 321 - changed.			
			//ThreadUIHelper.Current.ShowInformation(builder.ToString());
			this.ShowChangesTbd(builder.ToString());
		}

		// mbr - 24-07-2007 - case 321 - added.		
		private delegate void ShowChangesTbdDelegate(string changes);

		/// <summary>
		/// Shows changes.
		/// </summary>
		/// <param name="changes"></param>
		// mbr - 24-07-2007 - case 321 - added.		
		private void ShowChangesTbd(string changes)
		{
			if(this.InvokeRequired)
			{
				ShowChangesTbdDelegate d = new ShowChangesTbdDelegate(this.ShowChangesTbd);
				this.Invoke(d, new object[] { changes });
				return;
			}

			// show...
			using(DatabaseUpdateTbdDialog dialog = new DatabaseUpdateTbdDialog())
			{
				dialog.Changes = changes;
				dialog.ShowDialog(this);
			}
		}

		private void worker_UpdateComplete(object sender, EventArgs e)
		{
			_completedOk = true;
			ThreadUIHelper.Current.ShowInformation("The database update operation has completed.");
		}

		/// <summary>
		/// Gets the completedok.
		/// </summary>
		private bool CompletedOk
		{
			get
			{
				return _completedOk;
			}
		}
	}
}
