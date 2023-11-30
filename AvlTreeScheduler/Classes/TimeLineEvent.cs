using PropertyChanged;

namespace AvlTreeScheduler.Classes
{
    [AddINotifyPropertyChangedInterface]
    public class TimeLineEvent 
    {
        public int Start { get; set; }

        public int Duration { get; set; }

        /// <summary>
        /// 1 - Pending
        /// 2 - Jeopardy
        /// 3 - Completed
        /// </summary>
        public int Type { get; set; }
        public bool IsRendered { get; set; }
        public int Layer { get; set; }

        /// <summary>
        /// make sure to multiply it by containerWidth before using
        /// </summary>
        public double MarginMultiplayer { get; set; }
        /// <summary>
        /// make sure to multiply it by containerWidth before using
        /// </summary>
        public double WidthMultiplayer { get; set; }
    }
    
}

