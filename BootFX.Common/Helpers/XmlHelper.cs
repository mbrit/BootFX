// BootFX - Application framework for .NET applications
// 
// File: XmlHelper.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Data;
using System.Collections;
using System.Globalization;
using BootFX.Common.Data;

namespace BootFX.Common.Xml
{
	/// <summary>
	/// Provides helper functions for working with XML.
	/// </summary>
	public static class XmlHelper
	{
		/// <summary>
		/// Defines the namespace to ToXml activities.
		/// </summary>
		public const string ToXmlPrefix = "two47";

		/// <summary>
		/// Defines the namespace to ToXml activities.
		/// </summary>
		public const string ToXmlNamespaceUri = "http://BootFX.Common.com/schemas/toxml/";

		/// <summary>
		/// Defines the namespace used for data types.
		/// </summary>
		// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/xmlsdk/html/67e88191-8700-4782-b330-1e7c73601bd5.asp
		// document title: XDR Schema Data Types 
		public const string DataTypesNamespaceUri = "urn:schemas-microsoft-com:datatypes";
		public const string DataTypesNamespaceUriEx = "urn:uuid:c2f41010-65b3-11d1-a29f-00aa00c14882/";

		/// <summary>
		/// Defines the prefix used for data types.
		/// </summary>
		public const string DataTypesPrefix = "dt";

		private const string StringXdrTypeName = "string";
		private const string DateTimeXdrTypeName = "dateTime";
		private const string DateTimeTzXdrTypeName = "dateTime.tz";
		private const string BooleanXdrTypeName = "boolean";
		private const string Int16XdrTypeName = "i2";
		private const string Int32XdrTypeName = "i4";
		private const string IntXdrTypeName = "int";
		private const string Int64XdrTypeName = "i8";
		private const string BinBase64XdrTypeName = "bin.base64";
		private const string FloatXdrTypeName = "float";
		private const string SingleXdrTypeName = "r4";
		private const string ByteXdrTypeName = "i1";

		private const string MvStringXdrTypeName = "mv.string";
		private const string MvDateTimeTzTypeName = "mv.dateTime.tz";

		public const string XdrDateTimeFormat = "yyyy-MM-ddTHH:mm:ss.fff";
		public const string XdrDateTimeTzFormat = "yyyy-MM-ddTHH:mm:ss.fffZ";

		private const string GuidXdrTypeName = "uuid";

		/// <summary>
		/// Gets the value from the given attribute.
		/// </summary>
		/// <param name="node"></param>
		/// <param name="attributeName"></param>
		/// <param name="onNotFound"></param>
		/// <returns></returns>
        public static bool GetAttributeBoolean(this XmlNode node, string attributeName, OnNotFound onNotFound = OnNotFound.ThrowException)
        {
            // mbr - 2010-02-01 - added a default...
            return GetAttributeBoolean(node, attributeName, false, onNotFound);
        }

		/// <summary>
		/// Gets the value from the given attribute.
		/// </summary>
		/// <param name="node"></param>
		/// <param name="attributeName"></param>
		/// <param name="onNotFound"></param>
		/// <returns></returns>
        public static bool GetAttributeBoolean(this XmlNode node, string attributeName, bool defaultValue, OnNotFound onNotFound = OnNotFound.ThrowException)
		{
			return (bool)GetAttributeValue(node, attributeName, typeof(bool), defaultValue, onNotFound);
		}

		/// <summary>
		/// Gets the value from the given attribute.
		/// </summary>
		/// <param name="node"></param>
		/// <param name="attributeName"></param>
		/// <param name="onNotFound"></param>
		/// <returns></returns>
        public static Guid GetAttributeGuid(this XmlNode node, string attributeName, OnNotFound onNotFound = OnNotFound.ThrowException)
		{
			string asString = GetAttributeString(node, attributeName, onNotFound);
			if(asString != null && asString.Length > 0)
				return new Guid(asString);
			else
				return Guid.Empty;
		}

		/// <summary>
		/// Gets the value from the given attribute.
		/// </summary>
		/// <param name="node"></param>
		/// <param name="attributeName"></param>
		/// <param name="onNotFound"></param>
		/// <returns></returns>
        public static decimal GetAttributeDecimal(this XmlNode node, string attributeName, OnNotFound onNotFound = OnNotFound.ThrowException)
		{
			return (decimal)GetAttributeValue(node, attributeName, typeof(decimal), onNotFound);
		}

		/// <summary>
		/// Gets the value from the given attribute.
		/// </summary>
		/// <param name="node"></param>
		/// <param name="attributeName"></param>
		/// <param name="onNotFound"></param>
		/// <returns></returns>
        public static int GetAttributeInt32(this XmlNode node, string attributeName, OnNotFound onNotFound = OnNotFound.ThrowException)
		{
			return (int)GetAttributeValue(node, attributeName, typeof(int), onNotFound);
		}

		/// <summary>
		/// Gets the value from the given attribute.
		/// </summary>
		/// <param name="node"></param>
		/// <param name="attributeName"></param>
		/// <param name="onNotFound"></param>
		/// <returns></returns>
		public static short GetAttributeInt16(this XmlNode node, string attributeName, OnNotFound onNotFound = OnNotFound.ThrowException)
		{
			return (short)GetAttributeValue(node, attributeName, typeof(short), onNotFound);
		}

		/// <summary>
		/// Gets the value from the given attribute.
		/// </summary>
		/// <param name="node"></param>
		/// <param name="attributeName"></param>
		/// <param name="onNotFound"></param>
		/// <returns></returns>
		public static long GetAttributeInt64(this XmlNode node, string attributeName, OnNotFound onNotFound = OnNotFound.ThrowException)
		{
			return (long)GetAttributeValue(node, attributeName, typeof(long), onNotFound);
		}

		/// <summary>
		/// Gets the value from the given attribute.
		/// </summary>
		/// <param name="node"></param>
		/// <param name="attributeName"></param>
		/// <param name="onNotFound"></param>
		/// <returns></returns>
        public static string GetAttributeString(this XmlNode node, string attributeName, OnNotFound onNotFound = OnNotFound.ThrowException)
        {
            // mbr - 2010-02-01 - added a default...
            return GetAttributeString(node, attributeName, null, onNotFound);
        }

