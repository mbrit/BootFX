// BootFX - Application framework for .NET applications
// 
// File: SqlSearchSettings.cs
// Build: 5.2.10321.2307
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
using System.Text.RegularExpressions;
using System.Collections;
using BootFX.Common.Xml;
using BootFX.Common.Entities;
using BootFX.Common.Management;

namespace BootFX.Common.Data
{
	/// <summary>
	/// Defines an instance of <c>SearchSettings</c>.
	/// </summary>
	public class SqlSearchSettings : SqlFilterSource, IEntityType
	{
		private const string RootElementName = "SqlSearchSettings";

		/// <summary>
		/// Raised after the object has been deserialized from XML.
		/// </summary>
		public event EventHandler AfterXmlDeserialization;
		
		/// <summary>
		/// Private field to support <see cref="EntityType"/> property.
		/// </summary>
		private EntityType _entityType;
		
		/// <summary>
		/// Executed before the document is serialized to XML.
		/// </summary>
		public event EventHandler BeforeXmlSerialization;
		
		/// <summary>
		/// Private field to support <c>Sources</c> property.
		/// </summary>
		private FilterManglerCollection _manglers = new FilterManglerCollection();
		
		/// <summary>
		/// Private field to support <see cref="Settings"/> property.
		/// </summary>
		private SimpleXmlPropertyBag _settings = new SimpleXmlPropertyBag();

		protected SqlSearchSettings()
		{
		}

		public SqlSearchSettings(Type type) : this(EntityType.GetEntityType(type, OnNotFound.ThrowException))
		{
		}

		public SqlSearchSettings(EntityType entityType)
		{
			if(entityType == null)
				throw new ArgumentNullException("entityType");
			_entityType = entityType;
		}

		/// <summary>
		/// Gets the entitytype.
		/// </summary>
		public EntityType EntityType
		{
			get
			{
				return _entityType;
			}
		}

		/// <summary>
		/// Gets the settings.
		/// </summary>
		public PropertyBag Settings
		{
			get
			{
				return _settings;
			}
		}

		private SimpleXmlPropertyBag SettingsAsSimpleXmlPropertyBag
		{
			get
			{
				return (SimpleXmlPropertyBag)_settings;
			}
		}

		/// <summary>
		/// Gets a collection of ISearchConstraintSource objects.
		/// </summary>
		public FilterManglerCollection Manglers
		{
			get
			{
				return _manglers;
			}
		}

		/// <summary>
		/// Gets the settings as an XML string.
		/// </summary>
		/// <returns></returns>
		public string ToXml()
		{
			XmlDocument doc = this.ToXmlDocument();
			if(doc == null)
				throw new InvalidOperationException("doc is null.");

			// save...
			using(StringWriter writer = new StringWriter())
			{
				doc.Save(writer);

				// flush...
				writer.Flush();
				return writer.GetStringBuilder().ToString();
			}
		}

		public string ToBase64Xml()
		{
			string asString = this.ToXml();
			if(asString == null)
				throw new InvalidOperationException("'asString' is null.");
			if(asString.Length == 0)
				throw new InvalidOperationException("'asString' is zero-length.");

			// return...
			return Convert.ToBase64String(Encoding.Unicode.GetBytes(asString));
		}

		/// <summary>
		/// Raises the <c>BeforeXmlSerialization</c> event.
		/// </summary>
		private void OnBeforeXmlSerialization()
		{
			OnBeforeXmlSerialization(EventArgs.Empty);
		}
		
		/// <summary>
		/// Raises the <c>BeforeXmlSerialization</c> event.
		/// </summary>
		protected virtual void OnBeforeXmlSerialization(EventArgs e)
		{
			// raise...
			if(BeforeXmlSerialization != null)
				BeforeXmlSerialization(this, e);
		}

		private void AddTypeAttribute(XmlElement element, Type type)
		{
			if(element == null)
				throw new ArgumentNullException("element");
			if(type == null)
				throw new ArgumentNullException("type");
			
			// add...
			XmlAttribute typeAttr = element.OwnerDocument.CreateAttribute("type");
			element.Attributes.Append(typeAttr);
			typeAttr.InnerText = string.Format("{0}, {1}", type.FullName, type.Assembly.GetName().Name);
		}

