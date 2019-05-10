// BootFX - Application framework for .NET applications
// 
// File: StringComparer.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Globalization;
using System.ComponentModel;
using BootFX.Common.Data;

namespace BootFX.Common.Data.Comparers
{
	/// <summary>
	/// Defines a comparer implementation that compares string values.
	/// </summary>
	/// <remarks>By default, comparison is case-insensitive.  You can change this using the <see cref="IgnoreCase"></see> property.</remarks>
	public class StringComparer : ComparerBase
	{
		/// <summary>
		/// Private field to support <c>IgnoreCase</c> property.
		/// </summary>
		private bool _ignoreCase;

		/// <summary>
		/// Constructor,
		/// </summary>
		public StringComparer(CultureInfo culture) : base(culture)
		{
		}

        ///// <summary>
        ///// Constructor.
        ///// </summary>
        ///// <param name="direction"></param>
        //public StringComparer(CultureInfo culture, SortDirection direction) : base(culture, direction)
        //{
        //}

		/// <summary>
		/// Compares two strings.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		protected override int DoCompare(object x, object y)
		{
			string stringX = Convert.ToString(x, this.Culture);
			string stringY = Convert.ToString(y, this.Culture);

			// return...
			return string.Compare(stringX, stringY, this.IgnoreCase, this.Culture);
		}

		/// <summary>
		/// Gets or sets whether to perform case-insensitive comparisons.
		/// </summary>
		/// <remarks>By default, the culture of this comparer is <see cref="CultureInfo.InvariantCulture"></see>.  However, if you
		/// choose not to ignore case, you may wish to ensure that the culture is set appropriately.</remarks>
		public bool IgnoreCase
		{
			get
			{
				return _ignoreCase;
			}
			set
			{
				// check to see if the value has changed...
				if(value != _ignoreCase)
				{
					// set the value...
					_ignoreCase = value;
				}
			}
		}
	}
}
