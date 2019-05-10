// BootFX - Application framework for .NET applications
// 
// File: SqlMemberView.cs
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
using BootFX.Common.CodeGeneration;

namespace BootFX.Common.DbUtil
{
	/// <summary>
	/// Summary description for SqlMemberView.
	/// </summary>
	public class SqlMemberView 
	{
		/// <summary>
		/// Private field to support <see cref="Member"/> property.
		/// </summary>
		private SqlMember _member;
		
		public SqlMemberView(SqlMember member)
		{
			if(member == null)
				throw new ArgumentNullException("member");
			_member = member;
		}

		/// <summary>
		/// Gets the member.
		/// </summary>
		public SqlMember Member
		{
			get
			{
				return _member;
			}
		}
		
		[Category("Identification"), Description("Gets or sets the name that the member will be known as within your code.")]
		public string Name
		{
			get
			{
				if(Member == null)
					throw new InvalidOperationException("Member is null.");
				return this.Member.Name;
			}
			set
			{
				if(Member == null)
					throw new InvalidOperationException("Member is null.");
				this.Member.Name = value;
			}
		}

		public string CamelName
		{
			get
			{
				return CodeDomHelper.GetCamelName(this.Name);
			}
		}

		public string PascalName
		{
			get
			{
				return CodeDomHelper.GetPascalName(this.Name);
			}
		}

		public string LowerName
		{
			get
			{
				return this.Name.ToLower();
			}
		}

		[Category("Code Generation"), Description("Gets or sets whether to include the member in code generation.")]
		public bool Generate
		{
			get
			{
				if(Member == null)
					throw new InvalidOperationException("Member is null.");
				return this.Member.Generate;
			}
			set
			{
				if(Member == null)
					throw new InvalidOperationException("Member is null.");
				this.Member.Generate = value;
			}
		}

		[Category("Identification"), Description("Gets the name by which the member is known as in the underlying store.")]
		public string NativeName
		{
			get
			{
				if(Member == null)
					throw new InvalidOperationException("Member is null.");
				return this.Member.NativeName;
			}
		}
	}
}
