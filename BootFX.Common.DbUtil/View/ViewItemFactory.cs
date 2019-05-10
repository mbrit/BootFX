// BootFX - Application framework for .NET applications
// 
// File: ViewItemFactory.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using BootFX.Common.Data.Schema;

namespace BootFX.Common.DbUtil
{
	/// <summary>
	/// Summary description for SqlViewFactory.
	/// </summary>
	internal sealed class ViewItemFactory
	{
		internal ViewItemFactory()
		{
		}

		/// <summary>
		/// Creates a view item.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		internal static object CreateViewItem(object obj)
		{
			if(obj == null)
				throw new ArgumentNullException("obj");			

			// get...
			if(obj is SqlTable)
				return new SqlTableView((SqlTable)obj);
			if(obj is SqlColumn)
				return new SqlColumnView((SqlColumn)obj);
			if(obj is SqlChildToParentLink)
				return new SqlChildToParentLinkView((SqlChildToParentLink)obj);
			if(obj is SqlMember)
				return new SqlMemberView((SqlMember)obj);
			else
				return null;
		}

		/// <summary>
		/// Unwraps the object.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		internal static object Unwrap(object obj)
		{
			// if...
			if(obj is SqlMemberView)
				return ((SqlMemberView)obj).Member;
			else
				return obj;
		}
	}
}
