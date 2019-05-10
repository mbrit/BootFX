// BootFX - Application framework for .NET applications
// 
// File: Database.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.IO;
using System.Text;
using System.Data;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Configuration;
using BootFX.Common.Xml;
using BootFX.Common.Data.Schema;
using BootFX.Common.Data.Formatters;
using BootFX.Common.Crypto;
using BootFX.Common.Entities;
using BootFX.Common.Management;

namespace BootFX.Common.Data
{
    /// <summary>
    /// Provides quick access to the default connection.
    /// </summary>
    public static class Database
    {
        private const string ConfigDatabaseKey = "__Config";

        public static bool CommandCachingEnabled { get; set; }

        /// <summary>
        /// Private value to support the <see cref="StatementCallback">StatementCallback</see> property.
        /// </summary>
        private static ISqlStatementCallback _statementCallback;

        /// <summary>
        /// Private value to support the <see cref="ForcedSqlDialect">ForcedSqlDialect</see> property.
        /// </summary>
        private static SqlDialect _forcedDefaultDialect;

        /// <summary>
        /// Private field to support <c>DefaultDatabaseProvider</c> property.
        /// </summary>
        private static IDefaultDatabaseProvider _defaultDatabaseProvider = new StandardDefaultDatabaseProvider();

        /// <summary>
        /// Private field to support <c>DefaultDatabaseProvider</c> property.
        /// </summary>
        private static INamedDatabaseProvider _namedDatabaseProvider = new StandardNamedDatabaseProvider();

        public static ISystemDatabaseProvider SystemDatabaseProvider { get; set; }

        /// <summary>
        /// Private field to support <c>ExtensibilityProvider</c> property.
        /// </summary>
        private static DatabaseExtensibilityProvider _extensibilityProvider;

        /// <summary>
        /// Private field to support <see cref="AffinityState"/> property.
        /// </summary>
        private static object _affinityState;

        /// <summary>
        /// Private field to support <c>CommandDumpLevel</c> property.
        /// </summary>
        //private static DatabaseCommandDumpLevel _commandDumpLevel = DatabaseCommandDumpLevel.None;

        /// <summary>
        /// Raised when the <c>DefaultDatabase</c> property has changed.
        /// </summary>
        public static event EventHandler DefaultDatabaseChanged;

        /// <summary>
        /// Private field to support <c>DefaultSqlMode</c> property.
        /// </summary>
        private static SqlMode _defaultSqlMode = SqlMode.AdHoc;

        /// <summary>
        /// Private field to support <c>Databases</c> property.
        /// </summary>
        private static NamedDatabaseCollection _databases = new NamedDatabaseCollection();

        /// <summary>
        /// Delegate for <see cref="GetSchema"></see>.
        /// </summary>
        public delegate SqlSchema GetSchemaDelegate(IOperationItem operation);

        public static IsolationLevel DefaultIsolationLevel { get; set; }
        public static bool RetryDeadlocks { get; set; }
        public static TimeSpan RetryDeadlockWaitPeriod { get; set; }

        /// <summary>
        /// Private field to support <c>BoundConnection</c> property.
        /// </summary>
        [ThreadStatic()]
        private static IConnection _boundConnection;

        // mbr - 05-02-2008 - moved to standarddefaultdatabaseprovider...
        //		/// <summary>
        //		/// Private field to support <c>DatabaseType</c> property.
        //		/// </summary>
        //		private static Type _defaultConnectionType = null;
        //		
        //		/// <summary>
        //		/// private static field to support <c>DefaultConnectionString</c> property.
        //		/// </summary>
        //		private static string _defaultConnectionString = null;

        /// <summary>
        /// Private field to support <c>Logger</c> property.
        /// </summary>
        private static ILog _log = LogSet.GetLog(typeof(Database));

        /// <summary>
        /// Private field to support <c>DefaultDialect</c> property.
        /// </summary>
        private static SqlDialect _defaultSqlServerDialect = new SqlServerDialect();

        private static ArrayParameterType _arrayParameterType;
        private static bool _arrayParameterTypeSet;

        private static bool HasSetDefaultDatabase { get; set; }

        static Database()
        {
            DefaultIsolationLevel = IsolationLevel.Serializable;
            RetryDeadlockWaitPeriod = TimeSpan.FromMilliseconds(500);
            CommandCachingEnabled = true;
        }

        /// <summary>
        /// Gets the defaultdialect.
        /// </summary>
        public static SqlDialect DefaultDialect
        {
            get
            {
                // mbr - 2010-01-19 - added ability to force...
                if (_forcedDefaultDialect != null)
                    return _forcedDefaultDialect;

                // mbr - 01-02-2006 - added this to force the type to SQL Server if it hasn't been specified.				
                Type type = DefaultConnectionType;
                if (type == null)
                    type = typeof(SqlServerConnection);

                // defer...
                return GetDefaultDialectForConnectionType(type);
            }
        }

        /// <summary>
        /// Gets the ForcedSqlDialect value.
        /// </summary>
        // mbr - 2010-01-19 - added...
        private static SqlDialect ForcedDefaultDialect
        {
            get
            {
                return _forcedDefaultDialect;
            }
        }

        /// <summary>
        /// Sets a forced SQL dialect.
        /// </summary>
        /// <param name="connectionType"></param>
        public static void SetForcedDefaultDialect(Type connectionType)
        {
            if (connectionType == null)
                throw new ArgumentNullException("connectionType");

            // set...
            _forcedDefaultDialect = GetDefaultDialectForConnectionType(connectionType);
            if (_forcedDefaultDialect == null)
                throw new InvalidOperationException("'_forcedDefaultDialect' is null.");
        }

        internal static SqlDialect GetDefaultDialectForConnectionType(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            if (typeof(SqlServerConnection).IsAssignableFrom(type))
                return _defaultSqlServerDialect;
            else
            {
                // mbr - 2008-08-30 - use attribute...
                SqlDialectAttribute[] attrs = (SqlDialectAttribute[])type.GetCustomAttributes(typeof(SqlDialectAttribute), true);
                if (attrs.Length > 0)
                {
                    Type dialectType = attrs[0].Type;
                    if (dialectType == null)
                        throw new InvalidOperationException("A SqlDialect attribute was found, but the type was null.");

                    // create...
                    SqlDialect dialect = (SqlDialect)Activator.CreateInstance(dialectType);
                    if (dialect == null)
                        throw new InvalidOperationException(string.Format("Failed to create an instance of type '{0}'.", dialectType.AssemblyQualifiedName));

                    // return...
                    return dialect;
                }
                else
                    throw new InvalidOperationException("The dialect cannot be determined.");
            }
        }

        /// <summary>
        /// Gets the logger.
        /// </summary>
        private static ILog Log
        {
            get
            {
                // returns the value...
                return _log;
            }
        }

