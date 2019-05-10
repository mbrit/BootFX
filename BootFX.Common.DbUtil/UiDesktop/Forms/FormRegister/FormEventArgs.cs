// BootFX - Application framework for .NET applications
// 
// File: FormEventArgs.cs
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
	/// Delegate for use with <see cref="FormEventArgs"></see>.
	/// </summary>
	public delegate void FormEventHandler(object sender, FormEventArgs e);

	/// <summary>
	/// Summary description for FormEventArgs.
	/// </summary>
	public class FormEventArgs : EventArgs
	{
		/// <summary>
		/// Private field to support <see cref="Form"/> property.
		/// </summary>
		private Form _form;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="form"></param>
		public FormEventArgs(Form form)
		{
			if(form == null)
				throw new ArgumentNullException("form");
			
			_form = form;
		}

		/// <summary>
		/// Gets the form.
		/// </summary>
		public Form Form
		{
			get
			{
				return _form;
			}
		}
	}
}
