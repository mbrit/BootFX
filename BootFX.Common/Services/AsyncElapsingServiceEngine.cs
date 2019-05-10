// BootFX - Application framework for .NET applications
// 
// File: AsyncElapsingServiceEngine.cs
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
using System.Threading.Tasks;

namespace BootFX.Common.Services
{
    public abstract class AsyncElapsingServiceEngine : ElapsingServiceEngine
    {
        protected override sealed void OnElapsed(EventArgs e)
        {
            base.OnElapsed(e);

            Exception failed = null;
            var task = Task.Run(async () =>
            {
                try
                {
                    await this.OnElapsedAsync(EventArgs.Empty);
                }
                catch (Exception ex)
                {
                    failed = ex;
                }
            });
            task.Wait();

            if(failed != null)
                throw new InvalidOperationException("An error occurred.", failed);
        }

        protected abstract Task OnElapsedAsync(EventArgs e);
    }
}
