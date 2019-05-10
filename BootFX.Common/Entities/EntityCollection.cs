// BootFX - Application framework for .NET applications
// 
// File: EntityCollection.cs
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
using System.Data;
using System.Runtime.Serialization;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.ComponentModel;
using BootFX.Common.Xml;
using BootFX.Common.Data;
using BootFX.Common.Management;
using System.Security;

namespace BootFX.Common.Entities
{
    /// <summary>
    ///	 Holds a collection of entities.
    /// </summary>
    /// <remarks>The collection can either be strong, in which case the collection is provided an <see cref="EntityType"></see> instance when it is
    /// created, usually by an extending class.  In this case, <see cref="IsStrongCollection"></see> will return true and <see cref="EntityType"></see> will
    /// be non-null.  If the collection is strong, run-time checking will prevent entities of the wrong type from being added, inserted or set in place.
    /// </remarks>
    /// <example>
    /// <code>
    /// // Describes a collection of customer entities.
    /// public class CustomerCollection : EntityCollection
    /// {
    ///		public CustomerCollection() : base(typeof(Customer)
    ///		{
    ///		}
    /// }
    /// </code>
    /// </example>
    // mbr - 02-12-2005 - removed toxmlbase...	
    public class EntityCollection : Loggable, IList, IListSource, IEnumerable, IEntityCollection, ISerializable
    {
        /// <summary>
        /// Raised when an item is Removed.
        /// </summary>
        public event EntityEventHandler Removed;

        /// <summary>
        /// Raised when the collection is cleared.
        /// </summary>
        public event EventHandler Cleared;

        /// <summary>
        /// Raised when an item is added.
        /// </summary>
        public event EntityEventHandler Added;

        /// <summary>
        /// Private field to support <c>InnerList</c> property.
        /// </summary>
        private ArrayList _innerList = new ArrayList();

        /// <summary>
        /// Private field to support <c>EntityType</c> property.
        /// </summary>
        private EntityType _entityType;

        /// <summary>
        /// Constructor.
        /// </summary>
        public EntityCollection()
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        protected EntityCollection(Type entityType)
            : this(EntityType.GetEntityType(entityType, OnNotFound.ThrowException))
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        protected EntityCollection(EntityType entityType)
        {
            if (entityType == null)
                throw new ArgumentNullException("entityType");

            // set...
            _entityType = entityType;
        }

