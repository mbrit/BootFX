// BootFX - Application framework for .NET applications
// 
// File: ToXmlBase.cs
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
using BootFX.Common.Management;

namespace BootFX.Common.Xml
{
	/// <summary>
	/// Defines an object that supports IToXml.
	/// </summary>
	[Serializable()]
	public abstract class ToXmlBase : IToXml
	{
		/// <summary>
		/// Contructor.
		/// </summary>
		protected ToXmlBase()
		{
		}

		/// <summary>
		/// Gets the default element name.
		/// </summary>
		string IToXml.DefaultElementName
		{
			get
			{
				return this.DefaultElementName;
			}
		}

		/// <summary>
		/// Gets the default element name.
		/// </summary>
		/// <remarks>By default, this returns the name of the type.  (Not the full name, e.g. DataSet, not System.Data.DataSet.)</remarks>
		protected virtual string DefaultElementName
		{
			get
			{
				return this.GetType().Name;
			}
		}

		/// <summary>
		/// Gets the default encoding method.
		/// </summary>
		protected virtual Encoding DefaultEncoding
		{
			get
			{
				return Encoding.Unicode;
			}
		}

		/// <summary>
		/// Gets the XML representation of the object.
		/// </summary>
		/// <param name="xml"></param>
		/// <param name="elementName"></param>
		public string ToXml()
		{
			return this.ToXml(this.DefaultElementName, this.DefaultEncoding);
		}

		/// <summary>
		/// Gets the XML representation of the object.
		/// </summary>
		/// <param name="xml"></param>
		/// <param name="elementName"></param>
		public string ToXml(string elementName)
		{
			return this.ToXml(elementName, this.DefaultEncoding);
		}

		/// <summary>
		/// Gets the XML representation of the object.
		/// </summary>
		/// <param name="xml"></param>
		/// <param name="elementName"></param>
		public string ToXml(Encoding encoding)
		{
			if(encoding == null)
				throw new ArgumentNullException("encoding");

			// defer...
			return this.ToXml(this.DefaultElementName, encoding);
		}

		/// <summary>
		/// Gets the XML representation of the object.
		/// </summary>
		/// <returns></returns>
		public string ToXml(string elementName, Encoding encoding)
		{
			if(elementName == null)
				throw new ArgumentNullException("elementName");
			if(elementName.Length == 0)
				throw ExceptionHelper.CreateZeroLengthArgumentException("elementName");
			if(encoding == null)
				throw new ArgumentNullException("encoding");
			
			// create a memory stream... can't use a StringStream for this as it has problems
			// with the encoding...
			using(MemoryStream stream = new MemoryStream())
			{
				// create a writer...
				XmlTextWriter writer = new XmlTextWriter(stream, encoding);
				writer.Formatting = Formatting.Indented;
				this.ToXml(writer);

				// flush...
				writer.Flush();
				stream.Flush();

				// rewind and get...
				stream.Position = 0;
				using(StreamReader reader = new StreamReader(stream, encoding))
				{
					return reader.ReadToEnd();
				}
			}
		}

		/// <summary>
		/// Gets the XML representation of the object.
		/// </summary>
		/// <param name="xml"></param>
		/// <param name="elementName"></param>
		public void ToXml(System.Xml.XmlWriter xml)
		{
			if(xml == null)
				throw new ArgumentNullException("xml");

			// defer...
			this.ToXml(xml, this.DefaultElementName, this.DefaultEncoding);
		}

		/// <summary>
		/// Gets the XML representation of the object.
		/// </summary>
		/// <param name="xml"></param>
		/// <param name="elementName"></param>
		public void ToXml(System.Xml.XmlWriter xml, string elementName)
		{
			if(xml == null)
				throw new ArgumentNullException("xml");
			if(elementName == null)
				throw new ArgumentNullException("elementName");
			if(elementName.Length == 0)
				throw ExceptionHelper.CreateZeroLengthArgumentException("elementName");
			
			// defer...
			this.ToXml(xml, elementName, this.DefaultEncoding);
		}

		/// <summary>
		/// Gets the XML representation of the object.
		/// </summary>
		/// <param name="xml"></param>
		/// <param name="elementName"></param>
		public void ToXml(System.Xml.XmlWriter xml, Encoding encoding)
		{
			if(xml == null)
				throw new ArgumentNullException("xml");
			if(encoding == null)
				throw new ArgumentNullException("encoding");

			// defer...
			this.ToXml(xml, this.DefaultElementName, encoding);
		}

