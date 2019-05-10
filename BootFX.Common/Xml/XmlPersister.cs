// BootFX - Application framework for .NET applications
// 
// File: XmlPersister.cs
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
using System.Globalization;

namespace BootFX.Common.Xml
{
	/// <summary>
	/// Class that converts objects to their XML equivalents.  
	/// </summary>
	/// <remarks>This is slightly different to serialization in that it understands the classes own ability to handle transforming itself to or from XML.</remarks>
	public class XmlPersister
	{
		/// <summary>
		/// Private field to support <c>Culture</c> property.
		/// </summary>
		private CultureInfo _culture = Cultures.System;
		
		/// <summary>
		/// Private field to support <c>Strategy</c> property.
		/// </summary>
		private XmlPersistenceStrategy _strategy;
		
		/// <summary>
		/// Private field to support <c>Type</c> property.
		/// </summary>
		private Type _type;

		/// <summary>
		/// Private field to support <c>ElementName</c> property.
		/// </summary>
		private string _elementName;

		/// <summary>
		/// Private field to support <c>Encoding</c> property.
		/// </summary>
		private Encoding _encoding;
		
		/// <summary>
		/// Constructor.
		/// </summary>
		public XmlPersister(Type type, string elementName, Encoding encoding)
		{
			if(type == null)
				throw new ArgumentNullException("type");
			if(elementName == null)
				throw new ArgumentNullException("elementName");
			if(elementName.Length == 0)
				throw ExceptionHelper.CreateZeroLengthArgumentException("elementName");
			if(encoding == null)
				throw new ArgumentNullException("encoding");

			// set...
			_type = type;
			_elementName = elementName;
			_encoding = encoding;
			_strategy = GetStrategy(type);
		}

		/// <summary>
		/// Gets the encoding.
		/// </summary>
		public Encoding Encoding
		{
			get
			{
				return _encoding;
			}
		}
		
		/// <summary>
		/// Gets the elementname.
		/// </summary>
		public string ElementName
		{
			get
			{
				return _elementName;
			}
		}
		
		/// <summary>
		/// Gets the type.
		/// </summary>
		public Type Type
		{
			get
			{
				return _type;
			}
		}

		/// <summary>
		/// Gets the strategy for the given object.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public static XmlPersistenceStrategy GetStrategy(object obj)
		{
			if(obj == null)
				throw new ArgumentNullException("obj");
			
			return GetStrategy(obj.GetType());
		}

		/// <summary>
		/// Gets the strategy for the given type.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public static XmlPersistenceStrategy GetStrategy(Type type)
		{
			if(type == null)
				throw new ArgumentNullException("type");
			
			// check whether it's a CLR type...
			TypeCode typeCode = Type.GetTypeCode(type);
			switch(typeCode)
			{
					// is it a basic type?
				case TypeCode.Boolean:
				case TypeCode.Byte:
				case TypeCode.Char:
				case TypeCode.DateTime:
				case TypeCode.DBNull:
				case TypeCode.Decimal:
				case TypeCode.Double:
				case TypeCode.Int16:
				case TypeCode.Int32:
				case TypeCode.Int64:
				case TypeCode.SByte:
				case TypeCode.Single:
				case TypeCode.String:
				case TypeCode.UInt16:
				case TypeCode.UInt32:
				case TypeCode.UInt64:
					return XmlPersistenceStrategy.ClrType;

					// is it an object?  look closer...
				case TypeCode.Object:
					
					// check for specific support...
					if(typeof(IToXml).IsAssignableFrom(type) == true)
						return XmlPersistenceStrategy.IToXml;
					if(typeof(DataSet).IsAssignableFrom(type) == true)
						return XmlPersistenceStrategy.DataSet;
					if(typeof(XmlNode).IsAssignableFrom(type) == true)
						return XmlPersistenceStrategy.XmlNode;

					// sledgehammer check...
					if(type.IsSerializable == true)
						return XmlPersistenceStrategy.Soap;
					
					// just dump the tostring output...
					return XmlPersistenceStrategy.ToString;

				default:
					throw ExceptionHelper.CreateCannotHandleException(typeCode);
			}
		}

		/// <summary>
		/// Asserts that the given object is valid.
		/// </summary>
		/// <param name="obj"></param>
		private void AssertObject(object obj)
		{
			if(obj == null)
				throw new ArgumentNullException("obj");
			if(this.Type.IsAssignableFrom(obj.GetType()) == false)
				throw new InvalidOperationException(string.Format(Cultures.Exceptions, "'{0}' is not assignable from '{1}'.", obj.GetType(), this.Type));
		}

		/// <summary>
		/// Gets the XML for the given object.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public string ToXml(object obj)
		{
			this.AssertObject(obj);
			
			// create...
			using(MemoryStream stream = new MemoryStream())
			{
				// defer...
				XmlTextWriter xml = new XmlTextWriter(stream, this.Encoding);
				xml.Formatting = Formatting.Indented;
				this.ToXml(obj, xml);

				// flush...
				xml.Flush();
				stream.Flush();

				// reead...
				stream.Position = 0;
				using(StreamReader reader = new StreamReader(stream, this.Encoding))
				{
					return reader.ReadToEnd();
				}
			}
		}

