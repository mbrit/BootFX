// BootFX - Application framework for .NET applications
// 
// File: SqlMemberNode.cs
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
using System.ComponentModel;

namespace BootFX.Common.DbUtil
{
	/// <summary>
	/// Summary description for SqlMemberNode.
	/// </summary>
	internal abstract class SqlMemberNode : ObjectTreeNode
	{
		internal SqlMemberNode(SqlMember member) : base(member)
		{
			if(member == null)
				throw new ArgumentNullException("member");
			
			// subscribe...
			member.NameChanged += new EventHandler(member_NameChanged);
			member.PropertyChanged += new PropertyChangedEventHandler(member_GenerateChanged);
		}

		/// <summary>
		/// Gets the text for the node.
		/// </summary>
		/// <returns></returns>
		protected override string GetText()
		{
			if(Member == null)
				throw new InvalidOperationException("Member is null.");

			// name...
			string text = Member.Name;
			if(Member.Name != Member.NativeName)
				text = string.Format("{0} [{1}]", Member.Name, Member.NativeName);

			// gen
			if(!(this.Member.Generate))
				text += " (Not generated)";

			// return...
			return text;
		}

		/// <summary>
		/// Gets the member.
		/// </summary>
		internal SqlMember Member
		{
			get
			{
				// returns the value...
				return (SqlMember)this.InnerObject;
			}
		}

		private void member_NameChanged(object sender, EventArgs e)
		{
			this.RefreshTextAndIcon();
		}

		private void member_GenerateChanged(object sender, PropertyChangedEventArgs e)
		{
			this.SelectIcon();
			this.RefreshTextAndIcon();
		}

		internal virtual void SelectIcon()
		{
		}

		protected override void OnInnerObjectChanged(EventArgs e)
		{
			base.OnInnerObjectChanged (e);
			this.SelectIcon();
		}
	}
}
