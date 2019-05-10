// BootFX - Application framework for .NET applications
// 
// File: SortSpecification.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using BootFX.Common.Data;

namespace BootFX.Common.Entities
{
	/// <summary>
	///	 Defines a sort specification comprising a field and direction.
	/// </summary>
	public class SortSpecification : ICloneable
	{
		/// <summary>
		/// Private field to support <c>Field</c> property.
		/// </summary>
		private EntityField _field;

		/// <summary>
		/// Private field to support <c>Direction</c> property.
		/// </summary>
		private SortDirection _direction;

        public string FreeSort { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="field"></param>
        /// <param name="direction"></param>
        public SortSpecification(EntityField field, SortDirection direction)
		{
			if(field == null)
				throw new ArgumentNullException("field");
			
			_field = field;
			_direction = direction;
		}

        public SortSpecification(string freeSort, SortDirection direction)
        {
            this.FreeSort = freeSort;
            _direction = direction;
        }

        /// <summary>
        /// Gets the direction.
        /// </summary>
        public SortDirection Direction
		{
			get
			{
				return _direction;
			}
		}
		
		/// <summary>
		/// Gets the field.
		/// </summary>
		public EntityField Field
		{
			get
			{
				return _field;
			}
		}

        /// <summary>
        /// Copies the specification.
        /// </summary>
        /// <returns></returns>
        object ICloneable.Clone()
		{
			return this.Clone();
		}

		/// <summary>
		/// Copies the specification.
		/// </summary>
		/// <returns></returns>
		public SortSpecification Clone()
		{
			return new SortSpecification(this.Field, this.Direction);
		}
	}
}
