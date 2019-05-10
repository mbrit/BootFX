// BootFX - Application framework for .NET applications
// 
// File: Command.cs
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
using System.Reflection;
using System.Collections;
using BootFX.Common.Entities;

namespace BootFX.Common.UI
{
	/// <summary>
	/// Defines an instance of <c>Command</c>.
	/// </summary>
	public class Command
	{
        /// <summary>
		/// Private field to support <c>State</c> property.
		/// </summary>
		private object _state;
		
		/// <summary>
		/// Private field to support <see cref="Flags"/> property.
		/// </summary>
		private CommandFlags _flags;
		
		/// <summary>
		/// Private field to support <see cref="FullName"/> property.
		/// </summary>
		private string _fullName;
		
		/// <summary>
		/// Raised when the command is clicked.
		/// </summary>
		public event CommandEventHandler Click;

		/// <summary>
		/// Private field to support <c>CommonCommand</c> property.
		/// </summary>
		private CommonCommand _commonCommand = CommonCommand.None;

		/// <summary>
		/// Constructor.
		/// </summary>
		public Command(string fullName) : this(fullName, CommandFlags.Normal)
		{			
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		public Command(string fullName, CommandFlags flags)
		{
			if(fullName == null)
				throw new ArgumentNullException("fullName");
			if(fullName.Length == 0)
				throw new ArgumentOutOfRangeException("'fullName' is zero-length.");

			// set...
			_fullName = fullName;
			_flags = flags;
		}

		/// <summary>
		/// Gets the allowmultiple.
		/// </summary>
		public bool SingleOnly
		{
			get
			{
				return this.IsFlagSet(CommandFlags.SingleOnly);
			}
		}
		
		/// <summary>
		/// Gets the hasseparator.
		/// </summary>
		public bool HasSeparator
		{
			get
			{
				return this.IsFlagSet(CommandFlags.HasSeparator);
			}
		}

		private bool IsFlagSet(CommandFlags flags)
		{
			if((this.Flags & flags) != 0)
				return true;
			else
				return false;
		}

		/// <summary>
		/// Gets the flags.
		/// </summary>
		public CommandFlags Flags
		{
			get
			{
				return _flags;
			}
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		internal Command(CommonCommand commonCommand)
		{
			_commonCommand = commonCommand;
		}

		/// <summary>
		/// Gets the commoncommand.
		/// </summary>
		internal CommonCommand CommonCommand
		{
			get
			{
				// returns the value...
				return _commonCommand;
			}
		}

		/// <summary>
		/// Gets the fullname.
		/// </summary>
		public string FullName
		{
			get
			{
				return _fullName;
			}
		}

		public string GetText(CommandViewContext context)
		{
			if(this.CommonCommand == CommonCommand.None)
			{
				switch(context)
				{
					case CommandViewContext.ContextMenu:
					case CommandViewContext.Toolbar:
						string[] parts = this.GetNameParts();
						if(parts == null)
							throw new InvalidOperationException("'parts' is null.");
						if(parts.Length == 0)
							throw new InvalidOperationException("'parts' is zero-length.");
						return parts[0];

					case CommandViewContext.PopupMenu:
						return this.FullName;

					default:
						throw new NotSupportedException(string.Format("Cannot handle '{0}' ({1}).", context, context.GetType()));
				}
			}
			else
			{
				bool shortName = true;
				if(context == CommandViewContext.PopupMenu)
					shortName = false;

				// defer...
				return this.GetCommonCommandName(shortName);
			}
		}

		private string GetCommonCommandName(bool shortName)
		{
			switch(CommonCommand)
			{
				case CommonCommand.Delete:
					if(shortName)
						return "Delete";
					else
						return @"Edit\Delete";

				case CommonCommand.Open:
					if(shortName)
						return "Open";
					else
						return @"File\Open";

				case CommonCommand.Properties:
					if(shortName)
						return "Properties";
					else
						return @"View\Properties";

				default:
					throw new NotSupportedException(string.Format("Cannot handle '{0}' ({1}).", CommonCommand, CommonCommand.GetType()));
			}			
		}

		/// <summary>
		/// Raises the <c>Click</c> event.
		/// </summary>
		protected virtual void OnClick(CommandEventArgs e)
		{
			// raise...
			if(Click != null)
				Click(this, e);
		}

		public void RaiseClick(ICommandClickContext context)
		{
			if(context == null)
				throw new ArgumentNullException("context");
			
			// defer...
			this.OnClick(new CommandEventArgs(this, context));
		}

		public override string ToString()
		{
			return this.FullName;
		}

		/// <summary>
		/// Gets the command's name parts.
		/// </summary>
		/// <returns></returns>
		public string[] GetNameParts()
		{
			if(FullName == null)
				throw new InvalidOperationException("'FullName' is null.");
			if(FullName.Length == 0)
				throw new InvalidOperationException("'FullName' is zero-length.");
			return this.FullName.Split('\\');
		}

		/// <summary>
		/// Gets commands for the given object.
		/// </summary>
		/// <param name="provider"></param>
		/// <returns></returns>
		public static CommandCollection GetCommands(object provider)
		{
			if(provider == null)
				throw new ArgumentNullException("provider");
			
			// native provider?
			if(provider is ICommandProvider)
				return ((ICommandProvider)provider).GetCommands();
			else if(EntityType.IsEntity(provider))
			{
				// get it...
				EntityType et = EntityType.GetEntityType(provider, OnNotFound.ThrowException);
				if(et == null)
					throw new InvalidOperationException("et is null.");

				// get...
				return GetCommandsFromCommandBundles(et);
			}
			else
				throw new NotSupportedException(string.Format("There is no method for discovering commands on '{0}'.", provider));
		}

		/// <summary>
		/// Gets commands from the discovered command bundles.
		/// </summary>
		/// <param name="et"></param>
		/// <returns></returns>
		public static CommandCollection GetCommandsFromCommandBundles(EntityType et)
		{
			if(et == null)
				throw new ArgumentNullException("et");
			
			// finder...
			TypeFinder finder = new TypeFinder(typeof(CommandBundle));
			finder.AddAttributeSpecification(typeof(CommandBundleAttribute), false, "Type", et.Type);

			// create...
			CommandBundle[] bundles = (CommandBundle[])finder.CreateInstances();
			if(bundles == null)
				throw new InvalidOperationException("bundles is null.");

			// walk...
			CommandCollection all = new CommandCollection();
			foreach(CommandBundle bundle in bundles)
				all.AddRange(bundle.GetAllCommands());

			// return...
			return all;
		}

		/// <summary>
		/// Gets or sets the state
		/// </summary>
		public object State
		{
			get
			{
				return _state;
			}
			set
			{
				// check to see if the value has changed...
				if(value != _state)
				{
					// set the value...
					_state = value;
				}
			}
		}
	}
}
