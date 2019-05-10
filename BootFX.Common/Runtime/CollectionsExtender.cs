// BootFX - Application framework for .NET applications
// 
// File: CollectionsExtender.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using BootFX.Common.Data;
using BootFX.Common.Entities;
using BootFX.Common.Management;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BootFX.Common
{
    public static class CollectionsExtender
    {
        public static List<List<T>> SplitIntoPages<T>(this IEnumerable<T> items, int pageSize)
        {
            return Runtime.Current.GetPages<T>(items, pageSize);
        }

        public static PagedDataResult<T> GetPage<T>(this IEnumerable<T> items, IPagedDataRequestSource source)
        {
            var request = source.Request;
            return items.GetPageInternal(request);
        }

        private static PagedDataResult<T> GetPageInternal<T>(this IEnumerable<T> items, IPagedDataRequest request)
        {
            if (request.PageSize > 0)
            {
                var pages = items.SplitIntoPages(request.PageSize);
                if (request.PageNumber < pages.Count)
                    return new PagedDataResult<T>(pages[request.PageNumber], new PageData(request.PageNumber, request.PageSize, pages.Count, items.Count(), false));
                else
                    return new PagedDataResult<T>(new List<T>(), new PageData(request.PageNumber, request.PageSize, pages.Count, items.Count(), false));
            }
            else
                return new PagedDataResult<T>(items, PageData.GetUnpagedData(items.Count()));
        }

        public static void Merge<T>(this List<T> items, T item)
        {
            if (!(items.Contains(item)))
                items.Add(item);
        }

        public static void Merge<T>(this List<T> items, IEnumerable<T> toMerge)
        {
            foreach (var item in toMerge)
                items.Merge(item);
        }

        public static void Merge(this List<string> items, IEnumerable<string> toMerge, IEqualityComparer<string> comparer)
        {
            foreach (var item in toMerge)
            {
                if (!(items.Contains(item, comparer)))
                    items.Add(item);
            }
        }

        public static void Shuffle<T>(this IList<T> list, Random rand = null)
        {
            if(rand == null)
                rand = new Random();

            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rand.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static IEnumerable<long> ToInt64s(this IEnumerable<int> items)
        {
            var newItems = new List<long>();
            foreach (var item in items)
                newItems.Add(ConversionHelper.ToInt64(item));
            return newItems;
        }

        public static IEnumerable<int> ToInt32s(this IEnumerable<long> items)
        {
            var newItems = new List<int>();
            foreach (var item in items)
                newItems.Add(ConversionHelper.ToInt32(item));
            return newItems;
        }

        public static IEnumerable<string> TrimEnd(this IEnumerable<string> items)
        {
            var results = new List<string>();
            foreach (var item in items)
            {
                if (item != null)
                    results.Add(item.TrimEnd());
                else
                    results.Add(null);
            }

            return results;
        }

        public static IEnumerable<string> TrimStart(this IEnumerable<string> items)
        {
            var results = new List<string>();
            foreach (var item in items)
            {
                if (item != null)
                    results.Add(item.TrimStart());
                else
                    results.Add(null);
            }

            return results;
        }

        public static IEnumerable<string> Trim(this IEnumerable<string> items)
        {
            var results = new List<string>();
            foreach (var item in items)
            {
                if (item != null)
                    results.Add(item.Trim());
                else
                    results.Add(null);
            }

            return results;
        }

        public static void ProcessItems<T>(this IEnumerable<T> items, Action<T> doWork, Action<T> ok = null, Action<T, Exception> failure = null, 
            Action<T> finished = null, ITimingBucket timings = null)
        {
            items.ProcessItemsInternal<T>((item) =>
            {
                try
                {
                    doWork(item);
                    return Task.FromResult<Exception>(null);
                }
                catch (Exception ex)
                {
                    return Task.FromResult<Exception>(ex);
                }
                
            } , ok, failure, finished, timings);
        }

        public static Task ProcessItemsAsync<T>(this IEnumerable<T> items, Func<T, Task> doWork, Action<T> ok = null, Action<T, Exception> failure = null,
            Action<T> finished = null, ITimingBucket timings = null)
        {
            return Task.Run(() =>
            {
                items.ProcessItemsInternal<T>((item) =>
                {
                    return Task.Run<Exception>(async () =>
                    {
                        try
                        {
                            await doWork(item);
                            return null;
                        }
                        catch (Exception ex)
                        {
                            return ex;
                        }
                    });

                }, ok, failure, finished, timings);
            });
        }

        public static Task ProcessItemsInParallel<T>(this IEnumerable<T> items, Action<T> doWork, Action<T> ok = null, Action<T, Exception> failure = null,
            Action<T> finished = null, ITimingBucket timings = null)
        {
            var tasks = new List<Task>();
            items.ProcessItemsInternal<T>((item) =>
            {
                var task = Task.Run<Exception>(() =>
                {
                    try
                    {
                        doWork(item);
                        return null;
                    }
                    catch (Exception ex)
                    {
                        return ex;
                    }
                });
                tasks.Add(task);

                return task;

            }, ok, failure, finished, timings);

            // wait...
            var log = LogSet.GetLog(typeof(CollectionsExtender));
            if (log.IsInfoEnabled)
                log.InfoFormat("{0}: waiting for threads...", typeof(T).Name);

            return Task.WhenAll(tasks.ToArray());
        }

        internal static void ProcessItemsInternal<T>(this IEnumerable<T> items, Func<T, Task<Exception>> doWork, Action<T> ok, Action<T, Exception> failure,
            Action<T> finished, ITimingBucket bucket)
        {
            if (doWork == null)
                throw new ArgumentNullException("doWork");

            var log = LogSet.GetLog(typeof(CollectionsExtender));

            if (bucket == null)
                bucket = NullTimingBucket.Instance;

            var name = typeof(T).Name;
            var result = new ProcessItemsResult<T>(DateTime.UtcNow);
            if (items.Any())
            {
                log.LogTrace(() => string.Format("{0}: processing '{1}' items(s)...", name, items.Count()));

                var tasks = new List<Task>();
                foreach (var item in items)
                {
                    try
                    {
                        var timer = new AccurateTimer();
                        timer.Start();
                        var error = doWork(item);
                        timer.Stop();

                        if (error == null)
                            throw new InvalidOperationException("Work delegate returned null.");

                        // error?
                        error.Wait();
                        if (error.Result != null)
                            throw new InvalidOperationException("Item processing failed.", error.Result);

                        // set...
                        result.Timings[item] = timer.DurationAsDecimal;

                        if (ok != null)
                            ok(item);
                    }
                    catch (Exception ex)
                    {
                        if (log.IsErrorEnabled)
                        {
                            if (typeof(T).IsAssignableFrom(typeof(IEntityId)))
                                log.Error(string.Format("Failed to process item of type '{0}' with ID #{1}.", name, ((IEntityId)item).Id), ex);
                            else
                                log.Error(string.Format("Failed to process item of type '{0}'.", name), ex);
                        }

                        if (failure != null)
                            failure(item, ex);
                    }
                    finally
                    {
                        if (finished != null)
                            finished(item);
                    }
                }

                log.LogTrace(() => string.Format("{0}: finished.", name));
            }
            else
                log.LogTrace(() => string.Format("{0}: nothing to do.", name));
        }

        public static int IndexOf(this List<string> items, string value, StringComparison comparison)
        {
            for (var index = 0; index < items.Count; index++)
            {
                if (string.Compare(items[index], value, comparison) == 0)
                    return index;
            }
            return -1;
        }

        public static void Sort<T>(this List<T> items, Comparison<T> comparer, SortDirection direction)
        {
            items.Sort((a, b) =>
            {
                var result = comparer(a, b);
                if (direction == SortDirection.Descending)
                    result = 0 - result;
                return result;
            });
        }
    }
}
