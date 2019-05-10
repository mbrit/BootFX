// BootFX - Application framework for .NET applications
// 
// File: LineEventArgs.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;

namespace BootFX.Common.UI.Desktop
{
	/// <summary>
	/// Delegate for <see cref="LineEventArgs"></see>.
	/// </summary>
	internal delegate void LineEventHandler(object sender, LineEventArgs e);

	/// <summary>
	/// Event argument class extension containing a line of text held in a string.
	/// </summary>
	internal class LineEventArgs : EventArgs
	{
		public LineEventArgs(string line)
		{
			_line = line;
		}

		/// <summary>
		/// Private field to support <c>Line</c> property.
		/// </summary>
		private string _line;
		
		/// <summary>
		/// Gets the line.
		/// </summary>
		public string Line
		{
			get
			{
				return _line;
			}
		}
	}
}
