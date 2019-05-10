// BootFX - Application framework for .NET applications
// 
// File: EntityRuleApplies.cs
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
	/// Summary description for EntityRuleApplies.
	/// </summary>
	[Flags()]
	public enum EntityRuleApplies
	{
		None = 0,

//		BeforeInsertInTransaction = 1,
//		BeforeUpdateInTransaction = 2,
//		BeforeDeleteInTransaction = 4,
//		BeforeInsertOutTransaction = 8,
//		BeforeUpdateOutTransaction = 16,
//		BeforeDeleteOutTransaction = 32,
//
//		AfterInsertInTransaction = 64,
//		AfterUpdateInTransaction = 128,
//		AfterDeleteInTransaction = 256,
//		AfterInsertOutTransaction = 512,
//		AfterUpdateOutTransaction = 1024,
//		AfterDeleteOutTransaction = 2048,
//
//		AllBeforeInTransaction = BeforeInsertInTransaction | 
//			BeforeUpdateInTransaction | BeforeDeleteInTransaction,
//		AllAfterInTransaction = AfterInsertInTransaction | 
//			AfterUpdateInTransaction | AfterDeleteInTransaction,
//		AllBeforeOutTransaction = BeforeOutsertOutTransaction | 
//			BeforeUpdateOutTransaction | BeforeDeleteOutTransaction,
//		AllAfterOutTransaction = AfterOutsertOutTransaction | 
//			AfterUpdateOutTransaction | AfterDeleteOutTransaction,
//
//		AllInTranaction = AllBeforeInTransaction | AllBeforeOutTransaction,
//		AllOutTranaction = AllBeforeOutTransaction | AllBeforeOutTransaction,
//
//		All = AllInTranaction | AllOutTranaction,

		BeforeInsert = 1,
		BeforeUpdate = 2,
		BeforeDelete = 4,

		BeforeAll = BeforeInsert | BeforeUpdate | BeforeDelete,
	}
}
