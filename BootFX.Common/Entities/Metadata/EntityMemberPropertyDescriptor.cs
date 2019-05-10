// BootFX - Application framework for .NET applications
// 
// File: EntityMemberPropertyDescriptor.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.ComponentModel;
using BootFX.Common.Management;

namespace BootFX.Common.Entities
{
	/// <summary>
	/// Summary description for EntityMemberPropertyDescriptor.
	/// </summary>
	public abstract class PropertyDescriptorBase : LoggablePropertyDescriptor
	{
		/// <summary>
		/// Private field to support <c>DisplayName</c> property.
		/// </summary>
		private string _customDisplayName;
		
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="propertyName"></param>
		/// <param name="attributes"></param>
		protected PropertyDescriptorBase(string propertyName, Attribute[] attributes) : base(propertyName, attributes)
		{
		}

		/// <summary>
		/// Gets or sets the displayname
		/// </summary>
		public string CustomDisplayName
		{
			get
			{
				return _customDisplayName;
			}
			set
			{
				// check to see if the value has changed...
				if(value != _customDisplayName)
				{
					// set the value...
					_customDisplayName = value;
				}
			}
		}
	}
}
