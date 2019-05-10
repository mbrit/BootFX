// BootFX - Application framework for .NET applications
// 
// File: LineBasedDataWriter.cs
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
	/// Defines an instance of <c>LineBasedDataWriter</c>.
	/// </summary>
	public abstract class LineBasedDataWriter : DataWriter
	{

		protected LineBasedDataWriter(TextWriter writer) : base(writer)
		{
			// reset...
			ResetLine();
		}
	
		/// <summary>
		/// Writes a line break.
		/// </summary>
		public void WriteLine()
		{
			if(Writer == null)
				throw new InvalidOperationException("Writer is null.");
			this.Writer.WriteLine();

			// reset...
			this.ResetLine();
		}
		
		private void ResetLine()
		{
			this.ResetFieldNumber();
		}

        public void WriteValue(decimal value, int dp)
        {
            var asString = value.ToString("n" + dp);
            this.WriteValue(asString);
        }

		/// <summary>
		/// Writes a value.
		/// </summary>
		/// <param name="value"></param>
		public void WriteValue(object value)
		{
			string asString = this.FormatValue(value);

			// mbr - 07-05-2008 - replace line breaks...
			if(asString != null)
			{
				asString = asString.Replace("\r", string.Empty);
				asString = asString.Replace("\n", string.Empty);
			}

			// before...
			this.OnBeforeValueWritten(EventArgs.Empty);

			// write...
			this.Write(asString);

			// after...
			this.OnAfterValueWritten(EventArgs.Empty);

			// next...
			IncrementFieldNumber();
		}


		/// <summary>
		/// Writes an entire document based on a DataTable.
		/// </summary>
		/// <param name="table"></param>
		public override void WriteDocument(DataTable table)
		{
			if(table == null)
				throw new ArgumentNullException("table");
			if(table.Columns.Count == 0)
				throw new InvalidOperationException("'table.Columns' is zero-length.");

			// defer
			this.WriteDocument(table, LineBasedDataWriterFlags.Normal);
		}

		/// <summary>
		/// Writes an entire document based on a DataTable.
		/// </summary>
		/// <param name="table"></param>
		public virtual void WriteDocument(DataTable table, LineBasedDataWriterFlags flags)
		{
			if(table == null)
				throw new ArgumentNullException("table");
			if(table.Columns.Count == 0)
				throw new InvalidOperationException("'table.Columns' is zero-length.");
			
			// header?
			if((flags & LineBasedDataWriterFlags.ExcludeHeader) == 0)
			{
				foreach(DataColumn column in table.Columns)
					this.WriteValue(column.ColumnName);

				this.WriteLine();
			}

			// values...
			bool first = true;
			foreach(DataRow row in table.Rows)
			{
				if(first)
					first = false;
				else
					this.WriteLine();

				// values...
				foreach(DataColumn column in table.Columns)
					this.WriteValue(row[column]);
			}
		}
		
		protected void Write(string buf)
		{
			if(Writer == null)
				throw new InvalidOperationException("Writer is null.");
			this.Writer.Write(buf);
		}







	}
}
