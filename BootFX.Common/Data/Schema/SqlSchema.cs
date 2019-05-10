// BootFX - Application framework for .NET applications
// 
// File: SqlSchema.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Xml;
using System.Collections;
using System.Collections.Specialized;
using BootFX.Common.Xml;
using BootFX.Common.Data.Formatters;

namespace BootFX.Common.Data.Schema
{
	/// <summary>
	/// Describes a SQL schema.
	/// </summary>
	public class SqlSchema : ToXmlBase
	{
		/// <summary>
		/// Private field to support <c>Tables</c> property.
		/// </summary>
		private SqlTableCollection _tables = new SqlTableCollection();
		
		/// <summary>
		/// Private field to support <c>Procedures</c> property.
		/// </summary>
		private SqlProcedureCollection _procedures = new SqlProcedureCollection();
		
		/// <summary>
		/// Constructor.
		/// </summary>
		public SqlSchema()
		{
		}

		/// <summary>
		/// Outputs the XML.
		/// </summary>
		/// <param name="xml"></param>
		/// <param name="context"></param>
		protected override void WriteXml(System.Xml.XmlWriter xml, WriteXmlContext context)
		{
			// add...
			xml.WriteStartElement("Tables");
			foreach(SqlTable table in this.Tables)
				table.ToXml(xml, context.Encoding);
			xml.WriteEndElement();
		}

		/// <summary>
		/// Gets a collection of SqlTable objects.
		/// </summary>
		public SqlTableCollection Tables
		{
			get
			{
				return _tables;
			}
		}

		/// <summary>
		/// Merges the given XML file.
		/// </summary>
		/// <param name="xmlFilePath"></param>
		public void Merge(string xmlFilePath)
		{
			if(xmlFilePath == null)
				throw new ArgumentNullException("xmlFilePath");
			if(xmlFilePath.Length == 0)
				throw new ArgumentOutOfRangeException("'xmlFilePath' is zero-length.");
			
			// load...
			XmlDocument document = new XmlDocument();
			document.Load(xmlFilePath);

			// defer...
			Merge(document);
		}

		/// <summary>
		/// Merges the given XML element.
		/// </summary>
		/// <param name="document"></param>
		public void Merge(XmlElement element)
		{
			if(element == null)
				throw new ArgumentNullException("element");
			this.MergeInternal(element, false);
		}

		/// <summary>
		/// Merges the given XML document.
		/// </summary>
		/// <param name="document"></param>
		public void Merge(XmlDocument document)
		{
			if(document == null)
				throw new ArgumentNullException("document");

			// select...
			XmlElement schemaElement = (XmlElement)document.SelectSingleNode("SqlSchema");
			if(schemaElement == null)
				throw new InvalidOperationException("schemaElement is null.");

			// run...
			this.MergeInternal(schemaElement, false);
		}

		/// <summary>
		/// Merges the given XML node.
		/// </summary>
		/// <param name="xmlFilePath"></param>
		private void MergeInternal(XmlNode xml, bool createIfNotFound)
		{
			if(xml == null)
				throw new ArgumentNullException("xml");
			
			// element...
			if(xml.NodeType != XmlNodeType.Document && xml.NodeType != XmlNodeType.Element)
				throw new NotSupportedException(string.Format("Cannot handle '{0}'.", xml.NodeType));

			// select the tables...
			foreach(XmlElement tableElement in xml.SelectNodes("Tables/SqlTable"))
			{
				// get the name...
				string nativeName = XmlHelper.GetElementString(tableElement, "NativeName", OnNotFound.ThrowException);
				if(nativeName == null)
					throw new InvalidOperationException("'nativeName' is null.");
				if(nativeName.Length == 0)
					throw new InvalidOperationException("'nativeName' is zero-length.");

				// find that table...
				SqlTable table = this.Tables[nativeName];
				if(table == null && createIfNotFound)
				{
					table = new SqlTable(nativeName);
					this.Tables.Add(table);
				}

				// merge...
				if(table != null)
					table.Merge(tableElement, createIfNotFound);
			}

			// fix...
			this.Fixup();
		}

		/// <summary>
		/// Fixes up the project after the schema has been loaded, or a merge has been performed.
		/// </summary>
		/// <remarks>The principle of this is to automatically smooth over some of the sins of auto-generation.  For example,
		/// if the entity is called Rate, and this has a field called Rate, the field name will be changed to RateValue.</remarks>
		internal void Fixup()
		{
			// mangle...
			SqlSchema.MangleDuplicateNames(this.Tables.ToArray());

			// fix...
			foreach(SqlTable table in this.Tables)
				table.Fixup();

			// with each table done, walk them again and reset the child tables...
			foreach(SqlTable table in this.Tables)
			{
				// clear...
				table.AssociatedLinks.Clear();

				// walk the other tables...
				foreach(SqlTable childTable in this.Tables)
				{
					// not this...
					if(table != childTable)
					{
						// child to parent...
						foreach(SqlChildToParentLink link in childTable.LinksToParents)
						{
							// found...
							if(link.ParentTable == table)
								table.AssociatedLinks.Add(link);
						}
					}
				}
			}
		}

