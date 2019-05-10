// BootFX - Application framework for .NET applications
// 
// File: RuntimeStartArgs.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Reflection;
using BootFX.Common.Data;
using BootFX.Common.Entities;
using BootFX.Common.Services;

namespace BootFX.Common
{
	/// <summary>
	/// Summary description for RUntimeStartArgs.
	/// </summary>
	public class RuntimeStartArgs
	{
        /// <summary>
        /// Private value to support the <see cref="MultidatabaseCaching">MultidatabaseCaching</see> property.
        /// </summary>
        private bool _multidatabaseCaching;

        /// <summary>
        /// Private value to support the <see cref="ConfigDatabaseString">ConfigDatabaseString</see> property.
        /// </summary>
        private string _configDatabaseString;

        /// <summary>
        /// Private value to support the <see cref="ConfigDatabaseType">ConfigDatabaseType</see> property.
        /// </summary>
        private Type _configDatabaseType;

        /// <summary>
        /// Private value to support the <see cref="ManageRemotingClientRegistrations">ManageRemotingClientRegistrations</see> property.
        /// </summary>
        private bool _manageRemotingClientRegistrations = false;

        /// <summary>
        /// Private value to support the <see cref="AllowEncryptedConnectionStrings">AllowEncryptedConnectionStrings</see> property.
        /// </summary>
        private bool _allowEncryptedConnectionStrings = true;

        /// <summary>
        /// Private value to support the <see cref="ManageRemotingReciprocalClient">ManageRemotingReciprocalClient</see> property.
        /// </summary>
        private bool _manageRemotingReciprocalClient = true;

        /// <summary>
        /// Private value to support the <see cref="UseNewUserSettingsFormat">UseNewUserSettingsFormat</see> property.
        /// </summary>
        private bool _useNewUserSettingsFormat = true;

        /// <summary>
		/// Private field to support <c>IsLowTrust</c> property.
		/// </summary>
		private bool _isLowTrust = false;
		
		/// <summary>
		/// Private field to support <c>ManagementServiceEngines</c> property.
		/// </summary>
		private ServiceEngineCollection _managementServiceEngines = new ServiceEngineCollection();
		
		/// <summary>
		/// Private field to support <see cref="BootAssembly"/> property.
		/// </summary>
		private Assembly _bootAssembly;
		
		/// <summary>
		/// Private field to support <c>DatabaseExtensibilityProviderFactory</c> property.
		/// </summary>
		private IDatabaseExtensibilityProviderFactory _databaseExtensibilityProviderFactory;
		
		/// <summary>
		/// Private field to support <c>EntityTypeFactory</c> property.
		/// </summary>
		private IEntityTypeFactory _entityTypeFactory;
		
		public const string DefaultAppSettingsKey = "BfxInstallationName";

		/// <summary>
		/// Private field to support <c>InstallationName</c> property.
		/// </summary>
		private string _installationName = string.Empty;

        private string _eventLogSourceName = string.Empty;

        public string InstallationSettingsFilePath { get; set; }
        public string AllUsersApplicationDataFolderPath { get; set; }

        public IEntityPersistenceFactory EntityPersistenceFactory { get; set; }

