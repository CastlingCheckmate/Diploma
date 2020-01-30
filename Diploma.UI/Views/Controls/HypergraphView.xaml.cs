using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Diploma.UI.Views.Controls
{

    public partial class HypergraphView : UserControl
    {

        private readonly int VertexDiameter = 40;

        private Dictionary<Ellipse, VertexSimplicesView> _vertexSimplicesViews;
        private Dictionary<Ellipse, Line[]> _simplicesViews;

        public HypergraphView()
        {
            InitializeComponent();
            _vertexSimplicesViews = new Dictionary<Ellipse, VertexSimplicesView>();
            _simplicesViews = new Dictionary<Ellipse, Line[]>();
            Loaded += (sender, eventArgs) =>
            {
                Window.GetWindow(this).Closing += (_sender, _eventArgs) =>
                {
                    foreach (var vertex in _canvas.Children.OfType<Ellipse>().ToArray())
                    {
                        if (!(vertex.Tag is null) && !(vertex.Tag is bool))
                        {
                            (vertex.Tag as Timer).Dispose();
                        }
                        if (_vertexSimplicesViews.ContainsKey(vertex))
                        {
                            (_vertexSimplicesViews[vertex].Tag as Timer).Dispose();
                        }
                    }
                };
            };
        }

        public Hypergraph.Hypergraph Hypergraph
        {
            get =>
                (Hypergraph.Hypergraph)GetValue(HypergraphProperty);

            set =>
                SetValue(HypergraphProperty, value);
        }
        public static DependencyProperty HypergraphProperty = DependencyProperty.Register(
            nameof(Hypergraph), typeof(Hypergraph.Hypergraph), typeof(HypergraphView), new PropertyMetadata((d, e) => ((HypergraphView)d).Redraw()));

        public void Redraw()
        {
            foreach (var vertexOrSimplexCenter in _canvas.Children.OfType<Ellipse>().ToArray())
            {
                if (!(vertexOrSimplexCenter.Tag is null) && !(vertexOrSimplexCenter.Tag is bool))
                {
                    (vertexOrSimplexCenter.Tag as Timer).Dispose();
                }
                _canvas.Children.Remove(vertexOrSimplexCenter);
                if (_vertexSimplicesViews.Keys.Contains(vertexOrSimplexCenter))
                {
                    (_vertexSimplicesViews[vertexOrSimplexCenter].Tag as Timer).Dispose();
                    _vertexSimplicesViews.Remove(vertexOrSimplexCenter);
                }
                else if (_simplicesViews.Keys.Contains(vertexOrSimplexCenter))
                {
                    _simplicesViews.Remove(vertexOrSimplexCenter);
                }
            }
            foreach (var simplexPart in _canvas.Children.OfType<Line>().ToArray())
            {
                _canvas.Children.Remove(simplexPart);
            }
            if (Hypergraph is null)
            {
                return;
            }

            // simplices
            foreach (var simplex in Hypergraph.Simplices)
            {
                var verticesCenterPoints = simplex.Vertices.Select(vertex => GetVertexCenterPoint(vertex.Id));
                var simplexCenterPoint = new Point(verticesCenterPoints.Select(vertex => vertex.X).Average(), verticesCenterPoints.Select(vertex => vertex.Y).Average());
                var simplexCenter = new Ellipse()
                {
                    Width = VertexDiameter / 2
                    , Height = VertexDiameter / 2
                    , Fill = Brushes.Gray
                    , Margin = new Thickness(simplexCenterPoint.X - VertexDiameter / 4, simplexCenterPoint.Y - VertexDiameter / 4, 0, 0)
                    , Tag = false
                };
                simplexCenter.MouseEnter += (sender, eventArgs) =>
                {
                    simplexCenter.Fill = Brushes.Black;
                    for (var i = 0; i < _simplicesViews[simplexCenter].Length; i++)
                    {
                        _simplicesViews[simplexCenter][i].Stroke = Brushes.Black;
                    }
                };
                simplexCenter.MouseLeave += (sender, eventArgs) =>
                {
                    simplexCenter.Fill = Brushes.Gray;
                    for (var i = 0; i < _simplicesViews[simplexCenter].Length; i++)
                    {
                        _simplicesViews[simplexCenter][i].Stroke = Brushes.Gray;
                    }
                };
                simplexCenter.MouseDown += (sender, eventArgs) =>
                {
                    simplexCenter.Tag = true;
                };
                simplexCenter.MouseUp += (sender, eventArgs) =>
                {
                    simplexCenter.Tag = false;
                };
                simplexCenter.MouseMove += (sender, eventArgs) =>
                {
                    if (!(bool)simplexCenter.Tag)
                    {
                        return;
                    }
                    simplexCenter.Margin = new Thickness(eventArgs.GetPosition(_canvas).X - VertexDiameter / 4, eventArgs.GetPosition(_canvas).Y - VertexDiameter / 4, 0, 0);
                    for (var i = 0; i < _simplicesViews[simplexCenter].Length; i++)
                    {
                        _simplicesViews[simplexCenter][i].X1 = eventArgs.GetPosition(_canvas).X;
                        _simplicesViews[simplexCenter][i].Y1 = eventArgs.GetPosition(_canvas).Y;
                    }
                };
                var simplexEdges = new Line[simplex.Vertices.Length];
                for (var i = 0; i < simplexEdges.Length; i++)
                {
                    simplexEdges[i] = new Line()
                    {
                        X1 = simplexCenterPoint.X
                        , Y1 = simplexCenterPoint.Y
                        , X2 = GetVertexCenterPoint(simplex.Vertices[i].Id).X
                        , Y2 = GetVertexCenterPoint(simplex.Vertices[i].Id).Y
                        , Stroke = Brushes.Gray
                        , StrokeThickness = 1
                    };
                }
                _simplicesViews.Add(simplexCenter, simplexEdges);
            }
            foreach (var simplexView in _simplicesViews.Keys)
            {
                foreach (var simplexPart in _simplicesViews[simplexView])
                {
                    _canvas.Children.Add(simplexPart);
                }
                _canvas.Children.Add(simplexView);
            }

            // vertices
            for (var i = 0; i < Hypergraph.Vertices.Length; i++)
            {
                var vertexModel = Hypergraph.Vertices[i];
                var vertexCenterPoint = GetVertexCenterPoint(i);
                var vertexView = new Ellipse()
                {
                    Width = VertexDiameter
                    , Height = VertexDiameter
                    , Fill = Brushes.Black
                    , Margin = new Thickness(vertexCenterPoint.X - VertexDiameter / 2, vertexCenterPoint.Y - VertexDiameter / 2, 0, 0)
                    , Tag = new Timer(500)
                };
                var timer = vertexView.Tag as Timer;
                timer.Elapsed += async (_sender, _eventArgs) =>
                {
                    timer.Stop();
                    await Dispatcher.BeginInvoke(new Action(() =>
                    {
                        if (!_vertexSimplicesViews.ContainsKey(vertexView))
                        {
                            var vertexSimplicesView = new VertexSimplicesView()
                            {
                                Margin = new Thickness(vertexCenterPoint.X - 100, vertexCenterPoint.Y - 100, 0, 0)
                            };
                            var simplicesViewTimer = (Timer)(vertexSimplicesView.Tag = new Timer(1000));
                            simplicesViewTimer.Elapsed += async (__sender, __eventArgs) =>
                            {
                                await Dispatcher.BeginInvoke(new Action(() =>
                                {
                                    vertexSimplicesView.Visibility = Visibility.Collapsed;
                                }));
                            };
                            vertexSimplicesView.MouseEnter += (__sender, __eventArgs) =>
                            {
                                simplicesViewTimer.Stop();
                            };
                            vertexSimplicesView.MouseLeave += (__sender, __eventArgs) =>
                            {
                                simplicesViewTimer.Start();
                            };
                            _vertexSimplicesViews.Add(vertexView, vertexSimplicesView);
                            _canvas.Children.Add(vertexSimplicesView);
                        }
                        else
                        {
                            _vertexSimplicesViews[vertexView].Visibility = Visibility.Visible;
                        }
                    }));
                };
                vertexView.MouseEnter += (sender, eventArgs) =>
                {
                    vertexView.Fill = Brushes.Gray;
                    timer.Start();
                };
                vertexView.MouseLeave += (sender, eventArgs) =>
                {
                    timer.Stop();
                    vertexView.Fill = Brushes.Black;
                };
                _canvas.Children.Add(vertexView);
            }

            Point GetVertexCenterPoint(int vertexIndex)
            {
                var radius = (int)(0.75 * ActualHeight / 2);
                var centerPoint = new System.Drawing.Point((int)ActualWidth / 2, (int)ActualHeight / 2);
                var vertexPoint = new System.Drawing.Point[] { new System.Drawing.Point(centerPoint.X + radius, centerPoint.Y) };
                var angle = 360f / Hypergraph.Vertices.Length * vertexIndex;
                using (var rotateMatrix = new System.Drawing.Drawing2D.Matrix())
                {
                    rotateMatrix.RotateAt(angle, centerPoint);
                    rotateMatrix.TransformPoints(vertexPoint);
                }
                return new Point(vertexPoint[0].X, vertexPoint[0].Y);
            }
        }

    }

}