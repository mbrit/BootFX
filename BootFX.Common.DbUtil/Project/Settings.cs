// BootFX - Application framework for .NET applications
// 
// File: Settings.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Xml;
using System.Collections;
using System.Collections.Specialized;
using BootFX.Common.Xml;
using BootFX.Common.Data;
using BootFX.Common.Data.Schema;
using BootFX.Common.CodeGeneration;

namespace BootFX.Common.DbUtil
{
	/// <summary>
	/// Holds project settings.
	/// </summary>
	internal class Settings : ToXmlBase
	{
		private const string GenerationTypeNameKey = "GenerationTypeName";

		/// <summary>
		/// Private field to support <c>Language</c> property.
		/// </summary>
		private Language _language = Language.CSharp;
		
		/// <summary>
		/// Private field to support <c>ServicesBaseType</c> property.
		/// </summary>
		private string _dtoBaseType = string.Empty;

		/// <summary>
		/// Private field to support <c>DatabaseName</c> property.
		/// </summary>
		private string _databaseName;

		/// <summary>
		/// Private field to support <c>DatabaseName</c> property.
		/// </summary>
		private string _generationTypeName;

		/// <summary>
		/// Private field to support <c>TargetPlatform</c> property.
		/// </summary>
		private DotNetVersion _targetVersion = DotNetVersion.V1;	
		
		//		/// <summary>
		//		/// Private field to support <c>ConnectionType</c> property.
		//		/// </summary>
		//		private Type _connectionType;
		//
		//		/// <summary>
		//		/// Private field to support <c>ConnectionString</c> property.
		//		/// </summary>
		//		private string _connectionString;
		
		/// <summary>
		/// Raised when settings have changed.
		/// </summary>
		public event EventHandler Changed;
		
		/// <summary>
		/// Private field to support <c>BaseType</c> property.
		/// </summary>
		private string _baseType = string.Empty;

		/// <summary>
		/// Private field to support <c>NamespaceName</c> property.
		/// </summary>
		private string _namespaceName = "BootFX.Common.Generated";
		
		/// <summary>
		/// Constructor.
		/// </summary>
		public Settings()
		{
			//			if(Runtime.IsStarted)
			//			{
			//				string path = Runtime.Current.DocumentsFolderPath;
			//				if(path.EndsWith("\\") == false && path.EndsWith("/") == false)
			//					path += "\\";
			//				this.EntitiesFolderPath = path + "Entities";
			//				this.LocalSettings = path + "Services";
			//			}
		}

		/// <summary>
		/// Gets or sets the namespacename
		/// </summary>
		public string NamespaceName
		{
			get
			{
				return _namespaceName;
			}
			set
			{
				// check to see if the value has changed...
				if(value != _namespaceName)
				{
					// set the value...
					_namespaceName = value;
					this.OnChanged();
				}
			}
		}
		
		/// <summary>
		/// Gets or sets the entity base type
		/// </summary>
		public string BaseType
		{
			get
			{
				return _baseType;
			}
			set
			{
				// check to see if the value has changed...
				if(value != _baseType)
				{
					// set the value...
					_baseType = value;
					this.OnChanged();
				}
			}
		}

		protected override void WriteXml(System.Xml.XmlWriter xml, WriteXmlContext context)
		{
			if(xml == null)
				throw new ArgumentNullException("xml");
			if(context == null)
				throw new ArgumentNullException("context");
			
			// set...
			xml.WriteElementString("NamespaceName", this.NamespaceName);
			xml.WriteElementString("BaseType", this.BaseType);
			xml.WriteElementString("DtoBaseType", this.DtoBaseType);
			//			xml.WriteElementString("ConnectionString", this.ConnectionString);
			//			if(this.ConnectionType != null)
			//				xml.WriteElementString("ConnectionType", string.Format("{0},{1}", this.ConnectionType.FullName, 
			//					this.ConnectionType.Assembly.GetName().Name));
			xml.WriteElementString("DatabaseName", this.DatabaseName);
			xml.WriteElementString("TargetVersion", this.TargetVersion.ToString());

			// mbr - 29-11-2007 - for c7 - added generation type...
			xml.WriteElementString(GenerationTypeNameKey, this.GenerationTypeName);
		}

