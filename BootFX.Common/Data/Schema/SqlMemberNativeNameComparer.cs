// BootFX - Application framework for .NET applications
// 
// File: SqlMemberNativeNameComparer.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Collections;

namespace BootFX.Common.Data.Schema
{
	/// <summary>
	/// Defines a class that sorts lists of <see cref="SqlMember"></see> instances by their native name.
	/// </summary>
	internal class SqlMemberNativeNameComparer : IComparer
	{
		bool _sortByOrdinal = false;

		internal SqlMemberNativeNameComparer() : this(false)
		{
		}

		internal SqlMemberNativeNameComparer(bool sortByOrdinal)
		{
			_sortByOrdinal = sortByOrdinal;
		}


		public int Compare(object x, object y)
		{
			SqlMember a = (SqlMember)x;
			SqlMember b = (SqlMember)y;
	
			// compare...
			if(!_sortByOrdinal)
				return string.Compare(a.NativeName, b.NativeName, true, System.Globalization.CultureInfo.InvariantCulture);
				
			return a.Ordinal.CompareTo(b.Ordinal);
		}
	}
}