        /// <summary>
        /// Gets the value from the given attribute.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="attributeName"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        // mbr - 2010-02-01 - added this because we needed a default...
		public static string GetAttributeString(this XmlNode node, string attributeName, string defaultValue, OnNotFound onNotFound = OnNotFound.ThrowException)
		{
			if(node == null)
				throw new ArgumentNullException("node");
			if(attributeName == null)
				throw new ArgumentNullException("attributeName");
			if(attributeName.Length == 0)
				throw ExceptionHelper.CreateZeroLengthArgumentException("attributeName");
			
			// find the attribute...
			XmlAttribute attr = node.Attributes[attributeName];
			if(attr != null)
				return attr.InnerText;
			else
			{
				switch(onNotFound)
				{
					case OnNotFound.ReturnNull:
                        // mbr - 2010-02-01 - changed to return the default...
						//return null;
                        return defaultValue;

					case OnNotFound.ThrowException:
						throw new InvalidOperationException(string.Format(Cultures.Exceptions, "Attribute '{0}' not found on '{1}' ({2}).", attributeName, node.Name, node.NodeType));

					default:
						throw ExceptionHelper.CreateCannotHandleException(onNotFound);
				}
			}
		}

		/// <summary>
		/// Gets the type from an attribute.
		/// </summary>
		/// <param name="node"></param>
		/// <param name="attributeName"></param>
		/// <param name="onNotFound"></param>
		/// <returns></returns>
        public static object GetAttributeValue(this XmlNode node, string attributeName, Type type, OnNotFound onNotFound = OnNotFound.ThrowException)
        {
            return GetAttributeValue(node, attributeName, type, null, onNotFound);
        }

        /// <summary>
        /// Gets a value from an attribute.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="attributeName"></param>
        /// <param name="type"></param>
        /// <param name="onNotFound"></param>
        /// <returns></returns>
        // mbr - 2010-02-01 - changed to support default...
		public static object GetAttributeValue(this XmlNode node, string attributeName, Type type, object defaultValue, OnNotFound onNotFound = OnNotFound.ThrowException)
		{
			if(type == null)
				throw new ArgumentNullException("type");
			
			// xdr...
			XdrType xdrType = GetXdrType(type);

            // mbr - 2010-02-01 - added a default...
            //string asString = GetAttributeString(node, attributeName, onNotFound);
            string asString = GetAttributeString(node, attributeName, Convert.ToString(defaultValue, Cultures.System), onNotFound);
			return ParseValue(node, xdrType, asString);
		}

        public static T GetAttributeValue<T>(this XmlNode node, string attributeName, T defaultValue, OnNotFound onNotFound = OnNotFound.ThrowException)
        {
            return ConversionHelper.ChangeType<T>(node.GetAttributeValue(attributeName, typeof(T), defaultValue, onNotFound));
        }

        /// <summary>
        /// Gets the type from an attribute.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="attributeName"></param>
        /// <param name="onNotFound"></param>
        /// <returns></returns>
        public static Type GetAttributeType(this XmlNode node, string attributeName, OnNotFound onNotFound = OnNotFound.ThrowException)
		{
			// get the string...
			string typeName = GetAttributeString(node, attributeName, onNotFound);
			if(typeName == null)
			{
				if(onNotFound == OnNotFound.ReturnNull)
					return null;
				else
					throw new ArgumentNullException("typeName");
			}

			// convert...
			return ToType(typeName);
		}

		/// <summary>
		/// Converts a type name to a type.
		/// </summary>
		/// <param name="typeName"></param>
		/// <returns></returns>
		private static Type ToType(string typeName)
		{
			if(typeName == null)
				throw new ArgumentNullException("typeName");
			if(typeName.Length == 0)
				throw ExceptionHelper.CreateZeroLengthArgumentException("typeName");
			
			// get it...
			return Type.GetType(typeName, true, true);
		}

		/// <summary>
		/// Writes a safe CDATA block.
		/// </summary>
		/// <param name="xml"></param>
		/// <param name="buf"></param>
		/// <param name="strategy"></param>
		public static void WriteSafeCData(XmlWriter xml, string data, Encoding encoding, SafeCDataStrategy strategy)
		{
			if(xml == null)
				throw new ArgumentNullException("xml");
			if(encoding == null)
				throw new ArgumentNullException("encoding");
			
			switch(strategy)
			{
				case SafeCDataStrategy.ReplaceEndMarker:
					data = data.Replace("]]>", "%93%93%62");
					xml.WriteCData(data);
					return;

				case SafeCDataStrategy.Base64Encode:

					// does it?
					if(data.IndexOf("]]>") == -1)
						xml.WriteCData(data);
					else
					{
						// encode it...
						data = Convert.ToBase64String(encoding.GetBytes(data));
						xml.WriteAttributeString(ToXmlPrefix, "isBase64", ToXmlNamespaceUri, true.ToString());
						xml.WriteAttributeString(ToXmlPrefix, "encoding", ToXmlNamespaceUri, encoding.EncodingName);
						xml.WriteCData(data);
					}
					return;

				default:
					throw ExceptionHelper.CreateCannotHandleException(strategy);
			}
		}

		/// <summary>
		/// Writes the given data set to the given XML file.
		/// </summary>
		/// <param name="xml"></param>
		/// <param name="dataSet"></param>
		/// <remarks>This method is designed to write a *human readable* DataSet to the given XML writer.  It is used for logging
		/// and UI-related activities.  Use the DataSet's own XML reading/writing methods for machine exchange of data.</remarks>
		public static void WriteDataSet(XmlWriter xml, DataSet dataSet, Encoding encoding)
		{
			if(xml == null)
				throw new ArgumentNullException("xml");
			if(dataSet == null)
				throw new ArgumentNullException("dataSet");
			if(encoding == null)
				throw new ArgumentNullException("encoding");
			
			// write...
			xml.WriteStartElement(dataSet.DataSetName);
			foreach(DataTable table in dataSet.Tables)
				WriteDataTable(xml, table, encoding);

			// end...
			xml.WriteEndElement(); // dataSet.DataSetName
		}

		/// <summary>
		/// Writes the given data set to the given XML file.
		/// </summary>
		/// <param name="xml"></param>
		/// <param name="dataSet"></param>
		/// <remarks>This method is designed to write a *human readable* DataSet to the given XML writer.  It is used for logging
		/// and UI-related activities.  Use the DataSet's own XML reading/writing methods for machine exchange of data.</remarks>
		public static void WriteDataTable(XmlWriter xml, DataTable dataTable, Encoding encoding)
		{
			if(xml == null)
				throw new ArgumentNullException("xml");
			if(dataTable == null)
				throw new ArgumentNullException("dataTable");
			if(encoding == null)
				throw new ArgumentNullException("encoding");
			
			// write...
			xml.WriteStartElement(dataTable.TableName);
			foreach(DataRow row in dataTable.Rows)
			{
				// item...
				xml.WriteStartElement("Item");

				// lopo the columns...
				for(int index = 0; index < dataTable.Columns.Count; index++)
				{
					// data...
					DataColumn column = dataTable.Columns[index];
					object value = row[index];
					WriteDataElement(xml, column.ColumnName, value, encoding, dataTable.Locale);
				}

				// end...
				xml.WriteEndElement(); // Item
			}

			// end...
			xml.WriteEndElement(); // dataSet.TableName
		}

