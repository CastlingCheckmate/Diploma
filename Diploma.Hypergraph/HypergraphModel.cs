using System.Diagnostics;
using System.Linq;

using Diploma.Extensions;

namespace Diploma.Hypergraph
{

    // TODO: DD
    [DebuggerDisplay("")]
    public sealed class HypergraphModel
    {

        private VertexModel[] _vertices;
        private SimplexModel[] _simplices;

        public HypergraphModel(VertexModel[] vertices, SimplexModel[] simplices)
        {
            Vertices = vertices.Unnulable().ToArray();
            Simplices = simplices.Unnulable().ToArray();
        }

        public VertexModel[] Vertices
        {
            get =>
                _vertices;

            private set =>
                _vertices = value;
        }

        public SimplexModel[] Simplices
        {
            get =>
                _simplices;

            private set =>
                _simplices = value;
        }

    }

}