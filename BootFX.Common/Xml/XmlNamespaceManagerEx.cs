// BootFX - Application framework for .NET applications
// 
// File: XmlNamespaceManagerEx.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using System.Xml;

namespace BootFX.Common.Xml
{
	/// <summary>
	/// Defines an instance of <c>XmlNamespaceManagerEx</c>.
	/// </summary>
	public class XmlNamespaceManagerEx
	{
		/// <summary>
		/// Private field to support <see cref="Prefixes"/> property.
		/// </summary>
		private Lookup _prefixes;
		
		/// <summary>
		/// Private field to support <see cref="Document"/> property.
		/// </summary>
		private XmlDocument _document;

		/// <summary>
		/// Private field to support <c>Manager</c> property.
		/// </summary>
		private XmlNamespaceManager _manager;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="document"></param>
		public XmlNamespaceManagerEx(XmlDocument document) : this(document, null)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="document"></param>
		public XmlNamespaceManagerEx(XmlDocument document, XmlNamespaceManager manager)
		{
			if(document == null)
				throw new ArgumentNullException("document");
			_document = document;

			// set...
			if(manager == null)
			{
				manager = XmlHelper.GetNamespaceManager(_document);
				if(manager == null)
					throw new InvalidOperationException("manager is null.");		
			}

			// set...
			_manager = manager;

			// lookup...
			_prefixes = new Lookup();
			_prefixes.CreateItemValue += new CreateLookupItemEventHandler(_prefixes_CreateItemValue);
		}

		/// <summary>
		/// Gets the document.
		/// </summary>
		private XmlDocument Document
		{
			get
			{
				return _document;
			}
		}

		/// <summary>
		/// Gets the manager.
		/// </summary>
		public XmlNamespaceManager Manager
		{
			get
			{
				// returns the value...
				return _manager;
			}
		}

		/// <summary>
		/// Gets the prefixes.
		/// </summary>
		private Lookup Prefixes
		{
			get
			{
				return _prefixes;
			}
		}

		/// <summary>
		/// Gets the prefix for the given namespace.
		/// </summary>
		/// <param name="ns"></param>
		/// <returns></returns>
		public string GetPrefix(string ns)
		{
			if(ns == null)
				throw new ArgumentNullException("ns");
			if(ns.Length == 0)
				throw new ArgumentOutOfRangeException("'ns' is zero-length.");

			// return...
			return (string)this.Prefixes[ns];
		}

		private void _prefixes_CreateItemValue(object sender, CreateLookupItemEventArgs e)
		{
			// get...
			string ns = (string)e.Key;

			// create...
			int index = 1;
			while(true)
			{
				string prefix = this.EncodePrefix(index);
				if(prefix == null)
					throw new InvalidOperationException("'prefix' is null.");
				if(prefix.Length == 0)
					throw new InvalidOperationException("'prefix' is zero-length.");

				// check...
				if(!(this.Prefixes.ContainsValue(prefix)))
				{
					e.NewValue = prefix;
					return;
				}

				// next...
				index++;
			}
		}

		private string EncodePrefix(int index)
		{
			return new string(new char[] { (char)(index + 64) });
		}

		/// <summary>
		/// Returns true if we have this namespace.
		/// </summary>
		/// <param name="prefix"></param>
		/// <returns></returns>
		public bool HasNamespace(string prefix)
		{
			if(prefix == null)
				throw new ArgumentNullException("prefix");
			if(prefix.Length == 0)
				throw new ArgumentOutOfRangeException("'prefix' is zero-length.");

			// nope...
			return this.Prefixes.ContainsValue(prefix);
		}

		[Obsolete("Use the override that has the 'addToManager' argument.")]
		public void AddNamespace(string prefix, string ns)
		{
			this.AddNamespace(prefix, ns, false);
		}

		// mbr - 12-09-2006 - added.
		public void AddNamespace(string prefix, string ns, bool addToManager)
		{
			if(prefix == null)
				throw new ArgumentNullException("prefix");
			if(ns == null)
				throw new ArgumentNullException("ns");
			if(ns.Length == 0)
				throw new ArgumentOutOfRangeException("'ns' is zero-length.");
			
			// set...
			this.Prefixes[ns] = prefix;

			// mbr - 12-09-2006 - added...
			if(addToManager)
			{
				if(Manager == null)
					throw new InvalidOperationException("Manager is null.");
				this.Manager.AddNamespace(prefix, ns);
			}
		}

		public string GetNamespace(string prefix, bool throwIfNotFound)
		{
			if(prefix == null)
				throw new ArgumentNullException("prefix");
			if(prefix.Length == 0)
				throw new ArgumentOutOfRangeException("'prefix' is zero-length.");

			// do we?
			foreach(DictionaryEntry entry in this.Prefixes)
			{
				if((string)entry.Value == prefix)
					return (string)entry.Key;
			}			

			// throw...
			if(throwIfNotFound)
				throw new InvalidCastException(string.Format("Namespace for prefix '{0}' was not found.", prefix));
			else
				return null;
		}

		/// <summary>
		/// Splits a full name into its constituent parts.
		/// </summary>
		/// <param name="fullName"></param>
		/// <param name="prefix"></param>
		/// <param name="namespaceUri"></param>
		/// <param name="localName"></param>
		public void SplitFullName(string fullName, ref string namespaceUri, ref string localName)
		{
			string prefix = null;
			this.SplitFullName(fullName, ref prefix, ref namespaceUri, ref localName);
		}

		/// <summary>
		/// Splits a full name into its constituent parts.
		/// </summary>
		/// <param name="fullName"></param>
		/// <param name="prefix"></param>
		/// <param name="namespaceUri"></param>
		/// <param name="localName"></param>
		public void SplitFullName(string fullName, ref string prefix, ref string namespaceUri, ref string localName)
		{
			if(fullName == null)
				throw new ArgumentNullException("fullName");
			if(fullName.Length == 0)
				throw new ArgumentOutOfRangeException("'fullName' is zero-length.");
			
			// reset...
			prefix = null;
			namespaceUri = null;
			localName = null;

			// walk...
			for(int index = fullName.Length - 1; index >= 0; index--)
			{
				if(!(char.IsLetterOrDigit(fullName[index])))
				{
					// split...
					namespaceUri = fullName.Substring(0, index + 1);
					localName = fullName.Substring(index + 1);
					prefix = this.GetPrefix(namespaceUri);

					// return...
					return;
				}
			}

			// nope...
			localName = fullName;
		}

		/// <summary>
		/// Infers namespaces for a given node.
		/// </summary>
		/// <param name="node"></param>
		public void InferNamespaces(XmlNode node)
		{
			if(node == null)
				throw new ArgumentNullException("node");
			
			// walk...
			XmlHelper.WalkAndPopulatePrefixes(node, this.Manager, this);
		}

		public void AddDataTypeNamespace()
		{
			this.AddNamespace(XmlHelper.DataTypesPrefix, XmlHelper.DataTypesNamespaceUri, true);
		}
	}
}
