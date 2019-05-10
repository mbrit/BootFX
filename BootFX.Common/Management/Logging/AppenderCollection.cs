// BootFX - Application framework for .NET applications
// 
// File: AppenderCollection.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Collections;
using System.Collections.Generic;

namespace BootFX.Common.Management
{
	/// <summary>
	/// Holds a collection of <see ref="Appender">Appender</see> instances.
	/// </summary>
	public class AppenderCollection : List<Appender>
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		public AppenderCollection()
		{
		}
		
        ///// <summary>
        ///// Adds a Appender instance to the collection.
        ///// </summary>
        ///// <param name="item">The item to add.</param>
        //public void Add(Appender item)
        //{
        //    if(item == null)
        //        throw new ArgumentNullException("item");
        //    List.Add(item);
        //}  

        ///// <summary>
        ///// Adds a set of Appender instances to the collection.
        ///// </summary>
        ///// <param name="item">The item to add.</param>
        //public void AddRange(Appender[] items)
        //{
        //    if(items == null)
        //        throw new ArgumentNullException("items");
        //    for(int index = 0; index < items.Length; index++)
        //        Add(items[index]);
        //}  
	
        ///// <summary>
        ///// Adds a set of Appender instances to the collection.
        ///// </summary>
        ///// <param name="item">The item to add.</param>
        //public void AddRange(AppenderCollection items)
        //{
        //    if(items == null)
        //        throw new ArgumentNullException("items");
        //    for(int index = 0; index < items.Count; index++)
        //        Add(items[index]);
        //}  
		
        ///// <summary>
        ///// Inserts a Appender instance into the collection.
        ///// </summary>
        ///// <param name="item">The item to add.</param>
        //public void Insert(int index, Appender item)
        //{
        //    if(item == null)
        //        throw new ArgumentNullException("item");
        //    List.Insert(index, item);
        //}  
	
        ///// <summary>
        ///// Removes a Appender item to the collection.
        ///// </summary>
        ///// <param name="item">The item to remove.</param>
        //public void Remove(Appender item)
        //{
        //    if(item == null)
        //        throw new ArgumentNullException("item");
        //    List.Remove(item);
        //}  
		
        ///// <summary>
        ///// Gets or sets an item.
        ///// </summary>
        ///// <param name="index">The index in the collection.</param>
        //public Appender this[int index]
        //{
        //    get
        //    {
        //        return (Appender)List[index];
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
        //public int IndexOf(Appender item)
        //{
        //    return List.IndexOf(item);
        //}
		
        ///// <summary>
        ///// Discovers if the given item is in the collection.
        ///// </summary>
        ///// <param name="item">The item to find.</param>
        ///// <returns>Returns true if the given item is in the collection.</returns>
        //public bool Contains(Appender item)
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
        //public Appender[] ToArray()
        //{
        //    return (Appender[])InnerList.ToArray(typeof(Appender));
        //}

        ///// <summary>
        ///// Copies the entire collection to an array.
        ///// </summary>
        ///// <returns>Returns the array of items.</returns>
        //public void CopyTo(Appender[] items, int index)
        //{
        //    if(items == null)
        //        throw new ArgumentNullException("items");
        //    InnerList.CopyTo(items, index);
        //}
	}
}
