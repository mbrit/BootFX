// BootFX - Application framework for .NET applications
// 
// File: DatabaseAppender.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Xml;
using System.Security.Principal;
using System.Web;
using System.Text;
using System.Data;
using System.Collections;
using BootFX.Common.Xml;
using BootFX.Common.Data;
using BootFX.Common.Data.Schema;
using BootFX.Common.Data.Formatters;
using BootFX.Common.Entities;

namespace BootFX.Common.Management
{
	/// <summary>
	/// Summary description for DatabaseExceptionReportingStrategy.
	/// </summary>
    [Obsolete("No longer supported.")]
	public class DatabaseAppender : Appender
	{
        //internal const string DefaultTableName = "BfxLog";

        //internal const int ShortMessageLength = 1024;

        //private const string LevelColumnName = "Level";

        //// mbr - 25-04-2007 - added.		
        //private const int VendorNameLength = 64;
        //private const int ProductNameLength = 64;
        //private const int ModuleNameLength = 32;
        //private const int VersionLength = 24;
        //private const int EntryAssemblyNameLength = 64;
        //private const int UrlLength = 128;
        //private const int QueryStringLength = 256;
        //private const int ThreadNameLength = 64;
        //private const int MachineNameLength = 64;
        //private const int WindowsUsernameLength = 64;

        ///// <summary>
        ///// Private field to support <see cref="NextCheckExpiration"/> property.
        ///// </summary>
        //private DateTime _nextCheckExpiration = DateTime.UtcNow;
		
        ///// <summary>
        ///// Private field to support <see cref="ConnectionSettings"/> property.
        ///// </summary>
        //private ConnectionSettings _connectionSettings;
		
        ///// <summary>
        ///// Private field to support <c>TableExists</c> property.
        ///// </summary>
        //private bool _tableExists;
		
        ///// <summary>
        ///// Private field to support <c>TableExistenceLookup</c> property.
        ///// </summary>
        //private static Lookup _tableExistenceLookup;
		
        ///// <summary>
        ///// Private field to support <c>FxVersion</c> property.
        ///// </summary>
        //private static Version _fxVersion = new Version(0,0,0,0);
		
        ///// <summary>
        ///// Private field to support <c>TableName</c> property.
        ///// </summary>
        //private string _tableName;

        //private static object _lockUpdateSchema = new object();
		
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="tableName"></param>
		public DatabaseAppender(ILog log, ILogFormatter formatter, LogLevel minLevel, LogLevel maxLevel, ConnectionSettings connectionSettings, string tableName) 
            : base(log, formatter, minLevel, maxLevel)
		{
            //if(connectionSettings == null)
            //    throw new ArgumentNullException("connectionSettings");
            //if(tableName == null)
            //    throw new ArgumentNullException("tableName");
            //if(tableName.Length == 0)
            //    throw new ArgumentOutOfRangeException("'tableName' is zero-length.");			

            //// set...
            //_connectionSettings = connectionSettings;
            //_tableName = tableName;

            //// mbr - 07-11-2005 - lazy...
            //this.LazyWrite = true;

            //// check it exists...
            //_tableExists = (bool)TableExistenceLookup[tableName];
		}

        //static DatabaseAppender()
        //{
        //    _tableExistenceLookup = new Lookup();
        //    _tableExistenceLookup.CreateItemValue += new CreateLookupItemEventHandler(_tableExistenceLookup_CreateItemValue);
        //}
		
        //public static ILogFormatter DefaultFormatter
        //{
        //    get
        //    {
        //        return new GenericFormatter();
        //    }
        //}

        ///// <summary>
        ///// Gets the tableexistencelookup.
        ///// </summary>
        //private static Lookup TableExistenceLookup
        //{
        //    get
        //    {
        //        // returns the value...
        //        return _tableExistenceLookup;
        //    }
        //}

        ///// <summary>
        ///// Checks for expired messages.
        ///// </summary>
        //private void CheckExpiration()
        //{
        //    if(DateTime.UtcNow >= NextCheckExpiration)
        //    {
        //        try
        //        {
        //            // settings...
        //            InstallationSettings settings = Runtime.Current.InstallationSettings;
        //            if(settings == null)
        //                throw new InvalidOperationException("settings is null.");

        //            // mbr - 20-08-2007 - changed to support audit items and refactored cleanup to new method.					
        //            int expiration = settings.MaxDatabaseLogHistoryDays;
        //            if(expiration > 0)
        //                this.CleanupLog(settings, expiration, false);

        //            // now do audit...
        //            expiration = settings.MaxDatabaseAuditLogHistoryDays;
        //            if(expiration > 0)
        //                this.CleanupLog(settings, expiration, true);
        //        }
        //        catch(Exception ex)
        //        {
        //            if(this.InternalLog.IsErrorEnabled)
        //                this.InternalLog.Error("Failed to handle expiration check.", ex);
        //        }
        //        finally
        //        {
        //            _nextCheckExpiration = DateTime.UtcNow.AddHours(12);
        //        }
        //    }
        //}

        //private void CleanupLog(InstallationSettings settings, int expiration, bool isAuditCleanup)
        //{
        //    if(settings == null)
        //        throw new ArgumentNullException("settings");
			
        //    // get...
        //    DateTime deletePrior = DateTime.UtcNow.AddDays(0 - expiration);

        //    // run...
        //    using(IConnection connection = Connection.CreateConnection(settings.GetConnectionSettingsForLogging()))
        //    {
        //        // delete...
        //        StringBuilder builder = new StringBuilder();
        //        builder.Append(connection.Dialect.DeleteFromKeyword);
        //        builder.Append(" ");
        //        builder.Append(connection.Dialect.FormatTableName(this.TableName));
        //        builder.Append(" ");
        //        builder.Append(connection.Dialect.WhereKeyword);
        //        builder.Append(" ");
        //        builder.Append(connection.Dialect.FormatColumnName("DateTime"));
        //        builder.Append(" <= ");
        //        const string paramName = "deletePrior";
        //        builder.Append(connection.Dialect.FormatVariableNameForQueryText(paramName));

