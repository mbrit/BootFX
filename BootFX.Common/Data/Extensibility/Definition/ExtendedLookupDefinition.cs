// BootFX - Application framework for .NET applications
// 
// File: ExtendedLookupDefinition.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Xml;
using BootFX.Common.Xml;
using BootFX.Common.Entities;

namespace BootFX.Common.Entities
{
	/// <summary>
	/// Summary description for ExtendedLookup.
	/// </summary>
	public sealed class ExtendedLookupDefinition : ToXmlBase
	{
		/// <summary>
		/// Private field to support <see cref="Name"/> property.
		/// </summary>
		private string _name;

		public ExtendedLookupDefinition(string name)
		{
			_name = name;
		}

		/// <summary>
		/// Gets the Name.
		/// </summary>
		public string Name
		{
			get
			{
				return _name;
			}
			set
			{
				if(_name != value)
					_name = value;
			}
		}

		public override string ToString()
		{
			return string.Format("{0} (Lookup)",Name);
		}

		protected override string DefaultElementName
		{
			get
			{
				return "Lookup";
			}
		}

		protected override void WriteXml(System.Xml.XmlWriter xml, WriteXmlContext context)
		{
			if(xml == null)
				throw new ArgumentNullException("xml");
			if(context == null)
				throw new ArgumentNullException("context");
			
			// output...
			xml.WriteElementString("Name", this.Name);
		}

		public static ExtendedLookupDefinition FromXml(XmlElement node)
		{
			if(node == null)
				throw new ArgumentNullException("node");
			
			// get the name...
			string name = XmlHelper.GetElementString(node, "Name", OnNotFound.ThrowException);
			if(name == null)
				throw new InvalidOperationException("'name' is null.");
			if(name.Length == 0)
				throw new InvalidOperationException("'name' is zero-length.");

			// create...
			return new ExtendedLookupDefinition(name);
		}
	}
}
