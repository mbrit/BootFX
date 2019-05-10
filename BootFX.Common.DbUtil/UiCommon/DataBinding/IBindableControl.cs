// BootFX - Application framework for .NET applications
// 
// File: IBindableControl.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using BootFX.Common.Entities;

namespace BootFX.Common.UI.DataBinding
{
	/// <summary>
	/// Describes a control that supports data binding.
	/// </summary>
	public interface IBindableControl
	{
		/// <summary>
		/// Gets the property to bind.
		/// </summary>
		/// <returns></returns>
		string GetDefaultBindProperty();
	}
}
