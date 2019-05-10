// BootFX - Application framework for .NET applications
// 
// File: PropertyPage.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using BootFX.Common.UI.Common;
using BootFX.Common.UI.DataBinding;

namespace BootFX.Common.UI.Desktop
{
	/// <summary>
	/// Defines a property page.
	/// </summary>
	public class PropertyPage : DataPanel, IPropertyPage
	{
		/// <summary>
		/// Private field to support <c>Name</c> property.
		/// </summary>
		private string _pageName;
		
		/// <summary>
		/// Constructor.
		/// </summary>
		public PropertyPage()
		{
			this.InitializeComponent();
		}

		/// <summary>
		/// Initializes the view.
		/// </summary>
		private void InitializeComponent()
		{
		}

		/// <summary>
		/// Gets or sets the name
		/// </summary>
		public string PageName
		{
			get
			{
				if(_pageName == null || _pageName.Length == 0)
					return this.ToString();
				else
					return _pageName;
			}
			set
			{
				// check to see if the value has changed...
				if(value != _pageName)
				{
					// set the value...
					_pageName = value;
				}
			}
		}

		/// <summary>
		/// Applies changes.
		/// </summary>
		/// <param name="reason"></param>
		/// <returns></returns>
		bool IPropertyPage.Apply(ApplyReason reason)
		{
			return this.Apply(reason);
		}

		/// <summary>
		/// Applies changes to the page.
		/// </summary>
		/// <param name="reason"></param>
		/// <returns></returns>
		internal bool Apply(ApplyReason reason)
		{
			return this.DoApply(reason);
		}

		/// <summary>
		/// Called before changes to the page are applied.
		/// </summary>
		/// <param name="reason"></param>
		/// <returns></returns>
		protected virtual bool DoApply(ApplyReason reason)
		{
			return true;
		}
	}
}
