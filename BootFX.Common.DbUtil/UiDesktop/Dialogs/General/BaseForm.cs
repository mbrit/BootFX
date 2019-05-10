// BootFX - Application framework for .NET applications
// 
// File: BaseForm.cs
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
using BootFX.Common.Management;

namespace BootFX.Common.UI.Desktop
{
	/// <summary>
	/// Defines the base class for popup (in the classic, Win32 sense) windows.
	/// </summary>
	/// <remarks>On load, this dialog will attempt to fixup the UI using <see cref="BootFX.Common.UI.Desktop.UIFixer"></see> to ensure buttons and other
	/// controls automatically appear in an XP style.</remarks>
	public class BaseForm : System.Windows.Forms.Form, ILoggable, IDialog
	{
		/// <summary>
		/// Private field to support <see cref="LoadCalled"/> property.
		/// </summary>
		private bool _loadCalled = false;
		
		/// <summary>
		/// Raised when an apply operation is successful.
		/// </summary>
		public event EventHandler ApplySuccessful;

		/// <summary>
		/// Raised when an apply operation failed.
		/// </summary>
		public event EventHandler ApplyFailed;
		
		/// <summary>
		/// Raised before changes are applied.
		/// </summary>
		public event CancelEventHandler Applying;

		/// <summary>
		/// Raised when form position is restored.
		/// </summary>
		public event EventHandler PositionRestored;
		
		/// <summary>
		/// Raised when the position is saved.
		/// </summary>
		public event EventHandler PositionSaved;
		
		/// <summary>
		/// Private field to support <c>AutoSavePosition</c> property.
		/// </summary>
		private bool _autoSavePosition = false;
		
		/// <summary>
		/// Private field to support <c>Logs</c> property.
		/// </summary>
		private ILogSet _logs = null;

		/// <summary>
		/// Private field to support <c>Background</c> property.
		/// </summary>
		private GraduatedBackground _background = GraduatedBackground.None;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public BaseForm()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			// icon...
			if(DesktopRuntime.IsStarted)
				this.Icon = DesktopRuntime.Current.DefaultFormIcon;
		}

		/// <summary>
		/// Fixes the UI.
		/// </summary>
		private void FixupUI()
		{
			UIFixer fixer = new UIFixer();
			fixer.Fixup(this);
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
			// 
			// BaseForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(292, 266);
			this.Font = new System.Drawing.Font("Tahoma", 8F);
			this.Name = "BaseForm";
			this.Text = "BaseForm";

		}
		#endregion

		/// <summary>
		/// Gets or sets the background.
		/// </summary>
		[Browsable(true), Category("Appearance"), Description("Gets or sets the background.")]
		public GraduatedBackground Background
		{
			get
			{
				return _background;
			}
			set
			{
				// check to see if the value has changed...
				if(value != _background)
				{
					// set the value...
					_background = value;
					this.Invalidate();
				}
			}
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			if(this.Background == GraduatedBackground.None)
				base.OnPaint (e);
			else
				PaintHelper.PaintGraduatedBackground(e.Graphics, this.Background, this.ClientRectangle, e.ClipRectangle);
		}

		protected override void OnResize(EventArgs e)
		{
			base.OnResize (e);
			if(this.DesignMode)
				return;

			// redraw...
			this.Invalidate();
		}
		
		ILog ILoggable.Log
		{
			get
			{
				return this.Log;
			}
		}

		/// <summary>
		/// Gets the default log.
		/// </summary>
		protected ILog Log
		{
			get
			{
				return this.Logs.DefaultLog;
			}
		}

		ILogSet ILoggable.Logs
		{
			get
			{
				return this.Logs;
			}
		}

		/// <summary>
		/// Gets the set of logs for other activities.
		/// </summary>
		protected ILogSet Logs
		{
			get
			{
				// mbr - 11-10-2005 - provided an ability to invalidate logs if the context changes...				
				if(_logs != null && _logs.ContextId != LogSet.CurrentContextId)
					_logs = null;
				if(_logs == null)
					_logs = new LogSet(this.GetType());
				return _logs;
			}
		}

