// BootFX - Application framework for .NET applications
// 
// File: EnumHelper.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Text;
using System.Reflection;
using System.Collections.Generic;
using System.ComponentModel;

namespace BootFX.Common
{
	/// <summary>
	/// Summary description for EnumHelper.
	/// </summary>
	public class EnumHelper
	{
		public EnumHelper()
		{
		}

        public static string GetDescription(Type enumType, object value)
        {
            bool usedAttribute = false;
            return GetDescription(enumType, value, ref usedAttribute);
        }

		public static string GetDescription(Type enumType, object value, ref bool usedAttribute)
		{
            usedAttribute = false;

			String itemName = Enum.GetName(enumType, value);
			try
			{
				FieldInfo fi = enumType.GetField(itemName);

				Object[] attribs = fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
				if (attribs.Length > 0)
				{
                    usedAttribute = true;
					return ((DescriptionAttribute)attribs[0]).Description;
				}
				else
					return itemName;
			}
			catch
			{
				return itemName;
			}
		}

        public static string[] GetDescriptions(Type enumType)
        {
            string[] items = Enum.GetNames(enumType);

            for (int i = 0; i < items.Length; i++)
            {
                items[i] = GetDescription(enumType, Enum.Parse(enumType, items[i]));
            }

            return items;
        }

        /// <summary>
        /// Gets the descripitions for a given type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Dictionary<T, string> GetDescriptions<T>()
        {
            Dictionary<T, string> results = new Dictionary<T, string>();
            foreach(T value in Enum.GetValues(typeof(T)))
                results[value] = GetDescription(typeof(T), value);

            // return...
            return results;
        }

        // mbr - 2010-02-09 - removed this, but added it back.  changed the name...
//        public static object Parse(Type enumType, string description)
        public static object ParseDescription(Type enumType, string description)
        {
            if (description == null)
                throw new ArgumentNullException("description");
            if (description.Length == 0)
                throw new ArgumentException("'description' is zero-length.");

            // get all names in enum
            foreach (string name in Enum.GetNames(enumType))
            {
                // get value
                object enumValue = Enum.Parse(enumType, name);

                // check description
                if (GetDescription(enumType, enumValue) == description)
                    return enumValue;
                    //check name
                else if (name == description)
                    return enumValue;
            }

            // throw
            throw new InvalidOperationException(string.Format("Could not find value for {0} from enum {1}", description, enumType.Name));
        }
	}
}
