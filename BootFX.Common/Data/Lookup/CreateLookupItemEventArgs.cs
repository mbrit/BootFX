// BootFX - Application framework for .NET applications
// 
// File: CreateLookupItemEventArgs.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;

namespace BootFX.Common
{
	/// <summary>
	/// Delegate for use with <see cref="CreateLookupItemEventArgs"></see>.
	/// </summary>
	public delegate void CreateLookupItemEventHandler(object sender, CreateLookupItemEventArgs e);

	/// <summary>
	/// Event arguments for <see cref="Lookup"/> object item creation.
	/// </summary>
	public class CreateLookupItemEventArgs : EventArgs
	{
		/// <summary>
		/// Private field to support <c>NewValue</c> property.
		/// </summary>
		private object _newValue;
		
		/// <summary>
		/// Private field to support <see cref="Key"/> property.
		/// </summary>
		private object _key;
		
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="key"></param>
		public CreateLookupItemEventArgs(object key)
		{
			_key = key;
		}

		/// <summary>
		/// Gets the key.
		/// </summary>
		public object Key
		{
			get
			{
				return _key;
			}
		}

		/// <summary>
		/// Gets or sets the newvalue
		/// </summary>
		public object NewValue
		{
			get
			{
				return _newValue;
			}
			set
			{
				// check to see if the value has changed...
				if(value != _newValue)
				{
					// set the value...
					_newValue = value;
				}
			}
		}
	}
}
