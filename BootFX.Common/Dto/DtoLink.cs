// BootFX - Application framework for .NET applications
// 
// File: DtoLink.cs
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
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BootFX.Common.Dto
{
    public class DtoLink : DtoMember
    {
        internal ChildToParentEntityLink Link { get; private set; }
        internal DtoField ReferenceField { get; set; }

        internal DtoLink(string name, string jsonName, PropertyInfo dtoProp, ChildToParentEntityLink link)
            : base(name, jsonName, dtoProp)
        {
            if (link == null)
                throw new ArgumentNullException("link");

            this.Link = link;
        }
    }
}
