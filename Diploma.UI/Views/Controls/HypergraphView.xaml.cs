using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

using Diploma.Hypergraph;
using Diploma.UI.ViewModels.Hypergraph;

namespace Diploma.UI.Views.Controls
{

    public partial class HypergraphView : UserControl
    {
        // TODO: unsubscribe
        private readonly Action<object, MouseEventArgs> _onMouseMove;
        private readonly Action<object, MouseButtonEventArgs> _onMouseUp;
        private HypergraphViewModel _viewModel;

        public HypergraphView()
        {
            InitializeComponent();
            DataContextChanged += (sender, eventArgs) =>
            {
                Rebuild();
                Redraw();
            };
            _onMouseMove = (sender, eventArgs) =>
            {
                if (ViewModel is null || ViewModel.FixedSimplex is null)
                {
                    return;
                }
                if (ViewModel.FixedSimplex.Center.Fill == Brushes.Gray)
                {
                    ViewModel.FixedSimplex.Center.Fill = Brushes.Black;
                    for (var i = 0; i < ViewModel.FixedSimplex.Edges.Length; i++)
                    {
                        ViewModel.FixedSimplex.Edges[i].Stroke = Brushes.Black;
                    }
                }
                Canvas.SetLeft(ViewModel.FixedSimplex.Center, Mouse.GetPosition(_canvas).X - SimplexViewModel.SimplexCenterRadius);
                if (Canvas.GetLeft(ViewModel.FixedSimplex.Center) < 0)
                {
                    Canvas.SetLeft(ViewModel.FixedSimplex.Center, 5);
                }
                if (Canvas.GetLeft(ViewModel.FixedSimplex.Center) + VertexViewModel.VertexRadius * 2 > _canvas.ActualWidth)
                {
                    Canvas.SetLeft(ViewModel.FixedSimplex.Center, _canvas.ActualWidth - VertexViewModel.VertexRadius * 2);
                }
                Canvas.SetTop(ViewModel.FixedSimplex.Center, Mouse.GetPosition(_canvas).Y - SimplexViewModel.SimplexCenterRadius);
                if (Canvas.GetTop(ViewModel.FixedSimplex.Center) < 0)
                {
                    Canvas.SetTop(ViewModel.FixedSimplex.Center, 5);
                }
                if (Canvas.GetTop(ViewModel.FixedSimplex.Center) + VertexViewModel.VertexRadius * 2 > _canvas.ActualHeight)
                {
                    Canvas.SetTop(ViewModel.FixedSimplex.Center, _canvas.ActualHeight - VertexViewModel.VertexRadius * 2);
                }
                for (var i = 0; i < ViewModel.FixedSimplex.Edges.Length; i++)
                {
                    ViewModel.FixedSimplex.Edges[i].X1 = Canvas.GetLeft(ViewModel.FixedSimplex.Center) + SimplexViewModel.SimplexCenterRadius;
                    ViewModel.FixedSimplex.Edges[i].Y1 = Canvas.GetTop(ViewModel.FixedSimplex.Center) + SimplexViewModel.SimplexCenterRadius;
                }
            };
            _onMouseUp = (sender, eventArgs) =>
            {
                if (ViewModel is null || ViewModel.FixedSimplex is null)
                {
                    return;
                }
                ViewModel.FixedSimplex.Center.Fill = Brushes.Gray;
                for (var i = 0; i < ViewModel.FixedSimplex.Edges.Length; i++)
                {
                    ViewModel.FixedSimplex.Edges[i].Stroke = Brushes.Gray;
                }
                ViewModel.FixedSimplex = null;
            };
            Loaded += (sender, eventArgs) =>
            {
                Window.GetWindow(this).Closing += (_sender, _eventArgs) =>
                {
                    ViewModel?.Dispose();
                };
                Window.GetWindow(this).MouseMove += new MouseEventHandler(_onMouseMove);
                Window.GetWindow(this).MouseLeftButtonUp += new MouseButtonEventHandler(_onMouseUp);
                ((Menu)Window.GetWindow(this).FindName("_mainMenu")).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(_onMouseUp);
                ((NumericUpDown)Window.GetWindow(this).FindName("_simplexVerticesCount")).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(_onMouseUp);
                ((Button)Window.GetWindow(this).FindName("_restoreHypergraphButton")).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(_onMouseUp);
                _canvas.MouseMove += new MouseEventHandler(_onMouseMove);
            };
        }

        private HypergraphViewModel ViewModel
        {
            get =>
                _viewModel;

            set =>
                _viewModel = value;
        }

        public void Rebuild()
        {
            ViewModel?.Dispose();
            ViewModel = null;
            var model = (HypergraphModel)DataContext;
            if (model is null)
            {
                return;
            }
            ViewModel = new HypergraphViewModel(model, this, _onMouseMove);
        }

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
                _canvas.Children.Add(vertexViewModel.VertexSimplicesView);
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