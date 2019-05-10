// BootFX - Application framework for .NET applications
// 
// File: ITimingBucket.cs
// Build: 5.0.61009.900
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

namespace BootFX.Common
{
    public interface ITimingBucket : ITimingElement
    {
        int NumItems
        {
            get;
            set;
        }

        IDisposable GetTimer(string name);
        ITimingBucket GetChildBucket(string name, int numItems = 1);

        string Dump(string preamble);

        void IncrementNumItems(int increment = 1);

        void AddTag(string tag);

        bool IsLogging
        {
            get;
        }
    }
}
