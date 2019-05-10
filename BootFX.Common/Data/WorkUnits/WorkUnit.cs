// BootFX - Application framework for .NET applications
// 
// File: WorkUnit.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Runtime.Serialization;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using BootFX.Common.Data.Formatters;
using BootFX.Common.Entities;
using BootFX.Common.Management;
using System.Text;
using System.Security;

namespace BootFX.Common.Data
{
	/// <summary>
	///	 Base class for work units.
	/// </summary>
	[Serializable()]
	public abstract class WorkUnit : Loggable, IWorkUnit, ISerializable
	{
		/// <summary>
		/// Private field to support <see cref="TouchedValueBlocks"/> property.
		/// </summary>
		private ArrayList _touchedValueBlocks;
		
		/// <summary>
		/// Private field to support <c>ResultsBag</c> property.
		/// </summary>
		private WorkUnitResultsBag _resultsBag;
		
		/// <summary>
		/// Private field to support <c>EntityType</c> property.
		/// </summary>
		[NonSerialized()]
		private EntityType _entityType;

		/// <summary>
		/// Private field to support <c>Entity</c> property.
		/// </summary>
		private object _entity;
		
		/// <summary>
		/// Private field to support <c>Fields</c> property.
		/// </summary>
		[NonSerialized()]
		private EntityField[] _fields;

		/// <summary>
		/// Private field to support <c>Values</c> property.
		/// </summary>
		private object[] _values;

		/// <summary>
		/// Constructor.
		/// </summary>
		protected WorkUnit(EntityType entityType, object entity) : this(entityType, entity, new EntityField[] {}, new object[] {})
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		protected WorkUnit(EntityType entityType, object entity, EntityField[] fields, object[] values) 
		{
			if(entityType == null)
				throw new ArgumentNullException("entityType");
			if(entity == null)
				throw new ArgumentNullException("entity");
			if(fields == null)
				throw new ArgumentNullException("fields");
			if(values == null)
				throw new ArgumentNullException("values");

			// check...
			if(fields.Length != values.Length)
				throw ExceptionHelper.CreateLengthMismatchException("fields", "values", fields.Length, values.Length);

			// check...
			entityType.AssertIsOfType(entity);

			// set...
			_entityType = entityType;
			_entity = entity;
			_fields = fields;
			_values = values;
		}

		/// <summary>
		/// Deserialization constructor.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected WorkUnit(SerializationInfo info, StreamingContext context)
		{
			if(info == null)
				throw new ArgumentNullException("info");
			
			// get...
			_entityType = EntityType.Restore(info, OnNotFound.ThrowException);
			if(EntityType == null)
				throw new InvalidOperationException("EntityType is null.");
			_entity = info.GetValue("_entity", typeof(object));
			_values = (object[])info.GetValue("_values", typeof(object[]));

			// names...
			string[] fieldNames = (string[])info.GetValue("fieldNames", typeof(string[]));
			_fields = this.EntityType.Fields.GetFields(fieldNames, OnNotFound.ThrowException);
		}

		/// <summary>
		/// Gets the entity.
		/// </summary>
		public object Entity
		{
			get
			{
				return _entity;
			}
		}
		
		/// <summary>
		/// Gets the entitytype.
		/// </summary>
		public EntityType EntityType
		{
			get
			{
				return _entityType;
			}
		}

		/// <summary>
		/// Sets the bag containing results.
		/// </summary>
		/// <param name="resultsBag"></param>
		void IWorkUnit.SetResultsBag(WorkUnitResultsBag resultsBag)
		{
			if(resultsBag == null)
				throw new ArgumentNullException("resultsBag");
			
			// set...
			_resultsBag = resultsBag;
		}

		/// <summary>
		/// Gets the resultsbag.
		/// </summary>
		public WorkUnitResultsBag ResultsBag
		{
			get
			{
				// returns the value...
				return _resultsBag;
			}
		}

		/// <summary>
		/// Gets the fields.
		/// </summary>
		private EntityField[] Fields
		{
			get
			{
				return _fields;
			}
		}

		/// <summary>
		/// Gets the values.
		/// </summary>
		private object[] Values
		{
			get
			{
				return _values;
			}
		}

