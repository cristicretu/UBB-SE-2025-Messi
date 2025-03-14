using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Duo.Helpers
{
    public static class DateTimeHelper
    {
        public const string DefaultDateFormat = "yyyy-MM-dd";
        public const string DefaultDateTimeFormat = "yyyy-MM-dd HH:mm:ss";
        
        /// <summary>
        /// get current time
        /// </summary>
        public static DateTime GetCurrentTime()
        {
            return DateTime.Now;
        }

        /// <summary>
        /// get current time in UTC
        /// </summary>
        public static DateTime GetCurrentTimeUtc()
        {
            return DateTime.UtcNow;
        }

        /// <summary>
        /// convert UTC datetime to local timezone
        /// </summary>
        public static DateTime ConvertUtcToLocal(DateTime utcDateTime)
        {
            if (utcDateTime.Kind != DateTimeKind.Utc)
            {
                // Force the kind to be UTC if it's not already
                utcDateTime = DateTime.SpecifyKind(utcDateTime, DateTimeKind.Utc);
            }
            return utcDateTime.ToLocalTime();
        }

        /// <summary>
        /// convert local datetime to UTC
        /// </summary>
        public static DateTime ConvertLocalToUtc(DateTime localDateTime)
        {
            if (localDateTime.Kind != DateTimeKind.Local)
            {
                localDateTime = DateTime.SpecifyKind(localDateTime, DateTimeKind.Local);
            }
            return localDateTime.ToUniversalTime();
        }

        public static bool TryParseDateTime(string dateTimeString, out DateTime result)
        {
            return DateTime.TryParse(dateTimeString, CultureInfo.InvariantCulture, 
                DateTimeStyles.AdjustToUniversal, out result);
        }

        public static DateTime? ParseDateTime(string dateTimeString, string? format = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dateTimeString))
                    return null;

                if (format != null)
                {
                    if (DateTime.TryParseExact(dateTimeString, format, CultureInfo.InvariantCulture, 
                        DateTimeStyles.None, out DateTime result))
                    {
                        return result;
                    }
                }
                
                if (DateTime.TryParse(dateTimeString, CultureInfo.InvariantCulture, 
                    DateTimeStyles.None, out DateTime parsedDate))
                {
                    return parsedDate;
                }
                
                return null;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// format a DateTime as a standard date string (yyyy-MM-dd)
        /// </summary>
        public static string FormatAsDate(DateTime? dateTime)
        {
            return dateTime?.ToString(DefaultDateFormat) ?? string.Empty;
        }

        /// <summary>
        /// format a DateTime as a standard datetime string (yyyy-MM-dd HH:mm:ss)
        /// </summary>
        public static string FormatAsDateTime(DateTime? dateTime)
        {
            return dateTime?.ToString(DefaultDateTimeFormat) ?? string.Empty;
        }

        /// <summary>
        /// format a DateTime using a custom format
        /// </summary>
        public static string Format(DateTime? dateTime, string format)
        {
            return dateTime?.ToString(format) ?? string.Empty;
        }

        /// <summary>
        ///human-readable relative time string (e.g., "5 minutes ago")
        /// </summary>
        public static string GetRelativeTime(DateTime? dateTime)
        {
            if (!dateTime.HasValue)
                return string.Empty;
                
            var timeSpan = DateTime.Now - dateTime.Value;

            if (timeSpan.TotalDays > 365)
            {
                int years = (int)Math.Floor(timeSpan.TotalDays / 365);
                return years == 1 ? "1 year ago" : $"{years} years ago";
            }

            if (timeSpan.TotalDays > 30)
            {
                int months = (int)Math.Floor(timeSpan.TotalDays / 30);
                return months == 1 ? "1 month ago" : $"{months} months ago";
            }

            if (timeSpan.TotalDays > 7)
            {
                int weeks = (int)Math.Floor(timeSpan.TotalDays / 7);
                return weeks == 1 ? "1 week ago" : $"{weeks} weeks ago";
            }

            if (timeSpan.TotalDays >= 1)
            {
                int days = (int)Math.Floor(timeSpan.TotalDays);
                return days == 1 ? "1 day ago" : $"{days} days ago";
            }

            if (timeSpan.TotalHours >= 1)
            {
                int hours = (int)Math.Floor(timeSpan.TotalHours);
                return hours == 1 ? "1 hour ago" : $"{hours} hours ago";
            }

            if (timeSpan.TotalMinutes >= 1)
            {
                int minutes = (int)Math.Floor(timeSpan.TotalMinutes);
                return minutes == 1 ? "1 minute ago" : $"{minutes} minutes ago";
            }

            if (timeSpan.TotalSeconds >= 10)
            {
                int seconds = (int)Math.Floor(timeSpan.TotalSeconds);
                return seconds == 1 ? "1 second ago" : $"{seconds} seconds ago";
            }

            return "Just now";
        }
        
        /// <summary>
        /// ensures that a DateTime is stored in UTC format for database storage
        /// </summary>
        public static DateTime EnsureUtcKind(DateTime dateTime)
        {
            if (dateTime.Kind == DateTimeKind.Utc)
                return dateTime;
                
            if (dateTime.Kind == DateTimeKind.Local)
                return dateTime.ToUniversalTime();
                
            // Unspecified kind - assume it's already UTC
            return DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
        }
        
        /// <summary>
        /// formats a database (UTC) datetime for display in local timezone
        /// </summary>
        public static string FormatDbDateTimeForDisplay(DateTime? utcDateTime, string? format = null)
        {
            if (!utcDateTime.HasValue)
                return string.Empty;
                
            var localDateTime = ConvertUtcToLocal(utcDateTime.Value);
            return format == null ? 
                FormatAsDateTime(localDateTime) : 
                Format(localDateTime, format);
        }
    }
}