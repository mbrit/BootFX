// BootFX - Application framework for .NET applications
// 
// File: BeforeSaveChangesEventArgs.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using BootFX.Common.Data;

namespace BootFX.Common.Entities
{
	/// <summary>
	/// Delegate for use with <see cref="BeforeSaveChangesEventArgs"></see>.
	/// </summary>
	public delegate void BeforeSaveChangesEventHandler(object sender, BeforeSaveChangesEventArgs e);

	/// <summary>
	/// Event args for 'save changes' notifications.
	/// </summary>
	public class BeforeSaveChangesEventArgs : SaveChangesEventArgs
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		// mbr - 02-10-2007 - case 827 - changed.		
		[Obsolete("Use the overload that takes an IConnection instance.")]
		public BeforeSaveChangesEventArgs(IWorkUnit unit)
			: this(unit, null)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		// mbr - 02-10-2007 - case 827 - changed.		
		public BeforeSaveChangesEventArgs(IWorkUnit unit, IConnection connection)
			: base(unit, connection)
		{
		}

		/// <summary>
		/// Rebuilds the work units based on changes to the entity.
		/// </summary>
		// mbr - 06-09-2007 - for c7 - added.		
		public void RebuildWorkUnits()
		{
			// check...
			if(EntityType == null)
				throw new InvalidOperationException("EntityType is null.");
			if(Unit == null)
				throw new InvalidOperationException("Unit is null.");

			// get the persistence service...
			IEntityPersistence persistence = EntityType.Persistence;
			if(persistence == null)
				throw new InvalidOperationException("persistence is null.");

			// any good?
			if(!(persistence is EntityPersistence))
				throw new NotSupportedException(string.Format("A persistence type of '{0}' is not supported.", persistence));

			// now what?
			IWorkUnit newUnit = null;
			EntityPersistence sqlPersistence = (EntityPersistence)persistence;
			if(this.Unit is SqlInsertWorkUnit)
				newUnit = sqlPersistence.CreateInsertWorkUnit(this.Entity);
			else if(this.Unit is SqlUpdateWorkUnit)
				newUnit = sqlPersistence.CreateUpdateWorkUnit(this.Entity);
			else if(this.Unit is SqlDeleteWorkUnit)
				newUnit = sqlPersistence.CreateDeleteWorkUnit(this.Entity);
				// mbr - 29-11-2007 - for c7 - added...
			else if(this.Unit is IExtendedDataWorkUnit)
			{
				// the provider provides these, and there must only be one to be supported here...
				if(Database.ExtensibilityProvider == null)
					throw new InvalidOperationException("Database.ExtensibilityProvider is null.");
				IWorkUnit[] newUnits = Database.ExtensibilityProvider.GetReplacementWorkUnits(((IExtendedDataWorkUnit)this.Unit));
				if(newUnits == null)
					throw new InvalidOperationException("'newUnits' is null.");
				if(newUnits.Length == 0)
					throw new InvalidOperationException("'newUnits' is zero-length.");
				if(newUnits.Length > 1)
					throw new InvalidOperationException(string.Format("'newUnits' has an invalid length of '{0}'.", newUnits.Length));

				// set...
				newUnit = newUnits[0];
			}
			else
				throw new NotSupportedException(string.Format("Cannot handle '{0}'.", this.Unit.GetType()));

			// check...
			if(newUnit == null)
				throw new InvalidOperationException("newUnit is null.");
			SetUnit(newUnit);
		}
	}
}