		/// <summary>
		/// Gets the XML for these settings.
		/// </summary>
		/// <returns></returns>
		public XmlDocument ToXmlDocument()
		{
			this.OnBeforeXmlSerialization();

			if(EntityType == null)
				throw new InvalidOperationException("EntityType is null.");

			// create...
			XmlDocument doc = new XmlDocument();

			// mbr - 22-09-2006 - manager...
			XmlNamespaceManagerEx manager = XmlHelper.GetNamespaceManagerEx(doc);
			if(manager == null)
				throw new InvalidOperationException("manager is null.");

			// root...
			XmlElement rootElement = doc.CreateElement(RootElementName);
			doc.AppendChild(rootElement);

			// type...
			this.AddTypeAttribute(rootElement, this.GetType());

			// entity type...
			XmlElement etElement = doc.CreateElement("EntityType");
			rootElement.AppendChild(etElement);
			etElement.InnerText = string.Format("{0}, {1}", this.EntityType.Type.FullName, this.EntityType.Type.Assembly.GetName().Name);

			// sources?
			XmlElement manglersElement = doc.CreateElement("Manglers");
			rootElement.AppendChild(manglersElement);

			// sources...
			foreach(FilterMangler mangler in this.Manglers)
			{
				if(mangler.ManglerId == null)
					throw new InvalidOperationException("'mangler.ManglerId' is null.");
				if(mangler.ManglerId.Length == 0)
					throw new InvalidOperationException("'mangler.ManglerId' is zero-length.");

				// owner...
				XmlElement manglerElement = doc.CreateElement("Mangler");
				manglersElement.AppendChild(manglerElement);

				// type...
				XmlAttribute manglerIdAttr = doc.CreateAttribute("manglerId");
				manglerIdAttr.Value = mangler.ManglerId;
				manglerElement.Attributes.Append(manglerIdAttr);

				if(mangler.Filter != null)
					this.AddTypeAttribute(manglerElement, mangler.Filter.GetType());

				// settings...
				SimpleXmlPropertyBag bag = new SimpleXmlPropertyBag();
				mangler.Filter.SerializeSqlSearchSettings(bag);

				// add...
				this.SerializeSettings(manglerElement, manager, bag);
			}

			// settings...
			if(this.Settings.Count > 0)
				this.SerializeSettings(rootElement, manager, this.SettingsAsSimpleXmlPropertyBag);

			// return...
			return doc;
		}

		private void SerializeSettings(XmlElement parentElement, XmlNamespaceManagerEx manager, SimpleXmlPropertyBag bag)
		{
			if(parentElement == null)
				throw new ArgumentNullException("parentElement");
			if(bag == null)
				throw new ArgumentNullException("bag");
			
			// add...
			XmlElement element = parentElement.OwnerDocument.CreateElement("Settings");
			parentElement.AppendChild(element);

			// save...
			bag.Save(element, manager, SimpleXmlSaveMode.ReplaceExisting);
		}

		protected virtual void MangleFilter(SqlFilter filter)
		{
			if(filter == null)
				throw new ArgumentNullException("filter");			

			// walk...
			foreach(FilterMangler mangler in this.Manglers)
				mangler.Filter.MangleFilter(filter);
		}

		/// <summary>
		/// Gets the filter.
		/// </summary>
		/// <returns></returns>
		public override SqlFilter GetSqlFilter()
		{
			if(EntityType == null)
				throw new InvalidOperationException("EntityType is null.");
			SqlFilter filter = new SqlFilter(this.EntityType);

			// mangle...
			MangleFilter(filter);

			// return...
			return filter;
		}

		public static SqlSearchSettings FromBase64Xml(string xmlAsBase64)
		{
			if(xmlAsBase64 == null)
				throw new ArgumentNullException("xmlAsBase64");
			if(xmlAsBase64.Length == 0)
				throw new ArgumentOutOfRangeException("'xmlAsBase64' is zero-length.");
			
			// return...
			string xml = Encoding.Unicode.GetString(Convert.FromBase64String(xmlAsBase64));
			if(xml == null)
				throw new InvalidOperationException("'xml' is null.");
			if(xml.Length == 0)
				throw new InvalidOperationException("'xml' is zero-length.");

			// defer...
			return FromXml(xml);
		}

		public static SqlSearchSettings FromXml(string xml)
		{
			if(xml == null)
				throw new ArgumentNullException("xml");
			if(xml.Length == 0)
				throw new ArgumentOutOfRangeException("'xml' is zero-length.");
			
			// doc...
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(xml);

			// defer...
			return FromXml(doc);
		}