		/// <summary>
		/// Writes the given data element.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		public static void WriteDataElementAsCData(XmlWriter xml, string name, object value, Encoding encoding, IFormatProvider formatProvider, SafeCDataStrategy strategy)
		{
			if(xml == null)
				throw new ArgumentNullException("xml");
			if(name == null)
				throw new ArgumentNullException("name");
			if(name.Length == 0)
				throw ExceptionHelper.CreateZeroLengthArgumentException("name");
			if(encoding == null)
				throw new ArgumentNullException("encoding");
			
			// write...
			xml.WriteStartElement(name);
			if(value == null)
				xml.WriteAttributeString(ToXmlPrefix, "isNull", ToXmlNamespaceUri, true.ToString(formatProvider));
			else if(value is DBNull)
				xml.WriteAttributeString(ToXmlPrefix, "isDBNull", ToXmlNamespaceUri, true.ToString(formatProvider));
			else
			{
				// convert it?
				if(value != null)
				{
					// byte?
					if(value is byte[])
					{
						WriteDataElementAsBase64(xml, name, value, encoding);
						return;
					}
				}

				// convert...
				WriteSafeCData(xml, Convert.ToString(value, formatProvider), encoding, strategy);
			}

			// end...
			xml.WriteEndElement(); // name
		}

		/// <summary>
		/// Writes the given data element.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		public static void WriteDataElementAsBase64(XmlWriter xml, string name, object value, Encoding encoding)
		{
			if(xml == null)
				throw new ArgumentNullException("xml");
			if(name == null)
				throw new ArgumentNullException("name");
			if(name.Length == 0)
				throw ExceptionHelper.CreateZeroLengthArgumentException("name");
			if(encoding == null)
				throw new ArgumentNullException("encoding");
			
			// write...
			xml.WriteStartElement(name);
			if(value == null)
				xml.WriteAttributeString(ToXmlPrefix, "isNull", ToXmlNamespaceUri, true.ToString());
			else if(value is DBNull)
				xml.WriteAttributeString(ToXmlPrefix, "isDBNull", ToXmlNamespaceUri, true.ToString());
			else
			{
				// do we already have it?
				string valueAsString = null;
				if(!(value is byte[]))
					valueAsString = ConversionHelper.ToBase64String(value, encoding);
				else
					valueAsString = Convert.ToBase64String((byte[])value);

				// write that...
				xml.WriteAttributeString(ToXmlPrefix, "isBase64", ToXmlNamespaceUri, true.ToString());
				xml.WriteAttributeString(ToXmlPrefix, "encoding", ToXmlNamespaceUri, encoding.EncodingName);
				xml.WriteCData(valueAsString);
			}

			// end...
			xml.WriteEndElement(); // name
		}

		/// <summary>
		/// Writes the given data element.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		public static void WriteDataElement(XmlWriter xml, string name, object value, Encoding encoding, IFormatProvider formatProvider)
		{
			if(xml == null)
				throw new ArgumentNullException("xml");
			if(name == null)
				throw new ArgumentNullException("name");
			if(name.Length == 0)
				throw ExceptionHelper.CreateZeroLengthArgumentException("name");
			if(encoding == null)
				throw new ArgumentNullException("encoding");

			// are we really a null date?
			if(value is DateTime && (DateTime)value == DateTime.MinValue)
				value = null;
			
			// write...
			xml.WriteStartElement(name);
			if(value == null)
				xml.WriteAttributeString(ToXmlPrefix, "isNull", ToXmlNamespaceUri, true.ToString(formatProvider));
			else if(value is DBNull)
				xml.WriteAttributeString(ToXmlPrefix, "isDBNull", ToXmlNamespaceUri, true.ToString(formatProvider));
			else
			{
				// convert it?
				if(value != null)
				{
					// byte?
					if(value is byte[])
					{
						WriteDataElementAsBase64(xml, name, value, encoding);
						return;
					}
				}

				// convert...
				xml.WriteString(Convert.ToString(value, formatProvider));
			}

			// end...
			xml.WriteEndElement(); // name
		}

		/// <summary>
		/// Adds a child element to an element.
		/// </summary>
		/// <param name="document"></param>
		/// <param name="element"></param>
		/// <param name="name"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public static XmlElement AddElement(this XmlElement element, string name, object value)
		{
			if(element == null)
				throw new ArgumentNullException("element");
			if(name == null)
				throw new ArgumentNullException("name");
			if(name.Length == 0)
				throw new ArgumentOutOfRangeException("'name' is zero-length.");
			
			// check...
			if(element.OwnerDocument == null)
				throw new InvalidOperationException("element.OwnerDocument is null.");

			// create...
			XmlElement child = element.OwnerDocument.CreateElement(name);
			element.AppendChild(child);

			// set...
			SetElementValue(child, value);

			// return...
			return child;
		}

        public static XmlElement AddElement(this XmlElement element, string name, object value, string ns)
        {
            if (element == null)
                throw new ArgumentNullException("element");
            if (name == null)
                throw new ArgumentNullException("name");
            if (name.Length == 0)
                throw new ArgumentOutOfRangeException("'name' is zero-length.");

            // check...
            if (element.OwnerDocument == null)
                throw new InvalidOperationException("element.OwnerDocument is null.");

            // create...
            XmlElement child = element.OwnerDocument.CreateElement(name, ns);
            element.AppendChild(child);

            // set...
            SetElementValue(child, value);

            // return...
            return child;
        }

        public static XmlElement AddElement(this XmlElement element, string localName, object value, string prefix, string ns)
        {
            if (element == null)
                throw new ArgumentNullException("element");
            if (localName == null)
                throw new ArgumentNullException("name");
            if (localName.Length == 0)
                throw new ArgumentOutOfRangeException("'name' is zero-length.");

            // check...
            if (element.OwnerDocument == null)
                throw new InvalidOperationException("element.OwnerDocument is null.");

            // create...
            XmlElement child = element.OwnerDocument.CreateElement(prefix, localName, ns);
            element.AppendChild(child);

            // set...
            SetElementValue(child, value);

            // return...
            return child;
        }

        /// <summary>
        /// Adds a child element to an element.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="element"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [Obsolete("Deprecated.")]
		public static XmlElement AddElement(XmlDocument document, XmlElement element, string name, string value)
		{
			return AddElement(element, name, value);
		}

