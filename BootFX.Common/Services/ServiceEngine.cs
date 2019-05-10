// BootFX - Application framework for .NET applications
// 
// File: ServiceEngine.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Linq;
using System.Threading;
using BootFX.Common.Management;
using System.Collections.Generic;

namespace BootFX.Common.Services
{
    /// <summary>
    /// Defines a class that provides the base for an activity that makes up part of a service.  
    /// </summary>
    /// <remarks>Usually, these would be threaded.  Consider using <see cref="ThreadedServiceEngine"></see>.</remarks>
    public abstract class ServiceEngine : Loggable, IDisposable
    {
        /// <summary>
        /// Private field to support <see cref="Parent"/> property.
        /// </summary>
        private ServiceEngine _parent;

        /// <summary>
        /// Private field to support <c>ChildEngines</c> property.
        /// </summary>
        private ServiceEngineCollection _childEngines = new ServiceEngineCollection();

        /// <summary>
        /// Raised when there has been a processing error.
        /// </summary>
        public event ThreadExceptionEventHandler ProcessingError;

        /// <summary>
        /// Private field to support <c>Host</c> property.
        /// </summary>
        private ServiceHost _host;

        /// <summary>
        /// Raised when the engine is being disposed.
        /// </summary>
        public event EventHandler Disposing;

        /// <summary>
        /// Raised when the engine has been disposed.
        /// </summary>
        public event EventHandler Disposed;

        /// <summary>
        /// Raised when the engine in starting.
        /// </summary>
        public event EventHandler Starting;

        /// <summary>
        /// Raised when the event has been started.
        /// </summary>
        public event EventHandler Started;

        /// <summary>
        /// Private field to support <c>IsDisposed</c> property.
        /// </summary>
        private bool _isDisposed = false;

        private ServiceEngineStatus _status = ServiceEngineStatus.Instantiated;

        /// <summary>
        /// Raised when the value for <c>Status</c> changes.
        /// </summary>
        internal event EventHandler StatusChanged;

        public event EventHandler WorkStarted;

        public event EventHandler WorkFinished;

        public int? Instance { get; set; }

        /// <summary>
        /// Creates a new instance of <see cref="ServiceEngine"/>.
        /// </summary>
        protected ServiceEngine()
        {
        }

        /// <summary>
        /// Finalizer.
        /// </summary>
        ~ServiceEngine()
        {
            try
            {
                this.Dispose(DisposeReason.FromFinalizer);
            }
            catch
            {
                // ignore exceptions...
            }
        }

        /// <summary>
        /// Disposes the object.
        /// </summary>
        public void Dispose()
        {
            // dispose...
            this.Dispose(DisposeReason.ExplicitCall);
        }

        /// <summary>
        /// Disposes the object.
        /// </summary>
        /// <param name="reason"></param>
        protected virtual void DoDispose(DisposeReason reason)
        {
            // monitor...
            this.RunOperation(() => ServiceHostMonitor.Current.RaiseDisposed(this));
        }

        /// <summary>
        /// Raises the <c>Disposed</c> event.
        /// </summary>
        private void OnDisposed()
        {
            OnDisposed(EventArgs.Empty);
        }

        /// <summary>
        /// Raises the <c>Disposed</c> event.
        /// </summary>
        protected virtual void OnDisposed(EventArgs e)
        {
            // raise...
            if (Disposed != null)
                Disposed(this, e);
        }

        /// <summary>
        /// Raises the <c>Disposing</c> event.
        /// </summary>
        private void OnDisposing()
        {
            OnDisposing(EventArgs.Empty);
        }

        /// <summary>
        /// Raises the <c>Disposing</c> event.
        /// </summary>
        protected virtual void OnDisposing(EventArgs e)
        {
            // raise...
            if (Disposing != null)
                Disposing(this, e);
        }

        /// <summary>
        /// Disposes the object.
        /// </summary>
        /// <param name="reason"></param>
        private void Dispose(DisposeReason reason)
        {
            this.OnDisposing();
            try
            {
                // check...
                this.AssertNotDisposed();

                // defer...
                if (this.Log.IsInfoEnabled)
                    this.Log.Info(string.Format("Disposing: {0}", this));

                // dispose the children...
                foreach (ServiceEngine child in this.ChildEngines)
                    child.Dispose(reason);

                // mbr - 11-11-2005 - dispose us...
                DoDispose(reason);
            }
            finally
            {
                try
                {
                    // suppress...
                    if (reason == DisposeReason.ExplicitCall)
                        GC.SuppressFinalize(this);

                    // flag...
                    _isDisposed = true;

                    // set...
                    this.OnDisposed();
                }
                finally
                {
                    this.Status = ServiceEngineStatus.Disposed;
                }
            }
        }

        /// <summary>
        /// Starts the thread.
        /// </summary>
        internal void Start(ServiceEngine parent)
        {
            if (parent == null)
                throw new ArgumentNullException("parent");

            // check...
            if (parent.Host == null)
                throw new InvalidOperationException("parent.Host is null.");

            // set...
            _parent = parent;

            // start...
            this.Start(parent.Host);
        }

        /// <summary>
        /// Gets the parent.
        /// </summary>
        protected ServiceEngine Parent
        {
            get
            {
                return _parent;
            }
        }

