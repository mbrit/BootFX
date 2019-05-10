// BootFX - Application framework for .NET applications
// 
// File: EntityFieldFlags.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;

namespace BootFX.Common.Entities
{
	/// <summary>
	/// Describes flags used for field definitions.
	/// </summary>
	[Flags()]
	public enum EntityFieldFlags
	{
		/// <summary>
		/// Default, normal state.
		/// </summary>
		Normal = 0,

		/// <summary>
		/// Defines that that field is a key column.
		/// </summary>
		Key = 1,

		/// <summary>
		/// Defines that the field can contain a null value.
		/// </summary>
		/// <remarks>Cannot be used with <see cref="Key"></see>.</remarks>
		Nullable = 2,
		
		/// <summary>
		/// Defines that hte field is common.
		/// </summary>
		/// <remarks>Typically, when retrieving data, some fields are usually used, whereas some are rare.  Use this flag
		/// to define which fields are common.  Remember to make communication chunky rather than chatty - any unloaded
		/// fields will be demand loaded when they are requested.</remarks>
		Common = 4,

		/// <summary>
		/// Specifies a large field, e.g. a BLOB or SQL Server 'text/ntext' type.
		/// </summary>
		Large = 8,

		/// <summary>
		/// Ignore this field from XML *persistence* unless specifically requested.
		/// </summary>
		XmlIgnore = 16,

		/// <summary>
		/// Always persist this field as base-64.
		/// </summary>
		PersistAsBase64 = 32,

		/// <summary>
		/// Persist this field in a CData block using <see cref="SafeCDataStrategy.Base64Encode"></see> safe CData strategy 
		/// unless <see cref="ReplaceCDataEndMarker"></see> is specified.
		/// </summary>
		PersistAsCData = 64,

		/// <summary>
		/// If <see cref="PersistAsCData"></see> is specified, this will replace any CData end marker if found in the string.
		/// </summary>
		ReplaceCDataEndMarker = 128,

		/// <summary>
		/// Flags a field as being auto-incrementing.
		/// </summary>
		AutoIncrement = 256,

		// mbr - 12-12-2005 - both these are deprecated.
//		/// <summary>
//		/// Flags a field as being an extended property so will be persisted in a different manner
//		/// </summary>
//		ExtendedProperty = 512,
//		/// <summary>
//		/// Flags a field as being an extended lookup property so will be persisted in a different manner
//		/// </summary>
//		LookupProperty = 1024,

		/// <summary>
		/// Flags a field as being a partition ID.
		/// </summary>
		//PartitionId = 2048,
	}
}
