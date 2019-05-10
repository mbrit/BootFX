// BootFX - Application framework for .NET applications
// 
// File: IConnectionEventArgs.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;

namespace BootFX.Common.Data
{
	public delegate void IConnectionEventHandler(object state, IConnectionEventArgs e);

	/// <summary>
	/// Summary description for IConnectionEventArgs.
	/// </summary>
	public class IConnectionEventArgs : EventArgs
	{
		/// <summary>
		/// Private field to support <c>Connection</c> property.
		/// </summary>
		private IConnection _connection;

		/// <summary>
		/// Private field to support <see cref="AffinityState"/> property.
		/// </summary>
		private object _affinityState;
		
		/// <summary>
		/// Constructor.
		/// </summary>
		internal IConnectionEventArgs(object affinityState)
		{
			if(affinityState == null)
				throw new ArgumentNullException("affinityState");
			_affinityState = affinityState;
		}

		/// <summary>
		/// Gets the affinitystate.
		/// </summary>
		public object AffinityState
		{
			get
			{
				return _affinityState;
			}
		}

		/// <summary>
		/// Gets or sets the connection
		/// </summary>
		public IConnection Connection
		{
			get
			{
				return _connection;
			}
			set
			{
				// check to see if the value has changed...
				if(value != _connection)
				{
					// set the value...
					_connection = value;
				}
			}
		}
	}
}
