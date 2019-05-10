// BootFX - Application framework for .NET applications
// 
// File: DecimalComparer.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Globalization;

namespace BootFX.Common.Data.Comparers
{
	/// <summary>
	/// Defines a comparer implementation that compares decimal values.
	/// </summary>
	public class DecimalComparer : ComparerBase
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		public DecimalComparer(CultureInfo culture) : base(culture)
		{
		}

        ///// <summary>
        ///// Constructor.
        ///// </summary>
        ///// <param name="direction"></param>
        //public DecimalComparer(CultureInfo culture, SortDirection direction) : base(culture, direction)
        //{
        //}

		protected override int DoCompare(object x, object y)
		{
			if(x == null)
				x = 0;
			if(y == null)
				y = 0;

			// convert...
			decimal decimalX = Convert.ToDecimal(x, this.Culture);
			decimal decimalY = Convert.ToDecimal(y, this.Culture);

			// check...
			if(decimalX == decimalY)
				return 0;
			if(decimalX < decimalY)
				return -1;
			else
				return 1;
		}
	}
}
