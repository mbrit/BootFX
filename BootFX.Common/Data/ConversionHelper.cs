// BootFX - Application framework for .NET applications
// 
// File: ConversionHelper.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Text;
using System.Linq;
using System.Data;
using BootFX.Common.Entities;
using BootFX.Common.Management;

namespace BootFX.Common.Data
{
	/// <summary>
	/// Provides access to conversion helpers.
	/// </summary>
	public static class ConversionHelper
	{
        private static BfxLookup<string, object> EnumValuesLookup { get; set; }

        static ConversionHelper()
        {
            EnumValuesLookup = new BfxLookup<string, object>();
            EnumValuesLookup.CreateItemValue += EnumValuesLookup_CreateItemValue;
        }

		/// <summary>
		/// Returns true if the given DB type has a fixed size.
		/// </summary>
		/// <param name="type"></param>
		public static bool DoesDBTypeHaveFixedSize(DbType type)
		{
			switch(type)
			{
				case DbType.Boolean:
				case DbType.Byte:
				case DbType.Currency:
				case DbType.Date:
				case DbType.DateTime:
				case DbType.Decimal:
				case DbType.Double:
				case DbType.Guid:
				case DbType.Int16:
				case DbType.Int32:
				case DbType.Int64:
				case DbType.SByte:
				case DbType.Single:
				case DbType.Time:
				case DbType.UInt16:
				case DbType.UInt32:
				case DbType.UInt64:
					return true;

				case DbType.AnsiString:
				case DbType.AnsiStringFixedLength:
				case DbType.Binary:
				case DbType.Object:
				case DbType.String:
				case DbType.StringFixedLength:
				case DbType.VarNumeric:
					return false;

				default:
					throw ExceptionHelper.CreateCannotHandleException(type);
			}
		}

		/// <summary>
		/// Returns true if the given database type supports large (i.e. BLOB) values.
		/// </summary>
		/// <param name="type"></param>
		public static bool DoesDBTypeSupportLarge(DbType type)
		{
			switch(type)
			{
				case DbType.AnsiString:
				case DbType.String:
				case DbType.Binary:
				case DbType.Object:
					return true;

				case DbType.Boolean:
				case DbType.Byte:
				case DbType.Currency:
				case DbType.Date:
				case DbType.DateTime:
				case DbType.Decimal:
				case DbType.Double:
				case DbType.Guid:
				case DbType.Int16:
				case DbType.Int32:
				case DbType.Int64:
				case DbType.SByte:
				case DbType.Single:
				case DbType.Time:
				case DbType.UInt16:
				case DbType.UInt32:
				case DbType.UInt64:
				case DbType.AnsiStringFixedLength:
				case DbType.StringFixedLength:
				case DbType.VarNumeric:
					return false;

				default:
					throw ExceptionHelper.CreateCannotHandleException(type);
			}
		}

		/// <summary>
		/// Gets the default length for the given DB type.
		/// </summary>
		/// <param name="type"></param>
		/// <returns>The length of the DB type as it is stored in *memory*, not in the underlying store.  For example, if the underlying store
		/// can store a "date-only" value in less space than it takes to store a "date/time" value, this will not be reflected in the result of this value.
		/// As to store a "date-only", "time-only" or "date/time" value requires a <see cref="DateTime"></see> instance, the value "8" will
		/// be returned by this method.</returns>
		public static long GetDefaultDBTypeSize(DbType type)
		{
			switch(type)
			{
				case DbType.Boolean:
				case DbType.Byte:
				case DbType.SByte:
					return 1;

				case DbType.Int16:
				case DbType.UInt16:
					return 2;

				case DbType.Int32:
				case DbType.UInt32:
				case DbType.Single:
					return 4;

				case DbType.Int64:
				case DbType.UInt64:
				case DbType.Double:
				case DbType.Date:
				case DbType.Time:
				case DbType.DateTime:
				case DbType.Currency:
					return 8;

				case DbType.Decimal:
				case DbType.Guid:
					return 16;

				case DbType.AnsiString:
				case DbType.String:
				case DbType.AnsiStringFixedLength:
				case DbType.StringFixedLength:
				case DbType.Object:
				case DbType.Binary:
				case DbType.VarNumeric:
					throw new NotSupportedException(string.Format(Cultures.Exceptions, "'{0}' does not have a default size.", type));

				default:
					throw ExceptionHelper.CreateCannotHandleException(type);
			}
		}

