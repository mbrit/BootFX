// BootFX - Application framework for .NET applications
// 
// File: OperationItem.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.ComponentModel;
using BootFX.Common.Management;

namespace BootFX.Common
{
	/// <summary>
	/// Base class for an operation item.
	/// </summary>
	public class OperationItem : Loggable, IOperationItem, INotifyPropertyChanged
	{
		/// <summary>
		/// Raised when an error occurs.
		/// </summary>
		public event EventHandler Error;
		
		/// <summary>
		/// Private field to support <see cref="LastUpdateTime"/> property.
		/// </summary>
		private DateTime _lastUpdateTime;
		
		/// <summary>
		/// Private field to support <see cref="LastError"/> property.
		/// </summary>
		private Exception _lastError;
		
		/// <summary>
		/// Private field to support <c>IsCancelled</c> property.
		/// </summary>
		private bool _isCancelled = false;
		
		/// <summary>
		/// Raised when the item has been finished.
		/// </summary>
		public event EventHandler Finished;
		
		/// <summary>
		/// Raised when the item has been finished.
		/// </summary>
		public event EventHandler Cancelled;
		
		/// <summary>
		/// Private field to support <c>ProgressMaximum</c> property.
		/// </summary>
		private int _progressMaximum;

		/// <summary>
		/// Private field to support <c>ProgressMinimum</c> property.
		/// </summary>
		private int _progressMinimum;

		/// <summary>
		/// Private field to support <c>ProgressValue</c> property.
		/// </summary>
		private int _progressValue;
		
		/// <summary>
		/// Raised when the <c>ProgressValue</c> property has changed.
		/// </summary>
		[Browsable(true), Category("Property Changed"), Description("Raised when the ProgressValue property has changed.")]
		public event EventHandler ProgressValueChanged;

		/// <summary>
		/// Raised when the <c>ProgressMaximum</c> property has changed.
		/// </summary>
		[Browsable(true), Category("Property Changed"), Description("Raised when the ProgressMaximum property has changed.")]
		public event EventHandler ProgressMaximumChanged;

		/// <summary>
		/// Raised when the <c>ProgressMinimum</c> property has changed.
		/// </summary>
		[Browsable(true), Category("Property Changed"), Description("Raised when the ProgressMinimum property has changed.")]
		public event EventHandler ProgressMinimumChanged;
		
		/// <summary>
		/// Raised when the <c>Status</c> property has changed.
		/// </summary>
		[Browsable(true), Category("Property Changed"), Description("Raised when the Status property has changed.")]
		public event EventHandler StatusChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Private field to support <c>Status</c> property.
        /// </summary>
        private string _status;
		
		/// <summary>
		/// Constructor,
		/// </summary>
		public OperationItem()
		{
			this.Reset();
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
					this.SetLastUpdateTime();
					this.OnStatusChanged();
				}
			}
		}

		/// <summary>
		/// Raises the <c>StatusChanged</c> event.
		/// </summary>
		private void OnStatusChanged()
		{
			OnStatusChanged(EventArgs.Empty);
		}
		
		/// <summary>
		/// Raises the <c>StatusChanged</c> event.
		/// </summary>
		protected virtual void OnStatusChanged(EventArgs e)
		{
			if(StatusChanged != null)
				StatusChanged(this, e);
		}

        /// <summary>
        /// Raises the <c>StatusChanged</c> event.
        /// </summary>
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, e);
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
				// check to see if the value has changed...
				if(value > this.ProgressMaximum)
					value = this.ProgressMaximum;
				if(value < this.ProgressMinimum)
					value = this.ProgressMinimum;
				if(value != _progressValue)
				{
					// set the value...
					_progressValue = value;
					this.SetLastUpdateTime();
					this.OnProgressValueChanged();
				}
			}
		}
		
		/// <summary>
		/// Raises the <c>ProgressMinimumChanged</c> event.
		/// </summary>
		private void OnProgressMinimumChanged()
		{
			OnProgressMinimumChanged(EventArgs.Empty);
		}
		
		/// <summary>
		/// Raises the <c>ProgressMinimumChanged</c> event.
		/// </summary>
		protected virtual void OnProgressMinimumChanged(EventArgs e)
		{
			if(ProgressMinimumChanged != null)
				ProgressMinimumChanged(this, e);
		}
		
		/// <summary>
		/// Raises the <c>ProgressMaximumChanged</c> event.
		/// </summary>
		private void OnProgressMaximumChanged()
		{
			OnProgressMaximumChanged(EventArgs.Empty);
		}
		
		/// <summary>
		/// Raises the <c>ProgressMaximumChanged</c> event.
		/// </summary>
		protected virtual void OnProgressMaximumChanged(EventArgs e)
		{
			if(ProgressMaximumChanged != null)
				ProgressMaximumChanged(this, e);
		}
		
		/// <summary>
		/// Raises the <c>ProgressValueChanged</c> event.
		/// </summary>
		private void OnProgressValueChanged()
		{
			OnProgressValueChanged(EventArgs.Empty);
		}
		
		/// <summary>
		/// Raises the <c>ProgressValueChanged</c> event.
		/// </summary>
		protected virtual void OnProgressValueChanged(EventArgs e)
		{
			if(ProgressValueChanged != null)
				ProgressValueChanged(this, e);
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
					this.OnProgressMinimumChanged();
				}
			}
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

					// reset...
					this.ProgressValue = this.ProgressMinimum;

					// notify...
					this.OnProgressMaximumChanged();
				}
			}
		}

		/// <summary>
		/// Increments the progress value by one.
		/// </summary>
		public void IncrementProgress()
		{
			this.ProgressValue++;
		}

		/// <summary>
		/// Raises the <c>Canceled</c> event.
		/// </summary>
		private void OnCancelled()
		{
			this.OnCancelled(EventArgs.Empty);
		}
		
		/// <summary>
		/// Raises the <c>Canceled</c> event.
		/// </summary>
		protected virtual void OnCancelled(EventArgs e)
		{
			// raise...
			if(Cancelled != null)
				Cancelled(this, e);
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
		/// Gets the lasterror.
		/// </summary>
		public Exception LastError
		{
			get
			{
				return _lastError;
			}
		}

		void IOperationItem.SetLastError(string status, Exception error)
		{
			this.SetLastError(status, error);
		}

		protected void SetLastError(string status, Exception error)
		{
			_lastError = error;
			this.Status = status;

			// raise?
			if(_lastError != null)
				this.OnError();
		}

		/// <summary>
		/// Gets the lastupdatetime.
		/// </summary>
		public DateTime LastUpdateTime
		{
			get
			{
				return _lastUpdateTime;
			}
		}

		private void SetLastUpdateTime()
		{
			_lastUpdateTime = DateTime.UtcNow;
		}

		/// <summary>
		/// Raises the <c>LastError</c> event.
		/// </summary>
		private void OnError()
		{
			OnError(EventArgs.Empty);
		}
		
		/// <summary>
		/// Raises the <c>LastError</c> event.
		/// </summary>
		protected virtual void OnError(EventArgs e)
		{
			// raise...
			if(Error != null)
				Error(this, e);
		}

		void IOperationItem.Reset()
		{
			this.Reset();
		}

		/// <summary>
		/// Resets the last error.
		/// </summary>
		protected void Reset()
		{
			_lastError = null;
			_isCancelled = false;
			_lastUpdateTime = DateTime.MinValue;
			_progressMaximum = 0;
			_progressMinimum = 0;
			_progressValue = 0;
		}
	}
}
