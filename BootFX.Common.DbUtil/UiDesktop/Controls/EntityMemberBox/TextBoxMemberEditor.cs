// BootFX - Application framework for .NET applications
// 
// File: TextBoxMemberEditor.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Windows.Forms;
using BootFX.Common.Data;
using BootFX.Common.Entities;

namespace BootFX.Common.UI.Desktop
{
	/// <summary>
	/// Summary description for TextMemberEditor.
	/// </summary>
	internal class TextBoxMemberEditor : TextBox, IMemberEditor
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		public TextBoxMemberEditor()
		{
		}

		/// <summary>
		/// Gets or sets the value.
		/// </summary>
		public object Value
		{
			get
			{
				return this.Text;
			}
			set
			{
				if(value == null)
					this.Text = string.Empty;
				else if(value is string)
					this.Text = (string)value;
				else
					this.Text = ConversionHelper.ToString(value, Cultures.User);
			}
		}

		/// <summary>
		/// Initializes the view.
		/// </summary>
		public void Initializing(EntityMemberBox box, EntityMember member)
		{
			if(box == null)
				throw new ArgumentNullException("box");

			// readonly...
			this.ReadOnly = box.ReadOnly;
		}

		public void Initialized()
		{
		}
	}
}
