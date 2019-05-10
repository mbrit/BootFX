// BootFX - Application framework for .NET applications
// 
// File: XmlPersistenceStrategy.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;

namespace BootFX.Common.Xml
{
	/// <summary>
	/// Defines different types of XML persistence strategies.
	/// </summary>
	public enum XmlPersistenceStrategy
	{
		/// <summary>
		/// Uses the <see cref="Object.ToString"></see> representation of the object.
		/// </summary>
		ToString = 0,

		/// <summary>
		/// Uses the <see cref="IToXml"></see> interface.
		/// </summary>
		IToXml = 1,

		/// <summary>
		/// Uses the <see cref="System.Data.DataSet.WriteXml"></see> method.
		/// </summary>
		DataSet = 2,

		/// <summary>
		/// Uses the SOAP formatter - i.e. .NET serialization.
		/// </summary>
		Soap = 3,

		/// <summary>
		/// The object in question is an XML node, so <see cref="System.Xml.XmlNode.WriteTo"></see> is used.
		/// </summary>
		XmlNode = 4,

		/// <summary>
		/// The object is a base CLR type, and so a well-known representation is used, e.g. <see cref="Int32.ToString"></see>.
		/// </summary>
		ClrType = 5
	}
}
