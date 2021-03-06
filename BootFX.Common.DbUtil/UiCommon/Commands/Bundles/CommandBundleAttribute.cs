// BootFX - Application framework for .NET applications
// 
// File: CommandBundleAttribute.cs
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

namespace BootFX.Common.UI
{
	/// <summary>
	/// Defines an instance of <c>CommandBundleAttribute</c>.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple=true)]
	public sealed class CommandBundleAttribute : Attribute
	{
		/// <summary>
		/// Private field to support <see cref="Type"/> property.
		/// </summary>
		private Type _type;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="type"></param>
		public CommandBundleAttribute(Type type)
		{
			if(type == null)
				throw new ArgumentNullException("type");
			_type = type;
		}

		/// <summary>
		/// Gets the type.
		/// </summary>
		public Type Type
		{
			get
			{
				return _type;
			}
		}
	}
}
