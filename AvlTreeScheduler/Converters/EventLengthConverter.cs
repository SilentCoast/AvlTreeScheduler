using System;
using System.Windows.Data;
using System.Windows;

namespace AvlTreeScheduler.Classes
{
    /// <summary>
    /// Converts TimeLineEvent's value to Thickness/Width with right proportions
    /// </summary>
    public class EventLengthConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double timelineDuration = (double)(int)values[0];

            double eventStartTime = (double)(int)values[1];
            double eventDuration = (double)(int)values[2];

            double eventMarginMultiplayer = eventStartTime / timelineDuration;
            double containerWidth = (double)values[3];
            double eventMargin = eventMarginMultiplayer * containerWidth;

            double eventWidthMultiplayer = eventDuration / timelineDuration;
            double eventWidth = eventWidthMultiplayer * containerWidth;

            if (targetType == typeof(Thickness))
            {
                return new Thickness(eventMargin, 2, 0, 2);
            }
            else
            {
                return eventWidth;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
