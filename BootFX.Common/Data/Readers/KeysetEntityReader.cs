// BootFX - Application framework for .NET applications
// 
// File: KeysetEntityReader.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Text;
using System.Data;
using System.Collections;
using BootFX.Common;
using BootFX.Common.Data.Comparers;
using BootFX.Common.Entities;

namespace BootFX.Common.Data
{
	/// <summary>
	/// Summary description for BufferedEntityReader.
	/// </summary>
	public class KeysetEntityReader : OptimisticEntityReader
	{
		private const int PageSize = 16;

		/// <summary>
		/// Private field to support <c>KeyFields</c> property.
		/// </summary>
		private EntityField _keyField;
		
		/// <summary>
		/// Private field to support <c>CurrentSetPosition</c> property.
		/// </summary>
		private int _currentSetPosition;

		/// <summary>
		/// Private field to support <c>CurrentMasterPosition</c> property.
		/// </summary>
		private int _currentMasterPosition;
		
		/// <summary>
		/// Private field to support <see cref="CurrentSet"/> property.
		/// </summary>
		private IList _currentSet;
		
		/// <summary>
		/// Private field to support <c>Keys</c> property.
		/// </summary>
		private object[][] _keys;
		
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="source"></param>
		public KeysetEntityReader(IEntitySource source, OptimisticReadMode mode)
			: base(source, mode)
		{
		}

		protected override void Initialize()
		{
			if(Source == null)
				throw new InvalidOperationException("Source is null.");

			// check...
			if(EntityType == null)
				throw new InvalidOperationException("EntityType is null.");
			EntityField[] keyFields = this.EntityType.GetKeyFields();
			if(keyFields == null)
				throw new InvalidOperationException("keyFields is null.");
			if(keyFields.Length != 1)
			{
				throw new NotSupportedException(string.Format("Entity type '{0}' has '{1}' key fields, and this object can only be used with entity types that have a single key fields.", 
					this.EntityType, keyFields.Length));
			}

			// set...
			_keyField = keyFields[0];

			// get the keys...
			_keys = this.Source.ExecuteKeyValues();
			if(_keys == null)
				throw new InvalidOperationException("_keys is null.");
		}

		/// <summary>
		/// Gets the keyfields.
		/// </summary>
		private EntityField KeyField
		{
			get
			{
				// returns the value...
				return _keyField;
			}
		}

		/// <summary>
		/// Gets the keys.
		/// </summary>
		private object[][] Keys
		{
			get
			{
				// returns the value...
				return _keys;
			}
		}

		public override bool Read()
		{
			// loop...
			while(true)
			{
				// do we have data?
				if(this.CurrentSet == null)
				{
					this.InitializeCurrentSet();
					if(CurrentSet == null)
						throw new InvalidOperationException("CurrentSet is null.");
				}

				// anything?
				if(this.CurrentSet.Count > 0)
				{
					// current data
					this.SetCurrent(this.CurrentSet[this.CurrentSetPosition]);

					// ok, move us...
					_currentSetPosition++;
					if(this.CurrentSetPosition == this.CurrentSet.Count)
						_currentSet = null;

					// ok...
					return true;
				}
				else
				{
					// current set empty - we have no more data...
					this.SetCurrent(null);
					return false;
				}
			}
		}

		/// <summary>
		/// Gets the currentset.
		/// </summary>
		private IList CurrentSet
		{
			get
			{
				return _currentSet;
			}
		}

		private void InitializeCurrentSet()
		{
			if(Keys == null)
				throw new InvalidOperationException("Keys is null.");
			if(KeyField == null)
				throw new InvalidOperationException("KeyFields is null.");

			// build some ids...
			ArrayList ids = new ArrayList();
			while(this.CurrentMasterPosition < this.Keys.Length && ids.Count < PageSize)
			{
				// add the current master position...
				ids.Add(this.Keys[this.CurrentMasterPosition][0]);

				// next...
				this.CurrentMasterPosition++;
			}

			// do we have anything?
			IList newSet = this.EntityType.CreateCollectionInstance();
			if(ids.Count > 0)
			{
				// create a statement to load that lot...
				SqlFilter filter = new SqlFilter(this.EntityType);

				// if the master defines fields, we'll need the same ones...
				if(this.Source.Fields.Count > 0)
				{
					// add the fields - but we need to know if the key field is in there...
					bool keyFound = false;
					foreach(EntityField field in this.Source.Fields)
					{
						// found it?
						if(field == this.KeyField)
							keyFound = true;

						// add it...
						filter.Fields.Add(field);
					}

					// key?  we'll need that to get the items in the right order...
					if(!(keyFound))
						filter.Fields.Add(this.KeyField);
				}

				// build some sql...
				StringBuilder builder = new StringBuilder();
				builder.Append(filter.Dialect.FormatColumnName(this.KeyField.Name));
				builder.Append(" in (");
				for(int index = 0; index < ids.Count; index++)
				{
					if(index > 0)
						builder.Append(", ");
					builder.Append(filter.Dialect.FormatVariableNameForQueryText(filter.ExtraParameters.Add(this.KeyField.DBType, ids[index])));
				}
				builder.Append(")");

				// add...
				filter.Constraints.AddFreeConstraint(builder.ToString());

				// run it...
				IList newEntities = filter.ExecuteEntityCollection();
				if(newEntities == null)
					throw new InvalidOperationException("newEntities is null.");

				// comparer...
				IComparer comparer = ComparerBase.GetComparer(this.KeyField.DBType, Cultures.System);
				if(comparer == null)
					throw new InvalidOperationException("comparer is null.");

				// we now have to get the data in the right order...
				object[] entities = new object[ids.Count];
				for(int index = 0; index < ids.Count; index++)
				{
					// find it in the data set...
					foreach(object newEntity in newEntities)
					{
						object[] newKeys = this.EntityType.Storage.GetKeyValues(newEntity);
						if(newKeys == null)
							throw new InvalidOperationException("newKeys is null.");
						if(newKeys.Length != 1)
							throw new InvalidOperationException("New keys length is invalid.");

						// check...
						if(comparer.Compare(ids[index], newKeys[0]) == 0)
						{
							entities[index] = newEntity;
							break;
						}
					}
				}

				// create a new set from the ordered set...
				foreach(object entity in entities)
				{
					if(entity != null)
						newSet.Add(entity);
					else
					{
						switch(Mode)
						{
								// do nothing if skip missing...
							case OptimisticReadMode.SkipMissing:
								break;

								// add null in...
							case OptimisticReadMode.ThrowIfMissing:
								throw new InvalidOperationException("Item in keyset could not be found in the database.");

							default:
								throw new NotSupportedException(string.Format("Cannot handle '{0}' ({1}).", Mode, Mode.GetType()));
						}
					}
				}
			}

			// update...
			_currentSet = newSet;

			// position...
			_currentSetPosition = 0;
		}

		/// <summary>
		/// Gets or sets the currentmasterposition
		/// </summary>
		private int CurrentMasterPosition
		{
			get
			{
				return _currentMasterPosition;
			}
			set
			{
				// check to see if the value has changed...
				if(value != _currentMasterPosition)
				{
					// set the value...
					_currentMasterPosition = value;
				}
			}
		}
		
		/// <summary>
		/// Gets or sets the currentsetposition
		/// </summary>
		private int CurrentSetPosition
		{
			get
			{
				return _currentSetPosition;
			}
			set
			{
				// check to see if the value has changed...
				if(value != _currentSetPosition)
				{
					// set the value...
					_currentSetPosition = value;
				}
			}
		}
	}
}
