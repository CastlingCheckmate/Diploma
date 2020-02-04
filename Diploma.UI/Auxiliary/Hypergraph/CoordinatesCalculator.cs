using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;

using Diploma.Hypergraph;

namespace Diploma.UI.Auxiliary.Hypergraph
{

    public static class CoordinatesCalculator
    {

        public static Point GetVertexCenterPoint(Size controlSize, int verticesCount, VertexModel model)
        {
            var vertexId = model.Id;
            if (verticesCount <= 0)
            {
                throw new ArgumentException("Count of vertices can't be lower than 0", nameof(verticesCount));
            }
            if (vertexId < 0 || vertexId >= verticesCount)
            {
                throw new ArgumentException("Invalid count of vertices", nameof(vertexId));
            }
            var radius = (int)(0.375 * Math.Min(controlSize.Width, controlSize.Height));
            var centerPoint = new Point(controlSize.Width / 2, controlSize.Height / 2);
            var vertexPoint = new Point(centerPoint.X + radius, centerPoint.Y);
            var angle = 360f / verticesCount * vertexId;
            var rotateMatrix = new Matrix();
            rotateMatrix.RotateAt(angle, centerPoint.X, centerPoint.Y);
            return rotateMatrix.Transform(vertexPoint);
        }

        public static Point GetSimplexCenterPoint(Size controlSize, int verticesCount, SimplexModel model)
        {
            var verticesCenterPoints = model.Vertices.Select(vertex => GetVertexCenterPoint(controlSize, verticesCount, vertex));
            return new Point(verticesCenterPoints.Average(vertexCenterPoint => vertexCenterPoint.X)
                             , verticesCenterPoints.Average(vertexCenterPoint => vertexCenterPoint.Y));
        }

    }

}