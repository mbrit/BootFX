// BootFX - Application framework for .NET applications
// 
// File: SchemaWorkUnitType.cs
// Build: 5.2.10321.2307
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

namespace BootFX.Common.Data.Schema
{
	/// <summary>
	/// Defines schema work unit types.
	/// </summary>
	/// <remarks>This enumeration is used to sort work schema work units.</remarks>
	internal enum SchemaWorkUnitType
	{
		CreateTable,
		DropIndex,
		DropConstraint,
		DropForeignKey,
		AddColumn,
		AlterColumn,
		CreateIndex,
		AddConstraint,
		CreateForeignKey,
		Unknown
	}
}
