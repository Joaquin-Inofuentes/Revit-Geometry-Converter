using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using SharpGLTF.Geometry;
using SharpGLTF.Geometry.VertexTypes;
using SharpGLTF.Materials;
using SharpGLTF.Scenes;

namespace ConvertidorGeometrias
{
    public class MeshData
    {
        public int ElementId;
        public string Guid;
        public int MaterialId;
        public List<Vector3> Vertices = new List<Vector3>();
        public List<Vector3> Normals = new List<Vector3>();
        public List<int> Indices = new List<int>();
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Iniciando conversión de geometrías...");

            string defaultPath = @"C:\.TBT\Proyectos\_Revit_EXE_Geometrias\export.bin";
            string filePath = args.Length > 0 ? args[0] : defaultPath;

            if (!File.Exists(filePath))
            {
                Console.WriteLine($"Error: No se encontró el archivo '{filePath}'.");
                Console.ReadLine();
                return;
            }

            Console.WriteLine($"Leyendo archivo: {filePath}");
            List<MeshData> meshes = LeerBinario(filePath);
            Console.WriteLine($"Se leyeron {meshes.Count} mallas originales del archivo.");

            Console.WriteLine("Agrupando y diezmando (decimating) geometría...");
            // Decimado avanzado con Quadric Error Metrics (QEM)
            List<MeshData> optimizedMeshes = OptimizeMeshes(meshes);  
            Console.WriteLine($"Geometría reducida a {optimizedMeshes.Count} piezas separadas.");

            string directory = Path.GetDirectoryName(filePath);
            string baseName = Path.GetFileNameWithoutExtension(filePath);

            string objPath = Path.Combine(directory, baseName + ".obj");
            Console.WriteLine($"Exportando a OBJ: {objPath}");
            ExportToObj(optimizedMeshes, objPath);
            Console.WriteLine("OBJ exportado con éxito.");

            string glbPath = Path.Combine(directory, baseName + "_optimizado.glb");
            Console.WriteLine($"Exportando a GLB: {glbPath}");
            ExportToGlb(optimizedMeshes, glbPath);
            Console.WriteLine("GLB exportado con éxito.");

            Console.WriteLine("¡Proceso terminado!");
            Console.ReadLine(); // Pausa
        }

