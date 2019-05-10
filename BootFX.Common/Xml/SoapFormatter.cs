// BootFX - Application framework for .NET applications
// 
// File: XmlPersister.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;

namespace BootFX.Common.Xml
{
    public class SoapFormatter : IFormatter
    {
        public SoapFormatter()
        {
        }

        public SerializationBinder Binder { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public StreamingContext Context { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public ISurrogateSelector SurrogateSelector { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public FormatterAssemblyStyle AssemblyFormat { get; set; }

        public object Deserialize(Stream serializationStream)
        {
            throw new System.NotImplementedException();
        }

        public void Serialize(Stream serializationStream, object graph)
        {
            throw new System.NotImplementedException();
        }
    }
}