		/// <summary>
		/// Gets the <c>DbType</c> for the given CLR type.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static Type GetClrTypeForDBType(DbType type)
		{
			switch(type)
			{
				case DbType.Object:
					return typeof(object);

				case DbType.Byte:
					return typeof(byte);
				case DbType.SByte:
					return typeof(sbyte);

				case DbType.Boolean:
					return typeof(bool);

				case DbType.Int16:
					return typeof(short);
				case DbType.Int32:
					return typeof(int);
				case DbType.Int64:
					return typeof(long);

				case DbType.UInt16:
					return typeof(ushort);
				case DbType.UInt32:
					return typeof(uint);
				case DbType.UInt64:
					return typeof(ulong);

				case DbType.Single:
					return typeof(float);
				case DbType.Double:
					return typeof(double);

				case DbType.Decimal:
				case DbType.Currency:
				case DbType.VarNumeric:
					return typeof(decimal);

				case DbType.AnsiString:
				case DbType.String:
				case DbType.AnsiStringFixedLength:
				case DbType.StringFixedLength:
					return typeof(string);

				case DbType.Date:
				case DbType.Time:
				case DbType.DateTime:
					return typeof(DateTime);

				case DbType.Binary:
					return typeof(byte[]);

				case DbType.Guid:
					return typeof(Guid);

				default:
					throw ExceptionHelper.CreateCannotHandleException(type);
			}
		}

        public static DbType GetDBTypeForClrType<T>()
        {
            return GetDBTypeForClrType(typeof(T));
        }

        /// <summary>
        /// Gets the <c>DbType</c> for the given CLR type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static DbType GetDBTypeForClrType(Type type)
		{
			if(type == null)
				throw new ArgumentNullException("type");
			
			// return...
			// mbr - 27-10-2005 - handle byte array...
			TypeCode code = Type.GetTypeCode(type);
			if(code != TypeCode.Object)
				return GetDBTypeForClrType(code);
			else
			{
				if(typeof(byte[]).IsAssignableFrom(type))
					return DbType.Binary;
				else
					return GetDBTypeForClrType(code);
			}
		}

		/// <summary>
		/// Gets the <c>DbType</c> for the given CLR type.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		private static DbType GetDBTypeForClrType(TypeCode typeCode)
		{
			switch(typeCode)
			{
				case TypeCode.Boolean:
					return DbType.Boolean;

				case TypeCode.Byte:
					return DbType.Byte;
				case TypeCode.SByte:
					return DbType.SByte;

				case TypeCode.Char:
					return DbType.StringFixedLength;

				case TypeCode.DateTime:
					return DbType.DateTime;

				case TypeCode.Int16:
					return DbType.Int16;
				case TypeCode.Int32:
					return DbType.Int32;
				case TypeCode.Int64:
					return DbType.Int64;

				case TypeCode.UInt16:
					return DbType.UInt16;
				case TypeCode.UInt32:
					return DbType.UInt32;
				case TypeCode.UInt64:
					return DbType.UInt64;

				case TypeCode.Decimal:
					return DbType.Decimal;
				case TypeCode.Single:
					return DbType.Single;
				case TypeCode.Double:
					return DbType.Double;

				case TypeCode.String:
					return DbType.String;

				case TypeCode.Object:
					return DbType.Object;

				case TypeCode.DBNull:
					throw new InvalidOperationException("No defined result for DB null.");

				default:
					throw ExceptionHelper.CreateCannotHandleException(typeCode);
			}
		}

