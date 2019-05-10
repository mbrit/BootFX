// BootFX - Application framework for .NET applications
// 
// File: RssItemCollection.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Collections;
using System.Collections.Generic;

namespace BootFX.Common.Data.Rss
{
	/// <summary>
	/// Holds a collection of <see ref="RssItem">RssItem</see> instances.
	/// </summary>
	public class RssItemCollection : List<RssItem> // CollectionBase
	{
		///// <summary>
		///// Constructor.
		///// </summary>
		//public RssItemCollection()
		//{
		//}
		
		///// <summary>
		///// Adds a RssItem instance to the collection.
		///// </summary>
		///// <param name="item">The item to add.</param>
		//public void Add(RssItem item)
		//{
		//	if(item == null)
		//		throw new ArgumentNullException("item");
		//	List.Add(item);
		//}  

		///// <summary>
		///// Adds a set of RssItem instances to the collection.
		///// </summary>
		///// <param name="item">The item to add.</param>
		//public void AddRange(RssItem[] items)
		//{
		//	if(items == null)
		//		throw new ArgumentNullException("items");
		//	for(int index = 0; index < items.Length; index++)
		//		Add(items[index]);
		//}  
	
		///// <summary>
		///// Adds a set of RssItem instances to the collection.
		///// </summary>
		///// <param name="item">The item to add.</param>
		//public void AddRange(RssItemCollection items)
		//{
		//	if(items == null)
		//		throw new ArgumentNullException("items");
		//	for(int index = 0; index < items.Count; index++)
		//		Add(items[index]);
		//}  
		
		///// <summary>
		///// Inserts a RssItem instance into the collection.
		///// </summary>
		///// <param name="item">The item to add.</param>
		//public void Insert(int index, RssItem item)
		//{
		//	if(item == null)
		//		throw new ArgumentNullException("item");
		//	List.Insert(index, item);
		//}  
	
		///// <summary>
		///// Removes a RssItem item to the collection.
		///// </summary>
		///// <param name="item">The item to remove.</param>
		//public void Remove(RssItem item)
		//{
		//	if(item == null)
		//		throw new ArgumentNullException("item");
		//	List.Remove(item);
		//}  
		
		///// <summary>
		///// Gets or sets an item.
		///// </summary>
		///// <param name="index">The index in the collection.</param>
		//public RssItem this[int index]
		//{
		//	get
		//	{
		//		return (RssItem)List[index];
		//	}
		//	set
		//	{
		//		if(value == null)
		//			throw new ArgumentNullException("value");
		//		List[index] = value;
		//	}
		//}
		
		///// <summary>
		///// Returns the index of the item in the collection.
		///// </summary>
		///// <param name="item">The item to find.</param>
		///// <returns>The index of the item, or -1 if it is not found.</returns>
		//public int IndexOf(RssItem item)
		//{
		//	return List.IndexOf(item);
		//}
		
		///// <summary>
		///// Discovers if the given item is in the collection.
		///// </summary>
		///// <param name="item">The item to find.</param>
		///// <returns>Returns true if the given item is in the collection.</returns>
		//public bool Contains(RssItem item)
		//{
		//	if(IndexOf(item) == -1)
		//		return false;
		//	else
		//		return true;
		//}
		
		///// <summary>
		///// Copies the entire collection to an array.
		///// </summary>
		///// <returns>Returns the array of items.</returns>
		//public RssItem[] ToArray()
		//{
		//	return (RssItem[])InnerList.ToArray(typeof(RssItem));
		//}

		///// <summary>
		///// Copies the entire collection to an array.
		///// </summary>
		///// <returns>Returns the array of items.</returns>
		//public void CopyTo(RssItem[] items, int index)
		//{
		//	if(items == null)
		//		throw new ArgumentNullException("items");
		//	InnerList.CopyTo(items, index);
		//}
	}
}
