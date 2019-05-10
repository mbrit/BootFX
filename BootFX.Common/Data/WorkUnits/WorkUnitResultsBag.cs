// BootFX - Application framework for .NET applications
// 
// File: WorkUnitResultsBag.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Collections;

namespace BootFX.Common.Data
{
	/// <summary>
	/// Defines a class that holds the results of work units.
	/// </summary>
	public class WorkUnitResultsBag : DictionaryBase
	{
		/// <summary>
		/// Defines the last created ID.
		/// </summary>
		private const string LastCreatedIdKey = "__reserved_LastCreatedId";

		/// <summary>
		/// Constructor.
		/// </summary>
		internal WorkUnitResultsBag()
		{
		}

		/// <summary>
		/// Gets or sets the last created ID.
		/// </summary>
		public object LastCreatedId
		{
			get
			{
				return this[LastCreatedIdKey];
			}
			set
			{
				this[LastCreatedIdKey] = value;
			}
		}

		/// <summary>
		/// Gets the last created ID as a 32-bit integer.
		/// </summary>
		public int LastCreatedIdAsInt32
		{
			get
			{
				return (int)this.GetLastCreatedId(typeof(int), 0);
			}
		}

		/// <summary>
		/// Gets the last created ID as a 32-bit integer.
		/// </summary>
		public long LastCreatedIdAsInt64
		{
			get
			{
				return (long)this.GetLastCreatedId(typeof(long), (long)0);
			}
		}

		/// <summary>
		/// Gets the last created ID as a 32-bit integer.
		/// </summary>
		public decimal LastCreatedIdAsDecimal
		{
			get
			{
				return (decimal)this.GetLastCreatedId(typeof(decimal), (decimal)0);
			}
		}

		/// <summary>
		/// Gets the last created ID.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		/// <remarks>This method relies on the ID being a single value.</remarks>
		private object GetLastCreatedId(Type type, object defaultValue)
		{
			if(type == null)
				throw new ArgumentNullException("type");

			// get the ID...
			object lastCreated = this.LastCreatedId;
			if(lastCreated != null)
			{
				// check...
				if(lastCreated is Array)
				{
					object[] lastCreatedAsArray = (object[])lastCreated;
					if(lastCreatedAsArray.Length == 1)
						return ConversionHelper.ChangeType(lastCreatedAsArray[0], type, null, ConversionFlags.None);
					else
						throw new InvalidOperationException(string.Format("Last created ID must be of length 1, but it is of length '{0}'.", lastCreatedAsArray.Length));
				}
				else
					throw new InvalidOperationException(string.Format("Last created ID is '{0}'. and is not an array.", lastCreated.GetType()));
			}
			else
				return defaultValue;
		}

		/// <summary>
		/// Gets or sets an item in the results bag.
		/// </summary>
		public object this[object key]
		{
			get
			{
				return this.InnerHashtable[key];
			}
			set
			{
				this.InnerHashtable[key] = value;
			}
		}
	}
}