		/// <summary>
		/// Restores settings from the XML file.
		/// </summary>
		/// <param name="element"></param>
		/// <returns></returns>
		public static Settings FromXml(XmlElement element)
		{
			if(element == null)
				throw new ArgumentNullException("element");
			
			// load...
			Settings settings = new Settings();
			settings.NamespaceName = XmlHelper.GetElementString(element, "NamespaceName", OnNotFound.ThrowException);
			try{settings.BaseType = XmlHelper.GetElementString(element, "BaseType", OnNotFound.ThrowException);}
			catch{}
			try{settings.DtoBaseType = XmlHelper.GetElementString(element, "DtoBaseType", OnNotFound.ReturnNull);}
			catch{}	
			//			if(settings.ServicesFolderPath == null || settings.ServicesFolderPath.Length == 0)
			//				settings.ServicesFolderPath = settings.FolderPath;
			//			settings.ConnectionString = XmlHelper.GetElementString(element, "ConnectionString", OnNotFound.ReturnNull);
			//			settings.ConnectionType = XmlHelper.GetElementType(element, "ConnectionType", OnNotFound.ReturnNull, true);
			settings.DatabaseName = XmlHelper.GetElementString(element, "DatabaseName", OnNotFound.ReturnNull);
			object asObject = XmlHelper.GetElementEnumerationValue(element, "TargetVersion", typeof(DotNetVersion), OnNotFound.ReturnNull);
			if(asObject is DotNetVersion)
				settings.TargetVersion = (DotNetVersion)asObject;
			else
				settings.TargetVersion = DotNetVersion.V1;

			// generation type name...
			settings.GenerationTypeName = XmlHelper.GetElementString(element, GenerationTypeNameKey, OnNotFound.ReturnNull);

			// return...
			return settings;
		}

		/// <summary>
		/// Raises the <c>Changed</c> event.
		/// </summary>
		private void OnChanged()
		{
			OnChanged(EventArgs.Empty);
		}
		
		/// <summary>
		/// Raises the <c>Changed</c> event.
		/// </summary>
		protected virtual void OnChanged(EventArgs e)
		{
			// raise...
			if(Changed != null)
				Changed(this, e);
		}

		//		/// <summary>
		//		/// Gets or sets the connectionstring
		//		/// </summary>
		//		public string ConnectionString
		//		{
		//			get
		//			{
		//				return _connectionString;
		//			}
		//			set
		//			{
		//				// check to see if the value has changed...
		//				if(value != _connectionString)
		//				{
		//					// set the value...
		//					_connectionString = value;
		//					this.OnChanged();
		//				}
		//			}
		//		}
		//		
		//		/// <summary>
		//		/// Gets or sets the connectiontype
		//		/// </summary>
		//		public Type ConnectionType
		//		{
		//			get
		//			{
		//				return _connectionType;
		//			}
		//			set
		//			{
		//				// check to see if the value has changed...
		//				if(value != _connectionType)
		//				{
		//					// set the value...
		//					_connectionType = value;
		//					this.OnChanged();
		//				}
		//			}
		//		}

		//		/// <summary>
		//		/// Gets the connection settings.
		//		/// </summary>
		//		public ConnectionSettings ConnectionSettings
		//		{
		//			get
		//			{
		//				if(this.ConnectionType != null && this.ConnectionString != null && this.ConnectionString.Length > 0)
		//					return new ConnectionSettings(ConnectionSettings.DefaultName, this.ConnectionType, this.ConnectionString);
		//				else
		//					return null;
		//			}
		//		}

		/// <summary>
		/// Gets or sets the databasename
		/// </summary>
		public string GenerationTypeName
		{
			get
			{
				return _generationTypeName;
			}
			set
			{
				// check to see if the value has changed...
				if(value != _generationTypeName)
				{
					// set the value...
					_generationTypeName = value;
					this.OnChanged();
				}
			}
		}

		/// <summary>
		/// Gets or sets the databasename
		/// </summary>
		public string DatabaseName
		{
			get
			{
				return _databaseName;
			}
			set
			{
				// check to see if the value has changed...
				if(value != _databaseName)
				{
					// set the value...
					_databaseName = value;
					this.OnChanged();
				}
			}
		}

		/// <summary>
		/// Returns true if a database name is being used.
		/// </summary>
		public bool HasDatabaseName
		{
			get
			{
				if(this.DatabaseName == null || this.DatabaseName.Length == 0)
					return false;
				else
					return true;
			}
		}

		/// <summary>
		/// Gets or sets the ServicesBaseType
		/// </summary>
		public string DtoBaseType
		{
			get
			{
				return _dtoBaseType;
			}
			set
			{
				// check to see if the value has changed...
				if(value != _dtoBaseType)
				{
					// set the value...
					_dtoBaseType = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the targetplatform
		/// </summary>
		internal DotNetVersion TargetVersion
		{
			get
			{
				return _targetVersion;
			}
			set
			{
				// check to see if the value has changed...
				if(value != _targetVersion)
				{
					// set the value...
					_targetVersion = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the language
		/// </summary>
		internal Language Language
		{
			get
			{
				return _language;
			}
			set
			{
				// check to see if the value has changed...
				if(value != _language)
				{
					// set the value...
					_language = value;
				}
			}
		}

		/// <summary>
		/// Gets the SourceGear DiffMerge path.
		/// </summary>
		internal static string SgdmPath
		{
			get
			{
				string path = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
				if(!(path.EndsWith(@"\")))
					path += @"\";
				path += @"SourceGear\Vault Client\sgdm.exe";
				return path;
			}
		}
	}
}
