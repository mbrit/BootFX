// BootFX - Application framework for .NET applications
// 
// File: EntityLookupField.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Data;
using BootFX.Common.Data;

namespace BootFX.Common.Entities
{
	/// <summary>
	/// Summary description for EntityLookupField.
	/// </summary>
	public class EntityLookupField : EntityField
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="name"></param>
		/// <param name="prop"></param>
		public EntityLookupField(string name, string nativeName, ExtendedPropertyDefinition prop) 
			: base(name,nativeName,DbType.Int32,EntityFieldFlags.Nullable)
		{
			if(prop == null)
				throw new ArgumentNullException("prop");
			this.SetExtendedPropertyDefinition(prop);
		}

		/// <summary>
		/// Gets the lookup name of this lookup field
		/// </summary>
		public string LookupName
		{
			get
			{
				if(ExtendedPropertyDefinition == null)
					throw new InvalidOperationException("ExtendedPropertyDefinition is null.");
				if(ExtendedPropertyDefinition.DataType == null)
					throw new InvalidOperationException("ExtendedPropertyDefinition.DataType is null.");
				
				// return...
				if(this.ExtendedPropertyDefinition.DataType is LookupExtendedPropertyDataType)
					return ((LookupExtendedPropertyDataType)this.ExtendedPropertyDefinition.DataType).LookupName;
				else
					throw new NotSupportedException(string.Format("Cannot handle '{0}'.", this.ExtendedPropertyDefinition.DataType));
			}
		}

		/// <summary>
		/// Gets the lookup values for the specified field
		/// </summary>
		public LookupValueCollection LookupValues
		{
			get
			{
				return LookupValue.GetByName(this.Name);
			}
		}

		/// <summary>
		/// Sets the fields flag to be true or false
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="flag"></param>
		/// <param name="setFlag"></param>
		public void SetValue(object entity, int flag, bool setFlag)
		{
			if(GetValue(entity,flag) == setFlag)
				return;

			int currentValue = (int) GetValue(entity);
			if(setFlag)
				currentValue += flag;
			else
				currentValue -= flag;

			Entity setEntity = (Entity) entity;
			setEntity.SetValue(this, currentValue, SetValueReason.UserSet );
		}

		/// <summary>
		/// Gets the fields flag to be true or false
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="flag"></param>
		public bool GetValue(object entity, int flag)
		{
			int lookupValue =(int) GetValue(entity);
			return (lookupValue & flag) != 0;		
		}
	}
}