        /// <summary>
        /// Gets the default connection string.
        /// </summary>
        public static string DefaultConnectionString
        {
            get
            {
                // get...
                if (DefaultDatabaseProvider == null)
                    throw new InvalidOperationException("DefaultDatabaseProvider is null.");
                ConnectionSettings settings = DefaultDatabaseProvider.GetConnectionSettings();
                if (settings == null)
                    HandleNullSettings();

                // return..
                return settings.ConnectionString;
            }
        }

        private static void HandleNullSettings()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("The default database settings have not been configured.");
            if (Runtime.IsStarted)
            {
                builder.Append("  This application stores its connection settings at: ");
                builder.Append(Runtime.Current.InstallationSettingsFilePath);
            }
            else
                builder.Append("  (The runtime has not been started.)");

            // throw...
            throw new InvalidOperationException(builder.ToString());
        }

        // mbr - 05-02-2008 - deprecated.
        //		/// <summary>
        //		/// Returns  DefaultConnectionString.
        //		/// </summary>
        //		private static bool IsDefaultConnectionStringCreated()
        //		{
        //			if(_defaultConnectionString == null)
        //				return false;
        //			else
        //				return true;
        //		}

        /// <summary>
        /// Configures the details for the default database.
        /// </summary>
        /// <param name="settings"></param>
        public static void SetDefaultDatabase(ConnectionSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException("settings");

            // defer...
            SetDefaultDatabase(settings.ConnectionType, settings.ConnectionString);
        }

        /// <summary>
        /// Configures the details for the default database.
        /// </summary>
        /// <remarks>If this is not called, on the first request the class will examine the <c>.config</c> file for these details.</remarks>
        public static void SetDefaultDatabase(Type defaultConnectionType, string defaultConnectionString)
        {
            if (defaultConnectionType == null)
                throw new ArgumentNullException("defaultConnectionType");
            if (typeof(IConnection).IsAssignableFrom(defaultConnectionType) == false &&
                typeof(IConnectionFactory).IsAssignableFrom(defaultConnectionType) == false)
                throw new ArgumentException(string.Format(Cultures.Exceptions, "Connection type of '{0}' does not IConnection or IConnectionFactory.", defaultConnectionType), "defaultConnectionType");
            if (defaultConnectionString == null)
                throw new ArgumentNullException("defaultConnectionString");
            if (defaultConnectionString.Length == 0)
                throw new ArgumentOutOfRangeException("'defaultConnectionString' is zero-length.");

            // mbr - 05-02-2008 - changed to defer to provider...
            //			_defaultConnectionType = defaultConnectionType;
            //			_defaultConnectionString = defaultConnectionString;
            if (DefaultDatabaseProvider == null)
                throw new InvalidOperationException("DefaultDatabaseProvider is null.");
            DefaultDatabaseProvider.SetConnectionSettings(defaultConnectionType, defaultConnectionString);

            HasSetDefaultDatabase = true;

            // mbr - 07-12-2005 - raise...
            OnDefaultDatabaseChanged(EventArgs.Empty);
        }

        /// <summary>
        /// Gets or sets the extensibilityprovider
        /// </summary>
        internal static DatabaseExtensibilityProvider ExtensibilityProvider
        {
            get
            {
                if (_extensibilityProvider == null)
                    throw new InvalidOperationException("_extensibilityProvider is null.");
                return _extensibilityProvider;
            }
        }

        /// <summary>
        /// Raises the <c>DefaultDatabaseChanged</c> event.
        /// </summary>
        private static void OnDefaultDatabaseChanged(EventArgs e)
        {
            if (DefaultDatabaseChanged != null)
                DefaultDatabaseChanged(null, e);
        }

        /// <summary>
        /// Creates a new connection.
        /// </summary>
        /// <returns></returns>
        public static IConnection CreateConnection()
        {
            return CreateConnection((string)null);
        }

        /// <summary>
        /// Creates a new connection.
        /// </summary>
        /// <returns></returns>
        public static IConnection CreateConnection(SqlStatement statement)
        {
            if (statement == null)
                throw new ArgumentNullException("statement");

            // defer...
            return CreateConnection(statement.DatabaseName);
        }

        internal static IConnection CreateConnection(EntityType et)
        {
            if (et.IsSystemTable && SystemDatabaseProvider != null)
                return Connection.CreateConnection(SystemDatabaseProvider.GetConnectionSettings());
            else
                return CreateConnection(et.DatabaseName);
        }

        /// <summary>
        /// Creates a new connection.
        /// </summary>
        /// <returns></returns>
        internal static IConnection CreateConnection(string databaseName)
        {
            if (databaseName == null || databaseName.Length == 0)
                return CreateConnection((NamedDatabase)null);
            else
            {
                // mbr - 18-06-2008 - do we have a provider?
                //					// find the database...
                //					namedDatabase = Databases.GetDatabase(databaseName, OnNotFound.ThrowException);

                if (NamedDatabaseProvider == null)
                    throw new InvalidOperationException("NamedDatabaseProvider is null.");

                // get...
                NamedDatabase namedDatabase = NamedDatabaseProvider.GetNamedDatabase(databaseName);
                if (namedDatabase == null)
                    throw new InvalidOperationException("namedDatabase is null.");

                // defer...
                return CreateConnection(namedDatabase);
            }
        }

        /// <summary>
        /// Creates a new connection.
        /// </summary>
        /// <returns></returns>
        private static IConnection CreateConnection(NamedDatabase database)
        {
            // recycle the thread's connection if one exists...
            IConnection connection = null;

            // TODO: fix named databases to include transaction support...
            if (database == null)
                connection = BoundConnection;

            // create a connection...
            if (connection == null)
                connection = CreateNewConnection(database);

            // return...
            return connection;
        }

        /// <summary>
        /// Creates a new connection.
        /// </summary>
        /// <returns></returns>
        internal static IConnection CreateNewConnection(string databaseName = null)
        {
            NamedDatabase database = null;
            if (databaseName != null)
            {
                database = Databases.GetDatabase(databaseName, OnNotFound.ThrowException);
                if (database == null)
                    throw new InvalidOperationException("database is null.");
            }

            // defer...
            return CreateNewConnection(database);
        }