        //        // mbr - 20-08-2007 - audit...
        //        builder.Append(" ");
        //        builder.Append(connection.Dialect.AndKeyword);
        //        builder.Append(" ");
        //        builder.Append(connection.Dialect.FormatColumnName(LevelColumnName));
        //        builder.Append(" ");
        //        builder.Append(connection.Dialect.InKeyword);
        //        builder.Append(" (");
        //        if(isAuditCleanup)
        //            AddLevels(builder, LogSet.GetAuditLogLevels());
        //        else
        //            AddLevels(builder, LogSet.GetNonAuditLogLevels());
        //        builder.Append(")");
						
        //        // statement...
        //        SqlStatement statement = new SqlStatement();
        //        statement.CommandText = builder.ToString();
        //        statement.Parameters.Add(new SqlStatementParameter(paramName, DbType.DateTime, deletePrior));

        //        // run...
        //        connection.ExecuteNonQuery(statement);
        //    }
        //}

        //private static void AddLevels(StringBuilder builder, IList levels)
        //{
        //    if(builder == null)
        //        throw new ArgumentNullException("builder");
        //    if(levels == null)
        //        throw new ArgumentNullException("levels");
        //    if(levels.Count == 0)
        //        throw new ArgumentOutOfRangeException("'levels' is zero-length.");
			
        //    // walk...
        //    for(int index = 0; index < levels.Count; index++)
        //    {
        //        if(index > 0)
        //            builder.Append(",");
        //        builder.Append((int)levels[index]);
        //    }
        //}

        protected internal override void DoAppend(LogData data)
        {
        //    if(data == null)
        //        throw new ArgumentNullException("data");

        //    // expiration?
        //    CheckExpiration();

        //    // defer...
        //    if(ConnectionSettings == null)
        //        throw new InvalidOperationException("ConnectionSettings is null.");
        //    if(TableName == null)
        //        throw new InvalidOperationException("'TableName' is null.");
        //    if(TableName.Length == 0)
        //        throw new InvalidOperationException("'TableName' is zero-length.");
        //    WriteEntry(this.ConnectionSettings, this.TableName, data);
        }

        [Obsolete("No longer supported.")]
        public static void WriteEntry(LogLevel level, string message, Exception ex, bool throwOnError)
		{
            //if(throwOnError)
            //    Runtime.AssertIsStarted();
            //else if(!(Runtime.IsStarted))
            //    return;

            //try
            //{
            //    // create...
            //    LogData data = new LogData(level, message, ex, message);

            //    // send...
            //    InstallationSettings settings = Runtime.Current.InstallationSettings;
            //    if(settings == null)
            //        throw new InvalidOperationException("settings is null.");
            //    WriteEntry(settings.GetConnectionSettingsForLogging(), Runtime.Current.InstallationSettings.DatabaseLoggingTableNativeName, data);
            //}
            //catch(Exception logEx)
            //{
            //    Console.WriteLine(string.Format("------------------------------\r\nFailed to write this message:\r\nMessage: {0}\r\nException: {1}", 
            //        message, ex));

            //    // throw it?
            //    if(throwOnError)
            //        throw new InvalidOperationException("Failed to write a log message to the database.", logEx);
            //}
		}

        //internal static object WriteEntry(ConnectionSettings settings, string tableName, LogData data)
        //{
        //    if(settings == null)
        //        throw new ArgumentNullException("settings");
        //    if(tableName == null)
        //        throw new ArgumentNullException("tableName");
        //    if(tableName.Length == 0)
        //        throw new ArgumentOutOfRangeException("'tableName' is zero-length.");
        //    if(data == null)
        //        throw new ArgumentNullException("data");

        //    // application...
        //    MbrApplication app = Runtime.Current.Application;
        //    if(app == null)
        //        throw new InvalidOperationException("app is null.");

        //    // ex...
        //    string exceptionAsString = null;
        //    if(data.Exception != null)
        //        exceptionAsString = data.Exception.ToString();

        //    // asm...
        //    string entryAsmName = null;
        //    if(data.EntryAssembly != null)
        //        entryAsmName = data.EntryAssembly.GetName().Name;
        //    else
        //        entryAsmName = "<Unknown>";

        //    // write...
        //    return WriteEntryRaw(settings, tableName, 
        //        app.ProductCompany, app.ProductName, app.ProductModule, app.ProductVersion.ToString(), 
        //        FxVersion.ToString(), entryAsmName, data.EntryAssemblyVersion.ToString(), data.Level, 
        //        data.DateTime, data.IsHttpRequest, data.HttpUrl, data.HttpQueryString, data.ThreadName, 
        //        Environment.MachineName, data.IsAuthenticated, data.WindowsUsername, data.UnformattedBuf, 
        //        exceptionAsString);
        //}

        //public static object WriteEntryRaw(string vendorName, string productName, string moduleName, string appVersion, 
        //    string fxVersion, string entryAssemblyName, string entryAssemblyVersion, LogLevel level, DateTime dateTime, 
        //    bool isHttp, string httpUrl, string httpQueryString, string threadName, string machineName, bool isAuthenticated, 
        //    string windowsUsername, string message, string exceptionText)
        //{
        //    InstallationSettings settings = Runtime.Current.InstallationSettings;
        //    if(settings == null)
        //        throw new InvalidOperationException("settings is null.");

        //    // defer...
        //    return WriteEntryRaw(settings.GetConnectionSettingsForLogging(), Runtime.Current.InstallationSettings.DatabaseLoggingTableNativeName,
        //        vendorName, productName, moduleName, appVersion, fxVersion, entryAssemblyName, entryAssemblyVersion, 
        //        level, dateTime, isHttp, httpUrl, httpQueryString, threadName, machineName, isAuthenticated,
        //        windowsUsername, message, exceptionText);
        //}