		/// <summary>
		/// Adds an attribute to an element.
		/// </summary>
		/// <param name="document"></param>
		/// <param name="element"></param>
		/// <param name="name"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		[Obsolete("Use SetAttributeValue instead.")]
		public static XmlAttribute AddAttribute(this XmlElement element, string name, string value)
		{
			if(element == null)
				throw new ArgumentNullException("element");
			if(name == null)
				throw new ArgumentNullException("name");
			if(name.Length == 0)
				throw new ArgumentOutOfRangeException("'name' is zero-length.");
			
			// create...
			XmlAttribute attribute = element.OwnerDocument.CreateAttribute(name);
			element.Attributes.Append(attribute);

			// set...
			SetNodeValue(attribute, value);

			// return...
			return attribute;
		}

		/// <summary>
		/// Adds an attribute to an element.
		/// </summary>
		/// <param name="document"></param>
		/// <param name="element"></param>
		/// <param name="name"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		[Obsolete("Deprecated.")]
		public static XmlAttribute AddAttribute(XmlDocument document, XmlElement element, string name, string value)
		{
			return AddAttribute(element, name, value);
		}

		/// <summary>
		/// Gets the value for the given child element.
		/// </summary>
		/// <param name="node"></param>
		/// <param name="name"></param>
		/// <param name="onNotFound"></param>
		/// <returns></returns>
		public static Guid GetElementGuid(this XmlNode node, string name, OnNotFound onNotFound = OnNotFound.ThrowException)
		{
			if(node == null)
				throw new ArgumentNullException("node");
			if(name == null)
				throw new ArgumentNullException("name");
			if(name.Length == 0)
				throw new ArgumentOutOfRangeException("'name' is zero-length.");
			
			// bool...
			string asString = GetElementString(node, name, onNotFound);
			if(asString != null && asString.Length > 0)
				return new Guid(asString);
			else
				return Guid.Empty;
		}

		/// <summary>
		/// Gets the value for the given child element.
		/// </summary>
		/// <param name="node"></param>
		/// <param name="name"></param>
		/// <param name="onNotFound"></param>
		/// <returns></returns>
		public static short GetElementInt16(this XmlNode node, string name, OnNotFound onNotFound = OnNotFound.ThrowException)
		{
			if(node == null)
				throw new ArgumentNullException("node");
			if(name == null)
				throw new ArgumentNullException("name");
			if(name.Length == 0)
				throw new ArgumentOutOfRangeException("'name' is zero-length.");
			
			// bool...
			object result = GetElementValue(node, name, typeof(short), onNotFound);
			if(result == null)
				return 0;
			else
				return (short)result;
		}

		/// <summary>
		/// Gets the value for the given child element.
		/// </summary>
		/// <param name="node"></param>
		/// <param name="name"></param>
		/// <param name="onNotFound"></param>
		/// <returns></returns>
		public static decimal GetElementDecimal(this XmlNode node, string name, OnNotFound onNotFound = OnNotFound.ThrowException)
		{
			if(node == null)
				throw new ArgumentNullException("node");
			if(name == null)
				throw new ArgumentNullException("name");
			if(name.Length == 0)
				throw new ArgumentOutOfRangeException("'name' is zero-length.");
			
			// bool...
			object result = GetElementValue(node, name, typeof(decimal), onNotFound);
			if(result == null)
				return 0;
			else
				return (decimal)result;
		}

		/// <summary>
		/// Gets the value for the given child element.
		/// </summary>
		/// <param name="node"></param>
		/// <param name="name"></param>
		/// <param name="onNotFound"></param>
		/// <returns></returns>
		public static int GetElementInt32(this XmlNode node, string name, OnNotFound onNotFound = OnNotFound.ThrowException)
		{
			if(node == null)
				throw new ArgumentNullException("node");
			if(name == null)
				throw new ArgumentNullException("name");
			if(name.Length == 0)
				throw new ArgumentOutOfRangeException("'name' is zero-length.");
			
			// bool...
			object result = GetElementValue(node, name, typeof(int), onNotFound);
			if(result == null)
				return 0;
			else
				return (int)result;
		}

		/// <summary>
		/// Gets the value for the given child element.
		/// </summary>
		/// <param name="node"></param>
		/// <param name="name"></param>
		/// <param name="onNotFound"></param>
		/// <returns></returns>
		public static long GetElementInt64(this XmlNode node, string name, OnNotFound onNotFound = OnNotFound.ThrowException)
		{
			if(node == null)
				throw new ArgumentNullException("node");
			if(name == null)
				throw new ArgumentNullException("name");
			if(name.Length == 0)
				throw new ArgumentOutOfRangeException("'name' is zero-length.");
			
			// bool...
			object result = GetElementValue(node, name, typeof(long), onNotFound);
			if(result == null)
				return 0;
			else
				return (long)result;
		}

		// NMY - 31/10/2007 - Case 967: Superclient - Superclient Clerk Approval CCR
		/// <summary>
		/// Gets the value for the given child element.
		/// </summary>
		/// <param name="node"></param>
		/// <param name="name"></param>
		/// <param name="onNotFound"></param>
		/// <returns></returns>
		public static DateTime GetElementDateTime(this XmlNode node, string name, OnNotFound onNotFound = OnNotFound.ThrowException)
		{
			if(node == null)
				throw new ArgumentNullException("node");
			if(name == null)
				throw new ArgumentNullException("name");
			if(name.Length == 0)
				throw new ArgumentOutOfRangeException("'name' is zero-length.");
			
			// DateTime...
			object result = GetElementValue(node, name, typeof(DateTime), onNotFound);
			if(result == null)
				return DateTime.MinValue;
			else
				return (DateTime)result;
		}

		/// <summary>
		/// Gets the value for the given child element.
		/// </summary>
		/// <param name="node"></param>
		/// <param name="name"></param>
		/// <param name="onNotFound"></param>
		/// <returns></returns>
		public static bool GetElementBoolean(this XmlNode node, string name, OnNotFound onNotFound = OnNotFound.ThrowException)
		{
			if(node == null)
				throw new ArgumentNullException("node");
			if(name == null)
				throw new ArgumentNullException("name");
			if(name.Length == 0)
				throw new ArgumentOutOfRangeException("'name' is zero-length.");
			
			// bool...
			object result = GetElementValue(node, name, typeof(bool), onNotFound);
			if(result == null)
				return false;
			else
				return (bool)result;
		}

		/// <summary>
		/// Gets the value for the given node.
		/// </summary>
		/// <param name="node"></param>
		/// <param name="name"></param>
		/// <param name="onNotFound"></param>
		/// <returns></returns>
		public static object GetNodeValue(this XmlNode node, Type type)
		{
			if(node == null)
				throw new ArgumentNullException("node");
			if(type == null)
				throw new ArgumentNullException("type");
			
			// get...
			XdrType xdrType = GetXdrType(type);

