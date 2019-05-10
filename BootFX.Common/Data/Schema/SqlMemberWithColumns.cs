// BootFX - Application framework for .NET applications
// 
// File: SqlMemberWithColumns.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;

namespace BootFX.Common.Data.Schema
{
	/// <summary>
	/// Defines a <see cref="SqlMember"></see> instance that has columns.
	/// </summary>
	public abstract class SqlMemberWithColumns : SqlMember
	{
		/// <summary>
		/// Private field to support <c>Columns</c> property.
		/// </summary>
		private SqlColumnCollection _columns;
		
		/// <summary>
		/// Constructor.
		/// </summary>
		public SqlMemberWithColumns(string nativeName) : base(nativeName)
		{
			this.Initialize();
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		public SqlMemberWithColumns(string nativeName, string name) : base(nativeName, name)
		{
			this.Initialize();
		}

		/// <summary>
		/// Initializes the object.
		/// </summary>
		private void Initialize()
		{
			_columns = new SqlColumnCollection(this);
		}

		/// <summary>
		/// Gets a collection of SqlColumn objects.
		/// </summary>
		public SqlColumnCollection Columns
		{
			get
			{
				return _columns;
			}
		}

		protected override void WriteXml(System.Xml.XmlWriter xml, BootFX.Common.Xml.WriteXmlContext context)
		{
			// base...
			base.WriteXml(xml, context);

			// add...
			xml.WriteStartElement("Columns");
			foreach(SqlColumn column in this.Columns)
				column.ToXml(xml, context.Encoding);
			xml.WriteEndElement();
		}

		internal override void Fixup()
		{
			base.Fixup ();

			// mangle...
			SqlSchema.MangleDuplicateNames(this.Columns.ToArray());
		}
	}
}
