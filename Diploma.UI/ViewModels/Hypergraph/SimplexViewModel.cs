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

        private Point? _positionBeforeMoving;

        public SimplexViewModel(HypergraphViewModel hypergraphViewModel, SimplexModel model, Action<object, MouseEventArgs> onMouseMove, Point centerPoint)
        {
            HypergraphViewModel = hypergraphViewModel;
            Model = model;
            Center = new Ellipse()
            {
                Width = SimplexCenterRadius * 2
                , Height = SimplexCenterRadius * 2
                , Fill = Brushes.Gray
            };
            Canvas.SetLeft(Center, centerPoint.X - SimplexCenterRadius);
            Canvas.SetTop(Center, centerPoint.Y - SimplexCenterRadius);
            Canvas.SetZIndex(Center, 1);
            Center.MouseEnter += (sender, eventArgs) =>
            {
                if (HypergraphViewModel.CapturedSimplex != null)
                {
                    return;
                }
                Center.Fill = Brushes.Black;
                for (var i = 0; i < Edges.Length; i++)
                {
                    Edges[i].Stroke = Brushes.Black;
                }
            };
            Center.MouseLeave += (sender, eventArgs) =>
            {
                if (ReferenceEquals(Center, HypergraphViewModel.CapturedSimplex))
                {
                    return;
                }
                Center.Fill = Brushes.Gray;
                for (var i = 0; i < Edges.Length; i++)
                {
                    Edges[i].Stroke = Brushes.Gray;
                }
            };
            Center.MouseDown += (sender, eventArgs) =>
            {
                PositionBeforeMoving = new Point(Canvas.GetLeft(Center), Canvas.GetTop(Center));
                HypergraphViewModel.CapturedSimplex = this;
                Center.Fill = Brushes.Black;
                for (var i = 0; i < Edges.Length; i++)
                {
                    Edges[i].Stroke = Brushes.Black;
                }
            };
            Center.MouseUp += (sender, eventArgs) =>
            {
                if (HypergraphViewModel.CapturedSimplex != null)
                {
                    var simplexCenterPosition = new Point(Canvas.GetLeft(HypergraphViewModel.CapturedSimplex.Center) + SimplexCenterRadius
                                                          , Canvas.GetTop(HypergraphViewModel.CapturedSimplex.Center) + SimplexCenterRadius);
                    foreach (var vertexViewModel in HypergraphViewModel.Vertices)
                    {
                        var vertexPosition = new Point(Canvas.GetLeft(vertexViewModel.VertexView) + VertexViewModel.VertexRadius
                                                       , Canvas.GetTop(vertexViewModel.VertexView) + VertexViewModel.VertexRadius);
                        if ((simplexCenterPosition - vertexPosition).Length <= VertexViewModel.VertexRadius + SimplexCenterRadius)
                        {
                            Canvas.SetLeft(HypergraphViewModel.CapturedSimplex.Center, HypergraphViewModel.CapturedSimplex.PositionBeforeMoving.Value.X);
                            Canvas.SetTop(HypergraphViewModel.CapturedSimplex.Center, HypergraphViewModel.CapturedSimplex.PositionBeforeMoving.Value.Y);
                            for (var i = 0; i < HypergraphViewModel.CapturedSimplex.Edges.Length; i++)
                            {
                                HypergraphViewModel.CapturedSimplex.Edges[i].X1 = HypergraphViewModel.CapturedSimplex.PositionBeforeMoving.Value.X + SimplexCenterRadius;
                                HypergraphViewModel.CapturedSimplex.Edges[i].Y1 = HypergraphViewModel.CapturedSimplex.PositionBeforeMoving.Value.Y + SimplexCenterRadius;
                            }
                            HypergraphViewModel.CapturedSimplex.PositionBeforeMoving = null;
                            break;
                        }
                    }
                }
                if (!ReferenceEquals(HypergraphViewModel.CapturedSimplex, this) || Center.IsMouseOver)
                {
                    Center.Fill = Brushes.Black;
                    for (var i = 0; i < Edges.Length; i++)
                    {
                        Edges[i].Stroke = Brushes.Black;
                    }
                }
                HypergraphViewModel.CapturedSimplex = null;
            };
            Center.MouseMove += (sender, eventArgs) =>
            {
                if (!ReferenceEquals(this, HypergraphViewModel.CapturedSimplex))
                {
                    return;
                }
                Canvas.SetLeft(Center, Mouse.GetPosition(HypergraphViewModel.HypergraphView._canvas).X - SimplexCenterRadius);
                if (Canvas.GetLeft(Center) < 5)
                {
                    Canvas.SetLeft(Center, 5);
                }
                if (Canvas.GetLeft(Center) + SimplexCenterRadius * 2 > HypergraphViewModel.HypergraphView._canvas.ActualWidth - 5)
                {
                    Canvas.SetLeft(Center, HypergraphViewModel.HypergraphView._canvas.ActualWidth - SimplexCenterRadius * 2);
                }
                Canvas.SetTop(Center, Mouse.GetPosition(HypergraphViewModel.HypergraphView._canvas).Y - SimplexCenterRadius - 5);
                if (Canvas.GetTop(Center) < 5)
                {
                    Canvas.SetTop(Center, 5);
                }
                if (Canvas.GetTop(Center) + SimplexCenterRadius * 2 > HypergraphViewModel.HypergraphView._canvas.ActualHeight - 5)
                {
                    Canvas.SetTop(Center, HypergraphViewModel.HypergraphView._canvas.ActualHeight - SimplexCenterRadius * 2 - 5);
                }
                for (var i = 0; i < Edges.Length; i++)
                {
                    Edges[i].X1 = Canvas.GetLeft(Center) + SimplexCenterRadius;
                    Edges[i].Y1 = Canvas.GetTop(Center) + SimplexCenterRadius;
                }
            };

            Edges = new Line[Model.Vertices.Length];
            for (var i = 0; i < Edges.Length; i++)
            {
                var destinationPoint = CoordinatesCalculator.GetVertexCenterPoint(new Size(HypergraphViewModel.HypergraphView.ActualWidth, HypergraphViewModel.HypergraphView.ActualHeight)
                    , HypergraphViewModel.Vertices.Length, model.Vertices[i]);
                Edges[i] = new Line()
                {
                    X1 = centerPoint.X
                    , Y1 = centerPoint.Y
                    , X2 = destinationPoint.X
                    , Y2 = destinationPoint.Y
                    , Stroke = Brushes.Gray
                    , StrokeThickness = 1
                };
                Canvas.SetZIndex(Edges[i], 0);
                Edges[i].MouseMove += new MouseEventHandler(onMouseMove);
            }
        }

        public SimplexModel Model
        {
            get;

            private set;
        }

        public Ellipse Center
        {
            get;
        }

        public Line[] Edges
        {
            get;
        }

        public Point? PositionBeforeMoving
        {
            get =>
                _positionBeforeMoving;

            set =>
                _positionBeforeMoving = value;
        }

    }

}