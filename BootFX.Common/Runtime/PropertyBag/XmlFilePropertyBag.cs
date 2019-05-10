// BootFX - Application framework for .NET applications
// 
// File: XmlFilePropertyBag.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Collections;
using System.Collections.Specialized;
using System.IO;
using System.Xml;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using BootFX.Common.Xml;

namespace BootFX.Common
{
	/// <summary>
	/// Describes a property bag that uses an XML file on disk as a backing store.
	/// </summary>
    // mbr - 2008-12-19 - made this internal because it basically didn't work properly for storing user settings...
	internal class XmlFilePropertyBag : PropertyBag
	{
		/// <summary>
		/// Private field to support <c>DeleteOnFailedLoad</c> property.
		/// </summary>
		private bool _deleteOnFailedLoad = false;
		
		/// <summary>
		/// Private field to support <see cref="FilePath"/> property.
		/// </summary>
		private string _filePath;
		
		/// <summary>
		/// Constructor.
		/// </summary>
		internal XmlFilePropertyBag(string filePath, bool deleteOnFailedLoad)
		{
			if(filePath == null)
				throw new ArgumentNullException("filePath");
			if(filePath.Length == 0)
				throw new ArgumentOutOfRangeException("'filePath' is zero-length.");
			
			_filePath = filePath;
			this.DeleteOnFailedLoad = deleteOnFailedLoad;
		}

		/// <summary>
		/// Gets the filepath.
		/// </summary>
		internal string FilePath
		{
			get
			{
				return _filePath;
			}
		}

		/// <summary>
		/// Gets whether this property bag is read-only.
		/// </summary>
		public override bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// Initializes the bag by loading value from the backing store.
		/// </summary>
		/// <remarks>By default, if the file does not exist, this method will do nothing.</remarks>
		protected override void DoInitialize()
		{
			if(FilePath == null)
				throw new InvalidOperationException("'FilePath' is null.");
			if(FilePath.Length == 0)
				throw new InvalidOperationException("'FilePath' is zero-length.");

			// set up...
			if(File.Exists(this.FilePath))
			{
				try
				{
                    Load(this.FilePath);
				}
				catch(Exception ex)
				{
					if(this.DeleteOnFailedLoad)
					{
						try
						{
							if(Log.IsWarnEnabled)
								Log.Warn(string.Format("Failed to load '{0}'.  The file will be deleted.\r\n\t{1}", this.FilePath, ex));
							File.Delete(this.FilePath);
						}
						catch
						{
                            // no-op...
						}
					}
					else
						throw new InvalidOperationException(string.Format("Failed to load '{0}'.", this.FilePath), ex);
				}
			}
		}

        private void Load(string path)
        {
            if (path == null)
                throw new ArgumentNullException("path");
            if (path.Length == 0)
                throw new ArgumentException("'path' is zero-length.");

            // get...
            XmlDocument doc = new XmlDocument();
            doc.Load(path);

            // load...
            Load(doc);
        }

        private void Load(XmlDocument doc)
        {
            if (doc == null)
                throw new ArgumentNullException("doc");

            // get...
            XmlElement namesElement = (XmlElement)doc.SelectSingleNode("Values/Names");
            if (namesElement == null)
                throw new InvalidOperationException("'element' is null.");
            XmlElement valuesElement = (XmlElement)doc.SelectSingleNode("Values/Values");
            if (valuesElement == null)
                throw new InvalidOperationException("'valuesElement' is null.");

            // walk...
            int index = 0;
            IDictionary loadedValues = new HybridDictionary();
            while (true)
            {
                string name = "Item" + index.ToString();
                string key = XmlHelper.GetElementString(namesElement, name, OnNotFound.ReturnNull);
                if (string.IsNullOrEmpty(key))
                    break;

                // get...
                object value = XmlHelper.GetElementValue(valuesElement, name, null, OnNotFound.ReturnNull);

                // set...
                loadedValues[key] = value;

                // next...
                index++;
            }

            // set...
            this.ReplaceValues(loadedValues);
        }

		/// <summary>
		/// Saves the property bag.
		/// </summary>
		internal void Save()
		{
			this.Save(Encoding.Unicode);
		}

		/// <summary>
		/// Saves the property bag.
		/// </summary>
		internal void Save(Encoding encoding)
		{
			if(encoding == null)
				throw new ArgumentNullException("encoding");
			
			if(FilePath == null)
				throw new InvalidOperationException("'FilePath' is null.");
			if(FilePath.Length == 0)
				throw new InvalidOperationException("'FilePath' is zero-length.");

			// info...
			FileInfo info = new FileInfo(this.FilePath);
			if(info.Directory.Exists == false)
				info.Directory.Create();

			// temp...
            string tempPath = this.FilePath + ".tmp";

            // get an xml document...
            XmlDocument doc = this.ToXml();
            if (doc == null)
                throw new InvalidOperationException("'doc' is null.");
            doc.Save(tempPath);

			// move...
			File.Delete(this.FilePath);
			File.Move(tempPath, this.FilePath);
		}

        /// <summary>
        /// Creates an XML document of the values.
        /// </summary>
        /// <returns></returns>
        private XmlDocument ToXml()
        {
            XmlDocument doc = new XmlDocument();
            XmlElement root = doc.CreateElement("Values");
            doc.AppendChild(root);

            // ok, so we create two buckets...
            XmlElement namesElement = doc.CreateElement("Names");
            root.AppendChild(namesElement);
            XmlElement valuesElement = doc.CreateElement("Values");
            root.AppendChild(valuesElement);

            // walk...
            int index = 0;
            foreach(DictionaryEntry entry in this)
            {
                if (!(entry.Value is MissingItem))
                {
                    string name = "Item" + index.ToString();
                    XmlHelper.AddElement(namesElement, name, entry.Key);
                    XmlHelper.AddElement(valuesElement, name, entry.Value);

                    // next...
                    index++;
                }
            }

            // return...
            return doc;
        }

        /// <summary>
        /// Creates a formatter.
        /// </summary>
        /// <returns></returns>
        private IFormatter CreateFormatter()
		{
			var formatter = new SoapFormatter();
			formatter.AssemblyFormat = FormatterAssemblyStyle.Simple;

			// return...
			return formatter;
		}
		
		/// <summary>
		/// Disposes the object.
		/// </summary>
		/// <param name="disposeType"></param>
		/// <remarks>For this class, the method defers to <see cref="Save"></see>.</remarks>
		protected override void Dispose(DisposeType disposeType)
		{
			// us...
			this.Save();

			// base...
			base.Dispose (disposeType);
		}

		/// <summary>
		/// Gets or sets whether the file should be deleted if loading fails.
		/// </summary>
		/// <remarks>This is a useful setting for transient, non critical settings files.</remarks>
		internal bool DeleteOnFailedLoad
		{
			get
			{
				return _deleteOnFailedLoad;
			}
			set
			{
				// check to see if the value has changed...
				if(value != _deleteOnFailedLoad)
				{
					// set the value...
					_deleteOnFailedLoad = value;
				}
			}
		}

        protected override void OnValueSet(ValueSetEventArgs e)
        {
            base.OnValueSet(e);

            // save...
            this.Save();
        }
	}
}