        /// <summary>
        /// Starts the thread.
        /// </summary>
        internal void Start(ServiceHost host)
        {
            if (host == null)
                throw new ArgumentNullException("host");

            try
            {
                _host = host;

                if (this.Log.IsInfoEnabled)
                    this.Log.Info(string.Format("Starting: {0}", this));

                // on...
                this.OnStarting();

                // defer...
                this.DoStart();

                // do the children...
                foreach (ServiceEngine child in this.ChildEngines)
                    child.Start(this);

                // on...
                this.OnStarted();

                // monitor...
                this.RunOperation(() => ServiceHostMonitor.Current.RaiseStarted(this));
            }
            finally
            {
                this.Status = ServiceEngineStatus.Idle;
            }
        }

        /// <summary>
        /// Called when the service is started.
        /// </summary>
        protected virtual void DoStart()
        {
        }

        /// <summary>
        /// Raises the <c>Started</c> event.
        /// </summary>
        private void OnStarted()
        {
            OnStarted(EventArgs.Empty);
        }

        /// <summary>
        /// Raises the <c>Started</c> event.
        /// </summary>
        protected virtual void OnStarted(EventArgs e)
        {
            // raise...
            if (Started != null)
                Started(this, e);
        }

        /// <summary>
        /// Raises the <c>Starting</c> event.
        /// </summary>
        private void OnStarting()
        {
            OnStarting(EventArgs.Empty);
        }

        /// <summary>
        /// Raises the <c>Starting</c> event.
        /// </summary>
        protected virtual void OnStarting(EventArgs e)
        {
            // raise...
            if (Starting != null)
                Starting(this, e);
        }

        /// <summary>
        /// Gets the isdisposed.
        /// </summary>
        private bool IsDisposed
        {
            get
            {
                // returns the value...
                return _isDisposed;
            }
        }

        /// <summary>
        /// Asserts that the engine is not disposed.
        /// </summary>
        protected void AssertNotDisposed()
        {
            if (this.IsDisposed)
                throw new ObjectDisposedException(this.ToString(), string.Format("The service engine '{0}' has been disposed.", this));
        }

        /// <summary>
        /// Gets the host.
        /// </summary>
        protected ServiceHost Host
        {
            get
            {
                // returns the value...
                return _host;
            }
        }

        /// <summary>
        /// Handles the given work item in the thread pool.
        /// </summary>
        /// <param name="workItem"></param>
        protected void HandleInThreadPool(object workItem)
        {
            if (workItem == null)
                throw new ArgumentNullException("workItem");

            // thread...
            ThreadPool.QueueUserWorkItem(new WaitCallback(ThreadPoolEntryPoint), workItem);
        }

        private void ThreadPoolEntryPoint(object state)
        {
            if (state == null)
                throw new ArgumentNullException("state");
            try
            {
                this.DoWorkItem(state);
            }
            catch (Exception ex)
            {
                this.ProcessingErrorOccurred(ex, state);
            }
        }

        /// <summary>
        /// Override point for handling a work item.
        /// </summary>
        /// <param name="workItem"></param>
        protected virtual void DoWorkItem(object workItem)
        {
            throw new NotImplementedException(string.Format("Not implemented for '{0}'.", this.GetType()));
        }

        /// <summary>
        /// Called when a threading error occurs.
        /// </summary>
        /// <param name="ex"></param>
        internal void ProcessingErrorOccurred(Exception ex, object state)
        {
            if (ex == null)
                throw new ArgumentNullException("ex");

            if (this.Log.IsErrorEnabled)
                this.Log.Error(string.Format("The service engine '{0}' experienced an exception.", this), ex);

            // event...
            // mbr - 2009-09-09 - added a handler on this as an exception in an error handler will kill the process...
            try
            {
                this.OnProcessingError(new ThreadExceptionEventArgs(ex));
            }
            catch
            {
                // no-op...
            }
        }

        /// <summary>
        /// Raises the <c>ProcessingError</c> event.
        /// </summary>
        protected virtual void OnProcessingError(ThreadExceptionEventArgs e)
        {
            // raise...
            if (ProcessingError != null)
                ProcessingError(this, e);
        }

        /// <summary>
        /// Gets a collection of ServiceEngine objects.
        /// </summary>
        public ServiceEngineCollection ChildEngines
        {
            get
            {
                return _childEngines;
            }
        }

        public ServiceEngineStatus Status
        {
            get
            {
                return _status;
            }
            set
            {
                _status = value;
                this.OnStatusChanged();
            }
        }

        /// <summary>
        /// Raises the <c>StatusChanged</c> event.
        /// </summary>
        private void OnStatusChanged()
        {
            OnStatusChanged(EventArgs.Empty);
        }

        /// <summary>
        /// Raises the <c>StatusChanged</c> event.
        /// </summary>
        protected virtual void OnStatusChanged(EventArgs e)
        {
            // raise...
            if (StatusChanged != null)
                StatusChanged(this, e);
        }

        protected virtual void OnWorkStarted(EventArgs e)
        {
            this.RunOperation(() => ServiceHostMonitor.Current.RaiseWorkStarted(this));

            if (this.WorkStarted != null)
                this.WorkStarted(this, e);
        }

        protected virtual void OnWorkFinished(EventArgs e)
        {
            this.RunOperation(() => ServiceHostMonitor.Current.RaiseWorkFinished(this));

            if (this.WorkFinished != null)
                this.WorkFinished(this, e);
        }

        public string Id
        {
            get
            {
                var id = this.GetType().Name;
                if (this.Instance != null)
                    id += this.Instance;
                return id;
            }
        }

        public string Name => this.GetType().Name;
    }
}
