// BootFX - Application framework for .NET applications
// 
// File: DesktopCommandClickContext.cs
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
	/// Defines an instance of <c>DesktopCommandEventArgs</c>.
	/// </summary>
	public class DesktopCommandClickContext : CommandClickContextBase
	{
		/// <summary>
		/// Private field to support <see cref="Owner"/> property.
		/// </summary>
		private Control _owner;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="owner"></param>
		public DesktopCommandClickContext(Control owner)
		{
			if(owner == null)
				throw new ArgumentNullException("owner");
			_owner = owner;
		}

		/// <summary>
		/// Gets the owner.
		/// </summary>
		public Control Owner
		{
			get
			{
				return _owner;
			}
		}

		internal IEntityView OwnerAsEntityView
		{
			get
			{
				return this.Owner as IEntityView;
			}
		}

		/// <summary>
		/// Gets the owner form.
		/// </summary>
		public Form OwnerForm
		{
			get
			{
				if(Owner == null)
					throw new InvalidOperationException("Owner is null.");
				return DesktopRuntime.Current.GetOwnerForm(this.Owner, false);
			}
		}

		protected override IEntityView GetAssociatedView()
		{
			return this.Owner as IEntityView;
		}
	}
}
