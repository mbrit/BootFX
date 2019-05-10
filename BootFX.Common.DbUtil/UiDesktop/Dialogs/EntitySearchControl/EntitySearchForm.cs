// BootFX - Application framework for .NET applications
// 
// File: EntitySearchForm.cs
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
using BootFX.Common.Entities;

namespace BootFX.Common.UI.Desktop
{
	/// <summary>
	/// Summary description for EntitySearchForm.
	/// </summary>
	public class EntitySearchForm : BaseForm
	{
		/// <summary>
		/// Raised when the user double-clicks an entity.
		/// </summary>
		public event EntityEventHandler EntityDoubleClick;
		
		private BootFX.Common.UI.Desktop.EntitySearchControl search;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public EntitySearchForm()
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
			this.search = new BootFX.Common.UI.Desktop.EntitySearchControl();
			this.SuspendLayout();
			// 
			// search
			// 
			this.search.Dock = System.Windows.Forms.DockStyle.Fill;
			this.search.EntityType = null;
			this.search.Location = new System.Drawing.Point(4, 4);
			this.search.Name = "search";
			this.search.Size = new System.Drawing.Size(560, 330);
			this.search.TabIndex = 4;
			this.search.EntityDoubleClick += new BootFX.Common.Entities.EntityEventHandler(this.search_EntityDoubleClick);
			// 
			// EntitySearchForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(568, 342);
			this.Controls.Add(this.search);
			this.DockPadding.Bottom = 8;
			this.DockPadding.Left = 4;
			this.DockPadding.Right = 4;
			this.DockPadding.Top = 4;
			this.Name = "EntitySearchForm";
			this.Text = "Find Items";
			this.ResumeLayout(false);

		}
		#endregion

		private void search_EntityDoubleClick(object sender, BootFX.Common.Entities.EntityEventArgs e)
		{
			this.OnEntityDoubleClick(e);
		}

		/// <summary>
		/// Raises the <c>EntityDoubleClick</c> event.
		/// </summary>
		protected virtual void OnEntityDoubleClick(EntityEventArgs e)
		{
			// raise...
			if(EntityDoubleClick != null)
				EntityDoubleClick(this, e);
		}

		/// <summary>
		/// Gets or sets the entity type.
		/// </summary>
		public EntityType EntityType
		{
			get
			{
				return this.search.EntityType;
			}
			set
			{
				this.search.EntityType = value;
			}
		}

		/// <summary>
		/// Gets the viewexpressions.
		/// </summary>
		public ArrayList ViewExpressions
		{
			get
			{
				return search.ViewExpressions;
			}
		}
	}
}
