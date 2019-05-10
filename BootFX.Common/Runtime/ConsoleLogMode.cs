// BootFX - Application framework for .NET applications
// 
// File: Runtime.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

namespace BootFX.Common
{
    public enum ConsoleLogMode
    {
        Console = 0,
        Debug = 1,
        Trace = 2,
        Off = 3     // used for *nix where we need to control what goes to stdout...
    }
}