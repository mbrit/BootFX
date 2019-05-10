// BootFX - Application framework for .NET applications
// 
// File: TextDataWriter.cs
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
using System.Collections;
using System.Globalization;
using BootFX.Common.Entities;

namespace BootFX.Common.Data
{
	/// <summary>
	/// Summary description for <see cref="TextDataWriter"/>.
	/// </summary>
	public class TextDataWriter
	{
		/// <summary>
		/// Creates a new instance of <see cref="TextDataWriter"/>.
		/// </summary>
		public TextDataWriter()
		{
		}

		/// <summary>
		/// Gets the CSV for the given list of entities.
		/// </summary>
		/// <param name="entities"></param>
		/// <param name="culture"></param>
		/// <returns></returns>
		public string ToCsv(IList entities, IFormatProvider culture)
		{
			if(entities == null)
				throw new ArgumentNullException("entities");
			if(culture == null)
				throw new ArgumentNullException("culture");
			
			// create...
			using(StringWriter writer = new StringWriter())
			{
				this.ToCsv(entities, culture, writer);
				return writer.GetStringBuilder().ToString();
			}
		}

		/// <summary>
		/// Gets the CSV for the given list of entities.
		/// </summary>
		/// <param name="entities"></param>
		public void ToCsv(IList entities, IFormatProvider culture, TextWriter writer)
		{
			if(entities == null)
				throw new ArgumentNullException("entities");
			if(writer == null)
				throw new ArgumentNullException("writer");
			if(culture == null)
				culture = Cultures.System;

			// header...
			EntityType entityType = null;
			if(entities is IEntityType)
				entityType = ((IEntityType)entities).EntityType;
			else
				throw new NotSupportedException(string.Format("Cannot handle '{0}'.", entities));

			// defer...
			this.ToCsv(entities, entityType, entityType.Fields.ToArray(), culture, writer);
		}

		/// <summary>
		/// Gets the CSV for the given list of entities.
		/// </summary>
		/// <param name="entities"></param>
		private void ToCsv(IList entities, EntityType entityType, EntityField[] fields, IFormatProvider culture, TextWriter writer)
		{
			if(entities == null)
				throw new ArgumentNullException("entities");
			if(writer == null)
				throw new ArgumentNullException("writer");
			if(entityType == null)
				throw new ArgumentNullException("entityType");
			if(fields == null)
				throw new ArgumentNullException("fields");
			if(fields.Length == 0)
				throw new ArgumentOutOfRangeException("'fields' is zero-length.");
			if(culture == null)
				culture = Cultures.System;

			// header row...
			for(int index = 0; index < fields.Length; index++)
			{
				if(index > 0)
					writer.Write(",");
				writer.Write(this.FormatData(fields[index].Name, culture));
			}
			writer.WriteLine();

			// data...
			foreach(object entity in entities)
			{
				// loop...
				for(int index = 0; index < fields.Length; index++)
				{
					if(index > 0)
						writer.Write(",");
					object data = entityType.Storage.GetValue(entity, fields[index]);
					string dataAsString = this.FormatData(data, culture);
					writer.Write(dataAsString);
				}

				// next...
				writer.WriteLine();
			}
		}

		/// <summary>
		/// Renders some data.
		/// </summary>
		/// <param name="data"></param>
		private string FormatData(object data, IFormatProvider culture)
		{
			string dataAsString = null;
			if(data == null)
				dataAsString = string.Empty;
			else
			{
				if(data is DateTime && (DateTime)data == DateTime.MinValue)
					dataAsString = string.Empty;
				else
					dataAsString = ConversionHelper.ToString(data, culture);
			}

			// comma?
			char[] quotable = new char[] { ',', '\r', '\n' };
			if(dataAsString.IndexOfAny(quotable) != -1)
			{
				// we need to quote it...
				dataAsString = dataAsString.Replace("\"", "\"\"");
				dataAsString = "\"" + dataAsString + "\"";
			}

			// that's it...
			return dataAsString;
		}
	}
}
