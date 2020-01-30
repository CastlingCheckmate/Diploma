using System.Diagnostics;
using System.Linq;

using Diploma.Extensions;

namespace Diploma.Hypergraph
{

    // TODO: DD
    [DebuggerDisplay("")]
    public sealed class Hypergraph
    {

        private Vertex[] _vertices;
        private Simplex[] _simplices;

        public Hypergraph(Vertex[] vertices, Simplex[] simplices)
        {
            Vertices = vertices.Unnulable().ToArray();
            Simplices = simplices.Unnulable().ToArray();
        }

        public Vertex[] Vertices
        {
            get =>
                _vertices;

            private set =>
                _vertices = value;
        }

        public Simplex[] Simplices
        {
            get =>
                _simplices;

            private set =>
                _simplices = value;
        }

    }

}