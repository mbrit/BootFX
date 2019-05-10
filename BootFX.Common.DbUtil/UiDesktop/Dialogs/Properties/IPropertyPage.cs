// BootFX - Application framework for .NET applications
// 
// File: IPropertyPage.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using BootFX.Common.UI.Common;
using BootFX.Common.UI.Desktop.DataBinding;

namespace BootFX.Common.UI
{
	/// <summary>
	/// Defines a property page.
	/// </summary>
	public interface IPropertyPage : IEntityDataControl
	{
		/// <summary>
		/// Gets the page name.
		/// </summary>
		string PageName
		{
			get;
		}

		/// <summary>
		/// Applies changes.
		/// </summary>
		/// <returns></returns>
		bool Apply(ApplyReason reason);
	}
}