		/// <summary>
		/// Gets the values.
		/// </summary>
		/// <returns></returns>
		protected object[] GetValues()
		{
			object[] results = new object[this.Values.Length];
			Array.Copy(this.Values, 0, results, 0, results.Length);

			// return...
			return results;
		}

        public bool HasField(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (name.Length == 0)
                throw new ArgumentException("'name' is zero-length.");

            // get...
            EntityField field = this.EntityType.Fields.GetField(name, OnNotFound.ReturnNull);
            if (field != null)
                return true;
            else
                return false;
        }

		/// <summary>
		/// Gets the value in the given field.
		/// </summary>
		/// <returns></returns>
		// mbr - 24-07-2008 - created to replace or stream in values inside the work unit processor but before statements
		// are generated...
		public void SetFieldValue(string fieldName, object value)
		{
			if(fieldName == null)
				throw new ArgumentNullException("fieldName");
			if(fieldName.Length == 0)
				throw new ArgumentOutOfRangeException("'fieldName' is zero-length.");

            // mbr - 2010-05-17 - changed this so that it can append values......
			int index = this.GetFieldIndex(fieldName, OnNotFound.ReturnNull);
            if (index != -1)
                this.Values[index] = value;
            else
            {
                int length = this.Values.Length;

                // values...
                object[] newValues = new object[length + 1];
                Array.Copy(this.Values, 0, newValues, 0, length);
                newValues[length] = value;
                _values = newValues;

                // field...
                EntityField[] newFields = new EntityField[length + 1];
                Array.Copy(this.Fields, 0, newFields, 0, length);
                newFields[length] = this.EntityType.Fields.GetField(fieldName, OnNotFound.ThrowException);
                _fields = newFields;
            }
		}

		/// <summary>
		/// Gets the value in the given field.
		/// </summary>
		/// <returns></returns>
		protected object GetFieldValue(string fieldName, Type returnType, ConversionFlags conversionFlags, OnNotFound onNotFound)
		{
			if(fieldName == null)
				throw new ArgumentNullException("fieldName");
			if(fieldName.Length == 0)
				throw ExceptionHelper.CreateZeroLengthArgumentException("fieldName");

			// get it...
			int index = this.GetFieldIndex(fieldName, onNotFound);
			return this.GetFieldValue(index, returnType, conversionFlags);
		}

		/// <summary>
		/// Gets the value in the given field.
		/// </summary>
		/// <param name="field"></param>
		/// <returns></returns>
		protected object GetFieldValue(EntityField field, Type returnType, ConversionFlags conversionFlags, OnNotFound onNotFound)
		{
			if(field == null)
				throw new ArgumentNullException("field");
			
			// get it...
			int index = this.GetFieldIndex(field, onNotFound);
			object value = this.GetFieldValue(index, returnType, conversionFlags);

			// mbr - 31-08-2006 - added change in here for numeric types to round values...
			if(field.NeedsRounding)
			{
				if(value == null)
					throw new InvalidOperationException("value is null.");

				// convert...
				if(value is decimal)
					value = Math.Round((decimal)value, field.Scale);
				else if(value is double)
					value = Math.Round((double)value, field.Scale);
				else if(value is float)
					value = Math.Round((float)value, field.Scale);
				else
					throw new NotSupportedException(string.Format("Cannot handle '{0}'.", value.GetType()));
			}

			// return...
			return value;
		}

		/// <summary>
		/// Gets the value at the given index.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="returnType"></param>
		/// <returns></returns>
		private object GetFieldValue(int index, Type returnType, ConversionFlags conversionFlags)
		{
			object value = null;
			if(index != -1)
				value = this.Values[index];

			// return...
			if(returnType == null)
				return value;
			else
				return ConversionHelper.ChangeType(value, returnType, Cultures.System, conversionFlags);
		}

