// BootFX - Application framework for .NET applications
// 
// File: GeneratedCode.cs
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
using System.CodeDom;

namespace BootFX.Common.Entities
{
    public class GeneratedCode<T> : IGeneratedCode
        where T : CodeObject
    {
        public T CodeItem { get; private set; }
        public Dictionary<string, string> GenericMappings { get; private set; }

        internal GeneratedCode(T unit)
        {
            this.CodeItem = unit;
            this.GenericMappings = new Dictionary<string, string>();
        }
    }
}
