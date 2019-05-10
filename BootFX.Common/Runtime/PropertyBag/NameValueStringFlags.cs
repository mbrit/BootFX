// BootFX - Application framework for .NET applications
// 
// File: NameValueStringFlags.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Text;

namespace BootFX.Common
{
    // mbr - 2009-11-02 - added - in some instances, we need to support whitespaces in the values...
    [Flags()]
    public enum NameValueStringFlags
    {
        Normal = 0,
        AllowWhitespaceInValues = 1
    }
}
