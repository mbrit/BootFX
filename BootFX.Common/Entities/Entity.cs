// BootFX - Application framework for .NET applications
// 
// File: Entity.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Data;
using System.Drawing;
using System.Runtime.Serialization;
using System.Reflection;
using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics;
using System.ComponentModel;
using BootFX.Common.Xml;
using BootFX.Common.Data;
using BootFX.Common.Crypto;
using BootFX.Common.Management;
using System.Security;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BootFX.Common.Entities
{
	/// <summary>
	/// Base class for an entity.
	/// </summary>
	// mbr - 19-10-2005 - added cloneable...
	// mbr - 02-12-2005 - removed ToXmlBase.	
	[Serializable()]
	public abstract class Entity : Loggable, IEntityType, IFormattableToString, ISaveChangesNotification, ISerializable, ICloneable, IEntityId
	{
		/// <summary>
		/// Raised after the entity has been loaded from the database.
		/// </summary>
		// mbr - 02-10-2007 - for c7 - added.		
		public event EventHandler AfterLoad;
		
		/// <summary>
		/// Private field to support <see cref="AffinityState"/> property.
		/// </summary>
		private object _affinityState;
		
		/// <summary>
		/// Private field to support <c>OriginalValues</c> property.
		/// </summary>
		private IDictionary _originalValues = new HybridDictionary();

		// mbr - 06-09-2007 - removed.
		//		/// <summary>
		//		/// Private field to support <c>Events</c> property.
		//		/// </summary>
		//		private EventBag _changedEvents = new EventBag();
		
		/// <summary>
		/// Private field to support <c>Parents</c> property.
		/// </summary>
		private IDictionary _parents = null;
		
		/// <summary>
		/// Private field to support <c>Flags</c> property.
		/// </summary>
		private EntityFlags _flags = EntityFlags.Normal;
		
		/// <summary>
		/// Private field to support <c>InitializeCount</c> property.
		/// </summary>
		private int _initializeCount = 0;

		/// <summary>
		/// Private field to support <c>IsNew</c> property.
		/// </summary>
		private bool _isNew = true;
		
		/// <summary>
		/// Private field to support <c>EntityType</c> property.
		/// </summary>
		private EntityType _entityType;
		
		/// <summary>
		/// Private field to support <c>Slots</c> property.
		/// </summary>
		private object[] _slots;
		
		/// <summary>
		/// Private field to support <c>SlotFlags</c> property.
		/// </summary>
		private SlotFlags[] _slotFlags;

		/// <summary>
		/// Raised before changes are saved.
		/// </summary>
		// mbr - 02-10-2007 - case 827 - deprecated.
		[Obsolete("Use the version that indicates transaction state.")]
		public event BeforeSaveChangesEventHandler BeforeSaveChanges;

		/// <summary>
		/// Raised before changes are saved.
		/// </summary>
		// mbr - 02-10-2007 - case 827 - added.
		public event BeforeSaveChangesEventHandler BeforeSaveChangesInTransaction;

		/// <summary>
		/// Raised before changes are saved.
		/// </summary>
		// mbr - 02-10-2007 - case 827 - added.
		public event BeforeSaveChangesEventHandler BeforeSaveChangesOutTransaction;

		/// <summary>
		/// Raised after changes are saved.
		/// </summary>
		// mbr - 02-10-2007 - case 827 - deprecated.
		[Obsolete("Use the version that indicates transaction state.")]
		public event AfterSaveChangesEventHandler AfterSaveChanges;

		/// <summary>
		/// Raised after changes are saved.
		/// </summary>
		// mbr - 02-10-2007 - case 827 - added.
		public event AfterSaveChangesEventHandler AfterSaveChangesInTransaction;

		/// <summary>
		/// Raised after changes are saved.
		/// </summary>
		// mbr - 02-10-2007 - case 827 - added.
		public event AfterSaveChangesEventHandler AfterSaveChangesOutTransaction;

		/// <summary>
		/// Raised before a value change occurs.
		/// </summary>
		public event FieldChangedEventHandler BeforeValueChanged;

		/// <summary>
		/// Raised after a value change occurs.
		/// </summary>
		public event FieldChangedEventHandler AfterFieldChanged;

        public event EventHandler Saving;

        /// <summary>
        /// Constructor.
        /// </summary>
        protected Entity()
		{
			// set...
			_entityType = EntityType.GetEntityType(this.GetType(), OnNotFound.ThrowException);

			// setup...
			int numFields = this.EntityType.Fields.Count;
			_slots = new object[numFields];
			_slotFlags = new SlotFlags[numFields];

			// mbr - 02-02-2007 - added affinity...
			_affinityState = Database.AffinityState;
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		protected Entity(SerializationInfo info, StreamingContext context)
		{
			// set...
			_entityType = EntityType.Restore(info, OnNotFound.ThrowException);

			// other stuff...
			_originalValues = (IDictionary)info.GetValue("_originalValues", typeof(IDictionary));
			_flags = (EntityFlags)info.GetValue("_flags", typeof(EntityFlags));
			_initializeCount = (int)info.GetValue("_initializeCount", typeof(int));
			_slots = (object[])info.GetValue("_slots", typeof(object[]));
			_slotFlags = (SlotFlags[])info.GetValue("_slotFlags", typeof(SlotFlags[]));

			// mbr - 17-5-06 - removed.
            // mbr - 2014-11-27 -- um... this broke serialization...
			_isNew = info.GetBoolean("_isNew");
		}

		/// <summary>
		/// Gets the slotflags.
		/// </summary>
		private SlotFlags[] SlotFlags
		{
			get
			{
				// returns the value...
				return _slotFlags;
			}
		}

		/// <summary>
		/// Gets the slots.
		/// </summary>
		private object[] Slots
		{
			get
			{
				// returns the value...
				return _slots;
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
		/// Gets or sets the value against a given field.
		/// </summary>
		public object this[string fieldName]
		{
			get
			{
				if(fieldName == null)
					throw new ArgumentNullException("fieldName");
				if(fieldName.Length == 0)
					throw ExceptionHelper.CreateZeroLengthArgumentException("fieldName");
				
				// defer...
				return GetValue(fieldName);
			}
			set
			{
				if(fieldName == null)
					throw new ArgumentNullException("fieldName");
				if(fieldName.Length == 0)
					throw ExceptionHelper.CreateZeroLengthArgumentException("fieldName");
				
				// defer...
				SetValue(fieldName, value, SetValueReason.UserSet);
			}
		}

		/// <summary>
		/// Gets or sets the value against a given field.
		/// </summary>
		/// <example>
		/// </example>
		protected object this[EntityField field]
		{
			get
			{
				if(field == null)
					throw new ArgumentNullException("field");
				
				// defer...
				return GetValue(field);
			}
			set
			{
				if(field == null)
					throw new ArgumentNullException("field");
				
				// defer...
				SetValue(field, value, SetValueReason.UserSet);
			}
		}

		/// <summary>
		/// Gets the value for the given field.
		/// </summary>
		/// <param name="fieldName"></param>
		/// <returns></returns>
		private object GetValue(string fieldName)
		{
			if(fieldName == null)
				throw new ArgumentNullException("fieldName");
			if(fieldName.Length == 0)
				throw ExceptionHelper.CreateZeroLengthArgumentException("fieldName");
			if(EntityType == null)
				throw new ArgumentNullException("EntityType");

			// defer...
			return this.GetValue(this.EntityType.Fields.GetField(fieldName, OnNotFound.ThrowException));
		}

		/// <summary>
		/// Gets the value for the given field.
		/// </summary>
		/// <param name="fieldName"></param>
		/// <returns></returns>
		private void SetValue(string fieldName, object value, SetValueReason reason)
		{
			if(fieldName == null)
				throw new ArgumentNullException("fieldName");
			if(fieldName.Length == 0)
				throw ExceptionHelper.CreateZeroLengthArgumentException("fieldName");
			if(EntityType == null)
				throw new ArgumentNullException("EntityType");

			// defer...
			this.SetValue(this.EntityType.Fields.GetField(fieldName, OnNotFound.ThrowException), value, reason);
		}

		/// <summary>
		/// Indicate that we are new.
		/// </summary>
		/// <returns></returns>
		/// <remarks>If any of the key field values are not loaded or are at their default value, this returns true.</remarks>
		public bool IsNew()
		{
			if(EntityType == null)
				throw new ArgumentNullException("EntityType");

			if(_isNew)
				return true;

			// walk all the fields looking for keys...
			for(int index = 0; index < this.EntityType.Fields.Count; index++)
			{
				EntityField field = this.EntityType.Fields[index];
				if(field.IsKey() == true)
				{
					// is it loaded?
					if(this.IsLoaded(field.Ordinal) == false)
						return true;

					// is it default?
					object value = this.GetValue(field);
					if(object.Equals(value, field.GetDefaultValue()) == true)
						return true;
				}
			}

			// nope...
			return false;
		}

		/// <summary>
		/// Internal method used to reset the _isNew private variable
		/// The _isNew flag should only be true when a new instance has been created
		/// </summary>
		protected internal void ResetIsNew()
		{
			_isNew = false;
		}

		/// <summary>
		/// Internal method used to reset the _isNew private variable
		/// The _isNew flag should only be true when a new instance has been created
		/// </summary>
		protected internal void SetIsNew()
		{
			_isNew = false;
		}

		/// <summary>
		/// Gets the value for the given field.
		/// </summary>
		/// <param name="field"></param>
		/// <returns></returns>
		/// <remarks>By default, this method will use <see cref="ConversionFlags.Safe"></see> to clean up CLR null and DB null values.</remarks>
		internal object GetValue(EntityField field)
		{
			return this.GetValue(field, ConversionFlags.Safe, FetchFlags.None);
		}

		/// <summary>
		/// Gets the value for the given field.
		/// </summary>
		/// <param name="field"></param>
		/// <returns></returns>
		internal object GetValue(EntityField field, ConversionFlags flags, FetchFlags fetchFlags)
		{
			if(field == null)
				throw new ArgumentNullException("field");
				
			// is this column loaded?
			if(this.IsLoaded(field.Ordinal) == false)
			{
				// are we new, or is this a key field?
				if(this.IsInitializing() == true || field.IsKey() == true || this.IsNew() == true)
				{
					// if we get in here, basically we're doing lazy initialization of just setting up the fields
					// if we happen to use them...

					// set to be loaded...
					this.Slots[field.Ordinal] = field.GetDefaultValue();
					this.SetSlotFlags(field.Ordinal, Entities.SlotFlags.Loaded, true);
				}
				else
				{
					// demand load...
					DemandLoad(field);
				}
			}
		
			// get the value...
			object value = this.Slots[field.Ordinal];

			// encrypted?
			//if(value != null && !(value is DBNull) && field.IsEncrypted && (fetchFlags & FetchFlags.KeepEncrypted) == 0)
			//{
			//	if(!(value is EncryptedValue))
			//		throw new InvalidOperationException(string.Format("Encrypted field has stored value as '{0}'.", value));

			//	// return...
			//	value = ((EncryptedValue)value).Decrypt(field.EncryptionKeyName);
			//}

			// convert...
			if(flags != ConversionFlags.None) // && !(value is EncryptedValue))
				value = ConversionHelper.ChangeType(value, field.Type, Cultures.System, flags);

			// return...
			return value;
		}

		/// <summary>
		/// Gets the ordinal of the given field.
		/// </summary>
		/// <param name="fieldName"></param>
		/// <returns></returns>
		protected int GetFieldOrdinal(string fieldName)
		{
			// find it...
			if(EntityType == null)
				throw new InvalidOperationException("EntityType is null.");
			EntityField field = this.EntityType.Fields.GetField(fieldName, OnNotFound.ThrowException);
			if(field == null)
				throw new InvalidOperationException("field is null.");

			// return...
			return field.Ordinal;
		}

		/// <summary>
		/// Demand loads data for the given field.
		/// </summary>
		/// <param name="ordinal"></param>
		private void DemandLoad(EntityField field)
		{
			if(field == null)
				throw new ArgumentNullException("field");

			// new?
			if(this.IsNew() == true)
				throw new InvalidOperationException("Cannot demand load on a new entity.");

			if(EntityType == null)
				throw new ArgumentNullException("EntityType");

			// expand...
			ArrayList toFetch = new ArrayList();
			toFetch.Add(field);

			// large?
			if(field.IsLarge() == false)
			{
				// find other unloaded fields...  we have 4k to use, so try and fill that...
				long availableSpace = 4096L - field.Size;
				long initialSpace = availableSpace;

				// count from this to the end (on the basis that the most important columns are at the top...)
				for(int index = field.Ordinal + 1; index < this.Slots.Length; index++)
				{
					// good?
					EntityField checkField = this.EntityType.Fields[index];
					if(this.IsDemandLoadCandidate(checkField, availableSpace) == true)
					{
						// add...
						toFetch.Add(checkField);
						availableSpace -= field.Size;
						if(availableSpace < 0)
							break;
					}
				}

				// more?
				if(availableSpace > 0)
				{
					// walk from the beginning to this...
					for(int index = 0; index < field.Ordinal; index++)
					{
						// good?
						EntityField checkField = this.EntityType.Fields[index];
						if(this.IsDemandLoadCandidate(checkField, availableSpace) == true)
						{
							// add...
							toFetch.Add(checkField);
							availableSpace -= field.Size;
							if(availableSpace < 0)
								break;
						}					
					}
				}
			}

			// load...
			this.DemandLoad((EntityField[])toFetch.ToArray(typeof(EntityField)));
		}

		/// <summary>
		/// Returns true if the given field is a good candidate for a demand load.
		/// </summary>
		/// <param name="field"></param>
		/// <param name="spaceAvailable"></param>
		/// <returns></returns>
		private bool IsDemandLoadCandidate(EntityField field, long spaceAvailable)
		{
			if(field == null)
				throw new ArgumentNullException("field");
			if(spaceAvailable <= 0)
				return false;

			// check metrics...
			if(field.IsLarge() == true)
				return false;
			if(this.IsLoaded(field.Ordinal) == true)
				return false;
			if(field.Size > spaceAvailable)
				return false;

			// we're good to go!
			return true;
		}

		/// <summary>
		/// Demand loads data.
		/// </summary>
		/// <param name="fields"></param>
		private void DemandLoad(EntityField[] fields)
		{
			if(fields == null)
				throw new ArgumentNullException("fields");
			if(fields.Length == 0)
				throw ExceptionHelper.CreateZeroLengthArgumentException("fields");

			// new?
			if(this.IsNew() == true)
				throw new InvalidOperationException("Cannot demand load on a new entity.");

			// go out to persistence...
			if(EntityType == null)
				throw new ArgumentNullException("EntityType");
			if(EntityType.Persistence == null)
				throw new ArgumentNullException("EntityType.Persistence");

			// mbr - 02-02-2007 - affinity...
			IConnection affinityConnection = SetupAffinity();
			try
			{
				this.EntityType.Persistence.DemandLoad(this, fields);
			}
			finally
			{
				if(affinityConnection != null)
					Database.Unbind();
			}			
		}

		private IConnection SetupAffinity()
		{
			if(this.AffinityState == null)
				return null;

			// call an event on the runtime to get an affinity connection...
			IConnection conn = Runtime.Current.SetupAffinity(this.AffinityState);
			if(conn == null)
				throw new InvalidOperationException("conn is null.");

			// are we bound already?
			// JM - 2007-06-02 - changed to check if bound
			if(!Database.IsBound)
			{
				// bind that connection...
				Database.Bind(conn, this.AffinityState);

				// return it...
				return conn;
			}
			else
				return null;
		}

		/// <summary>
		/// Gets the value for the given field.
		/// </summary>
		/// <param name="field"></param>
		/// <returns></returns>
		internal void SetValue(EntityField field, object value, SetValueReason reason)
		{
			if(field == null)
				throw new ArgumentNullException("field");

			// readonly?
			if(reason == SetValueReason.UserSet)
			{
				if(this.IsReadOnly)
					throw new InvalidOperationException("The entity is read-only.  This can happen if the entity has been added to a cache.");
			}

			// is the value null?
			//if(!(value is DBNull) && !(value is EncryptedValue))
			//{
			//	value = ConversionHelper.ChangeType(value, field.Type, Cultures.System);
			//}

			// mbr - 17-03-2006 - if the value is null then we have modified it...			
			// has it changed?
			object existingValue = null;
			if(this.IsDBNull(field))
				existingValue = DBNull.Value;
			else
				existingValue = this.GetValue(field);

			// equals...
			if(object.Equals(value, existingValue) == false || IsNew())
			{
				// flag the entity as modified...
				if(this.IsInitializing() == false)
				{
					// mbr - 06-09-2007 - raise an event...  (although we're creating a value here, 
					FieldChangedEventArgs e = new FieldChangedEventArgs(this, field, existingValue, value);
					this.OnBeforeFieldChanged(e);
					if(e.Cancel)
						return;

					// do a complex set so that we can preserve the original value as it was loaded from the database...
					object key = this.GetOriginalValuesKey(field);
					if(this.OriginalValues.Contains(key) == false)
						this.OriginalValues[key] = existingValue;

					// is the field encrypted?
					//if(field.IsEncrypted)
					//{
					//	if(value != null && !(value is DBNull))
					//		value = EncryptedValue.Create(field.EncryptionKeyName, value);
					//}

					// set...
					this.Slots[field.Ordinal] = value;
					this.SetSlotFlags(field.Ordinal, Entities.SlotFlags.Modified, true);

					// mbr - 06-09-2007 - raise the after change event...
					//					this.RaiseChangedEvent(field);
					this.OnAfterFieldChanged(e);
				}
				else
				{
					// do a simple set...
					this.Slots[field.Ordinal] = value;
				}
			}
		}

		/// <summary>
		/// Gets the originalvalues.
		/// </summary>
		internal IDictionary OriginalValues
		{
			get
			{
				return _originalValues;
			}
		}

		/// <summary>
		/// Raised the changed event for the given field.
		/// </summary>
		/// <param name="field"></param>
		// mbr - 06-09-2007 - added.		
		[Obsolete("Use 'OnAfterFieldChanged'.")]
		protected void RaiseChangedEvent(string fieldName)
		{
			if(fieldName == null)
				throw new ArgumentNullException("fieldName");
			if(fieldName.Length == 0)
				throw new ArgumentOutOfRangeException("'fieldName' is zero-length.");

			// get...
			if(EntityType == null)
				throw new InvalidOperationException("EntityType is null.");
			EntityField field = this.EntityType.Fields.GetField(fieldName, OnNotFound.ThrowException);
			if(field == null)
				throw new InvalidOperationException("field is null.");
			this.RaiseChangedEvent(field);
		}

		/// <summary>
		/// Raised the changed event for the given field.
		/// </summary>
		/// <param name="field"></param>
		// mbr - 06-09-2007 - added.		
		[Obsolete("Use 'OnAfterFieldChanged'.")]
		protected void RaiseChangedEvent(EntityField field)
		{
			if(field == null)
				throw new ArgumentNullException("field");

			// mbr - 06-09-2007 - now what...
			this.OnBeforeFieldChanged(new FieldChangedEventArgs(this, field, null, this[field]));
		}

		/// <summary>
		/// Raises the <c>OnAfterFieldChanged</c> event.
		/// </summary>
		protected virtual void OnAfterFieldChanged(FieldChangedEventArgs e)
		{
			// raise...
			if(AfterFieldChanged != null)
				AfterFieldChanged(this, e);
		}
		
		/// <summary>
		/// Raises the <c>OnBeforeValueChanged</c> event.
		/// </summary>
		protected virtual void OnBeforeFieldChanged(FieldChangedEventArgs e)
		{
			// raise...
			if(BeforeValueChanged != null)
				BeforeValueChanged(this, e);
		}


		// mbr - 06-09-2007 - removed.		
		//		/// <summary>
		//		/// Removes a changed event handler.
		//		/// </summary>
		//		/// <param name="fieldName"></param>
		//		/// <param name="handler"></param>
		//		protected void RemoveFieldChangedHandler(string fieldName, EventHandler handler)
		//		{
		//			if(fieldName == null)
		//				throw new ArgumentNullException("fieldName");
		//			if(fieldName.Length == 0)
		//				throw new ArgumentOutOfRangeException("'fieldName' is zero-length.");
		//			if(handler == null)
		//				throw new ArgumentNullException("handler");
		//			
		//			// get...
		//			if(EntityType == null)
		//				throw new InvalidOperationException("EntityType is null.");
		//			EntityField field = this.EntityType.Fields.GetField(fieldName, OnNotFound.ThrowException);
		//			if(field == null)
		//				throw new InvalidOperationException("field is null.");
		//			this.RemoveFieldChangedHandler(field, handler);
		//		}
		//
		//		/// <summary>
		//		/// Removes a changed event handler.
		//		/// </summary>
		//		/// <param name="fieldName"></param>
		//		/// <param name="handler"></param>
		//		protected void RemoveFieldChangedHandler(EntityField field, EventHandler handler)
		//		{
		//			if(field == null)
		//				throw new ArgumentNullException("field");
		//			if(handler == null)
		//				throw new ArgumentNullException("handler");
		//			
		//			// remove...
		//			object key = this.GetChangedEventKey(field);
		//			if(key == null)
		//				throw new InvalidOperationException("key is null.");
		//			this.ChangedEvents.RemoveHandler(key, handler);
		//		}
		//
		//		/// <summary>
		//		/// Adds a changed event handler.
		//		/// </summary>
		//		/// <param name="fieldName"></param>
		//		/// <param name="handler"></param>
		//		protected void AddFieldChangedHandler(string fieldName, EventHandler handler)
		//		{
		//			if(fieldName == null)
		//				throw new ArgumentNullException("fieldName");
		//			if(fieldName.Length == 0)
		//				throw new ArgumentOutOfRangeException("'fieldName' is zero-length.");
		//			if(handler == null)
		//				throw new ArgumentNullException("handler");
		//			
		//			// get...
		//			if(EntityType == null)
		//				throw new InvalidOperationException("EntityType is null.");
		//			EntityField field = this.EntityType.Fields.GetField(fieldName, OnNotFound.ThrowException);
		//			if(field == null)
		//				throw new InvalidOperationException("field is null.");
		//			this.AddFieldChangedHandler(field, handler);
		//		}
		//
		//		/// <summary>
		//		/// Adds a changed event handler.
		//		/// </summary>
		//		/// <param name="fieldName"></param>
		//		/// <param name="handler"></param>
		//		protected void AddFieldChangedHandler(EntityField field, EventHandler handler)
		//		{
		//			if(field == null)
		//				throw new ArgumentNullException("field");
		//			if(handler == null)
		//				throw new ArgumentNullException("handler");
		//			
		//			// set...
		//			object key = this.GetChangedEventKey(field);
		//			if(key == null)
		//				throw new InvalidOperationException("key is null.");
		//			this.ChangedEvents.AddHandler(key, handler);
		//		}
		//
		//		/// <summary>
		//		/// Gets the key for a change event for the given field.
		//		/// </summary>
		//		/// <param name="field"></param>
		//		/// <returns></returns>
		//		private object GetChangedEventKey(EntityField field)
		//		{
		//			if(field == null)
		//				throw new ArgumentNullException("field");
		//			
		//			// return...
		//			return field.Ordinal;
		//		}

		/// <summary>
		/// Returns true if any of the fields are modified.
		/// </summary>
		/// <returns></returns>
		public bool IsModified()
		{
			for(int index = 0; index < this.Slots.Length; index++)
			{
				if(this.GetSlotFlags(index, Entities.SlotFlags.Modified) == true)
					return true;
			}

			// nope...
			return false;
		}

		/// <summary>
		/// Returns true if the field is loaded.
		/// </summary>
		/// <param name="ordinal"></param>
		/// <returns></returns>
		public bool IsModified(string fieldName)
		{
			if(fieldName == null)
				throw new ArgumentNullException("fieldName");
			if(fieldName.Length == 0)
				throw new ArgumentOutOfRangeException("'fieldName' is zero-length.");
			
			// get...
			if(EntityType == null)
				throw new InvalidOperationException("EntityType is null.");
			EntityField field = this.EntityType.Fields.GetField(fieldName, OnNotFound.ThrowException);
			if(field == null)
				throw new InvalidOperationException("field is null.");

			// get...
			return this.IsModified(field);
		}

		/// <summary>
		/// Returns true if the field is loaded.
		/// </summary>
		/// <param name="ordinal"></param>
		/// <returns></returns>
		public bool IsModified(EntityField field)
		{
			if(field == null)
				throw new ArgumentNullException("field");

			// get...
			return this.GetSlotFlags(field.Ordinal, BootFX.Common.Entities.SlotFlags.Modified);
		}

		/// <summary>
		/// Returns true if the given ordinal is loaded.
		/// </summary>
		/// <param name="ordinal"></param>
		/// <returns></returns>
		private bool IsLoaded(int ordinal)
		{
			return GetSlotFlags(ordinal, Entities.SlotFlags.Loaded);
		}

		/// <summary>
		/// Returns true if the given ordinal is loaded.
		/// </summary>
		/// <param name="ordinal"></param>
		/// <returns></returns>
		protected bool IsLoaded(EntityField field)
		{
			if(field == null)
				throw new ArgumentNullException("field");
			return GetSlotFlags(field.Ordinal, Entities.SlotFlags.Loaded);
		}

		/// <summary>
		/// Gets the given flag values.
		/// </summary>
		/// <param name="ordinal"></param>
		/// <param name="flags"></param>
		/// <returns></returns>
		private bool GetSlotFlags(int ordinal, SlotFlags flags)
		{
			this.AssertIsValidOrdinal(ordinal);

			// return...
			if((this.SlotFlags[ordinal] & flags) != 0)
				return true;
			else
				return false;
		}

		/// <summary>
		/// Gets the given flag values.
		/// </summary>
		/// <param name="ordinal"></param>
		/// <param name="flags"></param>
		/// <returns></returns>
		private void SetSlotFlags(int ordinal, SlotFlags flags, bool value)
		{
			this.AssertIsValidOrdinal(ordinal);

			// return...
			this.SlotFlags[ordinal] |= flags;
			if(value == false)
				this.SlotFlags[ordinal] ^= flags;
		}

		/// <summary>
		/// Returns true if the given ordinal is valid.
		/// </summary>
		/// <param name="ordinal"></param>
		/// <returns></returns>
		private bool IsValidOrdinal(int ordinal)
		{
			if(ordinal >= 0 && ordinal < this.Slots.Length)
				return true;
			else
				return false;
		}

		/// <summary>
		/// Asserts that the given ordinal is valid.
		/// </summary>
		/// <param name="ordinal"></param>
		private void AssertIsValidOrdinal(int ordinal)
		{
            if (this.IsValidOrdinal(ordinal) == false)
                throw new ArgumentOutOfRangeException("ordinal", ordinal, string.Format("Ordinal must be between 0 and {1}.", this.Slots.Length - 1));
		}

		/// <summary>
		/// Gets the initializecount.
		/// </summary>
		private int InitializeCount
		{
			get
			{
				// returns the value...
				return _initializeCount;
			}
		}

		/// <summary>
		/// Returns true if the entity is initializing.
		/// </summary>
		/// <returns></returns>
		protected internal bool IsInitializing()
		{
			if(this.InitializeCount > 0)
				return true;
			else
				return false;
		}

		/// <summary>
		/// Begins initiaizlizing the entity.
		/// </summary>
		protected internal void BeginInitialize()
		{
			this._initializeCount++;
		}

		/// <summary>
		/// Begins initiaizlizing the entity.
		/// </summary>
		protected internal void EndInitialize()
		{
			this._initializeCount--;

			// end...
			if(_initializeCount == 0)
				InitializationComplete();
		}

		/// <summary>
		/// Called when entity initialization has completed.
		/// </summary>
		private void InitializationComplete()
		{
			// mbr - 02-10-2007 - for c7 - call the event...
			this.OnAfterLoad();
		}

        public Task SaveChangesAsync()
        {
            return LazySaveEngine.Current.EnqueueAsync(this);
        }

        public void SaveChanges()
        {
            SaveChanges(null);
        }

		/// <summary>
		/// Saves changes to the entity.
		/// </summary>
		public void SaveChanges(ITimingBucket timings)
		{
            this.OnSaving();

			// pass this over to the persistence service...
            this.EntityType.Persistence.SaveChanges(this, timings);
		}

        private void OnSaving()
        {
            this.OnSaving(EventArgs.Empty);
        }

        protected virtual void OnSaving(EventArgs e)
        {
            if (this.Saving != null)
                this.Saving(this, e);
        }

        public void SaveChangesOutsideTransaction()
        {
            this.OnSaving();

            // pass this over to the persistence service...
            this.EntityType.Persistence.SaveChangesOutsideTransaction(this);
        }

        /// <summary>
        /// Gets the flags.
        /// </summary>
        private EntityFlags Flags
		{
			get
			{
				// returns the value...
				return _flags;
			}
		}

		/// <summary>
		/// Marks an entity so that it will be deleted on the next <c>SaveChanges</c>.
		/// </summary>
		/// <seealso cref="UnmarkForDeletion"></seealso>
		public void MarkForDeletion()
		{
			this.SetEntityFlags(EntityFlags.DeletePending, true);
		}
		
		/// <summary>
		/// Marks an entity so that it will not be deleted on the next <c>SaveChanges</c>.
		/// </summary>
		/// <seealso cref="MarkForDeletion"></seealso>
		public void UnmarkForDeletion()
		{
			this.SetEntityFlags(EntityFlags.DeletePending, false);
		}

		/// <summary>
		/// Returns true if the entity has been deleted.
		/// </summary>
		/// <returns></returns>
		public bool IsDeleted()
		{
			return this.IsDeleted(true);
		}

		/// <summary>
		/// Returns true if the entity has been deleted.
		/// </summary>
		/// <param name="treatedMarkedAsDeleted">True if <see cref="IsMarkedForDeletion"></see> should be treated as 'deleted'</param>
		/// <returns></returns>
		public bool IsDeleted(bool treatedMarkedAsDeletedAsDeleted)
		{
			if(treatedMarkedAsDeletedAsDeleted == true && this.IsMarkedForDeletion() == true)
				return true;
			else
			{
				// are we physically deleted?
				return this.GetEntityFlags(EntityFlags.Deleted);
			}
		}

		/// <summary>
		/// Returns true if the entity has been deleted.
		/// </summary>
		/// <returns></returns>
		public bool IsMarkedForDeletion()
		{
			// mbr - 20-07-2007 - changed this as have removed cascade delete...
			//			return this.GetEntityFlags(EntityFlags.DeletePending)  || this.GetEntityFlags(EntityFlags.CascadeDeletePending);
			return this.GetEntityFlags(EntityFlags.DeletePending); 
		}

		/// <summary>
		/// Sets entity flags.
		/// </summary>
		/// <param name="flags"></param>
		/// <param name="value"></param>
		private void SetEntityFlags(EntityFlags flags, bool value)
		{
			_flags |= flags;
			if(value == false)
				_flags ^= flags;
		}

		/// <summary>
		/// Gets entity flags.
		/// </summary>
		/// <param name="flags"></param>
		/// <returns></returns>
		private bool GetEntityFlags(EntityFlags flags)
		{
			if((Flags & flags) != 0)
				return true;
			else
				return false;
		}

		/// <summary>
		/// Marks the entity as physically removed from the database.
		/// </summary>
		internal void MarkAsDeleted()
		{
			this.SetEntityFlags(EntityFlags.Deleted, true);
		}
		
		/// <summary>
		/// Called when the entity should no longer be marked as modified.
		/// </summary>
		internal void ResetModifiedFlags()
		{
			for(int index = 0; index < this.Slots.Length; index++)
				this.SetSlotFlags(index, BootFX.Common.Entities.SlotFlags.Modified, false);
		}

		/// <summary>
		/// Ensures all columns have been loaded.
		/// </summary>
		private void EnsureAllColumnsLoaded()
		{
			if(EntityType == null)
				throw new ArgumentNullException("EntityType");

			// demand?
			if(this.IsNew() == true || this.IsDeleted(false) == true)
				return;

			// select...
			ArrayList toLoad = new ArrayList();
			foreach(EntityField field in this.EntityType.Fields)
			{
				if(this.IsLoaded(field.Ordinal) == false)
					toLoad.Add(field);
			}

			// load?
			if(toLoad.Count == 0)
				return;

			// demand load it...
			this.DemandLoad((EntityField[])toLoad.ToArray(typeof(EntityField)));
		}

		//		/// <summary>
		//		/// Gets the entity as XML.
		//		/// </summary>
		//		/// <param name="xml"></param>
		//		/// <param name="context"></param>
		//		protected override void WriteXml(System.Xml.XmlWriter xml, WriteXmlContext context)
		//		{
		//			if(xml == null)
		//				throw new ArgumentNullException("xml");
		//			if(context == null)
		//				throw new ArgumentNullException("context");
		//			if(EntityType == null)
		//				throw new ArgumentNullException("EntityType");
		//
		//			// load them all...
		//			this.EnsureAllColumnsLoaded();
		//
		//			// add attributes...
		//			if(this.IsDeleted() == true)
		//				xml.WriteAttributeString("deleted", true.ToString());
		//			else
		//			{
		//				if(this.IsNew() == true)
		//					xml.WriteAttributeString("isNew", true.ToString());
		//			}
		//
		//			// do each field in turn...
		//			foreach(EntityField field in this.EntityType.Fields)
		//			{
		//				// not ignored?
		//				if(field.XmlIgnore() == false)
		//				{
		//					// are we always persisting as base-64?
		//					object value = this.GetValue(field);
		//					if(field.PersistAsBase64() == true)
		//					{
		//						// write it as base 64...
		//						XmlHelper.WriteDataElementAsBase64(xml, field.Name, value, field.Encoding);
		//					}
		//					else if(field.PersistAsCData() == true)
		//					{
		//						// write it using CData...
		//						SafeCDataStrategy strategy = SafeCDataStrategy.Base64Encode;
		//						XmlHelper.WriteDataElementAsCData(xml, field.Name, value, field.Encoding, context.Culture, strategy);
		//					}
		//					else
		//					{
		//						// use the generic writer...
		//						XmlHelper.WriteDataElement(xml, field.Name, value, field.Encoding, context.Culture);
		//					}
		//				}
		//			}
		//		}

		/// <summary>
		/// Gets the entity from an XML string.
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		[Obsolete("This method has been removed.  There is no direct replacement within the framework.  Ask MaBR for more information.")]
		public static object FromXml(string xml)
		{
			if(xml == null)
				throw new ArgumentNullException("xml");
			if(xml.Length == 0)
				throw new ArgumentOutOfRangeException("'xml' is zero-length.");
			
			// load...
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(xml);

			// from...
			return FromXmlDocument(doc);
		}

		/// <summary>
		/// Gets the entity from an XML document.
		/// </summary>
		/// <param name="doc"></param>
		/// <returns></returns>
		[Obsolete("This method has been removed.  There is no direct replacement within the framework.  Ask MaBR for more information.")]
		public static object FromXmlDocument(XmlDocument doc)
		{
			if(doc == null)
				throw new ArgumentNullException("doc");

			// find the root...
			XmlElement root = XmlHelper.GetRootElement(doc, true);
			if(root == null)
				throw new InvalidOperationException("root is null.");

			// get...
			XmlNamespaceManager manager = XmlHelper.GetNamespaceManager(doc);
			if(manager == null)
				throw new InvalidOperationException("manager is null.");

			// get...
			return FromXmlElement(root, manager);
		}

		/// <summary>
		/// Gets the entity from an XML element.
		/// </summary>
		/// <param name="element"></param>
		/// <param name="manager"></param>
		/// <returns></returns>
		[Obsolete("This method has been removed.  There is no direct replacement within the framework.  Ask MaBR for more information.")]
		public static object FromXmlElement(XmlElement element, XmlNamespaceManager manager)
		{
			if(element == null)
				throw new ArgumentNullException("element");
			if(manager == null)
				throw new ArgumentNullException("manager");
			
			// discover the type...
			Type type = XmlHelper.GetAttributeType(element, "type", OnNotFound.ThrowException);
			if(type == null)
				throw new InvalidOperationException("type is null.");

			// metadata...
			EntityType et = EntityType.GetEntityType(type, OnNotFound.ThrowException);
			if(et == null)
				throw new InvalidOperationException("et is null.");

			// storage...
			IEntityStorage storage = et.Storage;
			if(storage == null)
				throw new InvalidOperationException("storage is null.");

			// create one...
			object entity = et.CreateInstance();
			if(entity == null)
				throw new InvalidOperationException("entity is null.");

			// walk the elements...
			foreach(XmlNode node in element.ChildNodes)
			{
				if(node is XmlElement)
				{
					// is it a field?  (if not a field, we might be loading a newer/older version, or this is some other data...)
					EntityField field = et.Fields[node.Name];
					if(field != null)
					{
						// is it null?
						bool isNull = XmlHelper.GetAttributeBoolean(node, "isDbNull", OnNotFound.ReturnNull);
						if(isNull && field.IsNullable())
							storage.SetDBNull(entity, field);
						else
						{
							// get the value...
							object value = XmlHelper.GetElementValue((XmlElement)node);
							storage.SetValue(entity, field, value, SetValueReason.ReaderLoad);
						}
					}
				}
			}

			// reset...
			storage.ResetModifiedFlags(entity);

			// set new...
			if(XmlHelper.GetAttributeBoolean(element, "IsNew", OnNotFound.ReturnNull))
				storage.SetIsNew(entity);
			else
				storage.ResetIsNew(entity);

			// deleted...
			if(XmlHelper.GetAttributeBoolean(element, "IsDeleted", OnNotFound.ReturnNull))
				storage.MarkAsDeleted(entity);

			// call...
			if(entity is Entity)
				((Entity)entity).LoadedFromXmlElement(element, manager);

			// return...
			return entity;
		}

		/// <summary>
		/// Called when the entity has been loaded from XML.
		/// </summary>
		/// <param name="element"></param>
		/// <param name="manager"></param>
		[Obsolete("This method has been removed.  There is no direct replacement within the framework.  Ask MaBR for more information.")]
		protected virtual void LoadedFromXmlElement(XmlElement element, XmlNamespaceManager manager)
		{
		}

		/// <summary>
		/// Gets the entity as an XML string.
		/// </summary>
		/// <returns></returns>	
		[Obsolete("This method has been removed.  There is no direct replacement within the framework.  Ask MaBR for more information.")]
		public string ToXml()
		{
			XmlDocument doc = this.ToXmlDocument();
			if(doc == null)
				throw new InvalidOperationException("doc is null.");

			// return...
			using(StringWriter writer = new StringWriter())
			{
				doc.Save(writer);

				// return...
				writer.Flush();
				return writer.GetStringBuilder().ToString();
			}
		}

		/// <summary>
		/// Gets the entity as an XML document.
		/// </summary>
		/// <returns></returns>
		[Obsolete("This method has been removed.  There is no direct replacement within the framework.  Ask MaBR for more information.")]
		public XmlDocument ToXmlDocument()
		{
			XmlDocument doc = new XmlDocument();
			XmlNamespaceManager manager = XmlHelper.GetNamespaceManager(doc);

			// pop...
			XmlElement element = this.ToXmlElement(doc, manager);
			if(element == null)
				throw new InvalidOperationException("element is null.");

			// add...
			doc.AppendChild(element);

			// return...
			return doc;
		}

		[Obsolete("This method has been removed.  There is no direct replacement within the framework.  Ask MaBR for more information.")]
		internal XmlElement ToXmlElement(XmlDocument doc, XmlNamespaceManager manager)
		{
			// create...
			XmlElement element = doc.CreateElement(this.GetType().Name);
			doc.AppendChild(element);

			// populate...
			this.PopulateXmlElement(element, manager);

			// return...
			return element;
		}

		/// <summary>
		/// Gets the entity as an XML string.
		/// </summary>
		/// <returns></returns>
		[Obsolete("This method has been removed.  There is no direct replacement within the framework.  Ask MaBR for more information.")]
		protected virtual void PopulateXmlElement(XmlElement element, XmlNamespaceManager manager)
		{
			if(element == null)
				throw new ArgumentNullException("element");
			if(manager == null)
				throw new ArgumentNullException("manager");
			
			// add the type...
			XmlHelper.SetAttributeValue(element, "type", this.GetType());

			// new?
			if(this.IsNew())
				XmlHelper.SetAttributeValue(element, "isNew", true);

			// new?
			if(this.IsDeleted())
				XmlHelper.SetAttributeValue(element, "isDeleted", true);

			// walk the fields...
			foreach(EntityField field in this.EntityType.Fields)
			{
				if(this.IsDBNull(field))
				{
					XmlElement child = XmlHelper.AddElement(element, field.Name, string.Empty);
					if(child == null)
						throw new InvalidOperationException("child is null.");
					XmlHelper.SetAttributeValue(child, "isDbNull", true);
				}
				else
					XmlHelper.AddElement(element, field.Name, this[field]);
			}
		}

		/// <summary>
		/// Gets the binary value.
		/// </summary>
		/// <param name="fieldName"></param>
		/// <returns></returns>
		protected byte[] GetBinaryValue(string fieldName)
		{
			if(fieldName == null)
				throw new ArgumentNullException("fieldName");
			if(fieldName.Length == 0)
				throw ExceptionHelper.CreateZeroLengthArgumentException("fieldName");
			
			// get...
			if(EntityType == null)
				throw new ArgumentNullException("EntityType");
			EntityField field = this.EntityType.Fields.GetField(fieldName, OnNotFound.ThrowException);
			if(field == null)
				throw new ArgumentNullException("field");

			// set...
			return this.GetBinaryValue(field);
		}

		/// <summary>
		/// Gets the binary value.
		/// </summary>
		/// <param name="fieldName"></param>
		/// <returns></returns>
		protected byte[] GetBinaryValue(EntityField field)
		{
			if(field == null)
				throw new ArgumentNullException("field");

			// get it...
			return (byte[])this.GetValue(field);
		}

		/// <summary>
		/// Gets the parent for the given link name.
		/// </summary>
		/// <param name="linkName"></param>
		/// <returns></returns>
		protected internal void SetParent(string linkName, object entity)
		{
			if(linkName == null)
				throw new ArgumentNullException("linkName");
			if(linkName.Length == 0)
				throw new ArgumentOutOfRangeException("'linkName' is zero-length.");
			
			// get...
			if(EntityType == null)
				throw new ArgumentNullException("EntityType");
			EntityLink link = this.EntityType.Links.GetLink(linkName, OnNotFound.ThrowException);
			if(link == null)
				throw new ArgumentNullException("link");

			// check...
			if(link is ChildToParentEntityLink)
			{
				this.SetParent((ChildToParentEntityLink)link, entity);

				// mbr - 10-10-2007 - moved to the other SetParent call...				
				//				if(this.Parents.Contains(link))
				//					this.Parents[link] = entity;
			}
			else
				throw new NotSupportedException(string.Format(Cultures.Exceptions, "Cannot handle '{0}'.", link.GetType()));
		}

		/// <summary>
		/// Gets the parent for the given link name.
		/// </summary>
		/// <param name="linkName"></param>
		/// <returns></returns>
		protected internal object GetParent(string linkName)
		{
			return this.GetParent(linkName, new EntityField[] {});
		}

		/// <summary>
		/// Gets the parent for the given link name.
		/// </summary>
		/// <param name="linkName"></param>
		/// <param name="parentFields"></param>
		/// <returns></returns>
		protected internal object GetParent(string linkName, EntityField[] parentFields)
		{
			if(linkName == null)
				throw new ArgumentNullException("linkName");
			if(linkName.Length == 0)
				throw ExceptionHelper.CreateZeroLengthArgumentException("linkName");
			
			// get...
			if(EntityType == null)
				throw new ArgumentNullException("EntityType");
			EntityLink link = this.EntityType.Links.GetLink(linkName, OnNotFound.ThrowException);
			if(link == null)
				throw new ArgumentNullException("link");

			// check...
			if(link is ChildToParentEntityLink)
				return this.GetParent((ChildToParentEntityLink)link);
			else
				throw new NotSupportedException(string.Format(Cultures.Exceptions, "Cannot handle '{0}'.", link.GetType()));
		}

		/// <summary>
		/// Gets the parent for the given link name.
		/// </summary>
		/// <param name="linkName"></param>
		/// <returns></returns>
		protected internal object GetParent(ChildToParentEntityLink link)
		{
			if(link == null)
				throw new ArgumentNullException("link");
			
			return this.GetParent(link, new EntityField[] {});
		}

		/// <summary>
		/// Gets the parent for the given link name.
		/// </summary>
		/// <param name="linkName"></param>
		/// <returns></returns>
		protected internal object GetParent(ChildToParentEntityLink link, EntityField[] parentFields)
		{
			if(link == null)
				throw new ArgumentNullException("link");
			if(parentFields == null)
				throw new ArgumentNullException("parentFields");
			
			// check...
			if(link.ParentEntityType == null)
				throw new ArgumentNullException("parentLink.ParentEntityType");

			// do we have it?
			object parent = null;
			if(this.Parents.Contains(link) == false)
			{
				// get...
				parent = LoadParent(link, parentFields);

				// add...
				this.Parents[link] = parent;
			}
			else
				parent = this.Parents[link];

			// return...
			return parent;
		}

		/// <summary>
		/// Loads the given parent.
		/// </summary>
		/// <param name="link"></param>
		/// <param name="parentFields"></param>
		/// <returns></returns>
		protected virtual object LoadParent(ChildToParentEntityLink link, EntityField[] parentFields)
		{
			if(link == null)
				throw new ArgumentNullException("link");

			// create...
			if(Persistence == null)
				throw new ArgumentNullException("Persistence");
			string[] names = null;
			if(parentFields != null)
			{
				names = new string[parentFields.Length];
				for(int index = 0; index < parentFields.Length; index++)
					names[index] = parentFields[index].Name;
			}
			else
				names = new string[] {};

			// mbr - 02-02-2007 - affinity...
			IConnection affinityConnection = this.SetupAffinity();
			try
			{
                object parent = this.Persistence.GetParent(this, link.Name, names);
				return parent;
			}
			finally
			{
				if(affinityConnection != null)
					Database.Unbind();
			}
		}

		/// <summary>
		/// Gets the parents.
		/// </summary>
		private IDictionary Parents
		{
			get
			{
				// returns the value...
				if(_parents == null)
					_parents = new HybridDictionary();
				return _parents;
			}
		}

		/// <summary>
		/// Updates the properties on the object to map to the given parent.
		/// </summary>
		/// <param name="link"></param>
		/// <param name="newParent"></param>
		protected internal void SetParent(ChildToParentEntityLink link, object newParent)
		{
			if(link == null)
				throw new ArgumentNullException("link");
			
			// get the parent type...
			if(link.ParentEntityType == null)
				throw new ArgumentNullException("parentLink.ParentEntityType");

			// get the fields to change...
			EntityField[] relatedFields = link.GetLinkFields();
			if(relatedFields == null)
				throw new ArgumentNullException("relatedFields");

			// update the local fields...
			if(newParent != null)
			{
				// get the key fields...
				EntityField[] keyFields = link.ParentEntityType.GetKeyFields();
				if(keyFields == null)
					throw new ArgumentNullException("keyFields");

				// check...
				if(keyFields.Length != relatedFields.Length)
					throw ExceptionHelper.CreateLengthMismatchException("keyFields", "relatedFields", keyFields.Length, relatedFields.Length);

				// get storage...
				IEntityStorage storage = link.ParentEntityType.Storage;
				if(storage == null)
					throw new ArgumentNullException("storage");

				// get the key values...
				object[] keyValues = storage.GetKeyValues(newParent);

				// walk and set...
				for(int index = 0; index < relatedFields.Length; index++)
					this[relatedFields[index]] = keyValues[index];
			}
			else
			{
				// set the related data to be null... 
				// TODO: if the field cannot be null, this *should* throw an exception when we set...
				for(int index = 0; index < relatedFields.Length; index++)
					this[relatedFields[index]] = DBNull.Value;
			}

			// mbr - 10-10-2007 - moved from the other SetParent call.  this originally had a 
			// Contains check - I've taken this out as I can't see it does anything useful?
			//			if(this.Parents.Contains(link))
			this.Parents[link] = newParent;
		}

		/// <summary>
		/// Gets the string represention of the entity with the system culture.
		/// </summary>
		/// <returns></returns>
		/// <remarks>Do not add further overrides to this overload.  Instead, use the alternative version that has an <c>IFormatProvider</c> pass through
		/// its arguments.</remarks>
		public override string ToString()
		{
			return this.ToString(Cultures.System);
		}

		/// <summary>
		/// Gets the string representation of the entity with the given culture.
		/// </summary>
		/// <param name="formatter"></param>
		/// <returns></returns>
		public virtual string ToString(IFormatProvider formatter)
		{
			try
			{
				StringBuilder builder = new StringBuilder();
				builder.Append(base.ToString());
				builder.Append(": ");

				// new?
				if(this.IsNew() == false)
				{
					EntityField[] keyFields = this.EntityType.GetKeyFields();
					if(keyFields.Length > 0)
					{
						for(int index = 0; index < keyFields.Length; index++)
						{
							if(index > 0)
								builder.Append(", ");
							builder.Append(keyFields[index].Name);
							builder.Append(":");

							// format the value...
							object value = this.GetValue(keyFields[index]);
							if(value == null)
								value = "(null)";
							else if(value is DBNull)
								value = "(DB null)";
							else
								value = Convert.ToString(value, formatter);

							// append...
							builder.Append(value);
						}
					}
					else
						builder.Append("*** No key ***");
				}
				else
					builder.Append("*** New ***");

				// return...
				return builder.ToString();
			}
			catch(Exception ex)
			{
				return string.Format(formatter, "{0}: [{1}]", base.ToString(), ex.Message);
			}
		}

		/// <summary>
		/// Compares one entity with another.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		/// <remarks>This override performs a value comparison on the key values.  It does not provide a comparion
		/// to tell that two instances are identical.  This means that two *new* entity instances of the same type will return
		/// 'true' from this method, even though they may be separate instances.  This method will also unwrap any <c>EntityView</c> 
		/// instance passed in.</remarks>
		public override bool Equals(object obj)
		{
			// return...
			return EntityIdentification.Equals(this, obj);
		}

		/// <summary>
		/// Gets the hash code.
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			return base.GetHashCode ();
		}

		/// <summary>
		/// Gets the persistence service for the entity.
		/// </summary>
		protected IEntityPersistence Persistence
		{
			get
			{
				if(EntityType == null)
					throw new ArgumentNullException("EntityType");
				return this.EntityType.Persistence;
			}
		}

		/// <summary>
		/// Sets the given field to be DB null.
		/// </summary>
		/// <param name="field"></param>
		/// <returns></returns>
		public void SetDBNull(string fieldName)
		{
			if(fieldName == null)
				throw new ArgumentNullException("fieldName");
			if(fieldName.Length == 0)
				throw new ArgumentOutOfRangeException("'fieldName' is zero-length.");

			// defer...
			this.SetDBNull(this.GetField(fieldName));
		}

		/// <summary>
		/// Sets the given field to be DB null.
		/// </summary>
		/// <param name="field"></param>
		/// <returns></returns>
		public void SetDBNull(EntityField field)
		{
			if(field == null)
				throw new ArgumentNullException("field");

			// check...
			if(field.IsNullable() == false)
				throw new InvalidOperationException(string.Format(Cultures.Exceptions, "Field '{0}' cannot be DB null.", field));

			// set...
			this.SetValue(field, DBNull.Value, SetValueReason.UserSet);
		}

		/// <summary>
		/// Returns true if the given field is DBNull.
		/// </summary>
		/// <param name="field"></param>
		/// <returns></returns>
		public bool IsDBNull(string fieldName)
		{
			if(fieldName == null)
				throw new ArgumentNullException("fieldName");
			if(fieldName.Length == 0)
				throw new ArgumentOutOfRangeException("'fieldName' is zero-length.");

			// defer...
			return this.IsDBNull(this.GetField(fieldName));
		}

		/// <summary>
		/// Returns true if the given field is DBNull.
		/// </summary>
		/// <param name="field"></param>
		/// <returns></returns>
		public bool IsDBNull(EntityField field)
		{
			if(field == null)
				throw new ArgumentNullException("field");
			
			// defer...
			object value = this.GetValue(field, ConversionFlags.None, FetchFlags.None);
			if(value is DBNull)
				return true;
			else
				return false;
		}
	
		/// <summary>
		/// Gets the field with the given ordinal.
		/// </summary>
		/// <param name="ordinal"></param>
		/// <returns></returns>
		protected EntityField GetField(string fieldName)
		{
			if(EntityType == null)
				throw new InvalidOperationException("EntityType is null.");
			return this.EntityType.Fields.GetField(fieldName, OnNotFound.ThrowException);
		}

		/// <summary>
		/// Loads a fresh copy of this entity from the database.
		/// </summary>
		/// <returns></returns>
		/// <remarks>This is a helper method that should be accessed by a strongly-typed, public version of this method on the extending class called <c>Reload</c>.  If the entity is 'new', an 
		/// additional copy of the entity will be created.  If the entity has been deleted from the database (or the key is invalid - eithter way, if the entity cannot be found), 
		/// an <see cref="EntityNotFoundException"></see> exception will be thrown..</remarks>
		protected object ReloadInternal(OnNotFound onNotFound)
		{
			// new?
			if(EntityType == null)
				throw new InvalidOperationException("EntityType is null.");
			if(this.IsNew() == true)
				return this.EntityType.CreateInstance();

			// check...
			if(EntityType.Persistence == null)
				throw new InvalidOperationException("EntityType.Persistence is null.");

			// return...
			object[] keyValues = this.GetKeyValues();
			return this.EntityType.Persistence.GetById(keyValues, onNotFound);
		}

		/// <summary>
		/// Gets the entities key values.
		/// </summary>
		/// <returns></returns>
		public object[] GetKeyValues()
		{
			if(EntityType == null)
				throw new InvalidOperationException("EntityType is null.");

			// get...
			EntityField[] keyFields = EntityType.GetKeyFields();
			object[] keyValues = new object[keyFields.Length];
			for(int index = 0; index < keyFields.Length; index++)
				keyValues[index] = this.GetValue(keyFields[index]);

			// return...
			return keyValues;
		}

		// mbr - 02-10-2007 - case 827 - changed interface.		
		IWorkUnit ISaveChangesNotification.BeforeSaveChangesInTransaction(IWorkUnit unit, IConnection connection)
		{
			BeforeSaveChangesEventArgs e = new BeforeSaveChangesEventArgs(unit, connection);

			// mbr - 02-10-2007 - case 827 - changed...			
			//			this.OnBeforeSaveChanges(e);
			this.OnBeforeSaveChangesInTransaction(e);

			// return...
			return e.Unit;
		}

		// mbr - 02-10-2007 - case 827 - changed interface.		
		void ISaveChangesNotification.BeforeSaveChangesOutTransaction(IWorkUnit unit)
		{
			BeforeSaveChangesEventArgs e = new BeforeSaveChangesEventArgs(unit, null);

			// mbr - 02-10-2007 - case 827 - changed...			
			//			this.OnBeforeSaveChanges(e);
			this.OnBeforeSaveChangesOutTransaction(e);
		}

		// mbr - 02-10-2007 - case 827 - changed interface.		
		void ISaveChangesNotification.AfterSaveChangesInTransaction(IWorkUnit unit, IConnection connection)
		{
			this.OnAfterSaveChangesInTransaction(new AfterSaveChangesEventArgs(unit, connection));
		}

		// mbr - 02-10-2007 - case 827 - changed interface.		
		void ISaveChangesNotification.AfterSaveChangesOutTransaction(IWorkUnit unit)
		{
			this.OnAfterSaveChangesOutTransaction(new AfterSaveChangesEventArgs(unit, null));
		}

		/// <summary>
		/// Raises the <c>AfterSaveChanges</c> event.
		/// </summary>
		// mbr - 02-10-2007 - case 827 - obsolete
		[Obsolete("Use a variant that indicates transaction state.")]
		protected virtual void OnAfterSaveChanges(AfterSaveChangesEventArgs e)
		{
			// raise...
			if(AfterSaveChanges != null)
				AfterSaveChanges(this, e);
		}

		/// <summary>
		/// Raises the <c>AfterSaveChangesInTransaction</c> event.
		/// </summary>
		// mbr - 02-10-2007 - case 827 - added.
		protected virtual void OnAfterSaveChangesInTransaction(AfterSaveChangesEventArgs e)
		{
			// raise...
			if(AfterSaveChangesInTransaction != null)
				AfterSaveChangesInTransaction(this, e);
		}

		/// <summary>
		/// Raises the <c>AfterSaveChangesInTransaction</c> event.
		/// </summary>
		// mbr - 02-10-2007 - case 827 - added.
		protected virtual void OnAfterSaveChangesOutTransaction(AfterSaveChangesEventArgs e)
		{
			// raise...
			if(AfterSaveChangesOutTransaction != null)
				AfterSaveChangesOutTransaction(this, e);
		}

		/// <summary>
		/// Raises the <c>BeforeSaveChanges</c> event.
		/// </summary>
		// mbr - 02-10-2007 - case 827 - obsolete.
		[Obsolete("Use a variant that indicates transaction state.")]
		protected virtual void OnBeforeSaveChanges(BeforeSaveChangesEventArgs e)
		{
			// raise...
			if(BeforeSaveChanges != null)
				BeforeSaveChanges(this, e);
		}

		/// <summary>
		/// Raises the <c>BeforeSaveChangesInTranasction</c> event.
		/// </summary>
		// mbr - 02-10-2007 - case 827 - added.		
		protected virtual void OnBeforeSaveChangesInTransaction(BeforeSaveChangesEventArgs e)
		{
			// raise...
			if(BeforeSaveChangesInTransaction != null)
				BeforeSaveChangesInTransaction(this, e);
		}

		/// <summary>
		/// <summary>
		/// Raises the <c>BeforeSaveChangesOutTranasction</c> event.
		/// </summary>
		// mbr - 02-10-2007 - case 827 - added.		
		protected virtual void OnBeforeSaveChangesOutTransaction(BeforeSaveChangesEventArgs e)
		{
			// raise...
			if(BeforeSaveChangesOutTransaction != null)
				BeforeSaveChangesOutTransaction(this, e);
		}

		/// <summary>
		/// Gets the original value for a field.
		/// </summary>
		/// <param name="field"></param>
		/// <returns></returns>
		// mbr - 04-10-2007 - for c7 - made public.
		public object GetOriginalValue(string fieldName)
		{
			if(fieldName == null)
				throw new ArgumentNullException("fieldName");
			if(fieldName.Length == 0)
				throw new ArgumentOutOfRangeException("'fieldName' is zero-length.");
			
			// return...
			if(EntityType == null)
				throw new InvalidOperationException("EntityType is null.");
			return this.GetOriginalValue(this.EntityType.Fields.GetField(fieldName, OnNotFound.ThrowException));
		}

		/// <summary>
		/// Gets the original value for a field.
		/// </summary>
		/// <param name="field"></param>
		/// <returns></returns>
		// mbr - 04-10-2007 - for c7 - made public.
		public object GetOriginalValue(EntityField field)
		{
			if(field == null)
				throw new ArgumentNullException("field");
			
			// defer...
			object key = this.GetOriginalValuesKey(field);
			return this.OriginalValues[key];
		}

		/// <summary>
		/// Gets the key for use with the original values collection.
		/// </summary>
		/// <param name="field"></param>
		/// <returns></returns>
		private object GetOriginalValuesKey(EntityField field)
		{
			if(field == null)
				throw new ArgumentNullException("field");
			return field.Name;
		}

		// mbr - 06-09-2007 - removed.		
		//		/// <summary>
		//		/// Gets the events.
		//		/// </summary>
		//		private EventBag ChangedEvents
		//		{
		//			get
		//			{
		//				// returns the value...
		//				return _changedEvents;
		//			}
		//		}

        [SecurityCritical]
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			this.GetObjectData(info, context);
		}

		protected virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if(info == null)
				throw new ArgumentNullException("info");
			
			// set...
			if(EntityType == null)
				throw new InvalidOperationException("EntityType is null.");
			this.EntityType.Store(info);

			// other stuff...
			info.AddValue("_originalValues", _originalValues);
			info.AddValue("_flags", _flags);
			info.AddValue("_initializeCount", _initializeCount);
			info.AddValue("_slots", _slots);
			info.AddValue("_slotFlags", _slotFlags);

			// mbr - 17-5-06 - removed.
            // mbr - 2014-11-27 -- why on earth was this removed?
			info.AddValue("_isNew", _isNew);
		}

		/// <summary>
		/// Gets or sets the isreadonly
		/// </summary>
		internal bool IsReadOnly
		{
			get
			{
				return this.GetEntityFlags(EntityFlags.ReadOnly);
			}
			set
			{
				this.SetEntityFlags(EntityFlags.ReadOnly, value);
			}
		}

		/// <summary>
		/// Takes a WsEntity and applies all the custom properties to it.
		/// </summary>
		/// <param name="wsEntity"></param>
		protected void PopulateWsEntity(WsEntity wsEntity)
		{
			ArrayList names = new ArrayList();
			ArrayList values = new ArrayList();
			foreach(EntityField field in this.EntityType.Fields)
			{
				if(field.IsExtendedProperty)
				{
					if(this[field] == null)
						continue;

					names.Add(field.Name);
					values.Add(this[field].ToString());
				}
			}

			wsEntity.ExtendedPropertyNames = (string[]) names.ToArray(typeof(string));
			wsEntity.ExtendedPropertyValues = (object[]) values.ToArray(typeof(string));
		}

		/// <summary>
		/// Set an array of values to an entity
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="properties"></param>
		/// <param name="values"></param>
		public void SetProperties(Entity entity, string[] properties, object[] values)
		{
			if(entity == null)
				throw new InvalidOperationException("entity is null.");
			if(properties == null)
				throw new InvalidOperationException("properties is null.");
			if(values == null)
				throw new InvalidOperationException("values is null.");

			if(properties.Length != values.Length)
				throw new InvalidOperationException("propeties and values cannot different in length when setting properties.");

			for(int index = 0; index < properties.Length; index++)
			{
				entity[properties[index]] = values[index];
			}
		}

		/// <summary>
		/// Trims a string field so that it fits in the space available.
		/// </summary>
		/// <param name="field"></param>
		public void TrimToFit(string fieldName)
		{
			if(fieldName == null)
				throw new ArgumentNullException("fieldName");
			if(fieldName.Length == 0)
				throw new ArgumentOutOfRangeException("'fieldName' is zero-length.");
			
			// get...
			if(EntityType == null)
				throw new InvalidOperationException("EntityType is null.");
			EntityField field = this.EntityType.Fields[fieldName];
			if(field == null)
				throw new InvalidOperationException("field is null.");

			// defer...
			this.TrimToFit(field);
		}

		/// <summary>
		/// Trims a string field so that it fits in the space available.
		/// </summary>
		/// <param name="field"></param>
		public void TrimToFit(EntityField field)
		{
			if(field == null)
				throw new ArgumentNullException("field");
			
			// get...
			if(field.DBType != DbType.String && field.DBType != DbType.AnsiString && field.DBType != DbType.StringFixedLength && 
				field.DBType != DbType.AnsiStringFixedLength)
				throw new InvalidOperationException(string.Format("'{0}' is not a string field.", field.Name));

			// large?
			if(field.IsLarge() || this.IsDBNull(field))
				return;

			// get...
			string value = (string)this[field];
			if(value == null)
				return;

			// check...
			if(value.Length > field.Size)
			{
				// JM - 21-2-2007
				this[field] = value.Substring(0, (int)field.Size);
			}
		}

		/// <summary>
		/// Clones the object.
		/// </summary>
		/// <returns></returns>
		object ICloneable.Clone()
		{
			return this.CloneInternal();
		}

		/// <summary>
		/// Clones the object.
		/// </summary>
		/// <returns>A copy of the entity.</returns>
		protected virtual object CloneInternal()
		{
			Type type = this.GetType();

			// get the object data directly...
			SerializationInfo info = new SerializationInfo(type, new FormatterConverter());
			StreamingContext context = new StreamingContext(StreamingContextStates.Clone);
			this.GetObjectData(info, context);

			// return...
			object clone = Activator.CreateInstance(type, BindingFlags.NonPublic | BindingFlags.Instance, null, new object[] { info, context }, System.Globalization.CultureInfo.InvariantCulture);
			if(clone == null)
				throw new InvalidOperationException("clone is null.");
			return clone;
		}
		
		/// <summary>
		/// Returns true if this entity has entities that reference it.
		/// </summary>
		/// <returns></returns>
		[Obsolete("This method has very poor performance - do not use.")]
		public bool IsUsed()
		{
			return GetChildrenEntities().Count > 0;
		}
		
		/// <summary>
		/// Returns a collection of entities that are direct children to this entity
		/// </summary>
		/// <returns></returns>
		[Obsolete("Removed - please flag to MaBR if breaking change.")]
		public EntityCollection GetChildrenEntities()
		{
			return GetChildrenEntities(false);
		}
		
		/// <summary>
		/// Returns a collection of entities that are children to this entity and its children
		/// </summary>
		/// <returns></returns>
		[Obsolete("Removed - please flag to MaBR if breaking change.")]
		public EntityCollection GetAllChildrenEntities()
		{
			return GetChildrenEntities(true);
		}
		
		/// <summary>
		/// Returns a collection of entities that are children to this entity
		/// </summary>
		/// <param name="walkHierarchy">If true will return every child entity including those 
		/// that are children of this entities children</param>
		/// <returns></returns>
		[Obsolete("Removed - please flag to MaBR if breaking change.")]
		private EntityCollection GetChildrenEntities(bool walkHierarchy)
		{
			EntityCollection children = new EntityCollection();
			
			// Walk each child link and get all the entities
			foreach(ChildToParentEntityLink link in EntityType.ChildLinks)
			{
				// Now we walk each 
				BootFX.Common.Data.SqlFilter filter = new BootFX.Common.Data.SqlFilter(link.EntityType);
				EntityField[] linkFields = link.GetLinkFields();
				for(int index = 0; index < linkFields.Length; index ++)
					filter.Constraints.Add(link.GetLinkFields()[index].Name,SqlOperator.EqualTo, this.GetKeyValues()[index]);
			
				// Lets add the collection of entities to the main list
				EntityCollection childEntities = (EntityCollection) filter.ExecuteEntityCollection();
				children.AddRange(childEntities);
			}
			
			// If we need to walk the tree we do it now
			if(walkHierarchy)
			{
				EntityCollection childrensEntities = new EntityCollection();
				foreach(Entity entity in children)
					childrensEntities.InsertRange(0,entity.GetChildrenEntities(walkHierarchy));
				
				children.InsertRange(0,childrensEntities);
			}
			
			// Lets return them
			return children;
		}

		/// <summary>
		/// Gets the affinitystate.
		/// </summary>
		private object AffinityState
		{
			get
			{
				return _affinityState;
			}
		}

        long IEntityId.Id
        {
            get
            {
                return ConversionHelper.ToInt64(this.EntityType.Storage.GetKeyValues(this)[0]);
            }
        }

        /// <summary>
        /// Gets the original value for a field.
        /// </summary>
        /// <param name="field"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        internal bool GetOriginalValue(EntityField field, ref object value)
		{
			if(field == null)
				throw new ArgumentNullException("field");
			
			// reset...
			value = null;

			// key...
			object key = this.GetOriginalValuesKey(field);
			if(key == null)
				throw new InvalidOperationException("key is null.");

			// contains?
			if(this.OriginalValues.Contains(key))
			{
				value = this.OriginalValues[key];
				return true;
			}
			else
				return false;
		}

		/// <summary>
		/// Raises the <c>AfterLoad</c> event.
		/// </summary>
		// mbr - 02-10-2007 - for c7 - added.		
		internal void OnAfterLoad()
		{
			OnAfterLoad(EventArgs.Empty);
		}
		
		/// <summary>
		/// Raises the <c>AfterLoad</c> event.
		/// </summary>
		// mbr - 02-10-2007 - for c7 - added.		
		protected virtual void OnAfterLoad(EventArgs e)
		{
			// raise...
			if(AfterLoad != null)
				AfterLoad(this, e);
		}

        public void TrimToFitAll()
        {
            foreach(EntityField field in this.EntityType.Fields)
            {
                if (field.IsStringType)
                    this.TrimToFit(field);
            }
        }

        public void MarkForDeletionAndSaveChanges()
        {
            this.MarkForDeletion();
            this.SaveChanges();
        }

        // replace any non-nullable strings that have null values with empty strings....
        public void FixupNonNullableStrings()
        {
            foreach (EntityField field in this.EntityType.Fields)
            {
                if (field.IsStringType && !(field.IsNullable()))
                {
                    var value = (string)this[field];
                    if (value == null)
                        this[field] = string.Empty;
                }
            }
        }

        public static IEnumerable<T> GetByIds<T>(IEnumerable<int> ids)
            where T : Entity
        {
            var et = typeof(T).ToEntityType();
            return et.Persistence.GetByIds<T>(ids);
        }

        public static IEnumerable<T> GetByIds<T>(IEnumerable<long> ids)
            where T : Entity
        {
            var et = typeof(T).ToEntityType();
            return et.Persistence.GetByIds<T>(ids);
        }

        public static Dictionary<int, T> GetByIdsAsDictionary<T>(IEnumerable<int> ids)
            where T : Entity
        {
            var et = typeof(T).ToEntityType();
            return et.Persistence.GetByIdsAsDictionary<T>(ids);
        }

        public static Dictionary<long, T> GetByIdsAsDictionary<T>(IEnumerable<long> ids)
            where T : Entity
        {
            var et = typeof(T).ToEntityType();
            return et.Persistence.GetByIdsAsDictionary<T>(ids);
        }

        public static PagedDataResult<T> GetByIdsPaged<T>(IEnumerable<int> ids, IPagedDataRequestSource source)
            where T : Entity
        {
            var et = typeof(T).ToEntityType();
            return et.Persistence.GetByIdsPaged<T>(ids, source);
        }

        public static PagedDataResult<T> GetByIdsPaged<T>(IEnumerable<long> ids, IPagedDataRequestSource source)
            where T : Entity
        {
            var et = typeof(T).ToEntityType();
            return et.Persistence.GetByIdsPaged<T>(ids, source);
        }

        public static T GetById<T>(int id)
            where T : Entity
        {
            var et = typeof(T).ToEntityType();
            return (T)et.Persistence.GetById(new object[] { id });
        }

        public static T GetById<T>(long id)
            where T : Entity
        {
            var et = typeof(T).ToEntityType();
            return (T)et.Persistence.GetById(new object[] { id });
        }
    }
}
