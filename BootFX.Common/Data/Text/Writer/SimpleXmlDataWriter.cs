// BootFX - Application framework for .NET applications
// 
// File: SimpleXmlDataWriter.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.IO;
using System.Xml;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using BootFX.Common.Xml;

namespace BootFX.Common.Data.Text
{
	/// <summary>
	/// Defines an instance of <c>SimpleXmlDataWriter</c>.
	/// </summary>
	public class SimpleXmlDataWriter : DataWriter
	{
		public SimpleXmlDataWriter(Stream stream) : base(stream)
		{
		}

		public SimpleXmlDataWriter(TextWriter writer) : base(writer)
		{
		}

		public override void WriteDocument(System.Data.DataTable table)
		{
			if(table == null)
				throw new ArgumentNullException("table");
			
			// create...
			XmlDocument doc = new XmlDocument();

			// top...
			XmlElement element = doc.CreateElement("Data");
			doc.AppendChild(element);

			// build a list of columns...
			Lookup names = new Lookup();
			names.CreateItemValue += new CreateLookupItemEventHandler(names_CreateItemValue);

			// walk...
			foreach(DataRow row in table.Rows)
			{
				foreach(DataColumn column in table.Columns)
				{
					string name = (string)names[column.ColumnName];
					XmlElement data = doc.CreateElement(name);
					element.AppendChild(data);

					// set...
					XmlHelper.SetElementValue(data, row[column]);
				}
			}

			// save it...
			if(this.Stream != null)
				doc.Save(this.Stream);
			else if(this.Writer != null)
				doc.Save(this.Writer);
			else
				throw new InvalidOperationException("No output strategies were available to write to.");
		}

		private void names_CreateItemValue(object sender, CreateLookupItemEventArgs e)
		{
			e.NewValue = e.Key;
		}
	}
}
