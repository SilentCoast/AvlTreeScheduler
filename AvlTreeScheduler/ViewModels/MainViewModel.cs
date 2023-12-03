using System;
using PropertyChanged;
using System.Windows.Threading;
using System.Windows;
using AvlTreeScheduler.Classes;
using Bitlush;
using System.Collections.Generic;

namespace AvlTreeScheduler.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class MainViewModel
    {
        #region consts
        private const int MinEventDuration = 1;
        private const int MaxEventDuration = 15;

        private const int MaxEventsAmount = 1000000;
        private const int MinEventsAmount = 15;
        private const int MaxLayersAmount = 100;

        public const int RulerStepValue = 5;
        #endregion
        public string Date {  get; set; }
        public int EventsAmount { get; set; } = 8000;
        public int LayersAmount { get; set; } = 10;
        public int PendingAmount { get; set; }
        public int JeopardyAmount { get; set; }
        public int CompletedAmount { get; set; }

        public bool IsGenerating {  get; set; }

        public int TimeLineEnd { get; set; }
        public int RulerStep => TimeLineEnd / (TimeLineEnd / RulerStepValue);
        public int GridRowDefinitionsCount { get; set; }

        public AvlTree<double, TimeLineEvent> MainTree { get; set; }
        public MainViewModel(IDateFormatter dateFormatter)
        {
            Date = dateFormatter.GetFormattedDate();
        }
        public bool IsValidParams()
        {
            string message = null;
            if (EventsAmount > MaxEventsAmount || EventsAmount < MinEventsAmount || LayersAmount > MaxLayersAmount)
            {
                message = $"Please check the values:\nLayers = {LayersAmount} (Allowed [1 - {MaxLayersAmount}])\n" +
                    $"Events = {EventsAmount} (Allowed [{MinEventsAmount} - {MaxEventsAmount}])";
            }
            if(message == null)
            {
                return true;
            }
            else
            {
                WrongInputDetected?.Invoke(this, new MessageEventArgs() { Message = message, Caption = "Wrong input" });
                return false;
            }
        }
        /// <summary>
        /// Populates the MainTree with random TimeLineEvents
        /// </summary>
        /// <returns>true if population is successfull, otherwise false</returns>
        public void CreateRandomEvents()
        {
            IsGenerating = true;
            GridRowDefinitionsCount = 0;
            //in order to make correct ruler's sections with proper steping
            TimeLineEnd = EventsAmount - (EventsAmount % RulerStepValue);

            int minEventEntryAt = 0;
            int maxEventEntryAt = TimeLineEnd - MaxEventDuration;

            PendingAmount = 0;
            JeopardyAmount = 0;
            CompletedAmount = 0;

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
            IsGenerating = false;

            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
            {
                DataHandled?.Invoke(this, EventArgs.Empty);
            }));
        }
        public List<TimeLineEvent> GetVisibleEvents(double containerWidth, double leftBoundary, double rigthBoundary)
        {
            List<TimeLineEvent> events = new List<TimeLineEvent>();
            AvlNode<double, TimeLineEvent> rootNode = MainTree.Root;
            if (rootNode != null)
            {
                HandleNode(rootNode);
            }
            void HandleNode(AvlNode<double, TimeLineEvent> node)
            {
                //check if the node is inside the boundaries.
                //If it is lower than leftBoundary - going to the right node
                if (node.Key * containerWidth + node.Value.WidthMultiplayer * containerWidth < leftBoundary)
                {
                    if (node.Right != null)
                    {
                        HandleNode(node.Right);
                    }
                }
                //if it is higher than rightBoundary - going to the left node
                else if (node.Key * containerWidth > rigthBoundary)
                {
                    if (node.Left != null)
                    {
                        HandleNode(node.Left);
                    }
                }
                //if it is inside the boundaries - going in both the left and the right nodes
                else
                {
                    TimeLineEvent timeLineEvent = node.Value;
                    if (timeLineEvent.IsRendered == false)
                    {
                        events.Add(timeLineEvent);
                    }
                    if (node.Left != null)
                    {
                        HandleNode(node.Left);
                    }
                    if (node.Right != null)
                    {
                        HandleNode(node.Right);
                    }
                }
            }
            return events;
        }
        /// <summary>
        /// Shoots when TimeLines generated and ready to be rendered
        /// </summary>
        public event EventHandler DataHandled;
        public delegate void MessageEventHandler(object sender, MessageEventArgs e);
        public event MessageEventHandler WrongInputDetected;
    }
}

