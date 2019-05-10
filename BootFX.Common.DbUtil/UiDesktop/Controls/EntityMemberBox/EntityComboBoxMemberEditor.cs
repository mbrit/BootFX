// BootFX - Application framework for .NET applications
// 
// File: EntityComboBoxMemberEditor.cs
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
using BootFX.Common.Data;
using BootFX.Common.Entities;

namespace BootFX.Common.UI.Desktop
{
	/// <summary>
	/// Summary description for EntityComboBoxMemberEditor.
	/// </summary>
	internal class EntityComboBoxMemberEditor : EntityComboBox, IMemberEditor
	{
		/// <summary>
		/// Private field to support <c>Member</c> property.
		/// </summary>
		private EntityMember _member;
		
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public EntityComboBoxMemberEditor()
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
			this.components = new System.ComponentModel.Container();
			this.Size = new System.Drawing.Size(300,300);
			this.Text = "EntityComboBoxMemberEditor";
		}
		#endregion

		/// <summary>
		/// Gets or sets the value.
		/// </summary>
		public object Value
		{
			get
			{
				return this.SelectedEntity;
			}
			set
			{
				this.SelectedEntity = value;
			}
		}

		/// <summary>
		/// Initializes the view.
		/// </summary>
		public void Initializing(EntityMemberBox box, EntityMember member)
		{
			if(box == null)
				throw new ArgumentNullException("box");
			if(member == null)
				throw new ArgumentNullException("member");

			// set...
			_member = member;
		}

		/// <summary>
		/// Gets the member.
		/// </summary>
		private EntityMember Member
		{
			get
			{
				// returns the value...
				return _member;
			}
		}

		/// <summary>
		/// Initializes the view.
		/// </summary>
		/// <param name="member"></param>
		private void Initializing(ChildToParentEntityLink link)
		{
			if(link == null)
				throw new ArgumentNullException("link");
			
			// get the fields...
			EntityField[] fields = link.GetLinkFields();
			if(fields == null)
				throw new InvalidOperationException("fields is null.");
			this.AllowNullSelection = false;
			foreach(EntityField field in fields)
			{
				if(field.IsNullable())
				{
					this.AllowNullSelection = true;
					break;
				}
			}
		}

		/// <summary>
		/// Called after initialization has completed.
		/// </summary>
		/// <param name="link"></param>
		private void Initialized(ChildToParentEntityLink link)
		{
			if(link == null)
				throw new ArgumentNullException("link");

			// do we have a list?
			if(this.DataSource != null)
				return;
			
			// get the items...
			if(link.ParentEntityType == null)
				throw new InvalidOperationException("link.ParentEntityType is null.");
			IEntityPersistence persistence = link.ParentEntityType.Persistence;
			if(persistence == null)
				throw new InvalidOperationException("persistence is null.");

			// get...
			this.DataSource = persistence.GetAll();
		}

		public void Initialized()
		{
			if(this.Member is ChildToParentEntityLink)
				this.Initialized((ChildToParentEntityLink)this.Member);
		}
	}
}
