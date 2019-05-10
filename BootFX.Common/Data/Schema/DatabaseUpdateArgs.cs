// BootFX - Application framework for .NET applications
// 
// File: DatabaseUpdateArgs.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Collections;

namespace BootFX.Common.Data
{
	/// <summary>
	/// Summary description for DatabaseUpdateArgs.
	/// </summary>
	public class DatabaseUpdateArgs
	{
        /// <summary>
        /// Private value to support the <see cref="DatabaseName">DatabaseName</see> property.
        /// </summary>
        private string _databaseName;

        /// <summary>
		/// Private field to support <see cref="LimitEntityTypes"/> property.
		/// </summary>
		private ArrayList _limitEntityTypes = new ArrayList();

        public bool AddArrayParameterUdts { get; set; }

        public bool Trace { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public DatabaseUpdateArgs()
		{
            if(Database.ArrayParameterType == ArrayParameterType.Array)
                this.AddArrayParameterUdts = true;

            this.Trace = true;
		}

		/// <summary>
		/// Gets a collection of entity types to limit the value.
		/// </summary>
		public ArrayList LimitEntityTypes
		{
			get
			{
				return _limitEntityTypes;
			}
		}

        /// <summary>
        /// Gets or sets the named database to update.
        /// </summary>
        /// <remarks>This is used to instruct database update to work with entities limited to the given database name.
        /// By default, database update skips named database entities.</remarks>
        // mbr - 2010-01-29 - added...
        public string DatabaseName
        {
            get
            {
                return _databaseName;
            }
            set
            {
                _databaseName = value;
            }
        }

        internal bool HasDatabaseName
        {
            get
            {
                return !(string.IsNullOrEmpty(this.DatabaseName));
            }
        }
	}
}
