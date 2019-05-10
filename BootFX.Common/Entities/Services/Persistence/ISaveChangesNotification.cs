// BootFX - Application framework for .NET applications
// 
// File: ISaveChangesNotification.cs
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
	/// Interface for feedback to an entity that changes have occured.
	/// </summary>
	// mbr - 02-10-2007 - case 827 - made internal.	
	internal interface ISaveChangesNotification
	{
		/// <summary>
		/// Called before changes are saved.
		/// </summary>
		/// <param name="action"></param>
		// mbr - 02-10-2007 - case 827 - added transaction indication.
		//		void BeforeSaveChanges(IWorkUnit unit);
		IWorkUnit BeforeSaveChangesInTransaction(IWorkUnit unit, IConnection connection);
		void BeforeSaveChangesOutTransaction(IWorkUnit unit);

		/// <summary>
		/// Called after changes are saved.
		/// </summary>
		/// <param name="action"></param>
		// mbr - 02-10-2007 - case 827 - added transaction indication.
//		void AfterSaveChanges(IWorkUnit unit);
		void AfterSaveChangesInTransaction(IWorkUnit unit, IConnection connection);
		void AfterSaveChangesOutTransaction(IWorkUnit unit);
	}
}
