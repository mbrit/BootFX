// BootFX - Application framework for .NET applications
// 
// File: TimeSpanConverter.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Collections;
using System.ComponentModel;
using System.Text;

namespace BootFX.Common.Data.Converters
{
	/// <summary>
	/// Summary description for TimeSpanConverter.
	/// </summary>
	public class TimeSpanConverter : TypeConverter
	{
		/// <summary>
		/// Returns a bool to determine if this converter can convert from the source type
		/// </summary>
		/// <param name="context"></param>
		/// <param name="sourceType"></param>
		/// <returns></returns>
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			if(sourceType.Equals(typeof(string)) || sourceType.Equals(typeof(TimeSpan)))
				return true;

			return base.CanConvertFrom (context, sourceType);
		}

		/// <summary>
		/// Returns a bool to determine if this converter can convert to the destination type
		/// </summary>
		/// <param name="context"></param>
		/// <param name="destinationType"></param>
		/// <returns></returns>
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			if(destinationType.Equals(typeof(TimeSpan)) || destinationType.Equals(typeof(string)))
				return true;

			return base.CanConvertTo (context, destinationType);
		}

		/// <summary>
		/// Converts the value provided into the destination type
		/// </summary>
		/// <param name="context"></param>
		/// <param name="culture"></param>
		/// <param name="value"></param>
		/// <param name="destinationType"></param>
		/// <returns></returns>
		public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
		{
			if(destinationType.Equals(typeof(TimeSpan)) && value.GetType().Equals(typeof(string)))
				return Parse((string) value);
			else if (destinationType.Equals(typeof(string)) && value.GetType().Equals(typeof(TimeSpan)))
				return Parse((TimeSpan) value);

			return base.ConvertTo (context, culture, value, destinationType);
		}

		/// <summary>
		/// Converts from the value provided
		/// </summary>
		/// <param name="context"></param>
		/// <param name="culture"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
		{
			if(value.GetType().Equals(typeof(string)))
				return Parse((string) value);
			else if (value.GetType().Equals(typeof(TimeSpan)))
				return Parse((TimeSpan) value);

			return base.ConvertFrom (context, culture, value);
		}


		#region Parse String and Convert to TimeSpan
		
		/// <summary>
		/// Valid name for durations
		/// </summary>
		private static ArrayList ValidDurations = new ArrayList(new string[] {"year","month","week","day","hour","hr","minute","min","second","sec","tick"});
		
		/// <summary>
		/// Valid duration lengths, this are aligned with the names
		/// </summary>
		private static ArrayList ValidDurationLength = new ArrayList(new TimeSpan[] {new TimeSpan(365,0,0,0,0),new TimeSpan(28,0,0,0,0),new TimeSpan(7,0,0,0,0),new TimeSpan(1,0,0,0,0),new TimeSpan(1,0,0),new TimeSpan(1,0,0),new TimeSpan(0,1,0),new TimeSpan(0,1,0),new TimeSpan(0,0,1),new TimeSpan(0,0,1),new TimeSpan(1)});

		/// <summary>
		/// Parse a string and return a TimeSpan
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static TimeSpan Parse(string value)
		{
			string[] split = value.Split(' ');
			return ParseElements(split);
		}

		/// <summary>
		/// Parse the elements that make up a string returning a TimeSpan
		/// </summary>
		/// <param name="elements"></param>
		/// <returns></returns>
		private static TimeSpan ParseElements(string[] elements)
		{
			TimeSpan span = new TimeSpan();
			
			bool hasIntElement = false;
			bool hasIntWithNoUnit = false;

			int intElement = 0;
			for(int elementIndex = 0 ;elementIndex< elements.Length ; elementIndex++)
			{
				string element = elements[elementIndex];

				try
				{intElement = ParseElementForInteger(element);hasIntElement = true;}
				catch
				{hasIntElement = false;}

				if(elements.Length-1 > elementIndex)
				{
					try{ParseElementForInteger(elements[elementIndex+1]);hasIntWithNoUnit = true;}
					catch{hasIntWithNoUnit = false;}
				}else{hasIntWithNoUnit = true;}

				if(hasIntElement && hasIntWithNoUnit)
				{
					span = new TimeSpan((new TimeSpan(1,0,0).Ticks * intElement) + span.Ticks);
					hasIntWithNoUnit = false;
					hasIntElement = false;
				}
				else if(hasIntElement && elementIndex < elements.Length - 1)
				{
					string secondElement = elements[elementIndex + 1];
					TimeSpan duration = ParseElementForStringDuration(secondElement);
					if(duration != TimeSpan.Zero)
						span = new TimeSpan((duration.Ticks * intElement) + span.Ticks);

					elementIndex += 1;
					hasIntElement = false;
				}
				else
				{
					TimeSpan duration = ParseElementForStringDuration(element);
					if(duration != TimeSpan.Zero)
						span = new TimeSpan((duration.Ticks) + span.Ticks);
				}
			}

			return span;
		}

		/// <summary>
		/// This will parse a value for an integer
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		private static int ParseElementForInteger(string value)
		{
			return int.Parse(value);
		}

		/// <summary>
		/// Parse the string for a duration element
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		private static TimeSpan ParseElementForStringDuration(string value)
		{
			string duration = value.ToLower();
			if(duration.EndsWith("s"))
				duration = duration.Substring(0,duration.Length - 1);

			int durationIndex = ValidDurations.IndexOf(duration);
			if(durationIndex > -1)
				return (TimeSpan) ValidDurationLength[durationIndex];

			return TimeSpan.Zero;
		}

		#endregion

		#region Parse TimeSpan and Convert to String

		/// <summary>
		/// Parse a TimeSpan and return a string
		/// </summary>
		/// <param name="timeSpan"></param>
		/// <returns></returns>
		public static string Parse(TimeSpan timeSpan)
		{
			if(timeSpan.Equals(TimeSpan.Zero))
				return string.Empty;

			return GetFriendlyStringNameForDuration(timeSpan);
		}

		/// <summary>
		/// Gets the friendly name for a TimeSpan
		/// </summary>
		/// <param name="timeSpan"></param>
		/// <returns></returns>
		private static string GetFriendlyStringNameForDuration(TimeSpan timeSpan)
		{
			int years = CalculatePeriods("year",ref timeSpan);
			int months = CalculatePeriods("month",ref timeSpan);
			int weeks = CalculatePeriods("week",ref timeSpan);
			int days = CalculatePeriods("day",ref timeSpan);
			int hours = CalculatePeriods("hour",ref timeSpan);
			int minutes = CalculatePeriods("minute",ref timeSpan);
			int seconds = CalculatePeriods("second",ref timeSpan);
			int ticks = CalculatePeriods("ticks",ref timeSpan);

			StringBuilder builder = new StringBuilder();
			if(years > 0) builder.AppendFormat("{0} {1} ",years,GetPeriodNameForLength(years,"year"));
			if(months > 0) builder.AppendFormat("{0} {1} ",months,GetPeriodNameForLength(months,"month"));
			if(weeks > 0) builder.AppendFormat("{0} {1} ",weeks,GetPeriodNameForLength(weeks,"week"));
			if(days > 0) builder.AppendFormat("{0} {1} ",days,GetPeriodNameForLength(days,"day"));
			if(hours > 0) builder.AppendFormat("{0} {1} ",hours,GetPeriodNameForLength(hours,"hour"));
			if(minutes > 0) builder.AppendFormat("{0} {1} ",minutes,GetPeriodNameForLength(minutes,"minute"));
			if(seconds > 0) builder.AppendFormat("{0} {1} ",seconds,GetPeriodNameForLength(seconds,"second"));
			if(ticks > 0) builder.AppendFormat("{0} {1} ",ticks,GetPeriodNameForLength(ticks,"tick"));

			return builder.ToString().Trim();
		}

		/// <summary>
		/// Gets the name for a period defined by its length
		/// </summary>
		/// <param name="length"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		private static string GetPeriodNameForLength(int length, string name)
		{
			if(length == 1)
				return name;

			return name + "s";
		}

		/// <summary>
		/// Calculate the period length extracting the length from the timespan
		/// </summary>
		/// <param name="period"></param>
		/// <param name="timeSpan"></param>
		/// <returns></returns>
		private static int CalculatePeriods(string period, ref TimeSpan timeSpan)
		{
			int length = (int) (timeSpan.Ticks / ParseElementForStringDuration(period).Ticks);
			if(length > 0)
			{
				timeSpan = new TimeSpan( timeSpan.Ticks - (ParseElementForStringDuration(period).Ticks * length));
				return length;
			}
			else
				return 0;
		}

		#endregion
	}
}
