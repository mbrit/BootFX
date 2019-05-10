// BootFX - Application framework for .NET applications
// 
// File: Dialog.cs
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

namespace BootFX.Common.UI.Desktop
{
	/// <summary>
	///	 Describes a dialog.
	/// </summary>
	/// <remarks>This defines a class that is centered on the parent, has no minimize or maximize buttons.</remarks>
	[Obsolete("Do not use - inherit from BaseForm.")]
	public class Dialog : BaseForm, IDialog
	{
//		/// <summary>
//		/// Raised when an apply operation is successful.
//		/// </summary>
//		public event EventHandler ApplySuccessful;
//
//		/// <summary>
//		/// Raised when an apply operation failed.
//		/// </summary>
//		public event EventHandler ApplyFailed;
//		
//		/// <summary>
//		/// Raised before changes are applied.
//		/// </summary>
//		public event CancelEventHandler Applying;
		
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public Dialog()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
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
			// Dialog
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(528, 394);
			this.Font = new System.Drawing.Font("Tahoma", 8F);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "Dialog";
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Dialog";

		}
		#endregion

//		/// <summary>
//		/// Applies changes to the dialog.
//		/// </summary>
//		/// <returns></returns>
//		public bool Apply()
//		{
//			return this.Apply(ApplyReason.ManuallyInvoked);
//		}
//
//		bool IDialog.Apply(ApplyReason reason)
//		{
//			return this.Apply(reason);
//		}
//
//		/// <summary>
//		/// Applies changes to the dialog.
//		/// </summary>
//		/// <returns></returns>
//		private bool Apply(ApplyReason reason)
//		{
//			// before...
//			CancelEventArgs e = new CancelEventArgs();
//			this.OnApplying(e);
//			if(e.Cancel)
//				return false;
//
//			// apply...
//			bool result = this.DoApply(reason);
//
//			// what happened...?
//			if(result)
//				this.OnApplySuccessful();
//			else
//				this.OnApplyFailed();
//
//			// return...
//			return result;
//		}
//
//		/// <summary>
//		/// Applies changes to the dialog.
//		/// </summary>
//		/// <param name="reason"></param>
//		/// <returns></returns>
//		protected virtual bool DoApply(ApplyReason reason)
//		{
//			return true;
//		}
//
//		bool IDialog.DialogApply()
//		{
//			return this.DialogApply();
//		}
//
//		bool IDialog.DialogOK()
//		{
//			return this.DialogOK();
//		}
//
//		void IDialog.DialogCancel()
//		{
//			this.DialogCancel();
//		}
//
//		/// <summary>
//		/// Called when the Apply button is pressed.
//		/// </summary>
//		/// <returns></returns>
//		protected virtual bool DialogApply()
//		{
//			return this.Apply(ApplyReason.ApplyPressed);
//		}
//
//		/// <summary>
//		/// Called when the OK button is pressed.
//		/// </summary>
//		/// <returns></returns>
//		protected virtual bool DialogOK()
//		{
//			if(this.Apply(ApplyReason.OKPressed))
//			{
//				this.DialogResult = DialogResult.OK;
//				return true;
//			}
//			else
//				return false;
//		}
//
//		/// <summary>
//		/// Called when the Cancel button is pressed.
//		/// </summary>
//		protected virtual void DialogCancel()
//		{
//			this.DialogResult = DialogResult.Cancel;
//		}
//
//		/// <summary>
//		/// Raises the <c>Applying</c> event.
//		/// </summary>
//		protected virtual void OnApplying(CancelEventArgs e)
//		{
//			// raise...
//			if(Applying != null)
//				Applying(this, e);
//		}
//
//		/// <summary>
//		/// Raises the <c>ApplyFailed</c> event.
//		/// </summary>
//		private void OnApplyFailed()
//		{
//			OnApplyFailed(EventArgs.Empty);
//		}
//		
//		/// <summary>
//		/// Raises the <c>ApplyFailed</c> event.
//		/// </summary>
//		protected virtual void OnApplyFailed(EventArgs e)
//		{
//			// raise...
//			if(ApplyFailed != null)
//				ApplyFailed(this, e);
//		}
//		
//		/// <summary>
//		/// Raises the <c>ApplySuccessful</c> event.
//		/// </summary>
//		private void OnApplySuccessful()
//		{
//			OnApplySuccessful(EventArgs.Empty);
//		}
//		
//		/// <summary>
//		/// Raises the <c>ApplySuccessful</c> event.
//		/// </summary>
//		protected virtual void OnApplySuccessful(EventArgs e)
//		{
//			// raise...
//			if(ApplySuccessful != null)
//				ApplySuccessful(this, e);
//		}
//
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
	}
}
