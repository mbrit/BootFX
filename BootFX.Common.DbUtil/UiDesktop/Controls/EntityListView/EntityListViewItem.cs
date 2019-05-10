// BootFX - Application framework for .NET applications
// 
// File: EntityListViewItem.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Windows.Forms;
using System.ComponentModel;
using BootFX.Common.Entities;

namespace BootFX.Common.UI.Desktop
{
	/// <summary>
	/// Describes an item containing an entity on an <see cref="EntityListView"></see>.
	/// </summary>
	internal class EntityListViewItem : ListViewItem
	{
		/// <summary>
		/// Private field to support <c>EntityListView</c> property.
		/// </summary>
		private EntityListView _entityListView;

		/// <summary>
		/// Private field to support <c>Entity</c> property.
		/// </summary>
		private object _entity;
		
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="entityListView"></param>
		/// <param name="entity"></param>
		public EntityListViewItem(EntityListView entityListView, object entity)
		{
			if(entityListView == null)
				throw new ArgumentNullException("entityListView");
			if(entity == null)
				throw new ArgumentNullException("entity");
			
			// set...
			_entityListView = entityListView;
			_entity = entity;

			// refresh...
			RefreshView();
		}

		/// <summary>
		/// Gets the entity.
		/// </summary>
		public object Entity
		{
			get
			{
				return EntityView.Unwrap(_entity);
			}
		}
		
		/// <summary>
		/// Gets the entitylistview.
		/// </summary>
		private EntityListView EntityListView
		{
			get
			{
				if(this.ListView == null)
					return _entityListView;
				else
					return this.ListView as EntityListView;
			}
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		internal void RefreshView()
		{
			// check...
			if(EntityListView == null)
				throw new ArgumentNullException("EntityListView");
			if(Entity == null)
				throw new ArgumentNullException("Entity");

			// walk the columns...
			this.SubItems.Clear();
			for(int index = 0; index < this.EntityListView.Columns.Count; index++)
			{
				object value = null;

				// header...
				ColumnHeader header = this.EntityListView.Columns[index];
				EntityListViewColumnHeader entityHeader = null;
				if(header is EntityListViewColumnHeader)
					entityHeader = (EntityListViewColumnHeader)header;

				// check...
				if(entityHeader == null)
					throw new ArgumentNullException("entityHeader");
				if(entityHeader.Property == null)
					throw new ArgumentNullException("entityHeader.Property");

				// get the value...
				value = entityHeader.Property.GetValue(this.Entity);

				// get a converter...
				TypeConverter converter = null;
				if(entityHeader.Property is EntityFieldPropertyDescriptor)
					converter = ((EntityFieldPropertyDescriptor)entityHeader.Property).Field.GetConverter();

				// convert...
				string valueAsString = null;
				if(converter != null)
					valueAsString = converter.ConvertToString(null, Cultures.User, value);
				else
					valueAsString = Convert.ToString(value, Cultures.User);

				// trim...
				if(valueAsString == null)
					valueAsString = string.Empty;
				else
					valueAsString = valueAsString.TrimEnd();

				// show...
				if(index == 0)
					this.Text = valueAsString;
				else
					this.SubItems.Add(valueAsString);
			}
		}
	}
}