        /// <summary>
        /// Creates a new connection.
        /// </summary>
        /// <returns></returns>
        private static IConnection CreateNewConnection(NamedDatabase database)
        {
            if (database == null)
            {
                // mbr - 05-02-2008 - pass to the default provider...

                //				// what type?
                //				if(DefaultConnectionType == null)
                //				{
                //					// check...
                //					string message = null;
                //					string path = Runtime.Current.InstallationSettingsFilePath;
                //					if(path == null || path.Length == 0)
                //						message = "The installation settings file path is not defined.";
                //					else
                //					{
                //						if(File.Exists(path))
                //							message = string.Format("An installation settings file path '{0}' DOES exist.", path);
                //						else
                //							message = string.Format("An installation settings file path '{0}' DOES NOT exist.", path);
                //					}
                //
                //					// throw...
                //					throw new InvalidOperationException(string.Format("'DefaultConnectionType' is null.  ({0})", path));
                //				}
                //
                //				// return...
                //				IConnection conn = Connection.CreateConnection(DefaultConnectionType, DefaultConnectionString);
                //				if(conn == null)
                //					throw new InvalidOperationException("conn is null.");
                //				conn.SqlMode = DefaultSqlMode;
                //				return conn;

                // return...
                if (DefaultDatabaseProvider == null)
                    throw new InvalidOperationException("DefaultDatabaseProvider is null.");
                return DefaultDatabaseProvider.CreateConnection();
            }
            else
            {
                // defer...
                return Connection.CreateConnection(database.Settings);
            }
        }

        /// <summary>
        /// Gets the type for default connections.
        /// </summary>
        public static Type DefaultConnectionType
        {
            get
            {
                // mbr - 05-02-2008 - changed to use provider...
                if (DefaultDatabaseProvider == null)
                    throw new InvalidOperationException("DefaultConnectionProvider is null.");
                ConnectionSettings settings = DefaultDatabaseProvider.GetConnectionSettings();
                if (settings == null)
                    HandleNullSettings();

                // return...
                return settings.ConnectionType;
            }
        }

        /// <summary>
        /// Gets whether the database has connection settings defined
        /// </summary>
        public static bool HasConnectionSettings
        {
            get
            {
                return DefaultConnectionType != null && DefaultConnectionString != null && DefaultConnectionString != string.Empty;
            }
        }

        // mbr - 05-02-2008 - obsolete.
        //		/// <summary>
        //		/// Returns  DefaultConnectionType.
        //		/// </summary>
        //		private static bool IsDefaultConnectionTypeCreated()
        //		{
        //			if(_defaultConnectionType == null)
        //				return false;
        //			else
        //				return true;
        //		}

        // mbr - 28-09-2007 - obsolete.		
        //		/// <summary>
        //		/// Ensures that DefaultConnectionType has been created.
        //		/// </summary>
        //		private static void EnsureDefaultConnectionTypeCreated()
        //		{
        //			if(IsDefaultConnectionTypeCreated() == false)
        //				_defaultConnectionType = CreateDefaultConnectionType();
        //		}
        //		
        //		/// <summary>
        //		/// Creates an instance of DefaultConnectionType.
        //		/// </summary>
        //		/// <remarks>This does not assign the instance to the _defaultConnectionType field</remarks>
        //		private static Type CreateDefaultConnectionType()
        //		{
        //			// get it...
        //			Type type = Runtime.Current.GlobalSettings.GetTypeValue(@"BootFX\DefaultConnectionType", null, Cultures.System, OnNotFound.ReturnNull);
        //			if(type != null)
        //			{
        //				if(typeof(Connection).IsAssignableFrom(type) == false)
        //					throw new InvalidOperationException(string.Format(Cultures.Exceptions, string.Format("The type '{0}' does not appear to extend 'Connection'.", type)));
        //			}
        //
        //			// return...
        //			return type;
        //		}

        /// <summary>
        /// Executes scalar, converting to the given type and smoothing out DB null values and CLR null values.
        /// </summary>
        /// <param name="statement"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static short ExecuteScalarInt16(ISqlStatementSource statement)
        {
            if (statement == null)
                throw new ArgumentNullException("statement");

            SqlStatement realStatement = statement.GetStatement();
            if (realStatement == null)
                throw new InvalidOperationException("realStatement is null.");

            // create...
            using (IConnection connection = CreateConnection(realStatement))
                return connection.ExecuteScalarInt16(realStatement);
        }

        /// <summary>
        /// Executes scalar, converting to the given type and smoothing out DB null values and CLR null values.
        /// </summary>
        /// <param name="statement"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static long ExecuteScalarInt64(ISqlStatementSource statement)
        {
            if (statement == null)
                throw new ArgumentNullException("statement");

            SqlStatement realStatement = statement.GetStatement();
            if (realStatement == null)
                throw new InvalidOperationException("realStatement is null.");

            // create...
            using (IConnection connection = CreateConnection(realStatement))
                return connection.ExecuteScalarInt64(realStatement);
        }

        /// <summary>
        /// Executes scalar, converting to the given type and smoothing out DB null values and CLR null values.
        /// </summary>
        /// <param name="statement"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Guid ExecuteScalarGuid(ISqlStatementSource statement)
        {
            if (statement == null)
                throw new ArgumentNullException("statement");

            SqlStatement realStatement = statement.GetStatement();
            if (realStatement == null)
                throw new InvalidOperationException("realStatement is null.");

            // create...
            using (IConnection connection = CreateConnection(realStatement))
                return connection.ExecuteScalarGuid(realStatement);
        }

        /// <summary>
        /// Executes scalar, converting to the given type and smoothing out DB null values and CLR null values.
        /// </summary>
        /// <param name="statement"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static float ExecuteScalarSingle(ISqlStatementSource statement)
        {
            if (statement == null)
                throw new ArgumentNullException("statement");

            SqlStatement realStatement = statement.GetStatement();
            if (realStatement == null)
                throw new InvalidOperationException("realStatement is null.");

            // create...
            using (IConnection connection = CreateConnection(realStatement))
                return connection.ExecuteScalarSingle(realStatement);
        }

        /// <summary>
        /// Executes scalar, converting to the given type and smoothing out DB null values and CLR null values.
        /// </summary>
        /// <param name="statement"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static double ExecuteScalarDouble(ISqlStatementSource statement)
        {
            if (statement == null)
                throw new ArgumentNullException("statement");

            SqlStatement realStatement = statement.GetStatement();
            if (realStatement == null)
                throw new InvalidOperationException("realStatement is null.");

            // create...
            using (IConnection connection = CreateConnection(realStatement))
                return connection.ExecuteScalarDouble(realStatement);
        }

        /// <summary>
        /// Executes scalar, converting to the given type and smoothing out DB null values and CLR null values.
        /// </summary>
        /// <param name="statement"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool ExecuteScalarBoolean(ISqlStatementSource statement)
        {
            if (statement == null)
                throw new ArgumentNullException("statement");

            SqlStatement realStatement = statement.GetStatement();
            if (realStatement == null)
                throw new InvalidOperationException("realStatement is null.");

            // create...
            using (IConnection connection = CreateConnection(realStatement))
                return connection.ExecuteScalarBoolean(realStatement);
        }

