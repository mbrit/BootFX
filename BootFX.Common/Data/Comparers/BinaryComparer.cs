// BootFX - Application framework for .NET applications
// 
// File: BinaryComparer.cs
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
	/// Defines a comparer implementation that compares binary values.
	/// </summary>
	public class BinaryComparer : ComparerBase
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		public BinaryComparer(CultureInfo culture) : base(culture)
		{
		}

        ///// <summary>
        ///// Constructor.
        ///// </summary>
        ///// <param name="direction"></param>
        //public BinaryComparer(CultureInfo culture, SortDirection direction) : base(culture, direction)
        //{
        //}

		protected override int DoCompare(object x, object y)
		{
			// TODO: implement...
			return 0;
		}
	}
}