        protected EntityCollection(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
                throw new ArgumentNullException("info");

            // add...
            _entityType = EntityType.Restore(info, OnNotFound.ThrowException);
            _innerList = (ArrayList)info.GetValue("_innerList", typeof(ArrayList));
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
        /// Returns true if the collection only contains entities of a certain type.
        /// </summary>
        /// <returns></returns>
        public bool IsStrongCollection
        {
            get
            {
                if (this.EntityType == null)
                    return false;
                else
                    return true;
            }
        }

        /// <summary>
        /// Throws an exception if the collection is not strong.
        /// </summary>
        private void AssertIsStrongCollection()
        {
            if (this.IsStrongCollection == false)
                throw new InvalidOperationException("Collection is not strong.");
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets or sets the item at the given index.
        /// </summary>
        object IList.this[int index]
        {
            get
            {
                return this.GetItem(index);
            }
            set
            {
                this.SetItem(index, value);
            }
        }

        /// <summary>
        /// Gets or sets the item at the given index.
        /// </summary>
        protected object GetItem(int index)
        {
            return this.InnerList[index];
        }

        /// <summary>
        /// Gets or sets the item at the given index.
        /// </summary>
        protected void SetItem(int index, object entity)
        {
            this.AssertEntity(entity);
            this.InnerList[index] = entity;
        }

        public void RemoveAt(int index)
        {
            this.InnerList.RemoveAt(index);
        }

        public void Insert(int index, object value)
        {
            this.AssertEntity(value);
            this.InnerList.Insert(index, value);
        }

        public void Remove(object value)
        {
            if (value == null)
                throw new ArgumentNullException("value");
            this.InnerList.Remove(value);
            this.OnRemoved(new EntityEventArgs(value));
        }

        public bool Contains(object value)
        {
            if (value == null)
                throw new ArgumentNullException("value");
            return this.InnerList.Contains(value);
        }

        public void Clear()
        {
            this.InnerList.Clear();
            this.OnCleared();
        }

        public int IndexOf(object value)
        {
            if (value == null)
                throw new ArgumentNullException("value");
            return this.InnerList.IndexOf(value);
        }

        /// <summary>
        /// Adds a range of entities.
        /// </summary>
        /// <param name="entities"></param>
        public void InsertRange(int index, IEnumerable entities)
        {
            if (entities == null)
                throw new ArgumentNullException("entities");

            // walk...
            foreach (object entity in entities)
            {
                this.Insert(index, entity);
                index += 1;
            }
        }

        /// <summary>
        /// Adds a range of entities.
        /// </summary>
        /// <param name="entities"></param>
        public void AddRange(IEnumerable entities)
        {
            if (entities == null)
                throw new ArgumentNullException("entities");

            // walk...
            foreach (object entity in entities)
                this.Add(entity);
        }

        /// <summary>
        /// Adds an item.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        int IList.Add(object value)
        {
            return this.Add(value);
        }

        /// <summary>
        /// Adds an item.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        protected int Add(object entity)
        {
            this.AssertEntity(entity);
            int index = this.InnerList.Add(entity);
            this.OnAdded(new EntityEventArgs(entity));
            return index;
        }

        /// <summary>
        /// Raises the <c>Added</c> event.
        /// </summary>
        protected virtual void OnAdded(EntityEventArgs e)
        {
            // raise...
            if (Added != null)
                Added(this, e);
        }

        /// <summary>
        /// Raises the <c>Removed</c> event.
        /// </summary>
        protected virtual void OnRemoved(EntityEventArgs e)
        {
            // raise...
            if (Removed != null)
                Removed(this, e);
        }

        /// <summary>
        /// Raises the <c>Cleared</c> event.
        /// </summary>
        private void OnCleared()
        {
            OnCleared(EventArgs.Empty);
        }

        /// <summary>
        /// Raises the <c>Cleared</c> event.
        /// </summary>
        protected virtual void OnCleared(EventArgs e)
        {
            // raise...
            if (Cleared != null)
                Cleared(this, e);
        }

        /// <summary>
        /// Asserts that the given entity can be stored in the collection.
        /// </summary>
        /// <param name="entity"></param>
        protected void AssertEntity(object entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");
            if (this.IsStrongCollection == true)
                this.EntityType.AssertIsOfType(entity);
            else
                EntityType.AssertIsEntity(entity);
        }

        public bool IsFixedSize
        {
            get
            {
                // TODO:  Add EntityCollection.IsFixedSize getter implementation
                return false;
            }
        }

        public bool IsSynchronized
        {
            get
            {
                // TODO:  Add EntityCollection.IsSynchronized getter implementation
                return false;
            }
        }

        public int Count
        {
            get
            {
                return this.InnerList.Count;
            }
        }

        /// <summary>
        /// Copies to the given array.
        /// </summary>
        /// <param name="array"></param>
        /// <param name="index"></param>
        public void CopyTo(Array array)
        {
            this.InnerList.CopyTo(array);
        }

        /// <summary>
        /// Copies to the given array.
        /// </summary>
        /// <param name="array"></param>
        /// <param name="index"></param>
        public void CopyTo(Array array, int index)
        {
            this.InnerList.CopyTo(array, index);
        }

        /// <summary>
        /// Copies to the given array.
        /// </summary>
        /// <param name="array"></param>
        /// <param name="index"></param>
        public void CopyTo(int index, Array array, int arrayIndex, int count)
        {
            this.InnerList.CopyTo(index, array, arrayIndex, count);
        }

        public object SyncRoot
        {
            get
            {
                // TODO:  Add EntityCollection.SyncRoot getter implementation
                return null;
            }
        }

        /// <summary>
        /// Gets an enumerator for the list.
        /// </summary>
        /// <returns></returns>
        public IEnumerator GetEnumerator()
        {
            return this.InnerList.GetEnumerator();
        }

        /// <summary>
        /// Gets the innerlist.
        /// </summary>
        protected ArrayList InnerList
        {
            get
            {
                return _innerList;
            }
        }

        /// <summary>
        /// Gets the list for this collection.
        /// </summary>
        /// <returns></returns>
        /// <remarks>To access this from a derived class, use <c>GetDefaultView</c>.</remarks>
        IList IListSource.GetList()
        {
            return this.GetDefaultView();
        }

        /// <summary>
        /// Gets the default view.
        /// </summary>
        public EntityViewCollection GetDefaultView()
        {
            if (EntityType == null)
                throw new ArgumentNullException("EntityType");

            // add...
            ArrayList members = new ArrayList();

            // TODO: add only fields and child-to-parent links...
            members.AddRange(this.EntityType.GetMembers());

            // return...
            return this.GetView(members);
        }

        /// <summary>
        /// Gets the default field view ignoring any large fields
        /// </summary>
        public EntityViewCollection GetDefaultFieldView()
        {
            if (EntityType == null)
                throw new ArgumentNullException("EntityType");

            // add...
            ArrayList members = new ArrayList();

            // TODO: add only fields and child-to-parent links...
            foreach (EntityField field in this.EntityType.Fields)
                if (!field.IsLarge() && !field.HasEnumerationType)
                    members.Add(field);

            // return...
            return this.GetView(members);
        }

        /// <summary>
        /// Gets the default view.
        /// </summary>
        public EntityViewCollection GetView(IList expressions)
        {
            if (expressions == null)
                throw new ArgumentNullException("expressions");

            return new EntityViewCollection(this, expressions);
        }

        /// <summary>
        /// Gets the default view.
        /// </summary>
        public EntityViewCollection GetView(params object[] expressions)
        {
            if (expressions == null)
                throw new ArgumentNullException("expressions");

            return new EntityViewCollection(this, expressions);
        }

        /// <summary>
        /// Gets the view with the given expression.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public EntityViewCollection GetView(string expression)
        {
            if (expression == null)
                throw new ArgumentNullException("expression");
            if (expression.Length == 0)
                throw ExceptionHelper.CreateZeroLengthArgumentException("expression");

            // defer...
            return this.GetView(new object[] { expression });
        }

        /// <summary>
        /// Returns true if the list contains a collection.
        /// </summary>
        /// <remarks>This is no alternative access 'method' for this property.  This method returns false.  This method means that if the collection
        /// is a collection of collections (e.g. a DataSet containing many DataTable instances), this would return 'true'.  As it is just a list, this returns
        /// 'false'.</remarks>
        bool IListSource.ContainsListCollection
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the items in the collection as an array.
        /// </summary>
        /// <returns></returns>
        /// <remarks>To access this from a derived class, use <c>ToArrayInternal</c>.</remarks>
        object[] IEntityCollection.ToArray()
        {
            return this.ToArrayInternal();
        }

        /// <summary>
        /// Gets the items in the collection as an array.
        /// </summary>
        /// <returns></returns>
        /// <remarks>If the collection is a strong collection, the array returned will be an array of the strong types, otherwise
        /// it will be a vanilla object array.</remarks>
        protected object[] ToArrayInternal()
        {
            // create...
            Type type = typeof(object);
            if (this.EntityType != null)
                type = this.EntityType.Type;

            // return...
            return (object[])this.InnerList.ToArray(type);
        }

        /// <summary>
        /// Saves changes to this list.
        /// </summary>
        public void SaveChanges()
        {
            EntityPersistenceHelper.Current.SaveChanges(this);
        }

        /// <summary>
        /// Copies the values in this collection to the given array.
        /// </summary>
        /// <param name="items"></param>
        /// <param name="index"></param>
        public void CopyTo(object[] items, int index)
        {
            if (items == null)
                throw new ArgumentNullException("items");
            this.InnerList.CopyTo(items, index);
        }

        /// <summary>
        /// Sorts the list using the given comparer.
        /// </summary>
        /// <param name="comparer"></param>
        public void Sort(string memberName, CultureInfo culture)
        {
            this.Sort(memberName, culture, SortDirection.Ascending);
        }

        /// <summary>
        /// Sorts the list using the given comparer.
        /// </summary>
        /// <param name="comparer"></param>
        public void Sort(string memberName, SortDirection direction = SortDirection.Ascending)
        {
            this.Sort(memberName, Cultures.System, direction);
        }

        /// <summary>
        /// Sorts the list using the given comparer.
        /// </summary>
        /// <param name="comparer"></param>
        public void Sort(string memberName, CultureInfo culture, SortDirection direction)
        {
            if (memberName == null)
                throw new ArgumentNullException("memberName");
            if (memberName.Length == 0)
                throw new ArgumentOutOfRangeException("'memberName' is zero-length.");

            // check...
            this.AssertIsStrongCollection();

            // get...
            EntityMember member = this.EntityType.GetMember(memberName, OnNotFound.ThrowException);
            if (member == null)
                throw new InvalidOperationException("member is null.");
            if (member is EntityField)
                this.Sort((EntityField)member, culture, direction);
            else
                throw new NotSupportedException(string.Format("Cannot handle '{0}'.", member));
        }

        /// <summary>
        /// Sorts the list using the given comparer.
        /// </summary>
        /// <param name="comparer"></param>
        public void Sort(EntityField field, CultureInfo culture)
        {
            this.Sort(field, culture, SortDirection.Ascending);
        }

        /// <summary>
        /// Sorts the list using the given comparer.
        /// </summary>
        /// <param name="comparer"></param>
        public void Sort(EntityField field, SortDirection direction = SortDirection.Ascending)
        {
            this.Sort(field, Cultures.System, direction);
        }

        /// <summary>
        /// Sorts the list using the given comparer.
        /// </summary>
        /// <param name="comparer"></param>
        public void Sort(EntityField field, CultureInfo culture, SortDirection direction)
        {
            if (field == null)
                throw new ArgumentNullException("field");

            // check...
            this.AssertIsStrongCollection();

            // get it...
            IComparer comparer = field.GetComparer(culture);
            this.Sort(comparer, direction);
        }

        /// <summary>
        /// Sorts the list using the given comparer.
        /// </summary>
        /// <param name="comparer"></param>
        public void Sort(IComparer comparer)
        {
            this.Sort(comparer, SortDirection.Ascending);
        }

        /// <summary>
        /// Sorts the list using the given comparer.
        /// </summary>
        /// <param name="comparer"></param>
        public void Sort(IComparer comparer, SortDirection direction)
        {
            if (comparer == null)
                throw new ArgumentNullException("comparer");

            IComparerInitialize init = comparer as IComparerInitialize;
            if (init != null)
                init.StartCompare(this);
            try
            {
                // wrap?
                IComparer toUse = comparer;
                if (direction == SortDirection.Descending)
                    toUse = new ComparerReverser(comparer);

                // sort...
                this.InnerList.Sort(toUse);
            }
            finally
            {
                if (init != null)
                    init.EndCompare();
            }
        }

        /// <summary>
        /// Sorts the list using the given comparer.
        /// </summary>
        /// <param name="comparer"></param>
        public void Sort(int index, int count, EntityField field, CultureInfo culture)
        {
            if (field == null)
                throw new ArgumentNullException("field");

            // check...
            this.AssertIsStrongCollection();

            // get it...
            IComparer comparer = field.GetComparer(culture);
            this.Sort(index, count, comparer);
        }

        /// <summary>
        /// Sorts the list using the given comparer.
        /// </summary>
        /// <param name="comparer"></param>
        public void Sort(int index, int count, IComparer comparer)
        {
            if (comparer == null)
                throw new ArgumentNullException("comparer");

            // sort...
            this.InnerList.Sort(index, count, comparer);
        }

        /// <summary>
        /// Gets the entity collection as a data set.
        /// </summary>
        /// <returns></returns>
        public DataSet ToDataSet()
        {
            DataSet dataSet = new DataSet();
            dataSet.Locale = Cultures.System;
            dataSet.Tables.Add(this.ToDataTable());

            // return...
            return dataSet;
        }

        /// <summary>
        /// Gets the entity collection as a data table.
        /// </summary>
        /// <returns></returns>
        public DataTable ToDataTable()
        {
            // get a view that just contains fields (i.e. don't include parent link entities)...
            if (EntityType == null)
                throw new InvalidOperationException("EntityType is null.");
            EntityViewCollection view = this.GetView(this.EntityType.Fields);

            // create a transformer...
            DataTableTransformer transformer = new DataTableTransformer();

            // transform...
            return transformer.Transform(view);
        }

        [SecurityCritical]
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            this.GetObjectData(info, context);
        }

        protected virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
                throw new ArgumentNullException("info");

            // add...
            if (EntityType == null)
                throw new InvalidOperationException("EntityType is null.");
            this.EntityType.Store(info);
            info.AddValue("_innerList", _innerList);
        }