		/// <summary>
		/// Converts the given value to a long
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		/// <remarks>This method uses <see cref="ConversionFlags.Safe"></see> to ensure a safe conversion from DB and CLR null values.</remarks>
		public static long ToInt64(object value, IFormatProvider formatProvider = null)
		{
			return (long)ChangeType(value, TypeCode.Int64, formatProvider, ConversionFlags.Safe);
		}

		/// <summary>
		/// Converts the given value to an int.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		/// <remarks>This method uses <see cref="ConversionFlags.Safe"></see> to ensure a safe conversion from DB and CLR null values.</remarks>
		public static int ToInt32(object value, IFormatProvider formatProvider = null)
		{
			return (int)ChangeType(value, TypeCode.Int32, formatProvider, ConversionFlags.Safe);
		}

		/// <summary>
		/// Converts the given value to a short.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		/// <remarks>This method uses <see cref="ConversionFlags.Safe"></see> to ensure a safe conversion from DB and CLR null values.</remarks>
		public static short ToInt16(object value, IFormatProvider formatProvider = null)
		{
			return (short)ChangeType(value, TypeCode.Int16, formatProvider, ConversionFlags.Safe);
		}

		/// <summary>
		/// Converts the given value to an int.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		/// <remarks>This method uses <see cref="ConversionFlags.Safe"></see> to ensure a safe conversion from DB and CLR null values.</remarks>
		public static decimal ToDecimal(object value, IFormatProvider formatProvider = null)
		{
			return (decimal)ChangeType(value, TypeCode.Decimal, formatProvider, ConversionFlags.Safe);
		}

		/// <summary>
		/// Converts the given value to an int.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		/// <remarks>This method uses <see cref="ConversionFlags.Safe"></see> to ensure a safe conversion from DB null values.  CLR values will be passed through, i.e.
		/// they will not be converted to <see cref="string.Empty"></see>.</remarks>
		public static string ToString(object value, IFormatProvider formatProvider = null)
		{
			return (string)ChangeType(value, TypeCode.String, formatProvider, ConversionFlags.Safe);
		}

		/// <summary>
		/// Converts the given value to a boolean.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		/// <remarks>This method uses <see cref="ConversionFlags.Safe"></see> to ensure a safe conversion from DB and CLR null values.</remarks>
		public static bool ToBoolean(object value, IFormatProvider formatProvider = null)
		{
			return (bool)ChangeType(value, TypeCode.Boolean, formatProvider, ConversionFlags.Safe);
		}

		/// <summary>
		/// Converts the given value to a byte.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		/// <remarks>This method uses <see cref="ConversionFlags.Safe"></see> to ensure a safe conversion from DB and CLR null values.</remarks>
		public static byte ToByte(object value, IFormatProvider formatProvider = null)
		{
			return (byte)ChangeType(value, TypeCode.Byte, formatProvider, ConversionFlags.Safe);
		}

		/// <summary>
		/// Converts the given value to a char.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		/// <remarks>This method uses <see cref="ConversionFlags.Safe"></see> to ensure a safe conversion from DB and CLR null values.</remarks>
		public static char ToChar(object value, IFormatProvider formatProvider = null)
		{
			return (char)ChangeType(value, TypeCode.Char, formatProvider, ConversionFlags.Safe);
		}

		/// <summary>
		/// Converts the given value to a char.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		/// <remarks>This method uses <see cref="ConversionFlags.Safe"></see> to ensure a safe conversion from DB and CLR null values.</remarks>
		public static DateTime ToDateTime(object value, IFormatProvider formatProvider = null)
		{
			return (DateTime)ChangeType(value, TypeCode.DateTime, formatProvider, ConversionFlags.Safe);
		}

		/// <summary>
		/// Converts the given value to a char.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		/// <remarks>This method uses <see cref="ConversionFlags.Safe"></see> to ensure a safe conversion from DB and CLR null values.</remarks>
		public static double ToDouble(object value, IFormatProvider formatProvider = null)
		{
			return (double)ChangeType(value, TypeCode.Double, formatProvider, ConversionFlags.Safe);
		}

