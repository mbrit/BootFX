// BootFX - Application framework for .NET applications
// 
// File: IToXml.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Xml;
using System.Text;

namespace BootFX.Common.Xml
{
	/// <summary>
	///	 Defines an interface that provides XML generation of an object.
	/// </summary>
	/// <seealso cref="XmlPersister"></seealso>
	public interface IToXml
	{
		/// <summary>
		/// Gets the default element name.
		/// </summary>
		string DefaultElementName
		{
			get;
		}

		/// <summary>
		/// Gets the XML representation of the object.
		/// </summary>
		/// <returns></returns>
		string ToXml();

		/// <summary>
		/// Gets the XML representation of the object.
		/// </summary>
		/// <returns></returns>
		string ToXml(string elementName);

		/// <summary>
		/// Gets the XML representation of the object.
		/// </summary>
		/// <returns></returns>
		string ToXml(Encoding encoding);

		/// <summary>
		/// Gets the XML representation of the object.
		/// </summary>
		/// <returns></returns>
		string ToXml(string elementName, Encoding encoding);

		/// <summary>
		/// Gets the XML representation of the object.
		/// </summary>
		/// <returns></returns>
		void ToXml(XmlWriter xml);

		/// <summary>
		/// Gets the XML representation of the object.
		/// </summary>
		/// <returns></returns>
		void ToXml(XmlWriter xml, Encoding encoding);

		/// <summary>
		/// Gets the XML representation of the object.
		/// </summary>
		/// <returns></returns>
		void ToXml(XmlWriter xml, string elementName);

		/// <summary>
		/// Gets the XML representation of the object.
		/// </summary>
		/// <returns></returns>
		void ToXml(XmlWriter xml, string elementName, Encoding encoding);

		/// <summary>
		/// Gets the XML representation of the object.
		/// </summary>
		/// <returns></returns>
		void ToXml(XmlWriter xml, WriteXmlContext context);

		/// <summary>
		/// Gets the object as an <c>XmlDocument</c>.
		/// </summary>
		/// <returns></returns>
		XmlDocument ToXmlDocument();

		/// <summary>
		/// Gets the object as an <c>XmlDocument</c>.
		/// </summary>
		/// <returns></returns>
		XmlDocument ToXmlDocument(string firstElementName);
	}
}
