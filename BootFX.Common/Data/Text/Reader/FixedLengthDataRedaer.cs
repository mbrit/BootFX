// BootFX - Application framework for .NET applications
// 
// File: FixedLengthDataRedaer.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.IO;

namespace BootFX.Common.Data.Text
{
	/// <summary>
	/// Summary description for FixedLengthDataRedaer.
	/// </summary>
	public class FixedLengthDataReader : TextDataReader
	{
		/// <summary>
		/// Private field to support <c>ColumnWidths</c> property.
		/// </summary>
		private int[] _columnWidths;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="reader"></param>
		/// <param name="firstRowIsHeader"></param>
		/// <param name="columnWidths"></param>
		public FixedLengthDataReader(TextReader reader, bool firstRowIsHeader, int[] columnWidths)
			: base(reader, firstRowIsHeader)
		{
			if(columnWidths == null)
				throw new ArgumentNullException("columnWidths");
			if(columnWidths.Length == 0)
				throw new ArgumentOutOfRangeException("'columnWidths' is zero-length.");

			// check...
			if(!(firstRowIsHeader))
				throw new NotSupportedException("The first row must contain headers.");
			
			// set...
			_columnWidths = columnWidths;
		}

		/// <summary>
		/// Gets the columnwidths.
		/// </summary>
		private int[] ColumnWidths
		{
			get
			{
				// returns the value...
				return _columnWidths;
			}
		}

		/// <summary>
		/// Reads in values from the stream.
		/// </summary>
		/// <returns></returns>
		protected override string[] ReadValuesFromStream()
		{
			string buf = this.ReadLine();
			if(buf == null)
				return null;

			// split it...
			int start = 0;
			string[] results = new string[this.ColumnWidths.Length];
			for(int index = 0; index < this.ColumnWidths.Length; index++)
			{
				// get...
				string part = this.GetPart(buf, start, this.ColumnWidths[index]);
				results[index] = part;

				// next...
				start += this.ColumnWidths[index];
			}

			// return...
			return results;
		}

		/// <summary>
		/// Gets a portion of the string.
		/// </summary>
		/// <param name="buf"></param>
		/// <param name="start"></param>
		/// <param name="length"></param>
		/// <returns></returns>
		private string GetPart(string buf, int start, int length)
		{
			try
			{
				string part = buf.Substring(start, length);
				if(part == null)
					throw new InvalidOperationException("part is null.");

				// return...
				return part.Trim();
			}
			catch(Exception ex)
			{
				throw new InvalidOperationException(string.Format("Failed to get '{0}' character(s), starting at offset '{1}' from this string: {2}", 
					length, start, buf), ex);
			}
		}
	}
}
