// BootFX - Application framework for .NET applications
// 
// File: GridSettings.cs
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
using BootFX.Common;

namespace BootFX.Common.UI
{
	/// <summary>
	/// Defines an instance of <c>GridSettings</c>.
	/// </summary>
	public class GridSettings : SimpleXmlPropertyBag
	{
		/// <summary>
		/// Private field to support <c>Columns</c> property.
		/// </summary>
		private GridSettingsColumnCollection _columns = new GridSettingsColumnCollection();

		/// <summary>
		/// Constructor.
		/// </summary>
		public GridSettings()
		{
		}

		/// <summary>
		/// Gets a collection of GridSettingsColumn objects.
		/// </summary>
		public GridSettingsColumnCollection Columns
		{
			get
			{
				return _columns;
			}
		}
	}
}