		/// <summary>
		/// Mangles duplicate names.
		/// </summary>
		/// <param name="members"></param>
		internal static void MangleDuplicateNames(SqlMember[] members)
		{
			if(members == null)
				throw new ArgumentNullException("members");
			
			// create...
			Hashtable names = CollectionsUtil.CreateCaseInsensitiveHashtable();
			foreach(SqlMember member in members)
			{
				// get it...
				string key = member.Name;
				if(names.Contains(key))
				{
					// count...
					int count = (int)names[key];
					count++;

					// adjust...
					member.Name += count.ToString();

					// reset...
					names[member.Name] = count;
				}
				else
					names.Add(key, 1);
			}
		}

		/// <summary>
		/// Loads the project from XML.
		/// </summary>
		/// <param name="filePath"></param>
		/// <returns></returns>
		public static SqlSchema FromXml(string filePath)
		{
			if(filePath == null)
				throw new ArgumentNullException("filePath");
			if(filePath.Length == 0)
				throw new ArgumentOutOfRangeException("'filePath' is zero-length.");
		
			// load...
			XmlDocument document = new XmlDocument();
			document.Load(filePath);

			// defer...
			return FromXml(document);
		}

		/// <summary>
		/// Loads the project from XML.
		/// </summary>
		/// <param name="filePath"></param>
		/// <returns></returns>
		public static SqlSchema FromXml(XmlElement element)
		{
			if(element == null)
				throw new ArgumentNullException("element");

			// create a new one...
			SqlSchema newSchema = new SqlSchema();
			newSchema.MergeInternal(element, true);

			// return...
			return newSchema;
		}

		/// <summary>
		/// Loads the project from XML.
		/// </summary>
		/// <param name="filePath"></param>
		/// <returns></returns>
		public static SqlSchema FromXml(XmlDocument document)
		{
			if(document == null)
				throw new ArgumentNullException("document");

			// create a new one...
			SqlSchema newSchema = new SqlSchema();
			newSchema.MergeInternal(document, true);

			// return...
			return newSchema;
		}

		/// <summary>
		/// Gets the Procedures included in generation.
		/// </summary>
		/// <returns></returns>
		public SqlProcedure[] GetProceduresForGeneration()
		{
			ArrayList list = new ArrayList();
			foreach(SqlProcedure proc in this.Procedures)
			{
				if(proc.Generate)
					list.Add(proc);
			}

			// return...
			return (SqlProcedure[])list.ToArray(typeof(SqlProcedure));
		}

		/// <summary>
		/// Gets the tables included in generation.
		/// </summary>
		/// <returns></returns>
		public SqlTable[] GetTablesForGeneration()
		{
			ArrayList list = new ArrayList();
			foreach(SqlTable table in this.Tables)
			{
				if(table.Generate)
					list.Add(table);
			}

			// return...
			return (SqlTable[])list.ToArray(typeof(SqlTable));
		}

		///// <summary>
		///// Gets the work units that describe the differences between two schemas.
		///// </summary>
		///// <param name="existingSchema"></param>
		///// <returns></returns>
		//public WorkUnitCollection GetSchemaWorkUnits(SqlSchema existingSchema)
		//{
		//	return this.GetSchemaWorkUnits(existingSchema, null);
		//}

		/// <summary>
		/// Gets the work units that describe the differences between two schemas.
		/// </summary>
		/// <param name="existingSchema"></param>
		/// <returns></returns>
		public WorkUnitCollection GetSchemaWorkUnits(SqlSchema existingSchema, IOperationItem operation = null)
		{
			if(existingSchema == null)
				throw new ArgumentNullException("existingSchema");		

			// item...
			if(operation == null)
				operation = new OperationItem();

			// results...
			WorkUnitCollection results = new WorkUnitCollection();

			// find missing tables...
			operation.ProgressMaximum = this.Tables.Count;
			operation.ProgressValue = 0;
			foreach(SqlTable table in this.Tables)
			{
				// walk...
				SqlTable matchingTable = null;
				foreach(SqlTable existingTable in existingSchema.Tables)
				{
					if(string.Compare(table.NativeName, existingTable.NativeName, true, System.Globalization.CultureInfo.InvariantCulture) == 0)
					{
						matchingTable = existingTable;
						break;
					}
				}

				// found?
				if(matchingTable == null)
					results.AddRange(table.GetCreateTableWorkUnit());
				else
					results.AddRange(table.GetSchemaWorkUnits(matchingTable));

				// next...
				operation.IncrementProgress();
			}

			// mbr - 14-12-2005 - sort the units...
			results.Sort(new SchemaWorkUnitTypeComparer());

			// return...
			return results;
		}

		/// <summary>
		/// Gets a collection of SqlProcedure objects.
		/// </summary>
		public SqlProcedureCollection Procedures
		{
			get
			{
				return _procedures;
			}
		}
	}
}
