using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

using Diploma.UI.ViewModels.Hypergraph;

namespace Diploma.UI.Views.Controls
{

    public partial class HypergraphView : UserControl
    {
        // TODO: unsubscribe
        public readonly Action<object, MouseEventArgs> _onMouseMove;
        public readonly Action<object, MouseButtonEventArgs> _onMouseUp;

        public HypergraphView()
        {
            InitializeComponent();
            DataContextChanged += (sender, eventArgs) =>
            {
                Redraw();
            };

            _onMouseMove = (sender, eventArgs) =>
            {
                if (ViewModel is null || ViewModel.CapturedSimplex is null)
                {
                    return;
                }
                if (ViewModel.CapturedSimplex.Center.Fill == Brushes.Gray)
                {
                    ViewModel.CapturedSimplex.Center.Fill = Brushes.Black;
                    for (var i = 0; i < ViewModel.CapturedSimplex.Edges.Length; i++)
                    {
                        ViewModel.CapturedSimplex.Edges[i].Stroke = Brushes.Black;
                    }
                }
                Canvas.SetLeft(ViewModel.CapturedSimplex.Center, Mouse.GetPosition(_canvas).X - SimplexViewModel.SimplexCenterRadius);
                if (Canvas.GetLeft(ViewModel.CapturedSimplex.Center) < 5)
                {
                    Canvas.SetLeft(ViewModel.CapturedSimplex.Center, 5);
                }
                if (Canvas.GetLeft(ViewModel.CapturedSimplex.Center) + SimplexViewModel.SimplexCenterRadius * 2 > _canvas.ActualWidth - 5)
                {
                    Canvas.SetLeft(ViewModel.CapturedSimplex.Center, _canvas.ActualWidth - SimplexViewModel.SimplexCenterRadius * 2 - 5);
                }
                Canvas.SetTop(ViewModel.CapturedSimplex.Center, Mouse.GetPosition(_canvas).Y - SimplexViewModel.SimplexCenterRadius);
                if (Canvas.GetTop(ViewModel.CapturedSimplex.Center) < 5)
                {
                    Canvas.SetTop(ViewModel.CapturedSimplex.Center, 5);
                }
                if (Canvas.GetTop(ViewModel.CapturedSimplex.Center) + SimplexViewModel.SimplexCenterRadius * 2 > _canvas.ActualHeight - 5)
                {
                    Canvas.SetTop(ViewModel.CapturedSimplex.Center, _canvas.ActualHeight - SimplexViewModel.SimplexCenterRadius * 2 - 5);
                }
                for (var i = 0; i < ViewModel.CapturedSimplex.Edges.Length; i++)
                {
                    ViewModel.CapturedSimplex.Edges[i].X1 = Canvas.GetLeft(ViewModel.CapturedSimplex.Center) + SimplexViewModel.SimplexCenterRadius;
                    ViewModel.CapturedSimplex.Edges[i].Y1 = Canvas.GetTop(ViewModel.CapturedSimplex.Center) + SimplexViewModel.SimplexCenterRadius;
                }
            };
            _onMouseUp = (sender, eventArgs) =>
            {
                if (ViewModel is null || ViewModel.CapturedSimplex is null)
                {
                    return;
                }
                ViewModel.CapturedSimplex.Center.Fill = Brushes.Gray;
                for (var i = 0; i < ViewModel.CapturedSimplex.Edges.Length; i++)
                {
                    ViewModel.CapturedSimplex.Edges[i].Stroke = Brushes.Gray;
                }
                ViewModel.CapturedSimplex = null;
            };

            Loaded += (sender, eventArgs) =>
            {
                var window = Window.GetWindow(this);

                var mainMenu = (Menu)window.FindName("_mainMenu");
                var tabs = (TabControl)window.FindName("_tabs");
                // TODO:
                //var selectedTab = (TabItem)VisualTreeHelper.GetParent(VisualTreeHelper.GetParent(this));
                //var simplexVerticesCountNumericUpDown = (NumericUpDown)selectedTab.FindName("_simplexVerticesCount");
                //var restoreHypergraphButton = (Button)selectedTab.FindName("_restoreHypergraphButton");
                //var clearButton = (Button)selectedTab.FindName("_clearButton");
                window.Closing += (_sender, _eventArgs) =>
                {
                    ViewModel?.Dispose();
                    window.MouseMove -= new MouseEventHandler(_onMouseMove);
                    window.MouseLeftButtonUp -= new MouseButtonEventHandler(_onMouseUp);
                    mainMenu.PreviewMouseLeftButtonUp -= new MouseButtonEventHandler(_onMouseUp);
                    //simplexVerticesCountNumericUpDown.PreviewMouseLeftButtonUp -= new MouseButtonEventHandler(_onMouseUp);
                    //restoreHypergraphButton.PreviewMouseLeftButtonUp -= new MouseButtonEventHandler(_onMouseUp);
                    //clearButton.PreviewMouseLeftButtonUp -= new MouseButtonEventHandler(_onMouseUp);
                    _canvas.MouseMove -= new MouseEventHandler(_onMouseMove);
                };
                window.MouseMove += new MouseEventHandler(_onMouseMove);
                window.MouseLeftButtonUp += new MouseButtonEventHandler(_onMouseUp);
                mainMenu.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(_onMouseUp);
                //simplexVerticesCountNumericUpDown.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(_onMouseUp);
                //restoreHypergraphButton.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(_onMouseUp);
                //clearButton.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(_onMouseUp);
                _canvas.MouseMove += new MouseEventHandler(_onMouseMove);
            };
        }

        private HypergraphViewModel ViewModel =>
            DataContext as HypergraphViewModel;

        public void Redraw()
        {
            _canvas.Children.Clear();
            if (ViewModel is null)
            {
                return;
            }
            foreach (var vertexViewModel in ViewModel.Vertices)
            {
                _canvas.Children.Add(vertexViewModel.VertexView);
                _canvas.Children.Add(vertexViewModel.VertexSimplicesViewModel.VertexSimplicesView);
            }
            foreach (var simplex in ViewModel.Simplices)
            {
                _canvas.Children.Add(simplex.Center);
                foreach (var edge in simplex.Edges)
                {
                    _canvas.Children.Add(edge);
                }
            }
        }

    }

}