        //private static object WriteEntryRaw(ConnectionSettings settings, string logTableName,
        //    string vendorName, string productName, string moduleName, string appVersion, 
        //    string fxVersion, string entryAssemblyName, string entryAssemblyVersion, LogLevel level, DateTime dateTime, 
        //    bool isHttp, string httpUrl, string httpQueryString, string threadName, string machineName, bool isAuthenticated, 
        //    string windowsUsername, string message, string exceptionText)
        //{
        //    if(settings == null)
        //        throw new ArgumentNullException("settings");
        //    if(logTableName == null)
        //        throw new ArgumentNullException("logTableName");
        //    if(logTableName.Length == 0)
        //        throw new ArgumentOutOfRangeException("'logTableName' is zero-length.");			

        //    // mbr - 07-11-2005 - if the table doesn't exist do nothing...
        //    bool exists = (bool)TableExistenceLookup[logTableName];
        //    if(!(exists))
        //        return null;
			
        //    // dialect...
        //    SqlDialect dialect = settings.GetDefaultDialect();
        //    if(dialect == null)
        //        throw new InvalidOperationException("dialect is null.");

        //    // create some sql...
        //    SqlStatement statement = new SqlStatement();
        //    statement.CommandText = GetSql(dialect, logTableName);

        //    // vendor...
        //    statement.Parameters.Add(new SqlStatementParameter("VendorName", DbType.String, EnsureSize(vendorName, VendorNameLength)));
        //    statement.Parameters.Add(new SqlStatementParameter("ProductName", DbType.String,EnsureSize(productName, ProductNameLength)));
        //    statement.Parameters.Add(new SqlStatementParameter("ModuleName", DbType.String, EnsureSize(moduleName, ModuleNameLength)));
        //    statement.Parameters.Add(new SqlStatementParameter("Version", DbType.String, EnsureSize(appVersion, VersionLength)));

        //    // level/datetime...
        //    statement.Parameters.Add(new SqlStatementParameter(LevelColumnName, DbType.Int32, (int)level));
        //    statement.Parameters.Add(new SqlStatementParameter("DateTime", DbType.DateTime, dateTime));

        //    // asms...
        //    statement.Parameters.Add(new SqlStatementParameter("EntryAssemblyName", DbType.String, EnsureSize(entryAssemblyName, EntryAssemblyNameLength)));
        //    statement.Parameters.Add(new SqlStatementParameter("EntryAssemblyVersion", DbType.String, EnsureSize(entryAssemblyVersion, VersionLength)));
        //    statement.Parameters.Add(new SqlStatementParameter("FxVersion", DbType.String, EnsureSize(fxVersion, VersionLength)));

        //    // http...
        //    statement.Parameters.Add(new SqlStatementParameter("IsHttp", DbType.Boolean, isHttp));

        //    // url...
        //    object urlAsObject = httpUrl;
        //    if(urlAsObject == null)
        //        urlAsObject = DBNull.Value;
        //    else
        //        urlAsObject = EnsureSize((string)urlAsObject, UrlLength);
        //    statement.Parameters.Add(new SqlStatementParameter("Url", DbType.String, urlAsObject));

        //    // qs...
        //    object queryAsObject = httpQueryString;
        //    if(queryAsObject == null)
        //        queryAsObject = DBNull.Value;
        //    else
        //        queryAsObject = EnsureSize((string)queryAsObject, QueryStringLength);
        //    statement.Parameters.Add(new SqlStatementParameter("QueryString", DbType.String, queryAsObject));

        //    // metric...
        //    statement.Parameters.Add(new SqlStatementParameter("ThreadName", DbType.String, EnsureSize(threadName, ThreadNameLength)));
        //    statement.Parameters.Add(new SqlStatementParameter("MachineName", DbType.String, EnsureSize(machineName, MachineNameLength)));

        //    // auth...
        //    statement.Parameters.Add(new SqlStatementParameter("IsAuthenticated", DbType.Boolean, isAuthenticated));
        //    if(isAuthenticated)
        //        statement.Parameters.Add(new SqlStatementParameter("WindowsUsername", DbType.String, EnsureSize(windowsUsername, WindowsUsernameLength)));
        //    else
        //        statement.Parameters.Add(new SqlStatementParameter("WindowsUsername", DbType.String, DBNull.Value));

        //    // message...
        //    bool isLong = true;
        //    if(message == null || message.Length <= ShortMessageLength)
        //    {
        //        statement.Parameters.Add(new SqlStatementParameter("ShortMessage", DbType.String, message));
        //        statement.Parameters.Add(new SqlStatementParameter("LongMessage", DbType.String, DBNull.Value));
        //        isLong = false;
        //    }
        //    else
        //    {
        //        statement.Parameters.Add(new SqlStatementParameter("ShortMessage", DbType.String, DBNull.Value));
        //        statement.Parameters.Add(new SqlStatementParameter("LongMessage", DbType.String, message));
        //    }
        //    statement.Parameters.Add(new SqlStatementParameter("IsLongMessage", DbType.Boolean, isLong));

        //    // exception...
        //    if(exceptionText != null && exceptionText.Length > 0)
        //    {	 
        //        statement.Parameters.Add(new SqlStatementParameter("ExceptionMessage", DbType.String, exceptionText.ToString()));
        //        statement.Parameters.Add(new SqlStatementParameter("HasExceptionMessage", DbType.Boolean, true));
        //    }
        //    else
        //    {
        //        statement.Parameters.Add(new SqlStatementParameter("ExceptionMessage", DbType.String, DBNull.Value));
        //        statement.Parameters.Add(new SqlStatementParameter("HasExceptionMessage", DbType.Boolean, false));
        //    }

        //    // we need to force this add to run outside of a transaction...
        //    using(IConnection connection = Connection.CreateConnection(settings))
        //        return connection.ExecuteScalar(statement);
        //}

