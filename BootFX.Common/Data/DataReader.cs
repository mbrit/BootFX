// BootFX - Application framework for .NET applications
// 
// File: DataReader.cs
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
using System.Data;
using System.Collections;

namespace BootFX.Common.Data
{
    /// <summary>
    /// Defines an instance of <c>DataReaderBase</c>.
    /// </summary>
    public abstract class DataReader : DataRecord, IDataReader
    {
        /// <summary>
        /// Private field to support <see cref="CurrentIndex"/> property.
        /// </summary>
        private int _currentIndex = -1;

        /// <summary>
        /// Private field to support <c>CurrentValues</c> property.
        /// </summary>
        private object[] _currentValues;

        /// <summary>
        /// Private field to support <see cref="SchemaTable"/> property.
        /// </summary>
        private DataTable _schemaTable;

        protected DataReader()
        {
        }

        ~DataReader()
        {
            try
            {
                this.Dispose();
            }
            catch
            {
                // no-op...
            }
        }

        public int RecordsAffected
        {
            get
            {
                return 0;
            }
        }

        public bool IsClosed
        {
            get
            {
                return false;
            }
        }

        public bool NextResult()
        {
            return false;
        }

        public void Close()
        {
            // TODO:  Add DataReaderBase.Close implementation
        }

        public bool Read()
        {
            // add...
            _currentIndex++;

            // get...
            object[] values = new object[this.FieldCount];
            bool result = this.ReadRecord(this.CurrentIndex, values);

            // set...
            _currentValues = values;

            // return...
            return result;
        }

        /// <summary>
        /// Gets the next item.
        /// </summary>
        /// <returns></returns>
        protected abstract bool ReadRecord(int index, object[] values);

        public int Depth
        {
            get
            {
                // TODO:  Add DataReaderBase.Depth getter implementation
                return 0;
            }
        }

        /// <summary>
        /// Gets the schematable.
        /// </summary>
        private DataTable SchemaTable
        {
            get
            {
                if (_schemaTable == null)
                {
                    DataTable schema = new DataTable();
                    InitializeSchema(schema);
                    _schemaTable = schema;
                }
                return _schemaTable;
            }
        }

        protected abstract void InitializeSchema(DataTable table);

        public DataTable GetSchemaTable()
        {
            return this.SchemaTable;
        }

        public void Dispose()
        {
            // sup...
            GC.SuppressFinalize(this);
        }

        public override string GetName(int i)
        {
            if (SchemaTable == null)
                throw new InvalidOperationException("SchemaTable is null.");
            return this.SchemaTable.Columns[i].ColumnName;
        }

        public override int FieldCount
        {
            get
            {
                if (SchemaTable == null)
                    throw new InvalidOperationException("SchemaTable is null.");
                return this.SchemaTable.Columns.Count;
            }
        }

        /// <summary>
        /// Gets the currentvalues.
        /// </summary>
        private object[] CurrentValues
        {
            get
            {
                // returns the value...
                return _currentValues;
            }
        }

        /// <summary>
        /// Gets the currentindex.
        /// </summary>
        private int CurrentIndex
        {
            get
            {
                return _currentIndex;
            }
        }

        protected override object GetRawValue(int index)
        {
            if (CurrentValues == null)
                throw new InvalidOperationException("CurrentValues is null.");
            return this.CurrentValues[index];
        }

        public override Type GetFieldType(int i)
        {
            if (SchemaTable == null)
                throw new InvalidOperationException("SchemaTable is null.");
            return this.SchemaTable.Columns[i].DataType;
        }

        /// <summary>
        /// Gets the next available column name.
        /// </summary>
        /// <param name="table"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        protected string GetNextColumnName(DataTable table, string name)
        {
            if (table == null)
                throw new ArgumentNullException("table");

            // are we missing a name?
            if (name == null || name.Length == 0)
                name = "Untitled";

            // walk...
            int index = 1;
            while (true)
            {
                string useName = name;
                if (index > 1)
                    useName = name + index.ToString();

                // get...
                DataColumn existing = table.Columns[useName];
                if (existing == null)
                    return useName;

                // next...
                index++;
            }
        }

        public int GetFieldIndex(string name, IErrorCollector errors)
        {
            var index = this.GetFieldIndex(name, false);
            if (index == -1)
                errors.AddIsRequiredError(name);
            return index;
        }

        public int GetFieldIndex(string name, bool throwIfNotFound = true)
        {
            for (var index = 0; index < this.SchemaTable.Columns.Count; index++)
            {
                if (string.Compare(this.SchemaTable.Columns[index].ColumnName, name, true) == 0)
                    return index;
            }

            if (throwIfNotFound)
                throw new InvalidOperationException(string.Format("A field with name '{0}' was not found.", name));
            else
                return -1;
        }
	}
}
