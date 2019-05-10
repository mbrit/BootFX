// BootFX - Application framework for .NET applications
// 
// File: DtoLinkWrapper.cs
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
using System.Threading.Tasks;

namespace BootFX.Common.Dto
{
    internal class DtoLinkWrapper
    {
        private bool _isLoaded;
        private IDtoBase _dto;

        internal DtoLinkWrapper()
        {
            this.IsLoaded = false;
        }

        internal bool IsLoaded
        {
            get
            {
                return _isLoaded;
            }
            set
            {
                _isLoaded = value;
            }
        }

        internal IDtoBase Dto
        {
            get
            {
                return _dto;
            }
            set
            {
                _dto = value;
                this.IsLoaded = true;
            }
        }
    }
}
