// BootFX - Application framework for .NET applications
// 
// File: GetEntriesFlags.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;

namespace BootFX.Common.Management
{
	/// <summary>
	/// Summary description for GetEntriesFlags.
	/// </summary>
	// mbr - 20-08-2007 - added.	
	public enum GetEntriesFlags
	{
		NonAuditItems = 1,
		AuditItems = 2,
		All = NonAuditItems | AuditItems
	}
}
