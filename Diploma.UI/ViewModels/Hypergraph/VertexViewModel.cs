using System;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

using Diploma.Hypergraph;
using Diploma.UI.Views.Controls;

namespace Diploma.UI.ViewModels.Hypergraph
{

    public sealed class VertexViewModel : HypergraphComponent, IDisposable
    {

        public static readonly double VertexRadius = 15d;

        private readonly Action<object, MouseButtonEventArgs> _onMouseUp;

        public VertexViewModel(HypergraphViewModel hypergraphViewModel, VertexModel model, Action<object, MouseEventArgs> onMouseMove, Point centerPoint)
        {
            HypergraphViewModel = hypergraphViewModel;
            Model = model;
            _onMouseUp = (sender, eventArgs) =>
            {
                if (HypergraphViewModel.CapturedSimplex != null)
                {
                    var simplexCenterPosition = new Point(Canvas.GetLeft(HypergraphViewModel.CapturedSimplex.Center) + SimplexViewModel.SimplexCenterRadius
                                                          , Canvas.GetTop(HypergraphViewModel.CapturedSimplex.Center) + SimplexViewModel.SimplexCenterRadius);
                    foreach (var vertexViewModel in HypergraphViewModel.Vertices)
                    {
                        var vertexPosition = new Point(Canvas.GetLeft(vertexViewModel.VertexView) + VertexRadius, Canvas.GetTop(vertexViewModel.VertexView) + VertexRadius);
                        if ((simplexCenterPosition - vertexPosition).Length <= VertexRadius + SimplexViewModel.SimplexCenterRadius)
                        {
                            Canvas.SetLeft(HypergraphViewModel.CapturedSimplex.Center, HypergraphViewModel.CapturedSimplex.PositionBeforeMoving.Value.X);
                            Canvas.SetTop(HypergraphViewModel.CapturedSimplex.Center, HypergraphViewModel.CapturedSimplex.PositionBeforeMoving.Value.Y);
                            for (var i = 0; i < HypergraphViewModel.CapturedSimplex.Edges.Length; i++)
                            {
                                HypergraphViewModel.CapturedSimplex.Edges[i].X1 = HypergraphViewModel.CapturedSimplex.PositionBeforeMoving.Value.X + SimplexViewModel.SimplexCenterRadius;
                                HypergraphViewModel.CapturedSimplex.Edges[i].Y1 = HypergraphViewModel.CapturedSimplex.PositionBeforeMoving.Value.Y + SimplexViewModel.SimplexCenterRadius;
                            }
                            HypergraphViewModel.CapturedSimplex.PositionBeforeMoving = null;
                            break;
                        }
                    }
                }
                HypergraphViewModel.CapturedSimplex = null;
                if (!ReferenceEquals(sender, VertexView))
                {
                    return;
                }
                VertexView.Fill = Brushes.Black;
                AfterVertexEnter.Start();
            };
            VertexView = new Ellipse()
            {
                Width = VertexRadius * 2
                , Height = VertexRadius * 2
                , Fill = Brushes.Gray
            };
            Canvas.SetLeft(VertexView, centerPoint.X - VertexRadius);
            Canvas.SetTop(VertexView, centerPoint.Y - VertexRadius);
            Canvas.SetZIndex(VertexView, 2);
            VertexView.MouseEnter += (sender, eventArgs) =>
            {
                if (HypergraphViewModel.CapturedSimplex != null)
                {
                    return;
                }
                VertexView.Fill = Brushes.Black;
                AfterVertexEnter.Start();
            };
            VertexView.MouseLeave += (sender, eventArgs) =>
            {
                AfterVertexEnter.Stop();
                VertexView.Fill = Brushes.Gray;
            };
            VertexView.MouseUp += new MouseButtonEventHandler(_onMouseUp);
            VertexView.MouseMove += new MouseEventHandler(onMouseMove);
            AfterVertexEnter.Elapsed += async (sender, eventArgs) =>
            {
                await VertexSimplicesView.Dispatcher.BeginInvoke(new Action(() =>
                {
                    VertexSimplicesView.Visibility = Visibility.Visible;
                }));
            };
            VertexSimplicesView = new VertexSimplicesView()
            {
                Visibility = Visibility.Collapsed
            };
            Canvas.SetLeft(VertexSimplicesView, centerPoint.X - VertexSimplicesView.Width / 2);
            Canvas.SetTop(VertexSimplicesView, centerPoint.Y - VertexSimplicesView.Height / 2);
            Canvas.SetZIndex(VertexSimplicesView, 3);
            VertexSimplicesView.MouseEnter += (sender, eventArgs) =>
            {
                AfterVertexLeave.Stop();
            };
            VertexSimplicesView.MouseLeave += (sender, eventArgs) =>
            {
                AfterVertexLeave.Start();
            };
            VertexSimplicesView.MouseMove += new MouseEventHandler(onMouseMove);
            VertexSimplicesView.MouseUp += new MouseButtonEventHandler(_onMouseUp);
            AfterVertexLeave.Elapsed += async (sender, eventArgs) =>
            {
                await VertexSimplicesView.Dispatcher.BeginInvoke(new Action(() =>
                {
                    VertexSimplicesView.Visibility = Visibility.Collapsed;
                }));
            };
        }

        public VertexModel Model
        {
            get;

            private set;
        }

        public Ellipse VertexView
        {
            get;
        }

        public VertexSimplicesView VertexSimplicesView
        {
            get;
        }

        private Timer AfterVertexEnter
        {
            get;
        } = new Timer(500);

        private Timer AfterVertexLeave
        {
            get;
        } = new Timer(1000);

        public void Dispose()
        {
            AfterVertexEnter.Dispose();
            AfterVertexLeave.Dispose();
        }

    }

}