// BootFX - Application framework for .NET applications
// 
// File: IConnection.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.IO;
using System.Data;
using System.Collections;
using BootFX.Common.Data.Schema;
using BootFX.Common.Data.Formatters;
using BootFX.Common.Entities;
using System.Collections.Generic;

namespace BootFX.Common.Data
{
	/// <summary>
	/// Describes an interface for connections.
	/// </summary>
	public interface IConnection : IDisposable
	{
		/// <summary>
		/// Executes the given statement and returns the first column value in the first row, or null if no row was returned.
		/// </summary>
		/// <param name="statement"></param>
		object ExecuteScalar(ISqlStatementSource statementSource);

		/// <summary>
		/// Executes the given statement and returns the first column value in the first row, or null if no row was returned, converting to the given type and smoothing out DB null values and CLR null values.
		/// </summary>
		/// <param name="statement"></param>
		object ExecuteScalar(ISqlStatementSource statementSource, Type type);

		DateTime ExecuteScalarDateTime(ISqlStatementSource statement);
		string ExecuteScalarString(ISqlStatementSource statement);
		short ExecuteScalarInt16(ISqlStatementSource statement);
		int ExecuteScalarInt32(ISqlStatementSource statement);
		long ExecuteScalarInt64(ISqlStatementSource statement);
		decimal ExecuteScalarDecimal(ISqlStatementSource statement);
		Guid ExecuteScalarGuid(ISqlStatementSource statement);
		float ExecuteScalarSingle(ISqlStatementSource statement);
		double ExecuteScalarDouble(ISqlStatementSource statement);
		bool ExecuteScalarBoolean(ISqlStatementSource statement);
		byte ExecuteScalarByte(ISqlStatementSource statement);

		/// <summary>
		/// Executes the given statement and returns the number of affected rows.
		/// </summary>
		/// <param name="statement"></param>
		int ExecuteNonQuery(ISqlStatementSource statementSource);

		bool Commit();
		bool Rollback();

		DataSet ExecuteDataSet(ISqlStatementSource statementSource);

		DataTable ExecuteDataTable(ISqlStatementSource statementSource);

		IDbCommand CreateCommand(ISqlStatementSource statementSource);

		object ExecuteEntity(ISqlStatementSource statementSource, OnNotFound onNotFound);

		IList ExecuteEntityCollection(ISqlStatementSource statementSource);

		bool BeginTransaction();

		bool BeginTransaction(IsolationLevel isolationLevel);

		object[] ExecuteValues(ISqlStatementSource statementSource);

		short[] ExecuteValuesVerticalInt16(ISqlStatementSource statementSource);
		int[] ExecuteValuesVerticalInt32(ISqlStatementSource statementSource);
		long[] ExecuteValuesVerticalInt64(ISqlStatementSource statementSource);
		string[] ExecuteValuesVerticalString(ISqlStatementSource statementSource);
		bool[] ExecuteValuesVerticalBoolean(ISqlStatementSource statementSource);
		float[] ExecuteValuesVerticalFloat(ISqlStatementSource statementSource);
		double[] ExecuteValuesVerticalDouble(ISqlStatementSource statementSource);
		decimal[] ExecuteValuesVerticalDecimal(ISqlStatementSource statementSource);
		object[] ExecuteValuesVertical(ISqlStatementSource statementSource);
        IEnumerable<T> ExecuteValuesVertical<T>(ISqlStatementSource statementSource);

		/// <summary>
		/// Executes the given statement, wrapping it in an IF EXISTS statement and returning a boolean.
		/// </summary>
		/// <param name="statementSource"></param>
		/// <returns></returns>
		// mbr - 28-09-2007 - added.		
		bool ExecuteExists(ISqlStatementSource statementSource);

		/// <summary>
		/// Gets the schema.
		/// </summary>
		/// <param name="tableSearchSpecification">Specifies an optional search specification for tables.</param>
		/// <returns></returns>
		// mbr - 02-10-2007 - for c7 - added table search specification.		
//		SqlSchema GetSchema();
		SqlSchema GetSchema(GetSchemaArgs args);

		void EnsureConnectivity();

		void EnsureOpen();

        bool DoesTableExist<T>()
            where T : Entity;

        bool DoesTableExist(string nativeName);

		bool DoesTableExist(NativeName nativeName);

		/// <summary>
		/// Returns true if the query can do multiple amounts of work in a single query.
		/// </summary>
		bool SupportsMultiStatementQueries
		{
			get;
		}

		SqlDialect Dialect
		{
			get;
		}

		SqlMode SqlMode
		{
			get;
			set;
		}

		void Bind();

		bool Unbind();

		/// <summary>
		/// Copies the blob in the given field to a stream returning the length
		/// </summary>
		/// <param name="field"></param>
		/// <param name="stream"></param>
		long CopyBlobValueToStream(EntityField storageField, object[] keyValues, Stream stream);

		/// <summary>
		/// Copies a stream to a BLOB value in the given field
		/// </summary>
		/// <param name="field"></param>
		/// <param name="keyValues"></param>
		/// <param name="stream"></param>
		void CopyStreamToBlobValue(EntityField storageField, object[] keyValues, Stream stream);

		/// <summary>
		/// Returns the available catalog names.
		/// </summary>
		/// <returns></returns>
		// mbr - 06-06-2006 - added.		
		string[] GetCatalogNames();

		/// <summary>
		/// Executes the key values for the source.
		/// </summary>
		/// <param name="source"></param>
		/// <returns></returns>
		// mbr - 26-11-2007 - for c7 - added.
		object[][] ExecuteKeyValues(ISqlStatementSource source);

		/// <summary>
		/// Gets the transation ID.
		/// </summary>
		// mbr - 17-04-2008 - added.		
		Guid TransactionId
		{
			get;
		}

        bool DoesColumnExist(string tableName, string columnName);
        bool DoesIndexExist(string tableName, string indexName);
    }
}
