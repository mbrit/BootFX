// BootFX - Application framework for .NET applications
// 
// File: OperationLogItemFormatFlags.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;

namespace BootFX.Common.Management
{
	/// <summary>
	/// Defines an instance of <c>OperationItemFormatFlags</c>.
	/// </summary>
	[Flags()]
	public enum OperationLogItemFormatFlags
	{
		IncludeFullException = 1,
		IncludeExceptionMessage = 2,
		IncludeLogLevel = 4,
		UseBrTagSeparator = 8
	}
}