        //private static string EnsureSize(string buf, int length)
        //{
        //    if(buf == null || buf.Length == 0)
        //        return buf;
        //    if(buf.Length > length)
        //        return buf.Substring(0, length);
        //    else
        //        return buf;
        //}

        ///// <summary>
        ///// Gets the tablename.
        ///// </summary>
        //private string TableName
        //{
        //    get
        //    {
        //        // returns the value...
        //        return _tableName;
        //    }
        //}

        ///// <summary>
        ///// Gets the dialect for the statement.
        ///// </summary>
        //private SqlDialect Dialect
        //{
        //    get
        //    {
        //        return Database.DefaultDialect;
        //    }
        //}

        ///// <summary>
        ///// Gets the fxversion.
        ///// </summary>
        //private static Version FxVersion
        //{
        //    get
        //    {
        //        // returns the value...
        //        if(_fxVersion.Major == 0)
        //            _fxVersion = typeof(string).Assembly.GetName().Version;
        //        return _fxVersion;
        //    }
        //}

        ///// <summary>
        ///// Gets a SQL table object that represents the structure of this table.
        ///// </summary>
        ///// <returns></returns>
        //private static SqlTable GetSqlTable(string nativeName)
        //{
        //    if(nativeName == null)
        //        throw new ArgumentNullException("nativeName");
        //    if(nativeName.Length == 0)
        //        throw new ArgumentOutOfRangeException("'nativeName' is zero-length.");
			
        //    // create...
        //    SqlTable table = new SqlTable(nativeName);
        //    table.Columns.Add(new SqlColumn("LogId", DbType.Int64, -1, EntityFieldFlags.Key | EntityFieldFlags.Common | EntityFieldFlags.AutoIncrement));
        //    table.Columns.Add(new SqlColumn("VendorName", DbType.String, VendorNameLength, EntityFieldFlags.Common));
        //    table.Columns.Add(new SqlColumn("ProductName", DbType.String, ProductNameLength, EntityFieldFlags.Common));
        //    table.Columns.Add(new SqlColumn("ModuleName", DbType.String, ModuleNameLength, EntityFieldFlags.Common));
        //    table.Columns.Add(new SqlColumn("Version", DbType.String, VersionLength, EntityFieldFlags.Common));
        //    table.Columns.Add(new SqlColumn(LevelColumnName, DbType.Int32, -1, EntityFieldFlags.Common));
        //    table.Columns.Add(new SqlColumn("DateTime", DbType.DateTime, -1, EntityFieldFlags.Common));
        //    table.Columns.Add(new SqlColumn("EntryAssemblyName", DbType.String, EntryAssemblyNameLength, EntityFieldFlags.Common));
        //    table.Columns.Add(new SqlColumn("EntryAssemblyVersion", DbType.String, VersionLength, EntityFieldFlags.Common));
        //    table.Columns.Add(new SqlColumn("FxVersion", DbType.String, VersionLength, EntityFieldFlags.Common));
        //    table.Columns.Add(new SqlColumn("IsHttp", DbType.Boolean, -1, EntityFieldFlags.Common));
        //    table.Columns.Add(new SqlColumn("Url", DbType.String, UrlLength, EntityFieldFlags.Common | EntityFieldFlags.Nullable));
        //    table.Columns.Add(new SqlColumn("QueryString", DbType.String, QueryStringLength, EntityFieldFlags.Common | EntityFieldFlags.Nullable));
        //    table.Columns.Add(new SqlColumn("ThreadName", DbType.String, ThreadNameLength, EntityFieldFlags.Common));
        //    table.Columns.Add(new SqlColumn("MachineName", DbType.String, MachineNameLength, EntityFieldFlags.Common));
        //    table.Columns.Add(new SqlColumn("IsAuthenticated", DbType.Boolean, -1, EntityFieldFlags.Common));
        //    table.Columns.Add(new SqlColumn("WindowsUsername", DbType.String, WindowsUsernameLength, EntityFieldFlags.Common | EntityFieldFlags.Nullable));
        //    table.Columns.Add(new SqlColumn("ShortMessage", DbType.String, ShortMessageLength, EntityFieldFlags.Common | EntityFieldFlags.Nullable));
        //    table.Columns.Add(new SqlColumn("LongMessage", DbType.String, -1, EntityFieldFlags.Large | EntityFieldFlags.Nullable));
        //    table.Columns.Add(new SqlColumn("IsLongMessage", DbType.Boolean, -1, EntityFieldFlags.Common));
        //    table.Columns.Add(new SqlColumn("HasExceptionMessage", DbType.Boolean, -1, EntityFieldFlags.Common));
        //    table.Columns.Add(new SqlColumn("ExceptionMessage", DbType.String, -1, EntityFieldFlags.Large | EntityFieldFlags.Nullable));

        //    // indexes...
        //    SqlIndex index = new SqlIndex(nativeName + "DateTime");
        //    index.Columns.Add(table.Columns["DateTime"]);
        //    table.Indexes.Add(index);
			
        //    // vendorname/productname...
        //    index = new SqlIndex(nativeName + "VendorNameProductName");
        //    index.Columns.Add(table.Columns["VendorName"]);
        //    index.Columns.Add(table.Columns["ProductName"]);
        //    table.Indexes.Add(index);

        //    // return...
        //    return table;
        //}

        //private static void _tableExistenceLookup_CreateItemValue(object sender, CreateLookupItemEventArgs e)
        //{
        //    // name...
        //    string name = (string)e.Key;

        //    // stop logging...
        //    ILog nullLog = new NullLog();
        //    LogSet.BindToContext(nullLog);
        //    try
        //    {
        //        // mbr - 07-11-2005 - why not try and create the table?
        //        lock(_lockUpdateSchema)
        //        {
        //            try
        //            {
        //                SqlDialect dialect = Database.DefaultDialect;
        //                if(dialect == null)
        //                    throw new InvalidOperationException("dialect is null.");

