using System;
using System.Collections.Generic;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

using Diploma.Hypergraph;
using Diploma.UI.Auxiliary.Hypergraph;

namespace Diploma.UI.ViewModels.Hypergraph
{

    public sealed class VertexViewModel : HypergraphComponent, IDisposable
    {

        public const double VertexRadius = 15d;
        private const int VertexZIndex = 2;

        private readonly Action<object, MouseButtonEventArgs> _onMouseUp;
        private VertexStates _state;

        static VertexViewModel()
        {
            VertexColors = new Dictionary<VertexStates, Brush>()
            {
                { VertexStates.None, Brushes.Gray },
                { VertexStates.MouseOn, Brushes.Black},
                { VertexStates.SimplicesViewActive, Brushes.Transparent }
            };
        }

        public VertexViewModel(HypergraphViewModel hypergraphViewModel, VertexModel model)
        {
            HypergraphViewModel = hypergraphViewModel;
            var centerPoint = CoordinatesCalculator.GetVertexCenterPoint(new Size(HypergraphViewModel.HypergraphView.ActualWidth, HypergraphViewModel.HypergraphView.ActualHeight),
                HypergraphViewModel.Vertices.Length, model);
            Model = model;
            _onMouseUp = (sender, eventArgs) =>
            {
                if (HypergraphViewModel.CapturedSimplex != null)
                {
                    Canvas.SetLeft(HypergraphViewModel.CapturedSimplex.Center, HypergraphViewModel.CapturedSimplex.PositionBeforeMoving.Value.X);
                    Canvas.SetTop(HypergraphViewModel.CapturedSimplex.Center, HypergraphViewModel.CapturedSimplex.PositionBeforeMoving.Value.Y);
                    for (var i = 0; i < HypergraphViewModel.CapturedSimplex.Edges.Length; i++)
                    {
                        HypergraphViewModel.CapturedSimplex.Edges[i].X1 = HypergraphViewModel.CapturedSimplex.PositionBeforeMoving.Value.X + SimplexViewModel.SimplexCenterRadius;
                        HypergraphViewModel.CapturedSimplex.Edges[i].Y1 = HypergraphViewModel.CapturedSimplex.PositionBeforeMoving.Value.Y + SimplexViewModel.SimplexCenterRadius;
                    }
                    HypergraphViewModel.CapturedSimplex.PositionBeforeMoving = null;
                }
                HypergraphViewModel.CapturedSimplex = null;
                if (!ReferenceEquals(sender, VertexView))
                {
                    return;
                }
                State = VertexStates.MouseOn;
                AfterVertexEnter.Start();
            };
            VertexView = new Ellipse()
            {
                Width = VertexRadius * 2
                , Height = VertexRadius * 2
            };
            Canvas.SetLeft(VertexView, centerPoint.X - VertexRadius);
            Canvas.SetTop(VertexView, centerPoint.Y - VertexRadius);
            Canvas.SetZIndex(VertexView, VertexZIndex);
            VertexView.MouseEnter += (sender, eventArgs) =>
            {
                if (HypergraphViewModel.CapturedSimplex != null)
                {
                    return;
                }
                State = VertexStates.MouseOn;
                AfterVertexEnter.Start();
            };
            VertexView.MouseLeave += (sender, eventArgs) =>
            {
                AfterVertexEnter.Stop();
                State = VertexStates.None;
            };
            VertexView.MouseUp += new MouseButtonEventHandler(_onMouseUp);

            VertexSimplicesViewModel = new VertexSimplicesViewModel(HypergraphViewModel, this, centerPoint, _onMouseUp);
            AfterVertexEnter.Elapsed += async (sender, eventArgs) =>
            {
                HypergraphViewModel.CapturedVertexSimplices = VertexSimplicesViewModel;
                State = VertexStates.SimplicesViewActive;
                await VertexSimplicesViewModel.VertexSimplicesView.Dispatcher.BeginInvoke(new Action(() =>
                {
                    VertexSimplicesViewModel.VertexSimplicesView.Visibility = Visibility.Visible;
                }));
            };

            State = VertexStates.None;
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

        public VertexSimplicesViewModel VertexSimplicesViewModel
        {
            get;
        }

        private Timer AfterVertexEnter
        {
            get;
        } = new Timer(500);

        public void Dispose()
        {
            AfterVertexEnter.Dispose();
            VertexSimplicesViewModel.Dispose();
        }

        public VertexStates State
        {
            get =>
                _state;

            set
            {
                _state = value;
                VertexView.Dispatcher.Invoke(new Action(() =>
                {
                    VertexView.Fill = VertexColors[State];
                }));
            }
        }

        private static readonly Dictionary<VertexStates, Brush> VertexColors;

    }

    public enum VertexStates
    {
        None
        , MouseOn
        , SimplicesViewActive
    }

}