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

        private VertexModel _model;
        private Ellipse _vertexView;
        private VertexSimplicesView _vertexSimplicesView;
        private Timer _vertexTimer = new Timer(500);
        private Timer _simplicesTimer = new Timer(1000);

        public VertexViewModel(HypergraphViewModel hypergraphViewModel, VertexModel model, Action<object, MouseEventArgs> onMouseMove, Point centerPoint)
        {
            HypergraphViewModel = hypergraphViewModel;
            Model = model;
            _vertexView = new Ellipse()
            {
                Width = VertexRadius * 2
                , Height = VertexRadius * 2
                , Fill = Brushes.Gray
            };
            Canvas.SetLeft(VertexView, centerPoint.X - VertexRadius);
            Canvas.SetTop(VertexView, centerPoint.Y - VertexRadius);
            Canvas.SetZIndex(VertexView, 2);
            _vertexView.MouseEnter += (sender, eventArgs) =>
            {
                if (HypergraphViewModel.FixedSimplex != null)
                {
                    return;
                }
                _vertexView.Fill = Brushes.Black;
                _vertexTimer.Start();
            };
            
            _vertexView.MouseLeave += (sender, eventArgs) =>
            {
                _vertexTimer.Stop();
                _vertexView.Fill = Brushes.Gray;
            };

            _vertexView.MouseUp += (sender, eventArgs) =>
            {
                if (HypergraphViewModel.FixedSimplex != null)
                {
                    var simplexCenterPosition = new Point(Canvas.GetLeft(HypergraphViewModel.FixedSimplex.Center) + SimplexViewModel.SimplexCenterRadius
                                                          , Canvas.GetTop(HypergraphViewModel.FixedSimplex.Center) + SimplexViewModel.SimplexCenterRadius);
                    foreach (var vertexViewModel in HypergraphViewModel.Vertices)
                    {
                        var vertexPosition = new Point(Canvas.GetLeft(vertexViewModel.VertexView) + VertexRadius, Canvas.GetTop(vertexViewModel.VertexView) + VertexRadius);
                        if ((simplexCenterPosition - vertexPosition).Length <= VertexRadius + SimplexViewModel.SimplexCenterRadius)
                        {
                            Canvas.SetLeft(HypergraphViewModel.FixedSimplex.Center, HypergraphViewModel.FixedSimplex.PositionBeforeMoving.Value.X);
                            Canvas.SetTop(HypergraphViewModel.FixedSimplex.Center, HypergraphViewModel.FixedSimplex.PositionBeforeMoving.Value.Y);
                            for (var i = 0; i < HypergraphViewModel.FixedSimplex.Edges.Length; i++)
                            {
                                HypergraphViewModel.FixedSimplex.Edges[i].X1 = HypergraphViewModel.FixedSimplex.PositionBeforeMoving.Value.X + SimplexViewModel.SimplexCenterRadius;
                                HypergraphViewModel.FixedSimplex.Edges[i].Y1 = HypergraphViewModel.FixedSimplex.PositionBeforeMoving.Value.Y + SimplexViewModel.SimplexCenterRadius;
                            }
                            HypergraphViewModel.FixedSimplex.PositionBeforeMoving = null;
                            break;
                        }
                    }
                }
                HypergraphViewModel.FixedSimplex = null;
            };

            _vertexView.MouseMove += new MouseEventHandler(onMouseMove);

            _vertexTimer.Elapsed += async (sender, eventArgs) =>
            {
                await _vertexSimplicesView.Dispatcher.BeginInvoke(new Action(() =>
                {
                    _vertexSimplicesView.Visibility = Visibility.Visible;
                }));
            };

            _vertexSimplicesView = new VertexSimplicesView()
            {
                Visibility = Visibility.Collapsed
            };
            Canvas.SetLeft(_vertexSimplicesView, centerPoint.X - _vertexSimplicesView.Width / 2);
            Canvas.SetTop(_vertexSimplicesView, centerPoint.Y - _vertexSimplicesView.Height / 2);
            Canvas.SetZIndex(_vertexSimplicesView, 3);
            _vertexSimplicesView.MouseEnter += (sender, eventArgs) =>
            {
                _simplicesTimer.Stop();
            };
            _vertexSimplicesView.MouseLeave += (sender, eventArgs) =>
            {
                _simplicesTimer.Start();
            };
            _vertexSimplicesView.MouseMove += new MouseEventHandler(onMouseMove);
            _simplicesTimer.Elapsed += async (sender, eventArgs) =>
            {
                await _vertexSimplicesView.Dispatcher.BeginInvoke(new Action(() =>
                {
                    _vertexSimplicesView.Visibility = Visibility.Collapsed;
                }));
            };
        }

        public VertexModel Model
        {
            get =>
                _model;

            private set =>
                _model = value;
        }

        public Ellipse VertexView =>
            _vertexView;

        public VertexSimplicesView VertexSimplicesView =>
            _vertexSimplicesView;

        public void Dispose()
        {
            _vertexTimer.Dispose();
            _simplicesTimer.Dispose();
        }

    }

}