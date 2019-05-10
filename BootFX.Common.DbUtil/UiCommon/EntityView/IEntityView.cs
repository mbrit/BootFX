// BootFX - Application framework for .NET applications
// 
// File: IEntityView.cs
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

namespace BootFX.Common.UI
{
	/// <summary>
	/// Defines an instance of <c>IEntityView</c>.
	/// </summary>
	public interface IEntityView
	{
		/// <summary>
		/// Gets the focused entity.
		/// </summary>
		object FocusedEntity
		{		
			get;
		}

		/// <summary>
		/// Gets the selected entities.
		/// </summary>
		IList SelectedEntities
		{
			get;	
		}

		/// <summary>
		/// Gets the number of selected items.
		/// </summary>
		int SelectedEntitiesCount
		{
			get;
		}
	}
}
