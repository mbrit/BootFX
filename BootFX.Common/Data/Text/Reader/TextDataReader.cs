// BootFX - Application framework for .NET applications
// 
// File: TextDataReader.cs
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
using System.Data;
using System.Collections;

namespace BootFX.Common.Data.Text
{
	/// <summary>
	/// Defines an instance of <c>TextDataReader</c>.
	/// </summary>
	public abstract class TextDataReader : DataReader
	{
		/// <summary>
		/// Private field to support <see cref="firstRowIsHeader"/> property.
		/// </summary>
		private bool _firstRowIsHeader;

		/// <summary>
		/// Private field to support <c>Reader</c> property.
		/// </summary>
		private TextReader _reader;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="reader"></param>
		protected TextDataReader(TextReader reader)
		{
			if(reader == null)
				throw new ArgumentNullException("reader");
			
			// set...
			_reader = reader;
		}

		protected TextDataReader(TextReader reader, bool firstRowIsHeader)
			: this(reader)
		{
			_firstRowIsHeader = firstRowIsHeader;
		}

		/// <summary>
		/// Gets the reader.
		/// </summary>
		private TextReader Reader
		{
			get
			{
				// returns the value...
				return _reader;
			}
		}

		protected string ReadLine()
		{
			if(Reader == null)
				throw new InvalidOperationException("Reader is null.");
			return this.Reader.ReadLine();
		}

		/// <summary>
		/// Gets the firstrowIsHeader.
		/// </summary>
		private bool FirstRowIsHeader
		{
			get
			{
				return _firstRowIsHeader;
			}
		}

		/// <summary>
		/// Reads all the data and returns a data table.
		/// </summary>
		/// <returns></returns>
		public DataTable ReadToEndAsDataTable()
		{
			DataTable results = new DataTable();

			// schema...
			DataTable schema = this.GetSchemaTable();
			if(schema == null)
				throw new InvalidOperationException("schema is null.");
			foreach(DataColumn column in this.GetSchemaTable().Columns)
				results.Columns.Add(column.ColumnName, column.DataType);

			// walk...
			while(this.Read())
			{
				object[] values = new object[this.FieldCount];
				this.GetValues(values);

				// add...
				results.Rows.Add(values);
			}

			// return...
			return results;
		}
		
		protected override void InitializeSchema(DataTable table)
		{
			if(table == null)
				throw new ArgumentNullException("table");
			
			// read...
			string[] values = this.ReadValuesFromStream();
			foreach(string value in values)
				table.Columns.Add(this.GetNextColumnName(table, value));
		}

		/// <summary>
		/// Reads in the values from the stream.
		/// </summary>
		/// <returns></returns>
		protected abstract string[] ReadValuesFromStream();
		
		protected override bool ReadRecord(int recordIndex, object[] values)
		{
			// load...
			object[] newValues = this.ReadValuesFromStream();
			if(newValues == null)
				return false;

			// copy...
			if(newValues.Length > values.Length)
				throw new InvalidOperationException(string.Format("There are more values in the input than there are columns in the table: {0} c.f. {1}.", newValues.Length, values.Length));

			// copy...
			for(int index = 0; index < values.Length; index++)
			{
				if(index < newValues.Length)
					values[index] = newValues[index];
				else
					values[index] = null;
			}

			// ok...
			return true;
		}

        public override object GetValue(int index, Type type)
        {
            if (typeof(DateTime).IsAssignableFrom(type))
            {
                var asString = this.GetValue<string>(index);
                if (!(string.IsNullOrEmpty(asString)))
                    return ConversionHelper.ToDateTime(asString);
                else
                    return DateTime.MinValue;
            }
            else
                return base.GetValue(index, type);
        }
	}
}
