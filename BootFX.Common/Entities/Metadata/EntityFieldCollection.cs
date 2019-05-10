// BootFX - Application framework for .NET applications
// 
// File: EntityFieldCollection.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Collections;

namespace BootFX.Common.Entities
{
	/// <summary>
	/// Holds a collection of <c ref="EntityField">EntityField</c> instances.
	/// </summary>
	public class EntityFieldCollection : CollectionBase, IEntityType
	{
		/// <summary>
		/// Private field to support <see cref="ByName"/> property.
		/// </summary>
		private Lookup _byName;
		
		/// <summary>
		/// Raised when a field is added.
		/// </summary>
		public event EntityFieldEventHandler FieldAdded;
		
		/// <summary>
		/// Private field to support <c>EntityType</c> property.
		/// </summary>
		private EntityType _entityType;
		
		/// <summary>
		/// Constructor.
		/// </summary>
		internal EntityFieldCollection(EntityType entityType)
		{
			if(entityType == null)
				throw new ArgumentNullException("entityType");
			_entityType = entityType;

			// mbr - 03-08-2008 - lookup...
			_byName = new Lookup(false);
			_byName.CreateItemValue += new CreateLookupItemEventHandler(_byName_CreateItemValue);
		}

		/// <summary>
		/// Gets the entitytype.
		/// </summary>
		public EntityType EntityType
		{
			get
			{
				return _entityType;
			}
		}
		
		/// <summary>
		/// Adds a EntityField instance to the collection.
		/// </summary>
		/// <param name="item">The item to add.</param>
		public int Add(EntityField item)
		{
			if(item == null)
				throw new ArgumentNullException("item");
			int index = List.Add(item);
			this.OnFieldAdded(new EntityFieldEventArgs(item));
			return index;
		}  

		/// <summary>
		/// Raises the <c>FieldAdded</c> event.
		/// </summary>
		protected virtual void OnFieldAdded(EntityFieldEventArgs e)
		{
			if(FieldAdded != null)
				FieldAdded(this, e);
		}

		/// <summary>
		/// Adds a EntityField instance to the collection.
		/// </summary>
		/// <param name="item">The item to add.</param>
		public void Insert(int index, EntityField item)
		{
			if(item == null)
				throw new ArgumentNullException("item");
			List.Insert(index, item);
			item.SetOrdinal(index);
		}  

		/// <summary>
		/// Adds a EntityField instance to the collection.
		/// </summary>
		/// <param name="item">The item to add.</param>
		public void AddRange(EntityFieldCollection items)
		{
			this.AddRangeInternal(items);
		}

		/// <summary>
		/// Adds a EntityField instance to the collection.
		/// </summary>
		/// <param name="item">The item to add.</param>
		public void AddRange(EntityField[] items)
		{
			this.AddRangeInternal(items);
		}

		private void AddRangeInternal(IEnumerable items)
		{
			if(items == null)
				throw new ArgumentNullException("items");
			foreach(EntityField item in items)
				this.Add(item);
		}  

		/// <summary>
		/// Removes a EntityField item to the collection.
		/// </summary>
		/// <param name="item">The item to remove.</param>
		public void Remove(EntityField item)
		{
			if(item == null)
				throw new ArgumentNullException("item");
			int index = IndexOf(item.Name);
			List.RemoveAt(IndexOf(item.Name));
		}  
		
		/// <summary>
		/// Gets or sets an item.
		/// </summary>
		/// <param name="index">The index in the collection.</param>
		public EntityField this[int index]
		{
			get
			{
				return (EntityField)List[index];
			}
			set
			{
				if(value == null)
					throw new ArgumentNullException("value");
				List[index] = value;
			}
		}

		/// <summary>
		/// Gets or sets an item.
		/// </summary>
		/// <param name="index">The index in the collection.</param>
		public EntityField this[string name]
		{
			get
			{
				if(name == null)
					throw new ArgumentNullException("name");
				if(name.Length == 0)
					throw ExceptionHelper.CreateZeroLengthArgumentException("name");
				
				// return...
				return this.GetField(name, OnNotFound.ReturnNull);
			}
		}
		
		/// <summary>
		/// Returns the index of the item in the collection.
		/// </summary>
		/// <param name="item">The item to find.</param>
		/// <returns>The index of the item, or -1 if it is not found.</returns>
		public int IndexOf(EntityField item)
		{
			return List.IndexOf(item);
		}
		
