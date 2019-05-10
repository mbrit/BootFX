// BootFX - Application framework for .NET applications
// 
// File: LogActivityAttribute.cs
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
	/// Defines an attributed used for decorating properties that provide activity-based logging..
	/// </summary>
	/// <seealso cref="Loggable"></seealso>
	[AttributeUsage(AttributeTargets.Property)]
	public sealed class LogActivityAttribute : Attribute
	{
		public LogActivityAttribute(string activityName)
		{
			_activityName = activityName;
		}

		/// <summary>
		/// Private field to support <c>ActivityName</c> property.
		/// </summary>
		private string _activityName;
		
		/// <summary>
		/// Gets the activityname.
		/// </summary>
		public string ActivityName
		{
			get
			{
				return _activityName;
			}
		}
	}
}