			// return...
			return ParseValue(node, xdrType, node.InnerText);
		}

        /// <summary>
        /// Gets the type from the given XDR type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Type GetTypeFromXdrType(XdrType type)
        {
            switch (type)
            {
                case XdrType.Boolean:
                    return typeof(bool);

                case XdrType.Byte:
                    return typeof(byte);

                case XdrType.DateTime:
                case XdrType.DateTimeTz:
                    return typeof(DateTime);

                case XdrType.Double:
                    return typeof(double);

                case XdrType.Guid:
                    return typeof(Guid);

                case XdrType.Int16:
                    return typeof(short);

                case XdrType.Int32:
                    return typeof(int);

                case XdrType.Int64:
                    return typeof(long);

                case XdrType.Single:
                    return typeof(float);

                case XdrType.String:
                    return typeof(string);

                default:
                    throw new NotSupportedException(string.Format("Cannot handle '{0}'.", type));
            }
        }

		/// <summary>
		/// Gets the value for the given child element.
		/// </summary>
		/// <param name="node"></param>
		/// <param name="name"></param>
		/// <param name="onNotFound"></param>
		/// <returns></returns>
		public static object GetElementValue(this XmlNode node, string name, Type type, OnNotFound onNotFound = OnNotFound.ThrowException)
		{
			if(node == null)
				throw new ArgumentNullException("node");
			if(name == null)
				throw new ArgumentNullException("name");
			if(name.Length == 0)
				throw new ArgumentOutOfRangeException("'name' is zero-length.");

			// find the element...
			XmlElement element = (XmlElement)node.SelectSingleNode(name);
            if (element != null)
            {
                // mbr - 2008-12-19 - if the type is null, load it from the element...
                if (type == null)
                {
                    XdrType xdrType = GetXdrType(element);
                    type = GetTypeFromXdrType(xdrType);
                    if (type == null)
                        throw new InvalidOperationException("'type' is null.");
                }

                // defer...
                return GetNodeValue(element, type);
            }
            else
            {
                switch (onNotFound)
                {
                    case OnNotFound.ThrowException:
                        throw new InvalidOperationException(string.Format("An element with name '{0}' was not found within '{1}'.", name, node.Name));

                    case OnNotFound.ReturnNull:
                        return null;

                    default:
                        throw new NotSupportedException(string.Format("Cannot handle '{0}' ({1}).", onNotFound, onNotFound.GetType()));
                }
            }

//			// get...
//			string value = GetElementString(node, name, onNotFound);
//			if(value != null)
//			{
//				// convert...
//				return ConversionHelper.ChangeType(value, type, Cultures.System);
//			}
//			else
//				return null;
		}

		/// <summary>
		/// Gets the type name from the element.
		/// </summary>
		/// <param name="node"></param>
		/// <param name="name"></param>
		/// <param name="onNotFound"></param>
		/// <returns></returns>
		public static Type GetElementType(this XmlNode node, string name, OnNotFound onNotFound, bool throwOnTypeLoadError)
		{
			if(name == null)
				throw new ArgumentNullException("name");
			if(name == null)
				throw new ArgumentNullException("name");
			if(name.Length == 0)
				throw new ArgumentOutOfRangeException("'name' is zero-length.");
			
			// get...
			string typeName = GetElementString(node, name, onNotFound);
			if(typeName == null)
				return null;

			// return...
			return Type.GetType(typeName, throwOnTypeLoadError, true);
		}

		/// <summary>
		/// Gets the value for the given child element.
		/// </summary>
		/// <param name="node"></param>
		/// <param name="name"></param>
		/// <param name="onNotFound"></param>
		/// <returns></returns>
		public static object GetElementEnumerationValue(this XmlNode node, string name, Type enumerationType, OnNotFound onNotFound = OnNotFound.ThrowException)
		{
			if(node == null)
				throw new ArgumentNullException("node");
			if(name == null)
				throw new ArgumentNullException("name");
			if(name.Length == 0)
				throw new ArgumentOutOfRangeException("'name' is zero-length.");
			if(enumerationType == null)
				throw new ArgumentNullException("enumerationType");
			if(enumerationType.IsEnum == false)
				throw new InvalidOperationException(string.Format("'{0}' is not an enumeration type.", enumerationType));
			
			// get it...
			string valueAsString = GetElementString(node, name, onNotFound);
			if(valueAsString == null)
				return null;

			// parse...
			return Enum.Parse(enumerationType, valueAsString, true);
		}

		/// <summary>
		/// Gets the value for the given child element.
		/// </summary>
		/// <param name="node"></param>
		/// <param name=",loica"></param>
		/// <param name="onNotFound"></param>
		/// <returns></returns>
		public static string GetElementString(this XmlNode node, string prefix, string localName, XmlNamespaceManagerEx manager, OnNotFound onNotFound = OnNotFound.ThrowException)
		{
			if(node == null)
				throw new ArgumentNullException("node");
			if(localName == null)
				throw new ArgumentNullException("localName");
			if(localName.Length == 0)
				throw new ArgumentOutOfRangeException("'localName' is zero-length.");

			// inner...
			XmlNamespaceManager innerManager = null;
			if(manager != null)
				innerManager = manager.Manager;
			
			// name...
			string name = localName;
			if(prefix != null && prefix.Length > 0)
			{
				if(innerManager == null)
					throw new InvalidOperationException("innerManager is null.");
				name = prefix + ":" + localName;
			}

			// get...
			XmlNode childNode = node.SelectSingleNode(name, innerManager);
			if(childNode != null)
				return childNode.InnerText;
			else
			{
				switch(onNotFound)
				{
					case OnNotFound.ReturnNull:
						return null;
					case OnNotFound.ThrowException:
						throw new InvalidOperationException(string.Format("Node '{0}' did not contain an element of type '{1}'.", node, name));

					default:
						throw new NotSupportedException(string.Format("Cannot handle '{0}' ({1}).", onNotFound, onNotFound.GetType()));
				}
			}
		}

		/// <summary>
		/// Gets the value for the given child element.
		/// </summary>
		/// <param name="node"></param>
		/// <param name="name"></param>
		/// <param name="onNotFound"></param>
		/// <returns></returns>
		public static string GetElementString(this XmlNode node, string name, OnNotFound onNotFound = OnNotFound.ThrowException)
		{
			return GetElementString(node, null, name, null, onNotFound);
		}

		/// <summary>
		/// Populates an existing value element, handling XDR data types.
		/// </summary>
		/// <param name="element"></param>
		/// <param name="value"></param>
		public static void SetElementValue(this XmlElement element, object value)
		{
			if(element == null)
				throw new ArgumentNullException("element");

			// clear all attributes and child elements.
			element.RemoveAll();

			// mbr - 02-12-2005 - defer...			
			SetNodeValue(element, value);
		}

