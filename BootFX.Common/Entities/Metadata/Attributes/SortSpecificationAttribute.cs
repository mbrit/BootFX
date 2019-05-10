// BootFX - Application framework for .NET applications
// 
// File: SortSpecificationAttribute.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using BootFX.Common.Data;

namespace BootFX.Common.Entities.Attributes
{
	/// <summary>
	/// Defines sort specifications.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class SortSpecificationAttribute : Attribute
	{
		/// <summary>
		/// Private field to support <c>FieldNames</c> property.
		/// </summary>
		private string[] _fieldNames;

		/// <summary>
		/// Private field to support <c>Directions</c> property.
		/// </summary>
		private SortDirection[] _directions;

		/// <summary>
		/// Constructor.
		/// </summary>
		public SortSpecificationAttribute(string fieldName, SortDirection direction) : this(new string[] { fieldName }, new SortDirection[] { direction })
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		public SortSpecificationAttribute(string[] fieldNames, SortDirection[] directions)
		{
			if(fieldNames == null)
				throw new ArgumentNullException("fieldNames");
			if(directions == null)
				throw new ArgumentNullException("directions");
			
			// set...
			_fieldNames = fieldNames;
			_directions = directions;
		}

		/// <summary>
		/// Gets the directions.
		/// </summary>
		private SortDirection[] Directions
		{
			get
			{
				return _directions;
			}
		}
		
		/// <summary>
		/// Gets the fieldnames.
		/// </summary>
		private string[] FieldNames
		{
			get
			{
				return _fieldNames;
			}
		}

		/// <summary>
		/// Gets the sort specification for this attribute.
		/// </summary>
		/// <returns></returns>
		public SortSpecificationCollection GetSortSpecification(EntityType entityType)
		{
			if(entityType == null)
				throw new ArgumentNullException("entityType");
			
			if(FieldNames == null)
				throw new ArgumentNullException("FieldNames");
			if(Directions == null)
				throw new ArgumentNullException("Directions");
			if(FieldNames.Length != Directions.Length)
				throw ExceptionHelper.CreateLengthMismatchException("FieldNames", "Directions", FieldNames.Length, Directions.Length);

			// create...
			SortSpecificationCollection specifications = new SortSpecificationCollection();
			for(int index = 0; index < this.FieldNames.Length; index++)
			{
				string fieldName = FieldNames[index];

				// get and add...
				EntityField field = entityType.Fields.GetField(fieldName, OnNotFound.ThrowException);
				specifications.Add(new SortSpecification(field, this.Directions[index]));
			}

			// return...
			return specifications;
		}
	}
}
