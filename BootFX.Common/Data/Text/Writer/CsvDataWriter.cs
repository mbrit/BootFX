// BootFX - Application framework for .NET applications
// 
// File: CsvDataWriter.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;

namespace BootFX.Common.Data.Text
{
	/// <summary>
	/// Defines an instance of <c>CsvDataWriter</c>.
	/// </summary>
	public class CsvDataWriter : DelimitedDataWriter
	{
        public CsvDataWriter(TextWriter writer)
            : this(writer, "\"")
        {
        }

        public CsvDataWriter(TextWriter writer, string quote)
            : base(writer, ",", quote)
		{
		}
	}
}
