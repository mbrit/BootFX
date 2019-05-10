// BootFX - Application framework for .NET applications
// 
// File: SqlMember.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Xml;
using System.Reflection;
using System.CodeDom;
using System.ComponentModel;
using BootFX.Common.Xml;

namespace BootFX.Common.Data.Schema
{
	/// <summary>
	/// Base class for a SQL member.
	/// </summary>
	public abstract class SqlMember : ToXmlBase
	{
		/// <summary>
		/// Private field to support <see cref="Schema"/> property.
		/// </summary>
		private SqlSchema _schema;
		
		/// <summary>
		/// Private field to support <c>Ordinal</c> property.
		/// </summary>
		private int _ordinal;
		
		/// <summary>
		/// Raised when the <c>Generate</c> property has changed.
		/// </summary>
		[Browsable(true), Category("Property Changed"), Description("Raised when the Generate property has changed.")]
		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// Raised when the <c>Name</c> property has changed.
		/// </summary>
		[Browsable(true), Category("Property Changed"), Description("Raised when the Name property has changed.")]
		public event EventHandler NameChanged;
		
		/// <summary>
		/// Private field to support <c>Generate</c> property.
		/// </summary>
		private bool _generate = true;
		
		/// <summary>
		/// Private field to support <see cref="Name"/> property.
		/// </summary>
		private string _name;

		/// <summary>
		/// Private field to support <see cref="NativeName"/> property.
		/// </summary>
		private string _nativeName;
		
		/// <summary>
		/// Constructor.
		/// </summary>
		protected SqlMember(string nativeName) : this(nativeName, CodeDomHelper.GetPascalName(nativeName))
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nativeName"></param>
		/// <param name="name"></param>
		protected SqlMember(string nativeName, string name)
		{
			if(nativeName == null)
				throw new ArgumentNullException("nativeName");
			if(nativeName.Length == 0)
				throw new ArgumentOutOfRangeException("'nativeName' is zero-length.");
			if(name == null)
				throw new ArgumentNullException("name");
			if(name.Length == 0)
				throw new ArgumentOutOfRangeException("'name' is zero-length.");
			
			_nativeName = nativeName;
			_name = name;
		}

		/// <summary>
		/// Gets the nativename.
		/// </summary>
		public string NativeName
		{
			get
			{
				return _nativeName;
			}
		}
		
		/// <summary>
		/// Gets the name.
		/// </summary>
		public string Name
		{
			get
			{
				if(_name == null)
					return this.NativeName;
				return _name;
			}
			set
			{
				if(value != null && value.Length == 0)
					value = null;
				if(_name != value)
				{
					_name = value;
					this.OnNameChanged();
				}
			}
		}

		protected override void WriteXml(System.Xml.XmlWriter xml, BootFX.Common.Xml.WriteXmlContext context)
		{
			xml.WriteElementString("NativeName", this.NativeName);
			xml.WriteElementString("Name", this.Name);
			xml.WriteElementString("Generate", this.Generate.ToString());
		}

		public override string ToString()
		{
			return this.Name;
		}

		/// <summary>
		/// Raises the <c>NameChanged</c> event.
		/// </summary>
		private void OnNameChanged()
		{
			OnNameChanged(EventArgs.Empty);
		}
		
		/// <summary>
		/// Raises the <c>NameChanged</c> event.
		/// </summary>
		protected virtual void OnNameChanged(EventArgs e)
		{
			if(NameChanged != null)
				NameChanged(this, e);
		}

		/// <summary>
		/// Gets or sets whether the member should be generated.
		/// </summary>
		public bool Generate
		{
			get
			{
				return _generate;
			}
			set
			{
				// check to see if the value has changed...
				if(value != _generate)
				{
					// set the value...
					_generate = value;
					this.OnPropertyChanged(new PropertyChangedEventArgs("Generate"));
				}
			}
		}

		/// <summary>
		/// Raises the <c>GenerateChanged</c> event.
		/// </summary>
		protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
		{
			if(PropertyChanged != null)
                PropertyChanged(this, e);
		}

		/// <summary>
		/// Fixes up the member.
		/// </summary>
		internal virtual void Fixup()
		{
		}

		/// <summary>
		/// Gets or sets the ordinal
		/// </summary>
		public int Ordinal
		{
			get
			{
				return _ordinal;
			}
			set
			{
				// check to see if the value has changed...
				if(value != _ordinal)
				{
					// set the value...
					_ordinal = value;
				}
			}
		}

		/// <summary>
		/// Merges the given values.
		/// </summary>
		/// <param name="element"></param>
		internal virtual void Merge(XmlElement element, bool createIfNotFound)
		{
			if(element == null)
				throw new ArgumentNullException("element");
			
			// get the user-defined name...
			string name = XmlHelper.GetElementString(element, "Name", OnNotFound.ReturnNull);
			if(name != null && name.Length > 0)
				this.Name = name;

			// generate?
			this.Generate = XmlHelper.GetElementBoolean(element, "Generate", OnNotFound.ReturnNull);
		}
		
		/// <summary>
		/// Gets the schema.
		/// </summary>
		internal SqlSchema Schema
		{
			get
			{
				return _schema;
			}
		}

		/// <summary>
		/// Sets the schema.
		/// </summary>
		/// <param name="schema"></param>
		internal void SetSchema(SqlSchema schema)
		{
			_schema = schema;
		}
	}
}
