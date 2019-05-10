// BootFX - Application framework for .NET applications
// 
// File: ExtendedPropertyDefinition.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Data;
using System.Text;
using System.Xml;
using BootFX.Common.Data;
using BootFX.Common.Entities;
using BootFX.Common.Xml;

namespace BootFX.Common.Entities
{
	/// <summary>
	/// Summary description for ExtendedProperty.
	/// </summary>
	public sealed class ExtendedPropertyDefinition : ToXmlBase
	{
		/// <summary>
		/// Private field to support <see cref="Settings"/> property.
		/// </summary>
		private SimpleXmlPropertyBag _settings = new SimpleXmlPropertyBag();
		
		/// <summary>
		/// Private field to support <see cref="DataType"/> property.
		/// </summary>
		// mbr - 25-09-2007 - renamed.
//		private ExtendedPropertyDataType _dataType;
		private ExtendedPropertyDataType _dataType;

		/// <summary>
		/// Private field to support <see cref="EntityType"/> property.
		/// </summary>
		private string _entityTypeId;
		
		/// <summary>
		/// Private field to support <see cref="Name"/> property.
		/// </summary>
		private string _name;

		/// <summary>
		/// Private field to support <see cref="NativeName"/> property.
		/// </summary>
		private string _nativeName;
		
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="type"></param>
		public ExtendedPropertyDefinition(string name, string nativeName, Type type, string entityTypeId)
		{
			if(type == null)
				throw new ArgumentNullException("type");
			
			// get...
			this.Initialize(name, nativeName, new ClrExtendedPropertyDataType(type), entityTypeId);
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="entityTypeId"></param>
		public ExtendedPropertyDefinition(string name, string nativeName, ExtendedLookupDefinition lookup, string entityTypeId)
		{
			if(lookup == null)
				throw new ArgumentNullException("lookup");
			
			// get...
			this.Initialize(name, nativeName, new LookupExtendedPropertyDataType(lookup.Name), entityTypeId);
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="type"></param>
		/// <param name="entityTypeId"></param>
		public ExtendedPropertyDefinition(string name, string nativeName, ExtendedPropertyDataType dataType, string entityTypeId)
		{
			this.Initialize(name, nativeName, dataType, entityTypeId);
		}

		private void Initialize(string name, string nativeName, ExtendedPropertyDataType dataType, string entityTypeId)
		{
			if(name == null)
				throw new ArgumentNullException("name");
			if(name.Length == 0)
				throw new ArgumentOutOfRangeException("'name' is zero-length.");			
			if(nativeName == null)
				throw new ArgumentNullException("nativeName");
			if(nativeName.Length == 0)
				throw new ArgumentOutOfRangeException("'nativeName' is zero-length.");
			if(dataType == null)
				throw new ArgumentNullException("dataType");
			if(entityTypeId == null)
				throw new ArgumentNullException("entityTypeId");
			if(entityTypeId.Length == 0)
				throw new ArgumentOutOfRangeException("'entityTypeId' is zero-length.");

			// check...
			EntityType.AssertIsLegalIdentifierName(nativeName);
			
			// set...
			_name = name;
			_nativeName = nativeName;
			_dataType = dataType;
			_entityTypeId = entityTypeId;
		}

		/// <summary>
		/// Gets the entitytype.
		/// </summary>
		public string EntityTypeId
		{
			get
			{
				return _entityTypeId;
			}
		}
		
		/// <summary>
		/// Gets the type.
		/// </summary>
		// mbr - 25-09-2007 - renamed.		
//		public ExtendedPropertyDataType DataType
		public ExtendedPropertyDataType DataType
		{
			get
			{
				return _dataType;
			}
			set
			{
				if(value == null)
					throw new ArgumentNullException("value");
				_dataType = value;
			}
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
				if(value == null)
					throw new ArgumentNullException("value");
				if(value.Length == 0)
					throw new ArgumentOutOfRangeException("'value' is zero-length.");

				if(_name != value)
					_name = value;
			}
		}

		/// <summary>
		/// Gets the EntityField for the Custom Property
		/// </summary>
		/// <returns></returns>
		public EntityField GetEntityField()
		{
			// get the type...
			if(DataType == null)
				throw new InvalidOperationException("DataType is null.");
			return DataType.GetEntityField(this);
		}

		internal static ExtendedPropertyDefinition FromXml(XmlElement node)
		{
			if(node == null)
				throw new ArgumentNullException("node");

			// type...
			string entityTypeId = XmlHelper.GetElementString(node, "EntityTypeId", OnNotFound.ThrowException);
			if(entityTypeId == null)
				throw new ArgumentNullException("entityTypeId");
			if(entityTypeId.Length == 0)
				throw new ArgumentOutOfRangeException("'entityTypeId' is zero-length.");

			// get the name...
			string name = XmlHelper.GetElementString(node, "Name", OnNotFound.ThrowException);
			string nativeName = XmlHelper.GetElementString(node, "NativeName", OnNotFound.ThrowException);

			// type...
			string typeId = XmlHelper.GetElementString(node, "Type", OnNotFound.ThrowException);
			ExtendedPropertyDataType dataType = ExtendedPropertyDataType.GetExtendedPropertyDataType(typeId);
			if(dataType== null)
				throw new InvalidOperationException("dataType is null.");

			// settings...
			string settingsXml = XmlHelper.GetElementString(node, "Settings", OnNotFound.ReturnNull);

			// return...
			ExtendedPropertyDefinition prop = new ExtendedPropertyDefinition(name, nativeName, dataType, entityTypeId);
			if(settingsXml != null && settingsXml.Length > 0)
				prop._settings = SimpleXmlPropertyBag.FromXml(settingsXml, typeof(SimpleXmlPropertyBag));

			// return...
			return prop;
		}

		protected override string DefaultElementName
		{
			get
			{
				return "ExtendedProperty";
			}
		}

		protected override void WriteXml(System.Xml.XmlWriter xml, WriteXmlContext context)
		{
			// render...
			xml.WriteElementString("Name", this.Name);
			xml.WriteElementString("NativeName", this.NativeName);
			xml.WriteElementString("Type", this.DataType.Id);
			xml.WriteElementString("EntityTypeId", this.EntityTypeId);

			// mbr - 09-12-2005 - write out the settings...			
			string settingsXml = this._settings.ToXml("Settings");
			if(settingsXml != null && settingsXml.Length > 0)
				xml.WriteElementString("Settings", settingsXml);
		}

		/// <summary>
		/// Gets the settings.
		/// </summary>
		internal PropertyBag Settings
		{
			get
			{
				return _settings;
			}
		}

		public override string ToString()
		{
			return string.Format("{0} ({1})", Name, this.DataTypeName);
		}

		// mbr - 25-09-2007 - renamed.
//		public string TypeName
		public string DataTypeName
		{
			get
			{
				return this.DataType.Name;	
			}
		}

		public string Id
		{
			get
			{
				return string.Format("{0}|{1}", this.NativeName, this.EntityTypeId);
			}
		}

		/// <summary>
		/// Gets the nativename.
		/// </summary>
		public string NativeName
		{
			get
			{
				return _nativeName;
			}
		}

		/// <summary>
		/// Gets or sets if the property is a multi-value property.
		/// </summary>
		public bool MultiValue
		{
			get
			{
				bool defaultValue = false;
				if(this.DataType is LookupExtendedPropertyDataType)
					defaultValue = true;
				return this.Settings.GetBooleanValue("MultiValue", defaultValue, Cultures.System, OnNotFound.ReturnNull);
			}
			set
			{
				this.Settings["MultiValue"] = value;
			}
		}

		/// <summary>
		/// Gets the size of the property.
		/// </summary>
		public long Size
		{
			get
			{
				if(DataType == null)
					throw new InvalidOperationException("DataType is null.");

				// mbr - 25-09-2007 - can't pass the default size through as new custom providers don't have this capability...				
//				long size = this.Settings.GetInt64Value("Size", this.DataType.DefaultSize, Cultures.System, OnNotFound.ReturnNull);
				long size = this.Settings.GetInt64Value("Size", -1, Cultures.System, OnNotFound.ReturnNull);
				if(size == -1)
					size = this.DataType.DefaultSize;

				// return...
				return size;
			}
			set
			{			
				this.Settings["Size"] = value;
			}
		}

		public string Metrics
		{
			get
			{
				// get...
				StringBuilder builder = new StringBuilder();
				builder.Append(this.DataType.Name);

				// lookup?
				if(this.DataType is LookupExtendedPropertyDataType)
				{
					builder.Append(" [Lookup");
					if(this.MultiValue)
						builder.Append(", Multiple");
					else
						builder.Append(", Single");
					builder.Append("]");
				}
				else if(this.DataType is ClrExtendedPropertyDataType)
				{
					// size...
					long size = this.Size;
					if(size != 0)
					{
						builder.Append("(");
						builder.Append(size);
						builder.Append(")");
					}
				}
				else
					throw new NotSupportedException(string.Format("Cannot handle '{0}'.", this.DataType));

				// return...
				return builder.ToString();
			}
		}
	}
}
