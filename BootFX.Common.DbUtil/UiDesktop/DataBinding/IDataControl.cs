// BootFX - Application framework for .NET applications
// 
// File: IDataControl.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Windows.Forms;
using System.Collections;
using System.ComponentModel;
using BootFX.Common.UI.DataBinding;

namespace BootFX.Common.UI.Desktop.DataBinding
{
	/// <summary>
	/// Represents a control that holds data.
	/// </summary>
	public interface IDataControl : IDataControlBase
	{
		/// <summary>
		/// Gets the binding manager.
		/// </summary>
		BindingManagerBase BindingManager
		{
			get;
		}

		/// <summary>
		/// Gets the currency binding manager.
		/// </summary>
		CurrencyManager CurrencyBindingManager
		{
			get;
		}

		/// <summary>
		/// Gets the property binding manager.
		/// </summary>
		PropertyManager PropertyBindingManager
		{
			get;
		}
	}
}
