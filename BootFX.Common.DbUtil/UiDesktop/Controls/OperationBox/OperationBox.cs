// BootFX - Application framework for .NET applications
// 
// File: OperationBox.cs
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
using BootFX.Common;

namespace BootFX.Common.UI.Desktop
{
	/// <summary>
	/// Summary description for OperationBox.
	/// </summary>
	// mbr - 21-09-2006 - changed base.	
//	public class OperationBox : System.Windows.Forms.UserControl, IOperationItem
	public class OperationBox : BaseControl, IOperationItem
	{
		/// <summary>
		/// Private field to support <c>CopyStatusChangesToLog</c> property.
		/// </summary>
		private bool _copyStatusChangesToLog = true;
		
		private bool _isCancelled = false;
		private DateTime _lastUpdateTime = DateTime.MinValue;

		/// <summary>
		/// Private field to support <see cref="LastError"/> property.
		/// </summary>
		private Exception _lastError = null;
		
		/// <summary>
		/// Raised when the operation is cancelled.
		/// </summary>
		public event EventHandler Cancelled;

		/// <summary>
		/// Raised when an error occurs.
		/// </summary>
		public event EventHandler Error;
		
		/// <summary>
		/// Private field to support <c>Status</c> property.
		/// </summary>
		private string _status = "Working, please wait...";
		
		/// <summary>
		/// Raised when the operation is finished.
		/// </summary>
		public event EventHandler Finished;
		
		/// <summary>
		/// Private field to support <c>ProgressValue</c> property.
		/// </summary>
		private int _progressValue;

		/// <summary>
		/// Private field to support <c>ProgressMinimum</c> property.
		/// </summary>
		private int _progressMinimum;

		/// <summary>
		/// Private field to support <c>ProgressMaximum</c> property.
		/// </summary>
		private int _progressMaximum;
		
		private System.Windows.Forms.ProgressBar progress;
		private System.Windows.Forms.Label labelStatus;
		private System.Windows.Forms.Timer timerUpdate;
		private System.ComponentModel.IContainer components;

		public OperationBox()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitializeComponent call

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
			this.components = new System.ComponentModel.Container();
			this.progress = new System.Windows.Forms.ProgressBar();
			this.labelStatus = new System.Windows.Forms.Label();
			this.timerUpdate = new System.Windows.Forms.Timer(this.components);
			this.SuspendLayout();
			// 
			// progress
			// 
			this.progress.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.progress.Location = new System.Drawing.Point(0, 52);
			this.progress.Name = "progress";
			this.progress.Size = new System.Drawing.Size(368, 16);
			this.progress.TabIndex = 0;
			// 
			// labelStatus
			// 
			this.labelStatus.Dock = System.Windows.Forms.DockStyle.Fill;
			this.labelStatus.Location = new System.Drawing.Point(0, 0);
			this.labelStatus.Name = "labelStatus";
			this.labelStatus.Size = new System.Drawing.Size(368, 52);
			this.labelStatus.TabIndex = 1;
			this.labelStatus.Text = "Working, please wait...";
			// 
			// timerUpdate
			// 
			this.timerUpdate.Enabled = true;
			this.timerUpdate.Interval = 333;
			this.timerUpdate.Tick += new System.EventHandler(this.timerUpdate_Tick);
			// 
			// OperationBox
			// 
			this.Controls.Add(this.labelStatus);
			this.Controls.Add(this.progress);
			this.Name = "OperationBox";
			this.Size = new System.Drawing.Size(368, 68);
			this.ResumeLayout(false);

		}
		#endregion

		public void Cancel()
		{
			if(!(this.IsCancelled))
			{
				_isCancelled = true;
				this.SetLastUpdateTime();
				this.OnCancelled();
			}
		}

		/// <summary>
		/// Gets or sets the iscancelled
		/// </summary>
		public bool IsCancelled
		{
			get
			{
				return _isCancelled;
			}
		}

		/// <summary>
		/// Resets the last error.
		/// </summary>
		public void Reset()
		{
			_lastError = null;
			_isCancelled = false;
			_lastUpdateTime = DateTime.MinValue;
			_progressMaximum = 0;
			_progressMinimum = 0;
			_progressValue = 0;
			
			// enable...
			this.timerUpdate.Enabled = true;
		}
		
		public void IncrementProgress()
		{
			this.ProgressValue++;
		}

