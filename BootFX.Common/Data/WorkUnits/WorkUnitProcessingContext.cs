// BootFX - Application framework for .NET applications
// 
// File: WorkUnitProcessingContext.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using BootFX.Common.Data.Formatters;
using BootFX.Common.Entities;

namespace BootFX.Common.Data
{
	/// <summary>
	/// Describes a context used with processing work units.
	/// </summary>
	public class WorkUnitProcessingContext
	{
		/// <summary>
		/// Private field to support <see cref="Connection"/> property.
		/// </summary>
		private IConnection _connection = null;
		
		/// <summary>
		/// Private field to support <c>Bag</c> property.
		/// </summary>
		private WorkUnitResultsBag _bag;
		
		/// <summary>
		/// Private field to support <c>LastCreatedId</c> property.
		/// </summary>
		private object[] _lastCreatedId = new object[] {};

		/// <summary>
		/// Constructor.
		/// </summary>
		// mbr - 26-11-2007 - don't take a connection at this point...
//		internal WorkUnitProcessingContext(IConnection connection)
		internal WorkUnitProcessingContext(ITimingBucket timings)
		{
//			if(connection == null)
//				throw new ArgumentNullException("connection");
//			_connection = connection;

			this.ResetBag();
		}

		/// <summary>
		/// Gets the connection.
		/// </summary>
		// mbr - 28-09-2007 - made public..
		public IConnection Connection
		{
			get
			{
				return _connection;
			}
		}

		internal SqlDialect Dialect
		{
			get
			{
                return this.Connection.Dialect;
			}
		}

		/// <summary>
		/// Resets the context bag.
		/// </summary>
		public void ResetBag()
		{
			_bag = new WorkUnitResultsBag();
		}

		/// <summary>
		/// Gets the bag.
		/// </summary>
		public WorkUnitResultsBag Bag
		{
			get
			{
				// returns the value...
				return _bag;
			}
		}

		/// <summary>
		/// Sets the connection.
		/// </summary>
		/// <param name="connection"></param>
		internal void SetConnection(IConnection connection)
		{
			if(connection == null)
				throw new ArgumentNullException("connection");
			
			// set...
			_connection = connection;
		}
    }
}