		/// <summary>
		/// Converts the given value to a char.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		/// <remarks>This method uses <see cref="ConversionFlags.Safe"></see> to ensure a safe conversion from DB and CLR null values.</remarks>
		public static float ToSingle(object value, IFormatProvider formatProvider = null)
		{
			return (float)ChangeType(value, TypeCode.Single, formatProvider, ConversionFlags.Safe);
		}

		/// <summary>
		/// Converts the given value to the given type.
		/// </summary>
		/// <param name="value"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		public static object ChangeType(object value, Type type, IFormatProvider formatProvider = null)
		{
			if(type == null)
				throw new ArgumentNullException("type");
			
			// defer...
			return ChangeType(value, type, formatProvider, ConversionFlags.Safe);
		}

		/// <summary>
		/// Converts the given value to the given type.
		/// </summary>
		/// <param name="value"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		public static object ChangeType(object value, Type type, IFormatProvider formatProvider, ConversionFlags flags)
		{
			if(type == null)
				throw new ArgumentNullException("type");

            // mbr - 2014-11-30 - short-circuit?
            if (value != null && value.GetType() == type)
                return value;

			// prep...
			value = PrepareValueForConversion(value, Type.GetTypeCode(type), flags);
			if(value == null)
			{
				// if we got null back from 'prepare', then we must be able to handle nulls in this type...
				return value;
			} 

			try
			{
				// Check to see if we are a guid, if we are we may have come directly from
				// a byte array so we will not have the { brackets } around the string
				if(type.Equals(typeof(Guid)) && value is string)
				{
					return new Guid(value as string);
				}

				// return...
				if(typeof(string).IsAssignableFrom(type) == true && value is IFormattableToString)
					return ((IFormattableToString)value).ToString(formatProvider);
				else
					return Convert.ChangeType(value, type, formatProvider);
			}
			catch(Exception ex)
			{
                throw new InvalidOperationException(string.Format("Failed to convert value '{0}' to '{1}'.", value, type), ex);
			}
		}

		/// <summary>
		/// Converts the given value to the given type.
		/// </summary>
		/// <param name="value"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		private static object ChangeType(object value, TypeCode typeCode, IFormatProvider formatProvider = null)
		{
			return ChangeType(value, typeCode, formatProvider, ConversionFlags.Safe);
		}

		/// <summary>
		/// Prepares a value before it is converted.
		/// </summary>
		/// <param name="value"></param>
		/// <param name="flags"></param>
		/// <returns></returns>
		private static object PrepareValueForConversion(object value, TypeCode typeCode, ConversionFlags flags)
		{
			// do we have dbnull?
			if(value is DBNull)
			{
				if((flags & ConversionFlags.DBNullToClrNull) != 0)
					value = null;
			}

			// do we have null?
			if(value == null)
			{
				if((flags & ConversionFlags.ClrNullToLegal) != 0)
					return GetClrNullLegalEquivalent(typeCode);
			}

			// mbr - 11-03-2007 - guid?
			if(value is Guid && typeCode == TypeCode.String)
				return value.ToString();

			// return...
			return value;
		}

		/// <summary>
		/// Converts the given value to the given type.
		/// </summary>
		/// <param name="value"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		private static object ChangeType(object value, TypeCode typeCode, IFormatProvider formatProvider, ConversionFlags flags)
		{
            if (formatProvider == null)
                formatProvider = Cultures.Default;

			// prep...
			value = PrepareValueForConversion(value, typeCode, flags);

			// do a conversion...
			if(value == null)
				value = GetClrNullLegalEquivalent(typeCode);
			else
			{
				try
				{
					value = Convert.ChangeType(value, typeCode, formatProvider);
				}
				catch(Exception ex)
				{
					throw new InvalidOperationException(string.Format(Cultures.Exceptions, "Failed to convert '{0}' ({1}) to '{2}'.  (Format provider: {3}, flags: {4}).",
						value, value.GetType(), typeCode, formatProvider, flags), ex);
				}
			}

			// return....
			return value;
		}