        static List<MeshData> OptimizeMeshes(List<MeshData> inputMeshes)
        {
            // El usuario solicitó mantener las piezas separadas.
            // Agrupamos por Guid (Elemento de Revit) en lugar de MaterialId
            var grouped = inputMeshes.GroupBy(m => m.Guid);
            var optimizedList = new List<MeshData>();

            foreach (var group in grouped)
            {
                var mergedMesh = new MeshData();
                mergedMesh.Guid = group.Key;
                mergedMesh.MaterialId = group.First().MaterialId;

                // Unir todas las caras de este elemento
                int vertexOffset = 0;
                foreach (var mesh in group)
                {
                    mergedMesh.Vertices.AddRange(mesh.Vertices);
                    foreach (var index in mesh.Indices)
                    {
                        mergedMesh.Indices.Add(index + vertexOffset);
                    }
                    vertexOffset += mesh.Vertices.Count;
                }
                
                int originalTriangles = mergedMesh.Indices.Count / 3;
                if (originalTriangles < 2) continue;

                // Preparar datos para QEM MeshDecimator
                var mdVertices = new MeshDecimator.Math.Vector3d[mergedMesh.Vertices.Count];
                for (int i = 0; i < mergedMesh.Vertices.Count; i++)
                {
                    mdVertices[i] = new MeshDecimator.Math.Vector3d(
                        mergedMesh.Vertices[i].X,
                        mergedMesh.Vertices[i].Y,
                        mergedMesh.Vertices[i].Z
                    );
                }
                var mdIndices = mergedMesh.Indices.ToArray();

                var srcMesh = new MeshDecimator.Mesh(mdVertices, mdIndices);

                // Cálculo dinámico del target:
                // Tazas/picaportes tendrán miles de triángulos, podemos diezmarlos al 5% o 10%
                // Muros planos tendrán menos, podemos dejarlos al 20% o 50%
                int targetTriangles = originalTriangles;
                if (originalTriangles > 1000) targetTriangles = (int)(originalTriangles * 0.10); // 10%
                else if (originalTriangles > 200) targetTriangles = (int)(originalTriangles * 0.20); // 20%
                else if (originalTriangles > 50) targetTriangles = (int)(originalTriangles * 0.40); // 40%
                else targetTriangles = Math.Max((int)(originalTriangles * 0.60), 2); // 60%, minimo 2

                var decimatedMdMesh = MeshDecimator.MeshDecimation.DecimateMesh(
                    MeshDecimator.Algorithm.FastQuadricMesh, 
                    srcMesh, 
                    targetTriangles
                );

                // Reconstruir MeshData de salida
                var outMesh = new MeshData
                {
                    Guid = mergedMesh.Guid,
                    MaterialId = mergedMesh.MaterialId
                };
                
                foreach (var v in decimatedMdMesh.Vertices)
                {
                    outMesh.Vertices.Add(new Vector3((float)v.x, (float)v.y, (float)v.z));
                }
                outMesh.Indices.AddRange(decimatedMdMesh.Indices);
                
                // Calculate smooth/flat normals
                outMesh.Normals = new List<Vector3>(new Vector3[outMesh.Vertices.Count]);
                var normalCounts = new int[outMesh.Vertices.Count];

                for (int i = 0; i < outMesh.Indices.Count; i += 3)
                {
                    int i0 = outMesh.Indices[i];
                    int i1 = outMesh.Indices[i + 1];
                    int i2 = outMesh.Indices[i + 2];

                    Vector3 v0 = outMesh.Vertices[i0];
                    Vector3 v1 = outMesh.Vertices[i1];
                    Vector3 v2 = outMesh.Vertices[i2];

                    Vector3 normal = Vector3.Normalize(Vector3.Cross(v1 - v0, v2 - v0));

                    outMesh.Normals[i0] += normal;
                    outMesh.Normals[i1] += normal;
                    outMesh.Normals[i2] += normal;
                    
                    normalCounts[i0]++;
                    normalCounts[i1]++;
                    normalCounts[i2]++;
                }

                for (int i = 0; i < outMesh.Normals.Count; i++)
                {
                    if (normalCounts[i] > 0)
                        outMesh.Normals[i] = Vector3.Normalize(outMesh.Normals[i] / normalCounts[i]);
                }

                if (outMesh.Indices.Count >= 3)
                {
                    optimizedList.Add(outMesh);
                }
            }

            return optimizedList;
        }

        static List<MeshData> LeerBinario(string path)
        {
            var meshes = new List<MeshData>();

            using (var fs = File.OpenRead(path))
            using (var reader = new BinaryReader(fs))
            {
                while (fs.Position < fs.Length)
                {
                    var mesh = new MeshData();
                    mesh.ElementId = reader.ReadInt32();
                    mesh.Guid = reader.ReadString();
                    mesh.MaterialId = reader.ReadInt32();

                    int vertexCount = reader.ReadInt32();
                    mesh.Vertices.Capacity = vertexCount;
                    for (int i = 0; i < vertexCount; i++)
                    {
                        float x = reader.ReadSingle();
                        float y = reader.ReadSingle();
                        float z = reader.ReadSingle();
                        
                        // Rotar de Revit (Z-Up) a estándar GLTF/Visualizadores (Y-Up)
                        mesh.Vertices.Add(new Vector3(x, z, -y));
                    }

                    int triangleCount = reader.ReadInt32();
                    mesh.Indices.Capacity = triangleCount * 3;
                    for (int i = 0; i < triangleCount; i++)
                    {
                        mesh.Indices.Add((int)reader.ReadUInt32());
                        mesh.Indices.Add((int)reader.ReadUInt32());
                        mesh.Indices.Add((int)reader.ReadUInt32());
                    }

                    meshes.Add(mesh);
                }
            }

            return meshes;
        }

