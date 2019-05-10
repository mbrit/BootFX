// BootFX - Application framework for .NET applications
// 
// File: DateTimeExtender.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BootFX.Common
{
    public static class DateTimeExtender
    {
        private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static DateTime TrimTime(this DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, dt.Day);
        }

        public static DateTime TrimSeconds(this DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, 0);
        }

        public static DateTime AdjustToOneSecondBeforeMidnight(this DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, dt.Day, 23, 59, 59);
        }

        public static DateTime AdjustToOneMillisecondBeforeMidnight(this DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, dt.Day, 23, 59, 59, 999);
        }

        public static bool IsLastDayOfMonth(this DateTime dt)
        {
            dt = dt.TrimTime();
            return dt.AddDays(1).Month != dt.Month;
        }

        public static DateTime GetStartOfMonth(this DateTime dt)
        {
            if (dt != DateTime.MinValue)
                return new DateTime(dt.Year, dt.Month, 1);
            else
                return DateTime.MinValue;
        }

        public static DateTime GetEndOfMonth(this DateTime dt)
        {
            if (dt != DateTime.MinValue)
            {
                // don't trim milliseconds here as it breaks SQL ranges...
                dt = dt.GetStartOfMonth().AddMonths(1).AddSeconds(-1);
                return dt;
            }
            else
                return DateTime.MinValue;
        }

        public static int GetNumDaysInMonth(this DateTime dt)
        {
            dt = dt.GetStartOfMonth();
            dt = dt.AddMonths(1).AddDays(-1);

            // return...
            return dt.Day;
        }

        public static bool IsInSameMonth(this DateTime a, DateTime b)
        {
            return a.Year == b.Year && a.Month == b.Month;
        }

        public static DateTime GetFromUnixTime(long seconds)
        {
            return UnixEpoch.AddSeconds(seconds);
        }

        public static DateTime GetMinimumDate(IEnumerable<DateTime> dts)
        {
            var min = DateTime.MinValue;
            foreach (var dt in dts)
            {
                if (min == DateTime.MinValue)
                    min = dt;
                else if (dt < min)
                    min = dt;
            }
            return min;
        }

        public static DateTime GetMaximumDate(IEnumerable<DateTime> dts)
        {
            var max = DateTime.MaxValue;
            foreach (var dt in dts)
            {
                if (max == DateTime.MaxValue)
                    max = dt;
                else if (dt > max)
                    max = dt;
            }
            return max;
        }

        public static IEnumerable<DateTime> GetContiguousRange(this IEnumerable<DateTime> dts)
        {
            var min = GetMinimumDate(dts);
            if (min == DateTime.MinValue || min == DateTime.MaxValue)
                throw new InvalidOperationException(string.Format("Cannot create a contiguous range with a minimum value of '{0}'.", min));

            var max = GetMaximumDate(dts);
            if (max == DateTime.MinValue || max == DateTime.MaxValue)
                throw new InvalidOperationException(string.Format("Cannot create a contiguous range with a maximum value of '{0}'.", max));

            var results = new List<DateTime>();
            var dt = min;
            while (dt < max)
            {
                results.Add(dt);
                dt = dt.AddDays(1);
            }

            return results;
        }

        public static int GetWeekNumber(this DateTime dt, CalendarWeekRule rule = CalendarWeekRule.FirstDay, DayOfWeek firstDay = DayOfWeek.Sunday)
        {
            var cal = DateTimeFormatInfo.CurrentInfo.Calendar;
            return cal.GetWeekOfYear(dt, rule, firstDay);
        }

        public static bool IsSameDay(this DateTime dt, DateTime other)
        {
            return dt.Day == other.Day && dt.Month == other.Month && dt.Year == other.Year;
        }
    }
}
