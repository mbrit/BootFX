// BootFX - Application framework for .NET applications
// 
// File: LazySaveQueueEntry.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using BootFX.Common.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BootFX.Common
{
    internal class LazySaveQueueEntry
    {
        internal Entity Entity { get; private set; }
        internal ManualResetEvent Wait { get; private set; }
        internal Exception Exception { get; set; }
        internal bool IsOk { get; set; }

        internal LazySaveQueueEntry(Entity entity)
        {
            this.Entity = entity;
            this.Wait = new ManualResetEvent(false);
        }
    }
}
