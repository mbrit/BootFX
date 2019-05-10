// BootFX - Application framework for .NET applications
// 
// File: DateTimePickerMemberEditor.cs
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
	internal class DateTimePickerMemberEditor : DateTimePicker, IMemberEditor
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		public DateTimePickerMemberEditor()
		{
		}

		/// <summary>
		/// Gets or sets the value.
		/// </summary>
		object IMemberEditor.Value
		{
			get
			{
				return this.Value;
			}
			set
			{
				if(value == null)
					this.Value = DateTime.MinValue;
				else if(value is DateTime)
					this.Value = (DateTime)value;
				else
					this.Value = ConversionHelper.ToDateTime(value, Cultures.User);
			}
		}

		/// <summary>
		/// Initializes the view.
		/// </summary>
		public void Initializing(EntityMemberBox box, EntityMember member)
		{
			if(box == null)
				throw new ArgumentNullException("box");
		}

		public void Initialized()
		{
		}
	}
}
