// BootFX - Application framework for .NET applications
// 
// File: SqlProceduresNode.cs
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
using BootFX.Common.UI.Desktop;
using BootFX.Common.Data.Schema;

namespace BootFX.Common.DbUtil
{
	/// <summary>
	/// Defines an instance of <c>SqlSprocsNode</c>.
	/// </summary>
	internal class SqlProceduresNode : ObjectTreeNode
	{
		internal SqlProceduresNode(SqlSchema schema)
		{
			if(schema == null)
				throw new ArgumentNullException("schema");
			
			// set...
			this.Text = "Stored Procedures";
			this.DemandLoad = false;

			// add...
			foreach(SqlProcedure proc in schema.Procedures)
				this.Nodes.Add(new SqlProcedureNode(proc));
		}
	}
}
