// BootFX - Application framework for .NET applications
// 
// File: TimingBucket.cs
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
using BootFX.Common.Management;

namespace BootFX.Common
{
    public class TimingBucket : Loggable, ITimingBucket
    {
        public string Name { get; private set; }
        private DateTime WallclockStartUtc { get; set; }
        private DateTime WallclockEndUtc { get; set; }
        private Dictionary<string, ITimingElement> Timings { get; set; }
        private List<string> Order { get; set; }
        private List<string> Tags { get; set; }
        private object _lock = new object();
        private int _numItems = 0;

        public TimingBucket(string name = null, int numItems = 1)
        {
            this.Name = name;
            this.NumItems = numItems;

            this.Timings = new Dictionary<string, ITimingElement>();
            this.Order = new List<string>();
            this.Tags = new List<string>();
            this.WallclockStartUtc = DateTime.UtcNow;
        }

        public IDisposable GetTimer(string name)
        {
            lock (_lock)
            {
                name = "t:" + name;

                TimingBucketTimer timer = null;
                if (this.Timings.ContainsKey(name))
                    timer = (TimingBucketTimer)this.Timings[name];
                else
                {
                    timer = new TimingBucketTimer(name);
                    this.Timings[name] = timer;
                    this.Order.Add(name);
                }

                // return...
                timer.Start();
                return timer;
            }
        }

        public ITimingBucket GetChildBucket(string name, int numItems = 1)
        {
            lock (_lock)
            {
                name = "c:" + name;

                TimingBucket timer = null;
                if (this.Timings.ContainsKey(name))
                {
                    timer = (TimingBucket)this.Timings[name];
                    timer.IncrementNumItems(numItems);
                }
                else
                {
                    timer = new TimingBucket(name, numItems);
                    this.Timings[name] = timer;
                    this.Order.Add(name);
                }

                // return...
                return timer;
            }
        }

        public string Dump(string preamble, int numItems)
        {
            this.NumItems = numItems;
            return this.Dump(preamble);
        }

        public string Dump(string preamble = null)
        {
            if (string.IsNullOrEmpty(preamble))
                preamble = "Timings";

            if (this.Log.IsInfoEnabled)
            {
                lock (_lock)
                {
                    var builder = new StringBuilder();
                    this.DumpInternal(preamble, 0, builder);

                    // send...
                    this.Log.Info(builder.ToString());

                    return builder.ToString();
                }
            }
            else
                return string.Empty;
        }

        public decimal Duration
        {
            get
            {
                var total = 0M;
                foreach (var item in this.Timings.Values)
                    total += item.Duration;
                return total;
            }
        }

        private void DumpInternal(string preamble, int level, StringBuilder builder)
        {
            this.Indent(builder, level);
            builder.Append("Timing bucket --> ");
            builder.Append(preamble);
            if (!(string.IsNullOrEmpty(preamble)))
                builder.Append(" ");
            builder.Append("[");
            builder.Append(this.NumItems);
            builder.Append(" item(s)]");

            if (this.Tags.Any())
            {
                builder.Append("\r\n");
                this.Indent(builder, level);
                builder.Append("Tags: ");
                foreach (var tag in Tags)
                {
                    builder.Append("\r\n");
                    this.Indent(builder, level);
                    builder.Append("==> {");
                    builder.Append(tag);
                    builder.Append("}");
                }
                builder.Append("\r\n");
            }

            var max = 0;
            foreach (var name in this.Order)
            {
                if (name.Length > max)
                    max = name.Length;
            }

            var total = this.Duration;
            foreach (var name in this.Order)
            {
                var duration = this.Timings[name].Duration;

                builder.Append("\r\n");
                this.Indent(builder, level + 1);
                this.Pad(builder, name, max + 3);
                builder.Append(": ");
                this.Pad(builder, duration.ToString("n5"), 10, false);
                builder.Append("s");

                // total...
                if (total > 0)
                {
                    builder.Append(" ");
                    this.Pad(builder, ((duration / total) * 100).ToString("n2"), 8, false);
                    builder.Append("%");
                }

                if (this.NumItems > 0)
                {
                    builder.Append(" ");
                    this.Pad(builder, (duration / (decimal)this.NumItems).ToString("n5"), 10, false);
                    builder.Append("s per");
                }

                // child...
                if (this.Timings[name] is TimingBucket)
                {
                    builder.Append("\r\n");

                    // child...
                    var child = (TimingBucket)this.Timings[name];
                    child.DumpInternal(child.Name, level + 1, builder);
                }
            }

            builder.Append("\r\n");
            this.Indent(builder, level + 1);
            builder.Append("---------------------------------------------------------------");

            builder.Append("\r\n");
            this.Indent(builder, level + 1);
            this.Pad(builder, "TOTAL", max + 3);
            builder.Append(": ");
            this.Pad(builder, total.ToString("n5"), 10, false);
            builder.Append("s");

            if (this.NumItems > 0)
            {
                this.Pad(builder, string.Empty, 13);

                builder.Append(" ");
                builder.Append((total / (decimal)this.NumItems).ToString("n5"));
                builder.Append("s per");
            }

            var wallclockSeconds = (decimal)(this.ResolvedWallclockEndUtc - this.WallclockStartUtc).TotalSeconds;
            builder.Append("\r\n");
            this.Indent(builder, level + 1);
            this.Pad(builder, "WALLCLOCK", max + 3);
            builder.Append(": ~");
            this.Pad(builder, wallclockSeconds.ToString("n5"), 9, false);
            builder.Append("s");

            if (this.NumItems > 0)
            {
                this.Pad(builder, string.Empty, 12);

                builder.Append(" ~");
                builder.Append((wallclockSeconds / (decimal)this.NumItems).ToString("n5"));
                builder.Append("s per");
            }

            //builder.Append("\r\n");
        }

