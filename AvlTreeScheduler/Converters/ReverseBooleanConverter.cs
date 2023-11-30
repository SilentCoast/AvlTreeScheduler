using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AvlTreeScheduler.Converters
{
    public sealed class ReverseBooleanConverter : BooleanConverter<bool>
    {
        public ReverseBooleanConverter() :
            base(false, true)
        { }
    }
}
