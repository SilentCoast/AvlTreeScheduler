using System.Windows;

namespace AvlTreeScheduler.Converters
{
    public class ReverseBooleanToVisibilityConverter : BooleanConverter<Visibility>
    {
        public ReverseBooleanToVisibilityConverter() : 
            base(Visibility.Collapsed, Visibility.Visible)
        {
        }
    }
}
