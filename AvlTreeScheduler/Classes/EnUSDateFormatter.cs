using System;
using System.Globalization;

namespace AvlTreeScheduler.Classes
{
    public class EnUSDateFormatter : IDateFormatter
    {
        public string GetFormattedDate()
        {
            CultureInfo culture = new CultureInfo("en-US");
            DateTime today = DateTime.Today;
            string month = today.ToString("MMMM", culture);
            int day = today.Day;
            string suffix;
            switch (day)
            {
                case 1: suffix = "st"; break;
                case 2: suffix = "nd"; break;
                case 3: suffix = "rd"; break;
                default: suffix = "th"; break;
            }
            string dayOfWeek = today.DayOfWeek.ToString();
            return $"{month} {day}{suffix} {dayOfWeek}";
        }
    }
}
