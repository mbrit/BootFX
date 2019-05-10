// BootFX - Application framework for .NET applications
// 
// File: MemberEditorEventArgs.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Windows.Forms;
using BootFX.Common.Entities;

namespace BootFX.Common.UI.Desktop
{
	/// <summary>
	/// Delegate for use with <see cref="MemberEditorEventArgs"></see>.
	/// </summary>
	public delegate void MemberEditorEventHandler(object sender, MemberEditorEventArgs e);

	/// <summary>
	/// Summary description for MemberEditorEventArgs.
	/// </summary>
	public class MemberEditorEventArgs : EventArgs
	{
		/// <summary>
		/// Private field to support <see cref="InnerEditor"/> property.
		/// </summary>
		private Control _innerEditor;

		/// <summary>
		/// Private field to support <see cref="Member"/> property.
		/// </summary>
		private EntityMember _member;
		
		/// <summary>
		/// Constructor.
		/// </summary>
		public MemberEditorEventArgs(Control innerEditor, EntityMember member)
		{
			if(innerEditor == null)
				throw new ArgumentNullException("innerEditor");
			if(member == null)
				throw new ArgumentNullException("member");
			
			_innerEditor = innerEditor;
			_member = member;
		}

		/// <summary>
		/// Gets the member.
		/// </summary>
		public EntityMember Member
		{
			get
			{
				return _member;
			}
		}
		
		/// <summary>
		/// Gets the innereditor.
		/// </summary>
		public Control InnerEditor
		{
			get
			{
				return _innerEditor;
			}
		}

		/// <summary>
		/// Gets the field.
		/// </summary>
		public EntityField Field
		{
			get
			{
				return this.Member as EntityField;
			}
		}

		/// <summary>
		/// Gets the field.
		/// </summary>
		public EntityLink Link
		{
			get
			{
				return this.Member as EntityLink;
			}
		}

		/// <summary>
		/// Gets the field.
		/// </summary>
		public ChildToParentEntityLink ChildToParentLink
		{
			get
			{
				return this.Member as ChildToParentEntityLink;
			}
		}
	}
}
