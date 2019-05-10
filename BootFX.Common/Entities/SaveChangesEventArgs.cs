// BootFX - Application framework for .NET applications
// 
// File: SaveChangesEventArgs.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using BootFX.Common;
using BootFX.Common.Data;

namespace BootFX.Common.Entities
{
	/// <summary>
	/// Summary description for SaveChangesEventArgs.
	/// </summary>
	public abstract class SaveChangesEventArgs : EntityEventArgs
	{
		/// <summary>
		/// Private field to support <see cref="Connection"/> property.
		/// </summary>
		private IConnection _connection;
		
		/// <summary>
		/// Private field to support <see cref="unit"/> property.
		/// </summary>
		private IWorkUnit _unit;

		// mbr - 02-10-2007 - case 827 - added connection.		
//		protected SaveChangesEventArgs(IWorkUnit unit)
		protected SaveChangesEventArgs(IWorkUnit unit, IConnection connection)
			: base(unit.Entity)
		{
			this.SetUnit(unit);

			// set...
			_connection = connection;
		}

		/// <summary>
		/// Sets the work unit.
		/// </summary>
		/// <param name="unit"></param>
		internal void SetUnit(IWorkUnit unit)
		{
			_unit = unit;
		}

		/// <summary>
		/// Gets the unit.
		/// </summary>
		public IWorkUnit Unit
		{
			get
			{
				return _unit;
			}
		}

		public WorkUnitType WorkUnitType
		{
			get
			{
				if(Unit == null)
					throw new InvalidOperationException("Unit is null.");
				return this.Unit.WorkUnitType;
			}
		}

		public SqlInsertWorkUnit InsertWorkUnit
		{
			get
			{
				return this.Unit as SqlInsertWorkUnit;
			}
		}

		public SqlUpdateWorkUnit UpdateWorkUnit
		{
			get
			{
				return this.Unit as SqlUpdateWorkUnit;
			}
		}

		public SqlDeleteWorkUnit DeleteWorkUnit
		{
			get
			{
				return this.Unit as SqlDeleteWorkUnit;
			}
		}

		/// <summary>
		/// Gets the connection.
		/// </summary>
		public IConnection Connection
		{
			get
			{
				return _connection;
			}
		}
	}
}
