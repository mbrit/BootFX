// BootFX - Application framework for .NET applications
// 
// File: EntityReader.cs
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
using BootFX.Common.Crypto;
using BootFX.Common.Entities;
using BootFX.Common.Management;

namespace BootFX.Common.Data
{
	/// <summary>
	/// Provides a forward-only (firehose) stream of entities from an <see cref="IDataReader"></see>
	/// </summary>
	// mbr - 22-09-2007 - made internal.	
	internal class EntityReader : IDisposable, IEntityReadState
	{
		/// <summary>
		/// Private field to support <c>Command</c> property.
		/// </summary>
		private IDbCommand _command;
		
		/// <summary>
		/// Private field to support <c>Reader</c> property.
		/// </summary>
		private IDataReader _reader;

		/// <summary>
		/// Private field to support <c>Map</c> property.
		/// </summary>
		private SelectMap _selectMap;

		/// <summary>
		/// Constructor.
		/// </summary>
		private EntityReader(IDbCommand command, IDataReader reader, SelectMap selectMap)
		{
			if(command == null)
				throw new ArgumentNullException("command");
			if(reader == null)
				throw new ArgumentNullException("reader");
			if(selectMap == null)
				throw new ArgumentNullException("selectMap");
			
			_command = command;
			_reader = reader;
			_selectMap = selectMap;
		}

		/// <summary>
		/// Creates a reader from the given connection.
		/// </summary>
		/// <param name="connection"></param>
		/// <param name="statement"></param>
		/// <returns></returns>
		internal static EntityReader CreateEntityReader(Connection connection, SqlStatement statement)
		{
			if(connection == null)
				throw new ArgumentNullException("connection");
			if(statement == null)
				throw new ArgumentNullException("statement");
			if(statement.SelectMap == null)
				throw new InvalidOperationException("Statement does not have a select map.");

            // execute...
            // mbr - 2014-11-30 - caching means we no longer dispose here...
            var command = connection.CreateCommand(statement);
            {
                IDataReader reader = null;
                try
                {
                    // open...
                    connection.EnsureNativeConnectionOpen();

                    // execute...lazy
                    //				ILog log = LogSet.GetLog(typeof(EntityReader));
                    //				if(log.IsInfoEnabled)
                    //					log.Info("About to run entity reader...");
                    reader = command.ExecuteReader();
                    //				if(log.IsInfoEnabled)
                    //					log.Info("\tEntity reader OK.");
                    // create...
                    return new EntityReader(command, reader, statement.SelectMap);
                }
                catch (Exception ex)
                {
                    // create...
                    var newEx = connection.CreateCommandException("Failed to execute entity reader.", command, ex, statement);

                    // dispose...
                    if (reader != null)
                        reader.Dispose();
                    //if (command != null)
                    //    command.Dispose();

                    // throw...
                    throw newEx;
                }
            }
		}

		/// <summary>
		/// Gets the command.
		/// </summary>
		public IDbCommand Command
		{
			get
			{
				return _command;
			}
		}

		/// <summary>
		/// Gets the map.
		/// </summary>
		public SelectMap SelectMap
		{
			get
			{
				return _selectMap;
			}
		}
		
		/// <summary>
		/// Gets the reader.
		/// </summary>  
		internal IDataReader Reader
		{
			get
			{
				return _reader;
			}
		}

