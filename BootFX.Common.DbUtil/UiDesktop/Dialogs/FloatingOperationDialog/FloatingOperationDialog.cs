// BootFX - Application framework for .NET applications
// 
// File: FloatingOperationDialog.cs
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

namespace BootFX.Common.UI.Desktop
{
	/// <summary>
	/// Summary description for FloatingOperationDialog.
	/// </summary>
	public class FloatingOperationDialog : BaseForm
	{
		/// <summary>
		/// Delegate for <see cref="ForceClose"></see>.
		/// </summary>
		internal delegate void ForceCloseDelegate();

		/// <summary>
		/// Private field to support <c>AllowClose</c> property.
		/// </summary>
		private bool _allowClose = false;
		
		/// <summary>
		/// Private field to support <c>DefaultBar</c> property.
		/// </summary>
		private FloatingOperationDialogBar _defaultBar = new FloatingOperationDialogBar();
		
		/// <summary>
		/// Delegate for <see cref="AddBar"></see>.
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		private delegate FloatingOperationDialogBar AddBarDelegate(IOperationItem item);

		/// <summary>
		/// Delegate for <see cref="RemoveBar"></see>.
		/// </summary>
		private delegate void RemoveBarDelegate(IOperationItem item);

		/// <summary>
		/// Delegate for use with <see cref="RefreshView"></see>.
		/// </summary>
		private delegate void RefreshViewDelegate();

		/// <summary>
		/// Private field to support <c>CurrentItem</c> property.
		/// </summary>
		private IOperationItem _currentItem;
		private System.Windows.Forms.Panel panelBars;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.Timer timerSample;
		private System.ComponentModel.IContainer components;