		/// <summary>
		/// Position the form so that it is centered on the screen.
		/// </summary>
		public void CenterOnScreen()
		{
			this.SuspendLayout();
			try
			{
				// get...
				// TODO: handle multiple monitors...
				Rectangle rect = System.Windows.Forms.SystemInformation.WorkingArea;

				// x...
				int x = (rect.Left + (rect.Width / 2)) - (this.Width / 2);
				int y = (rect.Top + (rect.Height / 2)) - (this.Height / 2);

				// move...
				this.Location = new Point(x, y);
			}
			finally
			{
				this.ResumeLayout();
			}
		}

		protected override void OnHandleCreated(EventArgs e)
		{
			// mbr - 07-03-2006 - added.			
			base.OnHandleCreated (e);
			if(this.DesignMode)
				return;

			try
			{
				this.FixupUI();
			}
			catch(Exception ex)
			{
				Alert.ShowWarning(this, "Failed to fixup the UI.", ex);
			}
		}

		/// <summary>
		/// Gets or sets the autosaveposition
		/// </summary>
		[Category("Layout"), Description("Gets or sets whether to automatically save and restore the form position."), DefaultValue(false)]
		public bool AutoSavePosition
		{
			get
			{
				return _autoSavePosition;
			}
			set
			{
				// check to see if the value has changed...
				if(value != _autoSavePosition)
				{
					// set the value...
					_autoSavePosition = value;
				}
			}
		}

		protected override void OnClosed(EventArgs e)
		{
			// mbr - 25-08-2006 - added.			
			if(!(this.DesignMode))
			{
				if(this.AutoSavePosition)
				{
					try
					{
						this.SavePosition();
					}
					catch(Exception ex)
					{
                        this.LogWarn(() => "Failed to save position.", ex);
                    }
				}
			}

			// base...
			base.OnClosed (e);
		}

		/// <summary>
		/// Restores the position of the form.
		/// </summary>
		private void RestorePosition()
		{
			if(Runtime.IsStarted == false)
				return;

			// load it...
			FormPosition position = FormPosition.FromXmlString(Runtime.Current.UserSettings.GetStringValue(this.FormSaveName, null, Cultures.System, OnNotFound.ReturnNull));
			if(position != null)
				this.RestorePosition(position);
		}

		/// <summary>
		/// Restores the position of the form.
		/// </summary>
		/// <param name="position"></param>
		private void RestorePosition(FormPosition position)
		{
			if(position == null)
				throw new ArgumentNullException("position");
			
			// defer...
			position.Restore(this);

			// event...
			this.OnPositionRestored();
		}

		/// <summary>
		/// Raises the <c>PositionRestored</c> event.
		/// </summary>
		private void OnPositionRestored()
		{
			OnPositionRestored(EventArgs.Empty);
		}
		
		/// <summary>
		/// Raises the <c>PositionRestored</c> event.
		/// </summary>
		protected virtual void OnPositionRestored(EventArgs e)
		{
			// raise...
			if(PositionRestored != null)
				PositionRestored(this, e);
		}

		/// <summary>
		/// Saves the position of the form.
		/// </summary>
		private void SavePosition()
		{
			if(Runtime.IsStarted == false)
				return;

			// get the position...
			FormPosition position = new FormPosition(this);

			// save it...
			Runtime.Current.UserSettings[this.FormSaveName] = position.ToXmlString();

			// saved...
			this.OnPositionSaved();
		}

		/// <summary>
		/// Raises the <c>PositionSaved</c> event.
		/// </summary>
		private void OnPositionSaved()
		{
			OnPositionSaved(EventArgs.Empty);
		}
		
		/// <summary>
		/// Raises the <c>PositionSaved</c> event.
		/// </summary>
		protected virtual void OnPositionSaved(EventArgs e)
		{
			// raise...
			if(PositionSaved != null)
				PositionSaved(this, e);
		}

		/// <summary>
		/// Gets the form save name.
		/// </summary>
		private string FormSaveName
		{
			get
			{
				// mbr - 2008-11-26 - changed this as UserSettings uses SOAP for some historical reason and it cannot
				// load UI.Desktop.
				//return string.Format(@"FormSaveState\{0}", this.GetType().AssemblyQualifiedName);
				return string.Format(@"FormSaveState2\{0}, {1}", this.GetType().FullName, this.GetType().Assembly.GetName().Name);
			}
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad (e);
			if(this.DesignMode)
				return;

			// set...
			_loadCalled = true;
			
			// restore the form position if we're flagged to do so...
			if(this.AutoSavePosition)
			{
				try
				{
					this.RestorePosition();
				}
				catch(Exception ex)
				{
					Alert.ShowWarning(this, "Failed to restore the form position.", ex);
				}
			}
		}

