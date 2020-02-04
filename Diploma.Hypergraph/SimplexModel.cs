using System.Diagnostics;
using System.Text;

namespace Diploma.Hypergraph
{

    [DebuggerDisplay("{ToString()}")]
    public sealed class SimplexModel
    {

        private string _name;
        private VertexModel[] _vertices;

        public SimplexModel(string name, VertexModel[] vertices)
        {
            Name = name;
            Vertices = vertices;
        }

        public string Name
        {
            get =>
                _name;

            private set =>
                _name = value;
        }

        public VertexModel[] Vertices
        {
            get =>
                _vertices;

            private set =>
                _vertices = value;
        }

        public override string ToString()
        {
            var simplexStringBuilder = new StringBuilder($"{{ \"{Name}\" ");
            foreach (var vertex in _vertices)
            {
                simplexStringBuilder.Append(vertex).Append(" ");
            }
            return simplexStringBuilder.Append("}").ToString();
        }

    }

}