// BootFX - Application framework for .NET applications
// 
// File: GridSettingsColumn.cs
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
	/// Defines an instance of <c>GridSettingsColumn</c>.
	/// </summary>
	public class GridSettingsColumn : SimpleXmlPropertyBag
	{
		public GridSettingsColumn(string name, int width)
		{
			if(name == null)
				throw new ArgumentNullException("name");
			if(name.Length == 0)
				throw new ArgumentOutOfRangeException("'name' is zero-length.");
			
			// set...
			this.SetValue("Name", name);
			this.SetValue("Width", width);
		}

		public string Name
		{
			get
			{
				return this.GetStringValue("Name", null, Cultures.System, OnNotFound.ThrowException);
			}
		}

		public int Width
		{
			get
			{
				return this.GetInt32Value("Width", 0, Cultures.System, OnNotFound.ThrowException);
			}
		}
	}
}