		/// <summary>
		/// Suggests the CLR equivalent of the given type.
		/// </summary>
		/// <param name="dbType"></param>
		/// <returns></returns>
		public static object GetClrLegalDBNullEquivalent(DbType dbType)
		{
			switch(dbType)
			{
				case DbType.Object:
				case DbType.Guid:
					return null;

				case DbType.Boolean:
					return false;

				case DbType.Byte:
					return byte.MinValue;
				case DbType.SByte:
					return (sbyte)0;

				case DbType.Int16:
					return (short)0;
				case DbType.Int32:
					return (int)0;
				case DbType.Int64:
					return (long)0;

				case DbType.Single:
					return (float)0;
				case DbType.Double:
					return (double)0;

				case DbType.UInt16:
					return ushort.MinValue;
				case DbType.UInt32:
					return uint.MinValue;
				case DbType.UInt64:
					return ulong.MinValue;

				case DbType.Currency:
				case DbType.VarNumeric:
				case DbType.Decimal:
					return (decimal)0;

				case DbType.String:
				case DbType.StringFixedLength:
				case DbType.AnsiString:
				case DbType.AnsiStringFixedLength:
				case DbType.Binary:
					return null;

				case DbType.Date:
				case DbType.Time:
				case DbType.DateTime:
					return DateTime.MinValue;

				default:
					throw ExceptionHelper.CreateCannotHandleException(dbType);
			}
		}

		/// <summary>
		/// Gets the legal equivalent for the given <see cref="DbType"></see>.
		/// </summary>
		/// <param name="typeCode"></param>
		/// <returns></returns>
		public static object GetClrNullLegalEquivalent(DbType dbType)
		{
			switch(dbType)
			{
				case DbType.String:
				case DbType.StringFixedLength:
				case DbType.AnsiString:
				case DbType.AnsiStringFixedLength:
					return null;

				case DbType.Int16:
					return GetClrNullLegalEquivalent(TypeCode.Int16);
				case DbType.Int32:
					return GetClrNullLegalEquivalent(TypeCode.Int32);
				case DbType.Int64:
					return GetClrNullLegalEquivalent(TypeCode.Int64);

				default:
					throw new NotSupportedException(string.Format("Cannot handle '{0}' ({1}).", dbType, dbType.GetType()));
			}
		}

		/// <summary>
		/// Gets the legal equivalent for the given CLR type.
		/// </summary>
		/// <param name="typeCode"></param>
		/// <returns></returns>
		public static object GetClrNullLegalEquivalent(Type type)
		{
			if(type == null)
				throw new ArgumentNullException("type");
			
			// get...
			TypeCode typeCode = Type.GetTypeCode(type);
			if(typeCode == TypeCode.Object)
				return null;
			else
				return GetClrNullLegalEquivalent(typeCode);
		}

		/// <summary>
		/// Gets the legal equivalent for the given CLR type.
		/// </summary>
		/// <param name="typeCode"></param>
		/// <returns></returns>
		private static object GetClrNullLegalEquivalent(TypeCode typeCode)
		{
			switch(typeCode)
			{
				case TypeCode.Byte:
					return (byte)0;
				case TypeCode.SByte:
					return (SByte)0;
				case TypeCode.Int16:
					return (short)0;
				case TypeCode.Int32:
					return (int)0;
				case TypeCode.Int64:
					return (long)0;
				case TypeCode.UInt16:
					return (UInt16)0;
				case TypeCode.UInt32:
					return (UInt32)0;
				case TypeCode.UInt64:
					return (UInt64)0;

				case TypeCode.Char:
					return char.MinValue;
				case TypeCode.String:
					return null;

				case TypeCode.Decimal:
					return (decimal)0;
				case TypeCode.Single:
					return (float)0;
				case TypeCode.Double:
					return (double)0;

				case TypeCode.Object:
					return null;

				case TypeCode.DBNull:
					return DBNull.Value;

				case TypeCode.DateTime:
					return DateTime.MinValue;

				case TypeCode.Boolean:
					return false;

				default:
					throw ExceptionHelper.CreateCannotHandleException(typeCode);
			}
		}