        public ConsoleLogMode ConsoleLogMode { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public RuntimeStartArgs()
			: this(Assembly.GetCallingAssembly())
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		public RuntimeStartArgs(Assembly bootAssembly)
		{
			if(bootAssembly == null)
				throw new ArgumentNullException("bootAssembly");
			_bootAssembly = bootAssembly;
		}

		/// <summary>
		/// Gets or sets the installationname
		/// </summary>
		public string InstallationName
        {
            get
            {
                return _installationName;
            }
            set
            {
                if (value == null)
                    value = string.Empty;

                // check to see if the value has changed...
                if (value != _installationName)
                {
                    // set the value...
                    _installationName = value;
                }
            }
        }

		/// <summary>
		/// Gets or sets the entitytypefactory
		/// </summary>
		public IEntityTypeFactory EntityTypeFactory
		{
			get
			{
				return _entityTypeFactory;
			}
			set
			{
				// check to see if the value has changed...
				if(value != _entityTypeFactory)
				{
					// set the value...
					_entityTypeFactory = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the databaseextensibilityproviderfactory
		/// </summary>
		public IDatabaseExtensibilityProviderFactory DatabaseExtensibilityProviderFactory
		{
			get
			{
				return _databaseExtensibilityProviderFactory;
			}
			set
			{
				// check to see if the value has changed...
				if(value != _databaseExtensibilityProviderFactory)
				{
					// set the value...
					_databaseExtensibilityProviderFactory = value;
				}
			}
		}

		/// <summary>
		/// Gets the assembly that contains the UI-agnostic objects for the application.
		/// </summary>
		public Assembly BootAssembly
		{
			get
			{
				return _bootAssembly;
			}
		}

		/// <summary>
		/// Gets a collection of ServiceEngine objects.
		/// </summary>
		public ServiceEngineCollection ManagementServiceEngines
		{
			get
			{
				return _managementServiceEngines;
			}
		}

		/// <summary>
		/// Gets or sets whether the application is running in low trust.
		/// </summary>
		/// <remarks>For example, on some hosted providers the trust level used prevents
		/// certain .NET features from being available.</remarks>
		public bool IsLowTrust
		{
			get
			{
				return _isLowTrust;
			}
			set
			{
				// check to see if the value has changed...
				if(value != _isLowTrust)
				{
					// set the value...
					_isLowTrust = value;
				}
			}
		}

        /// <summary>
        /// Gets the UseNewUserSettingsFormat value.
        /// </summary>
        public bool UseNewUserSettingsFormat
        {
            get
            {
                return _useNewUserSettingsFormat;
            }
            set
            {
                _useNewUserSettingsFormat = value;
            }
        }

        /// <summary>
        /// Gets the ManageRemotingReciprocalClient value.
        /// </summary>
        public bool ManageRemotingReciprocalClient
        {
            get
            {
                return _manageRemotingReciprocalClient;
            }
            set
            {
                _manageRemotingReciprocalClient = value;
            }
        }

        /// <summary>
        /// Gets the AllowEncryptedConnectionStrings value.
        /// </summary>
        public bool AllowEncryptedConnectionStrings
        {
            get
            {
                return _allowEncryptedConnectionStrings;
            }
            set
            {
                _allowEncryptedConnectionStrings = value;
            }
        }

        /// <summary>
        /// Gets the ManageRemotingClientRegistrations value.
        /// </summary>
        public bool ManageRemotingClientRegistrations
        {
            get
            {
                return _manageRemotingClientRegistrations;
            }
            set
            {
                _manageRemotingClientRegistrations = value;
            }
        }

        /// <summary>
        /// Gets the ConfigDatabaseType value.
        /// </summary>
        public Type ConfigDatabaseType
        {
            get
            {
                return _configDatabaseType;
            }
            set
            {
                _configDatabaseType = value;
            }
        }

        /// <summary>
        /// Gets the ConfigDatabaseString value.
        /// </summary>
        public string ConfigDatabaseString
        {
            get
            {
                return _configDatabaseString;
            }
            set
            {
                _configDatabaseString = value;
            }
        }

        internal bool HasConfigSettings
        {
            get
            {
                if (this.ConfigDatabaseType != null && !(string.IsNullOrEmpty(this.ConfigDatabaseString)))
                    return true;

                // check...
                if (this.ConfigDatabaseType != null || !(string.IsNullOrEmpty(this.ConfigDatabaseString)))
                    throw new InvalidOperationException("Both a configuration database type and string must be provided.");

                // nope...
                return false;
            }
        }

        /// <summary>
        /// Gets the MultidatabaseCaching value.
        /// </summary>
        public bool MultidatabaseCaching
        {
            get
            {
                return _multidatabaseCaching;
            }
            set
            {
                _multidatabaseCaching = value;
            }
        }

        /// <summary>
        /// Gets the MultidatabaseCaching value.
        /// </summary>
        public string EventLogSourceName
        {
            get
            {
                return _eventLogSourceName;
            }
            set
            {
                _eventLogSourceName = value;
            }
        }
    }
}
