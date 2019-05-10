// BootFX - Application framework for .NET applications
// 
// File: ReloadSchemaOperation.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Xml;
using BootFX.Common.UI.Desktop;
using BootFX.Common.Management;

namespace BootFX.Common.DbUtil
{
	/// <summary>
	/// Summary description for RealoadSchemaOperation.
	/// </summary>
	internal class ReloadSchemaOperation : OperationDialogProcess
	{
		private Generator _generator;

		internal ReloadSchemaOperation(OperationContext context, Generator generator)
			: base(context)
		{
			_generator = generator;
		}

		protected override object DoRun()
		{
			this.Generator.DoReloadSchema(this.Context);

			// ok...
			return null;
		}

		private Generator Generator
		{
			get
			{
				return _generator;
			}
		}
	}
}
