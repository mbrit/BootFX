// BootFX - Application framework for .NET applications
// 
// File: GeneratedTypeDeclaration.cs
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
using BootFX.Common.Data.Schema;

namespace BootFX.Common.Entities
{
    public class GeneratedTypeDeclaration : GeneratedCode<CodeTypeDeclaration>
    {
        internal GeneratedTypeDeclaration(CodeTypeDeclaration t)
            : base(t)
        {
        }

        internal GeneratedTypeDeclaration(CodeTypeDeclaration t, SqlTable table)
            : base(t)
        {
            this.GenericMappings.Add("`1", "<" + table.Name + ">");
        }
    }
}