		/// <summary>
		/// Constructor.
		/// </summary>
		public FloatingOperationDialog()
		{
			InitializeComponent();
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		public FloatingOperationDialog(string status) : this()
		{
			if(status == null)
				throw new ArgumentNullException("status");
			if(status.Length == 0)
				throw new ArgumentOutOfRangeException("'status' is zero-length.");
			this.SetupItem(status);
		}

		/// <summary>
		/// Displays the default bar.
		/// </summary>
		private void ShowDefaultBar()
		{
			this.panelBars.SuspendLayout();
			try
			{
				this.panelBars.Controls.Clear();
				this.panelBars.Controls.Add(_defaultBar);
			}
			finally
			{
				this.panelBars.ResumeLayout();
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
			this.components = new System.ComponentModel.Container();
			this.panelBars = new System.Windows.Forms.Panel();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.timerSample = new System.Windows.Forms.Timer(this.components);
			this.SuspendLayout();
			// 
			// panelBars
			// 
			this.panelBars.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.panelBars.Location = new System.Drawing.Point(52, 4);
			this.panelBars.Name = "panelBars";
			this.panelBars.Size = new System.Drawing.Size(264, 44);
			this.panelBars.TabIndex = 0;
			// 
			// pictureBox1
			// 
			this.pictureBox1.Location = new System.Drawing.Point(8, 8);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(32, 32);
			this.pictureBox1.TabIndex = 1;
			this.pictureBox1.TabStop = false;
			this.pictureBox1.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBox1_Paint);
			// 
			// timerSample
			// 
			this.timerSample.Interval = 500;
			this.timerSample.Tick += new System.EventHandler(this.timerSample_Tick);
			// 
			// FloatingOperationDialog
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(322, 52);
			this.Controls.Add(this.pictureBox1);
			this.Controls.Add(this.panelBars);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "FloatingOperationDialog";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Working, please wait...";
			this.Load += new System.EventHandler(this.FloatingOperationDialog_Load);
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// Creates an item to use with the dialog.
		/// </summary>
		/// <returns></returns>
		private void SetupItem(string status)
		{
			if(status == null)
				throw new ArgumentNullException("status");
			if(status.Length == 0)
				throw new ArgumentOutOfRangeException("'status' is zero-length.");

			OperationItem item = new OperationItem();
			item.Status = status;
			this.AddBar(item);
		}

		/// <summary>
		/// Gets or sets the currentitem
		/// </summary>
		public IOperationItem CurrentItem
		{
			get
			{
				return _currentItem;
			}
		}

		/// <summary>
		/// Called when the progress bar has changed.
		/// </summary>
		internal void RefreshStatus()
		{
			// no need for multiple versions of this method as they are both much of a muchness.
			// TODO: make this more sophisticated - prevent multiple calls in a small period of time.
			this.RefreshView();
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		public void RefreshView()
		{
			if(this.InvokeRequired)
			{
				RefreshViewDelegate d = new RefreshViewDelegate(this.RefreshView);
				this.Invoke(d);
				return;
			}

			// set...
			IOperationItem item = this.CurrentItem;
			if(item != null)
			{
				// get the bar to display status information in...
				FloatingOperationDialogBar bar = this.GetBarForItem(item);
				if(bar != null)
					bar.RefreshView();
			}
			else
			{
				if(this.IsDefaultBarVisible)
					this.DefaultBar.Status = this.Caption;
			}
		}

		/// <summary>
		/// Gets the bar for the given item.
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		private FloatingOperationDialogBar GetBarForItem(IOperationItem item)
		{
			if(item == null)
				throw new ArgumentNullException("item");
			
			// find the bar with the given item...
			FloatingOperationDialogBar bar = null;
			foreach(FloatingOperationDialogBar scanBar in this.panelBars.Controls)
			{
				if(scanBar.Item == item)
				{
					bar = scanBar;
					break;
				}
			}

			// not found... create one...
			if(bar == null)
				bar = this.AddBar(item);

			// return...
			return bar;
		}

		/// <summary>
		/// Gets the defaultbar.
		/// </summary>
		private FloatingOperationDialogBar DefaultBar
		{
			get
			{
				// returns the value...
				return _defaultBar;
			}
		}

		/// <summary>
		/// Adds a bar to the view.
		/// </summary>
		/// <returns></returns>
		private FloatingOperationDialogBar AddBar(IOperationItem item)
		{
			if(item == null)
				throw new ArgumentNullException("item");

			// invoke?
			if(this.InvokeRequired)
			{
				// flip...
				AddBarDelegate d = new AddBarDelegate(this.AddBar);
				return (FloatingOperationDialogBar)this.Invoke(d, new object[] { item });
			}
			
			// layout...
			this.panelBars.SuspendLayout();
			try
			{
				// clear...
				if(this.IsDefaultBarVisible)
					this.panelBars.Controls.Clear();

				// create...
				FloatingOperationDialogBar bar = new FloatingOperationDialogBar(item);
				bar.Dock = DockStyle.Top;
				this.panelBars.Controls.Add(bar);

				// listen...
				item.Finished += new EventHandler(item_Finished);

				// set...
				_currentItem = item;

				// return...
				return bar;
			}
			finally
			{
				this.panelBars.ResumeLayout();
			}
		}

		private void item_Finished(object sender, EventArgs e)
		{
			RemoveBar((IOperationItem)sender);
		}

		/// <summary>
		/// Removes the bar from the view.
		/// </summary>
		/// <param name="item"></param>
		private void RemoveBar(IOperationItem item)
		{
			if(item == null)
				throw new ArgumentNullException("item");

			// invoke...
			if(this.InvokeRequired)
			{
				// flip...
				RemoveBarDelegate d = new RemoveBarDelegate(this.RemoveBar);
				this.Invoke(d, new object[] { item });
				return;
			}

			// layout...
			this.panelBars.SuspendLayout();
			try
			{
				// fins...
				FloatingOperationDialogBar bar = this.GetBarForItem(item);
				if(bar != null)
				{
					// unsub...
					item.Finished -= new EventHandler(item_Finished);

					// remove...
					this.panelBars.Controls.Remove(bar);

					// count?
					if(this.panelBars.Controls.Count == 0)
						ShowDefaultBar();
				}
			}
			finally
			{
				this.panelBars.ResumeLayout();
			}
		}

		private void pictureBox1_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			e.Graphics.DrawIconUnstretched(SystemIcons.Information, this.pictureBox1.ClientRectangle);
		}

		/// <summary>
		/// Returns true if the default bar is visible.
		/// </summary>
		private bool IsDefaultBarVisible
		{
			get
			{
				if(this.panelBars.Controls.Count > 0 && this.panelBars.Controls[0] == this.DefaultBar)
					return true;
				else
					return false;
			}
		}

		/// <summary>
		/// Gets or sets the caption.
		/// </summary>
		public string Caption
		{
			get
			{
				if(DefaultBar == null)
					throw new InvalidOperationException("DefaultBar is null.");
				return this.DefaultBar.Status;
			}
			set
			{
				if(DefaultBar == null)
					throw new InvalidOperationException("DefaultBar is null.");
				this.DefaultBar.Status = value;
			}
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			base.OnClosing (e);

			// stop...
			if(this.AllowClose == false)
			{
				e.Cancel = true;
				this.Visible = false;
			}
		}

		/// <summary>
		/// Gets the allowclose.
		/// </summary>
		internal bool AllowClose
		{
			get
			{
				// returns the value...
				return _allowClose;
			}
			set
			{
				_allowClose = value;
			}
		}

		/// <summary>
		/// Closes the dialog.
		/// </summary>
		internal void ForceClose()
		{
			// flip...
			if(this.InvokeRequired)
			{
				ForceCloseDelegate d = new ForceCloseDelegate(this.ForceClose);
				this.Invoke(d);
				return;
			}

			// stop...
			_allowClose = true;
			this.DialogResult = DialogResult.OK;
		}

		private void FloatingOperationDialog_Load(object sender, System.EventArgs e)
		{
			if(this.DesignMode)
				return;

			// sample...
			this.timerSample.Enabled = true;
		}

		private void timerSample_Tick(object sender, System.EventArgs e)
		{
			try
			{
				if(this.CurrentItem != null)
				{
					FloatingOperationDialogBar bar = this.GetBarForItem(this.CurrentItem);
					if(bar != null)
						bar.RefreshView();
				}
			}
			catch (Exception ex)
			{
				this.timerSample.Enabled = false;
				Alert.ShowWarning(this, "Failed to update operation bar.", ex);
			}
		}
	}
}