		/// <summary>
		/// Gets a namespace manager for a document.
		/// </summary>
		/// <param name="document"></param>
		/// <returns></returns>
		public static XmlNamespaceManagerEx GetNamespaceManagerEx(XmlDocument document)
		{
			if(document == null)
				throw new ArgumentNullException("document");
			
			// create...
			XmlNamespaceManager manager = new XmlNamespaceManager(document.NameTable);
			XmlNamespaceManagerEx managerEx = new XmlNamespaceManagerEx(document, manager);

			// walk...
			WalkAndPopulatePrefixes(document, manager, managerEx);

			// return...
			return managerEx;
		}

		/// <summary>
		/// Gets a namespace manager for a document.
		/// </summary>
		/// <param name="document"></param>
		/// <returns></returns>
		public static XmlNamespaceManager GetNamespaceManager(XmlDocument document)
		{
			// create...
			XmlNamespaceManagerEx managerEx = GetNamespaceManagerEx(document);
			if(managerEx == null)
				throw new InvalidOperationException("managerEx is null.");

			// return...
			if(managerEx.Manager == null)
				throw new InvalidOperationException("managerEx.Manager is null.");
			return managerEx.Manager;
		}

		/// <summary>
		/// Sets up the prefixes.
		/// </summary>
		/// <param name="node"></param>
		/// <param name="manager"></param>
		internal static void WalkAndPopulatePrefixes(this XmlNode node, XmlNamespaceManager manager, XmlNamespaceManagerEx managerEx)
		{
			if(node == null)
				throw new ArgumentNullException("node");
			if(manager == null)
				throw new ArgumentNullException("manager");

			// walk everything...
			PopulatePrefix(node, manager, managerEx);

			// children...
			foreach(XmlNode child in node.ChildNodes)
				WalkAndPopulatePrefixes(child, manager, managerEx);
		}

		private static void PopulatePrefix(this XmlNode node, XmlNamespaceManager manager, XmlNamespaceManagerEx managerEx)
		{
			if(node == null)
				throw new ArgumentNullException("node");
			
			// do we have one?
			if(node.Prefix != null && node.Prefix.Length > 0)
			{
				if(!(manager.HasNamespace(node.Prefix)))
				{
					manager.AddNamespace(node.Prefix, node.NamespaceURI);

					// ex?
					if(managerEx != null)
						managerEx.AddNamespace(node.Prefix, node.NamespaceURI, false);
				}
			}
		}

		/// <summary>
		/// Populates an existing value element, handling XDR data types.
		/// </summary>
		/// <param name="node"></param>
		/// <param name="value"></param>
		private static void SetNodeValue(this XmlNode node, object value)
		{
			if(node == null)
				throw new ArgumentNullException("node");
						
			// what is it?
			if(value == null)
				node.InnerText = string.Empty;
			else if(value is string)
				node.InnerText = (string)value;
			else if(value is Type)
				node.InnerText = ((Type)value).FullName + ", " + ((Type)value).Assembly.GetName().Name;
			else
			{
				Type type = value.GetType();

				// mbr - 03-11-2005 - is the value an enumeration type?  string if so...
				if(type.IsEnum)
					type = typeof(string);

				// get the code...
				XdrType xdr = GetXdrType(type);
				string xdrName = GetXdrTypeName(xdr);
				if(xdrName == null)
					throw new InvalidOperationException("'xdrName' is null.");
				if(xdrName.Length == 0)
					throw new InvalidOperationException("'xdrName' is zero-length.");

				// mbr - 02-12-2005 - only if xmlelement...
				if(node is XmlElement)
				{
					// add the node indicating the type first...
					XmlAttribute attr = node.OwnerDocument.CreateAttribute(DataTypesPrefix, "type", DataTypesNamespaceUri);
					if(attr == null)
						throw new InvalidOperationException("attr is null.");
					node.Attributes.Append(attr);
					attr.InnerText = xdrName;
				}

				// now, format the value...
				string asString = FormatValue(xdr, value);

				// add...
				node.InnerText = asString;
			}
		}
		/// <summary>
		/// Formats up the value.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		private static string FormatValue(XdrType type, object value)
		{
			switch(type)
			{
				case XdrType.String:
				case XdrType.Int16:
				case XdrType.Int32:
				case XdrType.Int64:
				case XdrType.Float:
				case XdrType.Single:
				case XdrType.Byte:
					return ConversionHelper.ToString(value, Cultures.System);

				case XdrType.DateTime:
					DateTime dateTime = ConversionHelper.ToDateTime(value, Cultures.System);
					return dateTime.ToString(XdrDateTimeFormat);

				case XdrType.DateTimeTz:
					DateTime dateTimeTz = ConversionHelper.ToDateTime(value, Cultures.System);
					return dateTimeTz.ToString(XdrDateTimeTzFormat);
					
				case XdrType.Boolean:
					if(ConversionHelper.ToBoolean(value, Cultures.System))
						return "1";
					else
						return "0";

				case XdrType.Guid:
					if(value == null || value is DBNull)
						return Guid.Empty.ToString();
					if(value is Guid)
						return ((Guid)value).ToString();
					else
						throw new NotSupportedException(string.Format("Cannot handle '{0}'.", value.GetType()));

				default:
					throw new NotSupportedException(string.Format("Cannot handle '{0}' ({1}).", type, type.GetType()));
			}
		}

		/// <summary>
		/// Gets the XDR data type for the given type code.
		/// </summary>
		/// <param name="code"></param>
		/// <returns></returns>
		private static XdrType GetXdrType(Type type)
		{
			TypeCode code = Type.GetTypeCode(type);
			switch(code)
			{
				case TypeCode.String:
					return XdrType.String;

				// mbr - 08-06-2007 - changed this to DateTime cf DateTimeTz as the date/time object
				// has no notion of a time zone.
				case TypeCode.DateTime:
//					return XdrType.DateTimeTz;
					return XdrType.DateTime;

				case TypeCode.Boolean:
					return XdrType.Boolean;

				case TypeCode.Int16:
					return XdrType.Int16;
				case TypeCode.Int32:
					return XdrType.Int32;
				case TypeCode.Int64:
					return XdrType.Int64;

				case TypeCode.Double:
				case TypeCode.Decimal:
					return XdrType.Float;

				case TypeCode.Single:
					return XdrType.Single;

				case TypeCode.Object:
					if(typeof(Guid).IsAssignableFrom(type))
						return XdrType.Guid;
					else
						throw new NotSupportedException(string.Format("Cannot handle type '{0}'.", type));

				case TypeCode.Byte:
					return XdrType.Byte;

				default:
					throw new NotSupportedException(string.Format("Cannot handle '{0}' ({1}).", code, code.GetType()));
			}
		}

