// BootFX - Application framework for .NET applications
// 
// File: IDesktopBindableControl.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Windows.Forms;
using BootFX.Common.UI.DataBinding;

namespace BootFX.Common.UI.Desktop.DataBinding
{
	/// <summary>
	/// Summary description for IDesktopBindableControl.
	/// </summary>
	public interface IDesktopBindableControl : IBindableControl
	{
		/// <summary>
		/// Gets or sets the binding.
		/// </summary>
		Binding Binding
		{
			get;
			set;
		}
	}
}
