using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

using Diploma.Hypergraph;
using Diploma.UI.ViewModels.Hypergraph;

namespace Diploma.UI.Views.Controls
{

    public partial class HypergraphView : UserControl
    {
        // TODO: unsubscribe
        private Action<object, MouseEventArgs> _onMouseMove;
        private Action<object, MouseButtonEventArgs> _onMouseUp;

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
                if (_viewModel.FixedSimplex is null)
                {
                    return;
                }
                if (_viewModel.FixedSimplex.Center.Fill == Brushes.Gray)
                {
                    _viewModel.FixedSimplex.Center.Fill = Brushes.Black;
                    for (var i = 0; i < _viewModel.FixedSimplex.Edges.Length; i++)
                    {
                        _viewModel.FixedSimplex.Edges[i].Stroke = Brushes.Black;
                    }
                }
                Canvas.SetLeft(_viewModel.FixedSimplex.Center, Mouse.GetPosition(_canvas).X - SimplexViewModel.SimplexCenterRadius);
                if (Canvas.GetLeft(_viewModel.FixedSimplex.Center) < 0)
                {
                    Canvas.SetLeft(_viewModel.FixedSimplex.Center, 5);
                }
                if (Canvas.GetLeft(_viewModel.FixedSimplex.Center) + VertexViewModel.VertexRadius * 2 > _canvas.ActualWidth)
                {
                    Canvas.SetLeft(_viewModel.FixedSimplex.Center, _canvas.ActualWidth - VertexViewModel.VertexRadius * 2);
                }
                Canvas.SetTop(_viewModel.FixedSimplex.Center, Mouse.GetPosition(_canvas).Y - SimplexViewModel.SimplexCenterRadius);
                if (Canvas.GetTop(_viewModel.FixedSimplex.Center) < 0)
                {
                    Canvas.SetTop(_viewModel.FixedSimplex.Center, 5);
                }
                if (Canvas.GetTop(_viewModel.FixedSimplex.Center) + VertexViewModel.VertexRadius * 2 > _canvas.ActualHeight)
                {
                    Canvas.SetTop(_viewModel.FixedSimplex.Center, _canvas.ActualHeight - VertexViewModel.VertexRadius * 2);
                }
                for (var i = 0; i < _viewModel.FixedSimplex.Edges.Length; i++)
                {
                    _viewModel.FixedSimplex.Edges[i].X1 = Canvas.GetLeft(_viewModel.FixedSimplex.Center) + SimplexViewModel.SimplexCenterRadius;
                    _viewModel.FixedSimplex.Edges[i].Y1 = Canvas.GetTop(_viewModel.FixedSimplex.Center) + SimplexViewModel.SimplexCenterRadius;
                }
            };
            _onMouseUp = (sender, eventArgs) =>
            {
                if (_viewModel.FixedSimplex is null)
                {
                    return;
                }
                _viewModel.FixedSimplex.Center.Fill = Brushes.Gray;
                for (var i = 0; i < _viewModel.FixedSimplex.Edges.Length; i++)
                {
                    _viewModel.FixedSimplex.Edges[i].Stroke = Brushes.Gray;
                }
                _viewModel.FixedSimplex = null;
            };
            Loaded += (sender, eventArgs) =>
            {
                Window.GetWindow(this).Closing += (_sender, _eventArgs) =>
                {
                    _viewModel?.Dispose();
                };
                Window.GetWindow(this).MouseMove += new MouseEventHandler(_onMouseMove);
                Window.GetWindow(this).MouseLeftButtonUp += new MouseButtonEventHandler(_onMouseUp);
                ((Menu)Window.GetWindow(this).FindName("_mainMenu")).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(_onMouseUp);
                ((NumericUpDown)Window.GetWindow(this).FindName("_simplexVerticesCount")).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(_onMouseUp);
                ((Button)Window.GetWindow(this).FindName("_restoreHypergraphButton")).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(_onMouseUp);
                _canvas.MouseMove += new MouseEventHandler(_onMouseMove);
            };
        }

        public void Rebuild()
        {
            _viewModel?.Dispose();
            var model = (HypergraphModel)DataContext;
            if (model is null)
            {
                return;
            }
            _viewModel = new HypergraphViewModel(model, this, _onMouseMove);
        }

        public void Redraw()
        {
            _canvas.Children.Clear();
            foreach (var vertexViewModel in _viewModel.Vertices)
            {
                _canvas.Children.Add(vertexViewModel.VertexView);
                _canvas.Children.Add(vertexViewModel.VertexSimplicesView);
            }

            foreach (var simplex in _viewModel.Simplices)
            {
                _canvas.Children.Add(simplex.Center);
                foreach (var edge in simplex.Edges)
                {
                    _canvas.Children.Add(edge);
                }
            }

            //foreach (var vertexOrSimplexCenter in _canvas.Children.OfType<Ellipse>().ToArray())
            //{
            //    if (!(vertexOrSimplexCenter.Tag is null) && !_simplicesViews.ContainsKey(vertexOrSimplexCenter))
            //    {
            //        (vertexOrSimplexCenter.Tag as Timer).Dispose();
            //    }
            //    _canvas.Children.Remove(vertexOrSimplexCenter);
            //    if (_vertexSimplicesViews.Keys.Contains(vertexOrSimplexCenter))
            //    {
            //        (_vertexSimplicesViews[vertexOrSimplexCenter].Tag as Timer).Dispose();
            //        _vertexSimplicesViews.Remove(vertexOrSimplexCenter);
            //    }
            //    else if (_simplicesViews.Keys.Contains(vertexOrSimplexCenter))
            //    {
            //        _simplicesViews.Remove(vertexOrSimplexCenter);
            //    }
            //}
            //foreach (var simplexPart in _canvas.Children.OfType<Line>().ToArray())
            //{
            //    _canvas.Children.Remove(simplexPart);
            //}
            //if (Hypergraph is null)
            //{
            //    return;
            //}

            //// simplices
            //foreach (var simplex in Hypergraph.Simplices)
            //{
            //    var verticesCenterPoints = simplex.Vertices.Select(vertex => GetVertexCenterPoint(vertex.Id));
            //    var simplexCenterPoint = new Point(verticesCenterPoints.Select(vertex => vertex.X).Average(), verticesCenterPoints.Select(vertex => vertex.Y).Average());
            //    var simplexCenter = new Ellipse()
            //    {
            //        Width = VertexRadius
            //        , Height = VertexRadius
            //        , Fill = Brushes.Gray
            //    };
            //    Canvas.SetLeft(simplexCenter, simplexCenterPoint.X - SimplexCenterRadius);
            //    Canvas.SetTop(simplexCenter, simplexCenterPoint.Y - SimplexCenterRadius);
            //    simplexCenter.MouseEnter += (sender, eventArgs) =>
            //    {
            //        simplexCenter.Fill = Brushes.Black;
            //        for (var i = 0; i < _simplicesViews[simplexCenter].Length; i++)
            //        {
            //            _simplicesViews[simplexCenter][i].Stroke = Brushes.Black;
            //        }
            //    };
            //    simplexCenter.MouseLeave += (sender, eventArgs) =>
            //    {
            //        if (ReferenceEquals(simplexCenter, _fixedSimplexCenter))
            //        {
            //            return;
            //        }
            //        simplexCenter.Fill = Brushes.Gray;
            //        for (var i = 0; i < _simplicesViews[simplexCenter].Length; i++)
            //        {
            //            _simplicesViews[simplexCenter][i].Stroke = Brushes.Gray;
            //        }
            //    };
            //    simplexCenter.MouseDown += (sender, eventArgs) =>
            //    {
            //        simplexCenter.Tag = new Point(Canvas.GetLeft(simplexCenter), Canvas.GetTop(simplexCenter));
            //        _fixedSimplexCenter = simplexCenter;
            //    };
            //    simplexCenter.MouseUp += (sender, eventArgs) =>
            //    {
            //        _fixedSimplexCenter = null;
            //    };
            //    simplexCenter.MouseMove += (sender, eventArgs) =>
            //    {
            //        if (!ReferenceEquals(simplexCenter, _fixedSimplexCenter))
            //        {
            //            return;
            //        }
            //        Canvas.SetLeft(simplexCenter, Mouse.GetPosition(_canvas).X - SimplexCenterRadius);
            //        if (Canvas.GetLeft(simplexCenter) < 0)
            //        {
            //            Canvas.SetLeft(simplexCenter, 5);
            //        }
            //        if (Canvas.GetLeft(simplexCenter) + VertexRadius * 2 > _canvas.ActualWidth)
            //        {
            //            Canvas.SetLeft(simplexCenter, _canvas.ActualWidth - VertexRadius * 2);
            //        }
            //        Canvas.SetTop(simplexCenter, Mouse.GetPosition(_canvas).Y - SimplexCenterRadius);
            //        if (Canvas.GetTop(simplexCenter) < 0)
            //        {
            //            Canvas.SetTop(simplexCenter, 5);
            //        }
            //        if (Canvas.GetTop(simplexCenter) + VertexRadius * 2 > _canvas.ActualHeight)
            //        {
            //            Canvas.SetTop(simplexCenter, _canvas.ActualHeight - VertexRadius * 2);
            //        }
            //        for (var i = 0; i < _simplicesViews[simplexCenter].Length; i++)
            //        {
            //            _simplicesViews[simplexCenter][i].X1 = Canvas.GetLeft(simplexCenter) + SimplexCenterRadius;
            //            _simplicesViews[simplexCenter][i].Y1 = Canvas.GetTop(simplexCenter) + SimplexCenterRadius;
            //        }
            //    };
            //    var simplexEdges = new Line[simplex.Vertices.Length];
            //    for (var i = 0; i < simplexEdges.Length; i++)
            //    {
            //        simplexEdges[i] = new Line()
            //        {
            //            X1 = simplexCenterPoint.X
            //            , Y1 = simplexCenterPoint.Y
            //            , X2 = GetVertexCenterPoint(simplex.Vertices[i].Id).X
            //            , Y2 = GetVertexCenterPoint(simplex.Vertices[i].Id).Y
            //            , Stroke = Brushes.Gray
            //            , StrokeThickness = 1
            //        };
            //        simplexEdges[i].MouseMove += new MouseEventHandler(_onMouseMove);
            //    }
            //    _simplicesViews.Add(simplexCenter, simplexEdges);
            //}
            //foreach (var simplexView in _simplicesViews.Keys)
            //{
            //    foreach (var simplexPart in _simplicesViews[simplexView])
            //    {
            //        _canvas.Children.Add(simplexPart);
            //    }
            //    _canvas.Children.Add(simplexView);
            //}

            //// vertices
            //for (var i = 0; i < Hypergraph.Vertices.Length; i++)
            //{
            //    //var vertexModel = Hypergraph.Vertices[i];
            //    //var vertexCenterPoint = GetVertexCenterPoint(i);
            //    //var vertexView = new Ellipse()
            //    //{
            //    //    Width = VertexRadius * 2
            //    //    , Height = VertexRadius * 2
            //    //    , Fill = Brushes.Gray
            //    //    , Tag = new Timer(500)
            //    //};
            //    //Canvas.SetLeft(vertexView, vertexCenterPoint.X - VertexRadius);
            //    //Canvas.SetTop(vertexView, vertexCenterPoint.Y - VertexRadius);
            //    //var timer = vertexView.Tag as Timer;
            //    //timer.Elapsed += async (_sender, _eventArgs) =>
            //    //{
            //    //    timer.Stop();
            //    //    await Dispatcher.BeginInvoke(new Action(() =>
            //    //    {
            //    //        if (!_vertexSimplicesViews.ContainsKey(vertexView))
            //    //        {
            //    //            var vertexSimplicesView = new VertexSimplicesView();
            //    //            Canvas.SetLeft(vertexSimplicesView, vertexCenterPoint.X - vertexSimplicesView.Width / 2);
            //    //            Canvas.SetTop(vertexSimplicesView, vertexCenterPoint.Y - vertexSimplicesView.Height / 2);
            //    //            var simplicesViewTimer = (Timer)(vertexSimplicesView.Tag = new Timer(1000));
            //    //            simplicesViewTimer.Elapsed += async (__sender, __eventArgs) =>
            //    //            {
            //    //                await Dispatcher.BeginInvoke(new Action(() =>
            //    //                {
            //    //                    vertexSimplicesView.Visibility = Visibility.Collapsed;
            //    //                }));
            //    //            };
            //    //            vertexSimplicesView.MouseEnter += (__sender, __eventArgs) =>
            //    //            {
            //    //                simplicesViewTimer.Stop();
            //    //            };
            //    //            vertexSimplicesView.MouseLeave += (__sender, __eventArgs) =>
            //    //            {
            //    //                simplicesViewTimer.Start();
            //    //            };
            //    //            vertexSimplicesView.MouseMove += new MouseEventHandler(_onMouseMove);
            //    //            _vertexSimplicesViews.Add(vertexView, vertexSimplicesView);
            //    //            _canvas.Children.Add(vertexSimplicesView);
            //    //        }
            //    //        else
            //    //        {
            //    //            _vertexSimplicesViews[vertexView].Visibility = Visibility.Visible;
            //    //        }
            //    //    }));
            //    //};
            //    //vertexView.MouseEnter += (sender, eventArgs) =>
            //    //{
            //    //    if (_fixedSimplexCenter != null)
            //    //    {
            //    //        return;
            //    //    }
            //    //    vertexView.Fill = Brushes.Black;
            //    //    timer.Start();
            //    //};
            //    //vertexView.MouseLeave += (sender, eventArgs) =>
            //    //{
            //    //    timer.Stop();
            //    //    vertexView.Fill = Brushes.Gray;
            //    //};
            //    //vertexView.MouseUp += (sender, eventArgs) =>
            //    //{
            //    //    if (_fixedSimplexCenter != null)
            //    //    {
            //    //        var simplexCenterPosition = new Point(Canvas.GetLeft(_fixedSimplexCenter) + SimplexCenterRadius, Canvas.GetTop(_fixedSimplexCenter) + SimplexCenterRadius);
            //    //        foreach (var vertex in _vertexSimplicesViews.Keys)
            //    //        {
            //    //            var vertexPosition = new Point(Canvas.GetLeft(vertex) + VertexRadius, Canvas.GetTop(vertex) + VertexRadius);
            //    //            if ((simplexCenterPosition - vertexPosition).Length <= VertexRadius + SimplexCenterRadius)
            //    //            {
            //    //                Canvas.SetLeft(_fixedSimplexCenter, ((Point)_fixedSimplexCenter.Tag).X);
            //    //                Canvas.SetTop(_fixedSimplexCenter, ((Point)_fixedSimplexCenter.Tag).Y);
            //    //                _fixedSimplexCenter.Tag = null;
            //    //                break;
            //    //            }
            //    //        }
            //    //    }
            //    //    _fixedSimplexCenter = null;
            //    //};
                
            //    // TODO:
            //    //vertexView.MouseMove += new MouseEventHandler(_onMouseMove);
            //    //_canvas.Children.Add(vertexView);
            //}

            //Point GetVertexCenterPoint(int vertexIndex)
            //{
            //    var radius = (int)(0.75 * ActualHeight / 2);
            //    var centerPoint = new System.Drawing.Point((int)ActualWidth / 2, (int)ActualHeight / 2);
            //    var vertexPoint = new System.Drawing.Point[] { new System.Drawing.Point(centerPoint.X + radius, centerPoint.Y) };
            //    var angle = 360f / Hypergraph.Vertices.Length * vertexIndex;
            //    using (var rotateMatrix = new System.Drawing.Drawing2D.Matrix())
            //    {
            //        rotateMatrix.RotateAt(angle, centerPoint);
            //        rotateMatrix.TransformPoints(vertexPoint);
            //    }
            //    return new Point(vertexPoint[0].X, vertexPoint[0].Y);
            //}
        }

    }

}