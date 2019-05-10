// BootFX - Application framework for .NET applications
// 
// File: CommandBundle.cs
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
	/// <summary>
	/// Defines an instance of <c>CommandBundle</c>.
	/// </summary>
	public abstract class CommandBundle : ICommandProvider
	{
		/// <summary>
		/// Private field to support <c>CommonCommands</c> property.
		/// </summary>
		private CommonCommandHandler _commonCommands = new CommonCommandHandler();
		
		/// <summary>
		/// Private field to support <c>Commands</c> property.
		/// </summary>
		private CommandCollection _commands = new CommandCollection();

		/// <summary>
		/// Consturctor.
		/// </summary>
		protected CommandBundle()
		{
		}

		/// <summary>
		/// Gets a collection of Command objects.
		/// </summary>
		public CommandCollection Commands
		{
			get
			{
				return _commands;
			}
		}

		CommandCollection ICommandProvider.GetCommands()
		{
			return this.GetAllCommands();
		}

		/// <summary>
		/// Gets the commands for this bundle.
		/// </summary>
		/// <returns></returns>
		public virtual CommandCollection GetAllCommands()
		{
			// get the common commands.
			CommandCollection commands = this.CommonCommands.GetCommands();
			commands.AddRange(this.Commands);
			return commands;
		}

		/// <summary>
		/// Gets the commoncommands.
		/// </summary>
		public CommonCommandHandler CommonCommands
		{
			get
			{
				// returns the value...
				return _commonCommands;
			}
		}
	}
}
