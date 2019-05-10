// BootFX - Application framework for .NET applications
// 
// File: ServiceEngineStatus.cs
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

namespace BootFX.Common.Services
{
    public enum ServiceEngineStatus
    {
        Instantiated = 0,
        Active = 1,
        Idle = 2,
        Disposed = 3,
    }
}
