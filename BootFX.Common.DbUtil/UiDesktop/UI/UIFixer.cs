// BootFX - Application framework for .NET applications
// 
// File: UIFixer.cs
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
	/// Control that performs UI fixup tasks.
	/// </summary>
	public class UIFixer
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		public UIFixer()
		{
		}

		/// <summary>
		/// Fixes up the given control.
		/// </summary>
		/// <param name="control"></param>
		public void Fixup(Control control)
		{
			if(control == null)
				throw new ArgumentNullException("control");
			
			// switch...
			if(control is ButtonBase)
				Fixup((ButtonBase)control);
			else if(control is GroupBox)
				Fixup((GroupBox)control);
			else if(control is ProgressBar)
				Fixup((ProgressBar)control);

			// walk...
			foreach(Control child in control.Controls)
				this.Fixup(child);
		}

		/// <summary>
		/// Fixes the button.
		/// </summary>
		/// <param name="button"></param>
		public void Fixup(ProgressBar progress)
		{
			if(progress == null)
				throw new ArgumentNullException("progress");
		}

		/// <summary>
		/// Fixes the button.
		/// </summary>
		/// <param name="button"></param>
		public void Fixup(ButtonBase button)
		{
			if(button == null)
				throw new ArgumentNullException("button");
			
			// set...
			button.FlatStyle = FlatStyle.System;
		}

		/// <summary>
		/// Fixes the button.
		/// </summary>
		/// <param name="button"></param>
		public void Fixup(GroupBox groupBox)
		{
			if(groupBox == null)
				throw new ArgumentNullException("groupBox");
			
			// set...
			groupBox.FlatStyle = FlatStyle.System;
		}
	}
}
