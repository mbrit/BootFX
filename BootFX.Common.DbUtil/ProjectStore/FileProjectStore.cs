// BootFX - Application framework for .NET applications
// 
// File: FileProjectStore.cs
// Build: 5.0.61009.900
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
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Collections;

namespace BootFX.Common.DbUtil
{
	/// <summary>
	/// Defines an instance of <c>FileProjectStore</c>.
	/// </summary>
	internal class FileProjectStore : ProjectStore
	{
		/// <summary>
		/// Defines the project file filter.
		/// </summary>
		private string ProjectFileFilter = "DBUtil Project XML Files (*.xml)|*.xml|All Files (*.*)|*.*||";

		internal FileProjectStore()
		{
		}

		internal override string BrowseForProject(System.Windows.Forms.Control owner, bool forSave)
		{
			// open...
			FileDialog dialog = null;
			if(forSave)
				dialog = new SaveFileDialog();
			else
				dialog = new OpenFileDialog();
			using(dialog)
			{
				dialog.Filter = ProjectFileFilter;
				if(dialog.ShowDialog(owner) == DialogResult.OK)
					return dialog.FileName;
				else
					return null;
			}
		}

		/// <summary>
		/// Saves the file to the store.
		/// </summary>
		/// <param name="filePath"></param>
		/// <param name="xml"></param>
		internal override void Save(string filePath, string xml)
		{
			if(filePath == null)
				throw new ArgumentNullException("filePath");
			if(filePath.Length == 0)
				throw new ArgumentOutOfRangeException("'filePath' is zero-length.");
			if(xml == null)
				throw new ArgumentNullException("xml");
			if(xml.Length == 0)
				throw new ArgumentOutOfRangeException("'xml' is zero-length.");

			// save...
			using(StreamWriter writer = new StreamWriter(filePath, false))
				writer.Write(xml);
		}

		internal override bool IsReadOnly(string path)
		{
			if(path == null)
				throw new ArgumentNullException("path");
			if(path.Length == 0)
				throw new ArgumentOutOfRangeException("'path' is zero-length.");
			
			// check...
			if(File.Exists(path) && (File.GetAttributes(path) & FileAttributes.ReadOnly) != 0)
				return true;
			else
				return false;
		}

		internal override System.Xml.XmlDocument Load(string path)
		{
			if(path == null)
				throw new ArgumentNullException("path");
			if(path.Length == 0)
				throw new ArgumentOutOfRangeException("'path' is zero-length.");
			
			// create...
			XmlDocument doc = new XmlDocument();
			doc.Load(path);

			// return...
			return doc;
		}
	}
}
