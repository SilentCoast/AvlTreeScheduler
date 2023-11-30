using AvlTreeScheduler.Classes;
using AvlTreeScheduler.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace AvlTreeScheduler
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const int TimeLineRowHeight = 30;
        private readonly MainViewModel vm;
        private double WindowWidth { get; set; }
        private double WindowHeight {  get; set; }
        public MainWindow()
        {
            InitializeComponent();
            
            vm = new MainViewModel();
            DataContext = vm;
            vm.DataHandled += Vm_DataHandled;
            vm.WrongInputDetected += Vm_WrongInputDetected;

            btnGenerateSchedule.Click += BtnGenerateSchedule_Click;
            scrollViewerOuter.ScrollChanged += ScrollViewerOuter_ScrollChanged;
            this.Loaded += MainWindow_Loaded;
            this.SizeChanged += MainWindow_SizeChanged;

            vm.CreateRandomEvents();
        }

        private void Vm_WrongInputDetected(object sender, MessageEventArgs e)
        {
            MessageBox.Show(e.Message, e.Caption);
        }

        private void Vm_DataHandled(object sender, EventArgs e)
        {
            // to make grid nicely scrollable with proper scale
            mainGrid.Width = vm.TimeLineEnd * 30;

            SetupTimeLines();
            
            DispatcherTimer timer = new DispatcherTimer();
            timer.Tick += Timer_Tick;
            timer.Interval = new TimeSpan(0, 0, 0, 0, 10);
            timer.Start();

            rectCurrentTime.Margin = new Thickness();
        }
        private void ScrollViewerOuter_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (!vm.IsGenerating)
            {
                DrawVisibleEvents();
            }
        }
        private void BtnGenerateSchedule_Click(object sender, RoutedEventArgs e)
        {
            if (vm.IsValidParams())
            {
                scrollViewerOuter.ScrollToLeftEnd();
                ResetGraphic();
                Task.Run(() => { vm.CreateRandomEvents(); });
            }
        }
        private void DrawVisibleEvents()
        {
            DrawRuler();
            double containerWidth = mainGrid.ActualWidth;
            List<TimeLineEvent> events = vm.GetVisibleEvents(containerWidth, scrollViewerOuter.HorizontalOffset,
                scrollViewerOuter.HorizontalOffset + WindowWidth);

            foreach(var timeLineEvent in events)
            {
                DrawEvent(timeLineEvent);
            }

            void DrawEvent(TimeLineEvent timeLineEvent)
            {
                Grid eventGrid = new Grid
                {
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Width = timeLineEvent.WidthMultiplayer * containerWidth,
                    Margin = new Thickness(timeLineEvent.MarginMultiplayer * containerWidth, 2, 0, 2)
                };
                gridTimeLines.Children.Add(eventGrid);

                Rectangle rectangle = new Rectangle
                {
                    StrokeThickness = 3,
                    Stroke = Brushes.Black,
                    Opacity = 0.8
                };
                switch (timeLineEvent.Type)
                {
                    case 1:
                        rectangle.Fill = Brushes.Orange;
                        break;
                    case 2:
                        rectangle.Fill = Brushes.Red;
                        break;
                    case 3:
                        rectangle.Fill = Brushes.LightGreen;
                        break;
                    default:
                        rectangle.Fill = Brushes.Gray;
                        break;
                }
                eventGrid.Children.Add(rectangle);

                TextBlock textBlock = new TextBlock
                {
                    Text = "Generic Name",
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(5, 0, 0, 0)
                };

                eventGrid.Children.Add(textBlock);
                Panel.SetZIndex(eventGrid, 10);
                Grid.SetRow(eventGrid, timeLineEvent.Layer);
            }
        }
        private void ResetGraphic()
        {
            gridTimeStamps.Children.Clear();
            gridTimeStampsMicroSteps.Children.Clear();
            gridTimeLines.Children.Clear();
            gridTimeLines.RowDefinitions.Clear();
        }
        /// <summary>
        /// Draws timeline ruler at the top
        /// </summary>
        private void DrawRuler()
        {
            int duration = vm.TimeLineEnd;

            double timelineDuration = duration;
            double containerWidth = gridTimeStamps.ActualWidth;

            for (int i = 0; i < duration; i += vm.RulerStep)
            {
                double timeStampMargin = ((double)i) / timelineDuration * containerWidth;
                double timeStampWidth = ((double)MainViewModel.RulerStepValue / timelineDuration) * containerWidth;

                if (timeStampMargin >= scrollViewerOuter.HorizontalOffset - WindowWidth / MainViewModel.RulerStepValue &&
                    timeStampMargin <= scrollViewerOuter.HorizontalOffset + WindowWidth)
                {
                    Grid grid = new Grid
                    {
                        HorizontalAlignment = HorizontalAlignment.Left,
                        Margin = new Thickness(timeStampMargin, 0, 0, 0),
                        Width = timeStampWidth * 2
                    };

                    TextBlock textBlock = new TextBlock
                    {
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Top,
                        Text = i.ToString(),
                        Margin = new Thickness(5, 0, 0, 0)
                    };

                    grid.Children.Add(textBlock);

                    Style borderStyle = FindResource("blackBorder") as Style;

                    Border border = new Border
                    {
                        Style = borderStyle
                    };

                    grid.Children.Add(border);
                    gridTimeStamps.Children.Add(grid);

                    for(int j = 1; j <= MainViewModel.RulerStepValue; j++)
                    {
                        double timeStampMicroMargin = ((double)(i+j)) / timelineDuration * containerWidth;

                        Border bor = new Border
                        {
                            Margin = new Thickness(timeStampMicroMargin, 0, 0, 0),
                            HorizontalAlignment = HorizontalAlignment.Left,
                            Style = borderStyle
                        };

                        gridTimeStampsMicroSteps.Children.Add(bor);
                        Grid.SetRow(bor, 3);
                    }
                }
            }
        }
        /// <summary>
        /// Sets up grid rows according to viewmodel data
        /// </summary>
        private void SetupTimeLines()
        {
            for (int i = 0; i < vm.GridRowDefinitionsCount; i++)
            {
                gridTimeLines.RowDefinitions.Add(new RowDefinition() { MinHeight = TimeLineRowHeight, MaxHeight = TimeLineRowHeight });

                Border border = new Border();
                if (i % 2 == 0)
                {
                    border.Background = Brushes.AliceBlue;
                }
                else
                {
                    border.Background = Brushes.LightGray;
                }
                gridTimeLines.Children.Add(border);
                Grid.SetRow(border, i);
            }
        }
        private void SetScrollViewOffset()
        {
            WindowWidth = this.RenderSize.Width;
            WindowHeight = this.RenderSize.Height;
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            rectCurrentTime.Margin = new Thickness(rectCurrentTime.Margin.Left + 0.4, rectCurrentTime.Margin.Top, rectCurrentTime.Margin.Right, rectCurrentTime.Margin.Bottom);
        }
        private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            SetScrollViewOffset();
        }
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            SetScrollViewOffset();
        }
    }
}
