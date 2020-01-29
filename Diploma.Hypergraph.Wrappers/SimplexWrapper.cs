using System.Diagnostics;
using System.Windows;

using Diploma.Hypergraph;

namespace Diploma.Hypergraph.Wrappers
{

    [DebuggerDisplay("")]
    public sealed class SimplexWrapper
    {

        private int _id;
        private Simplex _simplex;

        public SimplexWrapper(Simplex simplex)
        {
            Simplex = simplex;
        }

        public Simplex Simplex
        {
            get =>
                _simplex;

            private set =>
                _simplex = value;
        }

        public int Id
        {
            get =>
                _id;

            private set =>
                _id = value;
        }

    }

}