        /// <summary>
        /// Executes scalar, converting to the given type and smoothing out DB null values and CLR null values.
        /// </summary>
        /// <param name="statement"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static byte ExecuteScalarByte(ISqlStatementSource statement)
        {
            if (statement == null)
                throw new ArgumentNullException("statement");

            SqlStatement realStatement = statement.GetStatement();
            if (realStatement == null)
                throw new InvalidOperationException("realStatement is null.");

            // create...
            using (IConnection connection = CreateConnection(realStatement))
                return connection.ExecuteScalarByte(realStatement);
        }

        /// <summary>
        /// Executes scalar, converting to the given type and smoothing out DB null values and CLR null values.
        /// </summary>
        /// <param name="statement"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static DateTime ExecuteScalarDateTime(ISqlStatementSource statement)
        {
            if (statement == null)
                throw new ArgumentNullException("statement");

            SqlStatement realStatement = statement.GetStatement();
            if (realStatement == null)
                throw new InvalidOperationException("realStatement is null.");

            // create...
            using (IConnection connection = CreateConnection(realStatement))
                return connection.ExecuteScalarDateTime(realStatement);
        }

        /// <summary>
        /// Executes scalar, converting to the given type and smoothing out DB null values and CLR null values.
        /// </summary>
        /// <param name="statement"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string ExecuteScalarString(ISqlStatementSource statement)
        {
            if (statement == null)
                throw new ArgumentNullException("statement");

            SqlStatement realStatement = statement.GetStatement();
            if (realStatement == null)
                throw new InvalidOperationException("realStatement is null.");

            // create...
            using (IConnection connection = CreateConnection(realStatement))
                return connection.ExecuteScalarString(realStatement);
        }

        public static int ExecuteScalarInt32(string sql, params object[] sqlParams)
        {
            return ExecuteScalarInt32(new SqlStatement(sql, sqlParams));
        }

        /// <summary>
        /// Executes scalar, converting to the given type and smoothing out DB null values and CLR null values.
        /// </summary>
        /// <param name="statement"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static int ExecuteScalarInt32(ISqlStatementSource statement)
        {
            if (statement == null)
                throw new ArgumentNullException("statement");

            SqlStatement realStatement = statement.GetStatement();
            if (realStatement == null)
                throw new InvalidOperationException("realStatement is null.");

            // create...
            using (IConnection connection = CreateConnection(realStatement))
                return connection.ExecuteScalarInt32(realStatement);
        }

        /// <summary>
        /// Executes scalar, converting to the given type and smoothing out DB null values and CLR null values.
        /// </summary>
        /// <param name="statement"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static decimal ExecuteScalarDecimal(ISqlStatementSource statement)
        {
            if (statement == null)
                throw new ArgumentNullException("statement");

            SqlStatement realStatement = statement.GetStatement();
            if (realStatement == null)
                throw new InvalidOperationException("realStatement is null.");

            // create...
            using (IConnection connection = CreateConnection(realStatement))
                return connection.ExecuteScalarDecimal(realStatement);
        }

        /// <summary>
        /// Executes scalar, converting to the given type and smoothing out DB null values and CLR null values.
        /// </summary>
        /// <param name="statement"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object ExecuteScalar(ISqlStatementSource statement, Type type)
        {
            if (statement == null)
                throw new ArgumentNullException("statement");

            SqlStatement realStatement = statement.GetStatement();
            if (realStatement == null)
                throw new InvalidOperationException("realStatement is null.");

            // create...
            using (IConnection connection = CreateConnection(realStatement))
                return connection.ExecuteScalar(realStatement);
        }

        /// <summary>
        /// Executes the given statement.
        /// </summary>
        /// <param name="statement"></param>
        public static object ExecuteScalar(ISqlStatementSource statement)
        {
            return ExecuteScalar(statement, null);
        }

        public static int ExecuteNonQuery(string sql, params object[] sqlParams)
        {
            return Database.ExecuteNonQuery(new SqlStatement(sql, sqlParams));
        }

        /// <summary>
        /// Executes the given statement.
        /// </summary>
        /// <param name="statement"></param>
        public static int ExecuteNonQuery(ISqlStatementSource statement)
        {
            if (statement == null)
                throw new ArgumentNullException("statement");

            SqlStatement realStatement = statement.GetStatement();
            if (realStatement == null)
                throw new InvalidOperationException("realStatement is null.");

            // create...
            using (IConnection connection = CreateConnection(realStatement))
            {
                // run...
                return connection.ExecuteNonQuery(realStatement);
            }
        }

        /// <summary>
        /// Executes the given statement.
        /// </summary>
        /// <param name="statement"></param>
        public static DataSet ExecuteDataSet(ISqlStatementSource statement)
        {
            if (statement == null)
                throw new ArgumentNullException("statement");

            SqlStatement realStatement = statement.GetStatement();
            if (realStatement == null)
                throw new InvalidOperationException("realStatement is null.");

            // create...
            using (IConnection connection = CreateConnection(realStatement))
            {
                // run...
                return connection.ExecuteDataSet(realStatement);
            }
        }

        /// <summary>
        /// Executes the given statement.
        /// </summary>
        /// <param name="statement"></param>
        public static float[] ExecuteValuesVerticalFloat(ISqlStatementSource statement)
        {
            if (statement == null)
                throw new ArgumentNullException("statement");

            // get it...
            SqlStatement realStatement = statement.GetStatement();
            if (realStatement == null)
                throw new InvalidOperationException("realStatement is null.");

            // create...
            using (IConnection connection = CreateConnection(realStatement))
            {
                // run...
                return connection.ExecuteValuesVerticalFloat(realStatement);
            }
        }

        /// <summary>
        /// Executes the given statement.
        /// </summary>
        /// <param name="statement"></param>
        public static double[] ExecuteValuesVerticalDouble(ISqlStatementSource statement)
        {
            if (statement == null)
                throw new ArgumentNullException("statement");

            // get it...
            SqlStatement realStatement = statement.GetStatement();
            if (realStatement == null)
                throw new InvalidOperationException("realStatement is null.");

            // create...
            using (IConnection connection = CreateConnection(realStatement))
            {
                // run...
                return connection.ExecuteValuesVerticalDouble(realStatement);
            }
        }

        public static bool[] ExecuteValuesVerticalBoolean(string sql, params object[] sqlParams)
        {
            return ExecuteValuesVerticalBoolean(new SqlStatement(sql, sqlParams));
        }

        /// <summary>
        /// Executes the given statement.
        /// </summary>
        /// <param name="statement"></param>
        public static bool[] ExecuteValuesVerticalBoolean(ISqlStatementSource statement)
        {
            if (statement == null)
                throw new ArgumentNullException("statement");

            // get it...
            SqlStatement realStatement = statement.GetStatement();
            if (realStatement == null)
                throw new InvalidOperationException("realStatement is null.");

            // create...
            using (IConnection connection = CreateConnection(realStatement))
            {
                // run...
                return connection.ExecuteValuesVerticalBoolean(realStatement);
            }
        }

