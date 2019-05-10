// BootFX - Application framework for .NET applications
// 
// File: EntityView.cs
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
using System.Collections.Specialized;
using System.ComponentModel;
using BootFX.Common.Data;

namespace BootFX.Common.Entities
{
	/// <summary>
	/// Defines a view on an entity.  This is a corollory to <see cref="System.Data.DataRowView"></see>.
	/// </summary>
	public class EntityView : IEditableObject, IInEdit, IEntityType, ICustomTypeDescriptor
	{
		/// <summary>
		/// Private field to support <c>Deleted</c> property.
		/// </summary>
		private bool _deleted;
		
		/// <summary>
		/// Private field to support <c>InEdit</c> property.
		/// </summary>
		private bool _inEdit = false;
		
		/// <summary>
		/// Raised when the <c>InEdit</c> property has changed.
		/// </summary>
		[Browsable(true), Category("Property Changed"), Description("Raised when the InEdit property has changed.")]
		public event EventHandler InEditChanged;
		
		/// <summary>
		/// Private field to support <c>Values</c> property.
		/// </summary>
		private IDictionary _values;
		
		/// <summary>
		/// Private field to support <c>EntityType</c> property.
		/// </summary>
		private EntityType _entityType;

		/// <summary>
		/// Private field to support <c>Entity</c> property.
		/// </summary>
		private object _entity;
		
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="view"></param>
		/// <param name="entity"></param>
		public EntityView(object entity)
		{
			if(entity == null)
				throw new ArgumentNullException("entity");
			
			_entity = entity;
			this.ResetValues();

			// mark the entity as deleted if it is deleted...
			if(Storage == null)
				throw new InvalidOperationException("Storage is null.");
			_deleted = Storage.IsDeleted(entity, true);
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
		/// Called when editing is committed.
		/// </summary>
		public void EndEdit()
		{
			this.PushValues();
			this.FinishEdit();
		}

		/// <summary>
		/// Called when editing is cancelled.
		/// </summary>
		public void CancelEdit()
		{
			this.FinishEdit();
		}

		/// <summary>
		/// Called when editing begins.
		/// </summary>
		public void BeginEdit()
		{
			this.SetInEdit(true);
		}

		/// <summary>
		/// Finishes editing the item.
		/// </summary>
		private void FinishEdit()
		{
			this.ResetValues();
			this.SetInEdit(false);
		}

		/// <summary>
		/// Gets the value for the given field.
		/// </summary>
		/// <param name="field"></param>
		/// <returns></returns>
		public void SetValue(EntityMember member, object value)
		{
			if(member == null)
				throw new ArgumentNullException("member");

			// set...
			object key = this.GetKey(member);
			this.Values[key] = value;
		}

		/// <summary>
		/// Gets the value for the given field.
		/// </summary>
		/// <param name="field"></param>
		/// <returns></returns>
		public object GetValue(EntityMember member)
		{
			if(member == null)
				throw new ArgumentNullException("member");

			// in the cache?
			object key = this.GetKey(member);
			object value = null;
			if(this.Values.Contains(key) == false)
			{
				value = this.PullValue(member);
				this.Values[key] = value;
			}
			else
				value = this.Values[key];

			// return...
			return value;
		}

		/// <summary>
		/// Resets the cache.
		/// </summary>
		private void ResetValues()
		{
			_values = new HybridDictionary();
		}

		/// <summary>
		/// Gets the key for the given field.
		/// </summary>
		/// <param name="field"></param>
		/// <returns></returns>
		private object GetKey(EntityMember member)
		{
			if(member == null)
				throw new ArgumentNullException("member");
			
			// just return it...
			return member;
		}

		/// <summary>
		/// Pulls the value from the entity.
		/// </summary>
		/// <param name="field"></param>
		/// <returns></returns>
		private object PullValue(EntityMember member)
		{
			if(member == null)
				throw new ArgumentNullException("member");
			
			if(member is EntityField)
				return this.PullValue((EntityField)member);
			if(member is ChildToParentEntityLink)
				return this.PullValue((ChildToParentEntityLink)member);
			else
				throw new NotSupportedException(string.Format(Cultures.Exceptions, "Cannot handle '{0}'.", member.GetType()));
		}

		/// <summary>
		/// Pulls the value from the entity.
		/// </summary>
		/// <param name="field"></param>
		/// <returns></returns>
		private object PullValue(ChildToParentEntityLink link)
		{
			if(link == null)
				throw new ArgumentNullException("link");

			// check...
			if(Entity == null)
				throw new ArgumentNullException("Entity");

			// get persistence...
			IEntityStorage storage = this.Storage;
			if(storage == null)
				throw new ArgumentNullException("storage");

			// get...
			return storage.GetParent(this.Entity, link);
		}

		/// <summary>
		/// Pulls the value from the entity.
		/// </summary>
		/// <param name="field"></param>
		/// <returns></returns>
		private object PullValue(EntityField field)
		{
			if(field == null)
				throw new ArgumentNullException("field");

			// check...
			if(Entity == null)
				throw new ArgumentNullException("Entity");

			// get storage...
			IEntityStorage storage = this.Storage;
			if(storage == null)
				throw new ArgumentNullException("storage");

			// get...
			return storage.GetValue(this.Entity, field);
		}

		/// <summary>
		/// Gets the entity type.
		/// </summary>
		public EntityType EntityType
		{
			get
			{
				EnsureEntityTypeCreated();
				return _entityType;
			}
		}
		
		/// <summary>
		/// Returns  EntityType.
		/// </summary>
		private bool IsEntityTypeCreated()
		{
			if(_entityType == null)
				return false;
			else
				return true;
		}
		
		/// <summary>
		/// Ensures that EntityType has been created.
		/// </summary>
		private void EnsureEntityTypeCreated()
		{
			if(IsEntityTypeCreated() == false)
				_entityType = CreateEntityType();
		}
		
		/// <summary>
		/// Creates an instance of EntityType.
		/// </summary>
		/// <remarks>This does not assign the instance to the _entityType field</remarks>
		private EntityType CreateEntityType()
		{
			if(Entity == null)
				throw new ArgumentNullException("Entity");
			return EntityType.GetEntityType(this.Entity, OnNotFound.ThrowException);
		}

		/// <summary>
		/// Gets storage.
		/// </summary>
		private IEntityStorage Storage
		{
			get
			{
				if(EntityType == null)
					throw new ArgumentNullException("EntityType");
				return this.EntityType.Storage;
			}
		}

		/// <summary>
		/// Gets the values.
		/// </summary>
		private IDictionary Values
		{
			get
			{
				// returns the value...
				return _values;
			}
		}

		/// <summary>
		/// Pushes values back to the entity.
		/// </summary>
		private void PushValues()
		{
			if(EntityType == null)
				throw new ArgumentNullException("EntityType");
			if(Entity == null)
				throw new ArgumentNullException("Entity");
			if(Storage == null)
				throw new InvalidOperationException("Storage is null.");

			// deleted?  don't worry with the update if we are...
			if(this.Deleted == false)
			{
				// walk...
				foreach(DictionaryEntry entry in this.Values)
				{
					if(entry.Key == null)
						throw new ArgumentNullException("entry.Key");
				
					// defer...
					if(entry.Key is EntityField)
						this.PushValue((EntityField)entry.Key, entry.Value);
					else if(entry.Key is ChildToParentEntityLink)
						this.PushValue((ChildToParentEntityLink)entry.Key, entry.Value);
					else
						throw new NotSupportedException(string.Format(Cultures.Exceptions, "Cannot handle '{0}'.", entry.Key.GetType()));
				}
			}
			else
			{
				// push the deleted flag...
				Storage.MarkForDeletion(this.Entity);
			}
		}

		/// <summary>
		/// Pushes a value back down to the underlying field.
		/// </summary>
		/// <param name="field"></param>
		/// <param name="value"></param>
		private void PushValue(ChildToParentEntityLink link, object value)
		{
			if(link == null)
				throw new ArgumentNullException("link");

			// get storage...
			IEntityStorage storage = this.Storage;
			if(storage == null)
				throw new ArgumentNullException("storage");

			// set...
			storage.SetParent(this.Entity, link, value);
		}

		/// <summary>
		/// Pushes a value back down to the underlying field.
		/// </summary>
		/// <param name="field"></param>
		/// <param name="value"></param>
		private void PushValue(EntityField field, object value)
		{
			if(field == null)
				throw new ArgumentNullException("field");

			// get storage...
			IEntityStorage storage = this.Storage;
			if(storage == null)
				throw new ArgumentNullException("storage");

			// get...
			storage.SetValue(this.Entity, field, value, SetValueReason.UserSet);
		}

		/// <summary>
		/// Raises the <c>InEditChanged</c> event.
		/// </summary>
		private void OnInEditChanged()
		{
			OnInEditChanged(EventArgs.Empty);
		}
		
		/// <summary>
		/// Raises the <c>InEditChanged</c> event.
		/// </summary>
		protected virtual void OnInEditChanged(EventArgs e)
		{
			if(InEditChanged != null)
				InEditChanged(this, e);
		}

		/// <summary>
		/// Gets the inedit.
		/// </summary>
		public bool InEdit
		{
			get
			{
				return _inEdit;
			}
		}

		/// <summary>
		/// Sets whether we are editing.
		/// </summary>
		/// <param name="inEdit"></param>
		private void SetInEdit(bool inEdit)
		{
			if(_inEdit == inEdit)
				return;

			// set...
			_inEdit = inEdit;
			this.OnInEditChanged();
		}

		/// <summary>
		/// Gets the string represention of the entity for the user's culture.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return this.ToString(Cultures.User);
		}