        public static bool IsStringType(this DbType dbType)
        {
            return dbType == DbType.String || dbType == DbType.AnsiString || dbType == DbType.StringFixedLength ||
                    dbType == DbType.AnsiStringFixedLength;
        }

        /// <summary>
        /// Converts the given value to a base 64 string.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToBase64String(object value, Encoding encoding)
		{
			// if null or dbnull, this should be handled before we get here...
			if(value == null)
				throw new ArgumentNullException("value");
			if(value is DBNull)
				throw new ArgumentException("'value' cannot be DB null.", "value");
			if(encoding == null)
				throw new ArgumentNullException("encoding");

			// convert...
			if(value is string)
				return Convert.ToBase64String(encoding.GetBytes((string)value));
			else
				throw new InvalidOperationException(string.Format(Cultures.Exceptions, "Cannot handle '{0}'.", value.GetType()));
		}

		/// <summary>
		/// Formats a value so that it can be used in a SQL string.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		/// <remarks>This method should virtually never be used.  It is used internally by <see cref="SqlFilter"></see> to create a
		/// filter expression for use with <see cref="DataTable.Select"></see>.</remarks>
		internal static string FormatValueForSql(object value, DbType type)
		{
			if(value == null)
			{
				// get...
				object legalNull = GetClrNullLegalEquivalent(type);
				return Convert.ToString(legalNull);
			}
			else
			{
				if(value is string)
					return "\'" + ((string)value).Replace("\'", "\'\'") + "\'";
				if(value is DBNull)
					return "null";
				if(value is int || value is short || value is long)
					return value.ToString();
				else
					throw new NotSupportedException(string.Format("Cannot handle '{0}' ({1}, DB type: {2}).", value, value.GetType(), type));
			}
		}

		// mbr - 27-04-2006 - added.
		public static Guid ToGuid(string value)
		{
			if(value == null)
				throw new ArgumentNullException("value");
			if(value.Length == 0)
				throw new ArgumentOutOfRangeException("'value' is zero-length.");
			
			try
			{
				return new Guid(value);
			}
			catch(Exception ex)
			{
				throw new InvalidOperationException(string.Format("Failed to convert '{0}' to a GUID.", value), ex);
			}
		}

		internal static bool DoesDbTypeNeedRounding(DbType type)
		{
			switch(type)
			{
				case DbType.Decimal:
				case DbType.Double:
				case DbType.Single:
				case DbType.VarNumeric:
					return true;

				default:
					return false;
			}
		}

        public static int ToInt32(this string value, IFormatProvider format = null)
        {
            return ToInt32((object)value, format ?? Cultures.System);
        }

        public static long ToInt64(this string value, IFormatProvider format = null)
        {
            return ToInt64((object)value, format ?? Cultures.System);
        }

        public static T ChangeType<T>(this object value, IFormatProvider format = null)
        {
            return (T)ChangeType(value, typeof(T), format ?? Cultures.System);
        }

        public static object ConvertToNativeEnumValue(object value, Type enumType)
        {
            if(value == null)
                return Enum.GetValues(enumType).GetValue(0);
            else if(value is string)
                return Enum.Parse(enumType, (string)value, true);
            else if(value is int || value is long || value is string || value is short)
            {
                var key = string.Format("{0}|{1}", enumType.GetPartiallyQualifiedName(), value);
                return EnumValuesLookup[key];
            }
            else
                return Convert.ChangeType(value, enumType);
        }

        static void EnumValuesLookup_CreateItemValue(object sender, CreateLookupItemEventArgs<string, object> e)
        {
            var index = e.Key.IndexOf('|');
            var typeName = e.Key.Substring(0, index);
            var type = Type.GetType(typeName);
            var toFind = ConversionHelper.ToInt64(e.Key.Substring(index + 1));

            // get...
            var masters = Enum.GetValues(type);
            foreach (var master in masters)
            {
                if (ConversionHelper.ToInt64(master) == toFind)
                {
                    e.NewValue = master;
                    return;
                }
            }

            // return...
            e.NewValue = ConversionHelper.ChangeType(toFind, Enum.GetUnderlyingType(type));
        }
    }
}
