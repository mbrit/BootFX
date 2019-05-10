// BootFX - Application framework for .NET applications
// 
// File: ValueFormatter.cs
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
using System.Globalization;
using BootFX.Common.Data;

namespace BootFX.Common.UI
{
	/// <summary>
	/// Defines a class that can format values for UI controls.
	/// </summary>
	public sealed class ValueFormatter
	{
		private ValueFormatter()
		{
		}

		/// <summary>
		/// Formats a value.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static string FormatValue(object value, CultureInfo culture)
		{
			// do we have a value?
			if(value == null || value is DBNull)
				return string.Empty;
			else
			{
				// convert it...
				if(value is string)
					return ((string)value).TrimEnd();
				else
				{
					string asString = ConversionHelper.ToString(value, culture);
					return asString;
				}
			}
		}
	}
}
