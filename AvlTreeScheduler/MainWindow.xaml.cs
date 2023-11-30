using AvlTreeScheduler.Classes;
using AvlTreeScheduler.ViewModels;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using Bitlush;
using System.Threading.Tasks;

namespace AvlTreeScheduler
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const int TimeLineRowHeight = 30;
        private readonly MainViewModel vm;
        private bool CanRenderEvents {  get; set; }
        private double WindowWidth { get; set; }
        private double WindowHeight {  get; set; }
        private double containerWidth {  get; set; }

        public MainWindow()
        {
            InitializeComponent();
            
            vm = new MainViewModel();
            DataContext = vm;
            vm.DataHandled += Vm_DataHandled;

            vm.CreateRandomEventsAsync();

            btnGenerateSchedule.Click += BtnGenerateSchedule_Click;
            scrollViewerOuter.ScrollChanged += ScrollViewerOuter_ScrollChanged;
            this.Loaded += MainWindow_Loaded;
            this.SizeChanged += MainWindow_SizeChanged;
        }
        private void Vm_DataHandled(object sender, EventArgs e)
        {
            txtGenerating.Visibility = Visibility.Hidden;

            // to make grid nicely scrollable with proper scale
            mainGrid.Width = vm.TimeLineEnd * 30;

            SetupTimeLines();
            
            DispatcherTimer timer = new DispatcherTimer();
            timer.Tick += Timer_Tick;
            timer.Interval = new TimeSpan(0, 0, 0, 0, 10);
            timer.Start();

            CanRenderEvents = true;
            btnGenerateSchedule.IsEnabled = true;
            rectCurrentTime.Margin = new Thickness();
            rectCurrentTime.Visibility = Visibility.Visible;
        }

        private void ScrollViewerOuter_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (CanRenderEvents)
            {
                DrawObservableAvlTree();
            }
        }
        private void BtnGenerateSchedule_Click(object sender, RoutedEventArgs e)
        {
            CanRenderEvents = false;
            btnGenerateSchedule.IsEnabled = false;

            txtGenerating.Visibility = Visibility.Visible;
            scrollViewerOuter.ScrollToLeftEnd();
            Task.Run(()=> vm.CreateRandomEventsAsync());
            ResetGraphic();
        }
        
        private void DrawEvent(TimeLineEvent timeLineEvent)
        {
            Grid eventGrid = new Grid();
            eventGrid.HorizontalAlignment = HorizontalAlignment.Left;
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

            eventGrid.Width = timeLineEvent.WidthMultiplayer * containerWidth;
            eventGrid.Margin = new Thickness(timeLineEvent.MarginMultiplayer * containerWidth, 2, 0, 2);

            eventGrid.Children.Add(rectangle);

            TextBlock textBlock = new TextBlock
            {
                Text = "Generic Name",
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(5, 0, 0, 0)
            };

            eventGrid.Children.Add(textBlock);
            Grid.SetZIndex(eventGrid, 10);
            Grid.SetRow(eventGrid, timeLineEvent.Layer);
        }
        private void DrawObservableAvlTree()
        {
            SetupTimeStampsGrid();
            containerWidth = mainGrid.ActualWidth;

            AvlNode<double, TimeLineEvent> node = vm.MainTree.Root;
            if (node != null)
            {
                HandleNode(node);
            }
        }
        private void HandleNode(AvlNode<double, TimeLineEvent> node)
        {
            //margin + width
            if (node.Key * containerWidth + node.Value.WidthMultiplayer * containerWidth < scrollViewerOuter.HorizontalOffset)
            {
                if (node.Right != null)
                {
                    HandleNode(node.Right);
                }
            }
            //just margin
            else if(node.Key * containerWidth > scrollViewerOuter.HorizontalOffset + WindowWidth)
            {
                if (node.Left != null)
                {
                    HandleNode(node.Left);
                }
            }
            else
            {
                TimeLineEvent timeLineEvent = node.Value;
                if (timeLineEvent.IsRendered == false)
                {
                    DrawEvent(timeLineEvent);
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

        private void Timer_Tick(object sender, EventArgs e)
        {
            rectCurrentTime.Margin = new Thickness(rectCurrentTime.Margin.Left+0.4, rectCurrentTime.Margin.Top, rectCurrentTime.Margin.Right, rectCurrentTime.Margin.Bottom);
        }
        private void ResetGraphic()
        {
            gridTimeStamps.Children.Clear();
            gridTimeLines.Children.Clear();
            gridTimeStampsMicroSteps.Children.Clear();
            gridTimeLines.RowDefinitions.Clear();
            rectCurrentTime.Visibility = Visibility.Collapsed;
        }
        
        /// <summary>
        /// Drawing ruler at the top
        /// </summary>
        private void SetupTimeStampsGrid()
        {
            int duration = vm.TimeLineEnd;
            int step = vm.RulerStep;

            double timelineDuration = duration;
            double containerWidth = gridTimeStamps.ActualWidth;

            for (int i = 0; i < duration; i += step)
            {
                double timeStampMargin = ((double)i) / timelineDuration * containerWidth;
                double timeStampWidth = (5.0 / timelineDuration) * containerWidth;

                if (timeStampMargin >= scrollViewerOuter.HorizontalOffset - WindowWidth / 5 &&
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

                    Border border = new Border
                    {
                        BorderThickness = new Thickness(2, 0, 0, 2),
                        BorderBrush = Brushes.Black
                    };

                    grid.Children.Add(border);
                    gridTimeStamps.Children.Add(grid);

                    for(int j = 1; j <= 5; j++)
                    {
                        double timeStampMicroMargin = ((double)(i+j)) / timelineDuration * containerWidth;

                        Border bor = new Border
                        {
                            Margin = new Thickness(timeStampMicroMargin, 0, 0, 0),
                            HorizontalAlignment = HorizontalAlignment.Left,
                            BorderThickness = new Thickness(2, 0, 0, 2),
                            BorderBrush = Brushes.Black
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

                Border border = new Border
                {
                    BorderBrush = Brushes.Black
                };
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
        private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            SetScrollViewOffset();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            SetScrollViewOffset();

            CanRenderEvents = true;
        }

    }
}