		/// <summary>
		/// Discovers if the given item is in the collection.
		/// </summary>
		/// <param name="item">The item to find.</param>
		/// <returns>Returns true if the given item is in the collection.</returns>
		public bool Contains(EntityField item)
		{
			if(IndexOf(item) == -1)
				return false;
			else
				return true;
		}

		/// <summary>
		/// Discovers if the given field name is in the collection.
		/// </summary>
		/// <param name="name">The field name to find.</param>
		/// <returns>Returns true if the given item is in the collection.</returns>
		public bool Contains(string name)
		{
			if(IndexOf(name) == -1)
				return false;
			else
				return true;
		}
		
		/// <summary>
		/// Copies the entire collection to an array.
		/// </summary>
		/// <returns>Returns the array of items.</returns>
		public EntityField[] ToArray()
		{
			return (EntityField[])InnerList.ToArray(typeof(EntityField));
		}

		/// <summary>
		/// Gets the 
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public int IndexOf(string name)
		{
			if(name == null)
				throw new ArgumentNullException("name");
			if(name.Length == 0)
				throw ExceptionHelper.CreateZeroLengthArgumentException("name");
			
			// mbr - 03-08-2008 - changed to lookup...
//			for(int index = 0; index < this.Count; index++)
//			{
//				if(string.Compare(this[index].Name, name, true, System.Globalization.CultureInfo.InvariantCulture) == 0)
//					return index;
//			}
//
//			// nope...
//			return -1;

			// return...
			return (int)this.ByName[name];
		}

		private void _byName_CreateItemValue(object sender, CreateLookupItemEventArgs e)
		{
			// name...
			string name = (string)e.Key;

			// walk...
			for(int index = 0; index < this.Count; index++)
			{
				if(string.Compare(this[index].Name, name, true, System.Globalization.CultureInfo.InvariantCulture) == 0)
				{
					e.NewValue = index;
					return;
				}
			}

			// nope...
			e.NewValue = -1;
		}

		/// <summary>
		/// Gets the byname.
		/// </summary>
		private Lookup ByName
		{
			get
			{
				return _byName;
			}
		}

		/// <summary>
		/// Gets the entity type for the given type.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="onNotFound"></param>
		/// <returns></returns>
		public EntityField GetField(string name, OnNotFound onNotFound = OnNotFound.ThrowException)
		{
			if(name == null)
				throw new ArgumentNullException("name");
			if(name.Length == 0)
				throw ExceptionHelper.CreateZeroLengthArgumentException("name");
			
			// check...
			if(EntityType == null)
				throw new ArgumentNullException("EntityType");

			// index...
			int index = this.IndexOf(name);
			if(index != -1)
				return this[index];
			else
			{
				switch(onNotFound)
				{
					case OnNotFound.ReturnNull:
						return null;

					case OnNotFound.ThrowException:
						throw new InvalidOperationException(string.Format(Cultures.Exceptions, "Field with name '{0}' was not found on '{1}'.", name, this.EntityType));

					default:
						throw ExceptionHelper.CreateCannotHandleException(onNotFound);
				}
			}
		}

		/// <summary>
		/// Copies the values in this collection to the given array.
		/// </summary>
		/// <param name="items"></param>
		/// <param name="index"></param>
		public void CopyTo(EntityField[] items, int index)
		{
			if(items == null)
				throw new ArgumentNullException("items");
			this.InnerList.CopyTo(items, index);
		}

		/// <summary>
		/// Gets an array of fields by name.
		/// </summary>
		/// <param name="names"></param>
		/// <param name="onNotFound"></param>
		/// <returns></returns>
		internal EntityField[] GetFields(string[] names, OnNotFound onNotFound)
		{
			if(names == null)
				throw new ArgumentNullException("names");
			
			// loop...
			EntityField[] fields = new EntityField[names.Length];
			for(int index = 0; index < names.Length; index++)
				fields[index] = this.GetField(names[index], onNotFound);

			// return...
			return fields;
		}

        public EntityField GetByNativeName(string name, OnNotFound onNotFound)
        {
            foreach (EntityField field in this.InnerList)
            {
                if (string.Compare(field.NativeName.Name, name, true, Cultures.System) == 0)
                    return field;
            }

            // if...
            if (onNotFound == OnNotFound.ThrowException)
                throw new InvalidOperationException(string.Format("A field with native name '{0}' was not found.", name));
            else if (onNotFound == OnNotFound.ReturnNull)
                return null;
            else
                throw new NotSupportedException(string.Format("Cannot handle '{0}'.", onNotFound));
        }
	}
}
