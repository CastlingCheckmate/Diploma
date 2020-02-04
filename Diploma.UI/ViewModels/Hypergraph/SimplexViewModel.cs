using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

using Diploma.Hypergraph;
using Diploma.UI.Auxiliary.Hypergraph;

namespace Diploma.UI.ViewModels.Hypergraph
{

    public sealed class SimplexViewModel : HypergraphComponent
    {

        public static readonly double SimplexCenterRadius = 7d;

        private SimplexModel _model;
        private Ellipse _center;
        private Line[] _edges;
        private Point? _positionBeforeMoving;

        public SimplexViewModel(HypergraphViewModel hypergraphViewModel, SimplexModel model, Action<object, MouseEventArgs> onMouseMove, Point centerPoint)
        {
            HypergraphViewModel = hypergraphViewModel;
            Model = model;
            _center = new Ellipse()
            {
                Width = SimplexCenterRadius * 2
                , Height = SimplexCenterRadius * 2
                , Fill = Brushes.Gray
            };
            Canvas.SetLeft(_center, centerPoint.X - SimplexCenterRadius);
            Canvas.SetTop(_center, centerPoint.Y - SimplexCenterRadius);
            Canvas.SetZIndex(_center, 1);
            _center.MouseEnter += (sender, eventArgs) =>
            {
                if (HypergraphViewModel.FixedSimplex != null)
                {
                    return;
                }
                _center.Fill = Brushes.Black;
                for (var i = 0; i < _edges.Length; i++)
                {
                    _edges[i].Stroke = Brushes.Black;
                }
            };
            _center.MouseLeave += (sender, eventArgs) =>
            {
                if (ReferenceEquals(_center, HypergraphViewModel.FixedSimplex))
                {
                    return;
                }
                _center.Fill = Brushes.Gray;
                for (var i = 0; i < _edges.Length; i++)
                {
                    _edges[i].Stroke = Brushes.Gray;
                }
            };
            _center.MouseDown += (sender, eventArgs) =>
            {
                PositionBeforeMoving = new Point(Canvas.GetLeft(_center), Canvas.GetTop(_center));
                HypergraphViewModel.FixedSimplex = this;
            };
            _center.MouseUp += (sender, eventArgs) =>
            {
                var fixedSimplex = HypergraphViewModel.FixedSimplex;
                HypergraphViewModel.FixedSimplex = null;
                if (!ReferenceEquals(fixedSimplex, this))
                {
                    _center.Fill = Brushes.Black;
                    for (var i = 0; i < _edges.Length; i++)
                    {
                        _edges[i].Stroke = Brushes.Black;
                    }
                }
            };
            _center.MouseMove += (sender, eventArgs) =>
            {
                if (!ReferenceEquals(this, HypergraphViewModel.FixedSimplex))
                {
                    return;
                }
                Canvas.SetLeft(_center, Mouse.GetPosition(HypergraphViewModel.HypergraphView._canvas).X - SimplexCenterRadius);
                if (Canvas.GetLeft(_center) < 0)
                {
                    Canvas.SetLeft(_center, 5);
                }
                if (Canvas.GetLeft(_center) + VertexViewModel.VertexRadius * 2 > HypergraphViewModel.HypergraphView._canvas.ActualWidth)
                {
                    Canvas.SetLeft(_center, HypergraphViewModel.HypergraphView._canvas.ActualWidth - VertexViewModel.VertexRadius * 2);
                }
                Canvas.SetTop(_center, Mouse.GetPosition(HypergraphViewModel.HypergraphView._canvas).Y - SimplexCenterRadius);
                if (Canvas.GetTop(_center) < 0)
                {
                    Canvas.SetTop(_center, 5);
                }
                if (Canvas.GetTop(_center) + VertexViewModel.VertexRadius * 2 > HypergraphViewModel.HypergraphView._canvas.ActualHeight)
                {
                    Canvas.SetTop(_center, HypergraphViewModel.HypergraphView._canvas.ActualHeight - VertexViewModel.VertexRadius * 2);
                }
                for (var i = 0; i < _edges.Length; i++)
                {
                    _edges[i].X1 = Canvas.GetLeft(_center) + SimplexCenterRadius;
                    _edges[i].Y1 = Canvas.GetTop(_center) + SimplexCenterRadius;
                }
            };

            _edges = new Line[_model.Vertices.Length];
            for (var i = 0; i < _edges.Length; i++)
            {
                var destinationPoint = CoordinatesCalculator.GetVertexCenterPoint(new Size(HypergraphViewModel.HypergraphView.ActualWidth, HypergraphViewModel.HypergraphView.ActualHeight)
                    , HypergraphViewModel.Vertices.Length, model.Vertices[i]);
                _edges[i] = new Line()
                {
                    X1 = centerPoint.X
                    , Y1 = centerPoint.Y
                    , X2 = destinationPoint.X
                    , Y2 = destinationPoint.Y
                    , Stroke = Brushes.Gray
                    , StrokeThickness = 1
                };
                Canvas.SetZIndex(_edges[i], 0);
                _edges[i].MouseMove += new MouseEventHandler(onMouseMove);
            }
        }

        public SimplexModel Model
        {
            get =>
                _model;

            private set =>
                _model = value;
        }

        public Ellipse Center =>
            _center;

        public Line[] Edges =>
            _edges;

        public Point? PositionBeforeMoving
        {
            get =>
                _positionBeforeMoving;

            set =>
                _positionBeforeMoving = value;
        }

    }

}