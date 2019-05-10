// BootFX - Application framework for .NET applications
// 
// File: CommandEventArgs.cs
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
using BootFX.Common.Data;

namespace BootFX.Common.UI
{
	public delegate void CommandEventHandler(object sender, CommandEventArgs e);

	/// <summary>
	/// Defines an instance of <c>CommandEventArgs</c>.
	/// </summary>
	public sealed class CommandEventArgs : EventArgs
	{
		/// <summary>
		/// Private field to support <see cref="Command"/> property.
		/// </summary>
		private Command _command;

		/// <summary>
		/// Private field to support <see cref="Context"/> property.
		/// </summary>
		private ICommandClickContext _context;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="context"></param>
        internal CommandEventArgs(Command command, ICommandClickContext context)
		{
			_command = command;
			_context = context;
		}

		/// <summary>
		/// Gets the context.
		/// </summary>
		public ICommandClickContext Context
		{
			get
			{
				return _context;
			}
		}
		
		/// <summary>
		/// Gets the command.
		/// </summary>
		public Command Command
		{
			get
			{
				return _command;
			}
		}
	}
}
