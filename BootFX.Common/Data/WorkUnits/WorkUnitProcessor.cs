// BootFX - Application framework for .NET applications
// 
// File: WorkUnitProcessor.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using BootFX.Common.Entities;
using BootFX.Common.Management;

namespace BootFX.Common.Data
{
	/// <summary>
	/// Processes work units.
	/// </summary>
	public class WorkUnitProcessor : LoggableMarshalByRefObject, IWorkUnitProcessor
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		public WorkUnitProcessor()
		{
		}

        ///// <summary>
        ///// Processes the given work units.
        ///// </summary>
        ///// <param name="units"></param>
        //public void Process(WorkUnitCollection units)
        //{
        //    this.Process(units, null);
        //}

        void IWorkUnitProcessor.Process(WorkUnitCollection units)
        {
            this.Process(units);
        }

		/// <summary>
		/// Processes the given work units.
		/// </summary>
		/// <param name="units"></param>
		public void Process(WorkUnitCollection units, bool trace = false, IOperationItem operation = null, bool outsideTransaction = false, ITimingBucket timings = null)
		{
			if(units == null)
				throw new ArgumentNullException("units");
			if(units.Count == 0)
				return;

			// operation...
            if (timings == null)
                timings = NullTimingBucket.Instance;
			if(operation == null)
				operation = NullOperationItem.Instance;

			// mbr - 02-10-2007 - case 827 - notify outside...
            // mbr - 2014-11-30 - changed this to load up the statements...
            var touchedEts = new List<EntityType>();
            var anyNeedsTransaction = units.Count > 1;
			var context = new WorkUnitProcessingContext(timings);
            using (timings.GetTimer("Count"))
            {
                foreach (IWorkUnit unit in units)
                {
                    ISaveChangesNotification notification = unit.Entity as ISaveChangesNotification;
                    if (notification != null)
                        notification.BeforeSaveChangesOutTransaction(unit);

                    // add...
                    if (!(touchedEts.Contains(unit.EntityType)))
                        touchedEts.Add(unit.EntityType);

                    // add...
                    if (unit.NeedsTransaction && !(anyNeedsTransaction))
                        anyNeedsTransaction = true;
                }
            }

			// mbr - 26-11-2007 - get a manager for this thread...
            IWorkUnitTransactionManager manager = null;
            using (timings.GetTimer("Initialize manager"))
            {
                if (outsideTransaction)
                    manager = new OutsideTransactionWorkUnitProcessorTransactionManager();
                else
                {
                    //manager = WorkUnitProcessorTransactionManager.Current;

                    // mbr - 2014-11-30 - we only want to use transactions if we are in a tranaction. the old
                    // approach created a transaction just for single row inserts, which turned out created
                    // quite a slowdown...
                    if (anyNeedsTransaction)
                        manager = WorkUnitProcessorTransactionManager.Current;
                    else
                        manager = WorkUnitProcessorNullTransactionManager.Current;
                }
                if (manager == null)
                    throw new InvalidOperationException("manager is null.");

                // initialise the manager...
                manager.Initialize();
            }

			try
			{
				// mbr - 26-11-2007 - now done in the manager...
//				if(connection == null)
//					connection = Database.CreateConnection(units[0].EntityType.DatabaseName);

				// mbr - 26-11-2007 - don't do transactions here (the manager will do it)...
//				connection.BeginTransaction();
				try
				{
					// reset the bag...
                    using(timings.GetTimer("Reset"))
					    context.ResetBag();

					// run...
					operation.ProgressMaximum = units.Count;
					operation.ProgressValue = 0;

					// mbr - 06-09-2007 - for c7 - the "before" event can replace the work unit.  (done by changing the entity and regenerating.)
					//foreach(IWorkUnit unit in units)
					for(int index = 0; index < units.Count; index++)
					{
                        using (var child = timings.GetChildBucket("Execute"))
                        {
                            // get a connection to use here...
                            IConnection connection = null;
                            using (child.GetTimer("Connect"))
                            {
                                connection = manager.GetConnection(units[index]);
                                if (connection == null)
                                    throw new InvalidOperationException("connection is null.");

                                // set...
                                context.SetConnection(connection);
                            }

                            // get the unit...
                            IWorkUnit unit = units[index];
                            try
                            {
                                if (trace)
                                    this.LogInfo(() => string.Format("Running unit '{0}'...", unit));

                                ISaveChangesNotification notification = unit.Entity as ISaveChangesNotification;
                                using (child.GetTimer("NotifyPre"))
                                {
                                    // before...
                                    if (notification != null)
                                    {
                                        // mbr - 02-10-2007 - case 827 - changed interface.								
                                        //								IWorkUnit newUnit = notification.BeforeSaveChanges(unit);
                                        IWorkUnit newUnit = notification.BeforeSaveChangesInTransaction(unit, connection);

                                        // patch it..
                                        if (newUnit != unit)
                                        {
                                            units[index] = newUnit;
                                            unit = newUnit;
                                        }
                                    }
                                }

                                // run it...
                                using(var childChild = child.GetChildBucket("Process"))
                                    unit.Process(context, childChild);

                                // set...
                                using(child.GetTimer("Results"))
                                    unit.SetResultsBag(context.Bag);

                                // mbr - 10-10-2007 - for c7 - do reconciliation here...
                                using (child.GetTimer("Reconcile"))
                                {
                                    WorkUnitCollection forReconciliation = new WorkUnitCollection();
                                    forReconciliation.Add(unit);
                                    unit.EntityType.Persistence.ReconcileWorkUnitProcessorResults(forReconciliation);
                                }

                                // mbr - 02-10-2007 - case 827 - call...
                                using (child.GetTimer("NotifyPost"))
                                {
                                    if (notification != null)
                                        notification.AfterSaveChangesInTransaction(unit, connection);
                                }
                            }
                            catch (Exception ex)
                            {
                                throw new InvalidOperationException(string.Format(Cultures.Exceptions, "Failed when processing '{0}'.", unit), ex);
                            }

                            // next...
                            operation.IncrementProgress();
                        }
					}

					// mbr - 26-11-2007 - it's the manager that needs to commit...
//					connection.Commit();

                    using(timings.GetTimer("Commit"))
					    manager.Commit();
				}
				catch(Exception ex)
				{
					// rollback...
					try
					{
						// mbr - 26-11-2007 - it's the manager that needs to rollback...
//						connection.Rollback();
						manager.Rollback(ex);
					}
					catch(Exception rollbackEx)
					{
						// mbr - 26-11-2007 - added.
						if(this.Log.IsWarnEnabled)
							this.Log.Warn("A further exception occurred whilst rolling back the transaction.", rollbackEx);
					}

					// throw...
					throw new InvalidOperationException("Failed to process work units.", ex);
				}
			}
			finally
			{
				// mbr - 26-11-2007 - get the manager to tear down...
//				if(connection != null)
//					connection.Dispose();
                using(timings.GetTimer("Dispose"))
				    manager.Dispose();

                // mbr - 2010-04-19 - invalidate the caches...
                //EntityCache.Invalidate(touchedEts);

                // flag...
                foreach (IWorkUnit unit in units)
                {
                    ISaveChangesNotification notification = unit.Entity as ISaveChangesNotification;
                    if (notification != null)
                        notification.AfterSaveChangesOutTransaction(unit);
                }
			}
		}
	}
}
