// BootFX - Application framework for .NET applications
// 
// File: PropertyTabPage.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Windows.Forms;
using BootFX.Common.UI;

namespace BootFX.Common.UI.Desktop
{
	/// <summary>
	/// Summary description for PropertyPage.
	/// </summary>
	internal class PropertyTabPage : TabPage
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		public PropertyTabPage(Control control)
		{
			if(control == null)
				throw new ArgumentNullException("control");
			
			// set...
			control.Dock = DockStyle.Fill;
			this.Controls.Add(control);
			this.RefreshView();
		}

		/// <summary>
		/// Gets the name for a property page.
		/// </summary>
		/// <param name="page"></param>
		/// <returns></returns>
		internal static string GetPropertyPageName(Control page)
		{
			// check...
			if(page == null)
				return string.Empty;
			if(page is IPropertyPage)
				return ((IPropertyPage)page).PageName;
			else
				return page.ToString();
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		private void RefreshView()
		{
			this.Text = GetPropertyPageName(this.PropertyPage);
		}

		/// <summary>
		/// Gets the property page within the tab page.
		/// </summary>
		private Control PropertyPage
		{
			get
			{
				if(this.Controls.Count > 0)
					return this.Controls[0];
				else
					return null;
			}
		}

		/// <summary>
		/// Gets the property page as an <see cref="IPropertyPage"></see> instance.
		/// </summary>
		internal IPropertyPage PropertyPageAsIPropertyPage
		{
			get
			{
				return this.PropertyPage as IPropertyPage;
			}
		}
	}
}