		/// <summary>
		/// Gets the type name for an XDR type.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		private static string GetXdrTypeName(XdrType type)
		{
			switch(type)
			{
				case XdrType.String:
					return StringXdrTypeName;

				case XdrType.DateTime:
					return DateTimeXdrTypeName;
				case XdrType.DateTimeTz:
					return DateTimeTzXdrTypeName;

				case XdrType.Boolean:
					return BooleanXdrTypeName;

				case XdrType.Int16:
					return Int16XdrTypeName;
				case XdrType.Int32:
					return Int32XdrTypeName;
				case XdrType.Int64:
					return Int64XdrTypeName;

				case XdrType.Guid:
					return GuidXdrTypeName;

				case XdrType.Float:
					return FloatXdrTypeName;
				case XdrType.Single:
					return SingleXdrTypeName;

				case XdrType.Byte:
					return ByteXdrTypeName;

				default:
					throw new NotSupportedException(string.Format("Cannot handle '{0}' ({1}).", type, type.GetType()));
			}
		}

		/// <summary>
		/// Sets the attribute value.
		/// </summary>
		/// <param name="element"></param>
		/// <param name="name"></param>
		/// <param name="value"></param>
		public static void SetAttributeValue(this XmlElement element, string name, object value)
		{
			if(element == null)
				throw new ArgumentNullException("element");
			if(name == null)
				throw new ArgumentNullException("name");
			if(name.Length == 0)
				throw new ArgumentOutOfRangeException("'name' is zero-length.");
			
			// find it...
			XmlAttribute attr = element.Attributes[name];
			if(attr == null)
			{
				attr = element.OwnerDocument.CreateAttribute(name);
				if(attr == null)
					throw new InvalidOperationException("attr is null.");

				// add...
				element.Attributes.Append(attr);
			}

			// set...
			SetNodeValue(attr, value);
		}

		/// <summary>
		/// Sets the attribute value.
		/// </summary>
		/// <param name="element"></param>
		/// <param name="name"></param>
		/// <param name="value"></param>
		public static void SetAttributeValue(this XmlElement element, string name, string value)
		{
			if(element == null)
				throw new ArgumentNullException("element");
			if(name == null)
				throw new ArgumentNullException("name");
			if(name.Length == 0)
				throw new ArgumentOutOfRangeException("'name' is zero-length.");
			
			// defer...
			SetAttributeValue(element, name, (object)value);
		}

		/// <summary>
		/// Gets the XDR type from the given name.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		internal static XdrType GetXdrType(string name)
		{
			if(name == null)
				throw new ArgumentNullException("name");
			if(name.Length == 0)
				throw new ArgumentOutOfRangeException("'name' is zero-length.");
			
			// get...
			if(name == StringXdrTypeName)
				return XdrType.String;
			else if(name == DateTimeXdrTypeName)
				return XdrType.DateTime;
			else if(name == DateTimeTzXdrTypeName)
				return XdrType.DateTimeTz;
			else if(name == BooleanXdrTypeName)
				return XdrType.Boolean;
			else if(name == Int16XdrTypeName)
				return XdrType.Int16;
			else if(name == Int32XdrTypeName || name == IntXdrTypeName)
				return XdrType.Int32;
			else if(name == Int64XdrTypeName)
				return XdrType.Int64;
			else if(name == GuidXdrTypeName)
				return XdrType.Guid;
			else if(name == BinBase64XdrTypeName)
				return XdrType.BinBase64;
			else if(name == FloatXdrTypeName)
				return XdrType.Float;
			else if(name == MvStringXdrTypeName)
				return XdrType.MvString;
			else if(name == MvDateTimeTzTypeName)
				return XdrType.MvDateTimeTz;
			else if(name == ByteXdrTypeName)
				return XdrType.Byte;
			else if(name == SingleXdrTypeName)
				return XdrType.Single;
			else if(name == FloatXdrTypeName)
				return XdrType.Float;
			else
				throw new NotSupportedException(string.Format("Cannot handle '{0}'.", name));
		}

        /// <summary>
		/// Gets the XDR type of the element.
		/// </summary>
		/// <param name="element"></param>
		/// <returns></returns>
        public static XdrType GetXdrType(this XmlNode node, string name)
        {
            if (node == null)
                throw new ArgumentNullException("node");
            if (name == null)
                throw new ArgumentNullException("name");
            if (name.Length == 0)
                throw new ArgumentException("'name' is zero-length.");

            // get...
            XmlElement element = (XmlElement)node.SelectSingleNode(name);
            if (element == null)
                throw new InvalidOperationException("'element' is null.");

            // defer...
            return GetXdrType(element);
        }

		/// <summary>
		/// Gets the XDR type of the element.
		/// </summary>
		/// <param name="element"></param>
		/// <returns></returns>
		public static XdrType GetXdrType(XmlElement element)
		{
			if(element == null)
				throw new ArgumentNullException("element");
			
			// get it...
			XmlAttribute attribute = element.Attributes["type", DataTypesNamespaceUri];
			if(attribute != null)
				return GetXdrType(attribute.Value);
			else
			{
				attribute = element.Attributes["dt", DataTypesNamespaceUriEx];
				if(attribute != null)
					return GetXdrType(attribute.Value);
				else
					return XdrType.String;
			}
		}

		public static object GetElementValue(XmlElement element)
		{
			if(element == null)
				throw new ArgumentNullException("element");
			
			// get teh type...
			XdrType xdrType = GetXdrType(element);

			// parse...
			return ParseValue(element, xdrType, element.InnerText);
		}

		private static DbType GetDbTypeForXdrType(XdrType type)
		{
			switch(type)
			{
				case XdrType.String:
					return DbType.String;

				case XdrType.Boolean:
					return DbType.Boolean;

				case XdrType.DateTime:
					return DbType.DateTime;

				case XdrType.Int16:
					return DbType.Int16;
				case XdrType.Int32:
					return DbType.Int32;
				case XdrType.Int64:
					return DbType.Int64;

				default:
					throw new NotSupportedException(string.Format("Cannot handle '{0}' ({1}).", type, type.GetType()));
			}
		}

