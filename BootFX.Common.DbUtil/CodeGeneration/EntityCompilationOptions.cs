// BootFX - Application framework for .NET applications
// 
// File: EntityCompilationOptions.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using BootFX.Common.CodeGeneration;

namespace BootFX.Common.CodeGeneration
{
	/// <summary>
	/// Options for entity compliation.
	/// </summary>
	public class EntityCompilationOptions
	{
		/// <summary>
		/// Private field to support <c>DatabaseName</c> property.
		/// </summary>
		private string _databaseName;
		
		/// <summary>
		/// Private field to support <c>AllowCustomEnumerations</c> property.
		/// </summary>
		private bool _allowCustomEnumerations = true;
		
		/// <summary>
		/// Private field to support <see cref="Language"/> property.
		/// </summary>
		private Language _language;
		
		/// <summary>
		/// Private field to support <c>CompilerOptions</c> property.
		/// </summary>
		private CompilerParameters _compilerOptions;
		
		/// <summary>
		/// Constructor.
		/// </summary>
		public EntityCompilationOptions(Language language)
		{
			_language = language;
			_compilerOptions = CodeDomExtender.GetDefaultCompilerOptions(this.Language);
		}

		/// <summary>
		/// Gets the language.
		/// </summary>
		public Language Language
		{
			get
			{
				return _language;
			}
		}

		/// <summary>
		/// Gets the compileroptions.
		/// </summary>
		public CompilerParameters CompilerOptions
		{
			get
			{
				// returns the value...
				return _compilerOptions;
			}
		}

		/// <summary>
		/// Gets or sets the allowcustomenumerations
		/// </summary>
		public bool AllowCustomEnumerations
		{
			get
			{
				return _allowCustomEnumerations;
			}
			set
			{
				// check to see if the value has changed...
				if(value != _allowCustomEnumerations)
				{
					// set the value...
					_allowCustomEnumerations = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the databasename
		/// </summary>
		public string DatabaseName
		{
			get
			{
				return _databaseName;
			}
			set
			{
				// check to see if the value has changed...
				if(value != _databaseName)
				{
					// set the value...
					_databaseName = value;
				}
			}
		}

		/// <summary>
		/// Returns true if the option has a database name.
		/// </summary>
		public bool HasDatabaseName
		{
			get
			{
				if(this.DatabaseName == null || this.DatabaseName.Length == 0)
					return false;
				else
					return true;
			}
		}
	}
}