        /// <summary>
        /// Executes the given statement.
        /// </summary>
        /// <param name="statement"></param>
        public static string[] ExecuteValuesVerticalString(ISqlStatementSource statement)
        {
            if (statement == null)
                throw new ArgumentNullException("statement");

            // get it...
            SqlStatement realStatement = statement.GetStatement();
            if (realStatement == null)
                throw new InvalidOperationException("realStatement is null.");

            // create...
            using (IConnection connection = CreateConnection(realStatement))
            {
                // run...
                return connection.ExecuteValuesVerticalString(realStatement);
            }
        }

        public static IEnumerable<string> ExecuteValuesVerticalStringAndTrimEnd(ISqlStatementSource statement)
        {
            var values = ExecuteValuesVerticalString(statement);
            return values.TrimEnd();
        }

        public static IEnumerable<string> ExecuteValuesVerticalStringAndTrimStart(ISqlStatementSource statement)
        {
            var values = ExecuteValuesVerticalString(statement);
            return values.TrimStart();
        }

        public static IEnumerable<string> ExecuteValuesVerticalStringAndTrim(ISqlStatementSource statement)
        {
            var values = ExecuteValuesVerticalString(statement);
            return values.Trim();
        }

        /// <summary>
        /// Executes the given statement.
        /// </summary>
        /// <param name="statement"></param>
        public static short[] ExecuteValuesVerticalInt16(ISqlStatementSource statement)
        {
            if (statement == null)
                throw new ArgumentNullException("statement");

            // get it...
            SqlStatement realStatement = statement.GetStatement();
            if (realStatement == null)
                throw new InvalidOperationException("realStatement is null.");

            // create...
            using (IConnection connection = CreateConnection(realStatement))
            {
                // run...
                return connection.ExecuteValuesVerticalInt16(realStatement);
            }
        }

        /// <summary>
        /// Executes the given statement.
        /// </summary>
        /// <param name="statement"></param>
        public static int[] ExecuteValuesVerticalInt32(ISqlStatementSource statement)
        {
            if (statement == null)
                throw new ArgumentNullException("statement");

            // get it...
            SqlStatement realStatement = statement.GetStatement();
            if (realStatement == null)
                throw new InvalidOperationException("realStatement is null.");

            // create...
            using (IConnection connection = CreateConnection(realStatement))
            {
                // run...
                return connection.ExecuteValuesVerticalInt32(realStatement);
            }
        }

        /// <summary>
        /// Executes the given statement.
        /// </summary>
        /// <param name="statement"></param>
        public static long[] ExecuteValuesVerticalInt64(ISqlStatementSource statement)
        {
            if (statement == null)
                throw new ArgumentNullException("statement");

            // get it...
            SqlStatement realStatement = statement.GetStatement();
            if (realStatement == null)
                throw new InvalidOperationException("realStatement is null.");

            // create...
            using (IConnection connection = CreateConnection(realStatement))
            {
                // run...
                return connection.ExecuteValuesVerticalInt64(realStatement);
            }
        }

        public static IEnumerable<T> ExecuteValuesVertical<T>(string commandText, params object[] parameters)
        {
            return ExecuteValuesVertical<T>(new SqlStatement(commandText, parameters));
        }

        public static IEnumerable<T> ExecuteValuesVertical<T>(ISqlStatementSource statement)
        {
            // get it...
            SqlStatement realStatement = statement.GetStatement();
            if (realStatement == null)
                throw new InvalidOperationException("realStatement is null.");

            // create...
            using (IConnection connection = CreateConnection(realStatement))
                return connection.ExecuteValuesVertical<T>(realStatement);
        }

        /// <summary>
        /// Executes the given statement.
        /// </summary>
        /// <param name="statement"></param>
        public static object[] ExecuteValuesVertical(ISqlStatementSource statement)
        {
            if (statement == null)
                throw new ArgumentNullException("statement");

            // get it...
            SqlStatement realStatement = statement.GetStatement();
            if (realStatement == null)
                throw new InvalidOperationException("realStatement is null.");

            // create...
            using (IConnection connection = CreateConnection(realStatement))
                return connection.ExecuteValuesVertical(realStatement);
        }

        /// <summary>
        /// Executes the given statement.
        /// </summary>
        /// <param name="statement"></param>
        public static object[] ExecuteValues(ISqlStatementSource statement)
        {
            if (statement == null)
                throw new ArgumentNullException("statement");

            // get it...
            SqlStatement realStatement = statement.GetStatement();
            if (realStatement == null)
                throw new InvalidOperationException("realStatement is null.");

            // create...
            using (IConnection connection = CreateConnection(realStatement))
            {
                // run...
                return connection.ExecuteValues(realStatement);
            }
        }

        /// <summary>
        /// Executes the given statement.
        /// </summary>
        /// <param name="statement"></param>
        public static DataTable ExecuteDataTable(ISqlStatementSource statement)
        {
            if (statement == null)
                throw new ArgumentNullException("statement");

            SqlStatement realStatement = statement.GetStatement();
            if (realStatement == null)
                throw new InvalidOperationException("realStatement is null.");

            // create...
            using (IConnection connection = CreateConnection(realStatement))
            {
                // run...
                return connection.ExecuteDataTable(realStatement);
            }
        }

        /// <summary>
        /// Executes the given statement.
        /// </summary>
        /// <param name="statement"></param>
        public static object ExecuteEntity(ISqlStatementSource statement)
        {
            return ExecuteEntity(statement, OnNotFound.ReturnNull);
        }

        /// <summary>
        /// Executes the given statement.
        /// </summary>
        /// <param name="statement"></param>
        public static object ExecuteEntity(ISqlStatementSource statement, OnNotFound onNotFound)
        {
            if (statement == null)
                throw new ArgumentNullException("statement");

            SqlStatement realStatement = statement.GetStatement();
            if (realStatement == null)
                throw new InvalidOperationException("realStatement is null.");

            // create...
            using (IConnection connection = CreateConnection(realStatement))
            {
                // run...
                return connection.ExecuteEntity(realStatement, onNotFound);
            }
        }

        /// <summary>
        /// Executes the given statement.
        /// </summary>
        /// <param name="statement"></param>
        public static IList ExecuteEntityCollection(ISqlStatementSource statement)
        {
            if (statement == null)
                throw new ArgumentNullException("statement");

            SqlStatement realStatement = statement.GetStatement();
            if (realStatement == null)
                throw new InvalidOperationException("realStatement is null.");

            // create...
            using (IConnection connection = CreateConnection(realStatement))
            {
                // run...
                return connection.ExecuteEntityCollection(realStatement);
            }
        }

