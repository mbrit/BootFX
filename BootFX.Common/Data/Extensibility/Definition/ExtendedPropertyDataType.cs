// BootFX - Application framework for .NET applications
// 
// File: ExtendedPropertyDataType.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Collections;

namespace BootFX.Common.Entities
{
	/// <summary>
	/// Defines an instance of <c>ExtendedPropertyType</c>.
	/// </summary>
	// mbr - 25-09-2007 - renamed.	
//	public abstract class ExtendedPropertyType
	public abstract class ExtendedPropertyDataType
	{
		protected ExtendedPropertyDataType()
		{
		}

		/// <summary>
		/// Gets the CLR type for this extended property type.
		/// </summary>
		public abstract Type Type
		{
			get;
		}


		/// <summary>
		/// Gets the ID.
		/// </summary>
		public string Id
		{
			get
			{
				if(Type == null)
					throw new InvalidOperationException("Type is null.");

				// return...
				Type thisType = this.GetType();
				return string.Format("{0}, {1}|{2}", thisType.FullName, thisType.Assembly.GetName().Name, 
					this.GetId());
			}
		}

		public abstract string Name
		{
			get;
		}

		public string NativeName
		{
			get
			{
				return this.Name;
			}
		}

		protected abstract string GetId();

		// mbr - 25-09-2007 - renamed.		
//		public static ExtendedPropertyType GetExtendedPropertyType(string id)
		public static ExtendedPropertyDataType GetExtendedPropertyDataType(string id)
		{
			if(id == null)
				throw new ArgumentNullException("id");
			if(id.Length == 0)
				throw new ArgumentOutOfRangeException("'id' is zero-length.");
			
			// split...
			int index = id.IndexOf("|");
			if(index == -1)
				throw new InvalidOperationException(string.Format("The ID '{0}' is invalid.", id));

			// get...
			string name = id.Substring(0, index);
			string specificId = id.Substring(index + 1);

			// get...
			Type type = Type.GetType(name, false, true);
			if(type == null)
				throw new InvalidOperationException(string.Format("A type with name '{0}' could not be found.", name));

			// create...
			try
			{
				return (ExtendedPropertyDataType)Activator.CreateInstance(type, new object[] { specificId });
			}
			catch(Exception ex)
			{
				throw new InvalidOperationException(string.Format("Failed to create an instance of '{0}' with ID '{1}'.", type, specificId), ex);
			}
		}

		/// <summary>
		/// Gets the EntityField for the Custom Property
		/// </summary>
		/// <returns></returns>
		internal abstract EntityField GetEntityField(ExtendedPropertyDefinition property);

		internal virtual long DefaultSize
		{
			get
			{			
				return 0;
			}
		}

		public virtual string DisplayName
		{
			get
			{
				return this.Name;
			}
		}

		public virtual bool HasSize
		{
			get
			{
				return false;
			}
		}

		public virtual bool SupportsMultiValue
		{		
			get
			{
				return false;
			}
		}
	}
}