		/// <summary>
		/// Gets the index of the given field.
		/// </summary>
		/// <param name="field"></param>
		/// <returns></returns>
		private int GetFieldIndex(string fieldName, OnNotFound onNotFound)
		{
			if(fieldName == null)
				throw new ArgumentNullException("fieldName");
			if(fieldName.Length == 0)
				throw ExceptionHelper.CreateZeroLengthArgumentException("fieldName");
			
			// walk...
			for(int index = 0; index < this.Fields.Length; index++)
			{
				if(string.Compare(fieldName, Fields[index].Name, true, System.Globalization.CultureInfo.InvariantCulture) == 0)
					return index;
			}

			// nope...
			switch(onNotFound)
			{
				case OnNotFound.ReturnNull:
					return -1;
				case OnNotFound.ThrowException:
					throw new InvalidOperationException(string.Format(Cultures.System, "Field '{0}' was not found.", fieldName));

				default:
					throw ExceptionHelper.CreateCannotHandleException(onNotFound);
			}
		}

		/// <summary>
		/// Gets the index of the given field.
		/// </summary>
		/// <param name="field"></param>
		/// <returns></returns>
		private int GetFieldIndex(EntityField field, OnNotFound onNotFound)
		{
			if(field == null)
				throw new ArgumentNullException("field");
			
			// walk...
			for(int index = 0; index < this.Fields.Length; index++)
			{
				if(this.Fields[index] == field)
					return index;
			}

			// nope...
			switch(onNotFound)
			{
				case OnNotFound.ReturnNull:
					return -1;
				case OnNotFound.ThrowException:
					throw new InvalidOperationException(string.Format(Cultures.Exceptions, "Field '{0}' was not found.", field));

				default:
					throw ExceptionHelper.CreateCannotHandleException(onNotFound);
			}
		}

		/// <summary>
		/// Gets the fields stored in the work unit.
		/// </summary>
		/// <returns></returns>
		/// <remarks>This does not return all the fields on the entity.  To do that, use EntityType.Fields</remarks>
		public EntityField[] GetFields()
		{
			if(Fields == null)
				throw new ArgumentNullException("Fields");

			// copy and return...
			EntityField[] results = new EntityField[this.Fields.Length];
			this.Fields.CopyTo(results, 0);
			return results;
		}

		/// <summary>
		/// Gets the key fields stored in the work unit.
		/// </summary>
		/// <returns></returns>
		/// <remarks>This does not return all the fields on the entity.  To do that, use EntityType.Fields</remarks>
		public EntityField[] GetKeyFields()
		{
			return this.GetFields(true,false);
		}

		/// <summary>
		/// Gets the non key fields stored in the work unit.
		/// </summary>
		/// <returns></returns>
		/// <remarks>This does not return all the fields on the entity.  To do that, use EntityType.Fields</remarks>
		protected EntityField[] GetNonKeyFields()
		{
			return this.GetFields(false,false);
		}

		/// <summary>
		/// Gets the non auto increment key fields in the work unit.
		/// </summary>
		/// <returns></returns>
		/// <remarks>This does not return all the fields on the entity.  To do that, use EntityType.Fields</remarks>
        public EntityField[] GetNonAutoIncrementFields()
		{
			ArrayList results = new ArrayList();
			foreach(EntityField field in this.Fields)
			{
				// mbr - 04-07-2007 - changed.				
				//if(!(field.IsAutoIncrement()))
				if(!(field.IsAutoIncrement))
                    results.Add(field);
			}

			return (EntityField[])results.ToArray(typeof(EntityField));
		}

		/// <summary>
		/// Gets the extended field stored in the work unit.
		/// </summary>
		/// <returns></returns>
		/// <remarks>This does not return all the fields on the entity.  To do that, use EntityType.Fields</remarks>
		protected EntityField[] GetExtendedFields()
		{
			return this.GetFields(false,true);
		}

		/// <summary>
		/// Gets the key fields stored in the work unit.
		/// </summary>
		/// <returns></returns>
		/// <remarks>This does not return all the fields on the entity.  To do that, use EntityType.Fields</remarks>
		private EntityField[] GetFields(bool isKey, bool isExtended)
		{
			ArrayList results = new ArrayList();
			foreach(EntityField field in this.Fields)
			{
				if(field.IsKey() == isKey && field.IsExtendedProperty == isExtended)
					results.Add(field);
			}

			return (EntityField[])results.ToArray(typeof(EntityField));
		}

