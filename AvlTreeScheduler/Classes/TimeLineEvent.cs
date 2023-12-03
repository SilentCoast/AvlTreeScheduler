using PropertyChanged;

namespace AvlTreeScheduler.Classes
{
    [AddINotifyPropertyChangedInterface]
    public class TimeLineEvent
    {
        /// <summary>
        /// Ratio of Start of the event to the Duration of the TimeLine
        /// </summary>
        public double StartRatio { get; set; }
        /// <summary>
        /// Ratio of Duration of the event to the Duration of the TimeLine
        /// </summary>
        public double DurationRatio { get; set; }
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
    }
}

