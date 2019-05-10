// BootFX - Application framework for .NET applications
// 
// File: DtoEntityMemberField.cs
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
    public class DtoEntityMemberField : DtoField
    {
        public EntityMember Member { get; private set; }

        internal DtoEntityMemberField(string name, string jsonName, PropertyInfo dtoProp, EntityMember concreteMember)
            : base(name, jsonName, dtoProp)
        {
            if (concreteMember == null)
                throw new ArgumentNullException("concreteMember");

            this.Member = concreteMember;
        }

        internal override bool CanGetValueFromConcreteItem
        {
            get
            {
                return true;
            }
        }

        internal override object GetValueFromConcreteItem(IDtoCapable item)
        {
            return this.Member.GetValue(item);
        }
    }
}