		/// <summary>
		/// Parses an XDR value.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="text"></param>
		/// <returns></returns>
		private static object ParseValue(this XmlNode node, XdrType type, string text)
		{
			// null?
			if(text == null || text.Length == 0)
			{
				DbType dbType = GetDbTypeForXdrType(type);
				return ConversionHelper.GetClrLegalDBNullEquivalent(dbType);
			}
			else
			{
				// switch...
				switch(type)
				{
					case XdrType.String:
						return text;

					case XdrType.DateTime:
					{
						try
						{
                            // mbr - 2007-06-08 - parameters reversed...!
                            //return DateTime.ParseExact(XdrDateTimeFormat, text, Cultures.System);
                            return DateTime.ParseExact(text, XdrDateTimeFormat, Cultures.System);
                        }
						catch(Exception ex)
						{
							throw new InvalidOperationException(string.Format("Failed to convert '{0}' to a date/time value.", text), ex);
						}
					}

					case XdrType.DateTimeTz:
					{
						try
						{
							return DateTime.Parse(text, Cultures.System);
						}
						catch(Exception ex)
						{
							throw new InvalidOperationException(string.Format("Failed to convert '{0}' to a date/time (TZ) value.", text), ex);
						}
					}

					case XdrType.Boolean:
						if(text == "0")
							return false;
						else if(text == "1")
							return true;
						else if(string.Compare(text, "false", true, System.Globalization.CultureInfo.InvariantCulture) == 0)
							return false;
						else if(string.Compare(text, "true", true, System.Globalization.CultureInfo.InvariantCulture) == 0)
							return true;
						else
							throw new InvalidOperationException(string.Format("Could not parse '{0}' to a Boolean.", text));

					case XdrType.Byte:
						return ConversionHelper.ToByte(text, Cultures.System);

					case XdrType.Int16:
						return ConversionHelper.ToInt16(text, Cultures.System);
					case XdrType.Int32:
						return ConversionHelper.ToInt32(text, Cultures.System);
					case XdrType.Int64:
						return ConversionHelper.ToInt64(text, Cultures.System);

						// mbr - 04-10-2006 - added.						
					case XdrType.Float:
					case XdrType.Single:
						return ConversionHelper.ToDecimal(text, Cultures.System);

					case XdrType.Guid:
						try
						{
							return new Guid(text);
						}
						catch(Exception ex)
						{
							throw new InvalidOperationException(string.Format("Could not convert '{0}' to a GUID.", text), ex);
						}

					case XdrType.BinBase64:
						return Convert.FromBase64String(text);

					case XdrType.MvString:
					case XdrType.MvDateTimeTz:
						return ParseMultiValue(node, type);

					default:
						throw new NotSupportedException(string.Format("Cannot handle '{0}' ({1}).", type, type.GetType()));
				}
			}
		}

		/// <summary>
		/// Parses a multivalue element.
		/// </summary>
		/// <param name="node"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		private static Array ParseMultiValue(this XmlNode node, XdrType type)
		{
			if(node == null)
				throw new ArgumentNullException("node");
			if(!(node is XmlElement))
				throw new InvalidOperationException(string.Format("Cannot handle '{0}'.  Only XmlElement nodes can contain multiple values.", node.GetType()));
			
			// get...
			XdrType singular = XdrType.String;
			Type singularType = null;
			switch(type)
			{
				case XdrType.MvString:
					singular = XdrType.String;
					singularType = typeof(string);
					break;

				case XdrType.MvDateTimeTz:
					singular = XdrType.DateTimeTz;
					singularType = typeof(DateTime);
					break;

				default:
					throw new NotSupportedException(string.Format("Cannot handle '{0}' ({1}).", type, type.GetType()));
			}

			// walk...
			ArrayList results = new ArrayList();
			foreach(XmlElement child in ((XmlElement)node).ChildNodes)
			{
				// get...
				object item = ParseValue(child, singular, child.InnerText);
				results.Add(item);
			}

			// return...
			return results.ToArray(singularType);
		}

		/// <summary>
		/// Gets the root element from a document.
		/// </summary>
		/// <param name="doc"></param>
		/// <returns></returns>
		public static XmlElement GetRootElement(XmlDocument doc, bool throwIfNotFound)
		{
			if(doc == null)
				throw new ArgumentNullException("doc");
			
			// walk...
			foreach(XmlNode node in doc.ChildNodes)
			{
				if(node is XmlElement)
					return (XmlElement)node;
			}

			// nope...
			if(throwIfNotFound)
				throw new InvalidOperationException("Document does not have a root element.");
			else
				return null;
		}

		public static string XmlDocumentToString(XmlDocument xml)
		{
			if(xml == null)
				throw new ArgumentNullException("xml");

			// mbr - 15-05-2008 - changed to return the outer xml...
//			return XmlDocumentToString(xml, Formatting.Indented);
			return xml.OuterXml;
		}

		// mbr - 15-05-2008 - maded obsolete.
		[Obsolete("Do not use this method.")]
		public static string XmlDocumentToString(XmlDocument xml, Formatting formatting)
		{
			return XmlDocumentToString(xml, Encoding.Unicode, formatting);
		}

		// mbr - 15-05-2008 - maded obsolete.
		[Obsolete("Do not use this method.")]
		public static string XmlDocumentToString(XmlDocument xml, Encoding encoding)
		{
			return XmlDocumentToString(xml, encoding, Formatting.Indented);
		}

		/// <summary>
		/// Converts the XML document to a string.
		/// </summary>
		/// <param name="xml"></param>
		/// <param name="encoding"></param>
		/// <returns></returns>
		// mbr - 15-05-2008 - maded obsolete.
		[Obsolete("Do not use this method.")]
		public static string XmlDocumentToString(XmlDocument xml, Encoding encoding, Formatting formatting)
		{
			if(xml == null)
				throw new ArgumentNullException("xml");
			if(encoding == null)
				throw new ArgumentNullException("encoding");
	
			// output...
			using(MemoryStream stream = new MemoryStream())
			{
				// output...
				XmlTextWriter xmlWriter = new XmlTextWriter(stream, encoding);
				xmlWriter.Formatting = formatting;
				xml.Save(xmlWriter);

				// return...
				xmlWriter.Flush();
				stream.Flush();

				// return...
				return encoding.GetString(stream.ToArray());
			}			
		}

        public static void AppendCrLfWhitespace(this XmlElement element)
        {
            element.AppendChild(element.OwnerDocument.CreateWhitespace("\r\n"));
        }

        public static void AppendTabs(this XmlElement element, int numTabs = 1)
        {
            StringBuilder builder = new StringBuilder();
            for (int index = 0; index < numTabs; index++)
                builder.Append("\r\n");

            element.AppendChild(element.OwnerDocument.CreateWhitespace(builder.ToString()));
        }

        public static void AppendComment(this XmlElement element, string comment)
        {
            var el = element.OwnerDocument.CreateComment(comment);
            element.AppendChild(el);
        }

        public static XmlElement AddElement(this XmlElement element, string name)
        {
            var el = element.OwnerDocument.CreateElement(name);
            element.AppendChild(el);
            return el;
        }
    }
}
