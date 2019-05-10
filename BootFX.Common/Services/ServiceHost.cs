// BootFX - Application framework for .NET applications
// 
// File: ServiceHost.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using BootFX.Common.Services;
using BootFX.Common.Management;
using System.Linq;
using System.Threading.Tasks;
using System.Text;

namespace BootFX.Common.Services
{
    /// <summary>
    /// Summary description for <see cref="ServiceHost"/>.
    /// </summary>
    public class ServiceHost : Loggable, IDisposable
    {
        /// <summary>
        /// Private field to support <c>Engines</c> property.
        /// </summary>
        private ServiceEngineCollection _engines;
        private object _enginesLock = new object();

        public bool IsStarted { get; private set; }
        private bool _useEnablementChecks;

        private List<string> _enabledNames;
        private bool _hasEnabledNames;
        private bool EnabledNamesInitialized { get; set; }

        public event EventHandler Starting;
        public event EventHandler Started;

        /// <summary>
        /// Creates a new instance of <see cref="ServiceHost"/>.
        /// </summary>
        public ServiceHost()
        {
            _engines = new ServiceEngineCollection(this);

            Runtime.Disposing += (sender, e) =>
            {
                if (this.IsStarted)
                    this.Dispose();
            };
        }

        /// <summary>
        /// Finalizer.
        /// </summary>
        ~ServiceHost()
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
        public virtual void Dispose()
        {
            this.Dispose(DisposeReason.ExplicitCall);
        }

        /// <summary>
        /// Disposes the object.
        /// </summary>
        /// <param name="reason"></param>
        private void Dispose(DisposeReason reason)
        {
            this.DisposeEngines();

            // suppress...
            if (reason == DisposeReason.ExplicitCall)
                GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes the service engines.
        /// </summary>
        private void DisposeEngines()
        {
            if (this.Log.IsInfoEnabled)
                this.Log.Info("Disposing all engines...");

            // dispose...
            List<ServiceEngine> toDispose = null;
            lock (_enginesLock)
            {
                try
                {
                    toDispose = new List<ServiceEngine>(this.Engines);
                }
                finally
                {
                    this.Engines.Clear();
                }
            }

            if (toDispose.Any())
            {
                var tasks = new List<Task>();
                foreach (ServiceEngine engine in toDispose)
                {
                    tasks.Add(Task.Run(() =>
                    {
                        try
                        {
                            engine.Dispose();
                        }
                        catch (Exception ex)
                        {
                            if (this.Log.IsErrorEnabled)
                                this.Log.Error(string.Format("Engine '{0}' failed to stop.", engine), ex);
                        }
                    }));
                }

                Task.WaitAll(tasks.ToArray(), (int)TimeSpan.FromSeconds(60).TotalMilliseconds);
            }
        }

        /// <summary>
        /// Gets a collection of ServiceEngine objects.
        /// </summary>
        public ServiceEngineCollection Engines
        {
            get
            {
                return _engines;
            }
        }

        /// <summary>
        /// Starts all of the engines.
        /// </summary>
        public void Start()
        {
            if (this.Log.IsInfoEnabled)
                this.Log.Info("Starting all engines...");

            // walk...
            try
            {
                this.OnStarting(EventArgs.Empty);

                var engines = this.GetEngines();
                foreach (ServiceEngine engine in engines)
                    engine.Start(this);
            }
            finally
            {
                this.IsStarted = true;

                this.OnStarted(EventArgs.Empty);
            }
        }

        protected virtual void OnStarting(EventArgs e)
        {
            if (this.Starting != null)
                this.Starting(this, e);
        }

        protected virtual void OnStarted(EventArgs e)
        {
            if (this.Started != null)
                this.Started(this, e);
        }

        public IEnumerable<ServiceEngine> GetEngines()
        {
            lock (_enginesLock)
                return new List<ServiceEngine>(_engines);
        }

        public void AddLate(ServiceEngine engine)
        {
            lock (_enginesLock)
            {
                this.Engines.Add(engine);

                if (this.IsStarted)
                    engine.Start(this);
            }
        }

        private bool HasEnabledNames
        {
            get
            {
                this.InitializeEnabledNames();
                return _hasEnabledNames;
            }
        }

        private List<string> EnabledNames
        {
            get
            {
                this.InitializeEnabledNames();
                return _enabledNames;
            }
        }

        private void InitializeEnabledNames()
        {
            if (this.EnabledNamesInitialized)
                return;

            this.LogInfo(() => "Initializing enabled names...");
            try
            {
                var asString = CommandLinePropertyBag.Current.GetStringValue("enabledEngines", null);
                if (!(string.IsNullOrEmpty(asString)))
                {
                    var names = new List<string>();
                    var parts = asString.Split(',');
                    var builder = new StringBuilder();
                    foreach (var part in parts)
                    {
                        var usePart = part.Trim();
                        if (usePart.Length > 0)
                        {
                            names.Add(usePart);

                            if (builder.Length > 0)
                                builder.Append(", ");
                            builder.Append(usePart);
                        }
                    }

                    this.LogInfo(() => $"Found {names.Count} enabled service name(s): {builder}");
                    this._enabledNames = names;
                    this._hasEnabledNames = true;
                }
                else
                {
                    this.LogInfo(() => "No enabled names were specified.");
                    this._hasEnabledNames = false;
                }
            }
            finally
            {
                this.EnabledNamesInitialized = true;
            }
        }

        internal bool IsEnabled(ServiceEngine engine)
        {
            if (this.UseEnablementChecks)
            {
                var enabled = DoEnablementCheck(engine);
                if (enabled)
                {
                    this.LogInfo(() => $"Engine '{engine}' is enabled (enablement checks are on).");
                    return true;
                }
                else
                {
                    this.LogInfo(() => $"Engine '{engine}' is not enabled (enablement checks are on).");
                    return false;
                }
            }
            else
            {
                this.LogInfo(() => $"Engine '{engine}' is enabled (enablement checks are off).");
                return true;
            }
        }

        protected virtual bool DoEnablementCheck(ServiceEngine engine)
        {
            if (this.HasEnabledNames)
            {
                if (this.EnabledNames.Contains(engine.Name, StringComparer.InvariantCultureIgnoreCase))
                {
                    this.LogInfo(() => $"Engine '{engine}' is enabled.");
                    return true;
                }
                else
                {
                    this.LogInfo(() => $"Engine '{engine}' is disabled.");
                    return false;
                }
            }
            else
            {
                this.LogInfo(() => $"Engine '{engine}' is enabled (enablement checks are empty).");
                return true;
            }
        }

        public bool UseEnablementChecks
        {
            get
            {
                return _useEnablementChecks;
            }
            set
            {
                _useEnablementChecks = value;
                this.EnabledNamesInitialized = false;
                this.LogInfo(() => $"Enablement checks are now '{value}'...");
            }
        }
    }
}
