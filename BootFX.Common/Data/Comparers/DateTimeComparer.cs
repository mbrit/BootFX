// BootFX - Application framework for .NET applications
// 
// File: DateTimeComparer.cs
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
	/// Defines a comparer implementation that compares date/time values.
	/// </summary>
	public class DateTimeComparer : ComparerBase
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		public DateTimeComparer(CultureInfo culture) : base(culture)
		{
		}

        ///// <summary>
        ///// Constructor.
        ///// </summary>
        ///// <param name="direction"></param>
        //public DateTimeComparer(CultureInfo culture, SortDirection direction) : base(culture, direction)
        //{
        //}

		protected override int DoCompare(object x, object y)
		{
			if(x == null)
				x = DateTime.MinValue;
			if(y == null)
				y = DateTime.MinValue;

			// convert..
			DateTime dateX = Convert.ToDateTime(x, this.Culture);
			DateTime dateY = Convert.ToDateTime(y, this.Culture);

			// check...
			if(dateX == dateY)
				return 0;
			if(dateX < dateY)
				return -1;
			else
				return 1;
		}
	}
}
