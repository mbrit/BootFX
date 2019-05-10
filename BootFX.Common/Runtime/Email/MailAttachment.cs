// BootFX - Application framework for .NET applications
// 
// File: MailAttachment.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using System.IO;
using System.Xml;
using BootFX.Common.Xml;

namespace BootFX.Common.Email
{
	/// <summary>
	/// Defines an instance of <c>MailAttachment</c>.
	/// </summary>
	public class MailAttachment
	{
		/// <summary>
		/// Private field to support <c>ContentId</c> property.
		/// </summary>
		private string _contentId;
		
		/// <summary>
		/// Private field to support <see cref="filePath"/> property.
		/// </summary>
		private string _filePath;
        private string _fileName;

        public MailAttachment(string filePath)
            : this(filePath, Path.GetFileName(filePath))
        {
		}

        public MailAttachment(string filePath, string fileName)
        {
            if (filePath == null)
                throw new ArgumentNullException("filePath");
            if (filePath.Length == 0)
                throw new ArgumentOutOfRangeException("'filePath' is zero-length.");

            // set...
            _filePath = filePath;
            _fileName = fileName;
        }

		/// <summary>
		/// Gets the filepath.
		/// </summary>
		public string FilePath
		{
			get
			{
				return _filePath;
			}
		}

        public string FileName
        {
            get
            {
                return _fileName;
            }
        }

		/// <summary>
		/// Gets or sets the contentid
		/// </summary>
		public string ContentId
		{
			get
			{
				return _contentId;
			}
			set
			{
				// check to see if the value has changed...
				if(value != _contentId)
				{
					// set the value...
					_contentId = value;
				}
			}
		}

		public bool HasContentId
		{
			get
			{
				if(this.ContentId != null && this.ContentId.Length > 0)
					return true;
				else
					return false;
			}
		}

        internal void PopulateXmlElement(XmlElement element)
        {
            XmlHelper.AddElement(element, "FileName", this.FileName);

            // data...
            string data = StreamHelper.CopyFileToBase64String(this.FilePath);
            XmlHelper.AddElement(element, "Data", data);
        }

        internal static MailAttachment FromXmlElement(XmlElement element)
        {
            // get...
            string filename = XmlHelper.GetElementString(element, "FileName", OnNotFound.ThrowException);

            // data...
            string data = XmlHelper.GetElementString(element, "Data", OnNotFound.ThrowException);
            string path = Runtime.Current.GetTempFilePath();
            StreamHelper.CreateFileFromBase64String(path, data);

            // ok...
            return new MailAttachment(path, filename);
        }
    }
}