		/// <summary>
		/// Gets the work unit type.
		/// </summary>
		public abstract WorkUnitType WorkUnitType
		{
			get;
		}

        [SecurityCritical]
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			this.GetObjectData(info, context);
		}

		/// <summary>
		/// Gets data for serialization.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if(info == null)
				throw new ArgumentNullException("info");
			
			// set...
			if(EntityType == null)
				throw new InvalidOperationException("EntityType is null.");
			this.EntityType.Store(info);
			info.AddValue("_entity", _entity);
			info.AddValue("_values", _values);

			// names...
			string[] fieldNames = new string[this.Fields.Length];
			for(int index = 0; index < this.Fields.Length; index++)
				fieldNames[index] = this.Fields[index].Name;
			info.AddValue("fieldNames", fieldNames);
		}

		/// <summary>
		/// Gets the SQL dialect.
		/// </summary>
		protected SqlDialect Dialect
		{
			get
			{
				if(EntityType == null)
					throw new InvalidOperationException("EntityType is null.");
				return this.EntityType.Dialect;
			}
		}

		/// <summary>
		/// Creates a parameter for the given field.
		/// </summary>
		/// <param name="field"></param>
		/// <returns></returns>
		protected SqlStatementParameter CreateParameterForField(EntityField field)
		{
			return CreateParameterForField(field, false, OnNotFound.ThrowException);
		}

		/// <summary>
		/// Creates a parameter for the given field.
		/// </summary>
		/// <param name="field"></param>
		/// <returns></returns>
		protected SqlStatementParameter CreateParameterForField(EntityField field, bool forceToDbNull, OnNotFound throwIfNotFound)
		{
			if(field == null)
				throw new ArgumentNullException("field");

			// mbr - 27-10-2005 - added ability to force to DBNull...
			object value = null;
			if(forceToDbNull)
				value = DBNull.Value;
			else
			{
				value = this.GetFieldValue(field, null, ConversionFlags.Safe, throwIfNotFound);

				// do we have a DB null equivalent?
				if(field.HasDBNullEquivalent)
				{
					// check...
					if(object.Equals(value, field.DBNullEquivalent))
						value = DBNull.Value;
				}

				// if we're null, or a min date time, use DB null.
				if(value == null || (value is DateTime && ((DateTime)value) == DateTime.MinValue))
					value = DBNull.Value;

				//				// mbr - 27-10-2005 - if we are DBNull, and we have an equivalent, use that...
				//				if(value is DBNull && field.HasDBNullEquivalent)
				//					value = field.DBNullEquivalent;
			}

			// return...
			return new SqlStatementParameter(field.NativeName.Name, field.DBType, value);
		}

		/// <summary>
		/// Gets the statement that will execute the unit.
		/// </summary>
		/// <param name="connection"></param>
		/// <returns></returns>
		public abstract SqlStatement[] GetStatements(WorkUnitProcessingContext context);

		/// <summary>
		/// Processes the work unit.
		/// </summary>
		/// <param name="context"></param>
		public virtual void Process(WorkUnitProcessingContext context, ITimingBucket timings)
		{
			if(context == null)
				throw new ArgumentNullException("context");
			
			// walk...
            SqlStatement[] statements = null;
            using(timings.GetTimer("GetStatements"))
                statements = this.GetStatements(context);

            using (timings.GetTimer("Execute"))
            {
                foreach (SqlStatement statement in statements)
                    context.Connection.ExecuteNonQuery(statement);
            }
		}

		/// <summary>
		/// Gets the touchedvalueblocks.
		/// </summary>
		private ArrayList TouchedValueBlocks
		{
			get
			{
				if(_touchedValueBlocks == null)
					_touchedValueBlocks = new ArrayList();
				return _touchedValueBlocks;
			}
		}

		/// <summary>
		/// Adds a touched value block.
		/// </summary>
		/// <returns></returns>
		protected TouchedValueCollection AddTouchedValueBlock()
		{
			TouchedValueCollection block = new TouchedValueCollection();
			this.TouchedValueBlocks.Add(block);

			// return...
			return block;
		}

		/// <summary>
		/// Gets values touched by this update.
		/// </summary>
		/// <returns></returns>
		public TouchedValueWrapper GetTouchedValues()
		{
			if(_touchedValueBlocks == null)
				return new TouchedValueWrapper(new TouchedValueCollection[] {});
			else
			{
				return new TouchedValueWrapper(
					(TouchedValueCollection[])this.TouchedValueBlocks.ToArray(typeof(TouchedValueCollection)));
			}
		}

        protected EntityField[] GetAutoIncrementFields()
        {
            ArrayList results = new ArrayList();
            foreach (EntityField field in this.Fields)
            {
                // mbr - 04-07-2007 - changed.				
                //if(!(field.IsAutoIncrement()))
                if (field.IsAutoIncrement)
                    results.Add(field);
            }

            return (EntityField[])results.ToArray(typeof(EntityField));
        }

        public object GetFieldValue(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (name.Length == 0)
                throw new ArgumentException("'name' is zero-length.");

            // defer...
            object oldValue = null;
            object newValue = null;
            this.GetFieldValue(name, ref oldValue, ref newValue);

            // return...
            return newValue;
        }

        public void GetFieldValue(string name, ref object oldValue, ref object newValue)
        {
            // get...
            if (EntityType == null)
                throw new InvalidOperationException("'EntityType' is null.");
            EntityField field = this.EntityType.Fields.GetField(name, OnNotFound.ThrowException);
            if (field == null)
                throw new InvalidOperationException("'field' is null.");

            // return...
            GetFieldValue(field, ref oldValue, ref newValue);
        }

        public void GetFieldValue<T>(string fieldName, ref T oldValue, ref T newValue)
        {
            oldValue = default(T);
            newValue = default(T);

            object oldObject = null;
            object newObject = null;
            this.GetFieldValue(fieldName, ref oldObject, ref newObject);

            oldValue = ConversionHelper.ChangeType<T>(oldObject);
            newValue = ConversionHelper.ChangeType<T>(newObject);
        }

        public void GetFieldValue<T>(EntityField field, ref T oldValue, ref T newValue)
        {
            oldValue = default(T);
            newValue = default(T);

            object oldObject = null;
            object newObject = null;
            this.GetFieldValue(field, ref oldObject, ref newObject);

            oldValue = ConversionHelper.ChangeType<T>(oldObject);
            newValue = ConversionHelper.ChangeType<T>(newObject);
        }

        public object GetFieldValue(EntityField field)
        {
            if (field == null)
                throw new ArgumentNullException("field");

            // go...
            object oldValue = null;
            object newValue = null;
            this.GetFieldValue(field, ref oldValue, ref newValue);

            // return...
            return newValue;
        }

        public void GetFieldValue(EntityField field, ref object oldValue, ref object newValue)
        {
            oldValue = null;
            newValue =null;

            // walk...
            TouchedValueWrapper wrapper = this.GetTouchedValues();
            if(wrapper == null)
	            throw new InvalidOperationException("'wrapper' is null.");

            // walk...
            foreach (TouchedValueCollection touched in wrapper.GetBlocks())
            {
                foreach (TouchedValue value in touched)
                {
                    if (value.Field == field)
                    {
                        oldValue = value.OldValue;
                        newValue = value.NewValue;
                        break;
                    }
                }
            }
        }

        public bool NeedsTransaction
        {
            get
            {
                return false;
            }
        }

        internal string Hash
        {
            get
            {
                // build it...
                var builder = new StringBuilder();
                builder.Append(this.GetType().Name);
                builder.Append("|");
                builder.Append(this.EntityType.FullName);

                // fields...
                foreach (var field in this.Fields)
                {
                    builder.Append("|");
                    builder.Append(field.Name);
                }

                // get...
                return HashHelper.GetMd5HashOfStringAsBase64(builder.ToString());
            }
        }

        public bool HasChangeForField(string fieldName)
        {
            return this.HasChangeForField(this.EntityType.Fields.GetField(fieldName));
        }

        public bool HasChangeForField(EntityField field)
        {
            foreach (var values in this.GetTouchedValues().GetBlocks())
            {
                foreach (TouchedValue value in values)
                {
                    if (value.Field == field)
                        return true;
                }
            }

            return false;
        }
    }
}