        static void ExportToObj(List<MeshData> meshes, string outputPath)
        {
            using (StreamWriter writer = new StreamWriter(outputPath))
            {
                writer.WriteLine("# Exportado desde ConvertidorGeometrias");
                
                int vertexOffset = 1;
                int normalOffset = 1;

                foreach (var mesh in meshes)
                {
                    writer.WriteLine($"o {mesh.Guid}");
                    writer.WriteLine($"usemtl {mesh.Guid}");

                    // Vertices
                    foreach (var v in mesh.Vertices)
                    {
                        string x = v.X.ToString(System.Globalization.CultureInfo.InvariantCulture);
                        string y = v.Y.ToString(System.Globalization.CultureInfo.InvariantCulture);
                        string z = v.Z.ToString(System.Globalization.CultureInfo.InvariantCulture);
                        writer.WriteLine($"v {x} {y} {z}");
                    }
                    
                    // Normals
                    bool hasNormals = mesh.Normals != null && mesh.Normals.Count == mesh.Vertices.Count;
                    if (hasNormals)
                    {
                        foreach (var vn in mesh.Normals)
                        {
                            string nx = vn.X.ToString(System.Globalization.CultureInfo.InvariantCulture);
                            string ny = vn.Y.ToString(System.Globalization.CultureInfo.InvariantCulture);
                            string nz = vn.Z.ToString(System.Globalization.CultureInfo.InvariantCulture);
                            writer.WriteLine($"vn {nx} {ny} {nz}");
                        }
                    }

                    // Faces
                    for (int i = 0; i < mesh.Indices.Count; i += 3)
                    {
                        int i0 = mesh.Indices[i] + vertexOffset;
                        int i1 = mesh.Indices[i + 1] + vertexOffset;
                        int i2 = mesh.Indices[i + 2] + vertexOffset;

                        if (hasNormals)
                        {
                            int n0 = mesh.Indices[i] + normalOffset;
                            int n1 = mesh.Indices[i + 1] + normalOffset;
                            int n2 = mesh.Indices[i + 2] + normalOffset;
                            writer.WriteLine($"f {i0}//{n0} {i1}//{n1} {i2}//{n2}");
                        }
                        else
                        {
                            writer.WriteLine($"f {i0} {i1} {i2}");
                        }
                    }

                    vertexOffset += mesh.Vertices.Count;
                    if (hasNormals) normalOffset += mesh.Normals.Count;
                }
            }
        }

        static void ExportToGlb(List<MeshData> meshes, string outputPath)
        {
            var scene = new SceneBuilder();

            // Usamos un diccionario para cachear materiales
            var materials = new Dictionary<int, MaterialBuilder>();
            var random = new Random(42);

            foreach (var meshData in meshes)
            {
                if (!materials.TryGetValue(meshData.MaterialId, out var material))
                {
                    // Asignar un color base para diferenciar los materiales en Unity
                    float r = (float)random.NextDouble() * 0.7f + 0.3f;
                    float g = (float)random.NextDouble() * 0.7f + 0.3f;
                    float b = (float)random.NextDouble() * 0.7f + 0.3f;

                    material = new MaterialBuilder(meshData.Guid)
                        .WithDoubleSide(true)
                        .WithMetallicRoughnessShader()
                        .WithChannelParam(KnownChannel.BaseColor, new Vector4(r, g, b, 1.0f));
                    materials[meshData.MaterialId] = material;
                }

                bool hasNormals = meshData.Normals != null && meshData.Normals.Count == meshData.Vertices.Count;

                if (hasNormals)
                {
                    var meshBuilder = new MeshBuilder<VertexPositionNormal, VertexEmpty, VertexEmpty>(meshData.Guid);
                    var prim = meshBuilder.UsePrimitive(material);

                    for (int i = 0; i < meshData.Indices.Count; i += 3)
                    {
                        var i0 = meshData.Indices[i];
                        var i1 = meshData.Indices[i + 1];
                        var i2 = meshData.Indices[i + 2];

                        prim.AddTriangle(
                            new VertexPositionNormal(meshData.Vertices[i0], meshData.Normals[i0]),
                            new VertexPositionNormal(meshData.Vertices[i1], meshData.Normals[i1]),
                            new VertexPositionNormal(meshData.Vertices[i2], meshData.Normals[i2])
                        );
                    }
                    scene.AddRigidMesh(meshBuilder, Matrix4x4.Identity);
                }
                else
                {
                    var meshBuilder = new MeshBuilder<VertexPosition, VertexEmpty, VertexEmpty>(meshData.Guid);
                    var prim = meshBuilder.UsePrimitive(material);

                    for (int i = 0; i < meshData.Indices.Count; i += 3)
                    {
                        var i0 = meshData.Indices[i];
                        var i1 = meshData.Indices[i + 1];
                        var i2 = meshData.Indices[i + 2];

                        prim.AddTriangle(
                            new VertexPosition(meshData.Vertices[i0]),
                            new VertexPosition(meshData.Vertices[i1]),
                            new VertexPosition(meshData.Vertices[i2])
                        );
                    }
                    scene.AddRigidMesh(meshBuilder, Matrix4x4.Identity);
                }
            }

            var model = scene.ToGltf2();
            model.SaveGLB(outputPath);
        }
    }
}