		/// <summary>
		/// Gets or sets the progressmaximum
		/// </summary>
		public int ProgressMaximum
		{
			get
			{
				return _progressMaximum;
			}
			set
			{
				// check to see if the value has changed...
				if(value != _progressMaximum)
				{
					// set the value...
					_progressMaximum = value;
				}
			}
		}
		
		/// <summary>
		/// Gets or sets the progressminimum
		/// </summary>
		public int ProgressMinimum
		{
			get
			{
				return _progressMinimum;
			}
			set
			{
				// check to see if the value has changed...
				if(value != _progressMinimum)
				{
					// set the value...
					_progressMinimum = value;
				}
			}
		}
		
		/// <summary>
		/// Gets or sets the progressvalue
		/// </summary>
		public int ProgressValue
		{
			get
			{
				return _progressValue;
			}
			set
			{
				if(value < this.ProgressMinimum)
					this.ProgressMinimum = value;
				if(value > this.ProgressMaximum)
					this.ProgressMaximum = value;

				// check to see if the value has changed...
				if(value != _progressValue)
				{
					// set the value...
					_progressValue = value;
					this.SetLastUpdateTime();
				}
			}
		}

		/// <summary>
		/// Gets the lasterror.
		/// </summary>
		public Exception LastError
		{
			get
			{
				return _lastError;
			}
		}

		/// <summary>
		/// Raises the <c>Error</c> event.
		/// </summary>
		private void OnError()
		{
			OnError(EventArgs.Empty);
		}
		
		/// <summary>
		/// Raises the <c>Error</c> event.
		/// </summary>
		protected virtual void OnError(EventArgs e)
		{
			// raise...
			if(Error != null)
				Error(this, e);
		}
		
		/// <summary>
		/// Raises the <c>Cancelled</c> event.
		/// </summary>
		private void OnCancelled()
		{
			OnCancelled(EventArgs.Empty);
		}
		
		/// <summary>
		/// Raises the <c>Cancelled</c> event.
		/// </summary>
		protected virtual void OnCancelled(EventArgs e)
		{
			// raise...
			if(Cancelled != null)
				Cancelled(this, e);
		}

		public void SetLastError(string status, Exception error)
		{
			this.Status = status;
			_lastError = error;
		}

		private void timerUpdate_Tick(object sender, System.EventArgs e)
		{
			try
			{
				// set...
				this.labelStatus.Text = this.Status;
				this.progress.Maximum = this.ProgressMaximum;
				this.progress.Minimum = this.ProgressMinimum;

				// mbr - 28-07-2006 - this wasn't constraining.				
				int value = this.ProgressValue;
				if(value < this.progress.Minimum)
					value = this.progress.Minimum;
				if(value > this.progress.Maximum)
					value = this.progress.Maximum;
				this.progress.Value = value;
			}
			catch(Exception ex)
			{
				this.timerUpdate.Enabled = false;
				Alert.ShowWarning(this, "Failed to update view.", ex);
			}
		}

		public DateTime LastUpdateTime
		{
			get
			{
				return _lastUpdateTime;
			}
		}

		/// <summary>
		/// Gets or sets the status
		/// </summary>
		public string Status
		{
			get
			{
				return _status;
			}
			set
			{
				// check to see if the value has changed...
				if(value != _status)
				{
					// set the value...
					_status = value;

					// mbr - 21-09-2006 - log...
					if(this.CopyStatusChangesToLog && this.Log.IsInfoEnabled)
						this.Log.Info(string.Format("[OP STATUS: {0}]", value));
					
					// update...
					this.SetLastUpdateTime();

				}
			}
		}

		/// <summary>
		/// Raises the <c>Finished</c> event.
		/// </summary>
		private void OnFinished()
		{
			OnFinished(EventArgs.Empty);
		}
		
		/// <summary>
		/// Raises the <c>Finished</c> event.
		/// </summary>
		protected virtual void OnFinished(EventArgs e)
		{
			// raise...
			if(Finished != null)
				Finished(this, e);
		}

		private void SetLastUpdateTime()
		{
			_lastUpdateTime = DateTime.UtcNow;
		}

		/// <summary>
		/// Gets or sets the copystatuschangestolog
		/// </summary>
		public bool CopyStatusChangesToLog
		{
			get
			{
				return _copyStatusChangesToLog;
			}
			set
			{
				// check to see if the value has changed...
				if(value != _copyStatusChangesToLog)
				{
					// set the value...
					_copyStatusChangesToLog = value;
				}
			}
		}
	}
}
