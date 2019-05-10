// BootFX - Application framework for .NET applications
// 
// File: TypeAheadComboBox.cs
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
	/// Defines an instance of <c>TypeAheadComboBox</c>.
	/// </summary>
	public class TypeAheadComboBox : ComboBox
	{
		public TypeAheadComboBox()
		{
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
			if(e.KeyCode == Keys.Return)
			{
				// find...
				this.MatchText(false);

				// return...
				e.Handled = true;
			}
			else
				base.OnKeyDown (e);
		}

		private void MatchText(bool quiet)
		{
			string text = this.Text.Trim();
			if(text.Length > 0)
			{
				// walk and compare...
				int index = this.FindString(text);
				if(index != -1)
				{
					this.SelectedIndex = index;

					// select...
					this.SelectAll();
					this.Focus();
				}
				else
				{
					if(!(quiet))
						Alert.ShowInformation(this, "Not found.");
				}
			}
			else
			{
				// select nothing...
				this.SelectedIndex = -1;
			}
		}
	}
}
