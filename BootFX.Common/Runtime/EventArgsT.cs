// BootFX - Application framework for .NET applications
// 
// File: EventArgsT.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace BootFX.Common
{
    public class EventArgs<T> : EventArgs, INotifyPropertyChanged
    {
        private T _item;
        public bool HasChanged { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public EventArgs()
        {
        }

        public EventArgs(T item)
        {
            _item = item;
        }

        public T Item
        {
            get
            {
                return _item;
            }
            set
            {
                if (!(object.Equals(_item, value)))
                {
                    _item = value;
                    this.HasChanged = true;
                    this.OnPropertyChanged(new PropertyChangedEventArgs("value"));
                }
            }
        }

        private void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, e);
        }
    }
}
