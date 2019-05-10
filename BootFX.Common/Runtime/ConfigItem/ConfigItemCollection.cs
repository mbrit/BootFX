// BootFX - Application framework for .NET applications
// 
// File: ConfigItemCollection.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;

namespace BootFX.Common
{
	using System;
	using System.IO;
	using System.Text;
	using System.Text.RegularExpressions;
	using System.Data;
	using System.Collections;
	using System.Collections.Specialized;
	using BootFX.Common;
	using BootFX.Common.Data;
	using BootFX.Common.Entities;
	using BootFX.Common.Entities.Attributes;
    
    
	/// <summary>
	/// Defines the collection for entities of type <see cref="ConfigItem"/>.
	/// </summary>
	[Serializable()]
	public class ConfigItemCollection : ConfigItemCollectionBase
	{
        
		/// <summary>
		/// Constructor.
		/// </summary>
		public ConfigItemCollection()
		{
		}
        
		/// <summary>
		/// Deserialization constructor.
		/// </summary>
		protected ConfigItemCollection(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : 
			base(info, context)
		{
		}
	}
}
