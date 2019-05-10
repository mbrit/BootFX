// BootFX - Application framework for .NET applications
// 
// File: OptionDialogButton.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Windows.Forms;

namespace BootFX.Common.UI.Desktop
{
	/// <summary>
	/// Defines a button for use on the option dialog.
	/// </summary>
	internal class OptionDialogButton : Button
	{
		internal OptionDialogButton(string text, DialogResult result)
		{
			this.Text = text;
			this.DialogResult = result;
			this.FlatStyle = FlatStyle.System;
		}
	}
}
