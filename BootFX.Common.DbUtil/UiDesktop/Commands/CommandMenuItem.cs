// BootFX - Application framework for .NET applications
// 
// File: CommandMenuItem.cs
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
using System.Drawing;
using System.Drawing.Text;
using System.Collections;
using System.Windows.Forms;
using BootFX.Common.Management;

namespace BootFX.Common.UI.Desktop
{
	/// <summary>
	/// Defines an instance of <c>CommandMenuItem</c>.
	/// </summary>
	internal class CommandMenuItem : ImageMenuItem
	{
		/// <summary>
		/// Private field to support <c>Context</c> property.
		/// </summary>
		private DesktopCommandClickContext _context;
		
		/// <summary>
		/// Private field to support <c>Command</c> property.
		/// </summary>
		private Command _command;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="command"></param>
		internal CommandMenuItem(DesktopCommandClickContext ClickContext, Command command, CommandViewContext viewContext) 
            : base(command.GetText(viewContext))
		{
			if(ClickContext == null)
				throw new ArgumentNullException("ClickContext");
			if(command == null)
				throw new ArgumentNullException("command");
			
			// set...
			_context = ClickContext;
			_command = command;

            // mbr - 2009-02-01 - added image support...
            if (command is DesktopCommand)
            {
                this.ImageResourceAssembly = ((DesktopCommand)command).ImageResourceAssembly;
                this.ImageResourceName = ((DesktopCommand)command).ImageResourceName;
            }
        }

		/// <summary>
		/// Gets the context.
		/// </summary>
		private DesktopCommandClickContext Context
		{
			get
			{
				// returns the value...
				return _context;
			}
		}

		/// <summary>
		/// Gets the command.
		/// </summary>
		private Command Command
		{
			get
			{
				// returns the value...
				return _command;
			}
		}

		protected override void OnClick(EventArgs e)
		{
			base.OnClick (e);

			// send...
			if(Command == null)
				throw new InvalidOperationException("Command is null.");
			this.Command.RaiseClick(this.Context);
		}
		
		private DesktopCommand DesktopCommand
		{
			get
			{
				return this.Command as DesktopCommand;
			}
		}

		public override Image Image
		{
			get
			{
				if(this.DesktopCommand != null && this.DesktopCommand.HasImage)
					return this.DesktopCommand.Image;
				else
					return base.Image;
			}
		}
	}
}