		/// <summary>
		/// Gets the XML representation of the object.
		/// </summary>
		/// <param name="xml"></param>
		/// <param name="elementName"></param>
		public virtual void ToXml(System.Xml.XmlWriter xml, string elementName, Encoding encoding)
		{
			if(xml == null)
				throw new ArgumentNullException("xml");
			if(elementName == null)
				throw new ArgumentNullException("elementName");
			if(elementName.Length == 0)
				throw ExceptionHelper.CreateZeroLengthArgumentException("elementName");
			if(encoding == null)
				throw new ArgumentNullException("encoding");
			
			// create a header element...
			xml.WriteStartElement(elementName);

			// attributes...
			Type type = this.GetType();
			xml.WriteAttributeString("two47", "typeName", XmlHelper.ToXmlNamespaceUri, type.FullName);
			xml.WriteAttributeString("two47", "assembly", XmlHelper.ToXmlNamespaceUri, type.Assembly.FullName);
			xml.WriteAttributeString("two47", "strategy", XmlHelper.ToXmlNamespaceUri, ((object)XmlPersistenceStrategy.IToXml).ToString());

			// write...
			WriteXmlContext context = new WriteXmlContext(encoding);
			this.WriteXml(xml, context);

			// footer...
			xml.WriteEndElement(); // elementName
		}

		/// <summary>
		/// Continues generating to the same context as another object.
		/// </summary>
		/// <param name="xml"></param>
		/// <param name="context"></param>
		public void ToXml(XmlWriter xml, WriteXmlContext context)
		{
			if(xml == null)
				throw new ArgumentNullException("xml");
			if(context == null)
				throw new ArgumentNullException("context");
			
			// defer...
			this.ToXml(xml, context.Encoding);
		}

		/// <summary>
		/// Writes the XML.
		/// </summary>
		/// <param name="xml"></param>
		/// <remarks>At this point, a header element has been created.</remarks>
		protected abstract void WriteXml(XmlWriter xml, WriteXmlContext context);		

		/// <summary>
		/// Gets the object as an <c>XmlDocument</c>.
		/// </summary>
		/// <returns></returns>
		public XmlDocument ToXmlDocument()
		{
			return this.ToXmlDocument(this.DefaultElementName);
		}

		/// <summary>
		/// Gets the object as an <c>XmlDocument</c>.
		/// </summary>
		/// <returns></returns>
		public XmlDocument ToXmlDocument(string firstElementName)
		{
			if(firstElementName == null)
				throw new ArgumentNullException("firstElementName");
			if(firstElementName.Length == 0)
				throw ExceptionHelper.CreateZeroLengthArgumentException("firstElementName");
			
			// get it, then load it back up...
			string xml = this.ToXml(firstElementName);

			// create, load, return...
			XmlDocument document = new XmlDocument();
			document.LoadXml(xml);
			return document;
		}

		/// <summary>
		/// Saves the XML to the given file.
		/// </summary>
		/// <param name="filename"></param>
		public void SaveXml(string filePath)
		{
			if(filePath == null)
				throw new ArgumentNullException("filePath");
			if(filePath.Length == 0)
				throw ExceptionHelper.CreateZeroLengthArgumentException("filePath");
			
			// defer...
			this.SaveXml(filePath, this.DefaultEncoding);
		
		}

		/// <summary>
		/// Saves the XML to the given file.
		/// </summary>
		/// <param name="filename"></param>
		public void SaveXml(string filePath, Encoding encoding)
		{
			if(filePath == null)
				throw new ArgumentNullException("filePath");
			if(filePath.Length == 0)
				throw ExceptionHelper.CreateZeroLengthArgumentException("filePath");
			if(encoding == null)
				throw new ArgumentNullException("encoding");

			// readonly check...
			if(File.Exists(filePath) && (File.GetAttributes(filePath) & FileAttributes.ReadOnly) != 0)
				throw new InvalidOperationException(string.Format("The file '{0}' is read-only.", filePath));

			// create a stream...
			using(FileStream stream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
			{
				SaveXml(stream, encoding);
			}
		}

		/// <summary>
		/// Saves XML to the given stream.
		/// </summary>
		/// <param name="stream"></param>
		/// <param name="encoding"></param>
		public void SaveXml(Stream stream, Encoding encoding)
		{
			if(stream == null)
				throw new ArgumentNullException("stream");
			if(encoding == null)
				throw new ArgumentNullException("encoding");
			
			// create...
			XmlTextWriter xml = new XmlTextWriter(stream, encoding);
			xml.Formatting = Formatting.Indented;
			this.ToXml(xml);

			// flush...
			xml.Flush();
		}

		/// <summary>
		/// Saves XML to the given writer.
		/// </summary>
		/// <param name="stream"></param>
		/// <param name="encoding"></param>
		public void SaveXml(TextWriter writer)
		{
            if (writer == null)
                throw new ArgumentNullException("writer");
			
			// save...
			XmlTextWriter xml = new XmlTextWriter(writer);
			xml.Formatting = Formatting.Indented;
			this.ToXml(xml);

			// flush...
			xml.Flush();
		}
	}
}