		/// <summary>
		/// Creates an instance (with exception reporting).
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		private static object CreateInstance(Type type)
		{
			if(type == null)
				throw new ArgumentNullException("type");
			
			try
			{
				return Activator.CreateInstance(type, true);
			}
			catch(Exception ex)
			{
				throw new InvalidOperationException(string.Format("Failed to create an instance of '{0}'.", type), ex);
			}
		}

		public static SqlSearchSettings FromXml(XmlDocument doc)
		{
			if(doc == null)
				throw new ArgumentNullException("doc");
		
			// root...
			XmlElement rootElement = (XmlElement)doc.SelectSingleNode(RootElementName);
			if(rootElement == null)
				throw new InvalidOperationException("The root element was not found.");

			// create...
			Type type = XmlHelper.GetAttributeType(rootElement, "type", OnNotFound.ThrowException);
			if(type == null)
				throw new InvalidOperationException("type is null.");

			// create one...
			SqlSearchSettings newSettings = (SqlSearchSettings)CreateInstance(type);
			if(newSettings == null)
				throw new InvalidOperationException("newSettings is null.");

			// load the entity type...
			type = XmlHelper.GetElementType(rootElement, "EntityType", OnNotFound.ThrowException, true);
			if(type == null)
				throw new InvalidOperationException("type is null.");
			newSettings._entityType = EntityType.GetEntityType(type, OnNotFound.ThrowException);
			
			// load the manglers...
			foreach(XmlElement manglerElement in rootElement.SelectNodes("Manglers/Mangler"))
			{
				FilterMangler mangler = LoadMangler(newSettings, manglerElement);
				if(mangler == null)
					throw new InvalidOperationException("mangler is null.");

				// add...
				newSettings.Manglers.Add(mangler);
			}

			// load the settings...
			XmlElement settingsElement = (XmlElement)rootElement.SelectSingleNode("Settings");
			if(settingsElement == null)
				throw new InvalidOperationException("settingsElement is null.");
			newSettings.LoadSettings(settingsElement);

			// deser...
			newSettings.OnAfterXmlDeserialization();

			// return...
			return newSettings;
		}

		private static FilterMangler LoadMangler(SqlSearchSettings settings, XmlElement element)
		{
			if(settings == null)
				throw new ArgumentNullException("settings");
			if(element == null)
				throw new ArgumentNullException("element");
			
			// get the type...
			string manglerId = XmlHelper.GetAttributeString(element, "manglerId", OnNotFound.ThrowException);
			if(manglerId == null)
				throw new InvalidOperationException("'manglerId' is null.");
			if(manglerId.Length == 0)
				throw new InvalidOperationException("'manglerId' is zero-length.");

			// get the type...
			Type type = XmlHelper.GetAttributeType(element, "type", OnNotFound.ThrowException);
			if(type == null)
				throw new InvalidOperationException("type is null.");

			// create...
			ISqlFilterMangler filter = (ISqlFilterMangler)CreateInstance(type);
			if(filter == null)
				throw new InvalidOperationException("filter is null.");

			// get the settings element...
			XmlElement settingsElement = (XmlElement)element.SelectSingleNode("Settings");
			if(settingsElement == null)
				throw new InvalidOperationException("settingsElement is null.");

			// load...
			SimpleXmlPropertyBag bag = SimpleXmlPropertyBag.Load(settingsElement, typeof(SimpleXmlPropertyBag));
			if(bag == null)
				throw new InvalidOperationException("bag is null.");

			// Lets deserialize the bag into the control
			filter.DeserializeSqlSearchSettings(bag);
			FilterMangler mangler = new FilterMangler(manglerId,bag,filter);

			// patch...
			//mangler.DeserializeSqlSearchSettings(bag);

			// return...
			return mangler;
		}

		private void LoadSettings(XmlElement element)
		{
			if(element == null)
				throw new ArgumentNullException("element");

			// load...
			_settings = SimpleXmlPropertyBag.Load(element, typeof(SimpleXmlPropertyBag));
			if(_settings == null)
				throw new InvalidOperationException("_settings is null.");
		}

		/// <summary>
		/// Raises the <c>AfterXmlDeserialization</c> event.
		/// </summary>
		private void OnAfterXmlDeserialization()
		{
			OnAfterXmlDeserialization(EventArgs.Empty);
		}
		
		/// <summary>
		/// Raises the <c>AfterXmlDeserialization</c> event.
		/// </summary>
		protected virtual void OnAfterXmlDeserialization(EventArgs e)
		{
			// raise...
			if(AfterXmlDeserialization != null)
				AfterXmlDeserialization(this, e);
		}
	}
}