        //                // exist?
        //                if(!(Database.DoesTableExist(name)))
        //                {
        //                    // get the table...
        //                    SqlTable table = GetSqlTable(name);
        //                    if(table == null)
        //                        throw new InvalidOperationException("table is null.");

        //                    // run...
        //                    using(IConnection connection = Database.CreateNewConnection(null))
        //                    {
        //                        foreach(string script in connection.Dialect.GetCreateTableScript(table, SqlTableScriptFlags.None))
        //                            connection.ExecuteNonQuery(new SqlStatement(script));
        //                    }
        //                }

        //                // ok...
        //                e.NewValue = true;
        //            }
        //            catch(Exception ex)
        //            {
        //                Console.WriteLine(string.Format("Failed to create the logging table.\r\n{0}", ex));
        //                e.NewValue = false;
        //            }
        //        }
        //    }
        //    finally
        //    {
        //        LogSet.UnbindFromContext(nullLog);
        //    }
        //}

        ///// <summary>
        ///// Gets the tableexists.
        ///// </summary>
        //private bool TableExists
        //{
        //    get
        //    {
        //        // returns the value...
        //        return _tableExists;
        //    }
        //}

        //private static string GetSql(SqlDialect dialect, string tableName)
        //{
        //    if(dialect == null)
        //        throw new ArgumentNullException("dialect");
        //    if(tableName == null)
        //        throw new ArgumentNullException("tableName");
        //    if(tableName.Length == 0)
        //        throw new ArgumentOutOfRangeException("'tableName' is zero-length.");

        //    // create...
        //    StringBuilder builder = new StringBuilder();
        //    builder.Append(dialect.InsertIntoKeyword);
        //    builder.Append(" ");
        //    builder.Append(dialect.FormatTableName(tableName));
        //    builder.Append(" (");
        //    builder.Append(dialect.FormatColumnName("VendorName"));
        //    builder.Append(", ");
        //    builder.Append(dialect.FormatColumnName("ProductName"));
        //    builder.Append(", ");
        //    builder.Append(dialect.FormatColumnName("ModuleName"));
        //    builder.Append(", ");
        //    builder.Append(dialect.FormatColumnName("Version"));
        //    builder.Append(", ");
        //    builder.Append(dialect.FormatColumnName(LevelColumnName));
        //    builder.Append(", ");
        //    builder.Append(dialect.FormatColumnName("DateTime"));
        //    builder.Append(", ");
        //    builder.Append(dialect.FormatColumnName("EntryAssemblyName"));
        //    builder.Append(", ");
        //    builder.Append(dialect.FormatColumnName("EntryAssemblyVersion"));
        //    builder.Append(", ");
        //    builder.Append(dialect.FormatColumnName("FxVersion"));
        //    builder.Append(", ");
        //    builder.Append(dialect.FormatColumnName("IsHttp"));
        //    builder.Append(", ");
        //    builder.Append(dialect.FormatColumnName("Url"));
        //    builder.Append(", ");
        //    builder.Append(dialect.FormatColumnName("QueryString"));
        //    builder.Append(", ");
        //    builder.Append(dialect.FormatColumnName("ThreadName"));
        //    builder.Append(", ");
        //    builder.Append(dialect.FormatColumnName("MachineName"));
        //    builder.Append(", ");
        //    builder.Append(dialect.FormatColumnName("IsAuthenticated"));
        //    builder.Append(", ");
        //    builder.Append(dialect.FormatColumnName("WindowsUsername"));
        //    builder.Append(", ");
        //    builder.Append(dialect.FormatColumnName("ShortMessage"));
        //    builder.Append(", ");
        //    builder.Append(dialect.FormatColumnName("LongMessage"));
        //    builder.Append(", ");
        //    builder.Append(dialect.FormatColumnName("IsLongMessage"));
        //    builder.Append(", ");
        //    builder.Append(dialect.FormatColumnName("HasExceptionMessage"));
        //    builder.Append(", ");
        //    builder.Append(dialect.FormatColumnName("ExceptionMessage"));

        //    // values...
        //    builder.Append(") ");
        //    builder.Append(dialect.ValuesKeyword);
        //    builder.Append(" (");
        //    builder.Append(dialect.FormatVariableNameForQueryText("VendorName"));
        //    builder.Append(", ");
        //    builder.Append(dialect.FormatVariableNameForQueryText("ProductName"));
        //    builder.Append(", ");
        //    builder.Append(dialect.FormatVariableNameForQueryText("ModuleName"));
        //    builder.Append(", ");
        //    builder.Append(dialect.FormatVariableNameForQueryText("Version"));
        //    builder.Append(", ");
        //    builder.Append(dialect.FormatVariableNameForQueryText(LevelColumnName));
        //    builder.Append(", ");
        //    builder.Append(dialect.FormatVariableNameForQueryText("DateTime"));
        //    builder.Append(", ");
        //    builder.Append(dialect.FormatVariableNameForQueryText("EntryAssemblyName"));
        //    builder.Append(", ");
        //    builder.Append(dialect.FormatVariableNameForQueryText("EntryAssemblyVersion"));
        //    builder.Append(", ");
        //    builder.Append(dialect.FormatVariableNameForQueryText("FxVersion"));
        //    builder.Append(", ");
        //    builder.Append(dialect.FormatVariableNameForQueryText("IsHttp"));
        //    builder.Append(", ");
        //    builder.Append(dialect.FormatVariableNameForQueryText("Url"));
        //    builder.Append(", ");
        //    builder.Append(dialect.FormatVariableNameForQueryText("QueryString"));
        //    builder.Append(", ");
        //    builder.Append(dialect.FormatVariableNameForQueryText("ThreadName"));
        //    builder.Append(", ");
        //    builder.Append(dialect.FormatVariableNameForQueryText("MachineName"));
        //    builder.Append(", ");
        //    builder.Append(dialect.FormatVariableNameForQueryText("IsAuthenticated"));
        //    builder.Append(", ");
        //    builder.Append(dialect.FormatVariableNameForQueryText("WindowsUsername"));
        //    builder.Append(", ");
        //    builder.Append(dialect.FormatVariableNameForQueryText("ShortMessage"));
        //    builder.Append(", ");
        //    builder.Append(dialect.FormatVariableNameForQueryText("LongMessage"));
        //    builder.Append(", ");
        //    builder.Append(dialect.FormatVariableNameForQueryText("IsLongMessage"));
        //    builder.Append(", ");
        //    builder.Append(dialect.FormatVariableNameForQueryText("HasExceptionMessage"));
        //    builder.Append(", ");
        //    builder.Append(dialect.FormatVariableNameForQueryText("ExceptionMessage"));
        //    builder.Append(")");
        //    builder.Append(dialect.StatementSeparator);

