using System;
using System.Collections.Generic;
using System.Linq;

namespace Diploma.Hypergraph
{

    public static class ReductionAlgorithm
    {

        public static Hypergraph ReductionRecovery(int[] verticesGradesVector, int simplexVerticesCount)
        {
            if (verticesGradesVector is null || simplexVerticesCount <= 0 || verticesGradesVector.Sum() % simplexVerticesCount != 0)
            {
                return null;
            }
            Array.Sort(verticesGradesVector, (first, second) => second.CompareTo(first));
            var verticesGradesWithIndices = verticesGradesVector.Select((grade, index) => (Index: index, Grade: grade)).ToArray();
            var vertices = verticesGradesWithIndices.Select(vertex => new Vertex(vertex.Index, vertex.Grade.ToString())).ToArray();
            var simplices = new List<Simplex>();
            var fixedVerticesIndices = new Stack<Vertex>(simplexVerticesCount);
            fixedVerticesIndices.Push(new Vertex(-1, string.Empty));
            GetSimplices(verticesGradesWithIndices, simplexVerticesCount, fixedVerticesIndices, vertices, simplices);
            var simplicesCount = verticesGradesVector.Sum() / simplexVerticesCount;
            if (simplicesCount != simplices.Count)
            {
                return null;
            }
            return new Hypergraph(vertices, simplices.ToArray());
        }

        private static void GetSimplices((int Index, int Grade)[] verticesGradesWithIndices, int simplexVerticesCount
            , Stack<Vertex> fixedVerticesIndices, Vertex[] vertices, List<Simplex> simplices)
        {
            if (fixedVerticesIndices.Count == simplexVerticesCount + 1)
            {
                if (verticesGradesWithIndices.Any(vertex => fixedVerticesIndices.Any(v => v.Id == vertex.Index) && vertex.Grade == 0))
                {
                    return;
                }
                var fixedVertices = new Vertex[simplexVerticesCount];
                int j = simplexVerticesCount;
                foreach (var fixedVertexIndex in fixedVerticesIndices)
                {
                    for (var i = 0; i < verticesGradesWithIndices.Length; i++)
                    {
                        if (verticesGradesWithIndices[i].Index == fixedVertexIndex.Id)
                        {
                            verticesGradesWithIndices[i].Grade--;
                            fixedVertices[--j] = vertices[fixedVertexIndex.Id];
                            break;
                        }
                    }
                }
                simplices.Add(new Simplex(string.Empty, fixedVertices));
                return;
            }
            for (int i = fixedVerticesIndices.Peek().Id + 1; i < verticesGradesWithIndices.Length; i++)
            {
                fixedVerticesIndices.Push(vertices[verticesGradesWithIndices[i].Index]);
                GetSimplices(verticesGradesWithIndices, simplexVerticesCount, fixedVerticesIndices, vertices, simplices);
                fixedVerticesIndices.Pop();
            }
        }

    }

}