        /// <summary>
        /// Gets the semi-colon separated components of the given connection string.
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public static IDictionary GetConnectionStringParts(string connectionString)
        {
            if (connectionString == null)
                throw new ArgumentNullException("connectionString");
            if (connectionString.Length == 0)
                throw new ArgumentOutOfRangeException("'connectionString' is zero-length.");

            // split...
            return Connection.SplitConnectionString(connectionString);
        }

        /// <summary>
        /// Initializes a transaction.
        /// </summary>
        /// <returns></returns>
        public static TransactionState StartTransaction()
        {
            TransactionState state = new TransactionState();
            return state;
        }

        /// <summary>
        /// Starts a transaction on the thread.
        /// </summary>
        internal static bool BeginTransactionInternal()
        {
            return BeginTransactionInternal(IsolationLevel.Serializable, true);
        }

        /// <summary>
        /// Starts a transaction on the thread.
        /// </summary>
        internal static bool BeginTransactionInternal(IsolationLevel isolationLevel)
        {
            return BeginTransactionInternal(isolationLevel, false);
        }

        private static bool BeginTransactionInternal(IsolationLevel isolationLevel, bool useDefault)
        {
            // do we have one?
            Bind();

            // begin...
            if (useDefault)
                return BoundConnection.BeginTransaction();
            else
                return BoundConnection.BeginTransaction(isolationLevel);
        }

        /// <summary>
        /// Rollsback a transaction on the thread.
        /// </summary>
        // mbr - 30-08-2007 - case 623 - added overload.		
        internal static bool RollbackInternal(Exception causedByException)
        {
            // mbr - 30-08-2007 - case 623 - added try...catch			
            try
            {
                // check...
                IConnection connection = BoundConnection;
                if (connection == null)
                {
                    // mbr - 2010-02-18 - changed...
                    //throw new InvalidOperationException("Current thread does not have a connection.");
                    throw Connection.CreateTransactionHandlingException("Current thread does not have a connection.");
                }

                // return...
                if (connection.Rollback())
                {
                    Unbind();
                    return true;
                }
                else
                {
                    Unbind();
                    return false;
                }
            }
            catch (Exception ex)
            {
                // do we have a caused by?
                StringBuilder builder = new StringBuilder();
                builder.Append("The rollback operation failed.  ");
                const string innerMessage = "The InnerException represents the exception from the rollback operation's internals.";
                if (causedByException != null)
                {
                    builder.Append("An exception was supplied.  ");
                    builder.Append(innerMessage);

                    // add the inner exception...
                    const string sep = "\r\n========================================\r\n";
                    builder.Append(sep);
                    builder.Append("The exception supplied through 'causedByException' reads:\r\n\t");
                    builder.Append(causedByException);
                    builder.Append(sep);
                }
                else
                {
                    builder.Append("No exception was supplied.  ");
                    builder.Append(innerMessage);
                }

                // throw...
                throw new InvalidOperationException(builder.ToString(), ex);
            }
        }

        /// <summary>
        /// Commits a transaction on the thread.
        /// </summary>
        internal static bool CommitInternal()
        {
            IConnection connection = BoundConnection;
            if (connection == null)
            {
                // mbr - 2010-02-18 - change...
                //throw new InvalidOperationException("Current thread does not have a connection.");
                throw Connection.CreateTransactionHandlingException("Current thread does not have a connection.");
            }

            // mbr - 13-02-2007 - found a major bug here with nested transactions - EndTransaction 
            // is deprecated, and Unbind needs to be called per call.

            // return...
            if (connection.Commit() == true)
            {
                // mbr - 13-02-2007 - as above...
                Unbind();
                //				EndTransaction();
                return true;
            }
            else
            {
                // mbr - 13-02-2007 - as above...
                Unbind();
                return false;
            }
        }

        public static void Unbind()
        {
            // get and dispose...
            if (BoundConnection == null)
            {
                // mbr - 2010-02-18 - changed...
                // throw new ArgumentNullException("BoundConnection");
                throw Connection.CreateTransactionHandlingException("BoundConnection is null.");
            }

            // stop...
            if (BoundConnection.Unbind())
            {
                BoundConnection.Dispose();

                // remove...
                _boundConnection = null;
                _affinityState = null;
            }
        }

        /// <summary>
        /// Binds the given connection.
        /// </summary>
        /// <param name="connection"></param>
        public static IDisposable Bind(IConnection connection)
        {
            return Bind(connection, null);
        }

        /// <summary>
        /// Binds the given connection.
        /// </summary>
        /// <param name="connection"></param>
        public static IDisposable Bind(IConnection connection, object affinityState)
        {
            if (connection == null)
                throw new ArgumentNullException("connection");

            // do we have one?
            if (_boundConnection != null)
            {
                // mbr - 2010-02-18 - changed...
                //throw new InvalidOperationException("A connection has already been bound.");
                throw Connection.CreateTransactionHandlingException("A connection has already been bound.");
            }

            // set...
            _boundConnection = connection;
            _affinityState = affinityState;
            BoundConnection.Bind();

            // mbr - 2014-11-30 - added...
            return new Unbinder();
        }

        public static IDisposable Bind()
        {
            // do we have one?
            if (BoundConnection == null)
                _boundConnection = CreateNewConnection((string)null);
            BoundConnection.Bind();

            // mbr - 2014-11-30 - added...
            return new Unbinder();
        }

        private class Unbinder : IDisposable
        {
            public void Dispose()
            {
                Unbind();
            }
        }

        /// <summary>
        /// Starts a transaction on the thread.
        /// </summary>
        public static bool IsInTransaction
        {
            get
            {
                if (BoundConnection == null)
                    return false;
                else
                    return true;
            }
        }

        /// <summary>
        /// Gets the boundconnection.
        /// </summary>
        private static IConnection BoundConnection
        {
            get
            {
                // returns the value...
                return _boundConnection;
            }
        }

        /// <summary>
        /// Gets the current transaction ID.
        /// </summary>
        public static Guid TransactionId
        {
            get
            {
                if (BoundConnection != null)
                    return BoundConnection.TransactionId;
                else
                    return Guid.Empty;
            }
        }

        // JM - 2007-06-02 - for use in Entity.SetupAffinity()
        public static bool IsBound
        {
            get
            {
                return BoundConnection != null;
            }
        }

        /// <summary>
        /// Gets the schema for the database.
        /// </summary>
        /// <returns></returns>
        public static bool DoesTableExist(string nativeName)
        {
            // get...
            using (IConnection connection = CreateConnection((NamedDatabase)null))
                return connection.DoesTableExist(nativeName);
        }