		/// <summary>
		/// Gets the string representation of the entity for the given culture.
		/// </summary>
		/// <param name="formatProvider"></param>
		/// <returns></returns>
		public string ToString(IFormatProvider formatProvider)
		{
			if(Entity == null)
				throw new ArgumentNullException("Entity");

			// does it?
			if(this.Entity is IFormattableToString)
				return ((IFormattableToString)this.Entity).ToString(formatProvider);
			else
				return Convert.ToString(this.Entity, formatProvider);
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
		/// Unwraps the given <c>EntityView</c> instance, or returns the object if it's not an entity view instance.
		/// </summary>
		/// <param name="entity"></param>
		/// <returns></returns>
		public static object Unwrap(object entity)
		{
			EntityView view = entity as EntityView;
			if(view != null)
				return view.Entity;
			else
				return entity;
		}

		/// <summary>
		/// Gets the deleted.
		/// </summary>
		public bool Deleted
		{
			get
			{
				return _deleted;
			}
		}

		/// <summary>
		/// Marks the entity as being deleted.
		/// </summary>
		public void MarkForDeletion()
		{
			_deleted = true;
		}

//		/// <summary>
//		/// Implementation for <see cref="ICustomTypeDescriptor"></see>.
//		/// </summary>
//		/// <returns>This method always returns null.  See implementation of <see cref="DataRowView"></see>.</returns>
//		TypeConverter ICustomTypeDescriptor.GetConverter()
//		{
//			return null;
//		}
//
//		/// <summary>
//		/// Implementation for <see cref="ICustomTypeDescriptor"></see>.
//		/// </summary>
//		/// <returns>This method always returns null.  See implementation of <see cref="DataRowView"></see>.</returns>
//		EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes)
//		{
//			return null;
//		}
//
//		/// <summary>
//		/// Implementation for <see cref="ICustomTypeDescriptor"></see>.
//		/// </summary>
//		/// <returns>This method always returns null.  See implementation of <see cref="DataRowView"></see>.</returns>
//		EventDescriptorCollection System.ComponentModel.ICustomTypeDescriptor.GetEvents()
//		{
//			return null;
//		}
//
//		/// <summary>
//		/// Implementation for <see cref="ICustomTypeDescriptor"></see>.
//		/// </summary>
//		/// <returns>This method always returns null.  See implementation of <see cref="DataRowView"></see>.</returns>
//		string ICustomTypeDescriptor.GetComponentName()
//		{
//			return null;
//		}
//
//		/// <summary>
//		/// Implementation for <see cref="ICustomTypeDescriptor"></see>.
//		/// </summary>
//		/// <returns>This method always returns null.  See implementation of <see cref="DataRowView"></see>.</returns>
//		object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd)
//		{
//			return null;
//		}
//
//		/// <summary>
//		/// Implementation for <see cref="ICustomTypeDescriptor"></see>.
//		/// </summary>
//		/// <returns>This method always returns null.  See implementation of <see cref="DataRowView"></see>.</returns>
//		AttributeCollection ICustomTypeDescriptor.GetAttributes()
//		{
//			return null;
//		}
//
//		/// <summary>
//		/// Implementation for <see cref="ICustomTypeDescriptor"></see>.
//		/// </summary>
//		/// <returns>This method always returns null.  See implementation of <see cref="DataRowView"></see>.</returns>
//		PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes)
//		{
//			return ((ICustomTypeDescriptor)this).GetProperties(new Attribute[] {});
//		}
//
//		/// <summary>
//		/// Implementation for <see cref="ICustomTypeDescriptor"></see>.
//		/// </summary>
//		/// <returns>This method always returns null.  See implementation of <see cref="DataRowView"></see>.</returns>
//		PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
//		{
//			if(EntityType == null)
//				throw new InvalidOperationException("EntityType is null.");
//			return EntityType.GetPropertyDescriptors();
//		}
//
//		/// <summary>
//		/// Implementation for <see cref="ICustomTypeDescriptor"></see>.
//		/// </summary>
//		/// <returns>This method always returns null.  See implementation of <see cref="DataRowView"></see>.</returns>
//		object ICustomTypeDescriptor.GetEditor(Type editorBaseType)
//		{
//			// TODO: This method has not been implemented.  Check whether it is a valid not implementated, or a code path that was not completed.
//			throw new NotImplementedException(string.Format("Not implemented for '{0}'.", this.GetType()));
//		}
//
//		/// <summary>
//		/// Implementation for <see cref="ICustomTypeDescriptor"></see>.
//		/// </summary>
//		/// <returns>This method always returns null.  See implementation of <see cref="DataRowView"></see>.</returns>
//		PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty()
//		{
//			return null;
//		}
//
//		/// <summary>
//		/// Implementation for <see cref="ICustomTypeDescriptor"></see>.
//		/// </summary>
//		/// <returns>This method always returns null.  See implementation of <see cref="DataRowView"></see>.</returns>
//		EventDescriptor ICustomTypeDescriptor.GetDefaultEvent()
//		{
//			return null;
//		}
//
//		/// <summary>
//		/// Implementation for <see cref="ICustomTypeDescriptor"></see>.
//		/// </summary>
//		/// <returns>This method always returns null.  See implementation of <see cref="DataRowView"></see>.</returns>
//		string ICustomTypeDescriptor.GetClassName()
//		{
//			return null;
//		}

		public TypeConverter GetConverter()
		{
			return null;
		}

		public EventDescriptorCollection GetEvents(Attribute[] attributes)
		{
			return null;
		}

		EventDescriptorCollection System.ComponentModel.ICustomTypeDescriptor.GetEvents()
		{
			return this.GetEvents(new Attribute[] {});
		}

		public string GetComponentName()
		{
			return this.EntityType.Name;
		}

		public object GetPropertyOwner(PropertyDescriptor pd)
		{
			return null;
		}

		public AttributeCollection GetAttributes()
		{
			return null;
		}

		public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
		{
			if(EntityType == null)
				throw new InvalidOperationException("EntityType is null.");

			// get the members...
			EntityMember[] members = this.EntityType.GetMembers();
			if(members == null)
				throw new InvalidOperationException("members is null.");

			// loop...
			PropertyDescriptor[] properties = new PropertyDescriptor[members.Length];
			for(int index = 0; index < members.Length; index++)
				properties[index] = members[index].GetPropertyDescriptor();

			// return...
			return new PropertyDescriptorCollection(properties);
		}

		PropertyDescriptorCollection System.ComponentModel.ICustomTypeDescriptor.GetProperties()
		{
			return this.GetProperties(new Attribute[] {});
		}

		public object GetEditor(Type editorBaseType)
		{
			return null;
		}

		public PropertyDescriptor GetDefaultProperty()
		{
			return null;
		}

		public EventDescriptor GetDefaultEvent()
		{
			return null;
		}

		public string GetClassName()
		{
			if(EntityType == null)
				throw new InvalidOperationException("EntityType is null.");
			return this.EntityType.Type.FullName;
		}
	}
}
