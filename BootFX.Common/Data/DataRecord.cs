// BootFX - Application framework for .NET applications
// 
// File: DataRecord.cs
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
using System.Data;
using System.Collections;

namespace BootFX.Common.Data
{
	/// <summary>
	/// Defines an instance of <c>DataRecord</c>.
	/// </summary>
	public abstract class DataRecord : IDataRecord
	{
		protected DataRecord()
		{
		}

		public int GetInt32(int i)
		{
			return (int)this.GetValue(i, typeof(int));
		}

		public int GetInt32(string name)
		{
			return this.GetInt32(this.GetOrdinal(name));
		}

		public object this[string name]
		{
			get
			{
				int index = this.GetOrdinal(name);
				if(index == -1)
					throw new InvalidOperationException(string.Format("A column with name '{0}' was not found.", name));

				// return...
				return this.GetValue(index);
			}
		}

		public object this[int i]
		{
			get
			{
				return this.GetValue(i);
			}
		}

		protected abstract object GetRawValue(int index);

		public object GetValue(int i)
		{
			return this.GetValue(i, null);
		}

		public virtual object GetValue(int index, Type type)
		{
			// get...
			object value = this.GetRawValue(index);

			// type?
			if(type != null)
				value = ConversionHelper.ChangeType(value, type, Cultures.System);

			// erturn...
			return value;
		}

        public T GetValue<T>(int index)
        {
            return (T)this.GetValue(index, typeof(T));
        }

		public bool IsDBNull(int i)
		{
			object value = this.GetValue(i);
			if(value is DBNull)
				return true;
			else
				return false;
		}

		public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
		{
			// TODO: This method has not been implemented.  Check whether it is a valid not implementated, or a code path that was not completed.
			throw new NotImplementedException(string.Format("Not implemented for '{0}'.", this.GetType()));
		}

		public byte GetByte(int i)
		{
			return (byte)this.GetValue(i, typeof(byte));
		}

		public byte GetByte(string name)
		{
			return this.GetByte(this.GetOrdinal(name));
		}

		public abstract Type GetFieldType(int i);

		public decimal GetDecimal(int i)
		{
			return (decimal)this.GetValue(i, typeof(decimal));
		}

		public decimal GetDecimal(string name)
		{
			return this.GetDecimal(this.GetOrdinal(name));
		}

		public int GetValues(object[] values)
		{
			// check...
			int max = values.Length;
			if(values.Length >= this.FieldCount)
				max = this.FieldCount;

			// get...
			for(int index = 0; index < max; index++)
				values[index] = this[index];

			// return...
			return max;
		}

		public abstract string GetName(int i);

		public abstract int FieldCount
		{
			get;
		}

		public long GetInt64(int i)
		{
			return (long)this.GetValue(i, typeof(long));
		}

		public long GetInt64(string name)
		{
			return this.GetInt64(this.GetOrdinal(name));
		}

		public double GetDouble(int i)
		{
			return (double)this.GetValue(i, typeof(double));
		}

		public double GetDouble(string name)
		{
			return this.GetDouble(this.GetOrdinal(name));
		}

		public bool GetBoolean(int i)
		{
			return (bool)this.GetValue(i, typeof(bool));
		}

		public bool GetBoolean(string name)
		{
			return this.GetBoolean(this.GetOrdinal(name));
		}

        public bool GetBooleanSafe(string name)
        {
            int ordinal = this.GetOrdinal(name, false);
            if (ordinal != -1)
                return GetBoolean(ordinal);
            else
                return false;
        }

        public Guid GetGuid(int i)
		{
			return (Guid)this.GetValue(i, typeof(Guid));
		}

		public Guid GetGuid(string name)
		{
			return this.GetGuid(this.GetOrdinal(name));
		}

		public DateTime GetDateTime(int i)
		{
			return (DateTime)this.GetValue(i, typeof(DateTime));
		}

		public DateTime GetDateTime(string name)
		{
			return this.GetDateTime(this.GetOrdinal(name));
		}

		public int GetOrdinal(string name)
		{
			return this.GetOrdinal(name, true);
		}

		// mbr - 08-02-2007 - added 'throw if not found'...		
		public int GetOrdinal(string name, bool throwIfNotFound)
		{
			if(name == null)
				throw new ArgumentNullException("name");
			if(name.Length == 0)
				throw new ArgumentOutOfRangeException("'name' is zero-length.");
			
			// walk...
			for(int index = 0; index < this.FieldCount; index++)
			{
				string check = this.GetName(index);
				if(string.Compare(check, name, true, System.Globalization.CultureInfo.InvariantCulture) == 0)
					return index;
			}

			// nope...
			if(throwIfNotFound)
				throw new InvalidOperationException(string.Format("A field with name '{0}' was not found.", name));
			else
				return -1;
		}

		public string GetDataTypeName(int i)
		{
			// TODO: This method has not been implemented.  Check whether it is a valid not implementated, or a code path that was not completed.
			throw new NotImplementedException(string.Format("Not implemented for '{0}'.", this.GetType()));
		}

		public float GetFloat(int i)
		{
			return (float)this.GetValue(i, typeof(float));
		}

		public float GetFloat(string name)
		{
			return this.GetFloat(this.GetOrdinal(name));
		}

		public virtual IDataReader GetData(int i)
		{
			return null;
		}

		public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
		{
			// TODO: This method has not been implemented.  Check whether it is a valid not implementated, or a code path that was not completed.
			throw new NotImplementedException(string.Format("Not implemented for '{0}'.", this.GetType()));
		}

        public string GetString(int i)
		{
			return (string)this.GetValue(i, typeof(string));
		}

        public string GetStringSafe(string name)
        {
            int ordinal = this.GetOrdinal(name, false);
            if (ordinal != -1)
                return GetString(ordinal);
            else
                return null;
        }

		public string GetString(string name)
		{
			return this.GetString(this.GetOrdinal(name));
		}

		public char GetChar(int i)
		{
			return (char)this.GetValue(i, typeof(char));
		}

		public char GetChar(string name)
		{
			return this.GetChar(this.GetOrdinal(name));
		}

		public short GetInt16(int i)
		{
			return (short)this.GetValue(i, typeof(short));
		}

		public short GetInt16(string name)
		{
			return this.GetInt16(this.GetOrdinal(name));
		}


        public bool IsField(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (name.Length == 0)
                throw new ArgumentException("'name' is zero-length.");

            // get...
            int ordinal = this.GetOrdinal(name);
            if (ordinal != -1)
                return true;
            else
                return false;
        }
	}
}
