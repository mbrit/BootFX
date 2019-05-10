// BootFX - Application framework for .NET applications
// 
// File: EntityListViewColumnHeader.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Windows.Forms;
using System.ComponentModel;
using BootFX.Common.Entities;

namespace BootFX.Common.UI.Desktop
{
	/// <summary>
	/// Defines the column header used for <see cref="EntityListView"></see>.
	/// </summary>
	internal class EntityListViewColumnHeader : ColumnHeader
	{
		/// <summary>
		/// Private field to support <c>Property</c> property.
		/// </summary>
		private PropertyDescriptor _property;
		
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="property"></param>
		public EntityListViewColumnHeader(PropertyDescriptor property)
		{
			if(property == null)
				throw new ArgumentNullException("property");
			
			// set...
			_property = property;

			// setup...
			if(property is PropertyDescriptorBase)
				this.Text = ((PropertyDescriptorBase)property).CustomDisplayName;
			else
				this.Text = property.Name;

			// set...
			this.Width = 100;
			this.TextAlign = HorizontalAlignment.Left;
		}

		/// <summary>
		/// Gets the property.
		/// </summary>
		public PropertyDescriptor Property
		{
			get
			{
				return _property;
			}
		}
	}
}
