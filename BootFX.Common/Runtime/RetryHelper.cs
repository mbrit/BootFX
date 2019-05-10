// BootFX - Application framework for .NET applications
// 
// File: RetryHelper.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using BootFX.Common.Management;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BootFX.Common
{
    public class RetryHelper : Loggable
    {
        public void Retry(int numTimes, TimeSpan interval, Action callback, bool failSilently = false)
        {
            this.Retry<object>(numTimes, interval, () =>
            {
                callback();
                return null;

            }, failSilently);
        }

        public T Retry<T>(int numTimes, TimeSpan interval, Func<T> callback, bool failSilently = false)
        {
            Exception lastError = null;
            for (int index = 0; index < numTimes; index++)
            {
                try
                {
                    var result = callback();

                    if (index > 0)
                        this.LogInfo(() => "Retryable operation recovered.");

                    return result;
                }
                catch (Exception ex)
                {
                    if (this.Log.IsWarnEnabled)
                        this.Log.WarnFormat("Retryable operation failed ({0} of {1} --> {2}: \"{3}\"). Waiting for '{4}'...", index + 1, numTimes, ex.GetType().FullName, ex.Message, interval);

                    // set...
                    lastError = ex;

                    // wait...
                    Thread.Sleep(interval);
                }
            }

            // throw...
            var message = string.Format("Retryable operation failed after '{0}' attempt(s).", numTimes);
            if (!(failSilently))
                throw new InvalidOperationException(message, lastError);
            else
            {
                if (this.Log.IsErrorEnabled)
                    this.Log.Error(message, lastError);
                return default(T);
            }
        }
    }
}
