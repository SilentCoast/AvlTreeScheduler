using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using PropertyChanged;
using System.Windows.Threading;
using System.Windows;
using AvlTreeScheduler.Classes;
using Bitlush;

namespace AvlTreeScheduler.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class MainViewModel
    {
        private const int MinEventDuration = 1;
        private const int MaxEventDuration = 15;
        public int EventsAmount { get; set; } = 8000;
        public int LayersAmount { get; set; } = 10;
        public int PendingAmount { get; set; }
        public int JeopardyAmount { get; set; }
        public int CompletedAmount { get; set; }

        public int TimeLineEnd { get; set; }
        public int RulerStep => TimeLineEnd / (TimeLineEnd / 5);
        public int GridRowDefinitionsCount { get; set; }

        public AvlTree<double, TimeLineEvent> MainTree { get; set; }

        public void CreateRandomEventsAsync()
        {
            GridRowDefinitionsCount = 0;
            if (EventsAmount < LayersAmount || EventsAmount >= int.MaxValue || EventsAmount < 15 || LayersAmount > 100)
            {
                MessageBox.Show("Make sure that the values you're entering are correct:\n" +
                    "Events >= Layers\nEvents >= 15\nLayers > 0\nLayers <= 100", "Wrong values");
                return;
            }

            //in order to make corrent ruler's sections with step of 5
            TimeLineEnd = (EventsAmount - (EventsAmount % 5));

            int minEventEntryAt = 0;
            int maxEventEntryAt = TimeLineEnd - MaxEventDuration;

            PendingAmount = 0;
            JeopardyAmount = 0;
            CompletedAmount = 0;

            //TODO: it does not account the fraction remaining after division. and it distributes events evenly which is not realistic
            int eventsPerLayer = EventsAmount / LayersAmount;

            MainTree = new AvlTree<double, TimeLineEvent>();

            Random random = new Random();

            //setup of timelines(layers) and events
            for (int i = 0; i < LayersAmount; i++)
            {
                for (int j = 0; j <= eventsPerLayer; j++)
                {
                    int duration = random.Next(MinEventDuration, MaxEventDuration + 1);
                    int start = random.Next(minEventEntryAt, maxEventEntryAt + 1);
                    int type = random.Next(1, 3 + 1);
                    switch (type)
                    {
                        case 1:
                            PendingAmount++;
                            break;
                        case 2:
                            JeopardyAmount++;
                            break;
                        case 3:
                            CompletedAmount++;
                            break;
                    }
                    double eventMarginMultiplayer = (double)start / (double)TimeLineEnd;

                    double eventWidthMultiplayer = (double)duration / (double)TimeLineEnd;

                    TimeLineEvent mEvent = new TimeLineEvent()
                    {
                        Duration = duration,
                        Start = start,
                        Type = type,
                        MarginMultiplayer = eventMarginMultiplayer,
                        WidthMultiplayer = eventWidthMultiplayer,
                        Layer = i
                    };
                    MainTree.Insert(eventMarginMultiplayer, mEvent);
                }
                GridRowDefinitionsCount++;
            }
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
            {
                DataHandled?.Invoke(this, EventArgs.Empty);
            }));
        }

        /// <summary>
        /// Shoots when TimeLines generated and ready to be rendered
        /// </summary>
        public event EventHandler DataHandled;
       
        private ObservableCollection<TimeLine> _timeLines = new ObservableCollection<TimeLine>();
        public ObservableCollection<TimeLine> TimeLines
        {
            get
            {
                return _timeLines;
            }
            set
            {
                _timeLines = value;
            }
        }
    }
}

