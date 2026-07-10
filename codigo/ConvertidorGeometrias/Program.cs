using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
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
        // Id de BuiltInCategory de Revit (formato v3); 0 si no vino en el archivo.
        public int CategoryId;
        // Color superficial simple del material de Revit (formato v2+). Alpha < 255 = vidrio/transparente.
        public byte ColR = 200, ColG = 200, ColB = 200, ColA = 255;
        public bool HasColor;
        public List<Vector3> Vertices = new List<Vector3>();
        public List<Vector3> Normals = new List<Vector3>();
        public List<int> Indices = new List<int>();
    }

    public class PipelineStats
    {
        public int FormatVersion = 1;
        public int InputMeshes;
        public int OutputPieces;
        public long VertsIn, VertsWelded, VertsOut;
        public long TrisIn, TrisOut;
        public long DegenerateRemoved;
        public int Fallbacks;      // piezas donde el decimado se descartó por romper la malla
        public int Retries;        // piezas que necesitaron un decimado más conservador
        public int Discarded;      // piezas sin geometría útil
        public int NotDecimated;   // piezas chicas (cubos/prismas) que se dejaron intactas
        public int ProtectedCat;   // piezas intactas por categoría protegida (muros/suelos/techos)
        public int Transparent;    // materiales con alpha (vidrios)
        public int Instanced;      // piezas que reusan la malla de otra idéntica (GLB más chico)
        public int Repaired;       // piezas rotas reparadas restaurando su geometría original
        public int PoolMeshes;     // geometrías únicas en el .tbv tras deduplicar
        public long TbvBytes;      // tamaño del binario de visor
    }

    public class PiezaRota
    {
        public string Guid;
        public int ElementId;
        public string Problema;
        public string Detalle;
    }

    class Program
    {
        // "TBT2" en little-endian: cabecera del formato v2 (con color de material)
        const int FormatMagic = 0x32544254;

        // Tolerancia de soldadura: grilla de 1 mm
        const double WeldGrid = 1000.0;
        // Un spike existe si la caja del decimado se sale de la original más de este % de su diagonal
        const float SpikeTolerance = 0.005f;
        // Guarda anti-cubos: un cubo soldado tiene 8 vértices / 12 triángulos.
        // Solo se decima con margen de seguridad por encima de eso — el decimado se aplica
        // únicamente cuando es necesario, no a todas las piezas.
        const int MinVertsParaDecimar = 16;
        const int MinTrisParaDecimar = 20;

        // ===== CATEGORÍAS PROTEGIDAS =====
        // Piezas de estas categorías NO se decimán ni descomponen: por naturaleza ya vienen
        // optimizadas de Revit y son las que más feo se rompen. Solo se les aplica soldadura
        // y limpieza de degenerados (sin pérdida). Ids de BuiltInCategory de Revit.
        // Agregar/quitar categorías acá según necesidad.
        static readonly HashSet<int> CategoriasProtegidas = new HashSet<int>
        {
            -2000011, // OST_Walls   (Muros)
            -2000032, // OST_Floors  (Suelos)
            -2000035, // OST_Roofs   (Techos)
            -2000014, // OST_Windows (Ventanas: el decimado rompe los paños)
        };

        // El decimado destroza los paños de vidrio (mallas casi planas y finas).
        // Un vidrio queda protegido si es translúcido (alpha < 255) o si es un
        // panel casi negro sin categoría/ventana — el patrón de un paño cuyo
        // asset de apariencia no llegó al exportador.
        static bool EsVidrioProtegido(MeshData m)
        {
            if (m.HasColor && m.ColA < 255) return true;
            bool casiNegro = m.HasColor && m.ColR < 70 && m.ColG < 70 && m.ColB < 70;
            return casiNegro && (m.CategoryId == 0 || m.CategoryId == -2000014 || m.CategoryId == -2000023);
        }

        static void Main(string[] args)
        {
            var sw = Stopwatch.StartNew();
            Console.OutputEncoding = Encoding.UTF8;
            Console.WriteLine("Iniciando conversión de geometrías...");

            string defaultPath = @"C:\.TBT\Proyectos\_Revit_EXE_Geometrias\export.bin";
            string filePath = args.Length > 0 && !args[0].StartsWith("--") ? args[0] : defaultPath;
            bool pause = !args.Contains("--nopause") && !Console.IsInputRedirected;

            if (!File.Exists(filePath))
            {
                Console.WriteLine($"Error: No se encontró el archivo '{filePath}'.");
                if (pause) Console.ReadLine();
                return;
            }

            var stats = new PipelineStats();

            Console.WriteLine($"Leyendo archivo: {filePath}");
            List<MeshData> meshes = LeerBinario(filePath, stats);
            Console.WriteLine($"Formato v{stats.FormatVersion}{(stats.FormatVersion >= 2 ? " (con color de material)" : " (legado, colores aleatorios)")}. Se leyeron {meshes.Count} mallas.");

            Console.WriteLine("Agrupando, soldando y diezmando (QEM) geometría...");
            // Guardamos la versión soldada de cada pieza para la validación final independiente
            var weldedOriginals = new Dictionary<string, MeshData>();
            List<MeshData> optimizedMeshes = OptimizeMeshes(meshes, stats, weldedOriginals);
            Console.WriteLine($"Geometría reducida a {optimizedMeshes.Count} piezas separadas.");

            // VALIDACIÓN: chequeo independiente pieza por pieza contra el original soldado.
            Console.WriteLine("Validando piezas...");
            var rotasDetectadas = ValidarPiezas(weldedOriginals, optimizedMeshes);

            // REPARACIÓN: toda pieza rota se restaura a su geometría original soldada
            // (sin decimar). Preferimos una pieza pesada a una pieza rota.
            if (rotasDetectadas.Count > 0)
            {
                Console.WriteLine($"Se detectaron {rotasDetectadas.Count} piezas rotas. Reparando...");
                RepararPiezas(rotasDetectadas, weldedOriginals, optimizedMeshes, stats);
            }

            // RE-VALIDACIÓN tras reparar: esto es lo que se informa como estado final.
            var rotasFinales = rotasDetectadas.Count > 0
                ? ValidarPiezas(weldedOriginals, optimizedMeshes)
                : rotasDetectadas;

            // Recalcular totales de salida (la reparación pudo devolver geometría original)
            stats.VertsOut = optimizedMeshes.Sum(m => (long)m.Vertices.Count);
            stats.TrisOut = optimizedMeshes.Sum(m => (long)m.Indices.Count / 3);
            stats.OutputPieces = optimizedMeshes.Count;

            string directory = Path.GetDirectoryName(filePath);
            string baseName = Path.GetFileNameWithoutExtension(filePath);

            string objPath = Path.Combine(directory, baseName + ".obj");
            Console.WriteLine($"Exportando a OBJ: {objPath}");
            ExportToObj(optimizedMeshes, objPath);

            string glbPath = Path.Combine(directory, baseName + "_optimizado.glb");
            Console.WriteLine($"Exportando a GLB: {glbPath}");
            ExportToGlb(optimizedMeshes, glbPath, stats);

            string tbvPath = Path.Combine(directory, baseName + ".tbv");
            Console.WriteLine($"Exportando binario de visor (dedup + índice espacial): {tbvPath}");
            ExportToViewerBin(optimizedMeshes, tbvPath, stats);

            string reportPath = Path.Combine(directory, baseName + "_reporte.txt");

            sw.Stop();
            string reporte = ConstruirReporte(stats, rotasDetectadas, rotasFinales, sw.Elapsed, objPath, glbPath);
            Console.WriteLine(reporte);
            File.WriteAllText(reportPath, reporte);
            Console.WriteLine($"Reporte guardado en: {reportPath}");

            Console.WriteLine("¡Proceso terminado!");
            if (pause) Console.ReadLine();
        }

        static string ConstruirReporte(PipelineStats s, List<PiezaRota> detectadas, List<PiezaRota> rotas, TimeSpan elapsed, string objPath, string glbPath)
        {
            var sb = new StringBuilder();
            sb.AppendLine();
            sb.AppendLine("=========== ESTADÍSTICAS ===========");
            sb.AppendLine($"Formato de entrada:     v{s.FormatVersion}");
            sb.AppendLine($"Mallas de entrada:      {s.InputMeshes}");
            sb.AppendLine($"Piezas de salida:       {s.OutputPieces} (descartadas vacías: {s.Discarded})");
            sb.AppendLine($"Vértices:  {s.VertsIn:N0} -> soldados {s.VertsWelded:N0} -> finales {s.VertsOut:N0}  ({Pct(s.VertsOut, s.VertsIn)})");
            sb.AppendLine($"Triángulos: {s.TrisIn:N0} -> finales {s.TrisOut:N0}  ({Pct(s.TrisOut, s.TrisIn)})");
            sb.AppendLine($"Triángulos degenerados eliminados: {s.DegenerateRemoved:N0}");
            sb.AppendLine($"Piezas chicas sin decimar (cubos/prismas protegidos): {s.NotDecimated}");
            sb.AppendLine($"Piezas intactas por categoría protegida (muros/suelos/techos): {s.ProtectedCat}");
            sb.AppendLine($"Piezas con reintento conservador:  {s.Retries}");
            sb.AppendLine($"Piezas con fallback (sin decimar): {s.Fallbacks}");
            sb.AppendLine($"Piezas rotas detectadas y REPARADAS con geometría original: {s.Repaired}");
            sb.AppendLine($"Materiales transparentes (vidrio): {s.Transparent}");
            sb.AppendLine($"Piezas instanciadas (malla compartida): {s.Instanced}");
            sb.AppendLine($"Geometrías únicas en el .tbv (dedup): {s.PoolMeshes} de {s.OutputPieces} piezas");
            if (s.TbvBytes > 0) sb.AppendLine($"Binario de visor (.tbv): {s.TbvBytes / 1024:N0} KB");
            sb.AppendLine($"OBJ: {new FileInfo(objPath).Length / 1024:N0} KB   GLB: {new FileInfo(glbPath).Length / 1024:N0} KB");
            sb.AppendLine($"Tiempo total: {elapsed.TotalSeconds:F1} s");
            sb.AppendLine("====================================");
            sb.AppendLine();

            if (detectadas.Count > 0)
            {
                sb.AppendLine($"Piezas rotas detectadas en la primera validación ({detectadas.Count}):");
                foreach (var r in detectadas)
                {
                    sb.AppendLine($"  - ElementId {r.ElementId} | GUID {r.Guid} | {r.Problema}");
                }
                sb.AppendLine();
            }

            if (rotas.Count == 0)
            {
                sb.AppendLine("VALIDACIÓN FINAL: OK — ninguna pieza rota en la salida.");
            }
            else
            {
                sb.AppendLine($"VALIDACIÓN FINAL: ¡ATENCIÓN! {rotas.Count} pieza(s) siguen con problemas tras reparar:");
                foreach (var r in rotas)
                {
                    sb.AppendLine($"  - ElementId {r.ElementId} | GUID {r.Guid}");
                    sb.AppendLine($"    Problema: {r.Problema}");
                    sb.AppendLine($"    Detalle:  {r.Detalle}");
                }
            }

            return sb.ToString();
        }

        static string Pct(long now, long before) =>
            before == 0 ? "n/a" : $"{100.0 * now / before:F1}% del original";

        // ================== VALIDACIÓN FINAL ==================
        // Chequeo independiente del pipeline: compara cada pieza final contra su original
        // soldado. Detecta piezas perdidas, NaN, spikes (vértices fuera de caja), colapsos
        // y pérdida/ganancia de superficie. Reporta GUID, ElementId y los vectores implicados.
        static List<PiezaRota> ValidarPiezas(Dictionary<string, MeshData> originales, List<MeshData> finales)
        {
            var rotas = new List<PiezaRota>();
            var finalesPorGuid = finales.ToDictionary(m => m.Guid);

            foreach (var kv in originales)
            {
                var orig = kv.Value;
                if (orig.Indices.Count < 3) continue; // no había geometría útil de entrada

                if (!finalesPorGuid.TryGetValue(kv.Key, out var fin))
                {
                    rotas.Add(new PiezaRota
                    {
                        Guid = kv.Key,
                        ElementId = orig.ElementId,
                        Problema = "PIEZA PERDIDA: existía en la entrada y no está en la salida",
                        Detalle = $"original: {orig.Vertices.Count} verts, {orig.Indices.Count / 3} tris, bbox {BBoxStr(orig.Vertices)}"
                    });
                    continue;
                }

                // 1. NaN / Infinity
                var nanVerts = new List<string>();
                for (int i = 0; i < fin.Vertices.Count && nanVerts.Count < 3; i++)
                {
                    var v = fin.Vertices[i];
                    if (float.IsNaN(v.X) || float.IsNaN(v.Y) || float.IsNaN(v.Z) ||
                        float.IsInfinity(v.X) || float.IsInfinity(v.Y) || float.IsInfinity(v.Z))
                    {
                        nanVerts.Add($"v[{i}]=({v.X}, {v.Y}, {v.Z})");
                    }
                }
                if (nanVerts.Count > 0)
                {
                    rotas.Add(new PiezaRota
                    {
                        Guid = kv.Key,
                        ElementId = fin.ElementId,
                        Problema = "VÉRTICES NaN/INFINITY",
                        Detalle = string.Join("; ", nanVerts)
                    });
                    continue;
                }

                GetBounds(orig.Vertices, out var oMin, out var oMax);
                GetBounds(fin.Vertices, out var fMin, out var fMax);
                float diag = (oMax - oMin).Length();
                float tol = diag * SpikeTolerance + 1e-4f;

                // 2. Spike: vértices finales fuera de la caja original
                var spikes = new List<string>();
                for (int i = 0; i < fin.Vertices.Count && spikes.Count < 3; i++)
                {
                    var v = fin.Vertices[i];
                    if (v.X < oMin.X - tol || v.Y < oMin.Y - tol || v.Z < oMin.Z - tol ||
                        v.X > oMax.X + tol || v.Y > oMax.Y + tol || v.Z > oMax.Z + tol)
                    {
                        spikes.Add($"v[{i}]=({v.X:F4}, {v.Y:F4}, {v.Z:F4})");
                    }
                }
                if (spikes.Count > 0)
                {
                    rotas.Add(new PiezaRota
                    {
                        Guid = kv.Key,
                        ElementId = fin.ElementId,
                        Problema = "SPIKE: vértices fuera del volumen original",
                        Detalle = $"bbox original {BBoxStr(orig.Vertices)}; vértices fuera: {string.Join("; ", spikes)}"
                    });
                    continue;
                }

                // 3. Colapso: la pieza final quedó mucho más chica que la original
                float finDiag = (fMax - fMin).Length();
                if (diag > 1e-4f && finDiag < diag * 0.5f)
                {
                    rotas.Add(new PiezaRota
                    {
                        Guid = kv.Key,
                        ElementId = fin.ElementId,
                        Problema = "COLAPSO: la pieza se encogió más del 50%",
                        Detalle = $"bbox original {BBoxStr(orig.Vertices)} (diag {diag:F3}) -> final {BBoxStr(fin.Vertices)} (diag {finDiag:F3})"
                    });
                    continue;
                }

                // 4. Superficie: agujeros grandes o geometría duplicada
                double aOrig = TotalArea(orig);
                double aFin = TotalArea(fin);
                if (aOrig > 1e-10 && (aFin < aOrig * 0.6 || aFin > aOrig * 1.4))
                {
                    rotas.Add(new PiezaRota
                    {
                        Guid = kv.Key,
                        ElementId = fin.ElementId,
                        Problema = "ÁREA ANÓMALA: superficie cambió más de ±40% (agujeros o colapso)",
                        Detalle = $"área original {aOrig:F4} -> final {aFin:F4} (ratio {aFin / aOrig:F2})"
                    });
                }
            }

            return rotas;
        }

        // ================== REPARACIÓN ==================
        // Toda pieza detectada como rota se restaura a su geometría original soldada
        // (weld + limpieza de degenerados, sin decimar). Si la pieza se perdió, se re-agrega.
        static void RepararPiezas(List<PiezaRota> rotas, Dictionary<string, MeshData> originales,
                                  List<MeshData> finales, PipelineStats stats)
        {
            foreach (var rota in rotas)
            {
                if (!originales.TryGetValue(rota.Guid, out var orig) || orig.Indices.Count < 3) continue;

                var reparada = ClonarConNormales(orig);

                int idx = finales.FindIndex(m => m.Guid == rota.Guid);
                if (idx >= 0) finales[idx] = reparada;
                else finales.Add(reparada);

                stats.Repaired++;
                Console.WriteLine($"  Reparada: ElementId {rota.ElementId} | {rota.Problema}");
            }
        }

        static MeshData ClonarConNormales(MeshData src)
        {
            var clon = new MeshData
            {
                Guid = src.Guid,
                ElementId = src.ElementId,
                MaterialId = src.MaterialId,
                CategoryId = src.CategoryId,
                ColR = src.ColR, ColG = src.ColG, ColB = src.ColB, ColA = src.ColA,
                HasColor = src.HasColor,
                Vertices = new List<Vector3>(src.Vertices),
                Indices = new List<int>(src.Indices)
            };
            ComputeNormals(clon);
            return clon;
        }

        static string BBoxStr(List<Vector3> verts)
        {
            GetBounds(verts, out var min, out var max);
            return $"min({min.X:F3}, {min.Y:F3}, {min.Z:F3}) max({max.X:F3}, {max.Y:F3}, {max.Z:F3})";
        }

        // ================== OPTIMIZACIÓN ==================

        static List<MeshData> OptimizeMeshes(List<MeshData> inputMeshes, PipelineStats stats,
                                             Dictionary<string, MeshData> weldedOriginals)
        {
            stats.InputMeshes = inputMeshes.Count;
            var grouped = inputMeshes.GroupBy(m => m.Guid);
            var optimizedList = new List<MeshData>();

            foreach (var group in grouped)
            {
                var first = group.First();
                var mergedMesh = new MeshData
                {
                    Guid = group.Key,
                    ElementId = first.ElementId,
                    MaterialId = first.MaterialId,
                    CategoryId = first.CategoryId,
                    ColR = first.ColR, ColG = first.ColG, ColB = first.ColB, ColA = first.ColA,
                    HasColor = first.HasColor
                };

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

                stats.VertsIn += mergedMesh.Vertices.Count;
                stats.TrisIn += mergedMesh.Indices.Count / 3;

                // 1. Soldar vértices duplicados (crucial para que QEM no rompa la malla)
                WeldVertices(mergedMesh);

                // 2. Eliminar triángulos degenerados que la soldadura pudo crear:
                //    alimentar QEM con caras de área cero es la causa típica de mallas rotas.
                stats.DegenerateRemoved += RemoveDegenerateTriangles(mergedMesh);
                CompactVertices(mergedMesh);
                stats.VertsWelded += mergedMesh.Vertices.Count;

                weldedOriginals[group.Key] = mergedMesh;

                if (mergedMesh.Indices.Count < 3)
                {
                    stats.Discarded++;
                    continue;
                }

                int originalTriangles = mergedMesh.Indices.Count / 3;

                // Categorías protegidas (muros/suelos/techos): NO se decimán nunca.
                // Ya vienen optimizadas de Revit; solo reciben soldadura y limpieza (sin pérdida).
                bool protegidaPorCategoria = CategoriasProtegidas.Contains(mergedMesh.CategoryId)
                                             || EsVidrioProtegido(mergedMesh);

                // Guarda anti-cubos: piezas chicas (cubos, prismas, detalles) NO se decimán.
                bool decimable = !protegidaPorCategoria &&
                                 mergedMesh.Vertices.Count >= MinVertsParaDecimar &&
                                 originalTriangles > MinTrisParaDecimar;

                if (protegidaPorCategoria) stats.ProtectedCat++;

                MeshData outMesh = mergedMesh;
                if (decimable)
                {
                    int targetTriangles = ComputeTarget(originalTriangles);
                    if (targetTriangles < originalTriangles)
                    {
                        // 3. Decimar con validación: si el resultado tiene spikes o queda vacío,
                        //    reintentar más conservador y, si aun así falla, conservar la malla soldada.
                        var decimated = TryDecimate(mergedMesh, targetTriangles, stats);
                        if (decimated == null && targetTriangles < originalTriangles / 2)
                        {
                            stats.Retries++;
                            decimated = TryDecimate(mergedMesh, originalTriangles / 2, stats);
                        }

                        if (decimated != null)
                        {
                            outMesh = decimated;
                        }
                        else
                        {
                            stats.Fallbacks++;
                        }
                    }
                }
                else if (!protegidaPorCategoria)
                {
                    stats.NotDecimated++;
                }

                if (outMesh.Indices.Count < 3)
                {
                    stats.Discarded++;
                    continue;
                }

                // outMesh comparte lista con mergedMesh cuando no se decimó: clonar para
                // que la validación final compare contra un original intacto.
                if (ReferenceEquals(outMesh, mergedMesh))
                {
                    outMesh = ClonarConNormales(mergedMesh);
                }
                else
                {
                    ComputeNormals(outMesh);
                }
                stats.VertsOut += outMesh.Vertices.Count;
                stats.TrisOut += outMesh.Indices.Count / 3;
                optimizedList.Add(outMesh);
            }

            stats.OutputPieces = optimizedList.Count;
            return optimizedList;
        }

        // Presupuesto CONSERVADOR: preferimos triángulos de más antes que piezas rotas.
        // Muros/suelos/techos ni siquiera llegan acá (categorías protegidas); esto aplica
        // a puertas, ventanas, mobiliario, columnas, sanitarios, etc.
        static int ComputeTarget(int originalTriangles)
        {
            if (originalTriangles > 10000) return (int)(originalTriangles * 0.25);
            if (originalTriangles > 2000) return (int)(originalTriangles * 0.35);
            if (originalTriangles > 500) return (int)(originalTriangles * 0.50);
            if (originalTriangles > 100) return (int)(originalTriangles * 0.65);
            return Math.Max((int)(originalTriangles * 0.80), 16);
        }

        // Devuelve la malla decimada y saneada, o null si el resultado no es confiable.
        static MeshData TryDecimate(MeshData source, int targetTriangles, PipelineStats stats)
        {
            var mdVertices = new MeshDecimator.Math.Vector3d[source.Vertices.Count];
            for (int i = 0; i < source.Vertices.Count; i++)
            {
                mdVertices[i] = new MeshDecimator.Math.Vector3d(
                    source.Vertices[i].X,
                    source.Vertices[i].Y,
                    source.Vertices[i].Z
                );
            }

            var srcMesh = new MeshDecimator.Mesh(mdVertices, source.Indices.ToArray());

            var algorithm = new MeshDecimator.Algorithms.FastQuadricMeshSimplification();
            algorithm.PreserveBorders = true;
            algorithm.PreserveSeams = true;
            algorithm.PreserveFoldovers = true; // Previene triángulos invertidos (sombras negras)
            algorithm.EnableSmartLink = false;  // No conectar muros interiores/exteriores (spikes)

            MeshDecimator.Mesh decimatedMdMesh;
            try
            {
                decimatedMdMesh = MeshDecimator.MeshDecimation.DecimateMesh(algorithm, srcMesh, targetTriangles);
            }
            catch
            {
                return null;
            }

            var outMesh = new MeshData
            {
                Guid = source.Guid,
                ElementId = source.ElementId,
                MaterialId = source.MaterialId,
                ColR = source.ColR, ColG = source.ColG, ColB = source.ColB, ColA = source.ColA,
                HasColor = source.HasColor
            };

            foreach (var v in decimatedMdMesh.Vertices)
            {
                if (double.IsNaN(v.x) || double.IsNaN(v.y) || double.IsNaN(v.z) ||
                    double.IsInfinity(v.x) || double.IsInfinity(v.y) || double.IsInfinity(v.z))
                {
                    return null; // el decimado produjo basura numérica
                }
                outMesh.Vertices.Add(new Vector3((float)v.x, (float)v.y, (float)v.z));
            }
            outMesh.Indices.AddRange(decimatedMdMesh.Indices);

            stats.DegenerateRemoved += RemoveDegenerateTriangles(outMesh);
            CompactVertices(outMesh);

            if (outMesh.Indices.Count < 3) return null;

            // Validación anti-spike: la malla decimada no puede salirse de la caja original.
            GetBounds(source.Vertices, out var srcMin, out var srcMax);
            GetBounds(outMesh.Vertices, out var outMin, out var outMax);
            float tolerance = (srcMax - srcMin).Length() * SpikeTolerance + 1e-4f;
            if (outMin.X < srcMin.X - tolerance || outMin.Y < srcMin.Y - tolerance || outMin.Z < srcMin.Z - tolerance ||
                outMax.X > srcMax.X + tolerance || outMax.Y > srcMax.Y + tolerance || outMax.Z > srcMax.Z + tolerance)
            {
                return null; // se generó un spike fuera del volumen original
            }

            // Validación de área (estricta): si la superficie cambió más de ±30%,
            // el decimado colapsó la malla, abrió agujeros o plegó triángulos.
            double srcArea = TotalArea(source);
            double outArea = TotalArea(outMesh);
            if (srcArea > 1e-10 && (outArea < srcArea * 0.7 || outArea > srcArea * 1.3))
            {
                return null;
            }

            return outMesh;
        }

        static double TotalArea(MeshData mesh)
        {
            double area = 0;
            for (int i = 0; i + 2 < mesh.Indices.Count; i += 3)
            {
                var a = mesh.Vertices[mesh.Indices[i]];
                var b = mesh.Vertices[mesh.Indices[i + 1]];
                var c = mesh.Vertices[mesh.Indices[i + 2]];
                area += Vector3.Cross(b - a, c - a).Length() * 0.5;
            }
            return area;
        }

        static void GetBounds(List<Vector3> verts, out Vector3 min, out Vector3 max)
        {
            min = new Vector3(float.MaxValue);
            max = new Vector3(float.MinValue);
            foreach (var v in verts)
            {
                min = Vector3.Min(min, v);
                max = Vector3.Max(max, v);
            }
        }

        static void WeldVertices(MeshData mesh)
        {
            var uniqueVertices = new List<Vector3>();
            // Clave por grilla de 1 mm con enteros (sin strings: más rápido y sin errores de formato)
            var vertexMap = new Dictionary<(long, long, long), int>(mesh.Vertices.Count);
            var newIndices = new List<int>(mesh.Indices.Count);

            foreach (var index in mesh.Indices)
            {
                if (index < 0 || index >= mesh.Vertices.Count) continue; // índice corrupto en el export
                var v = mesh.Vertices[index];
                var key = ((long)Math.Round(v.X * WeldGrid),
                           (long)Math.Round(v.Y * WeldGrid),
                           (long)Math.Round(v.Z * WeldGrid));

                if (!vertexMap.TryGetValue(key, out int newIndex))
                {
                    newIndex = uniqueVertices.Count;
                    uniqueVertices.Add(v);
                    vertexMap[key] = newIndex;
                }
                newIndices.Add(newIndex);
            }

            // Índices corruptos pueden dejar triángulos incompletos: recortar a múltiplo de 3
            int usable = newIndices.Count - (newIndices.Count % 3);
            if (usable != newIndices.Count) newIndices.RemoveRange(usable, newIndices.Count - usable);

            mesh.Vertices = uniqueVertices;
            mesh.Indices = newIndices;
        }

        // Elimina triángulos con índices repetidos o área nula. Devuelve cuántos se quitaron.
        static long RemoveDegenerateTriangles(MeshData mesh)
        {
            var newIndices = new List<int>(mesh.Indices.Count);
            long removed = 0;

            for (int i = 0; i + 2 < mesh.Indices.Count; i += 3)
            {
                int a = mesh.Indices[i], b = mesh.Indices[i + 1], c = mesh.Indices[i + 2];
                if (a == b || b == c || a == c) { removed++; continue; }

                var cross = Vector3.Cross(mesh.Vertices[b] - mesh.Vertices[a],
                                          mesh.Vertices[c] - mesh.Vertices[a]);
                if (cross.LengthSquared() < 1e-14f) { removed++; continue; }

                newIndices.Add(a);
                newIndices.Add(b);
                newIndices.Add(c);
            }

            mesh.Indices = newIndices;
            return removed;
        }

        // Quita vértices que ningún triángulo referencia y reindexa.
        static void CompactVertices(MeshData mesh)
        {
            var remap = new int[mesh.Vertices.Count];
            for (int i = 0; i < remap.Length; i++) remap[i] = -1;

            var newVertices = new List<Vector3>(mesh.Vertices.Count);
            for (int i = 0; i < mesh.Indices.Count; i++)
            {
                int old = mesh.Indices[i];
                if (remap[old] < 0)
                {
                    remap[old] = newVertices.Count;
                    newVertices.Add(mesh.Vertices[old]);
                }
                mesh.Indices[i] = remap[old];
            }

            mesh.Vertices = newVertices;
        }

        // Normales suavizadas ponderadas por área (el cross sin normalizar pesa por área:
        // caras grandes dominan y el sombreado queda más estable en muros).
        static void ComputeNormals(MeshData mesh)
        {
            mesh.Normals = new List<Vector3>(new Vector3[mesh.Vertices.Count]);

            for (int i = 0; i + 2 < mesh.Indices.Count; i += 3)
            {
                int i0 = mesh.Indices[i];
                int i1 = mesh.Indices[i + 1];
                int i2 = mesh.Indices[i + 2];

                Vector3 faceNormal = Vector3.Cross(mesh.Vertices[i1] - mesh.Vertices[i0],
                                                   mesh.Vertices[i2] - mesh.Vertices[i0]);
                mesh.Normals[i0] += faceNormal;
                mesh.Normals[i1] += faceNormal;
                mesh.Normals[i2] += faceNormal;
            }

            for (int i = 0; i < mesh.Normals.Count; i++)
            {
                float len = mesh.Normals[i].Length();
                mesh.Normals[i] = len > 1e-10f ? mesh.Normals[i] / len : Vector3.UnitY;
            }
        }

        // ================== LECTURA ==================
        // v1 (legado): [elementId][guid][materialId][verts][tris]...
        // v2 ("TBT2"): cabecera [magic][version]; cada bloque agrega color RGBA del material.
        // v3: además agrega el id de BuiltInCategory (int32) después del MaterialId.
        static List<MeshData> LeerBinario(string path, PipelineStats stats)
        {
            var meshes = new List<MeshData>();

            using (var fs = File.OpenRead(path))
            using (var reader = new BinaryReader(fs))
            {
                bool v2 = false, v3 = false;
                if (fs.Length >= 8)
                {
                    int magic = reader.ReadInt32();
                    if (magic == FormatMagic)
                    {
                        int version = reader.ReadInt32();
                        v2 = version >= 2;
                        v3 = version >= 3;
                        stats.FormatVersion = version;
                    }
                    else
                    {
                        fs.Position = 0; // formato legado sin cabecera
                    }
                }

                while (fs.Position < fs.Length)
                {
                    var mesh = new MeshData();
                    mesh.ElementId = reader.ReadInt32();
                    mesh.Guid = reader.ReadString();
                    mesh.MaterialId = reader.ReadInt32();

                    if (v3)
                    {
                        mesh.CategoryId = reader.ReadInt32();
                    }

                    if (v2)
                    {
                        mesh.ColR = reader.ReadByte();
                        mesh.ColG = reader.ReadByte();
                        mesh.ColB = reader.ReadByte();
                        mesh.ColA = reader.ReadByte();
                        mesh.HasColor = true;
                    }

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

        // ================== EXPORTACIÓN ==================

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
                    writer.WriteLine($"usemtl MAT_{mesh.MaterialId}");

                    foreach (var v in mesh.Vertices)
                    {
                        string x = v.X.ToString(System.Globalization.CultureInfo.InvariantCulture);
                        string y = v.Y.ToString(System.Globalization.CultureInfo.InvariantCulture);
                        string z = v.Z.ToString(System.Globalization.CultureInfo.InvariantCulture);
                        writer.WriteLine($"v {x} {y} {z}");
                    }

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

        static void ExportToGlb(List<MeshData> meshes, string outputPath, PipelineStats stats)
        {
            var scene = new SceneBuilder();

            // Un material compartido por MaterialId: menos draw calls (batching en Unity)
            var materials = new Dictionary<int, MaterialBuilder>();
            var random = new Random(42);

            // Cache de geometría para instanciado de piezas repetidas
            var meshCache = new Dictionary<(int, int, int, long),
                List<(MeshData, MeshBuilder<VertexPositionNormal, VertexEmpty, VertexEmpty>)>>();

            foreach (var meshData in meshes)
            {
                if (!materials.TryGetValue(meshData.MaterialId, out var material))
                {
                    float r, g, b, a;
                    if (meshData.HasColor)
                    {
                        // Color superficial simple del material de Revit; alpha < 1 = vidrio
                        r = meshData.ColR / 255f;
                        g = meshData.ColG / 255f;
                        b = meshData.ColB / 255f;
                        a = meshData.ColA / 255f;
                    }
                    else
                    {
                        // Formato legado: color aleatorio determinístico por material
                        r = (float)random.NextDouble() * 0.7f + 0.3f;
                        g = (float)random.NextDouble() * 0.7f + 0.3f;
                        b = (float)random.NextDouble() * 0.7f + 0.3f;
                        a = 1.0f;
                    }

                    material = new MaterialBuilder($"MAT_{meshData.MaterialId}")
                        .WithDoubleSide(true)
                        .WithMetallicRoughnessShader()
                        .WithChannelParam(KnownChannel.BaseColor, new Vector4(r, g, b, a))
                        .WithChannelParam(KnownChannel.MetallicRoughness, new Vector4(0.0f, 0.9f, 0, 0));

                    if (a < 0.995f)
                    {
                        material.WithAlpha(AlphaMode.BLEND);
                        stats.Transparent++;
                    }

                    materials[meshData.MaterialId] = material;
                }

                // Instanciado: si esta pieza es una copia trasladada de otra ya exportada
                // (ventanas/puertas/muebles repetidos), reusar su malla con un nodo movido.
                // Menos bytes de descarga y menos RAM de mallas en el móvil.
                Matrix4x4 transform = Matrix4x4.Identity;
                MeshBuilder<VertexPositionNormal, VertexEmpty, VertexEmpty> meshBuilder = null;

                var geoKey = (meshData.MaterialId, meshData.Vertices.Count, meshData.Indices.Count, GeometryHash(meshData));
                if (!meshCache.TryGetValue(geoKey, out var candidates))
                {
                    candidates = new List<(MeshData, MeshBuilder<VertexPositionNormal, VertexEmpty, VertexEmpty>)>();
                    meshCache[geoKey] = candidates;
                }

                foreach (var (refMesh, refBuilder) in candidates)
                {
                    if (EsCopiaTrasladada(refMesh, meshData, out var delta))
                    {
                        meshBuilder = refBuilder;
                        transform = Matrix4x4.CreateTranslation(delta);
                        stats.Instanced++;
                        break;
                    }
                }

                if (meshBuilder == null)
                {
                    // El nombre de la malla es el GUID de la primera pieza que la usa
                    meshBuilder = new MeshBuilder<VertexPositionNormal, VertexEmpty, VertexEmpty>(meshData.Guid);
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
                    candidates.Add((meshData, meshBuilder));
                }

                // El nodo (GameObject en Unity) lleva el GUID del elemento de Revit
                scene.AddRigidMesh(meshBuilder, transform).WithName(meshData.Guid);
            }

            var model = scene.ToGltf2();

            // Extras por nodo: cada pieza lleva su ElementId, CategoryId y MaterialId de Revit
            // además del GUID (nombre del nodo) y el color/alpha (material).
            var porGuid = new Dictionary<string, MeshData>();
            foreach (var m in meshes)
            {
                if (!porGuid.ContainsKey(m.Guid)) porGuid[m.Guid] = m;
            }
            foreach (var node in model.LogicalNodes)
            {
                if (node.Name != null && porGuid.TryGetValue(node.Name, out var md))
                {
                    node.Extras = System.Text.Json.Nodes.JsonNode.Parse(
                        $"{{\"elementId\":{md.ElementId},\"categoryId\":{md.CategoryId},\"materialId\":{md.MaterialId}}}");
                }
            }

            model.SaveGLB(outputPath);
        }

        // Hash rápido de la forma: posiciones relativas al primer vértice (grilla 0,05 mm) + índices.
        static long GeometryHash(MeshData mesh)
        {
            const double q = 20000.0;
            var v0 = mesh.Vertices[0];
            long hash = 1469598103934665603; // FNV-1a
            unchecked
            {
                foreach (var v in mesh.Vertices)
                {
                    hash = (hash ^ (long)Math.Round((v.X - v0.X) * q)) * 1099511628211;
                    hash = (hash ^ (long)Math.Round((v.Y - v0.Y) * q)) * 1099511628211;
                    hash = (hash ^ (long)Math.Round((v.Z - v0.Z) * q)) * 1099511628211;
                }
                foreach (var i in mesh.Indices)
                {
                    hash = (hash ^ i) * 1099511628211;
                }
            }
            return hash;
        }

        // Verificación exacta (el hash solo preselecciona): misma topología y mismas
        // posiciones relativas dentro de 0,1 mm. Devuelve la traslación entre ambas.
        static bool EsCopiaTrasladada(MeshData a, MeshData b, out Vector3 delta)
        {
            delta = default;
            if (a.Vertices.Count != b.Vertices.Count || a.Indices.Count != b.Indices.Count) return false;

            for (int i = 0; i < a.Indices.Count; i++)
            {
                if (a.Indices[i] != b.Indices[i]) return false;
            }

            delta = b.Vertices[0] - a.Vertices[0];
            const float eps = 1e-4f;
            for (int i = 0; i < a.Vertices.Count; i++)
            {
                var d = b.Vertices[i] - (a.Vertices[i] + delta);
                if (Math.Abs(d.X) > eps || Math.Abs(d.Y) > eps || Math.Abs(d.Z) > eps) return false;
            }
            return true;
        }

        // ================== BINARIO DE VISOR (.tbv) ==================
        // Formato "TBTV" ultra-optimizado para el visor web:
        //   - Deduplicación: la geometría repetida (ventanas, muebles, columnas iguales) se
        //     escribe UNA vez en un pool; cada pieza es una instancia que apunta al pool con
        //     una traslación. Enorme ahorro de bytes y de RAM/GPU en el visor.
        //   - Índice espacial: cada instancia guarda su AABB GLOBAL precalculado, para que el
        //     visor pueda cull-ear por "cubo visible" leyendo solo esos 6 floats (sin tocar
        //     la geometría) y para búsquedas por posición global.
        //   - Búsqueda por ID/GUID: cada instancia trae ElementId, CategoryId, MaterialId y GUID.
        //   - Compacto: normales en int8 (snorm), índices uint16 cuando se puede.
        //
        // Layout (little-endian):
        //   Header: int32 magic('TBTV'=0x56544254), int32 version(1),
        //           int32 matCount, int32 meshCount, int32 instCount,
        //           float32 sceneMin[3], float32 sceneMax[3]
        //   Materiales × matCount: int32 materialId, uint8 R,G,B,A
        //   Meshes (pool) × meshCount:
        //           int32 vertexCount, uint8 idx16(1/0), int32 triCount,
        //           float32 positions[vc*3], int8 normals[vc*3],
        //           (idx16? uint16 : uint32) indices[triCount*3]
        //   Instancias × instCount:
        //           int32 meshIndex, int32 elementId, int32 categoryId, int32 materialIndex,
        //           float32 tx,ty,tz, float32 gmin[3], float32 gmax[3],
        //           uint16 guidLen, byte[guidLen] guid(UTF8)
        static void ExportToViewerBin(List<MeshData> meshes, string path, PipelineStats stats)
        {
            const int MAGIC = 0x56544254; // "TBTV"
            const int VERSION = 1;

            // Tabla de materiales: un color por MaterialId
            var matIndexById = new Dictionary<int, int>();
            var matReps = new List<MeshData>();
            foreach (var m in meshes)
            {
                if (!matIndexById.ContainsKey(m.MaterialId))
                {
                    matIndexById[m.MaterialId] = matReps.Count;
                    matReps.Add(m);
                }
            }

            // Pool de geometría deduplicada + instancias (misma lógica que el instanciado del GLB)
            var pool = new List<MeshData>();
            var poolBounds = new List<(Vector3 min, Vector3 max)>();
            var poolKey = new Dictionary<(int, int, int, long), List<int>>();
            var instMeshIdx = new List<int>(meshes.Count);
            var instDelta = new List<Vector3>(meshes.Count);

            foreach (var m in meshes)
            {
                var key = (m.MaterialId, m.Vertices.Count, m.Indices.Count, GeometryHash(m));
                if (!poolKey.TryGetValue(key, out var cand))
                {
                    cand = new List<int>();
                    poolKey[key] = cand;
                }

                int found = -1;
                Vector3 delta = Vector3.Zero;
                foreach (var pi in cand)
                {
                    if (EsCopiaTrasladada(pool[pi], m, out delta)) { found = pi; break; }
                }

                if (found < 0)
                {
                    found = pool.Count;
                    pool.Add(m);
                    GetBounds(m.Vertices, out var mn, out var mx);
                    poolBounds.Add((mn, mx));
                    cand.Add(found);
                    delta = Vector3.Zero;
                }

                instMeshIdx.Add(found);
                instDelta.Add(delta);
            }

            stats.PoolMeshes = pool.Count;

            // AABB de toda la escena (para encuadre inicial del visor)
            Vector3 sMin = new Vector3(float.MaxValue), sMax = new Vector3(float.MinValue);
            for (int i = 0; i < meshes.Count; i++)
            {
                var (mn, mx) = poolBounds[instMeshIdx[i]];
                var d = instDelta[i];
                sMin = Vector3.Min(sMin, mn + d);
                sMax = Vector3.Max(sMax, mx + d);
            }

            var rndColors = new Random(42);
            using (var fs = File.Create(path))
            using (var w = new BinaryWriter(fs))
            {
                w.Write(MAGIC);
                w.Write(VERSION);
                w.Write(matReps.Count);
                w.Write(pool.Count);
                w.Write(meshes.Count);
                w.Write(sMin.X); w.Write(sMin.Y); w.Write(sMin.Z);
                w.Write(sMax.X); w.Write(sMax.Y); w.Write(sMax.Z);

                // Materiales
                foreach (var mr in matReps)
                {
                    w.Write(mr.MaterialId);
                    if (mr.HasColor)
                    {
                        w.Write(mr.ColR); w.Write(mr.ColG); w.Write(mr.ColB); w.Write(mr.ColA);
                    }
                    else
                    {
                        byte r = (byte)(rndColors.Next(0, 180) + 60);
                        byte g = (byte)(rndColors.Next(0, 180) + 60);
                        byte b = (byte)(rndColors.Next(0, 180) + 60);
                        w.Write(r); w.Write(g); w.Write(b); w.Write((byte)255);
                    }
                }

                // Meshes del pool
                foreach (var pm in pool)
                {
                    int vc = pm.Vertices.Count;
                    int tc = pm.Indices.Count / 3;
                    bool idx16 = vc <= 65535;

                    w.Write(vc);
                    w.Write((byte)(idx16 ? 1 : 0));
                    w.Write(tc);

                    for (int i = 0; i < vc; i++)
                    {
                        var v = pm.Vertices[i];
                        w.Write(v.X); w.Write(v.Y); w.Write(v.Z);
                    }
                    for (int i = 0; i < vc; i++)
                    {
                        var n = i < pm.Normals.Count ? pm.Normals[i] : Vector3.UnitY;
                        w.Write(Snorm(n.X)); w.Write(Snorm(n.Y)); w.Write(Snorm(n.Z));
                    }
                    if (idx16)
                        foreach (var ix in pm.Indices) w.Write((ushort)ix);
                    else
                        foreach (var ix in pm.Indices) w.Write((uint)ix);
                }

                // Instancias
                for (int i = 0; i < meshes.Count; i++)
                {
                    var m = meshes[i];
                    var (mn, mx) = poolBounds[instMeshIdx[i]];
                    var d = instDelta[i];
                    var gmin = mn + d;
                    var gmax = mx + d;

                    w.Write(instMeshIdx[i]);
                    w.Write(m.ElementId);
                    w.Write(m.CategoryId);
                    w.Write(matIndexById[m.MaterialId]);
                    w.Write(d.X); w.Write(d.Y); w.Write(d.Z);
                    w.Write(gmin.X); w.Write(gmin.Y); w.Write(gmin.Z);
                    w.Write(gmax.X); w.Write(gmax.Y); w.Write(gmax.Z);

                    var gb = Encoding.UTF8.GetBytes(m.Guid ?? "");
                    w.Write((ushort)gb.Length);
                    w.Write(gb);
                }
            }

            stats.TbvBytes = new FileInfo(path).Length;
        }

        static sbyte Snorm(float f)
        {
            f = Math.Max(-1f, Math.Min(1f, f));
            return (sbyte)Math.Round(f * 127f);
        }
    }
}
