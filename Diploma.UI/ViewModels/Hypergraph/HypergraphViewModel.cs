using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Diploma.Extensions;
using Diploma.Hypergraph;
using Diploma.UI.Auxiliary.Hypergraph;
using Diploma.UI.Views.Controls;

namespace Diploma.UI.ViewModels.Hypergraph
{

    public sealed class HypergraphViewModel : IDisposable
    {

        private HypergraphModel _model;
        private VertexViewModel[] _vertices;
        private SimplexViewModel[] _simplices;
        private SimplexViewModel _fixedSimplex;
        private HypergraphView _hypergraphView;

        public HypergraphViewModel(HypergraphModel model, HypergraphView hypergraphView, Action<object, MouseEventArgs> onMouseMove)
        {
            _hypergraphView = hypergraphView;
            _model = model;
            _vertices = Model.Vertices.Select(vertexModel => new VertexViewModel(this, vertexModel, onMouseMove
                , CoordinatesCalculator.GetVertexCenterPoint(new Size(hypergraphView.ActualWidth, hypergraphView.ActualHeight), Model.Vertices.Length, vertexModel))).ToArray();
            _simplices = Model.Simplices.Select(simplexModel => new SimplexViewModel(this, simplexModel, onMouseMove
                , CoordinatesCalculator.GetSimplexCenterPoint(new Size(hypergraphView.ActualWidth, hypergraphView.ActualHeight), Model.Vertices.Length, simplexModel))).ToArray();
            _fixedSimplex = null;
        }

        public HypergraphModel Model =>
            _model;

        public HypergraphView HypergraphView =>
            _hypergraphView;

        public VertexViewModel[] Vertices =>
            _vertices;

        public VertexViewModel[] SimplexContains(SimplexViewModel simplexViewModel)
        {
            return _vertices.Where(vertexViewModel => simplexViewModel.Model.Vertices.Contains(vertexViewModel.Model)).ToArray();
        }

        public SimplexViewModel[] Simplices =>
            _simplices;

        public SimplexViewModel[] ContainingVertex(VertexViewModel vertexViewModel)
        {
            return _simplices.Where(simplexViewModel => simplexViewModel.Model.Vertices.Contains(vertexViewModel.Model)).ToArray();
        }

        public SimplexViewModel FixedSimplex
        {
            get =>
                _fixedSimplex;

            set
            {
                if (FixedSimplex != null)
                {
                    FixedSimplex.Center.Fill = Brushes.Gray;
                    for (var i = 0; i < FixedSimplex.Edges.Length; i++)
                    {
                        FixedSimplex.Edges[i].Stroke = Brushes.Gray;
                    }
                }
                _fixedSimplex = value;
            }
        }

        public void Dispose()
        {
            _vertices.ForEach(vertex => vertex.Dispose());
        }

    }

}