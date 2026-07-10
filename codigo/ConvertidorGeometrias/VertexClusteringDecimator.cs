using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace ConvertidorGeometrias
{
    public static class VertexClusteringDecimator
    {
        public struct Int3
        {
            public int X, Y, Z;
            public Int3(int x, int y, int z) { X = x; Y = y; Z = z; }
            public override int GetHashCode()
            {
                unchecked
                {
                    int hash = 17;
                    hash = hash * 31 + X;
                    hash = hash * 31 + Y;
                    hash = hash * 31 + Z;
                    return hash;
                }
            }
            public override bool Equals(object obj)
            {
                if (!(obj is Int3)) return false;
                Int3 o = (Int3)obj;
                return X == o.X && Y == o.Y && Z == o.Z;
            }
        }

        public static MeshData Decimate(MeshData inputMesh, float voxelSize = 0.1f)
        {
            if (voxelSize <= 0 || inputMesh.Indices.Count == 0) return inputMesh;

            // 1. Asignar vértices a celdas y acumular posiciones
            var cellAccumulators = new Dictionary<Int3, (Vector3 Sum, int Count, int NewIndex)>();
            var vertexToCell = new Int3[inputMesh.Vertices.Count];

            for (int i = 0; i < inputMesh.Vertices.Count; i++)
            {
                Vector3 v = inputMesh.Vertices[i];
                Int3 cell = new Int3(
                    (int)Math.Floor(v.X / voxelSize),
                    (int)Math.Floor(v.Y / voxelSize),
                    (int)Math.Floor(v.Z / voxelSize)
                );
                
                vertexToCell[i] = cell;

                if (cellAccumulators.TryGetValue(cell, out var acc))
                {
                    cellAccumulators[cell] = (acc.Sum + v, acc.Count + 1, -1);
                }
                else
                {
                    cellAccumulators[cell] = (v, 1, -1);
                }
            }

            // 2. Crear vértices representativos para cada celda
            var newMesh = new MeshData
            {
                MaterialId = inputMesh.MaterialId,
                Guid = inputMesh.Guid
            };

            int currentIndex = 0;
            var cellKeys = cellAccumulators.Keys.ToList();
            foreach (var key in cellKeys)
            {
                var acc = cellAccumulators[key];
                newMesh.Vertices.Add(acc.Sum / acc.Count); // Promedio
                cellAccumulators[key] = (acc.Sum, acc.Count, currentIndex);
                currentIndex++;
            }

            // 3. Reconstruir triángulos
            for (int i = 0; i < inputMesh.Indices.Count; i += 3)
            {
                int i0 = inputMesh.Indices[i];
                int i1 = inputMesh.Indices[i + 1];
                int i2 = inputMesh.Indices[i + 2];

                Int3 c0 = vertexToCell[i0];
                Int3 c1 = vertexToCell[i1];
                Int3 c2 = vertexToCell[i2];

                // Si dos o más vértices caen en la misma celda, el triángulo colapsa a una línea o punto y se descarta
                if (c0.Equals(c1) || c1.Equals(c2) || c2.Equals(c0))
                    continue;

                int new_i0 = cellAccumulators[c0].NewIndex;
                int new_i1 = cellAccumulators[c1].NewIndex;
                int new_i2 = cellAccumulators[c2].NewIndex;

                newMesh.Indices.Add(new_i0);
                newMesh.Indices.Add(new_i1);
                newMesh.Indices.Add(new_i2);
            }

            return newMesh;
        }
    }
}
