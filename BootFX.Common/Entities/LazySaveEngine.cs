// BootFX - Application framework for .NET applications
// 
// File: LazySaveEngine.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BootFX.Common.Entities;
using System.Threading;
using BootFX.Common.Management;

namespace BootFX.Common
{
    internal class LazySaveEngine : Loggable
    {
        private List<LazySaveQueueEntry> Queue { get; set; }
        private ReaderWriterLockSlim Lock { get; set; }
        private AutoResetEvent Wait { get; set; }

        private Thread Thread { get; set; }

        /// <summary>
		/// Private field to hold singleton instance.
		/// </summary>
		private static LazySaveEngine _current = new LazySaveEngine();

        /// <summary>
        /// Private constructor.
        /// </summary>
        private LazySaveEngine()
        {
            this.Queue = new List<LazySaveQueueEntry>();
            this.Lock = new ReaderWriterLockSlim();
            this.Wait = new AutoResetEvent(false);

            this.Thread = new Thread(this.ThreadEntryPoint)
            {
                Name = "Lazy save thread"
            };
            this.Thread.Start();
        }

        /// <summary>
        /// Gets the singleton instance of <see cref="LazySaveEngine">LazySaveEngine</see>.
        /// </summary>
        internal static LazySaveEngine Current
        {
            get
            {
                if (_current == null)
                    throw new ObjectDisposedException("LazySaveEngine");
                return _current;
            }
        }

        internal Task EnqueueAsync(Entity entity)
        {
            this.Lock.EnterWriteLock();
            try
            {
                var entry = new LazySaveQueueEntry(entity);

                var tcs = new TaskCompletionSource<LazySaveQueueEntry>(entry);
                ThreadPool.RegisterWaitForSingleObject(entry.Wait, (state, timedOut) =>
                {
                    var theTcs = (TaskCompletionSource<LazySaveQueueEntry>)state;
                    theTcs.SetResult(null);

                }, tcs, -1, true);

                this.Queue.Add(entry);
                this.Wait.Set();
                return tcs.Task;
            }
            finally
            {
                this.Lock.ExitWriteLock();
            }
        }

        private void ThreadEntryPoint(object state)
        {
            while (true)
            {
                try
                {
                    this.Wait.WaitOne();

                    // get the work...
                    IEnumerable<LazySaveQueueEntry> work = null;
                    this.Lock.EnterWriteLock();
                    try
                    {
                        work = new List<LazySaveQueueEntry>(this.Queue);
                        this.Queue.Clear();
                    }
                    finally
                    {
                        this.Lock.ExitWriteLock();
                    }

                    if (work.Any())
                    {
                        work.ProcessItems((item) =>
                        {
                            item.Entity.SaveChanges();
                            item.IsOk = true;

                        }, null, (item, ex) =>
                        {
                            item.Exception = ex;

                        }, (item) =>
                        {
                            item.Wait.Set();
                        });
                    }
                }
                catch (ThreadAbortException ex)
                {
                    var a = ex;
                    break;
                }
                catch (Exception ex)
                {
                    this.LogError(() => "An error occurred.", ex);
                }
            }
        }
    }
}
