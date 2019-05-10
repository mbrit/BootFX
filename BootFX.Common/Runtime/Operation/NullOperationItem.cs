// BootFX - Application framework for .NET applications
// 
// File: NullOperationItem.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BootFX.Common
{
    public class NullOperationItem : IOperationItem
    {
        public static NullOperationItem Instance { get; private set; }

        public event EventHandler Finished;
        public event EventHandler Cancelled;
        public event EventHandler Error;

        private NullOperationItem()
        {
        }

        static NullOperationItem()
        {
            Instance = new NullOperationItem();
        }

        public string Status
        {
            get
            {
                return null;
            }
            set
            {
            }
        }

        public int ProgressMaximum
        {
            get
            {
                return 0;
            }
            set
            {
            }
        }

        public int ProgressMinimum
        {
            get
            {
                return 0;
            }
            set
            {
            }
        }

        public int ProgressValue
        {
            get
            {
                return 0;
            }
            set
            {
            }
        }

        public Exception LastError
        {
            get
            {
                return null;
            }
        }

        public void SetLastError(string status, Exception error)
        {
        }

        public void Reset()
        {
        }

        public DateTime LastUpdateTime
        {
            get
            {
                return DateTime.MinValue;
            }
        }

        public void Cancel()
        {
        }

        public bool IsCancelled
        {
            get
            {
                return false;
            }
        }

        public void IncrementProgress()
        {
        }

        protected void OnFinished(EventArgs e)
        {
            if (this.Finished != null)
                this.Finished(this, e);
        }

        protected void OnCancelled(EventArgs e)
        {
            if (this.Cancelled != null)
                this.Cancelled(this, e);
        }

        protected void OnError(EventArgs e)
        {
            if (this.Error != null)
                this.Error(this, e);
        }
    }
}
