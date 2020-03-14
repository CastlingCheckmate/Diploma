using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;

using Diploma.Extensions;
using Diploma.Hypergraph;
using Diploma.UI.Views.Controls;

namespace Diploma.UI.ViewModels.Hypergraph
{

    public sealed class HypergraphViewModel : IDisposable
    {

        private SimplexViewModel _capturedSimplex;
        private VertexSimplicesViewModel _capturedVertexSimplices;
        private HypergraphView _hypergraphView;

        public HypergraphViewModel(HypergraphModel model)
        {
            Model = model;
            CapturedSimplex = null;
            CapturedVertexSimplices = null;
        }

        public HypergraphModel Model
        {
            get;
        }

        public HypergraphView HypergraphView
        {
            get =>
                _hypergraphView;

            set
            {
                if (_hypergraphView != null)
                {
                    return;
                }
                _hypergraphView = value;
                Vertices = new VertexViewModel[Model.Vertices.Length];
                for (var i = 0; i < Vertices.Length; i++)
                {
                    Vertices[i] = new VertexViewModel(this, Model.Vertices[i]);
                }
                Simplices = new SimplexViewModel[Model.Simplices.Length];
                for (var i = 0; i < Simplices.Length; i++)
                {
                    Simplices[i] = new SimplexViewModel(this, Model.Simplices[i]);
                }
            }
        }

        public VertexViewModel[] Vertices
        {
            get;

            private set;
        }

        public VertexViewModel[] SimplexContains(SimplexViewModel simplexViewModel)
        {
            return Vertices.Where(vertexViewModel => simplexViewModel.Model.Vertices.Contains(vertexViewModel.Model)).ToArray();
        }

        public SimplexViewModel[] Simplices
        {
            get;

            private set;
        }

        public SimplexViewModel[] ContainingVertex(VertexViewModel vertexViewModel)
        {
            return Simplices.Where(simplexViewModel => simplexViewModel.Model.Vertices.Contains(vertexViewModel.Model)).ToArray();
        }

        public SimplexViewModel CapturedSimplex
        {
            get =>
                _capturedSimplex;

            set
            {
                if (CapturedSimplex != null)
                {
                    CapturedSimplex.State = SimplexStates.None;
                }
                _capturedSimplex = value;
            }
        }

        public VertexSimplicesViewModel CapturedVertexSimplices
        {
            get =>
                _capturedVertexSimplices;

            set
            {
                if (CapturedVertexSimplices != null)
                {
                    CapturedVertexSimplices.VertexSimplicesView.Dispatcher.Invoke(() =>
                    {
                        CapturedVertexSimplices.VertexSimplicesView.Visibility = Visibility.Collapsed;
                    });
                }
                _capturedVertexSimplices = value;
            }
        }

        public void Dispose()
        {
            Vertices.ForEach(vertex => vertex.Dispose());
        }

    }

}