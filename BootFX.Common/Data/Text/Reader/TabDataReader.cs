// BootFX - Application framework for .NET applications
// 
// File: TabDataReader.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace BootFX.Common.Data.Text
{
    public class TabDataReader : DelimitedDataReader
    {
		public TabDataReader(TextReader reader, bool firstRowIsHeader) 
			: base(reader, firstRowIsHeader, '\t')
		{
			if(!(firstRowIsHeader))
				throw new NotSupportedException("The first row must contain headers.");
		}
    }
}
