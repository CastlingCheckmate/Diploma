﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
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
            foreach (var vertex in _canvas.Children.OfType<Ellipse>().ToArray())
            {
                (vertex.Tag as Timer).Dispose();
                _canvas.Children.Remove(vertex);
                if (_vertexSimplicesViews.Keys.Contains(vertex))
                {
                    (_vertexSimplicesViews[vertex].Tag as Timer).Dispose();
                    _vertexSimplicesViews.Remove(vertex);
                }
            }
            foreach (var simplex in _canvas.Children.OfType<Polyline>().ToArray())
            {
                _canvas.Children.Remove(simplex);
            }
            if (Hypergraph is null)
            {
                return;
            }

            // simplices
            foreach (var simplex in Hypergraph.Simplices)
            {
                var simplexView = new Polyline();
                simplexView.Stroke = Brushes.Black;
                for (var i = 0; i < simplex.Vertices.Length; i++)
                {
                    simplexView.Points.Add(GetVertexCenterPoint(simplex.Vertices[i].Id));
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