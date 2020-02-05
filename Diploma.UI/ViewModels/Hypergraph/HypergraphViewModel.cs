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

        private SimplexViewModel _capturedSimplex;

        public HypergraphViewModel(HypergraphModel model, HypergraphView hypergraphView, Action<object, MouseEventArgs> onMouseMove)
        {
            HypergraphView = hypergraphView;
            Model = model;
            Vertices = Model.Vertices.Select(vertexModel => new VertexViewModel(this, vertexModel, onMouseMove
                , CoordinatesCalculator.GetVertexCenterPoint(new Size(hypergraphView.ActualWidth, hypergraphView.ActualHeight), Model.Vertices.Length, vertexModel))).ToArray();
            Simplices = Model.Simplices.Select(simplexModel => new SimplexViewModel(this, simplexModel, onMouseMove
                , CoordinatesCalculator.GetSimplexCenterPoint(new Size(hypergraphView.ActualWidth, hypergraphView.ActualHeight), Model.Vertices.Length, simplexModel))).ToArray();
            CapturedSimplex = null;
        }

        public HypergraphModel Model
        {
            get;
        }

        public HypergraphView HypergraphView
        {
            get;
        }

        public VertexViewModel[] Vertices
        {
            get;
        }

        public VertexViewModel[] SimplexContains(SimplexViewModel simplexViewModel)
        {
            return Vertices.Where(vertexViewModel => simplexViewModel.Model.Vertices.Contains(vertexViewModel.Model)).ToArray();
        }

        public SimplexViewModel[] Simplices
        {
            get;
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
                    CapturedSimplex.Center.Fill = Brushes.Gray;
                    for (var i = 0; i < CapturedSimplex.Edges.Length; i++)
                    {
                        CapturedSimplex.Edges[i].Stroke = Brushes.Gray;
                    }
                }
                _capturedSimplex = value;
            }
        }

        public void Dispose()
        {
            Vertices.ForEach(vertex => vertex.Dispose());
        }

    }

}