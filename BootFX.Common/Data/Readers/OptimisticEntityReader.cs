// BootFX - Application framework for .NET applications
// 
// File: OptimisticEntityReader.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Data;
using System.Collections;
using BootFX.Common;
using BootFX.Common.Entities;

namespace BootFX.Common.Data
{
	/// <summary>
	/// Summary description for BufferedEntityReader.
	/// </summary>
	public abstract class OptimisticEntityReader : IEntityType
	{		
		/// <summary>
		/// Private field to support <see cref="mode"/> property.
		/// </summary>
		private OptimisticReadMode _mode;
		
		/// <summary>
		/// Private field to support <see cref="Current"/> property.
		/// </summary>
		private object _current;
		
		/// <summary>
		/// Private field to support <see cref="Source"/> property.
		/// </summary>
		private IEntitySource _source;
		
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="source"></param>
		protected OptimisticEntityReader(IEntitySource source, OptimisticReadMode mode)
		{
			if(source == null)
				throw new ArgumentNullException("source");

			// set...
			_source = source;
			_mode = mode;

			// init...
			this.Initialize();
		}

		/// <summary>
		/// Initializes the object.
		/// </summary>
		protected abstract void Initialize();

		/// <summary>
		/// Gets the source.
		/// </summary>
		internal IEntitySource Source
		{
			get
			{
				return _source;
			}
		}

		/// <summary>
		/// Gets the type of entities returned from the reader.
		/// </summary>
		public EntityType EntityType
		{
			get
			{
				if(Source == null)	
					throw new InvalidOperationException("Source is null.");
				return this.Source.EntityType;
			}
		}

		/// <summary>
		/// Reads the next item.
		/// </summary>
		/// <returns></returns>
		public abstract bool Read();

		/// <summary>
		/// Gets the current.
		/// </summary>
		public object Current
		{
			get
			{
				return _current;
			}
		}

		/// <summary>
		/// Sets the current item.
		/// </summary>
		/// <param name="current"></param>
		protected void SetCurrent(object current)
		{
			_current = current;
		}

		/// <summary>
		/// Gets the mode.
		/// </summary>
		protected OptimisticReadMode Mode
		{
			get
			{
				return _mode;
			}
		}
	}
}
