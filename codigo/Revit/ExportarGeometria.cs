using System;
using System.Collections.Generic;
using System.IO;
using Autodesk.Revit.DB;

// Exportador para Revit 2021 — formato binario v3 ("TBT2" + versión 3).
// Reemplaza a la versión anterior manteniendo la misma firma de ExportRawDump.
// Novedades:
//   - Cabecera [magic "TBT2"][version] para que el conversor detecte el formato
//     (los .bin viejos sin cabecera se siguen leyendo como v1 automáticamente).
//   - Id de BuiltInCategory del elemento: el conversor lo usa para NO decimar
//     categorías protegidas (muros, suelos, techos).
//   - Color superficial simple del material (RGB de Material.Color) + alpha derivado
//     de Material.Transparency: los vidrios de ventanas salen translúcidos.
//   - Cache de colores por material, fallback al material de la categoría y gris neutro.
//   - try/catch por elemento: un elemento corrupto no aborta el export completo.
public static class ExportarGeometria
{
    private const int FORMAT_MAGIC = 0x32544254; // "TBT2" en little-endian
    private const int FORMAT_VERSION = 3;

    public static void ExportRawDump(Document doc, string filePath = @"C:\.TBT\Proyectos\_Revit_EXE_Geometrias\export.bin")
    {
        var categories = new List<BuiltInCategory>
        {
            BuiltInCategory.OST_Walls,
            BuiltInCategory.OST_Floors,
            BuiltInCategory.OST_Roofs,
            BuiltInCategory.OST_Doors,
            BuiltInCategory.OST_Windows,
            BuiltInCategory.OST_StructuralColumns,
            BuiltInCategory.OST_StructuralFraming,
            BuiltInCategory.OST_Furniture,
            BuiltInCategory.OST_PlumbingFixtures
        };
        var catFilter = new ElementMulticategoryFilter(categories);
        var geomOptions = new Options { DetailLevel = ViewDetailLevel.Fine };

        var collector = new FilteredElementCollector(doc)
            .WherePasses(catFilter)
            .WhereElementIsNotElementType();

        string directoryPath = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(directoryPath) && !Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        // Cache de color por material: evita resolver el mismo material en cada cara
        var colorCache = new Dictionary<int, byte[]>();

        using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, 4096 * 10))
        using (var writer = new BinaryWriter(fs))
        {
            writer.Write(FORMAT_MAGIC);
            writer.Write(FORMAT_VERSION);

            foreach (Element elem in collector)
            {
                try
                {
                    ExportarElemento(doc, elem, geomOptions, writer, colorCache);
                }
                catch
                {
                    // Un elemento con geometría corrupta no debe abortar el export completo
                }
            }
        }
    }

    private static void ExportarElemento(Document doc, Element elem, Options geomOptions,
                                         BinaryWriter writer, Dictionary<int, byte[]> colorCache)
    {
        GeometryElement geomElem = elem.get_Geometry(geomOptions);
        if (geomElem == null) return;

        int elementId = elem.Id.IntegerValue;
        string guid = elem.UniqueId;
        // Id de BuiltInCategory (negativo, p.ej. Muros = -2000011); 0 si no tiene categoría
        int categoryId = elem.Category != null ? elem.Category.Id.IntegerValue : 0;

        // Extraer TODOS los sólidos de forma recursiva (anidamiento ilimitado)
        List<Solid> solids = ObtenerTodosLosSolidos(geomElem);

        foreach (Solid solid in solids)
        {
            foreach (Face face in solid.Faces)
            {
                Mesh mesh = face.Triangulate();
                if (mesh == null || mesh.NumTriangles == 0 || mesh.Vertices.Count == 0) continue;

                int materialId = face.MaterialElementId != null ? face.MaterialElementId.IntegerValue : -1;
                byte[] rgba = ObtenerColorMaterial(doc, elem, face.MaterialElementId, colorCache);

                writer.Write(elementId);
                writer.Write(guid);
                writer.Write(materialId);
                writer.Write(categoryId); // v3: categoría (el conversor protege muros/suelos/techos)
                writer.Write(rgba[0]); // R
                writer.Write(rgba[1]); // G
                writer.Write(rgba[2]); // B
                writer.Write(rgba[3]); // A (vidrios < 255 según transparencia del material)

                int vertexCount = mesh.Vertices.Count;
                writer.Write(vertexCount);
                foreach (XYZ v in mesh.Vertices)
                {
                    writer.Write((float)v.X);
                    writer.Write((float)v.Y);
                    writer.Write((float)v.Z);
                }

                int triangleCount = mesh.NumTriangles;
                writer.Write(triangleCount);
                for (int i = 0; i < triangleCount; i++)
                {
                    MeshTriangle tri = mesh.get_Triangle(i);
                    writer.Write(tri.get_Index(0));
                    writer.Write(tri.get_Index(1));
                    writer.Write(tri.get_Index(2));
                }
            }
        }
    }

    /// <summary>
    /// Color superficial simple + alpha del material (API de Revit 2021).
    /// RGB sale de Material.Color; el alpha se deriva de Material.Transparency (0-100),
    /// con lo cual los vidrios de ventanas quedan translúcidos en el GLB.
    /// Fallbacks: material de la categoría del elemento -> gris neutro opaco.
    /// </summary>
    private static byte[] ObtenerColorMaterial(Document doc, Element elem, ElementId materialElementId,
                                               Dictionary<int, byte[]> cache)
    {
        int key = materialElementId != null ? materialElementId.IntegerValue : -1;
        byte[] cached;
        if (key >= 0 && cache.TryGetValue(key, out cached)) return cached;

        Material mat = null;
        if (key >= 0) mat = doc.GetElement(materialElementId) as Material;

        bool esDelMaterial = mat != null;
        if (mat == null && elem.Category != null) mat = elem.Category.Material;

        byte r = 190, g = 190, b = 190, a = 255; // gris neutro si no hay material
        if (mat != null)
        {
            Color col = mat.Color;
            if (col != null && col.IsValid)
            {
                r = col.Red;
                g = col.Green;
                b = col.Blue;
            }

            // 1) Transparencia de la pestaña Gráficos
            int t = mat.Transparency; // 0 (opaco) .. 100 (invisible)

            // 2) Muchos vidrios tienen la transparencia SOLO en la pestaña Apariencia
            //    (render) y Gráficos en 0: leer el Appearance Asset como respaldo.
            if (t <= 0) t = TransparenciaDesdeApariencia(doc, mat);

            // 3) Red final: materiales que por nombre/clase son vidrio pero no declaran
            //    transparencia en ningún lado.
            bool esVidrio = EsNombreDeVidrio(mat);
            if (t <= 0 && esVidrio) t = 60;

            // Un vidrio con color gráfico negro (sin definir) se vería como ventana negra:
            // usar un tinte de vidrio neutro.
            if (esVidrio && (r + g + b) < 90) { r = 170; g = 200; b = 210; }

            if (t > 0)
            {
                int alpha = (int)Math.Round(255.0 * (100 - t) / 100.0);
                if (alpha < 20) alpha = 20;   // piso: que el vidrio nunca desaparezca del todo
                if (alpha > 255) alpha = 255;
                a = (byte)alpha;
            }
        }

        byte[] rgba = new byte[] { r, g, b, a };
        // Solo cachear cuando el color salió del material real (el fallback por
        // categoría varía según el elemento y no debe quedar pegado a un MaterialId).
        if (key >= 0 && esDelMaterial) cache[key] = rgba;
        return rgba;
    }

    /// <summary>
    /// Transparencia (0-100) leída del Appearance Asset (pestaña Apariencia) del material.
    /// Cubre el caso típico: vidrios con Gráficos opaco pero apariencia de render translúcida.
    /// </summary>
    private static int TransparenciaDesdeApariencia(Document doc, Material mat)
    {
        try
        {
            if (mat.AppearanceAssetId == null || mat.AppearanceAssetId == ElementId.InvalidElementId) return 0;
            AppearanceAssetElement aae = doc.GetElement(mat.AppearanceAssetId) as AppearanceAssetElement;
            if (aae == null) return 0;
            Asset asset = aae.GetRenderingAsset();
            if (asset == null) return 0;

            // Esquema "Generic": transparencia directa 0..1
            AssetPropertyDouble p = asset.FindByName("generic_transparency") as AssetPropertyDouble;
            if (p != null && p.Value > 0.01) return (int)Math.Round(p.Value * 100);

            // Esquemas de vidrio (Glazing / SolidGlass): son vidrio por definición
            AssetPropertyString schema = asset.FindByName("BaseSchema") as AssetPropertyString;
            if (schema != null && (schema.Value == "GlazingSchema" || schema.Value == "SolidGlassSchema"))
            {
                return 70;
            }
        }
        catch
        {
            // Assets corruptos o de esquemas exóticos: seguir con las otras heurísticas
        }
        return 0;
    }

    private static bool EsNombreDeVidrio(Material mat)
    {
        string s = ((mat.Name ?? "") + "|" + (mat.MaterialClass ?? "")).ToLowerInvariant();
        return s.Contains("vidrio") || s.Contains("glass") || s.Contains("cristal") ||
               s.Contains("glazing") || s.Contains("transparen");
    }

    /// <summary>
    /// Navega recursivamente un GeometryElement y devuelve todos los sólidos válidos.
    /// </summary>
    private static List<Solid> ObtenerTodosLosSolidos(GeometryElement geomElem)
    {
        List<Solid> solidos = new List<Solid>();
        if (geomElem == null) return solidos;

        foreach (GeometryObject obj in geomElem)
        {
            if (obj is Solid)
            {
                Solid solido = obj as Solid;
                if (solido != null && solido.Faces.Size > 0 && solido.Volume > 0)
                {
                    solidos.Add(solido);
                }
            }
            else if (obj is GeometryInstance)
            {
                GeometryInstance instancia = obj as GeometryInstance;
                GeometryElement geometriaInterna = instancia.GetInstanceGeometry();
                // Llamada recursiva para resolver instancias anidadas
                solidos.AddRange(ObtenerTodosLosSolidos(geometriaInterna));
            }
        }
        return solidos;
    }
}
