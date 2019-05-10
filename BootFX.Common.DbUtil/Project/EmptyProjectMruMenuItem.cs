// BootFX - Application framework for .NET applications
// 
// File: EmptyProjectMruMenuItem.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Windows.Forms;

namespace BootFX.Common.DbUtil
{
	/// <summary>
	/// Summary description for EmptyProjectMruMenuItem.
	/// </summary>
	internal class EmptyProjectMruMenuItem : MenuItem
	{
		internal EmptyProjectMruMenuItem()
		{
			this.Text = "(None)";
			this.Enabled = false;
		}
	}
}