        //    // mbr - 06-02-2007 - return...
        //    builder.Append("select ");
        //    builder.Append(dialect.LastInsertedIdVariableName);
        //    builder.Append(dialect.StatementSeparator);

        //    // set...
        //    return builder.ToString();
        //}

        ///// <summary>
        ///// Gets the connectionsettings.
        ///// </summary>
        //private ConnectionSettings ConnectionSettings
        //{
        //    get
        //    {
        //        return _connectionSettings;
        //    }
        //}

        ///// <summary>
        ///// Gets log entries.
        ///// </summary>
        ///// <returns></returns>
        //public static DataTable GetEntries()
        //{
        //    return GetEntries(0, GetEntriesFlags.All);
        //}

        ///// <summary>
        ///// Gets a single log entry.
        ///// </summary>
        ///// <returns></returns>
        //// mbr - 19-05-2008 - added for phone home reporting.
        //public static DataTable GetEntry(long id)
        //{
        //    return GetEntriesInternal(0, GetEntriesFlags.All, id);
        //}

        ///// <summary>
        ///// Gets log entries.
        ///// </summary>
        ///// <returns></returns>
        //// mbr - 20-08-2007 - added overload to "select top" and audit...
        //public static DataTable GetEntries(int top, GetEntriesFlags flags)
        //{
        //    return GetEntriesInternal(top, flags, 0);
        //}

        ///// <summary>
        ///// Gets log entries.
        ///// </summary>
        ///// <returns></returns>
        //// mbr - 19-05-2008 - changed to separate method.
        //private static DataTable GetEntriesInternal(int top, GetEntriesFlags flags, long singleEntry)
        //{
        //    // mbr - 20-08-2007 - get...
        //    ArrayList levels = new ArrayList();
        //    if((int)(flags & GetEntriesFlags.AuditItems) != 0)
        //        levels.AddRange(LogSet.GetAuditLogLevels());
        //    if((int)(flags & GetEntriesFlags.NonAuditItems) != 0)
        //        levels.AddRange(LogSet.GetNonAuditLogLevels());

        //    // anything?
        //    if(levels.Count == 0)
        //        throw new InvalidOperationException("No levels were specified.");

        //    // settings...
        //    ConnectionSettings settings = Runtime.Current.InstallationSettings.GetConnectionSettingsForLogging();
        //    if(settings == null)
        //        throw new InvalidOperationException("settings is null.");

        //    // dialect...
        //    SqlDialect dialect = settings.GetDefaultDialect();
        //    if(dialect == null)
        //        throw new InvalidOperationException("dialect is null.");

        //    // builder...
        //    StringBuilder builder = new StringBuilder();
        //    builder.Append(dialect.SelectKeyword);
        //    builder.Append(" ");

        //    // mbr - 20-08-2007 - top?
        //    if(top > 0)
        //    {
        //        builder.Append("TOP");
        //        builder.Append(" ");
        //        builder.Append(top);
        //        builder.Append(" ");
        //    }

        //    // remainder...
        //    builder.Append(dialect.FormatColumnName("LogId"));
        //    builder.Append(", ");
        //    builder.Append(dialect.FormatColumnName("VendorName"));
        //    builder.Append(", ");
        //    builder.Append(dialect.FormatColumnName("ProductName"));
        //    builder.Append(", ");
        //    builder.Append(dialect.FormatColumnName("ModuleName"));
        //    builder.Append(", ");
        //    builder.Append(dialect.FormatColumnName("Version"));
        //    builder.Append(", ");
        //    builder.Append(dialect.FormatColumnName(LevelColumnName));
        //    builder.Append(", ");
        //    builder.Append(dialect.FormatColumnName("DateTime"));
        //    builder.Append(", ");
        //    builder.Append(dialect.FormatColumnName("EntryAssemblyName"));
        //    builder.Append(", ");
        //    builder.Append(dialect.FormatColumnName("EntryAssemblyVersion"));
        //    builder.Append(", ");
        //    builder.Append(dialect.FormatColumnName("FxVersion"));
        //    builder.Append(", ");
        //    builder.Append(dialect.FormatColumnName("IsHttp"));
        //    builder.Append(", ");
        //    builder.Append(dialect.FormatColumnName("Url"));
        //    builder.Append(", ");
        //    builder.Append(dialect.FormatColumnName("QueryString"));
        //    builder.Append(", ");
        //    builder.Append(dialect.FormatColumnName("ThreadName"));
        //    builder.Append(", ");
        //    builder.Append(dialect.FormatColumnName("MachineName"));
        //    builder.Append(", ");
        //    builder.Append(dialect.FormatColumnName("IsAuthenticated"));
        //    builder.Append(", ");
        //    builder.Append(dialect.FormatColumnName("WindowsUsername"));
        //    builder.Append(", ");
        //    builder.Append(dialect.FormatColumnName("ShortMessage"));
        //    builder.Append(", ");
        //    builder.Append(dialect.FormatColumnName("IsLongMessage"));
        //    builder.Append(", ");
        //    builder.Append(dialect.FormatColumnName("HasExceptionMessage"));

