// BootFX - Application framework for .NET applications
// 
// File: Project.cs
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
using System.ComponentModel;
using BootFX.Common.Xml;
using BootFX.Common.Data;
using BootFX.Common.Data.Schema;

namespace BootFX.Common.DbUtil
{
	/// <summary>
	/// Describes a project.
	/// </summary>
	internal class Project : ToXmlBase
	{
		/// <summary>
		/// Private field to support <c>LocalSettingsFilePath</c> property.
		/// </summary>
		private string _localSettingsFilePath;
		
		/// <summary>
		/// Private field to support <see cref="LocalSettings"/> property.
		/// </summary>
		private LocalSettings _localSettings;
		
		/// <summary>
		/// Raised when the <c>IsDirty</c> property has changed.
		/// </summary>
		[Browsable(true), Category("Property Changed"), Description("Raised when the IsDirty property has changed.")]
		public event EventHandler IsDirtyChanged;
		
		/// <summary>
		/// Private field to support <see cref="IsDirty"/> property.
		/// </summary>
		private bool _isDirty = false;
		
		/// <summary>
		/// Private field to support <c>Schema</c> property.
		/// </summary>
		private SqlSchema _schema;
		
		/// <summary>
		/// Private field to support <see cref="FilePath"/> property.
		/// </summary>
		private string _filePath = null;
		
		/// <summary>
		/// Private field to support <see cref="Settings"/> property.
		/// </summary>
		private Settings _settings;
		
		//private ExtendedPropertySettings _extendedPropertySettings;

		/// <summary>
		/// Constructor.
		/// </summary>
		public Project()
		{
			this.Settings = new Settings();
			//this.ExtendedPropertySettings = new ExtendedPropertySettings();
		}

		/// <summary>
		/// Gets the settings.
		/// </summary>
		public Settings Settings
		{
			get
			{
				return _settings;
			}
			set
			{
				if(value == null)
					throw new ArgumentNullException("value");

				if(_settings != value)
				{
					if(_settings != null)
						_settings.Changed -= new EventHandler(_settings_Changed);

					// set...
					_settings = value;

					// subscribe...
					if(_settings != null)
						_settings.Changed += new EventHandler(_settings_Changed);
				}
			}
		}

		/// <summary>
		/// Gets the filepath.
		/// </summary>
		public string FilePath
		{
			get
			{
				return _filePath;
			}
			set
			{
				if(_filePath != value)
				{
					_filePath = value;

					// mjr - 07-07-2005 - added local folder path...
					if(value != null)
					{
						_localSettingsFilePath = Path.Combine(Runtime.Current.ApplicationDataFolderPath, string.Format("Local Settings for {0} ({1}).xml", 
							Path.GetFileNameWithoutExtension(value), ProjectStore.CurrentStore.GetType().Name));
					}
				}
			}
		}

		/// <summary>
		/// Gets the localsettingsfilepath.
		/// </summary>
		private string LocalSettingsFilePath
		{
			get
			{
				if(!Directory.Exists(new FileInfo(_localSettingsFilePath).DirectoryName))
					Directory.CreateDirectory(new FileInfo(_localSettingsFilePath).DirectoryName);

				// returns the value...
				return _localSettingsFilePath;
			}
		}

		/// <summary>
		/// Returns true if the project is new.
		/// </summary>
		public bool IsNew
		{
			get
			{
				if(this.FilePath == null || this.FilePath.Length == 0)
					return true;
				else
					return false;
			}
		}

		/// <summary>
		/// Gets the project name.
		/// </summary>
		public string Name
		{
			get
			{
				if(this.IsNew)
					return null;
				else
					return new FileInfo(this.FilePath).Name;
			}
		}

		/// <summary>
		/// Gets the schema.
		/// </summary>
		public SqlSchema Schema
		{
			get
			{
				// returns the value...
				return _schema;
			}
			set
			{
				_schema = value;
			}
		}

		protected override void WriteXml(System.Xml.XmlWriter xml, WriteXmlContext context)
		{
			if(xml == null)
				throw new ArgumentNullException("xml");
			if(context == null)
				throw new ArgumentNullException("context");

			// settings...
			if(Settings == null)
				throw new InvalidOperationException("Settings is null.");

			// settings...
//			if(ExtendedPropertySettings == null)
//				throw new InvalidOperationException("ExtendedPropertySettings is null.");

			// settings...
			this.Settings.ToXml(xml, context);
			//this.ExtendedPropertySettings.ToXml(xml,context);

			// schema...
			if(this.Schema != null)
			{
				// write it...
				this.Schema.ToXml(xml, context);
			}
			else
			{
				// write a blank schema...
				xml.WriteElementString("Schema", string.Empty);
			}
		}

