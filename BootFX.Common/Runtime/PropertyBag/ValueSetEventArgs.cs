// BootFX - Application framework for .NET applications
// 
// File: ValueSetEventArgs.cs
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

namespace BootFX.Common
{
	public delegate void ValueSetEventHandler(object sender, ValueSetEventArgs e);

	/// <summary>
	/// Defines an instance of <c>ValueSetEventArgs</c>.
	/// </summary>
	public class ValueSetEventArgs : EventArgs
	{
		/// <summary>
		/// Private field to support <see cref="Name"/> property.
		/// </summary>
		private string _name;
		
		internal ValueSetEventArgs(string name)
		{
			_name = name;
		}

		/// <summary>
		/// Gets the name.
		/// </summary>
		public string Name
		{
			get
			{
				return _name;
			}
		}
	}
}
