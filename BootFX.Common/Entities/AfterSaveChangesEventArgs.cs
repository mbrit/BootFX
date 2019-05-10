// BootFX - Application framework for .NET applications
// 
// File: AfterSaveChangesEventArgs.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Collections;
using BootFX.Common.Data;

namespace BootFX.Common.Entities
{
	/// <summary>
	/// Delegate for use with <see cref="AfterSaveChangesEventArgs"></see>.
	/// </summary>
	public delegate void AfterSaveChangesEventHandler(object sender, AfterSaveChangesEventArgs e);

	/// <summary>
	/// Event args for 'save changes' notifications.
	/// </summary>
	public class AfterSaveChangesEventArgs : SaveChangesEventArgs
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		// mbr - 02-10-2007 - case 827 - changed.		
		[Obsolete("Use the overload that takes an IConnection instance.")]
		public AfterSaveChangesEventArgs(IWorkUnit unit)
			: this(unit, null)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		// mbr - 02-10-2007 - case 827 - changed.		
		public AfterSaveChangesEventArgs(IWorkUnit unit, IConnection connection)
			: base(unit, connection)
		{
		}

		/// <summary>
		/// Gets the resultsbag.
		/// </summary>
		public WorkUnitResultsBag ResultsBag
		{
			get
			{
				if(Unit == null)
					throw new InvalidOperationException("Unit is null.");
				return this.Unit.ResultsBag;
			}
		}
	}
}