        private DateTime ResolvedWallclockEndUtc
        {
            get
            {
                if (this.WallclockEndUtc != DateTime.MinValue)
                    return this.WallclockEndUtc;
                else
                    return DateTime.UtcNow;
            }
        }

        private void Indent(StringBuilder builder, int indent)
        {
            for (int index = 0; index < indent; index++)
                builder.Append("    ");
        }

        private void Pad(StringBuilder builder, string buf, int length, bool left = true)
        {
            if (!(left))
            {
                for (var index = buf.Length; index < length; index++)
                    builder.Append(" ");
            }

            builder.Append(buf);

            if (left)
            {
                for (var index = buf.Length; index < length; index++)
                    builder.Append(" ");
            }
        }

        //internal void AddTiming(string name, decimal duration)
        //{
        //    lock (_lock)
        //    {
        //        if (!(this.Timings.ContainsKey(name)))
        //        {
        //            this.Timings[name] = 0M;
        //            this.Order.Add(name);
        //        }
        //        this.Timings[name] += duration;
        //    }
        //}

        //public TimingInstruction ToTimingInstruction(string owner)
        //{
        //    var instruction = new TimingInstruction()
        //    {
        //        Owner = owner,
        //    };
        //    lock (_lock)
        //    {
        //        // set the base count...
        //        instruction.Counts["."] = this.NumItems;

        //        // tbd...
        //        this.WalkAndAdd(".", this, instruction);
        //    }

        //    return instruction;
        //}

        //private void WalkAndAdd(string parent, TimingBucket bucket, TimingInstruction instruction)
        //{
        //    foreach(var entry in bucket.Timings)
        //    {
        //        var name = parent + "/" + entry.Key;

        //        // write...
        //        instruction.Timings[name] = entry.Value.Duration;
        //        if (entry.Value is TimingBucket)
        //        {
        //            var child = (TimingBucket)entry.Value;
        //            instruction.Counts[child.Name] = child.NumItems;
        //            this.WalkAndAdd(name, child, instruction);
        //        }
        //    }
        //}

        public void Dispose()
        {
            this.WallclockEndUtc = DateTime.UtcNow;
        }

        public int NumItems
        {
            get
            {
                lock (_lock)
                    return _numItems;
            }
            set
            {
                lock (_lock)
                    _numItems = value;
            }
        }

        public void IncrementNumItems(int increment = 1)
        {
            lock (_lock)
                this.NumItems += increment;
        }

        public Dictionary<string, ITimingElement> GetTimings()
        {
            return new Dictionary<string, ITimingElement>(this.Timings);
        }

        public override string ToString()
        {
            lock (_lock)
            {
                var builder = new StringBuilder();
                this.DumpInternal(null, 0, builder);
                return builder.ToString();
            }
        }

        public void AddTag(string tag)
        {
            this.Tags.Add(tag);
        }

        public bool IsLogging
        {
            get
            {
                return true;
            }
        }

        public T MeasureOperation<T>(string name, Func<T> callback)
        {
            using (var timer = this.GetTimer(name))
                return callback();
        }
    }
}