		/// <summary>
		/// Saves the project.
		/// </summary>
		public void Save()
		{
			// save the xml...
			if(this.IsNew)
				throw new InvalidOperationException("Project is new.");

			// mbr - 12-12-2005 - save it to the selected store...			
			string xml = this.ToXml();
			if(xml == null)
				throw new InvalidOperationException("'xml' is null.");
			if(xml.Length == 0)
				throw new InvalidOperationException("'xml' is zero-length.");
			ProjectStore.CurrentStore.Save(this.FilePath, xml);

			// save the local settings...
			if(LocalSettingsFilePath == null)
				throw new InvalidOperationException("'LocalSettingsFilePath' is null.");
			if(LocalSettingsFilePath.Length == 0)
				throw new InvalidOperationException("'LocalSettingsFilePath' is zero-length.");
			this.LocalSettings.Save(this.LocalSettingsFilePath, "LocalSettings", true);

			// set clean...
			this.SetDirty(false);
		}

		/// <summary>
		/// Gets the isdirty.
		/// </summary>
		public bool IsDirty
		{
			get
			{
				return _isDirty;
			}
		}

		/// <summary>
		/// Loads the project from the given path.
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public static Project LoadXml(string filePath)
		{
			if(filePath == null)
				throw new ArgumentNullException("filePath");
			if(filePath.Length == 0)
				throw new ArgumentOutOfRangeException("'filePath' is zero-length.");

			// create...
			XmlDocument document = ProjectStore.CurrentStore.Load(filePath);

			// defer...
			Project newProject = LoadXml(document);
			if(newProject == null)
				throw new InvalidOperationException("newProject is null.");
			newProject.FilePath = filePath;

			// mjr - 07-07-2005 - load local settings...
			if(newProject.LocalSettingsFilePath == null)
				throw new InvalidOperationException("'newProject.LocalSettingsFilePath' is null.");
			if(newProject.LocalSettingsFilePath.Length == 0)
				throw new InvalidOperationException("'newProject.LocalSettingsFilePath' is zero-length.");
			if(File.Exists(newProject.LocalSettingsFilePath))
			{
				try
				{
					newProject._localSettings = (LocalSettings)LocalSettings.Load(newProject.LocalSettingsFilePath, typeof(LocalSettings));
				}
				catch(Exception ex)
				{
					throw new InvalidOperationException(string.Format("Failed to load settings from '{0}'.", newProject.LocalSettingsFilePath), ex);
				}
			}
			else
				newProject._localSettings = new LocalSettings();

			// return...
			return newProject;
		}

		/// <summary>
		/// Loads the project from the given path.
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public static Project LoadXml(XmlDocument document)
		{
			if(document == null)
				throw new ArgumentNullException("document");

			// select the project node...
			XmlElement projectElement = (XmlElement)document.SelectSingleNode("Project");
			if(projectElement == null)
				throw new InvalidOperationException("Project element not found.");

			// create...
			Project newProject = new Project();

			// load settings...
			XmlElement settingsElement = (XmlElement)projectElement.SelectSingleNode("Settings");
			if(settingsElement != null)
				newProject.Settings = Settings.FromXml(settingsElement);

			// load settings...
//			XmlElement extendedPropertySettingsElement = (XmlElement)projectElement.SelectSingleNode("ExtendedPropertySettings");
//			if(extendedPropertySettingsElement != null)
//				newProject.ExtendedPropertySettings = ExtendedPropertySettings.FromXml(extendedPropertySettingsElement);
//			else
//				newProject.ExtendedPropertySettings = new ExtendedPropertySettings();

			// merge the schema in...
			XmlElement schemaElement = (XmlElement)projectElement.SelectSingleNode("SqlSchema");
			if(schemaElement != null)
				newProject.Schema = SqlSchema.FromXml(schemaElement);

			// flag...
			newProject.SetDirty(false);

			// return...
			return newProject;
		}

		/// <summary>
		/// Sets the project as dirty or clean.
		/// </summary>
		internal void SetDirty(bool isDirty)
		{
			if(_isDirty != isDirty)
			{
				_isDirty = isDirty;
				this.OnIsDirtyChanged();
			}
		}

		/// <summary>
		/// Raises the <c>IsDirtyChanged</c> event.
		/// </summary>
		private void OnIsDirtyChanged()
		{
			OnIsDirtyChanged(EventArgs.Empty);
		}
		
		/// <summary>
		/// Raises the <c>IsDirtyChanged</c> event.
		/// </summary>
		protected virtual void OnIsDirtyChanged(EventArgs e)
		{
			if(IsDirtyChanged != null)
				IsDirtyChanged(this, e);
		}

		private void _settings_Changed(object sender, EventArgs e)
		{
			this.SetDirty(true);
		}

		/// <summary>
		/// Gets the localsettings.
		/// </summary>
		internal LocalSettings LocalSettings
		{
			get
			{
				if(_localSettings == null)
					_localSettings = new LocalSettings();
				return _localSettings;
			}
		}

		/// <summary>
		/// Gets the display name.
		/// </summary>
		internal string DisplayName
		{
			get
			{
				if(this.Name != null && this.Name.Length > 0)
					return Path.GetFileNameWithoutExtension(this.Name);
				else
					return null;
			}
		}
	}
}
