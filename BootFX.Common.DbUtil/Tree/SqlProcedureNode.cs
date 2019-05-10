// BootFX - Application framework for .NET applications
// 
// File: SqlProcedureNode.cs
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
using BootFX.Common.Data;
using BootFX.Common.Data.Schema;

namespace BootFX.Common.DbUtil
{
	/// <summary>
	/// Defines an instance of <c>SqlProcedureNode</c>.
	/// </summary>
	internal class SqlProcedureNode : SqlMemberNode
	{
		internal SqlProcedureNode(SqlProcedure proc) : base(proc)
		{
			this.DemandLoad = false;
		}
	}
}
