// BootFX - Application framework for .NET applications
// 
// File: SchemaWorkUnitTypeComparer.cs
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
	/// Defines an instance of <c>SchemaWorkUnitTypeComparer</c>.
	/// </summary>
	internal class SchemaWorkUnitTypeComparer : IComparer
	{
		internal SchemaWorkUnitTypeComparer()
		{
		}

		public int Compare(object x, object y)
		{
			int a = (int)this.GetSchemaWorkUnitType(x);
			int b = (int)this.GetSchemaWorkUnitType(y);

			// return...
			if(a < b)
				return -1;
			else if(a > b)
				return 1;
			else
				return 0;
		}

		private SchemaWorkUnitType GetSchemaWorkUnitType(object x)
		{
			if(x is AddColumnSchemaWorkUnit)
				return SchemaWorkUnitType.AddColumn;
			else if(x is AddConstraintSchemaWorkUnit)
				return SchemaWorkUnitType.AddConstraint;
			if(x is AlterColumnSchemaWorkUnit)
				return SchemaWorkUnitType.AlterColumn;
			if(x is CreateForeignKeySchemaWorkUnit)
				return SchemaWorkUnitType.CreateForeignKey;
			if(x is CreateIndexSchemaWorkUnit)
				return SchemaWorkUnitType.CreateIndex;
			if(x is CreateTableSchemaWorkUnit)
				return SchemaWorkUnitType.CreateTable;
			if(x is DropConstraintSchemaWorkUnit)
				return SchemaWorkUnitType.DropConstraint;
			if(x is DropForeignKeySchemaWorkUnit)
				return SchemaWorkUnitType.DropForeignKey;
			if(x is DropIndexSchemaWorkUnit)
				return SchemaWorkUnitType.DropIndex;

			// nope...
			return SchemaWorkUnitType.Unknown;
		}
	}
}