        //    // mbr - 19-05-2008 - get extra...
        //    if(singleEntry != 0)
        //    {
        //        builder.Append(", ");
        //        builder.Append(dialect.FormatColumnName("ExceptionMessage"));
        //        builder.Append(", ");
        //        builder.Append(dialect.FormatColumnName("LongMessage"));
        //    }

        //    // from...
        //    builder.Append(" ");
        //    builder.Append(dialect.FromKeyword);
        //    builder.Append(" ");
        //    builder.Append(dialect.FormatTableName(Runtime.Current.InstallationSettings.DatabaseLoggingTableNativeName));

        //    // where...
        //    builder.Append(" ");
        //    builder.Append(dialect.WhereKeyword);

        //    // mbr - 19-05-2008 - single entry?
        //    SqlStatement statement = new SqlStatement(dialect);
        //    if(singleEntry == 0)
        //    {
        //        builder.Append(" ");
        //        builder.Append(dialect.FormatColumnName("VendorName"));
        //        builder.Append("=");
        //        builder.Append(dialect.FormatVariableNameForQueryText("VendorName"));
        //        builder.Append(" ");
        //        builder.Append(dialect.AndKeyword);
        //        builder.Append(" ");
        //        builder.Append(dialect.FormatColumnName("ProductName"));
        //        builder.Append("=");
        //        builder.Append(dialect.FormatVariableNameForQueryText("ProductName"));

        //        // mbr - 20-08-2007 - and type...
        //        builder.Append(" ");
        //        builder.Append(dialect.AndKeyword);
        //        builder.Append(" ");
        //        builder.Append(dialect.FormatColumnName(LevelColumnName));
        //        builder.Append(" ");
        //        builder.Append(dialect.InKeyword);
        //        builder.Append(" (");
        //        AddLevels(builder, levels);
        //        builder.Append(")");

        //        // params...
        //        statement.Parameters.Add(new SqlStatementParameter("VendorName", DbType.String, Runtime.Current.Application.ProductCompany));
        //        statement.Parameters.Add(new SqlStatementParameter("ProductName", DbType.String, Runtime.Current.Application.ProductName));
        //    }
        //    else
        //    {
        //        // add...
        //        const string idParamName = "id";
        //        builder.Append(" ");
        //        builder.Append(dialect.FormatColumnName("LogId"));
        //        builder.Append("=");
        //        builder.Append(dialect.FormatVariableNameForQueryText(idParamName));

        //        // params...
        //        statement.Parameters.Add(new SqlStatementParameter(idParamName, DbType.Int64, singleEntry));
        //    }

        //    // order...
        //    builder.Append(" ");
        //    builder.Append(dialect.OrderByKeyword);
        //    builder.Append(" ");
        //    builder.Append(dialect.FormatColumnName("LogId"));
        //    builder.Append(" ");
        //    builder.Append(dialect.DescendingKeyword);

        //    // gets entries...
        //    statement.CommandText = builder.ToString();

        //    // connn...
        //    using(IConnection connection = Connection.CreateConnection(settings))
        //        return connection.ExecuteDataTable(statement);
        //}

        ///// <summary>
        ///// Gets the exception message text.
        ///// </summary>
        ///// <param name="logId"></param>
        ///// <returns></returns>
        //public static string GetLongMessageText(long logId)
        //{
        //    return GetLogText(logId, "LongMessage");
        //}

        ///// <summary>
        ///// Gets the exception message text.
        ///// </summary>
        ///// <param name="logId"></param>
        ///// <returns></returns>
        //public static string GetExceptionMessageText(long logId)
        //{
        //    return GetLogText(logId, "ExceptionMessage");
        //}

        //private static string GetLogText(long logId, string column)
        //{
        //    // settings...
        //    ConnectionSettings settings = Runtime.Current.InstallationSettings.GetConnectionSettingsForLogging();
        //    if(settings == null)
        //        throw new InvalidOperationException("settings is null.");

        //    // dialect...
        //    SqlDialect dialect = settings.GetDefaultDialect();
        //    if(dialect == null)
        //        throw new InvalidOperationException("dialect is null.");

        //    // build...
        //    StringBuilder builder = new StringBuilder();
        //    builder.Append(dialect.SelectKeyword);
        //    builder.Append(" ");
        //    builder.Append(dialect.FormatColumnName(column));
        //    builder.Append(" ");
        //    builder.Append(dialect.FromKeyword);
        //    builder.Append(" ");
        //    builder.Append(dialect.FormatTableName(Runtime.Current.InstallationSettings.DatabaseLoggingTableNativeName));
        //    builder.Append(" ");
        //    builder.Append(dialect.WhereKeyword);
        //    builder.Append(" ");
        //    builder.Append(dialect.FormatColumnName("LogId"));
        //    builder.Append("=");
        //    builder.Append(dialect.FormatVariableNameForQueryText("LogId"));

        //    // statement...
        //    SqlStatement statement = new SqlStatement();
        //    statement.CommandText = builder.ToString();
        //    statement.Parameters.Add(new SqlStatementParameter("LogId", DbType.Int32, logId));

        //    // return...
        //    using(IConnection connection = Connection.CreateConnection(settings))
        //        return ConversionHelper.ToString(connection.ExecuteScalar(statement), Cultures.User);
        //}

        ///// <summary>
        ///// Gets the next date/time that expired messages should be checked.
        ///// </summary>
        //private DateTime NextCheckExpiration
        //{
        //    get
        //    {
        //        return _nextCheckExpiration;
        //    }
        //}

        ///// <summary>
        ///// Clears the log.
        ///// </summary>
        //[Obsolete("Use the version that takes arguments.")]
        //public static void ClearEntries()
        //{
        //    throw new InvalidOperationException("The method has been deprecated.");
        //}

