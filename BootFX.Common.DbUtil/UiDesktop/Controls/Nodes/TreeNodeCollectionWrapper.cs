// BootFX - Application framework for .NET applications
// 
// File: TreeNodeCollectionWrapper.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Collections;
using System.Windows.Forms;
using BootFX.Common.UI.Desktop;

namespace BootFX.Common.UI.Desktop
{
	/// <summary>
	/// Holds a collection of <see ref="TreeNode">TreeNode</see> instances.
	/// </summary>
	public class TreeNodeCollectionWrapper : IEnumerable
	{
		/// <summary>
		/// Private field to support <c>TreeView</c> property.
		/// </summary>
		private ObjectTreeView _treeView;
		
		/// <summary>
		/// Private field to support <see cref="InnerList"/> property.
		/// </summary>
		private TreeNodeCollection _innerList;

		/// <summary>
		/// Private field to support <see cref="OwnerNode"/> property.
		/// </summary>
		private ObjectTreeNode _ownerNode;
		
		/// <summary>
		/// Constructor.
		/// </summary>
		private TreeNodeCollectionWrapper(TreeNodeCollection innerList)
		{
			_innerList = innerList;			
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		internal TreeNodeCollectionWrapper(ObjectTreeNode ownerNode, TreeNodeCollection innerList) : this(innerList)
		{
			if(ownerNode == null)
				throw new ArgumentNullException("ownerNode");
			_ownerNode = ownerNode;
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		internal TreeNodeCollectionWrapper(ObjectTreeView treeView, TreeNodeCollection innerList) : this(innerList)
		{
			if(treeView == null)
				throw new ArgumentNullException("treeView");
			_treeView = treeView;
		}

		/// <summary>
		/// Gets the ownernode.
		/// </summary>
		private ObjectTreeNode OwnerNode
		{
			get
			{
				return _ownerNode;
			}
		}

		/// <summary>
		/// Gets the treeview.
		/// </summary>
		private ObjectTreeView TreeView
		{
			get
			{
				// returns the value...
				if(this.OwnerNode != null)
					return this.OwnerNode.ObjectTreeView;
				else
					return _treeView;
			}
		}

		/// <summary>
		/// Gets the innerlist.
		/// </summary>
		internal TreeNodeCollection InnerList
		{
			get
			{
				return _innerList;
			}
		}
		
		/// <summary>
		/// Adds a TreeNode instance to the collection.
		/// </summary>
		/// <param name="node">The node to add.</param>
		public int Add(TreeNode node)
		{
			if(node == null)
				throw new ArgumentNullException("node");
			int index = InnerList.Add(node);

			// notify...
			if(this.TreeView != null)
				this.TreeView.NodeAdded(node);

			// return...
			return index;
		}  

		/// <summary>
		/// Adds a set of TreeNode instances to the collection.
		/// </summary>
		/// <param name="node">The node to add.</param>
		public void AddRange(TreeNode[] items)
		{
			if(items == null)
				throw new ArgumentNullException("items");
			for(int index = 0; index < items.Length; index++)
				Add(items[index]);
		}  
	
		/// <summary>
		/// Adds a set of TreeNode instances to the collection.
		/// </summary>
		/// <param name="node">The node to add.</param>
		public void AddRange(TreeNodeCollection items)
		{
			if(items == null)
				throw new ArgumentNullException("items");
			for(int index = 0; index < items.Count; index++)
				Add(items[index]);
		}  
		
		/// <summary>
		/// Inserts a TreeNode instance into the collection.
		/// </summary>
		/// <param name="node">The node to add.</param>
		public void Insert(int index, TreeNode node)
		{
			if(node == null)
				throw new ArgumentNullException("node");
			InnerList.Insert(index, node);
		}  
	
		/// <summary>
		/// Removes a TreeNode node to the collection.
		/// </summary>
		/// <param name="node">The node to remove.</param>
		public void Remove(TreeNode node)
		{
			if(node == null)
				throw new ArgumentNullException("node");
			InnerList.Remove(node);
		}  
		
		/// <summary>
		/// Gets or sets an node.
		/// </summary>
		/// <param name="index">The index in the collection.</param>
		public TreeNode this[int index]
		{
			get
			{
				return (TreeNode)InnerList[index];
			}
			set
			{
				if(value == null)
					throw new ArgumentNullException("value");
				InnerList[index] = value;
			}
		}
		
		/// <summary>
		/// Returns the index of the node in the collection.
		/// </summary>
		/// <param name="node">The node to find.</param>
		/// <returns>The index of the node, or -1 if it is not found.</returns>
		public int IndexOf(TreeNode node)
		{
			return InnerList.IndexOf(node);
		}
		
		/// <summary>
		/// Discovers if the given node is in the collection.
		/// </summary>
		/// <param name="node">The node to find.</param>
		/// <returns>Returns true if the given node is in the collection.</returns>
		public bool Contains(TreeNode node)
		{
			if(IndexOf(node) == -1)
				return false;
			else
				return true;
		}
		
		/// <summary>
		/// Copies the entire collection to an array.
		/// </summary>
		/// <returns>Returns the array of items.</returns>
		public void CopyTo(TreeNode[] items, int index)
		{
			if(items == null)
				throw new ArgumentNullException("items");
			InnerList.CopyTo(items, index);
		}

		public IEnumerator GetEnumerator()
		{
			return this.InnerList.GetEnumerator();
		}

		public int Count
		{
			get
			{
				return this.InnerList.Count;
			}
		}

		public void Clear()
		{		
			this.InnerList.Clear();
		}
	}
}