		/// <summary>
		/// Disposes the object.
		/// </summary>
		public void Dispose()
		{
			try
			{
				if(_reader != null)
				{
					_reader.Dispose();
					_reader = null;
				}
			}
			catch
			{
				// no-op...
			}

            //try
            //{
            //    if(_command != null)
            //    {
            //        _command.Dispose();
            //        _command = null;
            //    }
            //}
            //catch
            //{
            //    // no-op...
            //}

			// mbr - 10-06-2008 - a suppress finalize was missing...
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Reads the next entity.
		/// </summary>
		/// <returns></returns>
		public object Read()
		{
			// assert...
			if(Reader == null)
				throw new ArgumentNullException("Reader");
			if(SelectMap == null)
				throw new ArgumentNullException("SelectMap");

			// get...
			if(this.Reader.Read() == false)
				return null;

			// mbr - 04-10-2007 - for c7 - provide a lookahead to create inherited types...			
			object newEntity = this.SelectMap.EntityType.CreateInstance(this);
			if(newEntity == null)
				throw new InvalidOperationException("newEntity is null.");

			// get the storage service...
			IEntityStorage storage = this.SelectMap.EntityType.Storage;
			if(storage == null)
				throw new ArgumentNullException("storage");

			// mbr - 10-10-2007 - case 875 - create a lookup that stores other entities in it...
			IDictionary joinedEntities = new HybridDictionary();

			// setup...
			storage.BeginInitialize(newEntity);
			try
			{
				// hydrate...
				foreach(SelectMapField mapField in this.SelectMap.MapFields)
				{
					if(mapField.Field == null)
						throw new InvalidOperationException("mapField.Field is null.");

					// set...
					object value = this.Reader.GetValue(mapField.ResultOrdinal);

                    //// encrypted?
                    //if (mapField.Field.IsEncrypted)
                    //{
                    //    if (value == null)
                    //        value = new EncryptedValue(null, (byte[])null);
                    //    else if (value is DBNull)
                    //        value = new EncryptedValue(typeof(DBNull), (byte[])null);
                    //    else
                    //    {
                    //        // switch...
                    //        switch (mapField.Field.DBType)
                    //        {
                    //            case DbType.String:
                    //            case DbType.StringFixedLength:
                    //            case DbType.AnsiString:
                    //            case DbType.AnsiStringFixedLength:
                    //                value = new EncryptedValue(mapField.Field.Type, (string)value);
                    //                break;

                    //            default:
                    //                throw new NotSupportedException(string.Format("Cannot handle '{0}' ({1}).", mapField.Field.DBType, mapField.Field.DBType.GetType()));
                    //        }
                    //    }
                    //}

                    // mbr - 2016-02-25 - trim the end of any fixed length string...
                    if ((mapField.Field.DBType == DbType.AnsiStringFixedLength || mapField.Field.DBType == DbType.StringFixedLength) && value is string)
                        value = ((string)value).TrimEnd();

					// mbr - 10-10-2007 - case 875 - are we from another entity?
					object toUse = newEntity;
					IEntityStorage storageToUse = storage;
					if(mapField.EntityType != this.SelectMap.EntityType)
					{
						// entity...
						toUse = joinedEntities[mapField.EntityType];
						if(toUse == null)
						{
							// get...
							toUse = mapField.EntityType.CreateInstance();
							if(toUse == null)
								throw new InvalidOperationException("toUse is null.");

							// initialize it...
							mapField.EntityType.Storage.BeginInitialize(toUse);

							// what's the join?
							if(mapField.Join == null)
								throw new InvalidOperationException("mapField.Join is null.");

							// check...
							if(mapField.Join.ChildToParentEntityLink != null)
							{
								// find the link on the entity that has the right foreign name...
								ChildToParentEntityLink toSet = mapField.Join.ChildToParentEntityLink;
//								foreach(ChildToParentEntityLink link in mapField.EntityType.Links)
//								{
//									if(string.Compare(link.NativeName.Name, mapField.Join.ChildToParentEntityLink.NativeName.Name, true,
//										Cultures.System) == 0)
//									{
//										toSet = link;
//										break;
//									}
//								}

								// set...
								if(toSet != null)
									storage.EntityType.Storage.SetParent(newEntity, toSet, toUse);
								else
								{
									throw new InvalidOperationException(string.Format("A link matching name '{0}' was not found.", 
										mapField.Join.ChildToParentEntityLink.NativeName.Name));
								}
							}

							// add...
							joinedEntities[mapField.EntityType] = toUse;
						}

						// storage...
						storageToUse = EntityType.GetEntityType(toUse, OnNotFound.ThrowException).Storage;
						if(storageToUse == null)
							throw new InvalidOperationException("storageToUse is null.");
					}
					else
						toUse = newEntity;

					// set...
					storageToUse.SetValue(toUse, mapField.Field, value, SetValueReason.ReaderLoad);
				}
			}
			finally
			{
				// mbr - 02-10-2007 - for c7 - flipped these around to make the AfterLoad event work (plus it
				storage.ResetIsNew(newEntity);
				storage.EndInitialize(newEntity);

				// mbr - 10-10-2007 - case 875 - fixup joined entities...				
				if(joinedEntities.Count > 0)
				{
					// walk...
					foreach(object joinedEntity in joinedEntities.Values)
					{
						EntityType et = EntityType.GetEntityType(joinedEntity, OnNotFound.ThrowException);
						if(et == null)
							throw new InvalidOperationException("et is null.");

						// storage...
						et.Storage.ResetIsNew(joinedEntity);
						et.Storage.EndInitialize(joinedEntity);
					}
				}
			}

			// return the new entity...
			return newEntity;
		}

		/// <summary>
		/// Gets the current set of values from the reader.
		/// </summary>
		/// <returns></returns>
		public object[] GetValues()
		{
			if(Reader == null)
				throw new InvalidOperationException("Reader is null.");
			
			// get...
			object[] values = new object[this.Reader.FieldCount];
			this.Reader.GetValues(values);

			// return...
			return values;
		}

		/// <summary>
		/// Gets the given value from the reader.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public object GetValue(int index)
		{
			if(Reader == null)
				throw new InvalidOperationException("Reader is null.");

			// return...
			return this.Reader.GetValue(index);
		}
	}
}
