// BootFX - Application framework for .NET applications
// 
// File: DataTableTransformer.cs
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
using System.Globalization;
using System.ComponentModel;

namespace BootFX.Common.Data
{
	/// <summary>
	/// Converts ad hoc data into a DataTable.
	/// </summary>
	public class DataTableTransformer
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		public DataTableTransformer()
		{
		}

		/// <summary>
		/// Transforms the given object into a DataTable.
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		public DataTable Transform(object data)
		{
			// source?
			if(data is IListSource)
				data = ((IListSource)data).GetList();

			// basics...
			if(data == null)
				return this.CreateDataTable();
			if(data is DataTable)
				return (DataTable)data;

			// TODO: what happens here with binding lists and custom property descriptors?
			// should this method be changed to code such as exists in EntityListView?

			// ok, it's something else!
			if(data is NameValueCollection)
				return Transform((NameValueCollection)data);
			if(data is ITypedList)
				return Transform((ITypedList)data);
			if(data is IDictionary)
				return Transform((IDictionary)data);
			if(data is IEnumerable)
				return Transform((IEnumerable)data);

			// we can't do it...
			throw ExceptionHelper.CreateCannotHandleException(data);
		}

		/// <summary>
		/// Transforms the given dictionary.
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		private DataTable Transform(ITypedList data)
		{
			if(data == null)
				throw new ArgumentNullException("data");
			if(!(data is IEnumerable))
				throw new ArgumentException(string.Format("data is of type '{0}', not IEnumerable.", data.GetType()));

			// define a table...
			DataTable table = this.CreateDataTable();

			// get the props...
			PropertyDescriptorCollection properties = data.GetItemProperties(new PropertyDescriptor[] {});
			if(properties == null)
				throw new InvalidOperationException("properties is null.");

			// create columns...
			foreach(PropertyDescriptor property in properties)
			{
				// create...
				DataColumn column = new DataColumn(property.Name, property.PropertyType);
				table.Columns.Add(column);
			}

			// do the rows...
			object[] values = new object[properties.Count];
			foreach(object obj in (IEnumerable)data)
			{
				// get the values...
				for(int index = 0; index < properties.Count; index++)
				{
					// get a value...
					object value = properties[index].GetValue(obj);
					values[index] = value;
				}

				// add...
				table.Rows.Add(values);
			}

			// return...
			return table;
		}

		/// <summary>
		/// Transforms the given dictionary.
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		private DataTable Transform(IEnumerable data)
		{
			if(data == null)
				throw new ArgumentNullException("data");

			// define a table...
			// TODO: localize these strings...
			DataTable table = this.CreateDataTable();
			table.Columns.Add("Item");

			// loop...
			foreach(object obj in data)
				table.Rows.Add(new object[] { obj });

			// return...
			return table;
		}

		/// <summary>
		/// Transforms the given name/value collection.
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		private DataTable Transform(NameValueCollection data)
		{
			if(data == null)
				throw new ArgumentNullException("data");
			
			// define a table...
			// TODO: localize these strings...
			DataTable table = this.CreateDataTable();
			table.Columns.Add("Name");
			table.Columns.Add("Value");

			// loop...
			foreach(string key in data.Keys)
				table.Rows.Add(new object[] { key, data[key] });

			// return...
			return table;
		}

		/// <summary>
		/// Transforms the given dictionary.
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		private DataTable Transform(IDictionary data)
		{
			if(data == null)
				throw new ArgumentNullException("data");
			
			// define a table...
			// TODO: localize these strings...
			DataTable table = this.CreateDataTable();
			table.Columns.Add("Name");
			table.Columns.Add("Value");

			// loop...
			foreach(DictionaryEntry entry in data)
				table.Rows.Add(new object[] { entry.Key, entry.Value });

			// return...
			return table;
		}

		/// <summary>
		/// Creates a new data table.
		/// </summary>
		/// <returns></returns>
		private DataTable CreateDataTable()
		{
			DataTable table = new DataTable();
			table.Locale = CultureInfo.InvariantCulture;

			// return...
			return table;
		}
	}
}
