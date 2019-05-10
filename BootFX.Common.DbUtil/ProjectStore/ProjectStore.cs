// BootFX - Application framework for .NET applications
// 
// File: ProjectStore.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Xml;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Collections;
using System.ComponentModel;

namespace BootFX.Common.DbUtil
{
	/// <summary>
	/// Defines an instance of <c>ProjectStore</c>.
	/// </summary>
	internal abstract class ProjectStore
	{
		/// <summary>
		/// Raised when the <c>CurrentStore</c> property has changed.
		/// </summary>
		[Browsable(true), Category("Property Changed"), Description("Raised when the CurrentStore property has changed.")]
		public static event EventHandler CurrentStoreChanged;
		
		/// <summary>
		/// Private field to support <c>CurrentStore</c> property.
		/// </summary>
		private static ProjectStore _currentStore;
		
		/// <summary>
		/// Private field to support <c>Stores</c> property.
		/// </summary>
		private static ProjectStoreCollection _stores = new ProjectStoreCollection();
		
		private const string ProjectMruListKeyBase = "MruProjects2";

		/// <summary>
		/// Constructor.
		/// </summary>
		internal ProjectStore()
		{
		}

		static ProjectStore()
		{
			Stores.Add(new FileProjectStore());
//			Stores.Add(new DatabaseProjectStore());
		}

		/// <summary>
		/// Gets or sets the currentstore
		/// </summary>
		internal static ProjectStore CurrentStore
		{
			get
			{
				if(_currentStore == null)
					return Stores[0];
				else
					return _currentStore;
			}
			set
			{
				// check to see if the value has changed...
				if(value != _currentStore)
				{
					// set the value...
					_currentStore = value;
					OnCurrentStoreChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Raises the <c>CurrentStoreChanged</c> event.
		/// </summary>
		protected static void OnCurrentStoreChanged(EventArgs e)
		{
			if(CurrentStoreChanged != null)
				CurrentStoreChanged(null, e);
		}

		private string ProjectMruListKey
		{
			get
			{
				return ProjectMruListKeyBase + "\\" + this.GetType().FullName;
			}
		}

		/// <summary>
		/// Gets the MRU items.
		/// </summary>
		/// <returns></returns>
		internal MruList GetMruItems()
		{
            // xml...
            string xml = Runtime.Current.UserSettings.GetStringValue(ProjectMruListKey, null, Cultures.System, OnNotFound.ReturnNull);

            // get...
            MruList list = null;
            if (!(string.IsNullOrEmpty(xml)))
            {
                list = MruList.LoadXml(xml);
                if (list == null)
                    throw new InvalidOperationException("'list' is null.");
            }

            // dod we get it?
			if(list == null)
				list = new MruList(5);

			// return...
			return list;
		}

		/// <summary>
		/// Gets a collection of ProjectStore objects.
		/// </summary>
		internal static ProjectStoreCollection Stores
		{
			get
			{
				return _stores;
			}
		}

		internal void AddMruItem(string path)
		{
			if(path == null)
				throw new ArgumentNullException("path");
			if(path.Length == 0)
				throw new ArgumentOutOfRangeException("'path' is zero-length.");

			// get...
			MruList list = this.GetMruItems();
			if(list == null)
				throw new InvalidOperationException("list is null.");

			// add...
			list.Push(path);

			// save...
            string xml = list.ToXml();
            if (xml == null)
                throw new InvalidOperationException("'xml' is null.");
            if (xml.Length == 0)
                throw new InvalidOperationException("'xml' is zero-length.");
			Runtime.Current.UserSettings.SetValue(ProjectMruListKey, xml);
		}

		/// <summary>
		/// Browses for a project.
		/// </summary>
		/// <param name="owner"></param>
		/// <returns></returns>
		internal abstract string BrowseForProject(Control owner, bool forSave);

		/// <summary>
		/// Saves the file to the store.
		/// </summary>
		/// <param name="filePath"></param>
		/// <param name="xml"></param>
		internal abstract void Save(string filePath, string xml);

		/// <summary>
		/// Returns true if the file is read-only.
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		internal abstract bool IsReadOnly(string path);

		/// <summary>
		/// Loads a file.
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		internal abstract XmlDocument Load(string path);
	}
}
