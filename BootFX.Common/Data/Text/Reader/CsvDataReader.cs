// BootFX - Application framework for .NET applications
// 
// File: CsvDataReader.cs
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
using System.Data;
using System.Collections;

namespace BootFX.Common.Data.Text
{
	/// <summary>
	/// Defines an instance of <c>CsvDataReader</c>.
	/// </summary>
	public class CsvDataReader : DelimitedDataReader
	{
        public CsvDataReader(TextReader reader, bool firstRowIsHeader = true)
			: base(reader, firstRowIsHeader, ',')
		{
			if(!(firstRowIsHeader))
				throw new NotSupportedException("The first row must contain headers.");
		}
	}
}
