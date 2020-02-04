using System;
using System.Diagnostics;

namespace Diploma.Hypergraph
{

    [DebuggerDisplay("{ToString()}")]
    public sealed class VertexModel
    {

        private int _id;
        private string _name;

        public VertexModel(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public int Id
        {
            get =>
                _id;

            private set =>
                _id = value;
        }

        public string Name
        {
            get =>
                _name;

            private set =>
                _name = value;
        }

        public override string ToString()
        {
            return $"{{Name: {Name} Id: {Id}}}";
        }

    }

}