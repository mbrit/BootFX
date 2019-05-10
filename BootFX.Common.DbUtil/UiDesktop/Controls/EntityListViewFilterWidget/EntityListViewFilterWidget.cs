// BootFX - Application framework for .NET applications
// 
// File: EntityListViewFilterWidget.cs
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
	/// Summary description for EntityListViewFilterWidget.
	/// </summary>
	public class EntityListViewFilterWidget : UserControl
	{
		/// <summary>
		/// Private field to support <c>LazyTimer</c> property.
		/// </summary>
		private System.Timers.Timer _lazyTimer;
		
		/// <summary>
		/// Private field to support <c>InitializeCount</c> property.
		/// </summary>
		private int _initializeCount = 0;
		
		/// <summary>
		/// Private field to support <c>EntityListView</c> property.
		/// </summary>
		private EntityListView _entityListView;
		private System.Windows.Forms.Label labelFilter;
		private System.Windows.Forms.TextBox textFilter;
		private System.Windows.Forms.Button buttonFilter;
		private System.Windows.Forms.Panel panel1;
		
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public EntityListViewFilterWidget()
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
			this.DisposeLazyTimer();
			this.UnsubscribeListView();
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
			this.labelFilter = new System.Windows.Forms.Label();
			this.textFilter = new System.Windows.Forms.TextBox();
			this.buttonFilter = new System.Windows.Forms.Button();
			this.panel1 = new System.Windows.Forms.Panel();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// labelFilter
			// 
			this.labelFilter.Dock = System.Windows.Forms.DockStyle.Left;
			this.labelFilter.Location = new System.Drawing.Point(0, 0);
			this.labelFilter.Name = "labelFilter";
			this.labelFilter.Size = new System.Drawing.Size(40, 20);
			this.labelFilter.TabIndex = 0;
			this.labelFilter.Text = "&Filter:";
			this.labelFilter.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textFilter
			// 
			this.textFilter.AcceptsReturn = true;
			this.textFilter.Dock = System.Windows.Forms.DockStyle.Fill;
			this.textFilter.Location = new System.Drawing.Point(0, 0);
			this.textFilter.Name = "textFilter";
			this.textFilter.Size = new System.Drawing.Size(268, 20);
			this.textFilter.TabIndex = 1;
			this.textFilter.Text = "";
			this.textFilter.TextChanged += new System.EventHandler(this.textFilter_TextChanged);
			// 
			// buttonFilter
			// 
			this.buttonFilter.Dock = System.Windows.Forms.DockStyle.Right;
			this.buttonFilter.Location = new System.Drawing.Point(312, 0);
			this.buttonFilter.Name = "buttonFilter";
			this.buttonFilter.Size = new System.Drawing.Size(36, 20);
			this.buttonFilter.TabIndex = 2;
			this.buttonFilter.Text = "&Go";
			this.buttonFilter.Click += new System.EventHandler(this.buttonFilter_Click);
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.textFilter);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel1.DockPadding.Right = 4;
			this.panel1.Location = new System.Drawing.Point(40, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(272, 20);
			this.panel1.TabIndex = 3;
			// 
			// EntityListViewFilterWidget
			// 
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.buttonFilter);
			this.Controls.Add(this.labelFilter);
			this.Name = "EntityListViewFilterWidget";
			this.Size = new System.Drawing.Size(348, 20);
			this.panel1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// Gets or sets the entitylistview
		/// </summary>
		public EntityListView EntityListView
		{
			get
			{
				return _entityListView;
			}
			set
			{
				// check to see if the value has changed...
				if(value != _entityListView)
				{
					// set the value...
					_entityListView = value;
					this.RefreshView();
				}
			}
		}

		/// <summary>
		/// Subscribes to the list view.
		/// </summary>
		private void SubscribeListView()
		{
			if(this.EntityListView == null)
				return;

			// sub...
			this.EntityListView.FilterChanged += new EventHandler(EntityListView_FilterChanged);
		}

		/// <summary>
		/// Subscribes to the list view.
		/// </summary>
		private void UnsubscribeListView()
		{
			if(this.EntityListView == null)
				return;

			// sub...
			this.EntityListView.FilterChanged -= new EventHandler(EntityListView_FilterChanged);
		}

		/// <summary>
		/// Refreshes the view.
		/// </summary>
		private void RefreshView()
		{
			if(this.EntityListView != null)
				this.Enabled = true;
			else
				this.Enabled = false;
		}

		private void buttonFilter_Click(object sender, System.EventArgs e)
		{
			Filter();
		}

		/// <summary>
		/// Applies the filter.
		/// </summary>
		private void Filter()
		{
			// apply...
			if(this.EntityListView != null)
				this.EntityListView.Filter = this.textFilter.Text;
		}

		private void EntityListView_FilterChanged(object sender, EventArgs e)
		{
			if(EntityListView == null)
				throw new InvalidOperationException("EntityListView is null.");

			this.InitializeCount++;
			try
			{
				this.textFilter.Text = this.EntityListView.Filter;
			}
			finally
			{
				this.InitializeCount--;
			}
		}

		private void textFilter_TextChanged(object sender, System.EventArgs e)
		{
			this.DisposeLazyTimer();
			_lazyTimer = new System.Timers.Timer(333);
			this.LazyTimer.Elapsed += new System.Timers.ElapsedEventHandler(LazyTimer_Elapsed);
			this.LazyTimer.Enabled = true;
		}

		/// <summary>
		/// Gets or sets the initializecount
		/// </summary>
		public int InitializeCount
		{
			get
			{
				return _initializeCount;
			}
			set
			{
				// check to see if the value has changed...
				if(value != _initializeCount)
				{
					// set the value...
					_initializeCount = value;
				}
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

		private void LazyTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			// filter...
			this.DisposeLazyTimer();
			this.Filter();
		}
	}
}
