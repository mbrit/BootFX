// BootFX - Application framework for .NET applications
// 
// File: GetSchemaArgs.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Collections;
using System.Collections.Specialized;
using BootFX.Common.Management;

namespace BootFX.Common.Data
{
	/// <summary>
	/// Arguments for a <c>GetSchema</c> call.
	/// </summary>
	public class GetSchemaArgs
	{
        public string DatabaseName { get; internal set; }

		/// <summary>
		/// Private field to support <see cref="ConstraintTableNames"/> property.
		/// </summary>
		private StringCollection _constrainTableNames = new StringCollection();
		private StringCollection _constaintSchemaNames = new StringCollection();

		private IOperationItem _operationItem;
		
		public GetSchemaArgs()
			: this(new OperationItem())
		{
		}

		public GetSchemaArgs(IOperationItem op)
		{
			if(op == null)
				op = new OperationItem();
			_operationItem = op;
		}

		internal IOperationItem OperationItem
		{
			get
			{
				return _operationItem;
			}
		}

		/// <summary>
		/// Gets the constrainttablenames.
		/// </summary>
		public StringCollection ConstrainTableNames
		{
			get
			{
				return _constrainTableNames;
			}
		}

		public StringCollection ConstrainSchemaNames
		{
			get
			{
				return _constaintSchemaNames;
			}
		}
	}
}