		/// <summary>
		/// ToXmls the given object.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public void ToXml(object obj, Stream stream)
		{
			this.AssertObject(obj);
			if(stream == null)
				throw new ArgumentNullException("stream");
			
			// defer...
			using(StreamWriter writer = new StreamWriter(stream, this.Encoding))
			{
				this.ToXml(obj, writer);
			}
		}

		/// <summary>
		/// ToXmls the given object.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public void ToXml(object obj, TextWriter writer)
		{
			this.AssertObject(obj);
			if(writer == null)
				throw new ArgumentNullException("writer");

			// create and defer...
			XmlTextWriter xml = new XmlTextWriter(writer);
			xml.Formatting = Formatting.Indented;
			this.ToXml(obj, xml);

			// flush...
			xml.Flush();
		}

		/// <summary>
		/// ToXmls the given object.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public void ToXml(object obj, XmlWriter xml)
		{
			this.AssertObject(obj);
			if(xml == null)
				throw new ArgumentNullException("xml");

			try
			{
				// get the type of the object to serialize.
				Type type = obj.GetType();

				// what's the strategy?
				if(Strategy == XmlPersistenceStrategy.ToString)
					xml.WriteString(Convert.ToString(obj, this.Culture));
				else if(Strategy == XmlPersistenceStrategy.IToXml)
					((IToXml)obj).ToXml(xml, this.ElementName, this.Encoding);
				else
				{
					// header...
					xml.WriteStartElement(this.ElementName);
					xml.WriteAttributeString(XmlHelper.ToXmlPrefix, "typeName", XmlHelper.ToXmlNamespaceUri, type.FullName);
					xml.WriteAttributeString(XmlHelper.ToXmlPrefix, "assembly", XmlHelper.ToXmlNamespaceUri, type.Assembly.FullName);
					xml.WriteAttributeString(XmlHelper.ToXmlPrefix, "strategy", XmlHelper.ToXmlNamespaceUri, ((object)Strategy).ToString());

					// do it...
					switch(Strategy)
					{
							// use the dataset directly...
						case XmlPersistenceStrategy.DataSet:
							((DataSet)obj).WriteXml(xml, XmlWriteMode.WriteSchema);
							break;

							// use the dataset directly...
						case XmlPersistenceStrategy.ClrType:
							WriteClrTypeValue(obj, xml);
							break;

							// write the contents of the XML...
						case XmlPersistenceStrategy.XmlNode:
							XmlNode node = (XmlNode)obj;
							xml.WriteAttributeString(XmlHelper.ToXmlPrefix, "xmlNodeType", XmlHelper.ToXmlNamespaceUri, node.NodeType.ToString());
							node.WriteContentTo(xml);
							break;

							// use the SOAP formatter...
						case XmlPersistenceStrategy.Soap:
							SerializeToXml(obj, xml);
							break;

						default:
							throw ExceptionHelper.CreateCannotHandleException(Strategy);
					}

					// write the footer...
					xml.WriteEndElement(); // this.ElementName
				}
			}
			catch(Exception ex)
			{
				throw new InvalidOperationException(string.Format(Cultures.Exceptions, "Failed to persist object of type '{0}'.", obj.GetType()), ex);
			}
		}

		/// <summary>
		/// Writes a basic CLR value.
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="xml"></param>
		private void WriteClrTypeValue(object obj, XmlWriter xml)
		{
			if(obj == null)
				throw new ArgumentNullException("obj");
			if(xml == null)
				throw new ArgumentNullException("xml");
			
			// write the type...
			xml.WriteAttributeString(XmlHelper.ToXmlPrefix, "typeCode", XmlHelper.ToXmlNamespaceUri, Type.GetTypeCode(obj.GetType()).ToString());

			// check for special types...
			if(obj is DateTime)
			{
                throw new NotImplementedException("This operation has not been implemented.");
			}
			else
				xml.WriteString(Convert.ToString(obj, this.Culture));
		}

		/// <summary>
		/// Serializes the object to the XML writer.
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="xml"></param>
		private void SerializeToXml(object obj, XmlWriter xml)
		{
			if(obj == null)
				throw new ArgumentNullException("obj");
			if(xml == null)
				throw new ArgumentNullException("xml");

			// save it...
			var formatter = new SoapFormatter();
			using(MemoryStream stream = new MemoryStream())
			{
				// save...
				formatter.Serialize(stream, obj);

				// flush and reset...
				stream.Flush();
				stream.Position = 0;

				using(StreamReader reader = new StreamReader(stream, Encoding.ASCII))
				{
					// xml...
					string xmlAsString = reader.ReadToEnd();

					// write...
					xml.WriteRaw(xmlAsString);
				}
			}
		}

		/// <summary>
		/// Gets the strategy.
		/// </summary>
		public XmlPersistenceStrategy Strategy
		{
			get
			{
				return _strategy;
			}
		}

		/// <summary>
		/// Gets or sets the culture
		/// </summary>
		public CultureInfo Culture
		{
			get
			{
				return _culture;
			}
			set
			{
				// check to see if the value has changed...
				if(value != _culture)
				{
					// set the value...
					_culture = value;
				}
			}
		}
	}
}