        /// <summary>
        /// Gets the schema for the database.
        /// </summary>
        /// <returns></returns>
        public static bool DoesTableExist(NativeName nativeName)
        {
            // get...
            using (IConnection connection = CreateConnection((NamedDatabase)null))
                return connection.DoesTableExist(nativeName);
        }

        /// <summary>
        /// Returns true if a bound connection is available
        /// </summary>
        /// <returns></returns>
        public static bool HasDefaultDatabaseSettings()
        {
            // mbr - 05-02-2008 - changed to use provider...
            //			if(_defaultConnectionType != null && _defaultConnectionString != null && _defaultConnectionString.Length > 0)
            //				return true;
            //			else
            //				return false;

            // get...
            if (DefaultDatabaseProvider == null)
                throw new InvalidOperationException("DefaultDatabaseProvider is null.");
            ConnectionSettings settings = DefaultDatabaseProvider.GetConnectionSettings();
            if (settings == null)
                return false;

            // check...
            if (settings.ConnectionType == null || settings.ConnectionString == null || settings.ConnectionString.Length == 0)
                return false;
            else
                return true;
        }

        public static ConnectionSettings GetDefaultConnectionSettings()
        {
            // mbr - 05-02-2008 - changed to use provider...
            //			if(HasDefaultDatabaseSettings())
            //				return new ConnectionSettings(null, DefaultConnectionType, DefaultConnectionString);
            //			else
            //				return null;

            // return...
            if (DefaultDatabaseProvider == null)
                throw new InvalidOperationException("DefaultDatabaseProvider is null.");
            return DefaultDatabaseProvider.GetConnectionSettings();
        }

        /// <summary>
        /// Gets the schema for the database.
        /// </summary>
        /// <returns></returns>
        [Obsolete("Use the overload that takes an arguments object.")]
        public static SqlSchema GetSchema()
        {
            return GetSchema(null, new GetSchemaArgs());
        }

        /// <summary>
        /// Gets the schema for the database.
        /// </summary>
        /// <returns></returns>
        // mbr - 02-10-2007 - for c7 - added search specification.		
        public static SqlSchema GetSchema(GetSchemaArgs args)
        {
            return GetSchema(null, args);
        }

        /// <summary>
        /// Gets the schema for the database.
        /// </summary>
        /// <returns></returns>
        // mbr - 02-10-2007 - added search specification.		
        private static SqlSchema GetSchema(IOperationItem operation, GetSchemaArgs args)
        {
            if (args == null)
                throw new ArgumentNullException("args");

            // mbr - 2014-12-03 - database...
            NamedDatabase db = null;
            if (!(string.IsNullOrEmpty(args.DatabaseName)))
                db = Databases[args.DatabaseName];

            // get...
            using (IConnection connection = CreateConnection(db))
                return connection.GetSchema(args);
        }

        /// <summary>
        /// Gets the default connection settings.
        /// </summary>
        public static ConnectionSettings DefaultConnectionSettings
        {
            get
            {
                if (DefaultConnectionType != null && DefaultConnectionString != null && DefaultConnectionString.Length > 0)
                    return new ConnectionSettings(ConnectionSettings.DefaultName, DefaultConnectionType, DefaultConnectionString);
                else
                    return null;
            }
        }

        /// <summary>
        /// Gets the databases.
        /// </summary>
        public static NamedDatabaseCollection Databases
        {
            get
            {
                // returns the value...
                return _databases;
            }
        }

        /// <summary>
        /// Gets or sets the defaultsqlmode
        /// </summary>
        public static SqlMode DefaultSqlMode
        {
            get
            {
                return _defaultSqlMode;
            }
            set
            {
                // check to see if the value has changed...
                if (value != _defaultSqlMode)
                {
                    // set the value...
                    _defaultSqlMode = value;
                }
            }
        }
        /// <summary>
        /// Sets the default database from the installation settings.
        /// </summary>
        public static void SetDefaultDatabaseFromInstallationSettings()
        {
            if (Runtime.Current == null)
                throw new InvalidOperationException("Runtime.Current is null.");
            if (Runtime.Current.InstallationSettings == null)
                throw new InvalidOperationException("Runtime.Current.InstallationSettings is null.");

            // defer...
            SetDefaultDatabaseFromInstallationSettings(Runtime.Current.InstallationSettings);
        }

        /// <summary>
        /// Sets the default database from the installation settings.
        /// </summary>
        // mbr - 25-09-2007 - added args...
        internal static void SetDefaultDatabaseFromInstallationSettings(InstallationSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException("settings");

            // check...
            if (!(settings.IsDatabaseProperlyConfigured))
                throw new InvalidOperationException("Database settings are not properly configured in the installation settings.");

            // set...
            SetDefaultDatabase(settings.ConnectionType, settings.ConnectionString);
        }

        /// <summary>
        /// Copies a BLOB value to a stream.
        /// </summary>
        /// <param name="field"></param>
        /// <param name="outStream"></param>
        public static long CopyBlobValueToStream(EntityField field, object[] keyValues, Stream outStream)
        {
            if (field == null)
                throw new ArgumentNullException("field");
            if (keyValues == null)
                throw new ArgumentNullException("keyValues");
            if (keyValues.Length == 0)
                throw new ArgumentOutOfRangeException("'keyValues' is zero-length.");
            if (outStream == null)
                throw new ArgumentNullException("outStream");

            // get...
            if (field.EntityType == null)
                throw new InvalidOperationException("field.EntityType is null.");
            using (IConnection connection = CreateConnection(field.EntityType.DatabaseName))
                return connection.CopyBlobValueToStream(field, keyValues, outStream);
        }

        /// <summary>
        /// Copies a stream to a BLOB value in the given field
        /// </summary>
        /// <param name="field"></param>
        /// <param name="keyValues"></param>
        /// <param name="stream"></param>
        public static void CopyStreamToBlobValue(EntityField storageField, object[] keyValues, Stream inStream)
        {
            if (storageField == null)
                throw new ArgumentNullException("storageField");
            if (keyValues == null)
                throw new ArgumentNullException("keyValues");
            if (keyValues.Length == 0)
                throw new ArgumentOutOfRangeException("'keyValues' is zero-length.");
            if (inStream == null)
                throw new ArgumentNullException("inStream");

            // get...
            if (storageField.EntityType == null)
                throw new InvalidOperationException("field.EntityType is null.");
            using (IConnection connection = CreateConnection(storageField.EntityType.DatabaseName))
                connection.CopyStreamToBlobValue(storageField, keyValues, inStream);
        }

        ///// <summary>
        ///// Gets or sets the commanddumplevel
        ///// </summary>
        //public static DatabaseCommandDumpLevel CommandDumpLevel
        //{
        //    get
        //    {
        //        return _commandDumpLevel;
        //    }
        //    set
        //    {
        //        // check to see if the value has changed...
        //        if (value != _commandDumpLevel)
        //        {
        //            // set the value...
        //            _commandDumpLevel = value;
        //        }
        //    }
        //}

