// BootFX - Application framework for .NET applications
// 
// File: SchemaHelper.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using BootFX.Common.Entities;

namespace BootFX.Common.Data.Schema
{
	/// <summary>
	/// Defines an instance of <c>Class1</c>.
	/// </summary>
	public sealed class SchemaHelper
	{
		private SchemaHelper()
		{
		}

        public static void CreateTable<T>(IConnection conn)
            where T : Entity
        {
            CreateTable(typeof(T), conn);
        }

        /// <summary>
        /// Creates a table for the given type.
        /// </summary>
        /// <param name="type"></param>
        public static void CreateTable(Type type, IConnection conn = null)
		{
			if(type == null)
				throw new ArgumentNullException("type");
			
			// get it...
			EntityType et = EntityType.GetEntityType(type, OnNotFound.ThrowException);
			if(et == null)
				throw new InvalidOperationException("et is null.");

			// create...
			SqlSchema schema = new SqlSchema();	
			SqlTable table = new SqlTable(schema, et);
			string[] scripts = Database.DefaultDialect.GetCreateTableScript(table, SqlTableScriptFlags.None);
			if(scripts == null)
				throw new InvalidOperationException("scripts is null.");

            // walk...
            var doClose = false;
            if (conn == null)
            {
                conn = Database.CreateConnection(et);
                doClose = true;
            }
            try
            {
                foreach (string script in scripts)
                    conn.ExecuteNonQuery(new SqlStatement(script));
            }
            finally
            {
                if (doClose)
                    conn.Dispose();
            }
		}

		public static void EnsureTableExists(Type type)
		{	
			if(type == null)
				throw new ArgumentNullException("type");
			
			if(!(DoesTableExist(type)))
				CreateTable(type);
		}

		/// <summary>
		/// Gets the schema for the database.
		/// </summary>
		/// <returns></returns>
		internal static bool DoesTableExist(Type type)
		{
			if(type == null)
				throw new ArgumentNullException("type");
			
			// get it...
			EntityType et = EntityType.GetEntityType(type, OnNotFound.ThrowException);
			if(et == null)
				throw new InvalidOperationException("et is null.");

			// get...
			using(IConnection connection = Database.CreateConnection(et))
				return connection.DoesTableExist(et.NativeName);
		}
	}
}
