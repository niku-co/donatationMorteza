using System;
using System.Globalization;
using System.Runtime.InteropServices;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Common.Utilities
{
    public static class DateTimeExtension
    {
        public const string Format = "yyMMddHHmmss";
        public const string ShortFormat = "yyMMdd";
        public static string ToLongStringFormat(this long dateTime)
        {
            DateTime.TryParseExact(dateTime.ToString(), Format, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt);

            var p = new PersianCalendar();
            try
            {
                var date = $"{p.GetYear(dt)}/{p.GetMonth(dt):00.##}/{p.GetDayOfMonth(dt):00.##}";
                var time = $"{dt.Hour:00.##}:{dt.Minute:00.##}";
                return $"{date}-{time}";
            }
            catch
            {
                return $"{dateTime}-";
            }

        }
        public static string ToPersianDateStringFormat(this DateTime dateTime)
        {

            var p = new PersianCalendar();
            try
            {
                var date = $"{p.GetYear(dateTime)}/{p.GetMonth(dateTime):00.##}/{p.GetDayOfMonth(dateTime):00.##}";
                var time = $"{dateTime.Hour:00.##}:{dateTime.Minute:00.##}";
                return $"{date}-{time}";
            }
            catch
            {
                return $"{dateTime}-";
            }

        }

        public static string ToPersianShortDateStringFormat(this DateTime dateTime)
        {

            var p = new PersianCalendar();
            try
            {
                var date = $"{p.GetYear(dateTime)}/{p.GetMonth(dateTime):00.##}/{p.GetDayOfMonth(dateTime):00.##}";
                return $"{date}";
            }
            catch
            {
                return $"{dateTime}-";
            }

        }

        public static string ToPersianShortMonthDateStringFormat(this DateTime dateTime)
        {

            var p = new PersianCalendar();
            try
            {
                var date = $"{p.GetYear(dateTime)}/{p.GetMonth(dateTime):00.##}";
                return $"{date}";
            }
            catch
            {
                return $"{dateTime}-";
            }

        }
        public static string ToShortStringFormat(this long dateTime)
        {
            DateTime.TryParseExact(dateTime.ToString(), Format, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt);
            try
            {
                var p = new PersianCalendar();
                var date = $"{p.GetYear(dt)}/{p.GetMonth(dt):00.##}/{p.GetDayOfMonth(dt):00.##}";
                return date;
            }
            catch
            {
                return dt.ToString();
            }

        }
        public static string ToShortStringFormatGarigory(this long dateTime)
        {
            DateTime.TryParseExact(dateTime.ToString(), Format, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt);

            var date = dt.ToString();
            return date;
        }

        public static string ToShortStringTimeFormatGarigory(this long dateTime)
        {
            DateTime.TryParseExact(dateTime.ToString(), Format, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt);

            var date = dt.Hour + ":" + dt.Minute.ToString();
            return date;
        }
        public static long ToShortLongFormat(this long dateTime)
        {
            DateTime.TryParseExact(dateTime.ToString(), Format, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt);

            var date = dt.ToString(ShortFormat);
            return long.Parse(date, CultureInfo.InvariantCulture);
        }
        public static long ToShortDate(this long dateTime)
        {
            DateTime dt = new DateTime(dateTime);
            return long.Parse(dt.ToString(ShortFormat));
        }

        public static DateTime ToShortDateTime(this string dateTime)
        {
            var dt = DateTime.Parse(dateTime);
            return dt;
        }

        public static long GetLongDate(this string dateTime, string format = ShortFormat)
        {
            try
            {
                var dt = DateTime.Parse(dateTime);
                return long.Parse(dt.ToString(format));
            }
            catch (Exception e)
            {
                return long.Parse(DateTime.UtcNow.ConvertTimeZone().ToString(format));
            }
        }

        public static DateTime ConvertTimeZone(this DateTime now)
        {
            TimeZoneInfo easternStandardTime;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                easternStandardTime = TimeZoneInfo.FindSystemTimeZoneById("Iran Standard Time");
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                easternStandardTime = TimeZoneInfo.FindSystemTimeZoneById("Asia/Tehran");
            else
                easternStandardTime = TimeZoneInfo.FindSystemTimeZoneById("don't find!");

            DateTime iranTime = TimeZoneInfo.ConvertTimeFromUtc(now, easternStandardTime);
            return iranTime;
        }
        public static DateTime GetGregorianDateTime()
        {
            PersianCalendar pc = new PersianCalendar();
            DateTime fdate = Convert.ToDateTime("1400-07-18 08:27:39");
            GregorianCalendar gcalendar = new GregorianCalendar();
            DateTime eDate = pc.ToDateTime(
                gcalendar.GetYear(fdate),
                gcalendar.GetMonth(fdate),
                gcalendar.GetDayOfMonth(fdate),
                gcalendar.GetHour(fdate),
                gcalendar.GetMinute(fdate),
                gcalendar.GetSecond(fdate), 0);
            return eDate;
        }
    }
    public static class NativeDateTime
    {
        public static DateTime Now()
        {
            return DateTime.UtcNow.ConvertTimeZone();
        }
    }
}
