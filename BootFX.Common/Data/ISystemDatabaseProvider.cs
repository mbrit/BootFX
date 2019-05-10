// BootFX - Application framework for .NET applications
// 
// File: ISystemDatabaseProvider.cs
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
using BootFX.Common.Data;

namespace BootFX.Common.Data
{
    public interface ISystemDatabaseProvider
    {
        ConnectionSettings GetConnectionSettings();
    }
}
