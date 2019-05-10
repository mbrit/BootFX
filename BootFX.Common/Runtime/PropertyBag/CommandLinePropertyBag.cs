// BootFX - Application framework for .NET applications
// 
// File: CommandLinePropertyBag.cs
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
	/// <summary>
	/// Summary description for CommandLinePropertyBag.
	/// </summary>
	public class CommandLinePropertyBag : PropertyBag
	{
		/// <summary>
		/// Private field to hold the singleton instance.
		/// </summary>
		private static CommandLinePropertyBag _current = null;
		
		/// <summary>
		/// Private constructor.
		/// </summary>
		private CommandLinePropertyBag()
		{
		}
		
		/// <summary>
		/// Static constructor.
		/// </summary>
		static CommandLinePropertyBag()
		{
			_current = new CommandLinePropertyBag();

			// regex...
			string[] parts = ParseCommandLine(Environment.CommandLine);
			if(parts == null)
				throw new InvalidOperationException("parts is null.");

			// walk...
			// \s*-(?<name>[^:]*)\s*:?\s*(?<value>.*)
			Regex regex = new Regex(@"\s*-(?<name>[^:]*)\s*:?\s*(?<value>.*)", RegexOptions.Singleline | RegexOptions.IgnoreCase);
			for(int index = 1; index < parts.Length; index++)
			{
				foreach(Match match in regex.Matches(parts[index]))
				{
					// set...
					string name = match.Groups["name"].Value;
					string value = match.Groups["value"].Value;
					_current[name] = value;
				}
			}
		}

		/// <summary>
		/// Parses the command line.
		/// </summary>
		/// <param name="commandLine"></param>
		/// <returns></returns>
		private static string[] ParseCommandLine(string commandLine)
		{
			if(commandLine == null)
				return new string[] {};
			
			ArrayList parts = new ArrayList();
			StringBuilder builder = null;		
			bool inQuote = false;
			foreach(char c in commandLine)
			{
				if(c == '\"')
				{
					if(inQuote)
					{
						builder = null;
						inQuote = false;
					}
					else
						inQuote = true;
				}
				else
				{
					// split?
					if(inQuote == false && char.IsWhiteSpace(c))
						builder = null;

					// add...
					if(builder == null)
					{
						builder = new StringBuilder();
						parts.Add(builder);
					}
					builder.Append(c);
				}
			}

			// break up...
			string[] results = new string[parts.Count];
			for(int index = 0; index < parts.Count; index++)
				results[index] = ((StringBuilder)parts[index]).ToString();

			// return...
			return results;
		}
		
		/// <summary>
		/// Gets the singleton instance.
		/// </summary>
		public static CommandLinePropertyBag Current
		{
			get
			{
				return _current;
			}
		}

		public override bool IsReadOnly
		{
			get
			{
				return true;
			}
		}

		/// <summary>
		/// Loads a value from the property bag.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="found"></param>
		/// <returns></returns>
		protected override object LoadValue(string name, ref bool found)
		{
			// if it's not in here, it wasn't on the command line...
			found = false;
			return null;
		}
	}
}
