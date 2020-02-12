using System;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using Diploma.UI.Views.Controls;

namespace Diploma.UI.ViewModels.Hypergraph
{

    public sealed class VertexSimplicesViewModel : IDisposable
    {

        public VertexSimplicesViewModel(HypergraphViewModel hypergraphViewModel, VertexViewModel vertexViewModel, Point centerPoint
            , Action<object, MouseEventArgs> onMouseMove, Action<object, MouseButtonEventArgs> onMouseUp)
        {
            HypergraphViewModel = hypergraphViewModel;
            VertexViewModel = vertexViewModel;
            VertexSimplicesView = new VertexSimplicesView()
            {
                Visibility = Visibility.Collapsed
                , DataContext = this
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
            VertexSimplicesView.MouseUp += new MouseButtonEventHandler(onMouseUp);
            AfterVertexLeave.Elapsed += async (sender, eventArgs) =>
            {
                await VertexSimplicesView.Dispatcher.BeginInvoke(new Action(() =>
                {
                    VertexSimplicesView.Visibility = Visibility.Collapsed;
                }));
                VertexViewModel.State = VertexStates.None;
            };
        }

        public HypergraphViewModel HypergraphViewModel
        {
            get;
        }

        public VertexViewModel VertexViewModel
        {
            get;
        }

        public VertexSimplicesView VertexSimplicesView
        {
            get;
        }

        private Timer AfterVertexLeave
        {
            get;
        } = new Timer(1000);

        public void Dispose()
        {
            AfterVertexLeave.Dispose();
        }

    }

}