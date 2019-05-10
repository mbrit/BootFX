// BootFX - Application framework for .NET applications
// 
// File: ProjectStoreListItem.cs
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

namespace BootFX.Common.DbUtil
{
	/// <summary>
	/// Defines an instance of <c>ProjectStoreListItem</c>.
	/// </summary>
	internal class ProjectStoreListItem
	{
		/// <summary>
		/// Private field to support <see cref="Store"/> property.
		/// </summary>
		private ProjectStore _store;
		
		internal ProjectStoreListItem(ProjectStore store)
		{
			if(store == null)
				throw new ArgumentNullException("store");
			_store = store;
		}

		/// <summary>
		/// Gets the store.
		/// </summary>
		internal ProjectStore Store
		{
			get
			{
				return _store;
			}
		}

		public override string ToString()
		{
            return "File";
		}
	}
}
