// BootFX - Application framework for .NET applications
// 
// File: Connection.cs
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
using System.Text.RegularExpressions;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;
using BootFX.Common.Data.Schema;
using BootFX.Common.Data.Formatters;
using BootFX.Common.Crypto;
using BootFX.Common.Entities;
using BootFX.Common.Management;
using System.Collections.Generic;
using System.Security;

namespace BootFX.Common.Data
{
    /// <summary>
    /// Base connection class.
    /// </summary>
    public abstract class Connection : LoggableMarshalByRefObject, IConnection
    {
        /// <summary>
        /// Private field to support <see cref="TransactionId"/> property.
        /// </summary>
        private Guid _transactionId;

        ///// <summary>
        ///// Private field to support <c>NumCommandsCreated</c> property.
        ///// </summary>
        //private int _numCommandsCreated;

        /// <summary>
        /// Private field to support <c>BindUsage</c> property.
        /// </summary>
        private int _bindUsage = 0;

        // mbr - 2010-02-18 - deprecated...
        // mbr - 2014-11-30 - brought back...
        private Dictionary<string, IDbCommand> CommandCache { get; set; }

        /// <summary>
        /// Private field to support <c>SqlMode</c> property.
        /// </summary>
        private SqlMode _sqlMode;

        /// <summary>
        /// Private field to support <see cref="BoundCacheSlot"/> property.
        /// </summary>
        //private static LocalDataStoreSlot _boundCacheSlot = Thread.GetNamedDataSlot("__bfxBoundCache");

        /// <summary>
        /// Private field to support <c>TransactionRolledBack</c> property.
        /// </summary>
        private bool _transactionRolledBack = false;

        /// <summary>
        /// Private field to support <c>CurrentTransaction</c> property.
        /// </summary>
        private IDbTransaction _currentTransaction = null;

        /// <summary>
        /// Private field to support <c>TransactionDepth</c> property.
        /// </summary>
        private int _transactionDepth = 0;

        // mbr - 07-02-2008 - deprecated (never used)...
        //		/// <summary>
        //		/// Private field to support <c>ConnectionThread</c> property.
        //		/// </summary>
        //		private int _connectionThread = 0;

        /// <summary>
        /// Private field to support <c>NativeConnection</c> property.
        /// </summary>
        private IDbConnection _nativeConnection;

        /// <summary>
        /// Private field to support <c>ConnectionString</c> property.
        /// </summary>
        private string _connectionString;

        /// <summary>
        /// Stores whether the object has been disposed.
        /// </summary>
        private bool _disposed = false;

        /// <summary>
        /// Raised when the native connection is opened.
        /// </summary>
        public event EventHandler NativeConnectionOpened;

        public static IConnectionProfiler Profiler { get; set; }

        public static event EventHandler<CommandCreatedEventArgs> CommandCreated;

