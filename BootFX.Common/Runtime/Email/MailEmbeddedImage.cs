// BootFX - Application framework for .NET applications
// 
// File: MailEmbeddedImage.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;

namespace BootFX.Common.Email
{
    public class MailEmbeddedImage
    {
        internal string Name { get; private set; }
        internal string Path { get; private set; }
        internal string ContentType { get; private set; }
        public string ContentId { get; private set; }
        internal bool IsPersistent { get; private set; }

        private static Dictionary<string, MailEmbeddedImage> ResourceImages { get; set; }
        private static object _lock = new object();

        internal MailEmbeddedImage(string name, string path, string contentType, string contentId, bool isPersistent)
        {
            this.Name = name;
            this.Path = path;
            this.ContentType = contentType;
            this.ContentId = contentId;

            this.IsPersistent = isPersistent;
        }

        static MailEmbeddedImage()
        {
            ResourceImages = new Dictionary<string, MailEmbeddedImage>();
        }

        public static MailEmbeddedImage GetImageForResource(Assembly asm, string name, string contentType)
        {
            lock (_lock)
            {
                var key = string.Format("{0}|{1}", asm.GetName().Name, name);
                if (!(ResourceImages.ContainsKey(key)))
                {
                    var temp = ResourceHelper.CopyResourceToTempFile(asm, name);
                    var contentId = HashHelper.GetMd5HashOfStringAsHex(key);
                    ResourceImages[key] = new MailEmbeddedImage(name, temp, contentType, contentId, true);
                }

                return ResourceImages[key];
            }
        }

        internal string GetBytesAsBase64String()
        {
            return StreamHelper.CopyFileToBase64String(this.Path);
        }

        internal void DisposeImageIfNotPersistent()
        {
            if (!(this.IsPersistent))
                Runtime.Current.SafeDelete(this.Path);
        }

        public void WriteImageReference(StringBuilder builder)
        {
            builder.Append("<img src=\"cid:");
            builder.Append(WebUtility.UrlEncode(this.ContentId));
            builder.Append("\" />");
        }
    }
}
