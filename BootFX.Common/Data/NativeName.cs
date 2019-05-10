// BootFX - Application framework for .NET applications
// 
// File: NativeName.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using BootFX.Common.Data.Formatters;

namespace BootFX.Common.Data
{
	/// <summary>
	/// Defines a native name.
	/// </summary>
	public class NativeName
	{
		/// <summary>
		/// Private field to support <see cref="CatalogName"/> property.
		/// </summary>
		private string _catalogName;

		/// <summary>
		/// Private field to support <see cref="UserName"/> property.
		/// </summary>
		private string _userName;

		/// <summary>
		/// Private field to support <see cref="Name"/> property.
		/// </summary>
		private string _name;
		
		/// <summary>
		/// Constructor.
		/// </summary>
		public NativeName(string name)
		{
			if(name == null)
				throw new ArgumentNullException("name");
			if(name.Length == 0)
				throw new ArgumentOutOfRangeException("'name' is zero-length.");
			
			this.Initialize(null, null, name);
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		public NativeName(string userName, string name)
		{
			if(userName == null)
				throw new ArgumentNullException("userName");
			if(userName.Length == 0)
				throw new ArgumentOutOfRangeException("'userName' is zero-length.");
			if(name == null)
				throw new ArgumentNullException("name");
			if(name.Length == 0)
				throw new ArgumentOutOfRangeException("'name' is zero-length.");
			
			this.Initialize(null, userName, name);
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		public NativeName(string catalogName, string userName, string name)
		{
			if(catalogName == null)
				throw new ArgumentNullException("catalogName");
			if(catalogName.Length == 0)
				throw new ArgumentOutOfRangeException("'catalogName' is zero-length.");
			if(userName == null)
				throw new ArgumentNullException("userName");
			if(userName.Length == 0)
				throw new ArgumentOutOfRangeException("'userName' is zero-length.");
			if(name == null)
				throw new ArgumentNullException("name");
			if(name.Length == 0)
				throw new ArgumentOutOfRangeException("'name' is zero-length.");
			
			this.Initialize(catalogName, userName, name);
		}

		/// <summary>
		/// Initializes the object.
		/// </summary>
		/// <param name="catalogName"></param>
		/// <param name="userName"></param>
		/// <param name="name"></param>
		private void Initialize(string catalogName, string userName, string name)
		{
			if(name == null)
				throw new ArgumentNullException("name");
			if(name.Length == 0)
				throw new ArgumentOutOfRangeException("'name' is zero-length.");
			if(catalogName != null && catalogName.Length == 0)
				catalogName = null;
			if(userName != null && userName.Length == 0)
				userName = null;

			// set...
			_catalogName = catalogName;
			_userName = userName;
			_name = name;
		}

		/// <summary>
		/// Gets the name.
		/// </summary>
		public string Name
		{
			get
			{
				return _name;
			}
		}
		
		/// <summary>
		/// Gets the username.
		/// </summary>
		public string UserName
		{
			get
			{
				return _userName;
			}
		}
		
		/// <summary>
		/// Gets the catalogname.
		/// </summary>
		public string CatalogName
		{
			get
			{
				return _catalogName;
			}
		}

		/// <summary>
		/// Gets the string represetnation of the object.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			// defer...
			return this.ToString(null);
		}

		/// <summary>
		/// Gets the string represetnation of the object.
		/// </summary>
		/// <returns></returns>
		public string ToString(SqlDialect dialect)
		{
			// return...
			if(dialect != null)
				return dialect.FormatNativeName(this);

			// create...
			if(this.CatalogName == null && this.UserName == null)
				return this.Name;

			// do we have a catalogname?
			if(this.CatalogName != null)
			{
				if(this.UserName != null)
					return string.Format("{0}.{1}.{2}", this.CatalogName, this.UserName, this.Name);
				else
					return string.Format("{0}..{1}", this.CatalogName, this.Name);
			}

			// username?
			return string.Format("{0}.{1}", this.UserName, this.Name);
		}

		/// <summary>
		/// Gets a native name from the given string.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public static NativeName GetNativeName(string name)
		{
			if(name == null)
				throw new ArgumentNullException("name");
			if(name.Length == 0)
				throw new ArgumentOutOfRangeException("'name' is zero-length.");
			
			// split it up...
			// TODO: handle brackets in this name...
			string[] parts = name.Split('.');
			if(parts.Length == 1)
				return new NativeName(parts[0]);
			else if(parts.Length == 2)
				return new NativeName(parts[0], parts[1]);
			else if(parts.Length == 3)
				return new NativeName(parts[0], parts[1], parts[2]);
			else
				throw new InvalidOperationException(string.Format(Cultures.Exceptions, "The native name '{0}' does not appear to be valid.", name));
		}

		public bool HasCatalogName
		{
			get
			{
				if(this.CatalogName != null && this.CatalogName.Length > 0)
					return true;
				else
					return false;
			}
		}

		public bool HasUserName
		{
			get
			{
				if(this.UserName != null && this.UserName.Length > 0)
					return true;
				else
					return false;
			}
		}

	}
}
