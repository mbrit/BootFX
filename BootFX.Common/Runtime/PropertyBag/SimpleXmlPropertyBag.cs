// BootFX - Application framework for .NET applications
// 
// File: SimpleXmlPropertyBag.cs
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
using System.Runtime.Serialization;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using BootFX.Common.Xml;
using BootFX.Common.Management;

namespace BootFX.Common
{
	/// <summary>
	/// Defines a property bag that contains readable XML - i.e. single element, single value.
	/// </summary>
	[Serializable()]
	public class SimpleXmlPropertyBag : PropertyBag
	{
		private const string isArrayAttributeName = "isArray";
		
		/// <summary>
		/// Raised when the property bag has been saved.
		/// </summary>
		public event EventHandler Saved;
		
		/// <summary>
		/// Raised before the save operation runs.
		/// </summary>
		public event EventHandler Saving;
		
		/// <summary>
		/// Raised when the load from XML has been completed.
		/// </summary>
		public event EventHandler LoadComplete;
		
		/// <summary>
		/// Constructor.
		/// </summary>
		public SimpleXmlPropertyBag()
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		protected SimpleXmlPropertyBag(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		/// <summary>
		/// Loads a bag from an XML element.
		/// </summary>
		/// <param name="element"></param>
		/// <returns></returns>
		public static SimpleXmlPropertyBag Load(TextReader reader, Type type)
		{
			if(reader == null)
				throw new ArgumentNullException("reader");
			if(type == null)
				throw new ArgumentNullException("type");
			
			// create...
			XmlDocument doc = new XmlDocument();
			doc.Load(reader);

			// return...
			return Load(doc, type);
		}

		/// <summary>
		/// Loads a bag from an XML element.
		/// </summary>
		/// <param name="element"></param>
		/// <returns></returns>
		public static SimpleXmlPropertyBag Load(string filePath, Type type)
		{
			if(filePath == null)
				throw new ArgumentNullException("filePath");
			if(filePath.Length == 0)
				throw new ArgumentOutOfRangeException("'filePath' is zero-length.");
			if(type == null)
				throw new ArgumentNullException("type");
			
			// create...
			XmlDocument doc = new XmlDocument();
			doc.Load(filePath);

			// return...
			return Load(doc, type);
		}

		/// <summary>
		/// Loads a bag from an XML element.
		/// </summary>
		/// <param name="element"></param>
		/// <returns></returns>
		public static SimpleXmlPropertyBag Load(Stream stream, Type type)
		{
			if(stream == null)
				throw new ArgumentNullException("stream");
			if(type == null)
				throw new ArgumentNullException("type");
			
			// create...
			XmlDocument doc = new XmlDocument();
			doc.Load(stream);

			// return...
			return Load(doc, type);
		}

		/// <summary>
		/// Loads a bag from an XML element.
		/// </summary>
		/// <param name="element"></param>
		/// <returns></returns>
		public static SimpleXmlPropertyBag Load(XmlDocument document, Type type)
		{
			if(document == null)
				throw new ArgumentNullException("document");
			if(type == null)
				throw new ArgumentNullException("type");

			// find...
			foreach(XmlNode node in document.ChildNodes)
			{
				if(node is XmlElement)
					
					return LoadInternal(node, type);
			}

			// throw...
			throw new InvalidOperationException("The document doesd not have a container element.");
		}

		/// <summary>
		/// Loads a bag from an XML element.
		/// </summary>
		/// <param name="element"></param>
		/// <returns></returns>
		public static SimpleXmlPropertyBag Load(XmlElement element, Type type)
		{
			if(element == null)
				throw new ArgumentNullException("element");
			if(type == null)
				throw new ArgumentNullException("type");
			return LoadInternal(element, type);
		}

		/// <summary>
		/// Loads a bag from an XML element.
		/// </summary>
		/// <param name="element"></param>
		/// <returns></returns>
		private static SimpleXmlPropertyBag LoadInternal(XmlNode node, Type type)
		{
			if(node == null)
				throw new ArgumentNullException("node");
			if(type == null)
				throw new ArgumentNullException("type");

			// create the bag...
			SimpleXmlPropertyBag results = (SimpleXmlPropertyBag)Activator.CreateInstance(type, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, 
				null, null, System.Globalization.CultureInfo.InvariantCulture);

			// walk...
			foreach(XmlNode child in node.ChildNodes)
			{
				bool isArray = XmlHelper.GetAttributeBoolean(child,isArrayAttributeName,OnNotFound.ReturnNull);
				
				// do we have an element?
				if(child is XmlElement)
				{
					if(isArray == false)
						results[child.Name] = XmlHelper.GetElementValue((XmlElement)child);
					else
					{
						if(child.ChildNodes.Count == 0)
							results[child.Name] = null;
						else
						{
							ArrayList arrayItems = new ArrayList();
							for(int index = 0; index < child.ChildNodes.Count; index++)
								arrayItems.Add(XmlHelper.GetElementValue((XmlElement)child.ChildNodes[index]));
							
							results[child.Name] = arrayItems.ToArray(arrayItems[0].GetType());
						}
					}
				}
			}

			// mbr - 20-10-2005 - added...			
			results.OnLoadComplete();

			// return...
			return results;
		}

		/// <summary>
		/// Saves the property bag to the given file.
		/// </summary>
		/// <param name="filePath"></param>
		/// <param name="topElementName"></param>
		public void Save(string filePath, string topElementName, bool overwrite)
		{
			if(filePath == null)
				throw new ArgumentNullException("filePath");
			if(filePath.Length == 0)
				throw new ArgumentOutOfRangeException("'filePath' is zero-length.");
			if(topElementName == null)
				throw new ArgumentNullException("topElementName");
			if(topElementName.Length == 0)
				throw new ArgumentOutOfRangeException("'topElementName' is zero-length.");

			// exists...
			if(File.Exists(filePath))
			{
				if(!(overwrite))
					throw new InvalidOperationException(string.Format("File '{0}' already exists.", filePath));
			}

			// create a new xml document...
			XmlDocument doc = this.ToXmlDocument(topElementName);
			if(doc == null)
				throw new InvalidOperationException("doc is null.");

			// save...
			doc.Save(filePath);

			// mbr - 28-05-2008 - moved this event to run within the Save method (so that we always get a chance
			// to update internal state before the save operation runs).
			//			this.OnSaved();
		}

		/// <summary>
		/// Saves the property bag to a new XML document.
		/// </summary>
		/// <param name="topElementName"></param>
		/// <returns></returns>
		private XmlDocument ToXmlDocument(string topElementName)
		{
			if(topElementName == null)
				throw new ArgumentNullException("topElementName");
			if(topElementName.Length == 0)
				throw new ArgumentOutOfRangeException("'topElementName' is zero-length.");
			
			// create...
			XmlDocument doc = new XmlDocument();
			XmlNamespaceManagerEx manager = XmlHelper.GetNamespaceManagerEx(doc);
			if(manager == null)
				throw new InvalidOperationException("manager is null.");

			// create...
			XmlElement element = doc.CreateElement(topElementName);
			doc.AppendChild(element);

			// save...
			this.Save(element, manager, SimpleXmlSaveMode.ReplaceExisting);

			// return...
			return doc;
		}

		/// <summary>
		/// Saves the property bag to a new XML document.
		/// </summary>
		/// <param name="topElementName"></param>
		/// <returns></returns>
		[Obsolete("Use the version that requires an XmlNamespaceManagerEx.")]
		public void Save(XmlElement element, SimpleXmlSaveMode mode)
		{
			this.Save(element, XmlHelper.GetNamespaceManagerEx(element.OwnerDocument), mode);
		}

		/// <summary>
		/// Saves the property bag to a new XML document.
		/// </summary>
		/// <param name="topElementName"></param>
		/// <returns></returns>
		public void Save(XmlElement element, XmlNamespaceManagerEx manager, SimpleXmlSaveMode mode)
		{
			if(element == null)
				throw new ArgumentNullException("element");
			if(manager == null)
				throw new ArgumentNullException("manager");			

			// raise...
			this.OnSaving();
			
			// clear?
			if(mode == SimpleXmlSaveMode.ClearAllChildren)
			{
				while(element.FirstChild != null)
					element.RemoveChild(element.FirstChild);
			}

			// mbr - 20-10-2005 - to do this, we want to sort the names so that we are always serializing in a consistent order...			
			ArrayList names = new ArrayList(this.InnerValues.Keys);
			names.Sort();

			// walk and add items...
			foreach(string name in names)
			{
				// find a child with the name...
				object value = this.InnerValues[name];
				try
				{
					if((mode & SimpleXmlSaveMode.UseAttributes) != 0)
						this.SaveValueAsAttribute(element, name, value);
					else
						this.SaveValueAsElement(element, name, value, mode);
				}
				catch(Exception ex)
				{
					throw new InvalidOperationException(string.Format("Failed to append value '{0}'.  ({1})", name, value), ex);
				}
			}

			// raise...
			this.OnSaved();
		}

		/// <summary>
		/// Raises the <c>Saving</c> event.
		/// </summary>
		private void OnSaving()
		{
			OnSaving(EventArgs.Empty);
		}
		
		/// <summary>
		/// Raises the <c>Saving</c> event.
		/// </summary>
		protected virtual void OnSaving(EventArgs e)
		{
			// raise...
			if(Saving != null)
				Saving(this, e);
		}

		private void SaveValueAsAttribute(XmlElement element, string name, object value)
		{
			if(element == null)
				throw new ArgumentNullException("element");
			if(name == null)
				throw new ArgumentNullException("name");
			if(name.Length == 0)
				throw new ArgumentOutOfRangeException("'name' is zero-length.");
			
			// set...
			XmlHelper.SetAttributeValue(element, name, value);
		}

		private void SaveValueAsElement(XmlElement element, string name, object value, SimpleXmlSaveMode mode)
		{
			if(element == null)
				throw new ArgumentNullException("element");			
			if(name == null)
				throw new ArgumentNullException("name");
			if(name.Length == 0)
				throw new ArgumentOutOfRangeException("'name' is zero-length.");

			XmlElement child = null;

			// load the existing one if we're merging...
			if(mode == SimpleXmlSaveMode.ReplaceExisting)
				child = (XmlElement)element.SelectSingleNode(name);

			// create one if we need one...
			if(child == null)
			{
				child = element.OwnerDocument.CreateElement(name);
				element.AppendChild(child);
			}

			// set...
			if(!(value is MissingItem))
			{
				// mbr - 10-05-2006 - added null check.						
				if(value == null || !(value.GetType().IsArray))
					XmlHelper.SetElementValue(child, value);
				else
				{
					XmlHelper.SetAttributeValue(child,isArrayAttributeName,true);
							
					// We have an array so we will walk the array
					foreach(object elementValue in (Array) value)
					{
						XmlElement childElementValue = element.OwnerDocument.CreateElement("value");
						child.AppendChild(childElementValue);
								
						XmlHelper.SetElementValue(childElementValue, elementValue);
					}
				}
			}
		}

		/// <summary>
		/// Saves the object to a stream.
		/// </summary>
		/// <param name="stream"></param>
		/// <param name="topElementName"></param>
		public void Save(TextWriter writer, string topElementName)
		{
			if(writer == null)
				throw new ArgumentNullException("writer");
			if(topElementName == null)
				throw new ArgumentNullException("topElementName");
			if(topElementName.Length == 0)
				throw new ArgumentOutOfRangeException("'topElementName' is zero-length.");
			
			// save...
			XmlDocument document = this.ToXmlDocument(topElementName);
			if(document == null)
				throw new InvalidOperationException("document is null.");

			// write...
            try
            {
                document.Save(writer);
            }
            catch(Exception ex)
            {
                throw HandleSaveException(ex);
            }
		}

		/// <summary>
		/// Saves the object to a stream.
		/// </summary>
		/// <param name="stream"></param>
		/// <param name="topElementName"></param>
		public void Save(Stream stream, string topElementName)
		{
			if(stream == null)
				throw new ArgumentNullException("stream");
			if(topElementName == null)
				throw new ArgumentNullException("topElementName");
			if(topElementName.Length == 0)
				throw new ArgumentOutOfRangeException("'topElementName' is zero-length.");
			
			// save...
			XmlDocument document = this.ToXmlDocument(topElementName);
			if(document == null)
				throw new InvalidOperationException("document is null.");

			// write...
            try
            {
    			document.Save(stream);
            }
            catch(Exception ex)
            {
                throw HandleSaveException(ex);
            }
		}

		/// <summary>
		/// Raises the <c>Saved</c> event.
		/// </summary>
		private void OnSaved()
		{
			OnSaved(EventArgs.Empty);
		}
		
		/// <summary>
		/// Raises the <c>Saved</c> event.
		/// </summary>
		protected virtual void OnSaved(EventArgs e)
		{
			// raise...
			if(Saved != null)
				Saved(this, e);
		}

		/// <summary>
		/// Gets the object as an XML string.
		/// </summary>
		/// <returns></returns>
		public string ToXml(string topElementName)
		{
			if(topElementName == null)
				throw new ArgumentNullException("topElementName");
			if(topElementName.Length == 0)
				throw new ArgumentOutOfRangeException("'topElementName' is zero-length.");
			
            // mbr - 2009-02-15 - something is wrong here with the encoding, so changed to return outerxml...
            //// writer...
            //using(StringWriter writer = new StringWriter())
            //{
            //    this.Save(writer, topElementName);

            //    // flush...
            //    writer.Flush();
            //    return writer.GetStringBuilder().ToString();
            //}

            // get...
            XmlDocument doc = this.ToXmlDocument(topElementName);
            if (doc == null)
                throw new InvalidOperationException("'doc' is null.");

            // return...
            try
            {
                return doc.OuterXml;
            }
            catch (Exception ex)
            {
                throw HandleSaveException(ex);
            }
		}

        private Exception HandleSaveException(Exception ex)
        {
            // log...
            FileLog log = LogSet.CreateFileLogger("Simple XML Dump Failure", FileLoggerFlags.Default, new GenericFormatter());

            // dump...
            log.Error("Original error...", ex);

            // keys...
            List<object> testKeys = new List<object>();
            foreach (object key in Keys)
                testKeys.Add(key);

            // while...
            object failKey = null;
            while (testKeys.Count > 0)
            {
                // also, go through and create a document with each value in it and see if you can suss it that way...
                XmlDocument testDoc = new XmlDocument();
                XmlElement root = testDoc.CreateElement("Root");
                testDoc.AppendChild(root);
                foreach (string key in testKeys)
                    this.SaveValueAsElement(root, key, this.InnerValues[key], SimpleXmlSaveMode.ReplaceExisting);

                // out...
                try
                {
                    string buf = testDoc.OuterXml;
                    if (buf == null)
                        throw new InvalidOperationException("'buf' is null.");
                    if (buf.Length == 0)
                        throw new InvalidOperationException("'buf' is zero-length.");

                    // ok...
                    if (failKey == null)
                        log.Info("OK - fail key was CLR null.");
                    else
                    {
                        log.InfoFormat("OK - fail key was: {0}", failKey);
                        
                        // value...
                        object failValue = this.InnerValues[failKey];
                        log.InfoFormat("Fail value was: {0}", failValue);
                        
                        // builder...
                        if (failValue is string)
                        {
                            StringBuilder builder = new StringBuilder();
                            string failString = (string)failValue;
                            for(int index = 0; index < failString.Length; index++)
                            {
                                if (index > 0 && index % 16 == 0)
                                    builder.Append("\r\n");
                                builder.Append(((int)failString[index]).ToString("x"));
                                builder.Append(" ");
                            }

                            // show...
                            log.InfoFormat("Fail value hex dump:\r\n{0}", builder);
                        }
                    }

                    // stop...
                    break;
                }
                catch (Exception miniEx)
                {
                    log.Info(string.Format("Double-check failed ({0}).\r\n-------------------------------------", testKeys.Count), miniEx);
                }

                // remove...
                failKey = testKeys[0];
                log.InfoFormat("Key '{0}' removed...", failKey);
                testKeys.RemoveAt(0);
            }

            // dump...
            return new InvalidOperationException(string.Format("An error occurred when saving the property bag to XML.  A log file was written to: {0}", log.Path), ex);
        }

		public static SimpleXmlPropertyBag FromXml(string xml, Type type)
		{
			// load...
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(xml);

			// defer...
			return Load(doc, type);
		}

		/// <summary>
		/// Raises the <c>LoadComplete</c> event.
		/// </summary>
		private void OnLoadComplete()
		{
			OnLoadComplete(EventArgs.Empty);
		}
		
		/// <summary>
		/// Raises the <c>LoadComplete</c> event.
		/// </summary>
		protected virtual void OnLoadComplete(EventArgs e)
		{
			// raise...
			if(LoadComplete != null)
				LoadComplete(this, e);
		}
	}
}
