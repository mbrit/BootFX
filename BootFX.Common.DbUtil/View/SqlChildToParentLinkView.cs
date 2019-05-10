// BootFX - Application framework for .NET applications
// 
// File: SqlChildToParentLinkView.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.ComponentModel;
using BootFX.Common.Data.Schema;

namespace BootFX.Common.DbUtil
{
	/// <summary>
	/// Summary description for SqlChildToParentLinkView.
	/// </summary>
	public class SqlChildToParentLinkView : SqlMemberView
	{
		public SqlChildToParentLinkView(SqlChildToParentLink link) : base(link)
		{
		}

		private SqlChildToParentLink Link
		{
			get
			{
				return (SqlChildToParentLink)base.Member;
			}
		}

		[Category("Code Generation"), Description("Gets or sets whehter to generate methods to access this parent on the entity.")]
		public bool GenerateParentAccessMethods
		{
			get
			{
				if(Link == null)
					throw new InvalidOperationException("Link is null.");
				return this.Link.GenerateParentAccessMethods;
			}
			set
			{
				if(Link == null)
					throw new InvalidOperationException("Link is null.");
				this.Link.GenerateParentAccessMethods = value;
			}
		}

        [Category("DTO"), Description("Gets or sets whehter to generate methods a DTO field for this link.")]
        public bool GenerateDtoField
        {
            get
            {
                if (Link == null)
                    throw new InvalidOperationException("Link is null.");
                return this.Link.GenerateDtoField;
            }
            set
            {
                if (Link == null)
                    throw new InvalidOperationException("Link is null.");
                this.Link.GenerateDtoField = value;
            }
        }

        [Category("DTO"), Description("Gets or sets the JSON name for the field.")]
        public string JsonName
        {
            get
            {
                if (Link == null)
                    throw new InvalidOperationException("Link is null.");
                return this.Link.JsonName;
            }
            set
            {
                if (Link == null)
                    throw new InvalidOperationException("Link is null.");
                this.Link.JsonName = value;
            }
        }
    }
}
