// BootFX - Application framework for .NET applications
// 
// File: DtoLinkAttribute.cs
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
using System.Web;

namespace BootFX.Common.Dto
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple=true)]
    public class DtoLinkAttribute : Attribute
    {
        public string JsonName { get; private set; }

        public DtoLinkAttribute(string jsonName)
        {
            this.JsonName = jsonName;
        }
    }
}
