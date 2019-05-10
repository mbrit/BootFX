// BootFX - Application framework for .NET applications
// 
// File: StructuredParameterData.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BootFX.Common.Data
{
    public class StructuredParameterData
    {
        internal string NativeTypeName { get; private set; }
        internal DataTable Data { get; private set; }

        public StructuredParameterData(string nativeTypeName, DataTable data)
        {
            if (nativeTypeName == null)
                throw new ArgumentNullException("nativeTypeName");
            if (nativeTypeName.Length == 0)
                throw new ArgumentException("'nativeTypeName' is zero-length.");
            if (data == null)
                throw new ArgumentNullException("data");

            this.NativeTypeName = nativeTypeName;
            this.Data = data;
        }
    }
}
