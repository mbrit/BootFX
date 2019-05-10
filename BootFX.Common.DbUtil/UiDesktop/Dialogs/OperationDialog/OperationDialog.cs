// BootFX - Application framework for .NET applications
// 
// File: OperationDialog.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Drawing;
using System.Threading;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using BootFX.Common.Management;

namespace BootFX.Common.UI.Desktop
{
	/// <summary>
	/// Summary description for OperationDialog.
	/// </summary>
	public class OperationDialog : BaseForm, IOperationItem
	{
		/// <summary>
		/// Raised when the operation fails.
		/// </summary>
		public event ThreadExceptionEventHandler Failed;

		/// <summary>
		/// Private field to support <c>ShowErrorDialog</c> property.
		/// </summary>
		private bool _showErrorDialog = true;
		
		private ThreadUIHelper _thread;

		/// <summary>
		/// Private field to support <see cref="RunOnLoad"/> property.
		/// </summary>
		private OperationDialogProcess _runOnLoad;
		
		/// <summary>
		/// Private field to support <see cref="OperationBox"/> property.
		/// </summary>
		private OperationBox _operationBox;
		
		/// <summary>
		/// Private field to support <see cref="CaptionWidget"/> property.
		/// </summary>
		private DialogCaption _captionWidget;
		
		private System.Windows.Forms.StatusBar statusbar;
		private System.Windows.Forms.Panel panelBox;
		private System.Windows.Forms.Panel panelCaption;
		private System.Windows.Forms.StatusBarPanel panelText;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public OperationDialog()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			_captionWidget = new DialogCaption();
			_captionWidget.Dock = DockStyle.Fill;
			this.panelCaption.Controls.Add(_captionWidget);
			this.Caption = "Working...";

			// op...
			_operationBox = new OperationBox();
			_operationBox.Dock = DockStyle.Fill;
			this.panelBox.Controls.Add(_operationBox);
		}

		/// <summary>
		/// Gets the captionwidget.
		/// </summary>
		private DialogCaption CaptionWidget
		{
			get
			{
				return _captionWidget;
			}
		}

		public string Caption
		{
			get
			{
				return this.CaptionWidget.Caption;
			}
			set
			{
				this.CaptionWidget.Caption = value;
			}
		}

