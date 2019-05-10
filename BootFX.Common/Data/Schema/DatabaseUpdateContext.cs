// BootFX - Application framework for .NET applications
// 
// File: DatabaseUpdateContext.cs
// Build: 5.2.10321.2307
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

namespace BootFX.Common.Data
{
    /// <summary>
    /// Defines an instance of <c>DatabaseUpdateContext</c>.
    /// </summary>
    public class DatabaseUpdateContext
    {
        /// <summary>
        /// Private field to support <see cref="Operation"/> property.
        /// </summary>
        private IOperationItem _operation;

        public bool Trace { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="operation"></param>
        // mbr - 2009-07-21 - made public...
        //internal DatabaseUpdateContext(IOperationItem operation)
        public DatabaseUpdateContext(IOperationItem operation, bool trace)
		{
			if(operation == null)
				operation = new OperationItem();
			_operation = operation;
            this.Trace = trace;
		}

		/// <summary>
		/// Gets the operation.
		/// </summary>
		public IOperationItem Operation
		{
			get
			{
				return _operation;
			}
		}
	}
}
