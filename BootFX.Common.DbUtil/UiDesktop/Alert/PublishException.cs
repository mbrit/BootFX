// BootFX - Application framework for .NET applications
// 
// File: PublishException.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;

namespace BootFX.Common.UI.Desktop
{
	/// <summary>
	/// Specifies whether to publish an exception or not.
	/// </summary>
	public enum PublishExceptionMode
	{
		/// <summary>
		/// Indicates that the exception should be published.
		/// </summary>
		PublishException = 0,

		/// <summary>
		/// Indicates that the exception should not be published.
		/// </summary>
		DoNotPublishException = 1
	}
}