        public static IExistenceChecker ExistenceChecker { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="connectionString"></param>
        protected Connection(string connectionString)
        {
            if (connectionString == null)
                throw new ArgumentNullException("connectionString");
            if (connectionString.Length == 0)
                throw ExceptionHelper.CreateZeroLengthArgumentException("connectionString");

            // set...
            _connectionString = connectionString;
            this.CommandCache = new Dictionary<string, IDbCommand>();
        }

        /// <summary>
        /// Finalizer.
        /// </summary>
        ~Connection()
        {
            try
            {
                this.Dispose();
            }
            catch
            {
            }
        }

        /// <summary>
        /// Gets the connectionstring.
        /// </summary>
        public string ConnectionString
        {
            get
            {
                return _connectionString;
            }
        }

        /// <summary>
        /// Gets the native ADO.NET connection. 
        /// </summary>
        public IDbConnection NativeConnection
        {
            get
            {
                EnsureNativeConnectionCreated();
                return _nativeConnection;
            }
        }

        /// <summary>
        /// Returns  NativeConnection.
        /// </summary>
        private bool IsNativeConnectionCreated()
        {
            if (_nativeConnection == null)
                return false;
            else
                return true;
        }

        /// <summary>
        /// Ensures that NativeConnection has been created.
        /// </summary>
        internal void EnsureNativeConnectionCreated()
        {
            if (IsNativeConnectionCreated() == false)
                _nativeConnection = CreateNativeConnection();
        }

        /// <summary>
        /// Creates a strongly-typed data adapter.
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        protected abstract IDataAdapter CreateDataAdapter(IDbCommand command);

        /// <summary>
        /// Creates an instance of NativeConnection.
        /// </summary>
        /// <remarks>This does not assign the instance to the _nativeConnection field</remarks>
        protected abstract IDbConnection CreateNativeConnection();

        /// <summary>
        /// Executes the given statement.
        /// </summary>
        /// <param name="statement"></param>
        /// <returns></returns>
        public virtual DataTable ExecuteDataTable(ISqlStatementSource statement)
        {
            if (statement == null)
                throw new ArgumentNullException("statement");

            // get the dataset...
            DataSet dataSet = this.ExecuteDataSet(statement);
            if (dataSet.Tables.Count == 0)
                return null;
            if (dataSet.Tables.Count == 1)
                return dataSet.Tables[0];
            else
                throw new InvalidOperationException(string.Format(Cultures.Log, "Statement returned multiple ({0}) tables.", dataSet.Tables.Count));
        }

        /// <summary>
        /// Executes the given statement.
        /// </summary>
        /// <param name="statement"></param>
        public DataSet ExecuteDataSet(ISqlStatementSource statement)
        {
            if (statement == null)
                throw new ArgumentNullException("statement");

            // mbr - 2014-11-30 - caching means we no longer dispose here...
            //using (IDbCommand command = CreateCommand(statement))
            SqlStatement realStatement = null;
            var command = CreateCommand(statement, ref realStatement);
            {
                // ensure...
                EnsureNativeConnectionOpen();

                // run...
                return this.CaptureProfile<DataSet>(realStatement, command, SqlMethod.DataSet, (profiling) =>
                {
                    IDataAdapter adapter = this.CreateDataAdapter(command);
                    try
                    {
                        // create...
                        DataSet dataSet = new DataSet();
                        dataSet.Locale = CultureInfo.InvariantCulture;
                        adapter.Fill(dataSet);

                        // return...
                        return dataSet;
                    }
                    catch (Exception ex)
                    {
                        throw CreateCommandException("Failed to ExecuteDataSet.", command, ex, realStatement);
                    }
                    finally
                    {
                        if (adapter as IDisposable != null)
                            (adapter as IDisposable).Dispose();
                    }
                });
            }
        }

        /// <summary>
        /// Executes the given statement, returning an array of the first column values for all rows.
        /// </summary>
        /// <param name="statement"></param>
        public long[] ExecuteValuesVerticalInt64(ISqlStatementSource statement)
        {
            //return (long[])this.ExecuteValuesVertical(statement, typeof(long));
            return ExecuteValuesVertical<long>(statement).ToArray();
        }

        /// <summary>
        /// Executes the given statement, returning an array of the first column values for all rows.
        /// </summary>
        /// <param name="statement"></param>
        public int[] ExecuteValuesVerticalInt32(ISqlStatementSource statement)
        {
            //return (int[])this.ExecuteValuesVertical(statement, typeof(int));
            return ExecuteValuesVertical<int>(statement).ToArray();
        }

        /// <summary>
        /// Executes the given statement, returning an array of the first column values for all rows.
        /// </summary>
        /// <param name="statement"></param>
        public short[] ExecuteValuesVerticalInt16(ISqlStatementSource statement)
        {
            //return (short[])this.ExecuteValuesVertical(statement, typeof(short));
            return ExecuteValuesVertical<short>(statement).ToArray();
        }

        /// <summary>
        /// Executes the given statement, returning an array of the first column values for all rows.
        /// </summary>
        /// <param name="statement"></param>
        public string[] ExecuteValuesVerticalString(ISqlStatementSource statement)
        {
            //return (string[])this.ExecuteValuesVertical(statement, typeof(string));
            return ExecuteValuesVertical<string>(statement).ToArray();
        }

        /// <summary>
        /// Executes the given statement, returning an array of the first column values for all rows.
        /// </summary>
        /// <param name="statement"></param>
        public decimal[] ExecuteValuesVerticalDecimal(ISqlStatementSource statement)
        {
            //return (decimal[])this.ExecuteValuesVertical(statement, typeof(Decimal));
            return ExecuteValuesVertical<decimal>(statement).ToArray();
        }

        /// <summary>
        /// Executes the given statement, returning an array of the first column values for all rows.
        /// </summary>
        /// <param name="statement"></param>
        public bool[] ExecuteValuesVerticalBoolean(ISqlStatementSource statement)
        {
            //return (bool[])this.ExecuteValuesVertical(statement, typeof(bool));
            return ExecuteValuesVertical<bool>(statement).ToArray();
        }

        /// <summary>
        /// Executes the given statement, returning an array of the first column values for all rows.
        /// </summary>
        /// <param name="statement"></param>
        public float[] ExecuteValuesVerticalFloat(ISqlStatementSource statement)
        {
            //return (float[])this.ExecuteValuesVertical(statement, typeof(float));
            return ExecuteValuesVertical<float>(statement).ToArray();
        }

        /// <summary>
        /// Executes the given statement, returning an array of the first column values for all rows.
        /// </summary>
        /// <param name="statement"></param>
        public double[] ExecuteValuesVerticalDouble(ISqlStatementSource statement)
        {
            //return (double[])this.ExecuteValuesVertical(statement, typeof(double));
            return ExecuteValuesVertical<double>(statement).ToArray();
        }

        /// <summary>
        /// Executes the given statement, returning an array of the first column values for all rows.
        /// </summary>
        /// <param name="statement"></param>
        public IEnumerable<T> ExecuteValuesVertical<T>(ISqlStatementSource statement)
        {
            // get...
            object[] raw = this.ExecuteValuesVertical(statement);
            if (raw == null)
                throw new InvalidOperationException("raw is null.");

            // create...
            //Array results = Array.CreateInstance(type, raw.Length);
            var results = new List<T>();
            for (int index = 0; index < raw.Length; index++)
            {
                //results.SetValue(ConversionHelper.ChangeType(raw[index], typeof(T), Cultures.System), index);
                results.Add((T)ConversionHelper.ChangeType(raw[index], typeof(T), Cultures.System));
            }

            // return...
            return results;
        }

        /// <summary>
        /// Executes the given statement, returning an array of the first column values for all rows.
        /// </summary>
        /// <param name="statement"></param>
        public object[] ExecuteValuesVertical(ISqlStatementSource statement)
        {
            if (statement == null)
                throw new ArgumentNullException("statement");

            // mbr - 2014-11-30 - caching means we no longer dispose here...
            // create a command...
            SqlStatement realStatement = null;
            var command = CreateCommand(statement, ref realStatement);

            // open...
            this.EnsureNativeConnectionOpen();

            // run...
            IDataReader reader = null;
            return this.CaptureProfile<object[]>(realStatement, command, SqlMethod.ValuesVertical, (profiling) =>
            {
                var results = new ArrayList();
                try
                {
                    reader = command.ExecuteReader();

                    // read...
                    while (reader.Read())
                        results.Add(reader[0]);

                    // return...
                    return results.ToArray();
                }
                catch (Exception ex)
                {
                    throw CreateCommandException("Failed to ExecuteValuesVertical.", command, ex, realStatement);
                }
                finally
                {
                    if (reader != null)
                        reader.Dispose();
                }
            });
        }

        /// <summary>
        /// Executes the given statement.
        /// </summary>
        /// <param name="statement"></param>
        public object[] ExecuteValues(ISqlStatementSource statement)
        {
            if (statement == null)
                throw new ArgumentNullException("statement");

            // create a command...
            // mbr - 2014-11-30 - caching means we no longer dispose here...
            SqlStatement realStatement = null;
            var command = CreateCommand(statement, ref realStatement);
            {
                // open...
                this.EnsureNativeConnectionOpen();

                // run...
                IDataReader reader = null;
                try
                {
                    // run...
                    reader = command.ExecuteReader();

                    // read...
                    if (reader.Read())
                    {
                        // get...
                        object[] results = new object[reader.FieldCount];
                        reader.GetValues(results);

                        // return...
                        return results;
                    }
                    else
                        return null;
                }
                catch (Exception ex)
                {
                    throw CreateCommandException("Failed to ExecuteValues.", command, ex, realStatement);
                }
                finally
                {
                    if (reader != null)
                        reader.Dispose();
                }
            }
        }

        public DateTime ExecuteScalarDateTime(ISqlStatementSource statement)
        {
            return (DateTime)ExecuteScalar(statement, typeof(DateTime));
        }

        public string ExecuteScalarString(ISqlStatementSource statement)
        {
            return (string)ExecuteScalar(statement, typeof(string));
        }

        public int ExecuteScalarInt32(ISqlStatementSource statement)
        {
            return (int)ExecuteScalar(statement, typeof(int));
        }

        public short ExecuteScalarInt16(ISqlStatementSource statement)
        {
            return (short)ExecuteScalar(statement, typeof(short));
        }

        public long ExecuteScalarInt64(ISqlStatementSource statement)
        {
            return (long)ExecuteScalar(statement, typeof(long));
        }

        public Guid ExecuteScalarGuid(ISqlStatementSource statement)
        {
            object asObject = ExecuteScalar(statement, typeof(Guid));
            if (asObject is DBNull || asObject == null)
                return Guid.Empty;
            else if (asObject is Guid)
                return (Guid)asObject;
            else
                throw new NotSupportedException(string.Format("Cannot handle '{0}'.", asObject));
        }

        public float ExecuteScalarSingle(ISqlStatementSource statement)
        {
            return (float)ExecuteScalar(statement, typeof(float));
        }

        public double ExecuteScalarDouble(ISqlStatementSource statement)
        {
            return (double)ExecuteScalar(statement, typeof(double));
        }

        public bool ExecuteScalarBoolean(ISqlStatementSource statement)
        {
            return (bool)ExecuteScalar(statement, typeof(bool));
        }

        public byte ExecuteScalarByte(ISqlStatementSource statement)
        {
            return (byte)ExecuteScalar(statement, typeof(byte));
        }

        public decimal ExecuteScalarDecimal(ISqlStatementSource statement)
        {
            return (decimal)ExecuteScalar(statement, typeof(decimal));
        }

        /// <summary>
        /// Executes the given statement and returns the first column value in the first row, or null if no row was returned, converting to the given type and smoothing out DB null values and CLR null values.
        /// </summary>
        /// <param name="statement"></param>
        public object ExecuteScalar(ISqlStatementSource statement, Type type)
        {
            if (statement == null)
                throw new ArgumentNullException("statement");

            // mbr - 2014-11-30 - caching means we no longer dispose here...
            // create a command...
            SqlStatement realStatement = null;
            var command = CreateCommand(statement, ref realStatement);
            {
                // ensure...
                EnsureNativeConnectionOpen();

                // mbr - 2016-06-05 - added deadlock retry...
                int tries = 0;
                object value = null;
                while (true)
                {
                    var retryDeadlock = false;
                    try
                    {
                        value = this.CaptureProfile<object>(realStatement, command, SqlMethod.Scalar, (profiling) =>
                        {
                            return command.ExecuteScalar();
                        });

                        if (type != null)
                            value = ConversionHelper.ChangeType(value, type, Cultures.System, ConversionFlags.DBNullToLegal | ConversionFlags.ClrNullToLegal);
                    }
                    catch(SqlException ex)
                    {
                        var abort = true;
                        if (Database.RetryDeadlocks)
                        {
                            if (ex.Number == SqlServerErrorCodes.TransactionDeadlock && tries < 3)
                            {
                                retryDeadlock = true;
                                abort = false;
                                Thread.Sleep(Database.RetryDeadlockWaitPeriod);
                            }
                        }

                        if(abort)
                            throw CreateCommandException("Failed to ExecuteScalar.", command, ex, realStatement);
                    }
                    catch (Exception ex)
                    {
                        throw CreateCommandException("Failed to ExecuteScalar.", command, ex, realStatement);
                    }

                    // if...
                    if (!(retryDeadlock))
                        break;

                    // next...
                    tries++;
                }

                // return...
                return value;
            }
        }

        /// <summary>
        /// Executes the given statement.
        /// </summary>
        /// <param name="statement"></param>
        public object ExecuteScalar(ISqlStatementSource statement)
        {
            return ExecuteScalar(statement, null);
        }

        /// <summary>
        /// Executes the given statement.
        /// </summary>
        /// <param name="statement"></param>
        public int ExecuteNonQuery(ISqlStatementSource statement)
        {
            if (statement == null)
                throw new ArgumentNullException("statement");

            // create a command...
            SqlStatement statementToRun = statement.GetStatement();
            var command = CreateCommand(statementToRun);
            {
                // ensure...
                EnsureNativeConnectionOpen();

                // run...
                var tries = 0;
                int result = 0;
                while(true)
                {
                    var retryDeadlock = false;  
                    try
                    {
                        // run...
                        result = this.CaptureProfile<int>(statementToRun, command, SqlMethod.NonQuery, (profiling) =>
                        {
                            return command.ExecuteNonQuery();
                        });

                        // reconcile..
                        this.ReconcileParameterValues(command, statementToRun);
                    }
                    catch(SqlException ex)
                    {
                        var abort = true;
                        if (Database.RetryDeadlocks)
                        {
                            if (ex.Number == SqlServerErrorCodes.TransactionDeadlock && tries < 3)
                            {
                                retryDeadlock = true;
                                abort = false;
                                Thread.Sleep(Database.RetryDeadlockWaitPeriod);
                            }
                        }

                        if(abort)
                            throw CreateCommandException("Failed to ExecuteScalar.", command, ex, statementToRun);
                    }
                    catch (Exception ex)
                    {
                        throw CreateCommandException("Failed to ExecuteNonQuery.", command, ex, statementToRun);
                    }

                    // next...
                    tries++;

                    // stop...
                    if (!(retryDeadlock))
                        break;
                }

                // return...
                return result;
            }
        }

        protected T CaptureProfile<T>(SqlStatement statement, IDbCommand command, SqlMethod method, Func<IConnectionProfilingData, T> callback)
        {
            IConnectionProfilingData data = null;
            if (Profiler != null)
                data = new ActiveProfileWrapper(statement, command, method);
            else
                data = new NullProfileWrapper();

            T result = default(T);
            try
            {
                result = callback(data);
            }
            catch (Exception ex)
            {
                data.Exception = ex;

                // throw it...
                throw new InvalidOperationException("Failed to make database call.", ex);
            }
            finally
            {
                if (Profiler != null)
                    data.Dispose();
            }

            // return...
            return result;
        }

        private class NullProfileWrapper : IConnectionProfilingData
        {
            public IDbCommand Command { get; set; }
            public Exception Exception { get; set; }

            public void Dispose()
            {
            }
        }

        private class ActiveProfileWrapper : Loggable, IConnectionProfilingData
        {
            private AccurateTimer Timer { get; set; }
            private SqlStatement Statement { get; set; }
            public IDbCommand Command { get; set; }
            private SqlMethod Method { get; set; }
            public Exception Exception { get; set; }

            internal ActiveProfileWrapper(SqlStatement statement, IDbCommand command, SqlMethod method)
            {
                this.Statement = statement;
                this.Command = command;
                this.Method = method;

                this.Timer = new AccurateTimer();
                this.Timer.Start();
            }

            public void Dispose()
            {
                this.Timer.Stop();

                // ok...
                try
                {
                    Profiler.Profile(this.Statement, this.Command, this.Timer.DurationAsDecimal, this.Method, this.Exception);
                }
                catch (Exception ex)
                {
                    this.LogWarn(() => "An error occurred when handling profiling results.", ex);
                }
            }
        }

        /// <summary>
        /// Reconciles parameter values back to the original statement.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="statement"></param>
        private void ReconcileParameterValues(IDbCommand command, SqlStatement statement)
        {
            if (command == null)
                throw new ArgumentNullException("command");
            if (statement == null)
                throw new ArgumentNullException("statement");

            // walk and set...
            for (int index = 0; index < command.Parameters.Count; index++)
            {
                // get...
                IDbDataParameter parameter = (IDbDataParameter)command.Parameters[index];
                if (parameter.Direction != ParameterDirection.Input)
                    statement.Parameters[index].Value = parameter.Value;
            }
        }

        public bool DoesTableExist<T>()
            where T : Entity
        {
            var et = EntityType.GetEntityType<T>();
            return DoesTableExist(et.NativeName);
        }

        /// <summary>
        /// Returns a boolean determining if the table you requested exists in the database
        /// </summary>
        /// <param name="nativeName"></param>
        /// <returns></returns>
        public bool DoesTableExist(string nativeName)
        {
            if (nativeName == null)
                throw new ArgumentNullException("nativeName");
            if (nativeName.Length == 0)
                throw new ArgumentOutOfRangeException("'nativeName' is zero-length.");

            // defer...
            return this.DoesTableExist(new NativeName(nativeName));
        }

        /// <summary>
        /// Returns a boolean determining if the table you requested exists in the database
        /// </summary>
        /// <param name="nativeName"></param>
        /// <returns></returns>
        public bool DoesTableExist(NativeName nativeName)
        {
            if (ExistenceChecker != null)
                return ExistenceChecker.DoesTableExist(this, nativeName);
            else
                return DoDoesTableExist(nativeName);
        }

        protected virtual bool DoDoesTableExist(NativeName nativeName)
        {
            // create...
            SqlStatement sql = new SqlStatement(this.Dialect);

            // set...
            const string tableNameName = "tableName";
            sql.Parameters.Add(tableNameName, DbType.String, nativeName.Name);
            sql.CommandText = string.Format("SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME={0} AND TABLE_TYPE='BASE TABLE'",
                this.Dialect.FormatVariableNameForQueryText(tableNameName));

            // run...
            string result = this.ExecuteScalarString(sql);
            if (result == null || result.Length == 0)
                return false;
            else
                return true;
        }

        public bool DoesColumnExist(string tableName, string columnName)
        {
            if (ExistenceChecker != null)
                return ExistenceChecker.DoesColumnExist(this, tableName, columnName);
            else
            {
                return this.ExecuteScalarInt32(new SqlStatement("select column_id from sys.columns where name=@p0 and object_id = object_id(@p1)",
                   new object[] { columnName, tableName })) != 0;
            }
        }

        public bool DoesIndexExist(string tableName, string indexName)
        {
            if (ExistenceChecker != null)
                return ExistenceChecker.DoesIndexExist(this, tableName, indexName);
            else
            {
                return this.ExecuteScalarInt32(new SqlStatement("select index_id from sys.indexes where name=@p0 and object_id = object_id(@p1)",
                   new object[] { indexName, tableName })) != 0;
            }
        }

        void IConnection.EnsureOpen()
        {
            this.EnsureNativeConnectionOpen();
        }

        /// <summary>
        /// Ensures the connection is open.
        /// </summary>
        public void EnsureNativeConnectionOpen()
        {
            // check...
            this.AssertNotDisposed();

            if (NativeConnection == null)
                throw new ArgumentNullException("NativeConnection");

            // open...
            if (this.NativeConnection.State != ConnectionState.Open)
            {
                // mbr - 27-07-2006 - added exception...
                try
                {
                    this.NativeConnection.Open();
                }
                catch (Exception ex)
                {
                    throw this.CreateConnectionException(this.NativeConnection, ex);
                }

                // mbr - 2008-09-22 - added event...
                this.OnNativeConnectionOpened(EventArgs.Empty);
            }
        }

        protected virtual void OnNativeConnectionOpened(EventArgs e)
        {
            if (this.NativeConnectionOpened != null)
                this.NativeConnectionOpened(this, e);
        }

        public Exception CreateConnectionException(IDbConnection conn, Exception innerException)
        {
            if (conn == null)
                throw new ArgumentNullException("conn");

            // throw...
            throw new DatabaseException(string.Format("Failed to connect to database of type '{0}'.\r\nConnection string: {1}", conn.GetType(),
                GetSafeConnectionStringForDebug(conn)), innerException);
        }

        public static string GetSafeConnectionStringForDebug(IDbConnection conn)
        {
            // regex to remove the password from the connection string...
            // ;?\s*pwd\s*=\s*(?<password>[^;]*)
            Regex regex = new Regex(@";?\s*pwd\s*=\s*(?<password>[^;]*)", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            string connectionString = string.Empty;
            if (conn != null)
                connectionString = conn.ConnectionString;
            if (connectionString != null && connectionString.Length > 0)
                connectionString = regex.Replace(connectionString, ";pwd=******");
            else
                connectionString = "(No connection string)";

            // return...
            return connectionString;
        }

        /// <summary>
        /// Returns true if the object is disposed.
        /// </summary>
        /// <returns></returns>
        private bool IsDisposed()
        {
            return _disposed;
        }

        /// <summary>
        /// Throws an exception if the connection has been disposed.
        /// </summary>
        private void AssertNotDisposed()
        {
            if (this.IsDisposed() == true)
                throw new ObjectDisposedException(this.GetType().ToString());
        }

        /// <summary>
        /// Disposes the connection.
        /// </summary>
        public void Dispose()
        {
            // ensure...
            if (this.IsDisposed() == true)
                return;

            // mbr - 28-10-2005 - are we bound?  again, ignore if we are...
            // are we in a transaction? ignore this if we are...
            //			if(this.IsInTransaction)
            if (this.IsBound)
                return;

            // mbr - 28-10-2005 - dispose the command cache...
            // mbr - 2014-11-30 - redone --> dispose the command cache...
            if (this.CommandCache != null)
            {
                try
                {
                    foreach (var command in this.CommandCache.Values)
                    {
                        try
                        {
                            command.Dispose();
                        }
                        catch (Exception ex)
                        {
                            if (this.Log.IsWarnEnabled)
                                this.Log.Warn(string.Format("Failed to dispose command '{0}' ({1}).", command.CommandText, command.CommandType), ex);
                        }
                    }
                }
                finally
                {
                    CommandCache = null;
                }
            }

            // check...
            if (this.IsNativeConnectionCreated() == true)
            {
                if (this.NativeConnection.State != ConnectionState.Closed)
                {
                    // changes from Close to Dispose to satisfy FxCop rule...
                    // "Consider calling the IDisposable.Dispose methods of data members"
                    _nativeConnection.Dispose();
                }

                // clear...
                _nativeConnection = null;
            }

            // set...
            _disposed = true;
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Creates a new command.
        /// </summary>
        /// <returns></returns>
        public virtual IDbCommand CreateCommand()
        {
            // check...
            this.AssertNotDisposed();

            // mbr - 07-02-2008 - deprecated.			
            //			this.AssertThread();

            // return...
            if (NativeConnection == null)
                throw new ArgumentNullException("NativeConnection");
            IDbCommand command = this.NativeConnection.CreateCommand();

            // transaction?
            command.Transaction = this.CurrentTransaction;

            // return...
            return command;
        }

        /// <summary>
        /// Creates a new command.
        /// </summary>
        /// <param name="statement"></param>
        /// <returns></returns>
        public IDbCommand CreateCommand(ISqlStatementSource statement)
        {
            SqlStatement realStatement = null;
            return CreateCommand(statement, ref realStatement);
        }

        protected IDbCommand CreateCommand(ISqlStatementSource statement, ref SqlStatement realStatement)
        {
            if (statement == null)
                throw new ArgumentNullException("statement");

            // defer...
            realStatement = statement.GetStatement();
            return this.CreateCommand(realStatement);
        }

        private string GetCommandCacheKey(SqlStatement statement)
        {
            if (statement == null)
                throw new ArgumentNullException("statement");
            //return (int)statement.CommandType + "|" + statement.CommandText;
            return statement.GetHash(false).CommandHash;
        }

        ///// <summary>
        ///// Gets the numcommandscreated.
        ///// </summary>
        //private int NumCommandsCreated
        //{
        //    get
        //    {
        //        // returns the value...
        //        return _numCommandsCreated;
        //    }
        //}

        /// <summary>
        /// Creates a new command.
        /// </summary>
        /// <param name="statement"></param>
        /// <returns></returns>
        protected internal IDbCommand CreateCommand(SqlStatement statement)
        {
            if (statement == null)
                throw new ArgumentNullException("statement");

            // check...
            if (statement.Dialect == null)
                throw new ArgumentNullException("statement.Dialect");

            // mbr - 28-10-2005 - initialize the command cache...
            //_numCommandsCreated++;
            //if (this.NumCommandsCreated == 3)
            //    _commandCache = new HybridDictionary();

            // rewrite it?
            var rewriter = SqlRewriter.GetRewriter(statement.Dialect);
            if (rewriter != null)
                statement = rewriter.Rewrite(statement);

            // mbr - 28-10-2005 - added concept of command caching...
            // mbr - 2010-02-18 - removed command caching...
            IDbCommand command = null;
            //if (this.CommandCache != null)
            //{

                // get it from the command cache...
                string key = this.GetCommandCacheKey(statement);
                if (Database.CommandCachingEnabled && this.CommandCache.ContainsKey(key))
                {
                    command = (IDbCommand)this.CommandCache[key];

                    // check that we have not been disposed...
                    if (command.Connection == null)
                        throw new ObjectDisposedException(string.Format("Cached statement against hash '{0}' has been disposed.", key));

                    // patch in the parameter values...
                    foreach (SqlStatementParameter parameter in statement.Parameters)
                    {
                        string name = this.Dialect.FormatVariableNameForQueryText(parameter.Name);
                        IDataParameter dataParam = (IDataParameter)command.Parameters[name];
                        if (dataParam == null)
                            throw new InvalidOperationException(string.Format("A parameter with name '{0}' was not found.", name));

                        // set...
                        dataParam.Value = this.SanitizeParameterValue(parameter);
                    }

                    // mbr - 27-03-2007 - reset the transaction...
                    command.Transaction = this.CurrentTransaction;

                    // return it...
                    return command;
                }
            //}

            // base...
            if (command == null)
            {
                command = this.CreateCommand();
                if (command == null)
                    throw new ArgumentNullException("command");
                try
                {
                    // basic stuff...
                    command.CommandText = statement.CommandText;
                    command.CommandType = statement.CommandType;

                    // mbr - 2010-03-26 - timeout...
                    if (statement.HasTimeout)
                        command.CommandTimeout = statement.TimeoutSeconds;

                    // do the params...
                    var hasStructured = false;
                    foreach (SqlStatementParameter parameter in statement.Parameters)
                    {
                        // add...
                        try
                        {
                            command.Parameters.Add(CreateParameter(command, parameter, statement.Dialect));
                        }
                        catch (Exception ex)
                        {
                            throw new InvalidOperationException(string.Format("Failed to create parameter '{0}'.", parameter.Name), ex);
                        }

                        if (!(hasStructured) && parameter.IsStructured)
                            hasStructured = true;
                    }

                    //// mbr - 28-10-2005 - add to the cache - but only cache 48 commands...
                    //if (this.CommandCache != null && this.CommandCache.Count < 48)
                    //    this.CommandCache[this.GetCommandCacheKey(statement)] = command;
                    if(!(hasStructured))
                        this.CommandCache[key] = command;
                }
                catch (Exception ex)
                {
                    // cleanup...
                    if (command != null)
                        command.Dispose();

                    // throw...
                    throw new InvalidOperationException("Failed to create command from statement.", ex);
                }
            }

            // mbr - 2015-09-09 - send...
            if (CommandCreated != null)
            {
                var e = new CommandCreatedEventArgs(this, statement, command);
                CommandCreated(this, e);

                // flip?
                if (e.CommandChanged)
                    command = e.Command;    
            }

            // return...
            if (command == null)
                throw new InvalidOperationException("command is null.");
            return command;
        }

        private object SanitizeParameterValue(SqlStatementParameter parameter)
        {
            // mbr - 2014-11-26 - simplified...
            //if (parameter == null)
            //    throw new ArgumentNullException("parameter");

            //object value = parameter.Value;
            //if (value is EncryptedValue)
            //{
            //    // get...
            //    EncryptedValue encryptedValue = (EncryptedValue)value;

            //    // what's the field?
            //    switch (parameter.DBType)
            //    {
            //        case DbType.String:
            //        case DbType.StringFixedLength:
            //        case DbType.AnsiString:
            //        case DbType.AnsiStringFixedLength:
            //            value = encryptedValue.ToBase64String();
            //            break;
            //        default:
            //            throw new NotSupportedException(string.Format("Cannot handle '{0}' ({1}).", parameter.DBType, parameter.DBType.GetType()));
            //    }
            //}
            //else if (value is Uri)		// added this because this causes explosion weirdness...
            //    value = value.ToString();

            // return...
            return parameter.Value;
        }



        /// <summary>
        /// Creates a parameter.
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        protected virtual IDataParameter CreateParameter(IDbCommand command, SqlStatementParameter parameter, SqlDialect dialect)
        {
            if (command == null)
                throw new ArgumentNullException("command");
            if (parameter == null)
                throw new ArgumentNullException("parameter");
            if (dialect == null)
                throw new ArgumentNullException("dialect");

            // create...
            var newParameter = command.CreateParameter();

            // mbr - 28-10-2005 - defer this to another method...			
            object useValue = this.SanitizeParameterValue(parameter);

            // mbr - 24-07-2008 - oracle does not prefix parameters with the variable delimter...		
            //			string name = dialect.FormatVariableNameForQueryText(parameter.Name);
            string name = dialect.FormatVariableNameForCommandParameter(command.CommandType, parameter.Name);
            newParameter.ParameterName = name;
            newParameter.Value = useValue;
            newParameter.Direction = parameter.Direction;

            // this block doesn't JIT compile on Linux...
            if (!(parameter.IsStructured))
                newParameter.DbType = parameter.DBType;
            else
            {
                var sqlParameter = (SqlParameter)newParameter;

                if (parameter.Value == null)
                    throw new InvalidOperationException("'parameter.Value' is null.");

                if (parameter.Value is DataTable)
                {
                    var columnType = ((DataTable)parameter.Value).Columns[0].DataType;
                    if (typeof(long).IsAssignableFrom(columnType))
                        sqlParameter.TypeName = "__bfxIdsInt64";
                    else if (typeof(int).IsAssignableFrom(columnType))
                        sqlParameter.TypeName = "__bfxIdsInt32";
                    else if (typeof(string).IsAssignableFrom(columnType))
                        sqlParameter.TypeName = "__bfxIdsString";
                    else if (typeof(Guid).IsAssignableFrom(columnType))
                        sqlParameter.TypeName = "__bfxIdsGuid";
                    else
                        throw new NotSupportedException(string.Format("Cannot handle '{0}'.", columnType));
                }
                else if (parameter.Value is StructuredParameterData)
                {
                    var data = (StructuredParameterData)parameter.Value;
                    sqlParameter.TypeName = data.NativeTypeName;
                    sqlParameter.Value = data.Data;
                }
                else
                    throw new NotSupportedException(string.Format("Cannot handle '{0}'.", parameter.Value.GetType()));

                sqlParameter.SqlDbType = SqlDbType.Structured;
            }

            // finalise...
            this.FinalizeParameterConfiguration(command, parameter, newParameter, dialect);

            // return...
            return newParameter;
        }

        /// <summary>
        /// Finalises the configuration of a parameter.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="parameter"></param>
        /// <param name="formatter"></param>
        /// <param name="dataParameter"></param>
        protected virtual void FinalizeParameterConfiguration(IDbCommand command, SqlStatementParameter parameter, IDataParameter dataParameter,
            SqlDialect dialect)
        {
        }

        /// <summary>
        /// Creates an exception for the given command.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="originalException"></param>
        /// <returns></returns>
        // mbr - 01-08-2008 - changed this from abstract.		
        protected internal virtual ApplicationException CreateCommandException(string message, IDbCommand command, Exception originalException, 
            SqlStatement sql = null)
        {
            if (command == null)
                throw new ArgumentNullException("command");
            if (originalException == null)
                throw new ArgumentNullException("originalException");

            // mbr - 2014-12-05 - the command has failed, so remove it from the cache... 1) it's hard to 
            // just remove an item, 2) this might be a general problem with the connection so tear them 
            // all down...
            this.CommandCache.Clear();

            // create the new exception...
            return SqlServerException.CreateException(message, command, originalException, sql);
        }

        /// <summary>
        /// Executes the given statement end returns an entity reader.
        /// </summary>
        /// <param name="statement"></param>
        public IList ExecuteEntityCollection(ISqlStatementSource statement)
        {
            if (statement == null)
                throw new ArgumentNullException("statement");

            // run...
            return this.ExecuteEntityCollection(statement.GetStatement());
        }

        /// <summary>
        /// Executes the given statement end returns an entity reader.
        /// </summary>
        /// <param name="statement"></param>
        public IList ExecuteEntityCollection(SqlStatement statement)
        {
            if (statement == null)
                throw new ArgumentNullException("statement");

            // map...
            SelectMap map = statement.SelectMap;
            if (map == null)
                throw new InvalidOperationException("Statement does not have a select map.");

            // results...
            if (map.EntityType == null)
                throw new InvalidOperationException("map.EntityType is null.");
            IList results = map.EntityType.CreateCollectionInstance();

            // create a reader...
            this.CaptureProfile<object>(statement, null, SqlMethod.EntityCollection, (profiling) =>
            {
                using (EntityReader reader = this.ExecuteEntityReader(statement))
                {
                    profiling.Command = reader.Command;

                    // loop...
                    while (true)
                    {
                        object entity = reader.Read();
                        if (entity == null)
                            break;

                        // add...
                        results.Add(entity);
                    }
                }

                return null;
            });

            // return...
            return results;
        }

        /// <summary>
        /// Executes the given statement end returns an entity reader.
        /// </summary>
        /// <param name="statement"></param>
        /// <returns>The entity, or <c>null</c> if the entity could not be found.</returns>
        public object ExecuteEntity(ISqlStatementSource statement)
        {
            // defer...
            return ExecuteEntity(statement, OnNotFound.ReturnNull);
        }

        /// <summary>
        /// Executes the given statement end returns an entity reader.
        /// </summary>
        /// <param name="statement"></param>
        public object ExecuteEntity(ISqlStatementSource statement, OnNotFound onNotFound)
        {
            if (statement == null)
                throw new ArgumentNullException("statement");

            // defer...
            return this.ExecuteEntity(statement.GetStatement());
        }

        /// <summary>
        /// Executes the given statement end returns an entity reader.
        /// </summary>
        /// <param name="statement"></param>
        /// <returns>The entity, or <c>null</c> if it could not be found.</returns>
        public object ExecuteEntity(SqlStatement statement)
        {
            // defer...
            return ExecuteEntity(statement, OnNotFound.ReturnNull);
        }

        /// <summary>
        /// Executes the given statement end returns an entity reader.
        /// </summary>
        /// <param name="statement"></param>
        public object ExecuteEntity(SqlStatement statement, OnNotFound onNotFound)
        {
            if (statement == null)
                throw new ArgumentNullException("statement");

            // map...
            SelectMap map = statement.SelectMap;
            if (map == null)
                throw new InvalidOperationException("Statement does not have a select map.");

            // create a reader...
            object theResult = this.CaptureProfile<object>(statement, null, SqlMethod.EntityCollection, (profiling) =>
            {
                using (EntityReader reader = this.ExecuteEntityReader(statement))
                {
                    profiling.Command = reader.Command;

                    // read an entity...
                    object result = null;
                    object entity = reader.Read();
                    if (entity != null)
                        result = entity;
                    else
                    {
                        switch (onNotFound)
                        {
                            case OnNotFound.ReturnNull:
                                result = null;
                                break;

                            case OnNotFound.ThrowException:
                                throw new EntityNotFoundException(string.Format(Cultures.Exceptions, "An entity of type '{0}' was not found.", reader.SelectMap.EntityType));

                            default:
                                throw new NotSupportedException(string.Format("Cannot handle '{0}' ({1}).", onNotFound, onNotFound.GetType()));
                        }
                    }

                    return result;
                }
            });

            //// note added thread local cache...
            //if (cache != null)
            //    cache.SetValue(hash, result);

            // return...
            return theResult;
        }

        /// <summary>
        /// Executes the given statement end returns an entity reader.
        /// </summary>
        /// <param name="statement"></param>
        // mbr - 22-09-2007 - made internal.	
        internal EntityReader ExecuteEntityReader(ISqlStatementSource statement)
        {
            if (statement == null)
                throw new ArgumentNullException("statement");

            // run...
            return this.ExecuteEntityReader(statement.GetStatement());
        }

        /// <summary>
        /// Executes the given statement end returns an entity reader.
        /// </summary>
        /// <param name="statement"></param>
        // mbr - 22-09-2007 - made internal.	
        internal EntityReader ExecuteEntityReader(SqlStatement statement)
        {
            if (statement == null)
                throw new ArgumentNullException("statement");

            // create a data reader...
            return EntityReader.CreateEntityReader(this, statement);
        }

        protected virtual IsolationLevel DefaultIsolutionLevel
        {
            get
            {
                // mbr - 2014-05-11 - changed...
                return Database.DefaultIsolationLevel;
            }
        }

        /// <summary>
        /// Begins a transaction.
        /// </summary>
        /// <returns></returns>
        public bool BeginTransaction()
        {
            return this.BeginTransaction(this.DefaultIsolutionLevel);
        }

        /// <summary>
        /// Begins a transaction.
        /// </summary>
        /// <returns></returns>
        public bool BeginTransaction(IsolationLevel isolationLevel)
        {
            // check...
            if (this.TransactionDepth == 0)
            {
                // check...
                if (CurrentTransaction != null)
                {
                    // mbr - 2010-02-18 - change...
                    //throw new InvalidOperationException("CurrentTransaction is already set.");
                    throw CreateTransactionHandlingException("CurrentTransaction is already set.");
                }

                // create...
                if (NativeConnection == null)
                {
                    // mbr - 2010-02-18 - change...
                    //throw new ArgumentNullException("NativeConnection");
                    throw CreateTransactionHandlingException("NativeConnection is null.");
                }
                this.EnsureNativeConnectionOpen();
                try
                {
                    try
                    {
                        _currentTransaction = this.NativeConnection.BeginTransaction(isolationLevel);
                    }
                    catch (NotSupportedException)
                    {
                        _currentTransaction = this.NativeConnection.BeginTransaction();
                    }
                }
                catch (Exception ex)
                {
                    // mbr - 2010-02-18 - change...
                    //throw new InvalidOperationException(string.Format("Failed to start a transaction with isolation level '{0}' on '{1}'.", isolationLevel, this), ex);
                    throw CreateTransactionHandlingException(string.Format("Failed to start a transaction with isolation level '{0}' on '{1}'.", isolationLevel, this), ex);
                }

                // check...
                if (_currentTransaction == null)
                {
                    // mbr - 2010-02-18 - change...
                    //throw new InvalidOperationException("_currentTransaction is null.");
                    throw CreateTransactionHandlingException("_currentTransaction is null.");
                }

                // mbr - 07-02-2008 - deprecated.			
                //				_connectionThread = Runtime.GetCurrentThreadId();

                // mbr - 17-04-2008 - added...
                _transactionId = Guid.NewGuid();
            }

            // add...
            _transactionDepth++;

            // return...
            if (this.TransactionDepth == 1)
                return true;
            else
                return false;
        }

        internal static BadTransactionException CreateTransactionHandlingException(string message)
        {
            return CreateTransactionHandlingException(message, null);
        }

        internal static BadTransactionException CreateTransactionHandlingException(string message, Exception ex)
        {
            // mbr - 2011-08-03 - added more logging here...
            StringBuilder builder = new StringBuilder();
            builder.Append("Bad transaction encountered: ");
            builder.Append(message);
            builder.Append("\r\n\r\n");
            if (ex != null)
                builder.Append(ex);
            else
            {
                builder.Append("NO EXCEPTION --> ");
                builder.Append(Environment.StackTrace);
            }
            EventLogAppender.AppendError(builder.ToString(), false);

            // go...
            return new BadTransactionException(message, ex);
        }

        private bool IsBound
        {
            get
            {
                if (this.BindUsage == 0)
                    return false;
                else
                    return true;
            }
        }

        /// <summary>
        /// Returns true if the connection is in a transaction.
        /// </summary>
        public bool IsInTransaction
        {
            get
            {
                // check...
                if (this.TransactionDepth == 0)
                    return false;
                else
                    return true;
            }
        }

        /// <summary>
        /// Commits the transaction.
        /// </summary>
        /// <returns></returns>
        public bool Commit()
        {
            // defer...
            return this.EndTransaction();
        }

        /// <summary>
        /// Commits the transaction.
        /// </summary>
        /// <returns></returns>
        public bool Rollback()
        {
            // run...
            _transactionRolledBack = true;
            return this.EndTransaction();
        }

        /// <summary>
        /// Ends the transaction.
        /// </summary>
        private bool EndTransaction()
        {
            // check...
            if (CurrentTransaction == null)
            {
                // mbr - 2010-02-18 - changed...
                //throw new ArgumentNullException("CurrentTransaction");
                throw CreateTransactionHandlingException("CurrentTransaction is null.");
            }

            // decrement...
            _transactionDepth--;

            // error...
            if (_transactionDepth < 0)
            {
                // mbr - 2010-02-18 - changed...
                //throw new InvalidOperationException("Transaction depth is less than zero.");
                throw CreateTransactionHandlingException(string.Format("Transaction depth is less than zero ({0})", _transactionDepth));
            }

            // tear down?
            if (this.TransactionDepth == 0)
            {
                // mbr - 2008-08-26 - JM identified an issue here where if SQL is gone, it will never recover from this...
                try
                {
                    // what to do?
                    if (this.TransactionRolledBack == true)
                    {
                        // rollback...
                        this.CurrentTransaction.Rollback();
                    }
                    else
                    {
                        // commit...
                        this.CurrentTransaction.Commit();
                    }

                    // dispose...
                    this.CurrentTransaction.Dispose();
                }
                finally
                {
                    // clear...
                    _currentTransaction = null;

                    // mbr - 17-04-2008 - added...
                    _transactionId = Guid.Empty;

                    // set...
                    _transactionRolledBack = false;
                }

                // return...
                return true;
            }
            else
            {
                // return...
                return false;
            }
        }

        // mbr - 07-02-2008 - deprecated.			
        //		/// <summary>
        //		/// Asserts that the current thread is allowed to use this connection.
        //		/// </summary>
        //		private void AssertThread()
        //		{
        //			int threadId = Runtime.GetCurrentThreadId();
        //			if(this.ConnectionThread != 0 && this.ConnectionThread != threadId)
        //				throw new InvalidOperationException(string.Format(Cultures.Log, "Thread #{0} ({1}) is not allowed to use a connection enlisted for thread #{2}.", 
        //					threadId, Thread.CurrentThread.Name, this.ConnectionThread));
        //		}

        // mbr - 07-02-2008 - deprecated.			
        //		/// <summary>
        //		/// Gets the connectionthread.
        //		/// </summary>
        //		private int ConnectionThread
        //		{
        //			get
        //			{
        //				// returns the value...
        //				return _connectionThread;
        //			}
        //		}

        /// <summary>
        /// Gets the transactiondepth.
        /// </summary>
        private int TransactionDepth
        {
            get
            {
                return _transactionDepth;
            }
        }

        /// <summary>
        /// Gets the currenttransaction.
        /// </summary>
        private IDbTransaction CurrentTransaction
        {
            get
            {
                // returns the value...
                return _currentTransaction;
            }
        }

        /// <summary>
        /// Gets whether the transaction was rolled back.
        /// </summary>
        private bool TransactionRolledBack
        {
            get
            {
                // returns the value...
                return _transactionRolledBack;
            }
        }

        /// <summary>
        /// Creates a connection.
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        internal static IConnection CreateConnection(string databaseName)
        {
            if (databaseName == null)
                return CreateConnection(Database.DefaultConnectionSettings);
            else
            {
                NamedDatabase database = Database.Databases[databaseName];
                if (database == null)
                    throw new InvalidOperationException(string.Format("A database with name '{0}' was not found.", databaseName));

                // return...
                return CreateConnection(database.Settings);
            }
        }

        /// <summary>
        /// Creates a connection.
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        public static IConnection CreateConnection(EntityType entityType)
        {
            if (entityType == null)
                throw new ArgumentNullException("entityType");

            // has?
            if (entityType.HasDatabaseName)
            {
                // get the settings...
                NamedDatabase namedDatabase = Database.Databases[entityType.DatabaseName];
                if (namedDatabase == null)
                    throw new InvalidOperationException("namedDatabase is null.");

                // defer...
                return CreateConnection(namedDatabase.Settings);
            }
            else
                return CreateConnection(Database.DefaultConnectionType, Database.DefaultConnectionString);
        }

        public static IConnection CreateConnection<T>()
            where T : Entity
        {
            var et = typeof(T).ToEntityType();
            return CreateConnection(et);
        }

        /// <summary>
        /// Creates a connection.
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        public static IConnection CreateConnection(ConnectionSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException("settings");

            // defer...
            return CreateConnection(settings.ConnectionType, settings.ConnectionString);
        }

        /// <summary>
        /// Creates a connection.
        /// </summary>
        /// <param name="connectionType"></param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public static IConnection CreateConnection(Type connectionType, string connectionString)
        {
            if (connectionType == null)
                throw new ArgumentNullException("connectionType");
            if (connectionString == null)
                throw new ArgumentNullException("connectionString");
            if (connectionString.Length == 0)
                throw new ArgumentOutOfRangeException("'connectionString' is zero-length.");

            // do we have a connection factory?
            if (typeof(IConnectionFactory).IsAssignableFrom(connectionType))
            {
                IConnectionFactory factory = (IConnectionFactory)Activator.CreateInstance(connectionType);
                if (factory == null)
                    throw new InvalidOperationException("factory is null.");
                return factory.CreateConnection(connectionString);
            }
            else
                return (IConnection)Activator.CreateInstance(connectionType, new object[] { connectionString });
        }

        /// <summary>
        /// Gets the connection's schema.
        /// </summary>
        /// <returns></returns>
        // mbr - 02-10-2007 - for c7 - added optional specification.
        public SqlSchema GetSchema(GetSchemaArgs args)
        {
            if (args == null)
                throw new ArgumentNullException("args");

            // load...
            SqlSchema schema = this.CreateSchema(args);
            if (schema == null)
                throw new InvalidOperationException("schema is null.");

            // fix it...
            schema.Fixup();

            // return...
            return schema;
        }

        /// <summary>
        /// Gets teh schema for the database.
        /// </summary>
        /// <returns></returns>
        // mbr - 02-10-2007 - for c7 - added optional specification.		
        protected virtual SqlSchema CreateSchema(GetSchemaArgs args)
        {
            if (args == null)
                throw new ArgumentNullException("args");

            // get...
            InformationSchemaFactory factory = new InformationSchemaFactory(this);
            return factory.GetSchema(args);
        }

        /// <summary>
        /// Gets the database type for the given native type name.
        /// </summary>
        /// <param name="nativeTypeName"></param>
        /// <returns></returns>
        public FieldSpecification GetFieldSpecificationForNativeTypeName(string nativeTypeName)
        {
            return this.GetFieldSpecificationForNativeTypeName(nativeTypeName, true);
        }

        public abstract FieldSpecification GetFieldSpecificationForNativeTypeName(string nativeTypeName, bool throwIfNotFound);

        /// <summary>
        /// Tests that a connection can be made with the given settings.
        /// </summary>
        /// <param name="settings"></param>
        public static void TestConnection(ConnectionSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException("settings");

            // get it...
            using (IConnection connection = CreateConnection(settings))
            {
                // try and open it...
                connection.EnsureConnectivity();
            }
        }

        /// <summary>
        /// Ensures we can connect.
        /// </summary>
        public void EnsureConnectivity()
        {
            // defer...
            this.EnsureNativeConnectionOpen();
        }

        /// <summary>
        /// Returns a null lease.
        /// </summary>
        /// <returns></returns>
        [SecurityCritical]
        public override object InitializeLifetimeService()
        {
            return null;
        }

        /// <summary>
        /// Splits a connection string.
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public static IDictionary SplitConnectionString(string connectionString)
        {
            if (connectionString == null)
                throw new ArgumentNullException("connectionString");
            if (connectionString.Length == 0)
                throw new ArgumentOutOfRangeException("'connectionString' is zero-length.");

            // mbr - 05-10-2005 - refactored to work with property bag...
            NameValueStringPropertyBag bag = new NameValueStringPropertyBag(connectionString);
            return bag;
        }

        /// <summary>
        /// Returns true if the query can do multiple amounts of work in a single query.
        /// </summary>
        public virtual bool SupportsMultiStatementQueries
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets the dialect.
        /// </summary>
        public abstract SqlDialect Dialect
        {
            get;
        }

        /// <summary>
        /// Checks the previous execution of a statement.
        /// </summary>
        /// <param name="statement"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        /// <remarks>If the thread is marked as <c>Read-only</c>c>, multiple calls to the same statement </remarks>
        //private bool CheckPreviousExecution(ExecuteType type, SqlStatement statement, ref object result, ref string hash, ref BoundCache cache)
        //{
        //    //			if(statement == null)
        //    //				throw new ArgumentNullException("statement");
        //    //
        //    //			// pessimist...
        //    //			result = null;
        //    //
        //    //			// if we're in a transaction, don't do it...
        //    //			if(this.CurrentTransaction != null)
        //    //				return false;
        //    //			
        //    //			// get the bound cache...
        //    //			if(BoundCacheSlot == null)
        //    //				throw new InvalidOperationException("BoundCacheSlot is null.");
        //    //			cache = (BoundCache)Thread.GetData(BoundCacheSlot);
        //    //
        //    //			// do we have one?
        //    //			if(cache == null)
        //    //				return false;
        //    //
        //    //			// hash...
        //    //			hash = statement.GetHash(type);
        //    //			if(hash == null)
        //    //				throw new InvalidOperationException("'hash' is null.");
        //    //			if(hash.Length == 0)
        //    //				throw new InvalidOperationException("'hash' is zero-length.");
        //    //
        //    //			// do we have an item in it?
        //    //			return cache.GetValue(hash, ref result);
        //    result = null;
        //    return false;
        //}

        ///// <summary>
        ///// Gets the boundcacheslot.
        ///// </summary>
        //private static LocalDataStoreSlot BoundCacheSlot
        //{
        //    get
        //    {
        //        return _boundCacheSlot;
        //    }
        //}

        ///// <summary>
        ///// Binds a thread local cache.
        ///// </summary>
        //public static void BindThreadLocalCache()
        //{
        //    if (BoundCacheSlot == null)
        //        throw new InvalidOperationException("BoundCacheSlot is null.");

        //    ILog log = LogSet.GetLog(typeof(Connection));

        //    // get it...
        //    BoundCache cache = (BoundCache)Thread.GetData(BoundCacheSlot);
        //    if (cache == null)
        //    {
        //        cache = new BoundCache();
        //        Thread.SetData(BoundCacheSlot, cache);
        //    }

        //    // increment...
        //    cache.Usage++;
        //}

        ///// <summary>
        ///// Unbinds a thread local cache.
        ///// </summary>
        //public static void UnbindThreadLocalCache()
        //{
        //    if (BoundCacheSlot == null)
        //        throw new InvalidOperationException("BoundCacheSlot is null.");

        //    ILog log = LogSet.GetLog(typeof(Connection));

        //    // get it...
        //    BoundCache cache = (BoundCache)Thread.GetData(BoundCacheSlot);
        //    if (cache == null)
        //        throw new InvalidOperationException("cache is null.");

        //    // decrement...
        //    cache.Usage--;

        //    // remove?
        //    if (cache.Usage == 0)
        //    {
        //        Thread.SetData(BoundCacheSlot, null);
        //    }
        //}

        /// <summary>
        /// Gets or sets the sqlmode
        /// </summary>
        public SqlMode SqlMode
        {
            get
            {
                return _sqlMode;
            }
            set
            {
                // check to see if the value has changed...
                if (value != _sqlMode)
                {
                    // set the value...
                    _sqlMode = value;
                }
            }
        }

        ///// <summary>
        ///// Gets the commandcache.
        ///// </summary>
        //private IDictionary CommandCache
        //{
        //    get
        //    {
        //        // returns the value...
        //        return _commandCache;
        //    }
        //}

        /// <summary>
        /// Gets the bindusage.
        /// </summary>
        private int BindUsage
        {
            get
            {
                // returns the value...
                return _bindUsage;
            }
        }

        public void Bind()
        {
            _bindUsage++;
        }

        public bool Unbind()
        {
            _bindUsage--;

            if (_bindUsage == 0)
                return true;
            else
                return false;
        }

        internal void AppendFromAndKeyValues(SqlStatement statement, StringBuilder builder, EntityType entityType, object[] keyValues)
        {
            if (statement == null)
                throw new ArgumentNullException("statement");
            if (builder == null)
                throw new ArgumentNullException("builder");
            if (entityType == null)
                throw new ArgumentNullException("entityType");
            if (keyValues == null)
                throw new ArgumentNullException("keyValues");
            if (keyValues.Length == 0)
                throw new ArgumentOutOfRangeException("'keyValues' is zero-length.");

            // walk...
            builder.Append(" ");
            builder.Append(this.Dialect.FromKeyword);
            builder.Append(" ");
            builder.Append(this.Dialect.FormatTableName(entityType.NativeName.Name));
            builder.Append(" ");
            builder.Append(this.Dialect.WhereKeyword);

            // key...
            EntityField[] keyFields = entityType.GetKeyFields();
            if (keyFields == null)
                throw new InvalidOperationException("keyFields is null.");
            if (keyFields.Length != keyValues.Length)
                throw new InvalidOperationException(string.Format("Length mismatch for 'keyFields' and 'keyValues': {0} cf {1}.", keyFields.Length, keyValues.Length));

            // walk...
            for (int index = 0; index < keyFields.Length; index++)
            {
                builder.Append(" ");
                if (index > 0)
                {
                    builder.Append(this.Dialect.AndKeyword);
                    builder.Append(" ");
                }

                // column...
                builder.Append(this.Dialect.FormatColumnName(keyFields[index].NativeName.Name));
                builder.Append("=");

                // param...
                string name = statement.Parameters.GetUniqueParameterName();
                builder.Append(this.Dialect.FormatVariableNameForQueryText(name));
                statement.Parameters.Add(new SqlStatementParameter(name, keyFields[index].DBType, keyValues[index]));
            }
        }


        /// <summary>
        /// Copies a BLOB value to a stream.
        /// </summary>
        /// <param name="field"></param>
        /// <param name="outStream"></param>
        public long CopyBlobValueToStream(EntityField field, object[] keyValues, Stream outStream)
        {
            if (field == null)
                throw new ArgumentNullException("field");
            if (keyValues == null)
                throw new ArgumentNullException("keyValues");
            if (keyValues.Length == 0)
                throw new ArgumentOutOfRangeException("'keyValues' is zero-length.");
            if (outStream == null)
                throw new ArgumentNullException("outStream");

            // create a statement...
            SqlStatement statement = new SqlStatement(this.Dialect);

            // fields...
            EntityType entityType = field.EntityType;
            if (entityType == null)
                throw new InvalidOperationException("entityType is null.");

            // create a SQL statement...
            StringBuilder builder = new StringBuilder();
            builder.Append(this.Dialect.SelectKeyword);
            builder.Append(" ");
            builder.Append(this.Dialect.FormatColumnName(field.NativeName.Name));
            this.AppendFromAndKeyValues(statement, builder, entityType, keyValues);

            // set...
            statement.CommandText = builder.ToString();

            // mbr - 2014-11-30 - caching means we no longer dispose here...
            var command = this.CreateCommand(statement);
            {
                try
                {
                    // ensure...
                    this.EnsureConnectivity();

                    // read...
                    using (IDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // get...
                            const int bufLen = 10240;
                            byte[] buf = new byte[bufLen];
                            long offset = 0;
                            bool finished = false;
                            while (!(finished))
                            {
                                long read = reader.GetBytes(0, offset, buf, 0, bufLen);
                                if (read == 0)
                                    break;

                                // copy...
                                outStream.Write(buf, 0, (int)read);

                                // next...
                                offset += read;
                            }

                            return offset;
                        }
                        else
                            throw new InvalidOperationException("No record was returned.");
                    }
                }
                catch (Exception ex)
                {
                    throw this.CreateCommandException("Failed to read BLOB.", command, ex);
                }
            }
        }

        /// <summary>
        /// Copies a BLOB value to a stream.
        /// </summary>
        /// <param name="field"></param>
        /// <param name="outStream"></param>
        public virtual void CopyStreamToBlobValue(EntityField field, object[] keyValues, Stream inStream)
        {
            // TODO: This method has not been implemented.  Check whether it is a valid not implementated, or a code path that was not completed.
            throw new NotImplementedException(string.Format("Not implemented for '{0}'.", this.GetType()));
        }

        /// <summary>
        /// Returns the available catalog names.
        /// </summary>
        /// <returns></returns>
        // mbr - 06-06-2006 - added.		
        public virtual string[] GetCatalogNames()
        {
            return this.ExecuteValuesVerticalString(new SqlStatement("select name from sysdatabases order by name"));
        }

        /// <summary>
        /// Executes the given statement, wrapping it in an IF EXISTS statement and returning a boolean.
        /// </summary>
        /// <param name="statementSource"></param>
        /// <returns></returns>
        public bool ExecuteExists(ISqlStatementSource statementSource)
        {
            if (statementSource == null)
                throw new ArgumentNullException("statementSource");

            // get it...
            SqlStatement sql = statementSource.GetStatement();
            if (sql == null)
                throw new InvalidOperationException("sql is null.");

            //declare @result bit;
            //select @result=1 where exists
            //(SELECT     ConfigId, CompanyName, ProductName, Name, Data, Version
            //FROM         BfxConfiguration
            //WHERE     (Name = 'foo'));
            //return @result;
            sql.CommandText = string.Format(@"declare @result bit; select @result=1 where exists ({0}); select @result;",
                sql.CommandText);

            // result...
            object result = this.ExecuteScalar(sql);
            if (result is DBNull)
                return false;
            else
                return true;
        }

        /// <summary>
        /// Executes the key values for the source.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public object[][] ExecuteKeyValues(ISqlStatementSource source)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            // check...
            if (source is SqlStatementCreator)
                return ((SqlStatementCreator)source).ExecuteKeyValues(this);
            else
                throw new NotSupportedException(string.Format("Cannot handle '{0}'.", source));
        }

        /// <summary>
        /// Gets the transactionid.
        /// </summary>
        public Guid TransactionId
        {
            get
            {
                return _transactionId;
            }
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(base.ToString());
            if(this.CurrentTransaction != null)
            {
                builder.Append(" <");
                builder.Append(this.TransactionId);
                builder.Append(">");
            }

            // return...
            return builder.ToString();
        }
    }
}
