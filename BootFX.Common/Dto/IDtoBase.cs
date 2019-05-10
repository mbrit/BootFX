// BootFX - Application framework for .NET applications
// 
// File: IDtoBase.cs
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
    public interface IDtoBase
    {
        long Id
        {
            get;
        }

        object this[DtoField field]
        {
            get;
            set;
        }

        IDtoBase GetLink(DtoLink link);
        void SetLink(DtoLink link, IDtoBase dto);

        DtoType Type
        {
            get;
        }

        Dictionary<DtoField, object> GetChangedValues();
        Dictionary<DtoField, object> GetTouchedValues();

        void OnInitializingFromConcrete(IDtoCapable item);
        void OnInitializedFromConcrete(IDtoCapable item);

        bool IsLoaded(DtoLink link);
    }
}
