using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;

namespace AvlTreeScheduler.Classes
{
    public static class DateFormatter
    {
        public static string GetFormatedDate()
        {
            CultureInfo culture = new CultureInfo("en-US");
            DateTime today = DateTime.Today;
            string month = today.ToString("MMMM",culture);
            int day = today.Day;
            string suffix = GetSuffix(day);
            string dayOfWeek = today.DayOfWeek.ToString();
            return $"{month} {day}{suffix} {dayOfWeek}";
        }
        private static string GetSuffix(int day)
        {
            switch (day)
            {
                case 1: return "st";
                case 2: return "nd";
                case 3: return "rd";
                default: return "th";
            }
        }
    }
}
