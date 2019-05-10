// BootFX - Application framework for .NET applications
// 
// File: CommandUIObjectBuilder.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Drawing;
using System.Windows.Forms;

namespace BootFX.Common.UI.Desktop
{
	/// <summary>
	/// Summary description for ContextMenuBuilder.
	/// </summary>
	public class CommandUIObjectBuilder
	{
		public CommandUIObjectBuilder()
		{
		}

		/// <summary>
		/// Creates a context menu from a set of commands.
		/// </summary>
		/// <param name="commands"></param>
		/// <returns></returns>
		public ContextMenu CreateContextMenu(DesktopCommandClickContext context, CommandCollection commands)
		{
			if(context == null)
				throw new ArgumentNullException("context");
			if(commands == null)
				throw new ArgumentNullException("commands");
			
			// walk...
			ContextMenu menu = new ContextMenu();
			this.PopulateMenuItems(context, menu.MenuItems, commands);

			// return...
			return menu;
		}

		/// <summary>
		/// Populates menu items from the given command collection.
		/// </summary>
		/// <param name="items"></param>
		/// <param name="commands"></param>
		private void PopulateMenuItems(DesktopCommandClickContext context, Menu.MenuItemCollection items, CommandCollection commands)
		{
			if(context == null)
				throw new ArgumentNullException("context");
			if(items == null)
				throw new ArgumentNullException("items");
			if(commands == null)
				throw new ArgumentNullException("commands");
			
			// how many are selected?
			int numSelected = 1;
			if(context.OwnerAsEntityView != null)
				numSelected = context.OwnerAsEntityView.SelectedEntitiesCount;

			// walk...
			foreach(Command command in commands)
			{
				// ok?
				bool ok = true;
				if(numSelected > 1 && command.SingleOnly)
					ok = false;

				// ok?
				if(ok)
				{
					// sep?
					if(command.HasSeparator)
					{
						// add a separator...
						bool sepOk = true;
						if(items.Count > 0)
						{
							string above = items[items.Count - 1].Text;
							if(above != null && above == "-")
								sepOk = false;
						}
						else
							sepOk = false;

						// add...
						if(sepOk)
							items.Add("-");
					}

					// add...
					items.Add(new CommandMenuItem(context, command, CommandViewContext.ContextMenu));
				}
			}
		}

		/// <summary>
		/// Creates and tracks a context menu.
		/// </summary>
		public void CreateAndShowContextMenu(Control owner, Point point, object provider)
		{
			if(owner == null)
				throw new ArgumentNullException("owner");
			if(provider == null)
				throw new ArgumentNullException("provider");
			
			// get...
			CommandCollection commands = Command.GetCommands(provider);
			if(commands == null || commands.Count == 0)
				return;

			// context...
			DesktopCommandClickContext context = new DesktopCommandClickContext(owner);

			// create...
			ContextMenu menu = this.CreateContextMenu(context, commands);
			if(menu == null)
				throw new InvalidOperationException("menu is null.");

            // mbr - 2009-07-09 - added dispose...
		    menu.Show(owner, point);
		}
	}
}
