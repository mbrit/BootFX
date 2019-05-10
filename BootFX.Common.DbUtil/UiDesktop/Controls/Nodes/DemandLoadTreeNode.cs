// BootFX - Application framework for .NET applications
// 
// File: DemandLoadTreeNode.cs
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
using System.Windows.Forms;

namespace BootFX.Common.UI.Desktop
{
	/// <summary>
	/// Defines an instance of <c>DemandLoadTreeNode</c>.
	/// </summary>
	internal class DemandLoadTreeNode : TreeNode
	{
		internal DemandLoadTreeNode()
		{
			this.Text = "...";
		}
	}
}
