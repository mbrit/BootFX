// BootFX - Application framework for .NET applications
// 
// File: IntegerComparer.cs
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
	/// Defines a comparer implementation that compares integer values.
	/// </summary>
	public class IntegerComparer : ComparerBase
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		public IntegerComparer(CultureInfo culture) : base(culture)
		{
		}

        ///// <summary>
        ///// Constructor.
        ///// </summary>
        ///// <param name="direction"></param>
        //public IntegerComparer(CultureInfo culture, SortDirection direction) : base(culture, direction)
        //{
        //}

		protected override int DoCompare(object x, object y)
		{
			if(x == null)
				x = 0;
			if(y == null)
				y = 0;

			// convert...
			long longX = Convert.ToInt64(x, this.Culture);
			long longY = Convert.ToInt64(y, this.Culture);

			// check...
			if(longX == longY)
				return 0;
			if(longX < longY)
				return -1;
			else
				return 1;
		}
	}
}
