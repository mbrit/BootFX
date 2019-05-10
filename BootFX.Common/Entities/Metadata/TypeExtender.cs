// BootFX - Application framework for .NET applications
// 
// File: TypeExtender.cs
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

namespace BootFX.Common.Entities
{
    public static class TypeExtender
    {
        public static EntityType ToEntityType(this Type type)
        {
            return EntityType.GetEntityType(type);
        }

        public static string GetPartiallyQualifiedName(this Type type)
        {
            return Runtime.Current.GetPartiallyQualifiedName(type);
        }
    }
}
