// BootFX - Application framework for .NET applications
// 
// File: TouchedValueWrapper.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Xml;
using System.Text;
using BootFX.Common.Xml;

namespace BootFX.Common.Data
{
	/// <summary>
	/// Summary description for TouchValueWrapper.
	/// </summary>
	public class TouchedValueWrapper
	{
		/// <summary>
		/// Private field to support <c>blocks</c> property.
		/// </summary>
		private TouchedValueCollection[] _blocks;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="blocks"></param>
		internal TouchedValueWrapper(TouchedValueCollection[] blocks)
		{
			if(blocks == null)
				throw new ArgumentNullException("blocks");
			_blocks = blocks;
		}

		/// <summary>
		/// Gets the blocks.
		/// </summary>
		private TouchedValueCollection[] Blocks
		{
			get
			{
				// returns the value...
				return _blocks;
			}
		}

		/// <summary>
		/// Gets the blocks.
		/// </summary>
		/// <returns></returns>
		public TouchedValueCollection[] GetBlocks()
		{
			return (TouchedValueCollection[])_blocks.Clone();
		}

		/// <summary>
		/// Gets all of the blocks as an XML document.
		/// </summary>
		/// <returns></returns>
		public XmlDocument GetBlocksAsXml()
		{
			XmlDocument doc = new XmlDocument();
			XmlNamespaceManagerEx manager = XmlHelper.GetNamespaceManagerEx(doc);

			// add...
			manager.AddDataTypeNamespace();

			// root...
			XmlElement root = doc.CreateElement("Blocks");
			doc.AppendChild(root);

			// walk...
			foreach(TouchedValueCollection block in this.Blocks)
			{
				XmlElement element = doc.CreateElement("Block");
				root.AppendChild(element);
				block.PopulateXmlElement(element, manager);
			}

			// return...
			return doc;
		}

		/// <summary>
		/// Gets all of the blocks as a plain-text representation of the changes.
		/// </summary>
		/// <returns></returns>
		public string GetBlocksAsPlainText()
		{
			StringBuilder builder = new StringBuilder();

			// walk...
			foreach(TouchedValueCollection block in this.Blocks)
				block.ToPlainText(builder);
			
			// return...
			return builder.ToString();
		}
	}
}