        /// <summary>
        /// Gets the entity as an XML string.
        /// </summary>
        /// <returns></returns>
        [Obsolete("This method has been removed.  There is no direct replacement within the framework.  Ask MaBR fo more information.")]
        public string ToXml()
        {
            XmlDocument doc = this.ToXmlDocument();
            if (doc == null)
                throw new InvalidOperationException("doc is null.");

            // return...
            using (StringWriter writer = new StringWriter())
            {
                doc.Save(writer);

                // return...
                writer.Flush();
                return writer.GetStringBuilder().ToString();
            }
        }

        /// <summary>
        /// Gets the items as an XML document.
        /// </summary>
        /// <returns></returns>
        [Obsolete("This method has been removed.  There is no direct replacement within the framework.  Ask MaBR fo more information.")]
        public XmlDocument ToXmlDocument()
        {
            XmlDocument doc = new XmlDocument();
            XmlNamespaceManager manager = XmlHelper.GetNamespaceManager(doc);

            // create...
            if (EntityType == null)
                throw new InvalidOperationException("EntityType is null.");
            XmlElement root = doc.CreateElement(this.EntityType.Name + "Items");
            doc.AppendChild(root);

            // populate...
            foreach (Entity entity in this.InnerList)
            {
                XmlElement element = entity.ToXmlElement(doc, manager);
                if (element == null)
                    throw new InvalidOperationException("element is null.");

                // add...
                root.AppendChild(element);
            }

            // return...
            return doc;
        }

