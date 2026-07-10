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
            // Voxel de 0.02f (2cm) para no derretir detalles como marcos de ventanas ni solapar muros
            List<MeshData> optimizedMeshes = OptimizeMeshes(meshes, 0.02f); 
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

        static List<MeshData> OptimizeMeshes(List<MeshData> inputMeshes, float voxelSize)
        {
            // El usuario solicitó mantener las piezas separadas.
            // Agrupamos por Guid (Elemento de Revit) en lugar de MaterialId
            var grouped = inputMeshes.GroupBy(m => m.Guid);
            var optimizedList = new List<MeshData>();

            foreach (var group in grouped)
            {
                var mergedMesh = new MeshData();
                mergedMesh.Guid = group.Key;
                // Asumimos el MaterialId de la primera malla del grupo (usualmente el elemento tiene 1 material predominante)
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
                
                // Decimar (simplificar) la pieza con el tamaño de grilla para eliminar vértices duplicados y triángulos minúsculos
                var decimated = VertexClusteringDecimator.Decimate(mergedMesh, voxelSize);
                
                // Solo agregar si quedaron triángulos válidos
                if (decimated.Indices.Count >= 3)
                {
                    optimizedList.Add(decimated);
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
                foreach (var mesh in meshes)
                {
                    writer.WriteLine($"o {mesh.Guid}");
                    writer.WriteLine($"usemtl {mesh.Guid}");

                    foreach (var v in mesh.Vertices)
                    {
                        string x = v.X.ToString(System.Globalization.CultureInfo.InvariantCulture);
                        string y = v.Y.ToString(System.Globalization.CultureInfo.InvariantCulture);
                        string z = v.Z.ToString(System.Globalization.CultureInfo.InvariantCulture);
                        writer.WriteLine($"v {x} {y} {z}");
                    }

                    for (int i = 0; i < mesh.Indices.Count; i += 3)
                    {
                        int i0 = mesh.Indices[i] + vertexOffset;
                        int i1 = mesh.Indices[i + 1] + vertexOffset;
                        int i2 = mesh.Indices[i + 2] + vertexOffset;
                        writer.WriteLine($"f {i0} {i1} {i2}");
                    }

                    vertexOffset += mesh.Vertices.Count;
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

                var meshBuilder = new MeshBuilder<VertexPosition, VertexEmpty, VertexEmpty>(meshData.Guid);
                var prim = meshBuilder.UsePrimitive(material);

                for (int i = 0; i < meshData.Indices.Count; i += 3)
                {
                    var v0 = meshData.Vertices[meshData.Indices[i]];
                    var v1 = meshData.Vertices[meshData.Indices[i + 1]];
                    var v2 = meshData.Vertices[meshData.Indices[i + 2]];

                    prim.AddTriangle(
                        new VertexPosition(v0),
                        new VertexPosition(v1),
                        new VertexPosition(v2)
                    );
                }

                scene.AddRigidMesh(meshBuilder, Matrix4x4.Identity);
            }

            var model = scene.ToGltf2();
            model.SaveGLB(outputPath);
        }
    }
}