		/// <summary>
		/// Applies changes to the dialog.
		/// </summary>
		/// <returns></returns>
		public bool Apply()
		{
			return this.Apply(ApplyReason.ManuallyInvoked);
		}

		bool IDialog.Apply(ApplyReason reason)
		{
			return this.Apply(reason);
		}

		/// <summary>
		/// Applies changes to the dialog.
		/// </summary>
		/// <returns></returns>
		private bool Apply(ApplyReason reason)
		{
			// before...
			CancelEventArgs e = new CancelEventArgs();
			this.OnApplying(e);
			if(e.Cancel)
				return false;

			// apply...
			bool result = this.DoApply(reason);

			// what happened...?
			if(result)
				this.OnApplySuccessful();
			else
				this.OnApplyFailed();

			// return...
			return result;
		}

		/// <summary>
		/// Applies changes to the dialog.
		/// </summary>
		/// <param name="reason"></param>
		/// <returns></returns>
		protected virtual bool DoApply(ApplyReason reason)
		{
			return true;
		}

		bool IDialog.DialogApply()
		{
			return this.DialogApply();
		}

		bool IDialog.DialogOK()
		{
			return this.DialogOK();
		}

		void IDialog.DialogCancel()
		{
			this.DialogCancel();
		}

		/// <summary>
		/// Called when the Apply button is pressed.
		/// </summary>
		/// <returns></returns>
		protected virtual bool DialogApply()
		{
			return this.Apply(ApplyReason.ApplyPressed);
		}

		/// <summary>
		/// Called when the OK button is pressed.
		/// </summary>
		/// <returns></returns>
		protected virtual bool DialogOK()
		{
			if(this.Apply(ApplyReason.OKPressed))
			{
				this.DialogResult = DialogResult.OK;
				return true;
			}
			else
				return false;
		}

		/// <summary>
		/// Called when the Cancel button is pressed.
		/// </summary>
		protected virtual void DialogCancel()
		{
			this.DialogResult = DialogResult.Cancel;
		}

		/// <summary>
		/// Raises the <c>Applying</c> event.
		/// </summary>
		protected virtual void OnApplying(CancelEventArgs e)
		{
			// raise...
			if(Applying != null)
				Applying(this, e);
		}

		/// <summary>
		/// Raises the <c>ApplyFailed</c> event.
		/// </summary>
		private void OnApplyFailed()
		{
			OnApplyFailed(EventArgs.Empty);
		}
		
		/// <summary>
		/// Raises the <c>ApplyFailed</c> event.
		/// </summary>
		protected virtual void OnApplyFailed(EventArgs e)
		{
			// raise...
			if(ApplyFailed != null)
				ApplyFailed(this, e);
		}
		
		/// <summary>
		/// Raises the <c>ApplySuccessful</c> event.
		/// </summary>
		private void OnApplySuccessful()
		{
			OnApplySuccessful(EventArgs.Empty);
		}
		
		/// <summary>
		/// Raises the <c>ApplySuccessful</c> event.
		/// </summary>
		protected virtual void OnApplySuccessful(EventArgs e)
		{
			// raise...
			if(ApplySuccessful != null)
				ApplySuccessful(this, e);
		}

//		protected override void OnHandleCreated(EventArgs e)
//		{
//			// mbr - 07-03-2006 - added.			
//			base.OnHandleCreated (e);
//			if(this.DesignMode)
//				return;
//
//			// are we parented by a modal window?
//			if(this.Owner != null && this.Owner.Modal)
//			{
//				// move...
//				this.StartPosition = FormStartPosition.Manual;
//
//				// where's the owner...
//				Rectangle ownerRect = this.Owner.RectangleToScreen(this.Owner.ClientRectangle);
//
//				// position us...
//				this.Location = new Point(((ownerRect.Left + (ownerRect.Width / 2)) - (this.Width / 2)) + 25, 
//					((ownerRect.Top + (ownerRect.Height/ 2)) - (this.Height / 2)) + 25);
//
//				// ensure...
//				FormPosition.EnsureIsOnScreen(this);
//			}
//		}

		/// <summary>
		/// Gets the loadcalled.
		/// </summary>
		public bool LoadCalled
		{
			get
			{
				return _loadCalled;
			}
		}
	}
}