        protected IEnumerator<T> GetEnumerator<T>()
            where T : Entity
        {
            return this.ToList<T>().GetEnumerator();
        }

        public List<T> ToList<T>()
        {
            var results = new List<T>();
            foreach (T item in this.InnerList)
                results.Add(item);
            return results;
        }

        public void SortById(SortDirection direction = SortDirection.Ascending)
        {
            this.Sort(this.EntityType.GetKeyFields()[0], direction);
        }

        public int IndexOfId(int id)
        {
            return this.IndexOfId(ConversionHelper.ToInt64(id));
        }

        private int IndexOfId(long id)
        {
            var keyField = this.EntityType.GetKeyFields()[0];
            for (int index = 0; index < this.Count; index++)
            {
                var entity = this.InnerList[index];
                var found = ConversionHelper.ToInt64(this.EntityType.Storage.GetValue(entity, keyField));
                if (found == id)
                    return index;
            }

            return -1;
        }

        public bool ContainsId(int id)
        {
            return this.IndexOfId(id) != -1;
        }

        public bool ContainsId(long id)
        {
            return this.IndexOfId(id) != -1;
        }

        public bool ContainsId(Entity entity)
        {
            if (entity.EntityType != this.EntityType)
                throw new ArgumentException("Entity type mismatch.");

            var id = ConversionHelper.ToInt64(this.EntityType.Storage.GetKeyValues(entity)[0]);
            return this.ContainsId(id);
        }

        public void Merge(Entity entity)
        {
            if (!(this.ContainsId(entity)))
                this.Add(entity);
        }

        public void Merge(IEnumerable<Entity> entities)
        {
            foreach (var entity in entities)
                this.Merge(entity);
        }
    }
}
