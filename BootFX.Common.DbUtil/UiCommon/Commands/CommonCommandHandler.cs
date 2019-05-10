// BootFX - Application framework for .NET applications
// 
// File: CommonCommandHandler.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Collections.Specialized;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;

namespace BootFX.Common.UI
{
	/// <summary>
	/// Defines an instance of <c>CommonCommandHandler</c>.
	/// </summary>
	public class CommonCommandHandler : ICommandProvider
	{
		/// <summary>
		/// Private field to support <c>Handlers</c> property.
		/// </summary>
		private IDictionary _handlers = new HybridDictionary();
		
		/// <summary>
		/// Raised when the delete command is selected.
		/// </summary>
		public event CommandEventHandler DeleteCommand
		{
			add
			{
				this.AddHandler(CommonCommand.Delete, value);
			}
			remove
			{
				this.RemoveHandler(CommonCommand.Delete, value);
			}
		}
		
		/// <summary>
		/// Gets the handlers.
		/// </summary>
		private IDictionary Handlers
		{
			get
			{
				// returns the value...
				return _handlers;
			}
		}

		private void AddHandler(CommonCommand command, CommandEventHandler handler)
		{
			CommandEventHandler existing = (CommandEventHandler)this.Handlers[command];
			this.Handlers[command] = (CommandEventHandler)Delegate.Combine(existing, handler); 
		}

		private void RemoveHandler(CommonCommand command, CommandEventHandler handler)
		{
			CommandEventHandler existing = (CommandEventHandler)this.Handlers[command];
			this.Handlers[command] = (CommandEventHandler)Delegate.Remove(existing, handler); 
		}

		public CommonCommandHandler()
		{
		}

		public bool HasProperties
		{
			get
			{
				return this.HasCommonCommand(CommonCommand.Properties);
			}
		}

		public bool HasOpen
		{
			get
			{
				return this.HasCommonCommand(CommonCommand.Open);
			}
		}

		public bool HasDelete
		{
			get
			{
				return this.HasCommonCommand(CommonCommand.Delete);
			}
		}

		private bool HasCommonCommand(CommonCommand command)
		{
			return this.Handlers.Contains(command);
		}

		/// <summary>
		/// Gets commands for the node.
		/// </summary>
		/// <returns></returns>
		public CommandCollection GetCommands()
		{
			CommandCollection commands = new CommandCollection();

			// add...
			this.AddCommandIfNeeded(commands, CommonCommand.Open);
			this.AddCommandIfNeeded(commands, CommonCommand.Delete);
			this.AddCommandIfNeeded(commands, CommonCommand.Properties);

			// return...
			return commands;
		}

		private void AddCommandIfNeeded(CommandCollection commands, CommonCommand commonCommand)
		{
			if(this.HasCommonCommand(commonCommand))
				this.AddCommand(commands, commonCommand);
		}

		private void AddCommand(CommandCollection commands, CommonCommand commonCommand)
		{
			if(commands	 == null)
				throw new ArgumentNullException("commands	");
			
			// create...
			Command command = new Command(commonCommand);
			command.Click += new CommandEventHandler(HandleCommandClicked);
			commands.Add(command);
		}

		private void HandleCommandClicked(object sender, CommandEventArgs e)
		{
			// what...
			CommonCommand command = ((Command)sender).CommonCommand;

			// get...
			CommandEventHandler handler = (CommandEventHandler)this.Handlers[command];
			if(handler != null)
				handler(this, e);
		}
	}
}