        /// <summary>
        /// Gets the affinitystate.
        /// </summary>
        internal static object AffinityState
        {
            get
            {
                return _affinityState;
            }
        }

        /// <summary>
        /// Sets the extensibility provider to use.
        /// </summary>
        /// <param name="factory"></param>
        internal static void SetExtensibilityProvider(IDatabaseExtensibilityProviderFactory factory)
        {
            if (factory != null)
            {
                // call it...
                DatabaseExtensibilityProvider provider = factory.GetProvider();
                if (provider == null)
                    throw new InvalidOperationException(string.Format("Extensibility provider factory of type '{0}' returned null.", factory.GetType()));

                // set...
                _extensibilityProvider = provider;
            }
            else
                _extensibilityProvider = new FlatTableExtensibilityProvider();
        }

        /// <summary>
        /// Executes the given statement, wrapping it in an IF EXISTS statement and returning a boolean.
        /// </summary>
        /// <param name="statementSource"></param>
        /// <returns></returns>
        // mbr - 28-09-2007 - added.		
        public static bool ExecuteExists(ISqlStatementSource statement)
        {
            if (statement == null)
                throw new ArgumentNullException("statement");

            // get...
            SqlStatement realStatement = statement.GetStatement();
            if (realStatement == null)
                throw new InvalidOperationException("realStatement is null.");

            // create...
            using (IConnection connection = CreateConnection(realStatement))
            {
                // run...
                return connection.ExecuteExists(realStatement);
            }
        }

        /// <summary>
        /// Gets or sets the defaultdatabaseprovider
        /// </summary>
        public static IDefaultDatabaseProvider DefaultDatabaseProvider
        {
            get
            {
                return _defaultDatabaseProvider;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                // check to see if the value has changed...
                if (value != _defaultDatabaseProvider)
                {
                    // set the value...
                    _defaultDatabaseProvider = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the nameddatabaseprovider
        /// </summary>
        public static INamedDatabaseProvider NamedDatabaseProvider
        {
            get
            {
                return _namedDatabaseProvider;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                // check to see if the value has changed...
                if (value != _namedDatabaseProvider)
                {
                    // set the value...
                    _namedDatabaseProvider = value;
                }
            }
        }

        /// <summary>
        /// Waits for the default database to become available.  
        /// </summary>
        /// <remarks>This method was added to support service startup on hosted servers where SQL Server often doesn't come up first.</remarks>
        /// <param name="period"></param>
        public static bool WaitForDefaultDatabase(int seconds, bool throwOnTimeout)
        {
            return WaitForDefaultDatabase(new TimeSpan(0, 0, seconds), throwOnTimeout);
        }

        /// <summary>
        /// Waits for the default database to become available.  
        /// </summary>
        /// <remarks>This method was added to support service startup on hosted servers where SQL Server often doesn't come up first.</remarks>
        /// <param name="period"></param>
        public static bool WaitForDefaultDatabase(TimeSpan period, bool throwOnTimeout)
        {
            // try and connect...
            DateTime stopAt = DateTime.UtcNow.Add(period);
            bool ok = false;
            Exception lastException = null;
            Debug.WriteLine("Waiting for default database...");
            while (DateTime.UtcNow < stopAt)
            {
                // try...
                try
                {
                    // create...
                    Debug.WriteLine("\tTrying connection...");
                    using (IConnection conn = CreateConnection())
                        conn.EnsureConnectivity();

                    // flag...
                    Debug.WriteLine("\tConnection OK!");
                    ok = true;
                    break;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("\tConnection failed - " + ex.Message);

                    // log...
                    ILog log = LogSet.GetLog(typeof(Database));
                    if (log.IsWarnEnabled)
                        log.Warn(string.Format("Failed to establish database connection.  Will keep trying until {0} UTC.", stopAt), ex);

                    // set...
                    lastException = ex;
                    ok = false;
                }

                // wait...
                Thread.Sleep(2500);
            }

            // did we do it?
            if (ok)
                return true;
            else
            {
                if (throwOnTimeout)
                    throw new InvalidOperationException("A connection to the database could not be established within the timeout period.", lastException);
                else
                    return false;
            }
        }

        /// <summary>
        /// Gets the StatementCallback value.
        /// </summary>
        public static ISqlStatementCallback StatementCallback
        {
            get
            {
                return _statementCallback;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                _statementCallback = value;
            }
        }

        internal static void SetConfigDatabase(Type type, string connString)
        {
            if (type == null)
                throw new ArgumentNullException("type");
            if (connString == null)
                throw new ArgumentNullException("connString");
            if (connString.Length == 0)
                throw new ArgumentException("'connString' is zero-length.");

            // set...
            Databases.Add(new NamedDatabase(ConfigDatabaseKey, new ConnectionSettings(ConfigDatabaseKey, type, connString)));

            // set...
            EntityType et = EntityType.GetEntityType(typeof(ConfigItem), OnNotFound.ThrowException);
            if (et == null)
                throw new InvalidOperationException("'et' is null.");
            et.DatabaseName = ConfigDatabaseKey;
        }

        internal static IEnumerable<T> ExecuteValues<T>(ISqlStatementSource sql)
        {
            var results = new List<T>();
            foreach (var value in ExecuteValuesVertical(sql))
                results.Add(ConversionHelper.ChangeType<T>(value));

            return results;
        }

        public static bool DoesColumnExist(string tableName, string columnName)
        {
            //return Database.ExecuteScalarInt32("select column_id from sys.columns where name=@p0 and object_id = object_id(@p1)",
            //    columnName, tableName) != 0;

            using (IConnection connection = CreateConnection((NamedDatabase)null))
                return connection.DoesColumnExist(tableName, columnName);
        }

        public static bool DoesIndexExist(string tableName, string indexName)
        {
            //return Database.ExecuteScalarInt32("select index_id from sys.indexes where name=@p0 and object_id = object_id(@p1)",
            //    indexName, tableName) != 0;

            using (IConnection connection = CreateConnection((NamedDatabase)null))
                return connection.DoesIndexExist(tableName, indexName);
        }

        public static ArrayParameterType ArrayParameterType
        {
            get
            {
                if (!(_arrayParameterTypeSet))
                {
                    if (HasSetDefaultDatabase && typeof(SqlServerConnection).IsAssignableFrom(DefaultConnectionType))
                        return ArrayParameterType.Array;
                    else
                        return ArrayParameterType.BunchOfParameters;
                }
                else
                    return _arrayParameterType;
            }
            set
            {
                _arrayParameterType = value;
                _arrayParameterTypeSet = true;
            }
        }
    }
}
