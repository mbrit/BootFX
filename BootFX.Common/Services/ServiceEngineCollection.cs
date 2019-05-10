// BootFX - Application framework for .NET applications
// 
// File: ServiceEngineCollection.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using BootFX.Common.Management;
using System;
using System.Collections;
using System.Collections.Generic;

namespace BootFX.Common.Services
{
    /// <summary>
    /// Holds a collection of <see ref="ServiceEngine">ServiceEngine</see> instances.
    /// </summary>
    public class ServiceEngineCollection : IEnumerable<ServiceEngine> // CollectionBase
    {
        private List<ServiceEngine> InnerList { get; set; }
        private ServiceHost Owner { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        internal ServiceEngineCollection(ServiceHost owner = null)
        {
            this.Owner = owner;
            this.InnerList = new List<ServiceEngine>();
        }

        public void Add(ServiceEngine engine)
        {
            if (this.Owner == null || this.Owner.IsEnabled(engine))
                this.InnerList.Add(engine);
        }

        public int Count => this.InnerList.Count;

        public ServiceEngine this[int index]
        {
            get
            {
                return this.InnerList[index];
            }
        }

        public void Clear()
        {
            this.InnerList.Clear();
        }

        ///// <summary>
        ///// Adds a ServiceEngine instance to the collection.
        ///// </summary>
        ///// <param name="item">The item to add.</param>
        //public void Add(ServiceEngine item)
        //{
        //    if(item == null)
        //        throw new ArgumentNullException("item");
        //    List.Add(item);
        //}  

        ///// <summary>
        ///// Adds a set of ServiceEngine instances to the collection.
        ///// </summary>
        ///// <param name="item">The item to add.</param>
        //public void AddRange(ServiceEngine[] items)
        //{
        //    if(items == null)
        //        throw new ArgumentNullException("items");
        //    for(int index = 0; index < items.Length; index++)
        //        Add(items[index]);
        //}  
	
        ///// <summary>
        ///// Adds a set of ServiceEngine instances to the collection.
        ///// </summary>
        ///// <param name="item">The item to add.</param>
        //public void AddRange(ServiceEngineCollection items)
        //{
        //    if(items == null)
        //        throw new ArgumentNullException("items");
        //    for(int index = 0; index < items.Count; index++)
        //        Add(items[index]);
        //}  
		
        ///// <summary>
        ///// Inserts a ServiceEngine instance into the collection.
        ///// </summary>
        ///// <param name="item">The item to add.</param>
        //public void Insert(int index, ServiceEngine item)
        //{
        //    if(item == null)
        //        throw new ArgumentNullException("item");
        //    List.Insert(index, item);
        //}  
	
        ///// <summary>
        ///// Removes a ServiceEngine item to the collection.
        ///// </summary>
        ///// <param name="item">The item to remove.</param>
        //public void Remove(ServiceEngine item)
        //{
        //    if(item == null)
        //        throw new ArgumentNullException("item");
        //    List.Remove(item);
        //}  
		
        ///// <summary>
        ///// Gets or sets an item.
        ///// </summary>
        ///// <param name="index">The index in the collection.</param>
        //public ServiceEngine this[int index]
        //{
        //    get
        //    {
        //        return (ServiceEngine)List[index];
        //    }
        //    set
        //    {
        //        if(value == null)
        //            throw new ArgumentNullException("value");
        //        List[index] = value;
        //    }
        //}
		
        ///// <summary>
        ///// Returns the index of the item in the collection.
        ///// </summary>
        ///// <param name="item">The item to find.</param>
        ///// <returns>The index of the item, or -1 if it is not found.</returns>
        //public int IndexOf(ServiceEngine item)
        //{
        //    return List.IndexOf(item);
        //}
		
        ///// <summary>
        ///// Discovers if the given item is in the collection.
        ///// </summary>
        ///// <param name="item">The item to find.</param>
        ///// <returns>Returns true if the given item is in the collection.</returns>
        //public bool Contains(ServiceEngine item)
        //{
        //    if(IndexOf(item) == -1)
        //        return false;
        //    else
        //        return true;
        //}
		
        ///// <summary>
        ///// Copies the entire collection to an array.
        ///// </summary>
        ///// <returns>Returns the array of items.</returns>
        //public ServiceEngine[] ToArray()
        //{
        //    return (ServiceEngine[])InnerList.ToArray(typeof(ServiceEngine));
        //}

        ///// <summary>
        ///// Copies the entire collection to an array.
        ///// </summary>
        ///// <returns>Returns the array of items.</returns>
        //public void CopyTo(ServiceEngine[] items, int index)
        //{
        //    if(items == null)
        //        throw new ArgumentNullException("items");
        //    InnerList.CopyTo(items, index);
        //}

        public int IndexOf(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            for (int index = 0; index < this.Count; index++)
            {
                if (type.IsAssignableFrom(this[index].GetType()))
                    return index;
            }
            return -1;
        }

        //public ServiceEngine this[Type type]
        //{
        //    get
        //    {
        //        return this.GetEngine(type, false);
        //    }
        //}

		public ServiceEngine GetEngine(Type type, bool throwIfNotFound)
		{
			if(type == null)
				throw new ArgumentNullException("type");
			int index = this.IndexOf(type);
			if(index != -1)
				return this[index];
			else
			{
				if(throwIfNotFound)
					throw new InvalidOperationException(string.Format("An engine of type '{0}' was not found.", type));
				else
					return null;
			}
		}

        public IEnumerator<ServiceEngine> GetEnumerator()
        {
            return this.InnerList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
