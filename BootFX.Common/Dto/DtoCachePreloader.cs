// BootFX - Application framework for .NET applications
// 
// File: DtoCachePreloader.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using BootFX.Common.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BootFX.Common.Dto
{
    public class DtoCachePreloader : IDisposable
    {
        private BfxLookup<EntityType, BfxLookup<long, IDtoCapable>> ByType { get; set; }

        private static DtoCachePreloader _boundPreloader;

        private DtoCachePreloader()
        {
            this.ByType = new BfxLookup<EntityType, BfxLookup<long, IDtoCapable>>();
            this.ByType.CreateItemValue += ByType_CreateItemValue;
        }

        void ByType_CreateItemValue(object sender, CreateLookupItemEventArgs<EntityType, BfxLookup<long, IDtoCapable>> e)
        {
            var byId = new BfxLookup<long, IDtoCapable>();
            byId.CreateItemValue += (senderById, eById) =>
                {
                    eById.NewValue = (IDtoCapable)e.Key.Persistence.GetById(new object[] { eById.Key });
                };

            // return...
            e.NewValue = byId;
        }


        internal static DtoCachePreloader BoundPreloader
        {
            get
            {
                return _boundPreloader;
            }
        }

        private static void DisposePreloader()
        {
            if (_boundPreloader != null)
            {
                try
                {
                    _boundPreloader.Dispose();
                }
                finally
                {
                    _boundPreloader = null;
                }
            }
        }

        public static IDisposable Bind()
        {
            DisposePreloader();
            _boundPreloader = new DtoCachePreloader();
            return new PreloaderDisposer();
        }

        private class PreloaderDisposer : IDisposable
        {
            public void Dispose()
            {
                DisposePreloader();
            }
        }

        public void Dispose()
        {
        }

        internal IDtoCapable GetItem(EntityType et, long id)
        {
            var lookup = this.ByType[et];
            return lookup[id];
        }
    }
}
