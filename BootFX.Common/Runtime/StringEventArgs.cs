// BootFX - Application framework for .NET applications
// 
// File: StringEventArgs.cs
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
	public delegate void StringEventHandler(object sender, StringEventArgs e);

	/// <summary>
	/// Defines an instance of <c>StringEventArgs</c>.
	/// </summary>
	public class StringEventArgs : EventArgs
	{
		/// <summary>
		/// Private field to support <see cref="Value"/> property.
		/// </summary>
		private string _value;
		
		public StringEventArgs(string value)
		{
			_value = value;
		}

		/// <summary>
		/// Gets the value.
		/// </summary>
		public string Value
		{
			get
			{
				return _value;
			}
		}
	}
}
