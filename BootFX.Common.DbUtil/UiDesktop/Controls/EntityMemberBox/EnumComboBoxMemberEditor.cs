// BootFX - Application framework for .NET applications
// 
// File: EnumComboBoxMemberEditor.cs
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
	/// Summary description for EnumComboBoxMemberEditor.
	/// </summary>
	internal class EnumComboBoxMemberEditor : EnumComboBox, IMemberEditor
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public EnumComboBoxMemberEditor()
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
			this.Text = "EnumComboBoxMemberEditor";
		}
		#endregion

		object IMemberEditor.Value
		{
			get
			{
				return this.Value;
			}
			set
			{
				this.Value = value;
			}
		}

		public void Initializing(EntityMemberBox box, BootFX.Common.Entities.EntityMember member)
		{
			if(box == null)
				throw new ArgumentNullException("box");
			if(member == null)
				throw new ArgumentNullException("member");

			// set...
			if(member is EntityField)
			{
				EntityField field = (EntityField)member;
				if(field.EnumerationType != null)
					this.EnumType = field.EnumerationType;
				else
					throw new InvalidOperationException(string.Format("Field '{0}' does not have an enumeration mapping.", member));
			}
			else
				throw new NotSupportedException(string.Format("Cannot handle '{0}'.", member));
		}

		public void Initialized()
		{
		}

	}
}
