// BootFX - Application framework for .NET applications
// 
// File: ListViewEx.cs
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
using System.Windows.Forms;

namespace BootFX.Common.UI.Desktop
{
	/// <summary>
	/// Defines an instance of <c>ListViewEx</c>.
	/// </summary>
	public class ListViewEx : ListView
	{
		public ListViewEx()
		{
		}

		public void AutoSizeColumns()
		{
			ListViewHelper.AutoSizeColumns(this);
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
			base.OnKeyDown (e);
			ListViewHelper.HandleKeyDown(this, e);
		}

		public void SelectAll()
		{
			ListViewHelper.SelectAll(this, true);
		}

		public void SelectNone()
		{
			ListViewHelper.SelectAll(this, false);
		}

		/// <summary>
		/// Sets all items in the list to be checked.
		/// </summary>
		public void CheckAll()
		{
			ListViewHelper.CheckAll(this, true);
		}

		/// <summary>
		/// Sets all items in the list to be unchecked.
		/// </summary>
		public void CheckNone()
		{
			ListViewHelper.CheckAll(this, false);
		}
	}
}
