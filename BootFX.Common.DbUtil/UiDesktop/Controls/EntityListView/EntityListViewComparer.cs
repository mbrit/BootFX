// BootFX - Application framework for .NET applications
// 
// File: EntityListViewComparer.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Collections;
using System.ComponentModel;
using BootFX.Common.Data;
using BootFX.Common.Entities;

namespace BootFX.Common.UI.Desktop
{
	/// <summary>
	/// Defines the comparer used for <see cref="EntityListView"></see>.
	/// </summary>
	/// <remarks>This class uses a strategy pattern by passing off the actual comparison to the comparer stored in <see cref="InnerComparer"></see>.</remarks>
	internal class EntityListViewComparer : IComparer
	{
		/// <summary>
		/// Private field to support <c>Header</c> property.
		/// </summary>
		private EntityListViewColumnHeader _header;
				
		/// <summary>
		/// Private field to support <c>InnerComparer</c> property.
		/// </summary>
		private IComparer _innerComparer;
		
		/// <summary>
		/// Constructor.
		/// </summary>
		public EntityListViewComparer(EntityListViewColumnHeader header, IComparer innerComparer, SortDirection direction)
		{
			if(header == null)
				throw new ArgumentNullException("header");
			if(innerComparer == null)
				throw new ArgumentNullException("innerComparer");

			_header = header;
			_innerComparer = innerComparer;

            // mbr - 2010-04-05 - removed direction...
            //// set...
            //if(this.InnerComparer is IComparerDirection)
            //    ((IComparerDirection)this.InnerComparer).Direction = direction;
		}

		/// <summary>
		/// Gets the header.
		/// </summary>
		public EntityListViewColumnHeader Header
		{
			get
			{
				return _header;
			}
		}

		/// <summary>
		/// Gets the innercomparer.
		/// </summary>
		public IComparer InnerComparer
		{
			get
			{
				return _innerComparer;
			}
		}

		/// <summary>
		/// Compares the two list view items.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public int Compare(object x, object y)
		{
			if(x == null)
				throw new ArgumentNullException("x");
			if(y == null)
				throw new ArgumentNullException("y");

			// check...
			if(InnerComparer == null)
				throw new ArgumentNullException("InnerComparer");
			
			// get the values...
			object entityX = ((EntityListViewItem)x).Entity;
			object entityY = ((EntityListViewItem)y).Entity;

			// get the value...
			if(this.InnerComparer is EntityMemberComparer && ((EntityMemberComparer)this.InnerComparer).Member is EntityField)
				return this.InnerComparer.Compare(entityX, entityY);

			// get...
			if(Property == null)
				throw new ArgumentNullException("Propert");

			// get the value from the entities...
			object valueX = this.Property.GetValue(entityX);
			object valueY = this.Property.GetValue(entityY);

			// comparer...
			return this.InnerComparer.Compare(valueX, valueY);
		}

		/// <summary>
		/// Gets the property.
		/// </summary>
		private PropertyDescriptor Property
		{
			get
			{
				if(Header == null)
					throw new ArgumentNullException("Header");
				return this.Header.Property;
			}
		}
	}
}
