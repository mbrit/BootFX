// BootFX - Application framework for .NET applications
// 
// File: SharedSettingsItemAttribute.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;

namespace BootFX.Common.Management
{
	/// <summary>
	/// An attribute that indicates that a class contains shared settings.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
	public sealed class SharedSettingsItemAttribute : Attribute
	{
		/// <summary>
		/// Private field to support <see cref="DefaultValue"/> property.
		/// </summary>
		private object _defaultValue;
		
		/// <summary>
		/// Constructor.
		/// </summary>
		public SharedSettingsItemAttribute(object defaultValue)
		{
			_defaultValue = defaultValue;
		}

		/// <summary>
		/// Gets the defaultvalue.
		/// </summary>
		internal object DefaultValue
		{
			get
			{
				return _defaultValue;
			}
		}
	}
}
