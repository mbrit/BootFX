// BootFX - Application framework for .NET applications
// 
// File: SyncSchemaDatabaseUpdateStep.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Collections.Specialized;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using BootFX.Common;
using BootFX.Common.Data.Schema;
using BootFX.Common.Data.Formatters;

namespace BootFX.Common.Data
{
	/// <summary>
	/// Defines an instance of <c>SyncSchemaDatabaseUpdateStep</c>.
	/// </summary>
	internal class SyncSchemaDatabaseUpdateStep : DatabaseUpdateStep
	{
		/// <summary>
		/// Private field to support <see cref="WorkUnits"/> property.
		/// </summary>
		private WorkUnitCollection _workUnits = new WorkUnitCollection();
		
		/// <summary>
		/// Constructor.
		/// </summary>
		internal SyncSchemaDatabaseUpdateStep()
		{
		}

		/// <summary>
		/// Execute.
		/// </summary>
		/// <param name="context"></param>
		public override void Execute(DatabaseUpdateContext context)
		{
			// process...
			context.Operation.Status = "Applying schema changes...";
			WorkUnitProcessor processor = new WorkUnitProcessor();
			processor.Process(this.WorkUnits, context.Trace, context.Operation);
		}

		/// <summary>
		/// Gets the workunits.
		/// </summary>
		internal WorkUnitCollection WorkUnits
		{
			get
			{
				return _workUnits;
			}
		}

		public override void GetFriendlyWorkMessages(StringCollection messages)
		{
			if(messages == null)
				throw new ArgumentNullException("messages");
			
			// walk...
			for(int index = 0; index < this.WorkUnits.Count; index++)
				messages.Add(this.WorkUnits[index].ToString());
		}

		public override bool IsUpToDate
		{
			get
			{
				if(this.WorkUnits.Count > 0)
					return false;
				else
					return true;
			}
		}

		/// <summary>
		/// Populates the sync step based on the schema.
		/// </summary>
		internal void Initialize(SqlSchema entitySchema, SqlSchema databaseSchema)
		{
			if(entitySchema == null)
				throw new ArgumentNullException("entitySchema");
			if(databaseSchema == null)
				throw new ArgumentNullException("databaseSchema");			
			
			// get...
			WorkUnitCollection units = entitySchema.GetSchemaWorkUnits(databaseSchema);
			if(units == null)
				throw new InvalidOperationException("units is null.");

			// set...
			_workUnits = units;
		}
	}
}
