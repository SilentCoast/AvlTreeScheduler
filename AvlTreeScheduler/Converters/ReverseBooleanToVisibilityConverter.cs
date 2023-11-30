using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
