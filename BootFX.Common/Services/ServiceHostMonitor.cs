using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BootFX.Common.Services;

namespace BootFX.Common.Management
{
    public class ServiceHostMonitor
    {
        public event EventHandler Started;
        public event EventHandler Disposed;
        public event EventHandler WorkStarted;
        public event EventHandler WorkFinished;

        private static ServiceHostMonitor _current = new ServiceHostMonitor();

        private ServiceHostMonitor()
        {
        }

        public static ServiceHostMonitor Current
        {
            get
            {
                if (_current == null)
                    throw new ObjectDisposedException("ServiceHostMonitor");
                return _current;
            }
        }

        internal void RaiseDisposed(ServiceEngine engine)
        {
            if (this.Disposed != null)
                this.Disposed(engine, EventArgs.Empty);
        }

        internal void RaiseStarted(ServiceEngine engine)
        {
            if(this.Started != null)
                this.Started(engine, EventArgs.Empty);
        }

        internal void RaiseWorkFinished(ServiceEngine engine)
        {
            if (this.WorkFinished != null)
                this.WorkFinished(engine, EventArgs.Empty);
        }

        internal void RaiseWorkStarted(ServiceEngine engine)
        {
            if (this.WorkStarted != null)
                this.WorkStarted(engine, EventArgs.Empty);
        }
    }
}
