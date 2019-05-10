// BootFX - Application framework for .NET applications
// 
// File: ClrExtendedPropertyDataType.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using BootFX.Common.Data;

namespace BootFX.Common.Entities
{
	/// <summary>
	/// Defines an instance of <c>ClrExtendedPropertyType</c>.
	/// </summary>
	// mbr - 25-09-2007 - renamed.	
//	public class ClrExtendedPropertyType : ExtendedPropertyType
	public class ClrExtendedPropertyDataType : ExtendedPropertyDataType
	{
		/// <summary>
		/// Private field to support <see cref="Type"/> property.
		/// </summary>
		private Type _type;
		
		public ClrExtendedPropertyDataType(Type type)
		{
			this.Initialize(type);
		}

		public ClrExtendedPropertyDataType(string name)
		{
			if(name == null)
				throw new ArgumentNullException("name");
			if(name.Length == 0)
				throw new ArgumentOutOfRangeException("'name' is zero-length.");
			
			// load...
			Type type = Type.GetType(name, false, true);
			if(type == null)
				throw new InvalidOperationException(string.Format("A type with name '{0}' could not be found.", name));

			// init...
			this.Initialize(type);
		}

		private void Initialize(Type type)
		{
			if(type == null)
				throw new ArgumentNullException("type");
			_type = type;
		}

		public override Type Type 
		{
			get
			{
				return _type;
			}
		}

		/// <summary>
		/// Gets the name.
		/// </summary>
		public override string Name
		{
			get
			{
				if(Type == null)
					throw new InvalidOperationException("Type is null.");

				// switch...
				switch(Type.GetTypeCode(this.Type))
				{
					case TypeCode.String:
						return "String";

					case TypeCode.Boolean:
						return "Boolean";

					case TypeCode.DateTime:
						return "Date/time";

					case TypeCode.Int32:
					case TypeCode.Int64:
						return "Integer";

					case TypeCode.Double:
						return "Double";

					case TypeCode.Decimal:
						return "Decimal";

					default:
						return Type.ToString();
				}
			}
		}
		
		protected override string GetId()
		{
			if(Type == null)
				throw new InvalidOperationException("Type is null.");
			return string.Format("{0}, {1}", this.Type.FullName, this.Type.Assembly.GetName().Name); 
		}

		internal override EntityField GetEntityField(ExtendedPropertyDefinition property)
		{
			if(property == null)
				throw new ArgumentNullException("property");
			
			// Now check we have a DBType for our type
			DbType dbType = ConversionHelper.GetDBTypeForClrType(this.Type);
			EntityField newField = null;
			if(dbType == DbType.String || dbType == DbType.StringFixedLength)
			{
				long size = property.Size;
				if(size <= 0)
				{
					// mbr - 25-09-2007 - this can only happen with legacy...
					if(Database.ExtensibilityProvider is FlatTableExtensibilityProvider)
						size = FlatTableExtensibilityProvider.MaxStringPropertyLength;
					else
					{
						// TODO: This method has not been implemented.  Check whether it is a valid not implementated, or a code path that was not completed.
						throw new NotImplementedException(string.Format("Not implemented for '{0}'.", this.GetType()));
					}
				}
				newField = new EntityField(property.Name, property.NativeName, dbType, EntityFieldFlags.Nullable, size);
			}
			else
				newField = new EntityField(property.Name, property.NativeName, dbType, EntityFieldFlags.Nullable);

			// set...
			newField.SetExtendedPropertyDefinition(property);

			// return...
			return newField;
		}

		internal override long DefaultSize
		{
			get
			{
				if(typeof(string).IsAssignableFrom(this.Type))
				{
					// mbr - 25-09-2007 - handle on flat...
					if(Database.ExtensibilityProvider is FlatTableExtensibilityProvider)
						return FlatTableExtensibilityProvider.MaxStringPropertyLength;
					else
					{
						// TODO: This method has not been implemented.  Check whether it is a valid not implementated, or a code path that was not completed.
						throw new NotImplementedException(string.Format("Not implemented for '{0}'.", this.GetType()));
					}
				}
				else
					return 0;
			}
		}

		public override bool HasSize
		{
			get
			{
				if(typeof(string).IsAssignableFrom(this.Type))
					return true;
				else
					return false;
			}
		}
	}
}
