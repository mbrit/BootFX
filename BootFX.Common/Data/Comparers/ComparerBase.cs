// BootFX - Application framework for .NET applications
// 
// File: ComparerBase.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Data;
using System.Collections;
using System.Globalization;
using System.ComponentModel;

namespace BootFX.Common.Data.Comparers
{
	/// <summary>
	/// Defines the base class for comparers.
	/// </summary>
	/// <remarks>This class provides access to a culture through the <c>Culture</c></remarks>
	public abstract class ComparerBase : IComparer//, IComparerDirection
	{
		/// <summary>
		/// Private field to support <c>Culture</c> property.
		/// </summary>
		private CultureInfo _culture = Cultures.System;
		
		/// <summary>
		/// Private field to support <c>DefaultComparer</c> property.
		/// </summary>
		private static IComparer _defaultComparer = new StringComparer(Cultures.System);
		
        ///// <summary>
        ///// Private field to support <c>Direction</c> property.
        ///// </summary>
        //private SortDirection _direction;

		/// <summary>
		/// Constructor.
		/// </summary>
		protected ComparerBase(CultureInfo culture) 
		{
			this.Culture = culture;
		}

        // mbr - 2010-04-05 - deprecated direction...
        ///// <summary>
        ///// Constructor.
        ///// </summary>
        //protected ComparerBase(CultureInfo culture, SortDirection direction) : this(culture)
        //{
        //    this.Direction = direction;
        //}

        // mbr - 2010-04-05 - deprecated...
        ///// <summary>
        ///// Gets the direction.
        ///// </summary>
        //public SortDirection Direction
        //{
        //    get
        //    {
        //        return _direction;
        //    }
        //    set
        //    {
        //        _direction = value;
        //    }
        //}

		/// <summary>
		/// Compares two items.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public int Compare(object x, object y)
		{
			// remove dbnull and replace it with CLR null...
			if(x is DBNull)
				x = null;
			if(y is DBNull)
				y = null;

			// run...
			int result = this.DoCompare(x, y);
			return result;
		}

		/// <summary>
		/// Does the actual comparison.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		protected abstract int DoCompare(object x, object y);

		/// <summary>
		/// Gets the defaultcomparer.
		/// </summary>
		public static IComparer DefaultComparer
		{
			get
			{
				return _defaultComparer;
			}
		}

		/// <summary>
		/// Gets the comparer for the given type.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static IComparer GetComparer(Type type, CultureInfo culture)
		{
			if(type == null)
				throw new ArgumentNullException("type");

			// type code...
			TypeCode typeCode = Type.GetTypeCode(type);
			if(typeCode != TypeCode.Object)
				return GetComparer(typeCode, culture);
			else
			{
				if(typeof(byte[]).IsAssignableFrom(type) == true)
					return new BinaryComparer(culture);
				else
					return DefaultComparer;
			}
		}

		/// <summary>
		/// Gets the comparer for the given type code.
		/// </summary>
		/// <param name="typeCode"></param>
		/// <returns></returns>
		public static IComparer GetComparer(TypeCode typeCode, CultureInfo culture)
		{
			switch(typeCode)
			{
				case TypeCode.DateTime:
					return new DateTimeComparer(culture);

				case TypeCode.Single:
				case TypeCode.Double:
				case TypeCode.Decimal:
					return new DecimalComparer(culture);

				case TypeCode.Boolean:
				case TypeCode.Byte:
				case TypeCode.SByte:
				case TypeCode.Int16:
				case TypeCode.Int32:
				case TypeCode.Int64:
					return new IntegerComparer(culture);

				case TypeCode.UInt16:
				case TypeCode.UInt32:
				case TypeCode.UInt64:
					return new UnsignedIntegerComparer(culture);

				case TypeCode.Char:
				case TypeCode.Object:
				case TypeCode.String:
					return new StringComparer(culture);

				default:
					return DefaultComparer;
			}
		}

		/// <summary>
		/// Gets the comparer for the given database type.
		/// </summary>
		/// <param name="typeCode"></param>
		/// <returns></returns>
		public static IComparer GetComparer(DbType dbType, CultureInfo culture)
		{
			return GetComparer(ConversionHelper.GetClrTypeForDBType(dbType), culture);
		}

		/// <summary>
		/// Gets the culture.
		/// </summary>
		public CultureInfo Culture
		{
			get
			{
				return _culture;
			}
			set
			{
				if(value == null)
					value = CultureInfo.InvariantCulture;
				if(_culture != value)
					_culture = value;
			}
		}
	}
}