        ///// <summary>
        ///// Clears the log.
        ///// </summary>
        //public static void ClearEntries(bool auditItems)
        //{
        //    // settings...
        //    ConnectionSettings settings = Runtime.Current.InstallationSettings.GetConnectionSettingsForLogging();
        //    if(settings == null)
        //        throw new InvalidOperationException("settings is null.");

        //    // dialect...
        //    SqlDialect dialect = settings.GetDefaultDialect();
        //    if(dialect == null)
        //        throw new InvalidOperationException("dialect is null.");

        //    // build...
        //    StringBuilder builder = new StringBuilder();
        //    builder.Append(dialect.DeleteFromKeyword);
        //    builder.Append(" ");
        //    builder.Append(dialect.FormatTableName(Runtime.Current.InstallationSettings.DatabaseLoggingTableNativeName));

        //    // mbr - 20-08-2007 - add...			
        //    builder.Append(" ");
        //    builder.Append(dialect.WhereKeyword);
        //    builder.Append(" ");
        //    builder.Append(dialect.FormatColumnName(LevelColumnName));
        //    builder.Append(" ");
        //    builder.Append(dialect.InKeyword);
        //    builder.Append(" (");
        //    if(auditItems)
        //        AddLevels(builder, LogSet.GetAuditLogLevels());
        //    else
        //        AddLevels(builder, LogSet.GetNonAuditLogLevels());
        //    builder.Append(")");

        //    // statement...
        //    SqlStatement statement = new SqlStatement();
        //    statement.CommandText = builder.ToString();

        //    // return...
        //    using(IConnection connection = Connection.CreateConnection(settings))
        //        connection.ExecuteNonQuery(statement);
        //}		

        ///// <summary>
        ///// Writes a phone home entry to the BfxLog table.
        ///// </summary>
        ///// <param name="result"></param>
        ///// <returns></returns>
        //public static object WritePhoneHomeEntry(XmlDocument result, ConnectionSettings settings)
        //{
        //    if(result == null)
        //        throw new ArgumentNullException("result");
			
        //    // find...
        //    XmlElement applicationElement = (XmlElement)result.SelectSingleNode("ErrorReport/Application");
        //    if(applicationElement == null)
        //        throw new InvalidOperationException("applicationElement is null.");

        //    // find...
        //    XmlElement environmentElement = (XmlElement)result.SelectSingleNode("ErrorReport/Environment");
        //    if(environmentElement == null)
        //        throw new InvalidOperationException("environmentElement is null.");

        //    // find...
        //    XmlElement httpElement = (XmlElement)result.SelectSingleNode("ErrorReport/Http");
        //    if(httpElement == null)
        //        throw new InvalidOperationException("httpElement is null.");

        //    // are we?
        //    bool isHttp = XmlHelper.GetAttributeBoolean(httpElement, "isHttp", OnNotFound.ReturnNull);
        //    string httpUrl = null;
        //    string httpQueryString = null;
        //    if(isHttp)
        //    {
        //        // get...
        //        httpUrl = XmlHelper.GetElementString(httpElement, "Url", OnNotFound.ThrowException);
        //        httpQueryString = XmlHelper.GetElementString(httpElement, "QueryString", OnNotFound.ThrowException);
        //    }

        //    // find...
        //    XmlElement errorElement = (XmlElement)result.SelectSingleNode("ErrorReport/Error");
        //    if(errorElement == null)
        //        throw new InvalidOperationException("errorElement is null.");

        //    //public static object WriteEntryRaw(string vendorName, string productName, string moduleName, string appVersion, 
        //    //string fxVersion, string entryAssemblyName, string entryAssemblyVersion, LogLevel level, DateTime dateTime, 
        //    //bool isHttp, string httpUrl, string httpQueryString, string threadName, string machineName, bool isAuthenticated, 
        //    //string windowsUsername, string message, string exceptionText)

        //    // settings...
        //    if(settings == null)
        //        settings = Runtime.Current.InstallationSettings.GetConnectionSettingsForLogging();

        //    // ok...
        //    return WriteEntryRaw(settings, DefaultTableName, 
        //        XmlHelper.GetElementString(applicationElement, "VendorName", OnNotFound.ThrowException), 
        //        XmlHelper.GetElementString(applicationElement, "ProductName", OnNotFound.ThrowException),
        //        XmlHelper.GetElementString(applicationElement, "ModuleName", OnNotFound.ThrowException),
        //        XmlHelper.GetElementString(applicationElement, "Version", OnNotFound.ThrowException),
        //        XmlHelper.GetElementString(applicationElement, "FxVersion", OnNotFound.ThrowException),
        //        XmlHelper.GetElementString(environmentElement, "EntryAssemblyName", OnNotFound.ThrowException),
        //        XmlHelper.GetElementString(environmentElement, "EntryAssemblyVersion", OnNotFound.ThrowException),
        //        LogLevel.PhoneHome, DateTime.UtcNow,
        //        isHttp, httpUrl, httpQueryString, 
        //        XmlHelper.GetElementString(environmentElement, "ThreadName", OnNotFound.ThrowException),
        //        XmlHelper.GetElementString(environmentElement, "MachineName", OnNotFound.ThrowException),
        //        XmlHelper.GetElementBoolean(environmentElement, "IsAuthenticated", OnNotFound.ThrowException),
        //        XmlHelper.GetElementString(environmentElement, "WindowsUsername", OnNotFound.ThrowException), 
        //        result.OuterXml, null);
        //}

        ///// <summary>
        ///// Gets the entry as an XML document.
        ///// </summary>
        ///// <param name="logId"></param>
        ///// <returns></returns>
        //public XmlDocument ToXmlDocument(long logId)
        //{
        //    // TODO: This method has not been implemented.  Check whether it is a valid not implementated, or a code path that was not completed.
        //    throw new NotImplementedException(string.Format("Not implemented for '{0}'.", this.GetType()));
        //}
	}
}
