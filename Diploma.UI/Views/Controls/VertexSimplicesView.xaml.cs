using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Media;

using Diploma.UI.ViewModels.Hypergraph;
using System.IO;

namespace Diploma.UI.Views.Controls
{

    public partial class VertexSimplicesView : UserControl
    {

        private bool _fixed = false;
        private SimplexViewModel[] _simplices;
        private int _currentSimplexIndex;
        private double _currentAngle;

        public VertexSimplicesView()
        {
            InitializeComponent();
            var m = new Matrix();
            m.Translate(0, 101 - 13);
            var centerPoint = new Point(101, 101);
            m.RotateAt(30, centerPoint.X, centerPoint.Y);
            _scroll.Center = m.Transform(centerPoint);
        }

        private SimplexViewModel[] Simplices
        {
            get
            {
                if (_simplices is null)
                {
                    var dc = (VertexSimplicesViewModel)DataContext;
                    _simplices = dc.HypergraphViewModel.ContainingVertex(dc.VertexViewModel);
                }
                return _simplices;
            }
        }

        private int CurrentSimplexIndex
        {
            get =>
                _currentSimplexIndex;

            set
            {
                if (value < 0 || value >= Simplices.Length)
                {
                    return;
                }
                Simplices[CurrentSimplexIndex].State = SimplexStates.None;
                _currentSimplexIndex = value;
                Simplices[CurrentSimplexIndex].State = SimplexStates.ContainingVertex;

                _currentAngle = 30 + CurrentSimplexIndex * 300d / (Simplices.Length - 1);
                var centerPoint = new Point(_host.ActualWidth / 2, _host.ActualHeight / 2);
                var m = new Matrix();
                m.Translate(0, _host.ActualHeight / 2 - 13);
                m.RotateAt(_currentAngle, centerPoint.X, centerPoint.Y);
                _scroll.Center = m.Transform(centerPoint);
            }
        }

        private void EllipseGeometry_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _fixed = true;
        }

        private void EllipseGeometry_MouseUp(object sender, MouseButtonEventArgs e)
        {
            _fixed = false;
        }

        private void EllipseGeometry_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_fixed)
            {
                return;
            }
            var centerPoint = new Point(_host.ActualWidth / 2, _host.ActualHeight / 2);
            var pos = Mouse.GetPosition(_host);
            var zeroAnglePoint = new Point(_host.ActualWidth / 2, _host.ActualHeight);
            var v1 = zeroAnglePoint - centerPoint;
            v1.Y = -v1.Y;
            var v2 = pos - centerPoint;
            v2.Y = -v2.Y;
            if (v2.Length < 50)
            {
                return;
            }
            var angle = Vector.AngleBetween(v1, v2);
            if (v2.X <= 0)
            {
                angle = Math.Abs(angle);
            }
            else
            {
                angle = 360d - angle;
            }
            if (angle < 30d || angle > 330d)
            {
                return;
            }

            for (var i = 0; i < Simplices.Length; i++)
            {
                var startAngle = 30 + i * 300d / (Simplices.Length - 1);
                var endAngle = 30 + (i + 1) * 300d / (Simplices.Length - 1);
                if (angle >= startAngle && angle < endAngle)
                {
                    if (angle - startAngle < endAngle - angle)
                    {
                        angle = startAngle;
                        CurrentSimplexIndex = i;
                    }
                    else
                    {
                        angle = endAngle;
                        CurrentSimplexIndex = i + 1;
                    }
                    break;
                }
            }
            _currentAngle = angle;

            var m = new Matrix();
            m.Translate(0, _host.ActualHeight / 2 - 13);
            m.RotateAt(_currentAngle, centerPoint.X, centerPoint.Y);
            _scroll.Center = m.Transform(centerPoint);
        }

        private void UserControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                Simplices[_currentSimplexIndex].State = SimplexStates.ContainingVertex;
            }
            else
            {
                Simplices[_currentSimplexIndex].State = SimplexStates.None;
            }
            _fixed = false;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            CurrentSimplexIndex--;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            CurrentSimplexIndex++;
        }

    }

}