// BootFX - Application framework for .NET applications
// 
// File: PropertyBagByNameBaseWithSafeMethods.cs
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
    [Serializable()]
    public abstract class PropertyBagByNameBaseWithSafeMethods : PropertyBagByNameBase
    {
        public string GetStringValue(string name)
        {
            return GetStringValue(name, null, Cultures.System, OnNotFound.ReturnNull);
        }

        public bool GetBooleanValue(string name)
        {
            return GetBooleanValue(name, false, Cultures.System, OnNotFound.ReturnNull);
        }
    }
}
