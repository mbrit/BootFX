// BootFX - Application framework for .NET applications
// 
// File: CommonCommandEventArgs.cs
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

namespace BootFX.Common.UI
{
	public delegate void CommonCommandEventHandler(object sender, CommonCommandEventArgs e);

	/// <summary>
	/// Defines an instance of <c>CommonCommandEventArgs</c>.
	/// </summary>
	public class CommonCommandEventArgs : EventArgs
	{
		/// <summary>
		/// Private field to support <see cref="Command"/> property.
		/// </summary>
		private CommonCommand _command;
		
		internal CommonCommandEventArgs(CommonCommand command)
		{
			_command = command;
		}

		/// <summary>
		/// Gets the command.
		/// </summary>
		internal CommonCommand Command
		{
			get
			{
				return _command;
			}
		}
	}
}
