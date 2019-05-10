// BootFX - Application framework for .NET applications
// 
// File: EntityGenerationContext.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using BootFX.Common;
using BootFX.Common.Management;

namespace BootFX.Common.CodeGeneration
{
	/// <summary>
	/// Defines a class that 
	/// </summary>
	public class EntityGenerationContext : OperationContext
	{
		/// <summary>
		/// Private field to support <see cref="generator"/> property.
		/// </summary>
		private EntityGenerator _generator;
		
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="log"></param>
		/// <param name="innerOperation"></param>
		public EntityGenerationContext(EntityGenerator generator, ILog log, IOperationItem innerOperation)
			: base(log, innerOperation)
		{
			if(generator == null)
				throw new ArgumentNullException("generator");
			_generator = generator;
		}

		/// <summary>
		/// Gets the generator.
		/// </summary>
		public EntityGenerator Generator
		{
			get
			{
				return _generator;
			}
		}
	}
}