		/// <summary>
		/// Gets the operationbox.
		/// </summary>
		private OperationBox OperationBox
		{
			get
			{
				return _operationBox;
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

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.statusbar = new System.Windows.Forms.StatusBar();
			this.panelBox = new System.Windows.Forms.Panel();
			this.panelCaption = new System.Windows.Forms.Panel();
			this.panelText = new System.Windows.Forms.StatusBarPanel();
			((System.ComponentModel.ISupportInitialize)(this.panelText)).BeginInit();
			this.SuspendLayout();
			// 
			// statusbar
			// 
			this.statusbar.Location = new System.Drawing.Point(0, 127);
			this.statusbar.Name = "statusbar";
			this.statusbar.Panels.AddRange(new System.Windows.Forms.StatusBarPanel[] {
																						 this.panelText});
			this.statusbar.ShowPanels = true;
			this.statusbar.Size = new System.Drawing.Size(348, 24);
			this.statusbar.SizingGrip = false;
			this.statusbar.TabIndex = 0;
			// 
			// panelBox
			// 
			this.panelBox.Location = new System.Drawing.Point(4, 60);
			this.panelBox.Name = "panelBox";
			this.panelBox.Size = new System.Drawing.Size(340, 60);
			this.panelBox.TabIndex = 1;
			// 
			// panelCaption
			// 
			this.panelCaption.Dock = System.Windows.Forms.DockStyle.Top;
			this.panelCaption.Location = new System.Drawing.Point(0, 0);
			this.panelCaption.Name = "panelCaption";
			this.panelCaption.Size = new System.Drawing.Size(348, 52);
			this.panelCaption.TabIndex = 2;
			// 
			// panelText
			// 
			this.panelText.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Spring;
			this.panelText.Width = 348;
			// 
			// OperationDialog
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(348, 151);
			this.Controls.Add(this.panelCaption);
			this.Controls.Add(this.panelBox);
			this.Controls.Add(this.statusbar);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "OperationDialog";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Working...";
			((System.ComponentModel.ISupportInitialize)(this.panelText)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion


		public void Cancel()
		{
			this.OperationBox.Cancel();
		}

		public int ProgressMinimum
		{
			get
			{
				return this.OperationBox.ProgressMinimum;
			}
			set
			{
				this.OperationBox.ProgressMinimum = value;
			}
		}

		public void Reset()
		{
			this.OperationBox.Reset();
		}

		public event System.EventHandler Finished
		{
			add
			{
				this.OperationBox.Finished += value;
			}
			remove
			{
				this.OperationBox.Finished -= value;
			}
		}

		public int ProgressMaximum
		{
			get
			{
				return this.OperationBox.ProgressMaximum;
			}
			set
			{
				this.OperationBox.ProgressMaximum = value;
			}
		}

		public void IncrementProgress()
		{
			this.OperationBox.IncrementProgress();
		}

		public bool IsCancelled
		{
			get
			{
				return this.OperationBox.IsCancelled;
			}
		}

		public int ProgressValue
		{
			get
			{
				return this.OperationBox.ProgressValue;
			}
			set
			{
				this.OperationBox.ProgressValue = value;
			}
		}

		public Exception LastError
		{
			get
			{
				return this.OperationBox.LastError;
			}
		}

		public event System.EventHandler Cancelled
		{
			add
			{
				this.OperationBox.Cancelled += value;
			}
			remove
			{
				this.OperationBox.Cancelled -= value;
			}
		}

		public event System.EventHandler Error
		{
			add
			{
				this.OperationBox.Error += value;
			}
			remove
			{
				this.OperationBox.Error -= value;
			}
		}

		public void SetLastError(string status, Exception error)
		{
			this.OperationBox.SetLastError(status, error);
		}

		public DateTime LastUpdateTime
		{
			get
			{
				return this.OperationBox.LastUpdateTime;
			}
		}

		public string Status
		{
			get
			{
				return this.OperationBox.Status;
			}
			set
			{
				this.OperationBox.Status = value;
			}
		}

		/// <summary>
		/// Runs the supplied process.
		/// </summary>
		/// <param name="process"></param>
		public void Run(OperationDialogProcess process)
		{
			if(process == null)
				throw new ArgumentNullException("process");
			
			// check...
			if(process.Context == null)
				throw new InvalidOperationException("process.Context is null.");

			// mbr - 2008-11-27 - if it doesn't have an operation, give it us...
			if(process.Context.InnerOperation == null || process.Context.DefaultInnerOperationUsed)
				process.Context.InnerOperation = this;

			// load called?
			if(!(this.LoadCalled))
			{
				_runOnLoad = process;
				return;
			}

			// thread?
			if(_thread != null)
				throw new InvalidOperationException("A thread is already running.");

			// create...
			_thread = new ThreadUIHelper(this, this);

			// mbr - 10-05-2007 - changed this to create a log file if we haven't been given one explicitly...
			if(process.Context.HasInnerLog)
				_thread.BoundLog = process.Context.InnerLog;
			else
			{
				// log...
				FileLog log = LogSet.CreateFileLogger(process.GetType().Name, FileLoggerFlags.AddDateToFileName | 
					FileLoggerFlags.EnsureNewFile | FileLoggerFlags.OwnFolder);
				if(log == null)
					throw new InvalidOperationException("log is null.");

				// set...
				_thread.BoundLog = log;
			}

			// events...
			_thread.Failed += new System.Threading.ThreadExceptionEventHandler(_thread_Failed);
			_thread.Succeeded += new ResultEventHandler(_thread_Succeeded);

			// run...
			_thread.RunAsync(process, "Run");
		}

		/// <summary>
		/// Gets the thread.
		/// </summary>
		private ThreadUIHelper Thread
		{
			get
			{
				return _thread;
			}
		}

		/// <summary>
		/// Gets the runonload.
		/// </summary>
		private OperationDialogProcess RunOnLoad
		{
			get
			{
				return _runOnLoad;
			}
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad (e);
			if(this.DesignMode)
				return;

			// do we run on load?
			if(this.RunOnLoad != null)
			{
				OperationDialogProcess toRun = this.RunOnLoad;
				_runOnLoad = null;

				// run...
				this.Run(toRun);
			}
		}

		/// <summary>
		/// Gets or sets the showerrordialog
		/// </summary>
		public bool ShowErrorDialog
		{
			get
			{
				return _showErrorDialog;
			}
			set
			{
				// check to see if the value has changed...
				if(value != _showErrorDialog)
				{
					// set the value...
					_showErrorDialog = value;
				}
			}
		}

		private void _thread_Failed(object sender, System.Threading.ThreadExceptionEventArgs e)
		{
			this.OnFailed(e);
		}

		protected virtual void OnFailed(ThreadExceptionEventArgs e)
		{
			if(this.Failed == null)
			{
				if(this.ShowErrorDialog)
					Alert.ShowWarning(this, "An error occurred when running the operation.", e.Exception);
			}
			else
				this.Failed(this, e);

			// nope...
			this.DialogResult = DialogResult.Abort;
		}

		private void _thread_Succeeded(object sender, ResultEventArgs e)
		{
			// ok...
			this.DialogOK();
		}

		/// <summary>
		/// Gets or sets the status bar text.
		/// </summary>
		[Browsable(true), Category("Appearance")]
		public string StatusBarText
		{
			get
			{
				return this.panelText.Text;
			}
			set
			{
				this.panelText.Text = value;
			}
		}
	}
}
