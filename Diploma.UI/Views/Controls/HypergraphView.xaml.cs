using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace Diploma.UI.Views.Controls
{

    public partial class HypergraphView : UserControl
    {

        private readonly int VertexDiameter = 20;

        private Dictionary<Ellipse, VertexSimplicesView> _vertexSimplicesViews;

        public HypergraphView()
        {
            InitializeComponent();
            _vertexSimplicesViews = new Dictionary<Ellipse, VertexSimplicesView>();
            Loaded += (sender, eventArgs) =>
            {
                Window.GetWindow(this).Closing += (_sender, _eventArgs) =>
                {
                    foreach (var vertex in _canvas.Children.OfType<Ellipse>().ToArray())
                    {
                        (vertex.Tag as Timer).Dispose();
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
            foreach (var vertex in _canvas.Children.OfType<Ellipse>().ToArray())
            {
                (vertex.Tag as Timer).Dispose();
                _canvas.Children.Remove(vertex);
                _vertexSimplicesViews.Remove(vertex);
            }
            if (Hypergraph is null)
            {
                return;
            }
            for (var i = 0; i < Hypergraph.Vertices.Length; i++)
            {
                var vertexModel = Hypergraph.Vertices[i];
                var vertexCenterPoint = GetVertexCenterPoint(i);
                var vertexView = new Ellipse()
                {
                    Width = VertexDiameter
                    , Height = VertexDiameter
                    , Fill = System.Windows.Media.Brushes.Black
                    , Margin = new Thickness(vertexCenterPoint.X - VertexDiameter / 2, vertexCenterPoint.Y - VertexDiameter / 2, 0, 0)
                    , Tag = new Timer(500)
                };
                vertexView.MouseEnter += (sender, eventArgs) =>
                {
                    var timer = vertexView.Tag as Timer;
                    vertexView.Fill = System.Windows.Media.Brushes.Gray;
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
                                vertexSimplicesView.MouseLeave += (__sender, __eventArgs) =>
                                {
                                    vertexSimplicesView.Visibility = Visibility.Collapsed;
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
                    timer.Start();
                };
                vertexView.MouseLeave += (sender, eventArgs) =>
                {
                    (vertexView.Tag as Timer).Stop();
                    vertexView.Fill = System.Windows.Media.Brushes.Black;
                };
                _canvas.Children.Add(vertexView);
            }

            System.Drawing.Point GetVertexCenterPoint(int vertexIndex)
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
                return vertexPoint[0];
            }
        }

    }

}