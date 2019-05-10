// BootFX - Application framework for .NET applications
// 
// File: SchemaWorkUnit.cs
// Build: 5.2.10321.2307
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
using BootFX.Common.Entities;
using BootFX.Common.Management;

namespace BootFX.Common.Data.Schema
{
	/// <summary>
	/// Defines an instance of <c>SqlSchemaWorkUnit</c>.
	/// </summary>
	public abstract class SchemaWorkUnit : IWorkUnit, IIsLoggable
	{
		/// <summary>
		/// Private field to support <see cref="Reason"/> property.
		/// </summary>
		private string _reason;
		
		/// <summary>
		/// Private field to support <see cref="ResultsBag"/> property.
		/// </summary>
		private WorkUnitResultsBag _resultsBag;
		
		/// <summary>
		/// Private field to support <see cref="EntityType"/> property.
		/// </summary>
		private EntityType _entityType = null;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="entityType"></param>
		internal SchemaWorkUnit(EntityType entityType)
		{
			_entityType = entityType;
		}

		public object Entity
		{
			get
			{
				return null;
			}
		}

		/// <summary>
		/// Gets the entitytype.
		/// </summary>
		public EntityType EntityType
		{
			get
			{
				return _entityType;
			}
		}

		/// <summary>
		/// Gets the resultsbag.
		/// </summary>
		public WorkUnitResultsBag ResultsBag
		{
			get
			{
				if(_resultsBag == null)
					_resultsBag = new WorkUnitResultsBag();
				return _resultsBag;
			}
		}

		public BootFX.Common.Data.WorkUnitType WorkUnitType
		{
			get
			{
				return WorkUnitType.Schema;
			}
		}

		/// <summary>
		/// Gets the statement(s) that have to be executed to complete this work unit.
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		public abstract SqlStatement[] GetStatements(WorkUnitProcessingContext context);

		/// <summary>
		/// Processes the work unit.
		/// </summary>
		/// <param name="context"></param>
		void IWorkUnit.Process(WorkUnitProcessingContext context, ITimingBucket timings)
		{
			this.Process(context, timings);
		}

		protected virtual void Process(WorkUnitProcessingContext context, ITimingBucket timings)
		{
			if(context == null)
				throw new ArgumentNullException("context");
			
			// walk...
			SqlStatement[] statements = this.GetStatements(context);
            foreach (SqlStatement statement in statements)
            {
                // mbr - 2016-08-08 - put a longer timeout on schema statements...
                statement.TimeoutSeconds = 5 * 60;

                try
                {
                    context.Connection.ExecuteNonQuery(statement);
                }
                catch (Exception ex)
                {
                    const string message = "The work unit failed.";
                    if (this.ContinueOnError)
                        this.LogError(() => message, ex);
                    else
                        throw new InvalidOperationException(message, ex);
                }
            }
		}

        protected virtual bool ContinueOnError
        {
            get
            {
                return false;
            }
        }

		/// <summary>
		/// Sets the bag for the results
		/// </summary>
		/// <param name="resultsBag"></param>
		void IWorkUnit.SetResultsBag(WorkUnitResultsBag resultsBag)
		{
			this.SetResultsBag(resultsBag);
		}

		protected virtual void SetResultsBag(WorkUnitResultsBag resultsBag)
		{			
		}

		public TouchedValueWrapper GetTouchedValues()
		{
			return new TouchedValueWrapper(new TouchedValueCollection[] {});
		}

		/// <summary>
		/// Gets the reason.
		/// </summary>
		internal string Reason
		{
			get
			{
				return _reason;
			}
		}

		protected void SetReason(string reason)
		{
			_reason = reason;
		}

        public bool NeedsTransaction
        {
            get
            {
                return false;
            }
        }
    }
}
