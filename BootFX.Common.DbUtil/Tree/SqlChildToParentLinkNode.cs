// BootFX - Application framework for .NET applications
// 
// File: SqlChildToParentLinkNode.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using BootFX.Common.UI.Desktop;
using BootFX.Common.Data.Schema;
using BootFX.Common.Entities;

namespace BootFX.Common.DbUtil
{
	/// <summary>
	/// Summary description for SqlChildToParentLinkNode.
	/// </summary>
	internal class SqlChildToParentLinkNode : SqlMemberNode
	{
		internal SqlChildToParentLinkNode(SqlChildToParentLink link) : base(link)
		{
			// set...
			if(!(link.ParentTable.Generate))
				this.Text += " (parent not generated)";
		}

        private SqlChildToParentLink Link
        {
            get
            {
                return (SqlChildToParentLink)this.Member;
            }
        }

        internal override void SelectIcon()
        {
            base.SelectIcon();
            if (Member.Generate)
            {
                if(this.Link.GenerateDtoField)
                    this.ImageResourceName = "BootFX.Common.DbUtil.Resources.ChildToParentGenerateDto.ico";
                else
                    this.ImageResourceName = "BootFX.Common.DbUtil.Resources.ChildToParentGenerate.ico";
            }
            else
                this.ImageResourceName = "BootFX.Common.DbUtil.Resources.NonChildToParentGenerate.ico";
		}
	}
}
