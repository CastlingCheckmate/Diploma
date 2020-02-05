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
                if (HypergraphViewModel.FixedSimplex != null)
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
                if (ReferenceEquals(Center, HypergraphViewModel.FixedSimplex))
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
                HypergraphViewModel.FixedSimplex = this;
                Center.Fill = Brushes.Black;
                for (var i = 0; i < Edges.Length; i++)
                {
                    Edges[i].Stroke = Brushes.Black;
                }
            };
            Center.MouseUp += (sender, eventArgs) =>
            {
                var fixedSimplex = HypergraphViewModel.FixedSimplex;
                HypergraphViewModel.FixedSimplex = null;
                if (!ReferenceEquals(fixedSimplex, this) || Center.IsMouseOver)
                {
                    Center.Fill = Brushes.Black;
                    for (var i = 0; i < Edges.Length; i++)
                    {
                        Edges[i].Stroke = Brushes.Black;
                    }
                }
            };
            Center.MouseMove += (sender, eventArgs) =>
            {
                if (!ReferenceEquals(this, HypergraphViewModel.FixedSimplex))
                {
                    return;
                }
                Canvas.SetLeft(Center, Mouse.GetPosition(HypergraphViewModel.HypergraphView._canvas).X - SimplexCenterRadius);
                if (Canvas.GetLeft(Center) < 0)
                {
                    Canvas.SetLeft(Center, 5);
                }
                if (Canvas.GetLeft(Center) + VertexViewModel.VertexRadius * 2 > HypergraphViewModel.HypergraphView._canvas.ActualWidth)
                {
                    Canvas.SetLeft(Center, HypergraphViewModel.HypergraphView._canvas.ActualWidth - VertexViewModel.VertexRadius * 2);
                }
                Canvas.SetTop(Center, Mouse.GetPosition(HypergraphViewModel.HypergraphView._canvas).Y - SimplexCenterRadius);
                if (Canvas.GetTop(Center) < 0)
                {
                    Canvas.SetTop(Center, 5);
                }
                if (Canvas.GetTop(Center) + VertexViewModel.VertexRadius * 2 > HypergraphViewModel.HypergraphView._canvas.ActualHeight)
                {
                    Canvas.SetTop(Center, HypergraphViewModel.HypergraphView._canvas.ActualHeight - VertexViewModel.VertexRadius * 2);
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