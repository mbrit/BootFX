// BootFX - Application framework for .NET applications
// 
// File: DataWriter.cs
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
using System.Globalization;
using BootFX.Common.Entities;

namespace BootFX.Common.Data.Text
{
	/// <summary>
	/// Defines a class that can write text data.
	/// </summary>
	public abstract class DataWriter
	{
		/// <summary>
		/// Private field to support <see cref="Stream"/> property.
		/// </summary>
		private Stream _stream;
		
		/// <summary>
		/// Private field to support <see cref="Writer"/> property.
		/// </summary>
		private TextWriter _writer;

		/// <summary>
		/// Private field to support <see cref="Culture"/> property.
		/// </summary>
		private CultureInfo _culture = Cultures.System;
		
		/// <summary>
		/// Raised before a value is written.
		/// </summary>
		public event EventHandler BeforeValueWritten;

		/// <summary>
		/// Raised after a value is written.
		/// </summary>
		public event EventHandler AfterValueWritten;
		
		/// <summary>
		/// Private field to support <see cref="FieldNumber"/> property.
		/// </summary>
		private int _fieldNumber;

		protected DataWriter(Stream stream)
		{
			if(stream == null)
				throw new ArgumentNullException("stream");
			_stream = stream;
		}

		/// <summary>
		/// Gets the stream.
		/// </summary>
		protected Stream Stream
		{
			get
			{
				return _stream;
			}
		}
		
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="writer"></param>
		protected DataWriter(TextWriter writer)
		{
			if(writer == null)
				throw new ArgumentNullException("writer");
			_writer = writer;
		}

		/// <summary>
		/// Raises the <c>AfterValueWritten</c> event.
		/// </summary>
		private void OnAfterValueWritten()
		{
			OnAfterValueWritten(EventArgs.Empty);
		}
		
		/// <summary>
		/// Raises the <c>AfterValueWritten</c> event.
		/// </summary>
		protected virtual void OnAfterValueWritten(EventArgs e)
		{
			// raise...
			if(AfterValueWritten != null)
				AfterValueWritten(this, e);
		}
		
		/// <summary>
		/// Raises the <c>BeforeValueWritten</c> event.
		/// </summary>
		private void OnBeforeValueWritten()
		{
			OnBeforeValueWritten(EventArgs.Empty);
		}
		
		/// <summary>
		/// Raises the <c>BeforeValueWritten</c> event.
		/// </summary>
		protected virtual void OnBeforeValueWritten(EventArgs e)
		{
			// raise...
			if(BeforeValueWritten != null)
				BeforeValueWritten(this, e);
		}

		/// <summary>
		/// Gets the fieldnumber.
		/// </summary>
		protected int FieldNumber
		{
			get
			{
				return _fieldNumber;
			}
		}

		/// <summary>
		/// Formats a value for output.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		protected virtual string FormatValue(object value)
		{
			return ConversionHelper.ToString(value, this.Culture);
		}

		/// <summary>
		/// Gets the culture.
		/// </summary>
		public CultureInfo Culture
		{
			get
			{
				return _culture;
			}
			set
			{
				_culture = value;
			}
		}

		/// <summary>
		/// Writes an entire document from data.
		/// </summary>
		/// <remarks>This uses <see cref="DataTableTransformer"></see> to transform data to a table.</remarks>
		/// <param name="table"></param>
		public void WriteDocument(object data)
		{
			if(data == null)
				throw new ArgumentNullException("data");
			
			// get...
			DataTableTransformer transformer = new DataTableTransformer();
			DataTable table = transformer.Transform(data);
			if(table == null)
				throw new InvalidOperationException("table is null.");

			// output...
			this.WriteDocument(table);
		}

		/// <summary>
		/// Writes an entire document based on a collection of entities.
		/// </summary>
		/// <param name="table"></param>
		public void WriteDocument(EntityCollection entities)
		{
			if(entities == null)
				throw new ArgumentNullException("entities");
			
			// get...
			DataTable table = entities.ToDataTable();
			if(table == null)
				throw new InvalidOperationException("table is null.");

			// output...
			this.WriteDocument(table);
		}

		/// <summary>
		/// Writes an entire document based on a SQL statement.
		/// </summary>
		/// <param name="source"></param>
		public void WriteDocument(ISqlStatementSource source)
		{
			if(source == null)
				throw new ArgumentNullException("source");
			
			// execute...
			DataTable table = Database.ExecuteDataTable(source);
			if(table == null)
				throw new InvalidOperationException("table is null.");

			// write...
			this.WriteDocument(table);
		}

		/// <summary>
		/// Writes an entire document based on a DataTable.
		/// </summary>
		/// <param name="table"></param>
		public abstract void WriteDocument(DataTable table);
				
		/// <summary>
		/// Gets the writer.
		/// </summary>
		protected TextWriter Writer
		{
			get
			{
				return _writer;
			}
		}

		protected void IncrementFieldNumber()
		{
			_fieldNumber++;
		}

		protected void ResetFieldNumber()
		{
			_fieldNumber = 0;
		}
	}
}
