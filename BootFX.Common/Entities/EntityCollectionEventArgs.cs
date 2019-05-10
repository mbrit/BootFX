// BootFX - Application framework for .NET applications
// 
// File: EntityCollectionEventArgs.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Text;

namespace BootFX.Common.Entities
{
    public delegate void EntityCollectionEventHandler(object sender, EntityCollectionEventArgs e);

    public class EntityCollectionEventArgs
    {
        private EntityCollection _items;

        public EntityCollectionEventArgs()
        {
        }

        public EntityCollection Items
        {
            get
            {
                return _items;
            }
            set
            {
                _items = value;
            }
        }
    }
}
