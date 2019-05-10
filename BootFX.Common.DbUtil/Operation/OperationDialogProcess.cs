//// BootFX - Application framework for .NET applications
//// 
//// File: OperationDialogProcess.cs
//// Build: 5.0.61009.900
//// 
//// An open source project by Matthew Reynolds (@mbrit).  
//// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
//// Elixia Solutions Limited.  All Rights Reserved.
////
//// Licensed under the MIT license.

//using System;
//using BootFX.Common.Management;

//namespace BootFX.Common.UI.Desktop
//{
//    /// <summary>
//    /// Decribes a process that can be passed to the operation dialog for execution.
//    /// </summary>
//    public abstract class OperationDialogProcess : Loggable
//    {
//        /// <summary>
//        /// Private field to support <see cref="RunResult"/> property.
//        /// </summary>
//        private object _runResult;

//        /// <summary>
//        /// Private field to support <see cref="Context"/> property.
//        /// </summary>
//        private OperationContext _context;

//        /// <summary>
//        /// Constructor.
//        /// </summary>
//        /// <param name="context"></param>
//        protected OperationDialogProcess(OperationContext context)
//        {
//            if (context == null)
//                throw new ArgumentNullException("context");

//            // set...
//            _context = context;
//        }

//        /// <summary>
//        /// Runs the process.
//        /// </summary>
//        public object Run()
//        {
//            // reset...
//            _runResult = null;

//            // run...
//            _runResult = DoRun();

//            // return...
//            return _runResult;
//        }

//        /// <summary>
//        /// Gets the context.
//        /// </summary>
//        protected internal OperationContext Context
//        {
//            get
//            {
//                return _context;
//            }
//        }

//        /// <summary>
//        /// Runs the process.
//        /// </summary>
//        /// <returns></returns>
//        protected abstract object DoRun();

//        /// <summary>
//        /// Gets the runresult.
//        /// </summary>
//        public object RunResult
//        {
//            get
//            {
//                return _runResult;
//            }
//        }
//    }
//}
