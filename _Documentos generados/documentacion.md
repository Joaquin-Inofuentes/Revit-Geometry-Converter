# Documentación de _Revit_EXE_Geometrias

Generado: 2026-07-28 19:26:08

## Resumen

- Archivos: 78
- Carpetas: 26
- Namespaces: 14
- Clases: 93
- Interfaces: 2
- Enums: 2
- Métodos: 790
- Propiedades: 167
- Campos: 537
- Líneas totales: 28516 (código 18367, comentarios 7497, blancos 2652)

## Mapa de relaciones

- **Logging** usa **ILogger**
- **Logging** usa **ConsoleLogger**
- **DecimationAlgorithm** usa **MathHelper**
- **DecimationAlgorithm** usa **Mesh**
- **UVChannels** usa **Mesh**
- **UVChannels** usa **ResizableArray**
- **ConsoleLogger** implementa **ILogger**
- **LODBackupComponent** hereda de **MonoBehaviour**
- **MeshUtils** usa **Mesh**
- **MeshUtils** usa **Vector3**
- **MeshUtils** usa **Vector4**
- **MeshUtils** usa **BoneWeight**
- **MeshUtils** usa **Vector2**
- **MeshUtils** usa **BlendShape**
- **MeshUtils** usa **BlendShapeFrame**
- **MeshDecimatorUtility** usa **Logging**
- **MeshDecimatorUtility** usa **ConsoleLogger**
- **MeshDecimatorUtility** usa **UnityLogger**
- **MeshDecimatorUtility** usa **Vector3d**
- **MeshDecimatorUtility** usa **Vector2**
- **MeshDecimatorUtility** usa **Vector3**
- **MeshDecimatorUtility** usa **Vector4**
- **MeshDecimatorUtility** usa **BoneWeight**
- **MeshDecimatorUtility** usa **Mesh**
- **MeshDecimatorUtility** usa **DecimationAlgorithm**
- **MeshDecimatorUtility** usa **MeshDecimation**
- **MeshDecimatorUtility** usa **Algorithm**
- **LODGenerator** usa **Mesh**
- **LODGenerator** usa **DecimationAlgorithm**
- **LODGenerator** usa **MeshDecimatorUtility**
- **LODGenerator** usa **LODSettings**
- **LODGenerator** usa **Vector3**
- **DecimatedObject** hereda de **MonoBehaviour**
- **DecimatedObject** usa **LODSettings**
- **DecimatedObject** usa **LODGenerator**
- **Vector3** implementa **IEquatable**
- **Vector3** usa **Vector3d**
- **Vector3** usa **Vector3i**
- **Vector3** usa **MathHelper**
- **Vector2i** implementa **IEquatable**
- **Vector2i** usa **Vector2**
- **Vector2i** usa **Vector2d**
- **Vector2d** implementa **IEquatable**
- **Vector2d** usa **Vector2**
- **Vector2d** usa **Vector2i**
- **Vector2** implementa **IEquatable**
- **Vector2** usa **Vector2d**
- **Vector2** usa **Vector2i**
- **ShowExample** hereda de **MonoBehaviour**
- **ShowExample** usa **Vector3**
- **Triangle** implementa **IEquatable**
- **Triangle** usa **Vector3d**
- **BorderVertexComparer** implementa **IComparer**
- **BorderVertexComparer** usa **BorderVertex**
- **BlendShapeFrameContainer** usa **Vector3**
- **BlendShapeFrameContainer** usa **BlendShapeFrame**
- **BlendShapeFrameContainer** usa **ResizableArray**
- **UnityLogger** implementa **ILogger**
- **BlendShapeContainer** usa **BlendShapeFrameContainer**
- **BlendShapeContainer** usa **BlendShape**
- **BlendShapeContainer** usa **Vector3**
- **BlendShapeContainer** usa **BlendShapeFrame**
- **Vector3d** implementa **IEquatable**
- **Vector3d** usa **Vector3**
- **Vector3d** usa **MathHelper**
- **MathHelper** usa **Vector3d**
- **LODGeneratorHelperEditor** hereda de **Editor**
- **LODGeneratorHelperEditor** usa **LODGeneratorHelper**
- **LODGeneratorHelperEditor** usa **IOUtils**
- **LODGeneratorHelperEditor** usa **LODGenerator**
- **LODGeneratorHelper** hereda de **MonoBehaviour**
- **LODGeneratorHelper** usa **SimplificationOptions**
- **LODGeneratorHelper** usa **LODLevel**
- **UVChannels** usa **Mesh**
- **UVChannels** usa **ResizableArray**
- **ConsoleLogger** implementa **ILogger**
- **Vector4i** implementa **IEquatable**
- **Vector4i** usa **Vector4**
- **Vector4i** usa **Vector4d**
- **Vector3d** implementa **IEquatable**
- **Vector3d** usa **Vector3**
- **Vector3d** usa **Vector3i**
- **Vector3d** usa **MathHelper**
- **ValidateSimplificationOptionsException** hereda de **Exception**
- **Vertex** implementa **IEquatable**
- **Vertex** usa **Vector3d**
- **Vertex** usa **SymmetricMatrix**
- **UVChannels** usa **MeshUtils**
- **UVChannels** usa **ResizableArray**
- **Vector2** implementa **IEquatable**
- **Vector2** usa **Vector2d**
- **Vector2** usa **Vector2i**
- **MathHelper** usa **Vector3**
- **MathHelper** usa **Vector3d**
- **Program** usa **MathHelper**
- **Program** usa **ObjMesh**
- **Program** usa **Mesh**
- **Program** usa **MeshDecimation**
- **Program** usa **Algorithm**
- **ObjMesh** usa **FaceIndex**
- **ObjMesh** usa **Vector3d**
- **ObjMesh** usa **Vector3**
- **ObjMesh** usa **Vector2**
- **FaceIndex** implementa **IEquatable**
- **DecimatedObjectEditor** hereda de **Editor**
- **DecimatedObjectEditor** usa **DecimatedObject**
- **BoneWeight** implementa **IEquatable**
- **BoneWeight** usa **Vector4**
- **MeshData** usa **Vector3**
- **Program** usa **MeshData**
- **Program** usa **PipelineStats**
- **Program** usa **PiezaRota**
- **Program** usa **Vector3**
- **Program** usa **Vector3d**
- **Program** usa **Mesh**
- **Program** usa **FastQuadricMeshSimplification**
- **Program** usa **MeshDecimation**
- **Program** usa **Vector4**
- **FastQuadricMeshSimplification** hereda de **DecimationAlgorithm**
- **FastQuadricMeshSimplification** usa **Vector3d**
- **FastQuadricMeshSimplification** usa **SymmetricMatrix**
- **FastQuadricMeshSimplification** usa **BorderVertex**
- **FastQuadricMeshSimplification** usa **BorderVertexComparer**
- **FastQuadricMeshSimplification** usa **Triangle**
- **FastQuadricMeshSimplification** usa **Vertex**
- **FastQuadricMeshSimplification** usa **Ref**
- **FastQuadricMeshSimplification** usa **Vector3**
- **FastQuadricMeshSimplification** usa **Vector4**
- **FastQuadricMeshSimplification** usa **Vector2**
- **FastQuadricMeshSimplification** usa **BoneWeight**
- **FastQuadricMeshSimplification** usa **Logging**
- **FastQuadricMeshSimplification** usa **MathHelper**
- **FastQuadricMeshSimplification** usa **Mesh**
- **FastQuadricMeshSimplification** usa **ResizableArray**
- **FastQuadricMeshSimplification** usa **UVChannels**
- **Triangle** usa **Vector3d**
- **Vertex** usa **Vector3d**
- **Vertex** usa **SymmetricMatrix**
- **BorderVertexComparer** implementa **IComparer**
- **BorderVertexComparer** usa **BorderVertex**
- **Vector4d** implementa **IEquatable**
- **Vector4d** usa **Vector4**
- **Vector4d** usa **Vector4i**
- **Vector4** implementa **IEquatable**
- **Vector4** usa **Vector4d**
- **Vector4** usa **Vector4i**
- **Vector3i** implementa **IEquatable**
- **Vector3i** usa **Vector3**
- **Vector3i** usa **Vector3d**
- **Vector3d** implementa **IEquatable**
- **Vector3d** usa **Vector3**
- **Vector3d** usa **Vector3i**
- **Vector3d** usa **MathHelper**
- **Vector4i** implementa **IEquatable**
- **Vector4i** usa **Vector4**
- **Vector4i** usa **Vector4d**
- **MeshDecimation** usa **DecimationAlgorithm**
- **MeshDecimation** usa **Algorithm**
- **MeshDecimation** usa **FastQuadricMeshSimplification**
- **MeshDecimation** usa **Mesh**
- **Mesh** usa **Vector3d**
- **Mesh** usa **Vector3**
- **Mesh** usa **Vector4**
- **Mesh** usa **Vector2**
- **Mesh** usa **BoneWeight**
- **Mesh** usa **MathHelper**
- **Logging** usa **ILogger**
- **Logging** usa **ConsoleLogger**
- **BoneWeight** implementa **IEquatable**
- **BoneWeight** usa **Vector4**
- **MeshSimplifier** usa **MeshUtils**
- **MeshSimplifier** usa **SimplificationOptions**
- **MeshSimplifier** usa **Triangle**
- **MeshSimplifier** usa **Vertex**
- **MeshSimplifier** usa **Ref**
- **MeshSimplifier** usa **Vector3**
- **MeshSimplifier** usa **Vector4**
- **MeshSimplifier** usa **Vector2**
- **MeshSimplifier** usa **BoneWeight**
- **MeshSimplifier** usa **BlendShapeContainer**
- **MeshSimplifier** usa **Mesh**
- **MeshSimplifier** usa **SymmetricMatrix**
- **MeshSimplifier** usa **Vector3d**
- **MeshSimplifier** usa **MathHelper**
- **MeshSimplifier** usa **BorderVertex**
- **MeshSimplifier** usa **BorderVertexComparer**
- **MeshSimplifier** usa **BlendShape**
- **MeshSimplifier** usa **ValidateSimplificationOptionsException**
- **MeshSimplifier** usa **ResizableArray**
- **MeshSimplifier** usa **UVChannels**
- **BlendShape** usa **BlendShapeFrame**
- **BlendShapeFrame** usa **Vector3**
- **MeshUtilsTest** usa **Mesh**
- **MeshUtilsTest** usa **Vector3**
- **MeshUtilsTest** usa **BlendShape**
- **MeshUtilsTest** usa **BlendShapeFrame**
- **MeshUtilsTest** usa **MeshUtils**
- **MeshUtilsTest** usa **Vector4**
- **MeshUtilsTest** usa **Vector2**
- **MeshUtilsTest** usa **BoneWeight**
- **ExportarGeometria** usa **Mesh**
- **FastQuadricMeshSimplification** hereda de **DecimationAlgorithm**
- **FastQuadricMeshSimplification** usa **Vector3d**
- **FastQuadricMeshSimplification** usa **SymmetricMatrix**
- **FastQuadricMeshSimplification** usa **BorderVertex**
- **FastQuadricMeshSimplification** usa **BorderVertexComparer**
- **FastQuadricMeshSimplification** usa **Triangle**
- **FastQuadricMeshSimplification** usa **Vertex**
- **FastQuadricMeshSimplification** usa **Ref**
- **FastQuadricMeshSimplification** usa **Vector3**
- **FastQuadricMeshSimplification** usa **Vector4**
- **FastQuadricMeshSimplification** usa **Vector2**
- **FastQuadricMeshSimplification** usa **BoneWeight**
- **FastQuadricMeshSimplification** usa **Logging**
- **FastQuadricMeshSimplification** usa **MathHelper**
- **FastQuadricMeshSimplification** usa **Mesh**
- **FastQuadricMeshSimplification** usa **ResizableArray**
- **FastQuadricMeshSimplification** usa **UVChannels**
- **Triangle** usa **Vector3d**
- **Vertex** usa **Vector3d**
- **Vertex** usa **SymmetricMatrix**
- **BorderVertexComparer** implementa **IComparer**
- **BorderVertexComparer** usa **BorderVertex**
- **DecimationAlgorithm** usa **MathHelper**
- **DecimationAlgorithm** usa **Mesh**
- **Vector3** implementa **IEquatable**
- **Vector3** usa **Vector3d**
- **Vector3** usa **Vector3i**
- **Vector3** usa **MathHelper**
- **Vector2i** implementa **IEquatable**
- **Vector2i** usa **Vector2**
- **Vector2i** usa **Vector2d**
- **Vector2d** implementa **IEquatable**
- **Vector2d** usa **Vector2**
- **Vector2d** usa **Vector2i**
- **Vector4d** implementa **IEquatable**
- **Vector4d** usa **Vector4**
- **Vector4d** usa **Vector4i**
- **Vector4** implementa **IEquatable**
- **Vector4** usa **Vector4d**
- **Vector4** usa **Vector4i**
- **Vector3i** implementa **IEquatable**
- **Vector3i** usa **Vector3**
- **Vector3i** usa **Vector3d**
- **MathHelper** usa **Vector3**
- **MathHelper** usa **Vector3d**
- **MeshDecimation** usa **DecimationAlgorithm**
- **MeshDecimation** usa **Algorithm**
- **MeshDecimation** usa **FastQuadricMeshSimplification**
- **MeshDecimation** usa **Mesh**
- **Mesh** usa **Vector3d**
- **Mesh** usa **Vector3**
- **Mesh** usa **Vector4**
- **Mesh** usa **Vector2**
- **Mesh** usa **BoneWeight**
- **Mesh** usa **MathHelper**
- **MeshCombiner** usa **Mesh**
- **MeshCombiner** usa **Vector3**
- **MeshCombiner** usa **Vector4**
- **MeshCombiner** usa **BoneWeight**
- **MeshCombiner** usa **MeshUtils**
- **LODGenerator** usa **Mesh**
- **LODGenerator** usa **LODGeneratorHelper**
- **LODGenerator** usa **SimplificationOptions**
- **LODGenerator** usa **LODLevel**
- **LODGenerator** usa **MeshSimplifier**
- **LODGenerator** usa **RendererInfo**
- **LODGenerator** usa **MeshCombiner**
- **LODGenerator** usa **Vector3**
- **LODGenerator** usa **LODBackupComponent**
- **RendererInfo** usa **Mesh**

## Tipos

### Namespace `(sin namespace)`

#### static class ExportarGeometria

- Archivo: `ExportarGeometria.cs` (172 líneas)
- Usa: `Mesh`

**Campos:**

- FORMAT_MAGIC : int
- FORMAT_VERSION : int

**Métodos:**

- public static ExportRawDump(Document doc, string filePath = @"C:\.TBT\Proyectos\_Revit_EXE_Geometrias\PostProcesadoEXE\IN\export2.bin") : void
- private static ExportarElemento(Document doc, Element elem, Options geomOptions, BinaryWriter writer, Dictionary<int, byte[]> colorCache) : void
- private static ObtenerColorMaterial(Document doc, Element elem, ElementId materialElementId, Dictionary<int, byte[]> cache) : byte[]
- private static ObtenerTodosLosSolidos(GeometryElement geomElem) : List<Solid>

#### class ShowExample

- Archivo: `ShowExample.cs` (35 líneas)
- Hereda de: `MonoBehaviour`
- Usa: `Vector3`

**Campos:**

- cameraTransform : Transform
- targetTransform : Transform
- cameraAngleTime : float
- cameraMinDistance : float
- cameraMaxDistance : float
- cameraDistanceTime : float
- cameraSwayHeight : float
- cameraSwayTime : float

**Métodos:**

- private Update() : void

### Namespace `ConvertidorGeometrias`

#### class MeshData

- Archivo: `Program.cs` (14 líneas)
- Usa: `Vector3`

**Campos:**

- ElementId : int
- Guid : string
- MaterialId : int
- CategoryId : int
- ColR : byte
- ColG : byte
- ColB : byte
- ColA : byte
- HasColor : bool
- Vertices : List<Vector3>
- Normals : List<Vector3>
- Indices : List<int>

#### class PiezaRota

- Archivo: `Program.cs` (7 líneas)

**Campos:**

- Guid : string
- ElementId : int
- Problema : string
- Detalle : string

#### class PipelineStats

- Archivo: `Program.cs` (19 líneas)

**Campos:**

- FormatVersion : int
- InputMeshes : int
- OutputPieces : int
- VertsIn : long
- VertsWelded : long
- VertsOut : long
- TrisIn : long
- TrisOut : long
- DegenerateRemoved : long
- Fallbacks : int
- Retries : int
- Discarded : int
- NotDecimated : int
- ProtectedCat : int
- Transparent : int
- Instanced : int
- Repaired : int
- PoolMeshes : int
- TbvBytes : long

#### class Program

- Archivo: `Program.cs` (1206 líneas)
- Usa: `MeshData`, `PipelineStats`, `PiezaRota`, `Vector3`, `Vector3d`, `Mesh`, `FastQuadricMeshSimplification`, `MeshDecimation`, `Vector4`

**Campos:**

- FormatMagic : int
- WeldGrid : double
- SpikeTolerance : float
- MinVertsParaDecimar : int
- MinTrisParaDecimar : int
- CategoriasProtegidas : HashSet<int>
- APP_GUID : string
- CARPETA_TEMP : string
- RUTA_BASE_SALIDA : string
- RUTA_LOG : string

**Métodos:**

- private static EsVidrioProtegido(MeshData m) : bool
- private static Main(string[] args) : void
- private static EstaBloqueado(string path) : bool
- private static ProcesarArchivo(string filePath) : void
- private static Log(string msg) : void
- private static ConstruirReporte(PipelineStats s, List<PiezaRota> detectadas, List<PiezaRota> rotas, TimeSpan elapsed) : string
- private static Pct(long now, long before) : string
- private static ValidarPiezas(Dictionary<string, MeshData> originales, List<MeshData> finales) : List<PiezaRota>
- private static RepararPiezas(List<PiezaRota> rotas, Dictionary<string, MeshData> originales, List<MeshData> finales, PipelineStats stats) : void
- private static ClonarConNormales(MeshData src) : MeshData
- private static BBoxStr(List<Vector3> verts) : string
- private static OptimizeMeshes(List<MeshData> inputMeshes, PipelineStats stats, Dictionary<string, MeshData> weldedOriginals) : List<MeshData>
- private static ComputeTarget(int originalTriangles) : int
- private static TryDecimate(MeshData source, int targetTriangles, PipelineStats stats) : MeshData
- private static TotalArea(MeshData mesh) : double
- private static GetBounds(List<Vector3> verts, out Vector3 min, out Vector3 max) : void
- private static WeldVertices(MeshData mesh) : void
- private static RemoveDegenerateTriangles(MeshData mesh) : long
- private static CompactVertices(MeshData mesh) : void
- private static ComputeNormals(MeshData mesh) : void
- private static LeerBinario(string path, PipelineStats stats) : List<MeshData>
- private static ExportToObj(List<MeshData> meshes, string outputPath) : void
- private static ExportToGlb(List<MeshData> meshes, string outputPath, PipelineStats stats) : void
- private static GeometryHash(MeshData mesh) : long
- private static EsCopiaTrasladada(MeshData a, MeshData b, out Vector3 delta) : bool
- private static ExportToViewerBin(List<MeshData> meshes, string path, PipelineStats stats) : void
- private static Snorm(float f) : sbyte

### Namespace `MeshDecimator`

#### enum Algorithm

- Archivo: `MeshDecimation.cs` (11 líneas)

**Campos:**

- Default : enum
- FastQuadricMesh : enum

#### enum Algorithm

- Archivo: `MeshDecimation.cs` (11 líneas)

**Campos:**

- Default : enum
- FastQuadricMesh : enum

#### struct BoneWeight

- Archivo: `BoneWeight.cs` (214 líneas)
- Implementa: `IEquatable`
- Usa: `Vector4`

**Campos:**

- boneIndex0 : int
- boneIndex1 : int
- boneIndex2 : int
- boneIndex3 : int
- boneWeight0 : float
- boneWeight1 : float
- boneWeight2 : float
- boneWeight3 : float

**Métodos:**

- public BoneWeight(int boneIndex0, int boneIndex1, int boneIndex2, int boneIndex3, float boneWeight0, float boneWeight1, float boneWeight2, float boneWeight3) : (constructor)
- private MergeBoneWeight(int boneIndex, float weight) : void
- private Normalize() : void
- public GetHashCode() : int
- public Equals(object obj) : bool
- public Equals(BoneWeight other) : bool
- public ToString() : string
- public static Merge(ref BoneWeight a, ref BoneWeight b) : void

#### struct BoneWeight

- Archivo: `BoneWeight.cs` (214 líneas)
- Implementa: `IEquatable`
- Usa: `Vector4`

**Campos:**

- boneIndex0 : int
- boneIndex1 : int
- boneIndex2 : int
- boneIndex3 : int
- boneWeight0 : float
- boneWeight1 : float
- boneWeight2 : float
- boneWeight3 : float

**Métodos:**

- public BoneWeight(int boneIndex0, int boneIndex1, int boneIndex2, int boneIndex3, float boneWeight0, float boneWeight1, float boneWeight2, float boneWeight3) : (constructor)
- private MergeBoneWeight(int boneIndex, float weight) : void
- private Normalize() : void
- public GetHashCode() : int
- public Equals(object obj) : bool
- public Equals(BoneWeight other) : bool
- public ToString() : string
- public static Merge(ref BoneWeight a, ref BoneWeight b) : void

#### interface ILogger

- Archivo: `Logging.cs` (20 líneas)

**Métodos:**

- public LogVerbose(string text) : void
- public LogWarning(string text) : void
- public LogError(string text) : void

#### interface ILogger

- Archivo: `Logging.cs` (20 líneas)

**Métodos:**

- public LogVerbose(string text) : void
- public LogWarning(string text) : void
- public LogError(string text) : void

#### static class Logging

- Archivo: `Logging.cs` (116 líneas)
- Usa: `ILogger`, `ConsoleLogger`

**Propiedades:**

- Logger : ILogger

**Campos:**

- logger : ILogger
- syncObj : object

**Métodos:**

- private static Logging() : (constructor)
- public static LogVerbose(string text) : void
- public static LogVerbose(string format, params object[] args) : void
- public static LogWarning(string text) : void
- public static LogWarning(string format, params object[] args) : void
- public static LogError(string text) : void
- public static LogError(string format, params object[] args) : void

#### static class Logging

- Archivo: `Logging.cs` (116 líneas)
- Usa: `ILogger`, `ConsoleLogger`

**Propiedades:**

- Logger : ILogger

**Campos:**

- logger : ILogger
- syncObj : object

**Métodos:**

- private static Logging() : (constructor)
- public static LogVerbose(string text) : void
- public static LogVerbose(string format, params object[] args) : void
- public static LogWarning(string text) : void
- public static LogWarning(string format, params object[] args) : void
- public static LogError(string text) : void
- public static LogError(string format, params object[] args) : void

#### class Mesh

- Archivo: `Mesh.cs` (919 líneas)
- Usa: `Vector3d`, `Vector3`, `Vector4`, `Vector2`, `BoneWeight`, `MathHelper`

**Propiedades:**

- VertexCount : int
- SubMeshCount : int
- TriangleCount : int
- Vertices : Vector3d[]
- Indices : int[]
- Normals : Vector3[]
- Tangents : Vector4[]
- UV1 : Vector2[]
- UV2 : Vector2[]
- UV3 : Vector2[]
- UV4 : Vector2[]
- Colors : Vector4[]
- BoneWeights : BoneWeight[]

**Campos:**

- UVChannelCount : int
- vertices : Vector3d[]
- indices : int[][]
- normals : Vector3[]
- tangents : Vector4[]
- uvs2D : Vector2[][]
- uvs3D : Vector3[][]
- uvs4D : Vector4[][]
- colors : Vector4[]
- boneWeights : BoneWeight[]
- emptyIndices : int[]

**Métodos:**

- public Mesh(Vector3d[] vertices, int[] indices) : (constructor)
- public Mesh(Vector3d[] vertices, int[][] indices) : (constructor)
- private ClearVertexAttributes() : void
- public RecalculateNormals() : void
- public RecalculateTangents() : void
- public GetTriangleCount(int subMeshIndex) : int
- public GetIndices(int subMeshIndex) : int[]
- public GetSubMeshIndices() : int[][]
- public SetIndices(int subMeshIndex, int[] indices) : void
- public GetUVDimension(int channel) : int
- public GetUVs2D(int channel) : Vector2[]
- public GetUVs3D(int channel) : Vector3[]
- public GetUVs4D(int channel) : Vector4[]
- public GetUVs(int channel, List<Vector2> uvs) : void
- public GetUVs(int channel, List<Vector3> uvs) : void
- public GetUVs(int channel, List<Vector4> uvs) : void
- public SetUVs(int channel, Vector2[] uvs) : void
- public SetUVs(int channel, Vector3[] uvs) : void
- public SetUVs(int channel, Vector4[] uvs) : void
- public SetUVs(int channel, List<Vector2> uvs) : void
- public SetUVs(int channel, List<Vector3> uvs) : void
- public SetUVs(int channel, List<Vector4> uvs) : void
- public ToString() : string

#### class Mesh

- Archivo: `Mesh.cs` (919 líneas)
- Usa: `Vector3d`, `Vector3`, `Vector4`, `Vector2`, `BoneWeight`, `MathHelper`

**Propiedades:**

- VertexCount : int
- SubMeshCount : int
- TriangleCount : int
- Vertices : Vector3d[]
- Indices : int[]
- Normals : Vector3[]
- Tangents : Vector4[]
- UV1 : Vector2[]
- UV2 : Vector2[]
- UV3 : Vector2[]
- UV4 : Vector2[]
- Colors : Vector4[]
- BoneWeights : BoneWeight[]

**Campos:**

- UVChannelCount : int
- vertices : Vector3d[]
- indices : int[][]
- normals : Vector3[]
- tangents : Vector4[]
- uvs2D : Vector2[][]
- uvs3D : Vector3[][]
- uvs4D : Vector4[][]
- colors : Vector4[]
- boneWeights : BoneWeight[]
- emptyIndices : int[]

**Métodos:**

- public Mesh(Vector3d[] vertices, int[] indices) : (constructor)
- public Mesh(Vector3d[] vertices, int[][] indices) : (constructor)
- private ClearVertexAttributes() : void
- public RecalculateNormals() : void
- public RecalculateTangents() : void
- public GetTriangleCount(int subMeshIndex) : int
- public GetIndices(int subMeshIndex) : int[]
- public GetSubMeshIndices() : int[][]
- public SetIndices(int subMeshIndex, int[] indices) : void
- public GetUVDimension(int channel) : int
- public GetUVs2D(int channel) : Vector2[]
- public GetUVs3D(int channel) : Vector3[]
- public GetUVs4D(int channel) : Vector4[]
- public GetUVs(int channel, List<Vector2> uvs) : void
- public GetUVs(int channel, List<Vector3> uvs) : void
- public GetUVs(int channel, List<Vector4> uvs) : void
- public SetUVs(int channel, Vector2[] uvs) : void
- public SetUVs(int channel, Vector3[] uvs) : void
- public SetUVs(int channel, Vector4[] uvs) : void
- public SetUVs(int channel, List<Vector2> uvs) : void
- public SetUVs(int channel, List<Vector3> uvs) : void
- public SetUVs(int channel, List<Vector4> uvs) : void
- public ToString() : string

#### static class MeshDecimation

- Archivo: `MeshDecimation.cs` (128 líneas)
- Usa: `DecimationAlgorithm`, `Algorithm`, `FastQuadricMeshSimplification`, `Mesh`

**Métodos:**

- public static CreateAlgorithm(Algorithm algorithm) : DecimationAlgorithm
- public static DecimateMesh(Mesh mesh, int targetTriangleCount) : Mesh
- public static DecimateMesh(Algorithm algorithm, Mesh mesh, int targetTriangleCount) : Mesh
- public static DecimateMesh(DecimationAlgorithm algorithm, Mesh mesh, int targetTriangleCount) : Mesh
- public static DecimateMeshLossless(Mesh mesh) : Mesh
- public static DecimateMeshLossless(Algorithm algorithm, Mesh mesh) : Mesh
- public static DecimateMeshLossless(DecimationAlgorithm algorithm, Mesh mesh) : Mesh

#### static class MeshDecimation

- Archivo: `MeshDecimation.cs` (128 líneas)
- Usa: `DecimationAlgorithm`, `Algorithm`, `FastQuadricMeshSimplification`, `Mesh`

**Métodos:**

- public static CreateAlgorithm(Algorithm algorithm) : DecimationAlgorithm
- public static DecimateMesh(Mesh mesh, int targetTriangleCount) : Mesh
- public static DecimateMesh(Algorithm algorithm, Mesh mesh, int targetTriangleCount) : Mesh
- public static DecimateMesh(DecimationAlgorithm algorithm, Mesh mesh, int targetTriangleCount) : Mesh
- public static DecimateMeshLossless(Mesh mesh) : Mesh
- public static DecimateMeshLossless(Algorithm algorithm, Mesh mesh) : Mesh
- public static DecimateMeshLossless(DecimationAlgorithm algorithm, Mesh mesh) : Mesh

### Namespace `MeshDecimator.Algorithms`

#### struct BorderVertex

- Archivo: `FastQuadricMeshSimplification.cs` (11 líneas)

**Campos:**

- index : int
- hash : int

**Métodos:**

- public BorderVertex(int index, int hash) : (constructor)

#### struct BorderVertex

- Archivo: `FastQuadricMeshSimplification.cs` (11 líneas)

**Campos:**

- index : int
- hash : int

**Métodos:**

- public BorderVertex(int index, int hash) : (constructor)

#### class BorderVertexComparer

- Archivo: `FastQuadricMeshSimplification.cs` (9 líneas)
- Implementa: `IComparer`
- Usa: `BorderVertex`

**Campos:**

- instance : BorderVertexComparer

**Métodos:**

- public Compare(BorderVertex x, BorderVertex y) : int

#### class BorderVertexComparer

- Archivo: `FastQuadricMeshSimplification.cs` (9 líneas)
- Implementa: `IComparer`
- Usa: `BorderVertex`

**Campos:**

- instance : BorderVertexComparer

**Métodos:**

- public Compare(BorderVertex x, BorderVertex y) : int

#### abstract class DecimationAlgorithm

- Archivo: `DecimationAlgorithm.cs` (129 líneas)
- Usa: `MathHelper`, `Mesh`

**Propiedades:**

- KeepBorders : bool
- PreserveBorders : bool
- KeepLinkedVertices : bool
- MaxVertexCount : int
- Verbose : bool

**Campos:**

- preserveBorders : bool
- maxVertexCount : int
- verbose : bool
- statusReportInvoker : StatusReportCallback

**Métodos:**

- protected ReportStatus(int iteration, int originalTris, int currentTris, int targetTris) : void
- public Initialize(Mesh mesh) : void
- public DecimateMesh(int targetTrisCount) : void
- public DecimateMeshLossless() : void
- public ToMesh() : Mesh

#### abstract class DecimationAlgorithm

- Archivo: `DecimationAlgorithm.cs` (129 líneas)
- Usa: `MathHelper`, `Mesh`

**Propiedades:**

- KeepBorders : bool
- PreserveBorders : bool
- KeepLinkedVertices : bool
- MaxVertexCount : int
- Verbose : bool

**Campos:**

- preserveBorders : bool
- maxVertexCount : int
- verbose : bool
- statusReportInvoker : StatusReportCallback

**Métodos:**

- protected ReportStatus(int iteration, int originalTris, int currentTris, int targetTris) : void
- public Initialize(Mesh mesh) : void
- public DecimateMesh(int targetTrisCount) : void
- public DecimateMeshLossless() : void
- public ToMesh() : Mesh

#### class FastQuadricMeshSimplification

- Archivo: `FastQuadricMeshSimplification.cs` (1490 líneas)
- Hereda de: `DecimationAlgorithm`
- Usa: `Vector3d`, `SymmetricMatrix`, `BorderVertex`, `BorderVertexComparer`, `Triangle`, `Vertex`, `Ref`, `Vector3`, `Vector4`, `Vector2`, `BoneWeight`, `Logging`, `MathHelper`, `Mesh`, `ResizableArray`, `UVChannels`

**Propiedades:**

- PreserveSeams : bool
- PreserveFoldovers : bool
- EnableSmartLink : bool
- MaxIterationCount : int
- Agressiveness : double
- VertexLinkDistanceSqr : double

**Campos:**

- DoubleEpsilon : double
- preserveSeams : bool
- preserveFoldovers : bool
- enableSmartLink : bool
- maxIterationCount : int
- agressiveness : double
- vertexLinkDistanceSqr : double
- subMeshCount : int
- triangles : ResizableArray<Triangle>
- vertices : ResizableArray<Vertex>
- refs : ResizableArray<Ref>
- vertNormals : ResizableArray<Vector3>
- vertTangents : ResizableArray<Vector4>
- vertUV2D : UVChannels<Vector2>
- vertUV3D : UVChannels<Vector3>
- vertUV4D : UVChannels<Vector4>
- vertColors : ResizableArray<Vector4>
- vertBoneWeights : ResizableArray<BoneWeight>
- remainingVertices : int
- errArr : double[]
- attributeIndexArr : int[]

**Métodos:**

- public FastQuadricMeshSimplification() : (constructor)
- private InitializeVertexAttribute(T[] attributeValues, string attributeName) : ResizableArray<T>
- private VertexError(ref SymmetricMatrix q, double x, double y, double z) : double
- private CalculateError(ref Vertex vert0, ref Vertex vert1, out Vector3d result, out int resultIndex) : double
- private Flipped(ref Vector3d p, int i0, int i1, ref Vertex v0, bool[] deleted) : bool
- private UpdateTriangles(int i0, int ia0, ref Vertex v, ResizableArray<bool> deleted, ref int deletedTriangles) : void
- private MoveVertexAttributes(int i0, int i1) : void
- private MergeVertexAttributes(int i0, int i1) : void
- private AreUVsTheSame(int channel, int indexA, int indexB) : bool
- private RemoveVertexPass(int startTrisCount, int targetTrisCount, double threshold, ResizableArray<bool> deleted0, ResizableArray<bool> deleted1, ref int deletedTris) : void
- private UpdateMesh(int iteration) : void
- private UpdateReferences() : void
- private CompactMesh() : void
- public Initialize(Mesh mesh) : void
- public DecimateMesh(int targetTrisCount) : void
- public DecimateMeshLossless() : void
- public ToMesh() : Mesh

#### class FastQuadricMeshSimplification

- Archivo: `FastQuadricMeshSimplification.cs` (1490 líneas)
- Hereda de: `DecimationAlgorithm`
- Usa: `Vector3d`, `SymmetricMatrix`, `BorderVertex`, `BorderVertexComparer`, `Triangle`, `Vertex`, `Ref`, `Vector3`, `Vector4`, `Vector2`, `BoneWeight`, `Logging`, `MathHelper`, `Mesh`, `ResizableArray`, `UVChannels`

**Propiedades:**

- PreserveSeams : bool
- PreserveFoldovers : bool
- EnableSmartLink : bool
- MaxIterationCount : int
- Agressiveness : double
- VertexLinkDistanceSqr : double

**Campos:**

- DoubleEpsilon : double
- preserveSeams : bool
- preserveFoldovers : bool
- enableSmartLink : bool
- maxIterationCount : int
- agressiveness : double
- vertexLinkDistanceSqr : double
- subMeshCount : int
- triangles : ResizableArray<Triangle>
- vertices : ResizableArray<Vertex>
- refs : ResizableArray<Ref>
- vertNormals : ResizableArray<Vector3>
- vertTangents : ResizableArray<Vector4>
- vertUV2D : UVChannels<Vector2>
- vertUV3D : UVChannels<Vector3>
- vertUV4D : UVChannels<Vector4>
- vertColors : ResizableArray<Vector4>
- vertBoneWeights : ResizableArray<BoneWeight>
- remainingVertices : int
- errArr : double[]
- attributeIndexArr : int[]

**Métodos:**

- public FastQuadricMeshSimplification() : (constructor)
- private InitializeVertexAttribute(T[] attributeValues, string attributeName) : ResizableArray<T>
- private VertexError(ref SymmetricMatrix q, double x, double y, double z) : double
- private CalculateError(ref Vertex vert0, ref Vertex vert1, out Vector3d result, out int resultIndex) : double
- private Flipped(ref Vector3d p, int i0, int i1, ref Vertex v0, bool[] deleted) : bool
- private UpdateTriangles(int i0, int ia0, ref Vertex v, ResizableArray<bool> deleted, ref int deletedTriangles) : void
- private MoveVertexAttributes(int i0, int i1) : void
- private MergeVertexAttributes(int i0, int i1) : void
- private AreUVsTheSame(int channel, int indexA, int indexB) : bool
- private RemoveVertexPass(int startTrisCount, int targetTrisCount, double threshold, ResizableArray<bool> deleted0, ResizableArray<bool> deleted1, ref int deletedTris) : void
- private UpdateMesh(int iteration) : void
- private UpdateReferences() : void
- private CompactMesh() : void
- public Initialize(Mesh mesh) : void
- public DecimateMesh(int targetTrisCount) : void
- public DecimateMeshLossless() : void
- public ToMesh() : Mesh

#### struct Ref

- Archivo: `FastQuadricMeshSimplification.cs` (11 líneas)

**Campos:**

- tid : int
- tvertex : int

**Métodos:**

- public Set(int tid, int tvertex) : void

#### struct Ref

- Archivo: `FastQuadricMeshSimplification.cs` (11 líneas)

**Campos:**

- tid : int
- tvertex : int

**Métodos:**

- public Set(int tid, int tvertex) : void

#### struct Triangle

- Archivo: `FastQuadricMeshSimplification.cs` (101 líneas)
- Usa: `Vector3d`

**Campos:**

- v0 : int
- v1 : int
- v2 : int
- subMeshIndex : int
- va0 : int
- va1 : int
- va2 : int
- err0 : double
- err1 : double
- err2 : double
- err3 : double
- deleted : bool
- dirty : bool
- n : Vector3d

**Métodos:**

- public Triangle(int v0, int v1, int v2, int subMeshIndex) : (constructor)
- public GetAttributeIndices(int[] attributeIndices) : void
- public SetAttributeIndex(int index, int value) : void
- public GetErrors(double[] err) : void

#### struct Triangle

- Archivo: `FastQuadricMeshSimplification.cs` (101 líneas)
- Usa: `Vector3d`

**Campos:**

- v0 : int
- v1 : int
- v2 : int
- subMeshIndex : int
- va0 : int
- va1 : int
- va2 : int
- err0 : double
- err1 : double
- err2 : double
- err3 : double
- deleted : bool
- dirty : bool
- n : Vector3d

**Métodos:**

- public Triangle(int v0, int v1, int v2, int subMeshIndex) : (constructor)
- public GetAttributeIndices(int[] attributeIndices) : void
- public SetAttributeIndex(int index, int value) : void
- public GetErrors(double[] err) : void

#### struct Vertex

- Archivo: `FastQuadricMeshSimplification.cs` (21 líneas)
- Usa: `Vector3d`, `SymmetricMatrix`

**Campos:**

- p : Vector3d
- tstart : int
- tcount : int
- q : SymmetricMatrix
- border : bool
- seam : bool
- foldover : bool

**Métodos:**

- public Vertex(Vector3d p) : (constructor)

#### struct Vertex

- Archivo: `FastQuadricMeshSimplification.cs` (21 líneas)
- Usa: `Vector3d`, `SymmetricMatrix`

**Campos:**

- p : Vector3d
- tstart : int
- tcount : int
- q : SymmetricMatrix
- border : bool
- seam : bool
- foldover : bool

**Métodos:**

- public Vertex(Vector3d p) : (constructor)

### Namespace `MeshDecimator.Collections`

#### class ResizableArray

- Archivo: `ResizableArray.cs` (144 líneas)

**Propiedades:**

- Length : int
- Data : T[]

**Campos:**

- items : T[]
- length : int
- emptyArr : T[]

**Métodos:**

- public ResizableArray(int capacity) : (constructor)
- public ResizableArray(int capacity, int length) : (constructor)
- private IncreaseCapacity(int capacity) : void
- public Clear() : void
- public Resize(int length, bool trimExess = false) : void
- public TrimExcess() : void
- public Add(T item) : void

#### class ResizableArray

- Archivo: `ResizableArray.cs` (144 líneas)

**Propiedades:**

- Length : int
- Data : T[]

**Campos:**

- items : T[]
- length : int
- emptyArr : T[]

**Métodos:**

- public ResizableArray(int capacity) : (constructor)
- public ResizableArray(int capacity, int length) : (constructor)
- private IncreaseCapacity(int capacity) : void
- public Clear() : void
- public Resize(int length, bool trimExess = false) : void
- public TrimExcess() : void
- public Add(T item) : void

#### class UVChannels

- Archivo: `UVChannels.cs` (70 líneas)
- Usa: `Mesh`, `ResizableArray`

**Propiedades:**

- Data : TVec[][]

**Campos:**

- channels : ResizableArray<TVec>[]
- channelsData : TVec[][]

**Métodos:**

- public UVChannels() : (constructor)
- public Resize(int capacity, bool trimExess = false) : void

#### class UVChannels

- Archivo: `UVChannels.cs` (70 líneas)
- Usa: `Mesh`, `ResizableArray`

**Propiedades:**

- Data : TVec[][]

**Campos:**

- channels : ResizableArray<TVec>[]
- channelsData : TVec[][]

**Métodos:**

- public UVChannels() : (constructor)
- public Resize(int capacity, bool trimExess = false) : void

### Namespace `MeshDecimator.Loggers`

#### class ConsoleLogger

- Archivo: `ConsoleLogger.cs` (29 líneas)
- Implementa: `ILogger`

**Métodos:**

- public LogVerbose(string text) : void
- public LogWarning(string text) : void
- public LogError(string text) : void

#### class ConsoleLogger

- Archivo: `ConsoleLogger.cs` (29 líneas)
- Implementa: `ILogger`

**Métodos:**

- public LogVerbose(string text) : void
- public LogWarning(string text) : void
- public LogError(string text) : void

### Namespace `MeshDecimator.Math`

#### static class MathHelper

- Archivo: `MathHelper.cs` (252 líneas)
- Usa: `Vector3`, `Vector3d`

**Campos:**

- PI : float
- PId : double
- Deg2Rad : float
- Deg2Radd : double
- Rad2Deg : float
- Rad2Degd : double

**Métodos:**

- public static Min(int val1, int val2) : int
- public static Min(int val1, int val2, int val3) : int
- public static Min(float val1, float val2) : float
- public static Min(float val1, float val2, float val3) : float
- public static Min(double val1, double val2) : double
- public static Min(double val1, double val2, double val3) : double
- public static Max(int val1, int val2) : int
- public static Max(int val1, int val2, int val3) : int
- public static Max(float val1, float val2) : float
- public static Max(float val1, float val2, float val3) : float
- public static Max(double val1, double val2) : double
- public static Max(double val1, double val2, double val3) : double
- public static Clamp(float value, float min, float max) : float
- public static Clamp(double value, double min, double max) : double
- public static Clamp01(float value) : float
- public static Clamp01(double value) : double
- public static TriangleArea(ref Vector3 p0, ref Vector3 p1, ref Vector3 p2) : float
- public static TriangleArea(ref Vector3d p0, ref Vector3d p1, ref Vector3d p2) : double

#### static class MathHelper

- Archivo: `MathHelper.cs` (252 líneas)
- Usa: `Vector3`, `Vector3d`

**Campos:**

- PI : float
- PId : double
- Deg2Rad : float
- Deg2Radd : double
- Rad2Deg : float
- Rad2Degd : double

**Métodos:**

- public static Min(int val1, int val2) : int
- public static Min(int val1, int val2, int val3) : int
- public static Min(float val1, float val2) : float
- public static Min(float val1, float val2, float val3) : float
- public static Min(double val1, double val2) : double
- public static Min(double val1, double val2, double val3) : double
- public static Max(int val1, int val2) : int
- public static Max(int val1, int val2, int val3) : int
- public static Max(float val1, float val2) : float
- public static Max(float val1, float val2, float val3) : float
- public static Max(double val1, double val2) : double
- public static Max(double val1, double val2, double val3) : double
- public static Clamp(float value, float min, float max) : float
- public static Clamp(double value, double min, double max) : double
- public static Clamp01(float value) : float
- public static Clamp01(double value) : double
- public static TriangleArea(ref Vector3 p0, ref Vector3 p1, ref Vector3 p2) : float
- public static TriangleArea(ref Vector3d p0, ref Vector3d p1, ref Vector3d p2) : double

#### struct SymmetricMatrix

- Archivo: `SymmetricMatrix.cs` (269 líneas)

**Campos:**

- m0 : double
- m1 : double
- m2 : double
- m3 : double
- m4 : double
- m5 : double
- m6 : double
- m7 : double
- m8 : double
- m9 : double

**Métodos:**

- public SymmetricMatrix(double c) : (constructor)
- public SymmetricMatrix(double m0, double m1, double m2, double m3, double m4, double m5, double m6, double m7, double m8, double m9) : (constructor)
- public SymmetricMatrix(double a, double b, double c, double d) : (constructor)
- internal Determinant1() : double
- internal Determinant2() : double
- internal Determinant3() : double
- internal Determinant4() : double
- public Determinant(int a11, int a12, int a13, int a21, int a22, int a23, int a31, int a32, int a33) : double

#### struct SymmetricMatrix

- Archivo: `SymmetricMatrix.cs` (269 líneas)

**Campos:**

- m0 : double
- m1 : double
- m2 : double
- m3 : double
- m4 : double
- m5 : double
- m6 : double
- m7 : double
- m8 : double
- m9 : double

**Métodos:**

- public SymmetricMatrix(double c) : (constructor)
- public SymmetricMatrix(double m0, double m1, double m2, double m3, double m4, double m5, double m6, double m7, double m8, double m9) : (constructor)
- public SymmetricMatrix(double a, double b, double c, double d) : (constructor)
- internal Determinant1() : double
- internal Determinant2() : double
- internal Determinant3() : double
- internal Determinant4() : double
- public Determinant(int a11, int a12, int a13, int a21, int a22, int a23, int a31, int a32, int a33) : double

#### struct Vector2

- Archivo: `Vector2.cs` (390 líneas)
- Implementa: `IEquatable`
- Usa: `Vector2d`, `Vector2i`

**Propiedades:**

- Magnitude : float
- MagnitudeSqr : float
- Normalized : Vector2

**Campos:**

- zero : Vector2
- Epsilon : float
- x : float
- y : float

**Métodos:**

- public Vector2(float value) : (constructor)
- public Vector2(float x, float y) : (constructor)
- public Set(float x, float y) : void
- public Scale(ref Vector2 scale) : void
- public Normalize() : void
- public Clamp(float min, float max) : void
- public GetHashCode() : int
- public Equals(object other) : bool
- public Equals(Vector2 other) : bool
- public ToString() : string
- public ToString(string format) : string
- public static Dot(ref Vector2 lhs, ref Vector2 rhs) : float
- public static Lerp(ref Vector2 a, ref Vector2 b, float t, out Vector2 result) : void
- public static Scale(ref Vector2 a, ref Vector2 b, out Vector2 result) : void
- public static Normalize(ref Vector2 value, out Vector2 result) : void

#### struct Vector2

- Archivo: `Vector2.cs` (390 líneas)
- Implementa: `IEquatable`
- Usa: `Vector2d`, `Vector2i`

**Propiedades:**

- Magnitude : float
- MagnitudeSqr : float
- Normalized : Vector2

**Campos:**

- zero : Vector2
- Epsilon : float
- x : float
- y : float

**Métodos:**

- public Vector2(float value) : (constructor)
- public Vector2(float x, float y) : (constructor)
- public Set(float x, float y) : void
- public Scale(ref Vector2 scale) : void
- public Normalize() : void
- public Clamp(float min, float max) : void
- public GetHashCode() : int
- public Equals(object other) : bool
- public Equals(Vector2 other) : bool
- public ToString() : string
- public ToString(string format) : string
- public static Dot(ref Vector2 lhs, ref Vector2 rhs) : float
- public static Lerp(ref Vector2 a, ref Vector2 b, float t, out Vector2 result) : void
- public static Scale(ref Vector2 a, ref Vector2 b, out Vector2 result) : void
- public static Normalize(ref Vector2 value, out Vector2 result) : void

#### struct Vector2d

- Archivo: `Vector2d.cs` (390 líneas)
- Implementa: `IEquatable`
- Usa: `Vector2`, `Vector2i`

**Propiedades:**

- Magnitude : double
- MagnitudeSqr : double
- Normalized : Vector2d

**Campos:**

- zero : Vector2d
- Epsilon : double
- x : double
- y : double

**Métodos:**

- public Vector2d(double value) : (constructor)
- public Vector2d(double x, double y) : (constructor)
- public Set(double x, double y) : void
- public Scale(ref Vector2d scale) : void
- public Normalize() : void
- public Clamp(double min, double max) : void
- public GetHashCode() : int
- public Equals(object other) : bool
- public Equals(Vector2d other) : bool
- public ToString() : string
- public ToString(string format) : string
- public static Dot(ref Vector2d lhs, ref Vector2d rhs) : double
- public static Lerp(ref Vector2d a, ref Vector2d b, double t, out Vector2d result) : void
- public static Scale(ref Vector2d a, ref Vector2d b, out Vector2d result) : void
- public static Normalize(ref Vector2d value, out Vector2d result) : void

#### struct Vector2d

- Archivo: `Vector2d.cs` (390 líneas)
- Implementa: `IEquatable`
- Usa: `Vector2`, `Vector2i`

**Propiedades:**

- Magnitude : double
- MagnitudeSqr : double
- Normalized : Vector2d

**Campos:**

- zero : Vector2d
- Epsilon : double
- x : double
- y : double

**Métodos:**

- public Vector2d(double value) : (constructor)
- public Vector2d(double x, double y) : (constructor)
- public Set(double x, double y) : void
- public Scale(ref Vector2d scale) : void
- public Normalize() : void
- public Clamp(double min, double max) : void
- public GetHashCode() : int
- public Equals(object other) : bool
- public Equals(Vector2d other) : bool
- public ToString() : string
- public ToString(string format) : string
- public static Dot(ref Vector2d lhs, ref Vector2d rhs) : double
- public static Lerp(ref Vector2d a, ref Vector2d b, double t, out Vector2d result) : void
- public static Scale(ref Vector2d a, ref Vector2d b, out Vector2d result) : void
- public static Normalize(ref Vector2d value, out Vector2d result) : void

#### struct Vector2i

- Archivo: `Vector2i.cs` (313 líneas)
- Implementa: `IEquatable`
- Usa: `Vector2`, `Vector2d`

**Propiedades:**

- Magnitude : int
- MagnitudeSqr : int

**Campos:**

- zero : Vector2i
- x : int
- y : int

**Métodos:**

- public Vector2i(int value) : (constructor)
- public Vector2i(int x, int y) : (constructor)
- public Set(int x, int y) : void
- public Scale(ref Vector2i scale) : void
- public Clamp(int min, int max) : void
- public GetHashCode() : int
- public Equals(object other) : bool
- public Equals(Vector2i other) : bool
- public ToString() : string
- public ToString(string format) : string
- public static Scale(ref Vector2i a, ref Vector2i b, out Vector2i result) : void

#### struct Vector2i

- Archivo: `Vector2i.cs` (313 líneas)
- Implementa: `IEquatable`
- Usa: `Vector2`, `Vector2d`

**Propiedades:**

- Magnitude : int
- MagnitudeSqr : int

**Campos:**

- zero : Vector2i
- x : int
- y : int

**Métodos:**

- public Vector2i(int value) : (constructor)
- public Vector2i(int x, int y) : (constructor)
- public Set(int x, int y) : void
- public Scale(ref Vector2i scale) : void
- public Clamp(int min, int max) : void
- public GetHashCode() : int
- public Equals(object other) : bool
- public Equals(Vector2i other) : bool
- public ToString() : string
- public ToString(string format) : string
- public static Scale(ref Vector2i a, ref Vector2i b, out Vector2i result) : void

#### struct Vector3

- Archivo: `Vector3.cs` (459 líneas)
- Implementa: `IEquatable`
- Usa: `Vector3d`, `Vector3i`, `MathHelper`

**Propiedades:**

- Magnitude : float
- MagnitudeSqr : float
- Normalized : Vector3

**Campos:**

- zero : Vector3
- Epsilon : float
- x : float
- y : float
- z : float

**Métodos:**

- public Vector3(float value) : (constructor)
- public Vector3(float x, float y, float z) : (constructor)
- public Vector3(Vector3d vector) : (constructor)
- public Set(float x, float y, float z) : void
- public Scale(ref Vector3 scale) : void
- public Normalize() : void
- public Clamp(float min, float max) : void
- public GetHashCode() : int
- public Equals(object other) : bool
- public Equals(Vector3 other) : bool
- public ToString() : string
- public ToString(string format) : string
- public static Dot(ref Vector3 lhs, ref Vector3 rhs) : float
- public static Cross(ref Vector3 lhs, ref Vector3 rhs, out Vector3 result) : void
- public static Angle(ref Vector3 from, ref Vector3 to) : float
- public static Lerp(ref Vector3 a, ref Vector3 b, float t, out Vector3 result) : void
- public static Scale(ref Vector3 a, ref Vector3 b, out Vector3 result) : void
- public static Normalize(ref Vector3 value, out Vector3 result) : void
- public static OrthoNormalize(ref Vector3 normal, ref Vector3 tangent) : void

#### struct Vector3

- Archivo: `Vector3.cs` (459 líneas)
- Implementa: `IEquatable`
- Usa: `Vector3d`, `Vector3i`, `MathHelper`

**Propiedades:**

- Magnitude : float
- MagnitudeSqr : float
- Normalized : Vector3

**Campos:**

- zero : Vector3
- Epsilon : float
- x : float
- y : float
- z : float

**Métodos:**

- public Vector3(float value) : (constructor)
- public Vector3(float x, float y, float z) : (constructor)
- public Vector3(Vector3d vector) : (constructor)
- public Set(float x, float y, float z) : void
- public Scale(ref Vector3 scale) : void
- public Normalize() : void
- public Clamp(float min, float max) : void
- public GetHashCode() : int
- public Equals(object other) : bool
- public Equals(Vector3 other) : bool
- public ToString() : string
- public ToString(string format) : string
- public static Dot(ref Vector3 lhs, ref Vector3 rhs) : float
- public static Cross(ref Vector3 lhs, ref Vector3 rhs, out Vector3 result) : void
- public static Angle(ref Vector3 from, ref Vector3 to) : float
- public static Lerp(ref Vector3 a, ref Vector3 b, float t, out Vector3 result) : void
- public static Scale(ref Vector3 a, ref Vector3 b, out Vector3 result) : void
- public static Normalize(ref Vector3 value, out Vector3 result) : void
- public static OrthoNormalize(ref Vector3 normal, ref Vector3 tangent) : void

#### struct Vector3d

- Archivo: `Vector3d.cs` (446 líneas)
- Implementa: `IEquatable`
- Usa: `Vector3`, `Vector3i`, `MathHelper`

**Propiedades:**

- Magnitude : double
- MagnitudeSqr : double
- Normalized : Vector3d

**Campos:**

- zero : Vector3d
- Epsilon : double
- x : double
- y : double
- z : double

**Métodos:**

- public Vector3d(double value) : (constructor)
- public Vector3d(double x, double y, double z) : (constructor)
- public Vector3d(Vector3 vector) : (constructor)
- public Set(double x, double y, double z) : void
- public Scale(ref Vector3d scale) : void
- public Normalize() : void
- public Clamp(double min, double max) : void
- public GetHashCode() : int
- public Equals(object other) : bool
- public Equals(Vector3d other) : bool
- public ToString() : string
- public ToString(string format) : string
- public static Dot(ref Vector3d lhs, ref Vector3d rhs) : double
- public static Cross(ref Vector3d lhs, ref Vector3d rhs, out Vector3d result) : void
- public static Angle(ref Vector3d from, ref Vector3d to) : double
- public static Lerp(ref Vector3d a, ref Vector3d b, double t, out Vector3d result) : void
- public static Scale(ref Vector3d a, ref Vector3d b, out Vector3d result) : void
- public static Normalize(ref Vector3d value, out Vector3d result) : void

#### struct Vector3d

- Archivo: `Vector3d.cs` (446 líneas)
- Implementa: `IEquatable`
- Usa: `Vector3`, `Vector3i`, `MathHelper`

**Propiedades:**

- Magnitude : double
- MagnitudeSqr : double
- Normalized : Vector3d

**Campos:**

- zero : Vector3d
- Epsilon : double
- x : double
- y : double
- z : double

**Métodos:**

- public Vector3d(double value) : (constructor)
- public Vector3d(double x, double y, double z) : (constructor)
- public Vector3d(Vector3 vector) : (constructor)
- public Set(double x, double y, double z) : void
- public Scale(ref Vector3d scale) : void
- public Normalize() : void
- public Clamp(double min, double max) : void
- public GetHashCode() : int
- public Equals(object other) : bool
- public Equals(Vector3d other) : bool
- public ToString() : string
- public ToString(string format) : string
- public static Dot(ref Vector3d lhs, ref Vector3d rhs) : double
- public static Cross(ref Vector3d lhs, ref Vector3d rhs, out Vector3d result) : void
- public static Angle(ref Vector3d from, ref Vector3d to) : double
- public static Lerp(ref Vector3d a, ref Vector3d b, double t, out Vector3d result) : void
- public static Scale(ref Vector3d a, ref Vector3d b, out Vector3d result) : void
- public static Normalize(ref Vector3d value, out Vector3d result) : void

#### struct Vector3i

- Archivo: `Vector3i.cs` (333 líneas)
- Implementa: `IEquatable`
- Usa: `Vector3`, `Vector3d`

**Propiedades:**

- Magnitude : int
- MagnitudeSqr : int

**Campos:**

- zero : Vector3i
- x : int
- y : int
- z : int

**Métodos:**

- public Vector3i(int value) : (constructor)
- public Vector3i(int x, int y, int z) : (constructor)
- public Set(int x, int y, int z) : void
- public Scale(ref Vector3i scale) : void
- public Clamp(int min, int max) : void
- public GetHashCode() : int
- public Equals(object other) : bool
- public Equals(Vector3i other) : bool
- public ToString() : string
- public ToString(string format) : string
- public static Scale(ref Vector3i a, ref Vector3i b, out Vector3i result) : void

#### struct Vector3i

- Archivo: `Vector3i.cs` (333 líneas)
- Implementa: `IEquatable`
- Usa: `Vector3`, `Vector3d`

**Propiedades:**

- Magnitude : int
- MagnitudeSqr : int

**Campos:**

- zero : Vector3i
- x : int
- y : int
- z : int

**Métodos:**

- public Vector3i(int value) : (constructor)
- public Vector3i(int x, int y, int z) : (constructor)
- public Set(int x, int y, int z) : void
- public Scale(ref Vector3i scale) : void
- public Clamp(int min, int max) : void
- public GetHashCode() : int
- public Equals(object other) : bool
- public Equals(Vector3i other) : bool
- public ToString() : string
- public ToString(string format) : string
- public static Scale(ref Vector3i a, ref Vector3i b, out Vector3i result) : void

#### struct Vector4

- Archivo: `Vector4.cs` (432 líneas)
- Implementa: `IEquatable`
- Usa: `Vector4d`, `Vector4i`

**Propiedades:**

- Magnitude : float
- MagnitudeSqr : float
- Normalized : Vector4

**Campos:**

- zero : Vector4
- Epsilon : float
- x : float
- y : float
- z : float
- w : float

**Métodos:**

- public Vector4(float value) : (constructor)
- public Vector4(float x, float y, float z, float w) : (constructor)
- public Set(float x, float y, float z, float w) : void
- public Scale(ref Vector4 scale) : void
- public Normalize() : void
- public Clamp(float min, float max) : void
- public GetHashCode() : int
- public Equals(object other) : bool
- public Equals(Vector4 other) : bool
- public ToString() : string
- public ToString(string format) : string
- public static Dot(ref Vector4 lhs, ref Vector4 rhs) : float
- public static Lerp(ref Vector4 a, ref Vector4 b, float t, out Vector4 result) : void
- public static Scale(ref Vector4 a, ref Vector4 b, out Vector4 result) : void
- public static Normalize(ref Vector4 value, out Vector4 result) : void

#### struct Vector4

- Archivo: `Vector4.cs` (432 líneas)
- Implementa: `IEquatable`
- Usa: `Vector4d`, `Vector4i`

**Propiedades:**

- Magnitude : float
- MagnitudeSqr : float
- Normalized : Vector4

**Campos:**

- zero : Vector4
- Epsilon : float
- x : float
- y : float
- z : float
- w : float

**Métodos:**

- public Vector4(float value) : (constructor)
- public Vector4(float x, float y, float z, float w) : (constructor)
- public Set(float x, float y, float z, float w) : void
- public Scale(ref Vector4 scale) : void
- public Normalize() : void
- public Clamp(float min, float max) : void
- public GetHashCode() : int
- public Equals(object other) : bool
- public Equals(Vector4 other) : bool
- public ToString() : string
- public ToString(string format) : string
- public static Dot(ref Vector4 lhs, ref Vector4 rhs) : float
- public static Lerp(ref Vector4 a, ref Vector4 b, float t, out Vector4 result) : void
- public static Scale(ref Vector4 a, ref Vector4 b, out Vector4 result) : void
- public static Normalize(ref Vector4 value, out Vector4 result) : void

#### struct Vector4d

- Archivo: `Vector4d.cs` (432 líneas)
- Implementa: `IEquatable`
- Usa: `Vector4`, `Vector4i`

**Propiedades:**

- Magnitude : double
- MagnitudeSqr : double
- Normalized : Vector4d

**Campos:**

- zero : Vector4d
- Epsilon : double
- x : double
- y : double
- z : double
- w : double

**Métodos:**

- public Vector4d(double value) : (constructor)
- public Vector4d(double x, double y, double z, double w) : (constructor)
- public Set(double x, double y, double z, double w) : void
- public Scale(ref Vector4d scale) : void
- public Normalize() : void
- public Clamp(double min, double max) : void
- public GetHashCode() : int
- public Equals(object other) : bool
- public Equals(Vector4d other) : bool
- public ToString() : string
- public ToString(string format) : string
- public static Dot(ref Vector4d lhs, ref Vector4d rhs) : double
- public static Lerp(ref Vector4d a, ref Vector4d b, double t, out Vector4d result) : void
- public static Scale(ref Vector4d a, ref Vector4d b, out Vector4d result) : void
- public static Normalize(ref Vector4d value, out Vector4d result) : void

#### struct Vector4d

- Archivo: `Vector4d.cs` (432 líneas)
- Implementa: `IEquatable`
- Usa: `Vector4`, `Vector4i`

**Propiedades:**

- Magnitude : double
- MagnitudeSqr : double
- Normalized : Vector4d

**Campos:**

- zero : Vector4d
- Epsilon : double
- x : double
- y : double
- z : double
- w : double

**Métodos:**

- public Vector4d(double value) : (constructor)
- public Vector4d(double x, double y, double z, double w) : (constructor)
- public Set(double x, double y, double z, double w) : void
- public Scale(ref Vector4d scale) : void
- public Normalize() : void
- public Clamp(double min, double max) : void
- public GetHashCode() : int
- public Equals(object other) : bool
- public Equals(Vector4d other) : bool
- public ToString() : string
- public ToString(string format) : string
- public static Dot(ref Vector4d lhs, ref Vector4d rhs) : double
- public static Lerp(ref Vector4d a, ref Vector4d b, double t, out Vector4d result) : void
- public static Scale(ref Vector4d a, ref Vector4d b, out Vector4d result) : void
- public static Normalize(ref Vector4d value, out Vector4d result) : void

#### struct Vector4i

- Archivo: `Vector4i.cs` (353 líneas)
- Implementa: `IEquatable`
- Usa: `Vector4`, `Vector4d`

**Propiedades:**

- Magnitude : int
- MagnitudeSqr : int

**Campos:**

- zero : Vector4i
- x : int
- y : int
- z : int
- w : int

**Métodos:**

- public Vector4i(int value) : (constructor)
- public Vector4i(int x, int y, int z, int w) : (constructor)
- public Set(int x, int y, int z, int w) : void
- public Scale(ref Vector4i scale) : void
- public Clamp(int min, int max) : void
- public GetHashCode() : int
- public Equals(object other) : bool
- public Equals(Vector4i other) : bool
- public ToString() : string
- public ToString(string format) : string
- public static Scale(ref Vector4i a, ref Vector4i b, out Vector4i result) : void

#### struct Vector4i

- Archivo: `Vector4i.cs` (353 líneas)
- Implementa: `IEquatable`
- Usa: `Vector4`, `Vector4d`

**Propiedades:**

- Magnitude : int
- MagnitudeSqr : int

**Campos:**

- zero : Vector4i
- x : int
- y : int
- z : int
- w : int

**Métodos:**

- public Vector4i(int value) : (constructor)
- public Vector4i(int x, int y, int z, int w) : (constructor)
- public Set(int x, int y, int z, int w) : void
- public Scale(ref Vector4i scale) : void
- public Clamp(int min, int max) : void
- public GetHashCode() : int
- public Equals(object other) : bool
- public Equals(Vector4i other) : bool
- public ToString() : string
- public ToString(string format) : string
- public static Scale(ref Vector4i a, ref Vector4i b, out Vector4i result) : void

### Namespace `MeshDecimator.Unity`

#### class DecimatedObject

- Archivo: `DecimatedObject.cs` (67 líneas)
- Hereda de: `MonoBehaviour`
- Usa: `LODSettings`, `LODGenerator`

**Propiedades:**

- Levels : LODSettings[]
- IsGenerated : bool

**Campos:**

- levels : LODSettings[]
- generated : bool

**Métodos:**

- private Reset() : void
- public GenerateLODs(LODStatusReportCallback statusCallback = null) : void
- public ResetLODs() : void

#### static class LODGenerator

- Archivo: `LODGenerator.cs` (368 líneas)
- Usa: `Mesh`, `DecimationAlgorithm`, `MeshDecimatorUtility`, `LODSettings`, `Vector3`

**Campos:**

- ParentGameObjectName : string

**Métodos:**

- private static CombineRenderers(MeshRenderer[] meshRenderers, SkinnedMeshRenderer[] skinnedRenderers) : Renderer[]
- private static GenerateStaticLOD(Transform transform, MeshRenderer renderer, float quality, out Material[] materials, DecimationAlgorithm.StatusReportCallback statusCallback) : Mesh
- private static GenerateStaticLOD(Transform transform, MeshRenderer[] renderers, float quality, out Material[] materials, DecimationAlgorithm.StatusReportCallback statusCallback) : Mesh
- private static GenerateSkinnedLOD(Transform transform, SkinnedMeshRenderer renderer, float quality, out Material[] materials, out Transform[] mergedBones, DecimationAlgorithm.StatusReportCallback statusCallback) : Mesh
- private static GenerateSkinnedLOD(Transform transform, SkinnedMeshRenderer[] renderers, float quality, out Material[] materials, out Transform[] mergedBones, DecimationAlgorithm.StatusReportCallback statusCallback) : Mesh
- private static FindRootBone(Transform transform, Transform[] transforms) : Transform
- private static SetupLODRenderer(Renderer renderer, LODSettings settings) : void
- public static GenerateLODs(GameObject gameObj, LODSettings[] levels, LODStatusReportCallback statusCallback = null) : void
- public static DestroyLODs(GameObject gameObj) : void

#### struct LODSettings

- Archivo: `LODGenerator.cs` (127 líneas)

**Campos:**

- quality : float
- combineMeshes : bool
- skinQuality : SkinQuality
- receiveShadows : bool
- shadowCasting : ShadowCastingMode
- motionVectors : MotionVectorGenerationMode
- skinnedMotionVectors : bool
- lightProbeUsage : LightProbeUsage
- reflectionProbeUsage : ReflectionProbeUsage

**Métodos:**

- public LODSettings(float quality) : (constructor)
- public LODSettings(float quality, SkinQuality skinQuality) : (constructor)
- public LODSettings(float quality, SkinQuality skinQuality, bool receiveShadows, ShadowCastingMode shadowCasting) : (constructor)
- public LODSettings(float quality, SkinQuality skinQuality, bool receiveShadows, ShadowCastingMode shadowCasting, MotionVectorGenerationMode motionVectors, bool skinnedMotionVectors) : (constructor)

#### static class MeshDecimatorUtility

- Archivo: `MeshDecimatorUtility.cs` (876 líneas)
- Usa: `Logging`, `ConsoleLogger`, `UnityLogger`, `Vector3d`, `Vector2`, `Vector3`, `Vector4`, `BoneWeight`, `Mesh`, `DecimationAlgorithm`, `MeshDecimation`, `Algorithm`

**Métodos:**

- private static MeshDecimatorUtility() : (constructor)
- private static ToSimplifyVertices(UVector3[] vertices) : Vector3d[]
- private static ToSimplifyVec(UVector2[] vectors) : Vector2[]
- private static ToSimplifyVec(UVector3[] vectors) : Vector3[]
- private static ToSimplifyVec(UVector4[] vectors) : Vector4[]
- private static ToSimplifyVec(UColor[] colors) : Vector4[]
- private static ToSimplifyBoneWeights(UBoneWeight[] boneWeights) : BoneWeight[]
- private static FromSimplifyVertices(Vector3d[] vertices) : UVector3[]
- private static FromSimplifyVec(Vector2[] vectors) : UVector2[]
- private static FromSimplifyVec(Vector3[] vectors) : UVector3[]
- private static FromSimplifyVec(Vector4[] vectors) : UVector4[]
- private static FromSimplifyColor(Vector4[] vectors) : UColor[]
- private static FromSimplifyBoneWeights(BoneWeight[] boneWeights) : UBoneWeight[]
- private static AddToList(List<Vector3d> list, UVector3[] arr, int previousVertexCount, int totalVertexCount) : void
- private static AddToList(ref List<Vector2> list, UVector2[] arr, int previousVertexCount, int currentVertexCount, int totalVertexCount, Vector2 defaultValue) : void
- private static AddToList(ref List<Vector3> list, UVector3[] arr, int previousVertexCount, int currentVertexCount, int totalVertexCount, Vector3 defaultValue) : void
- private static AddToList(ref List<Vector4> list, UVector4[] arr, int previousVertexCount, int currentVertexCount, int totalVertexCount, Vector4 defaultValue) : void
- private static AddToList(ref List<Vector4> list, UColor[] arr, int previousVertexCount, int currentVertexCount, int totalVertexCount) : void
- private static AddToList(ref List<BoneWeight> list, UBoneWeight[] arr, int previousVertexCount, int currentVertexCount, int totalVertexCount) : void
- private static TransformVertices(UVector3[] vertices, ref UMatrix transform) : void
- private static TransformVertices(UVector3[] vertices, UBoneWeight[] boneWeights, UMatrix[] oldBindposes, UMatrix[] newBindposes) : void
- private static ScaleMatrix(ref UMatrix m, float scale) : UMatrix
- private static MergeArrays(T[] arr1, T[] arr2) : T[]
- private static RemapBones(UBoneWeight[] boneWeights, int[] boneIndices) : void
- private static CreateMesh(UMatrix[] bindposes, UVector3[] vertices, Mesh destMesh, bool recalculateNormals) : UMesh
- public static DecimateMesh(UMesh mesh, UMatrix transform, float quality, bool recalculateNormals, DecimationAlgorithm.StatusReportCallback statusCallback = null) : UMesh
- public static DecimateMeshes(UMesh[] meshes, UMatrix[] transforms, UMaterial[][] materials, float quality, bool recalculateNormals, out UMaterial[] resultMaterials, DecimationAlgorithm.StatusReportCallback statusCallback = null) : UMesh
- public static DecimateMeshes(UMesh[] meshes, UMatrix[] transforms, UMaterial[][] materials, UTransform[][] meshBones, float quality, bool recalculateNormals, out UMaterial[] resultMaterials, out UTransform[] mergedBones, DecimationAlgorithm.StatusReportCallback statusCallback = null) : UMesh

### Namespace `MeshDecimator.Unity.Loggers`

#### class UnityLogger

- Archivo: `UnityLogger.cs` (29 líneas)
- Implementa: `ILogger`

**Métodos:**

- public LogVerbose(string text) : void
- public LogWarning(string text) : void
- public LogError(string text) : void

### Namespace `MeshDecimator.UnityEditor`

#### class DecimatedObjectEditor

- Archivo: `DecimatedObjectEditor.cs` (174 líneas)
- Hereda de: `Editor`
- Usa: `DecimatedObject`

**Propiedades:**

- target : DecimatedObject

**Campos:**

- levelsProp : SerializedProperty
- generatedProp : SerializedProperty
- isGeneratingNew : bool
- settingsExpanded : bool[]
- settingsContent : GUIContent

**Métodos:**

- private OnEnable() : void
- private GenerateLODs() : void
- private ResetLODs() : void
- public OnInspectorGUI() : void

### Namespace `MeshDecimatorTool`

#### struct FaceIndex

- Archivo: `ObjMesh.cs` (40 líneas)
- Implementa: `IEquatable`

**Campos:**

- vertexIndex : int
- texCoordIndex : int
- normalIndex : int
- hashCode : int

**Métodos:**

- public FaceIndex(int vertexIndex, int texCoordIndex, int normalIndex) : (constructor)
- public GetHashCode() : int
- public Equals(object obj) : bool
- public Equals(FaceIndex other) : bool
- public ToString() : string

#### class ObjMesh

- Archivo: `ObjMesh.cs` (853 líneas)
- Usa: `FaceIndex`, `Vector3d`, `Vector3`, `Vector2`

**Propiedades:**

- Vertices : Vector3d[]
- Normals : Vector3[]
- TexCoords2D : Vector2[]
- TexCoords3D : Vector3[]
- SubMeshCount : int
- Indices : int[]
- SubMeshIndices : int[][]
- SubMeshMaterials : string[]
- MaterialLibraries : string[]

**Campos:**

- VertexInitialCapacity : int
- IndexInitialCapacity : int
- vertices : Vector3d[]
- normals : Vector3[]
- texCoords2D : Vector2[]
- texCoords3D : Vector3[]
- subMeshIndices : int[][]
- subMeshMaterials : string[]
- materialLibraries : string[]

**Métodos:**

- public ObjMesh() : (constructor)
- public ObjMesh(Vector3d[] vertices, int[] indices) : (constructor)
- public ObjMesh(Vector3d[] vertices, int[][] indices) : (constructor)
- public ReadFile(string path) : void
- public WriteFile(string path) : void
- private static WriteVertices(TextWriter writer, Vector3d[] vertices) : void
- private static WriteNormals(TextWriter writer, Vector3[] normals) : void
- private static WriteTextureCoords(TextWriter writer, Vector2[] texCoords2D, Vector3[] texCoords3D) : void
- private static WriteSubMeshes(TextWriter writer, int[][] subMeshIndices, string[] subMeshMaterials, bool hasTexCoords, bool hasNormals) : void
- private static WriteFaces(TextWriter writer, int[] indices, bool hasTexCoords, bool hasNormals) : void
- private static ShiftIndex(int value, int count) : int
- private static CountOccurrences(string text, char character) : int

#### class Program

- Archivo: `Program.cs` (111 líneas)
- Usa: `MathHelper`, `ObjMesh`, `Mesh`, `MeshDecimation`, `Algorithm`

**Métodos:**

- private static Main(string[] args) : void
- private static PrintUsage() : void

### Namespace `UnityMeshSimplifier`

#### struct BlendShape

- Archivo: `BlendShape.cs` (24 líneas)
- Usa: `BlendShapeFrame`

**Campos:**

- ShapeName : string
- Frames : BlendShapeFrame[]

**Métodos:**

- public BlendShape(string shapeName, BlendShapeFrame[] frames) : (constructor)

#### struct BlendShapeFrame

- Archivo: `BlendShape.cs` (36 líneas)
- Usa: `Vector3`

**Campos:**

- FrameWeight : float
- DeltaVertices : Vector3[]
- DeltaNormals : Vector3[]
- DeltaTangents : Vector3[]

**Métodos:**

- public BlendShapeFrame(float frameWeight, Vector3[] deltaVertices, Vector3[] deltaNormals, Vector3[] deltaTangents) : (constructor)

#### static class IOUtils

- Archivo: `IOUtils.cs` (93 líneas)

**Métodos:**

- internal static MakeSafeRelativePath(string path) : string
- internal static MakeSafeFileName(string name) : string

#### class LODBackupComponent

- Archivo: `LODBackupComponent.cs` (12 líneas)
- Hereda de: `MonoBehaviour`

**Propiedades:**

- OriginalRenderers : Renderer[]

**Campos:**

- originalRenderers : Renderer[]

#### static class LODGenerator

- Archivo: `LODGenerator.cs` (787 líneas)
- Usa: `Mesh`, `LODGeneratorHelper`, `SimplificationOptions`, `LODLevel`, `MeshSimplifier`, `RendererInfo`, `MeshCombiner`, `Vector3`, `LODBackupComponent`

**Campos:**

- LODParentGameObjectName : string
- LODAssetDefaultParentPath : string
- AssetsRootPath : string
- LODAssetUserData : string

**Métodos:**

- public static GenerateLODs(LODGeneratorHelper generatorHelper) : LODGroup
- public static GenerateLODs(GameObject gameObject, LODLevel[] levels, bool autoCollectRenderers, SimplificationOptions simplificationOptions) : LODGroup
- public static GenerateLODs(GameObject gameObject, LODLevel[] levels, bool autoCollectRenderers, SimplificationOptions simplificationOptions, string saveAssetsPath) : LODGroup
- public static DestroyLODs(LODGeneratorHelper generatorHelper) : bool
- public static DestroyLODs(GameObject gameObject) : bool
- private static GetStaticRenderers(MeshRenderer[] renderers) : RendererInfo[]
- private static GetSkinnedRenderers(SkinnedMeshRenderer[] renderers) : RendererInfo[]
- private static CombineStaticMeshes(Transform transform, int levelIndex, MeshRenderer[] renderers) : RendererInfo[]
- private static CombineSkinnedMeshes(Transform transform, int levelIndex, SkinnedMeshRenderer[] renderers) : RendererInfo[]
- private static ParentAndResetTransform(Transform transform, Transform parentTransform) : void
- private static ParentAndOffsetTransform(Transform transform, Transform parentTransform, Transform originalTransform) : void
- private static CreateLevelRenderer(GameObject gameObject, int levelIndex, in LODLevel level, Transform levelTransform, int rendererIndex, in RendererInfo renderer, in SimplificationOptions simplificationOptions, string saveAssetsPath) : Renderer
- private static CreateStaticLevelRenderer(string name, Transform parentTransform, Transform originalTransform, Mesh mesh, Material[] materials, in LODLevel level) : MeshRenderer
- private static CreateSkinnedLevelRenderer(string name, Transform parentTransform, Transform originalTransform, Mesh mesh, Material[] materials, Transform rootBone, Transform[] bones, in LODLevel level) : SkinnedMeshRenderer
- private static FindBestRootBone(Transform transform, SkinnedMeshRenderer[] skinnedMeshRenderers) : Transform
- private static SetupLevelRenderer(Renderer renderer, in LODLevel level) : void
- private static GetChildRenderersForLOD(GameObject gameObject) : Renderer[]
- private static CollectChildRenderersForLOD(Transform transform, List<Renderer> resultRenderers) : void
- private static SimplifyMesh(Mesh mesh, float quality, in SimplificationOptions options) : Mesh
- private static DestroyObject(Object obj) : void
- private static CreateBackup(GameObject gameObject, Renderer[] originalRenderers) : void
- private static RestoreBackup(GameObject gameObject) : void
- private static ValidateSaveAssetsPath(string saveAssetsPath) : string

#### class LODGeneratorHelper

- Archivo: `LODGeneratorHelper.cs` (138 líneas)
- Hereda de: `MonoBehaviour`
- Usa: `SimplificationOptions`, `LODLevel`

**Propiedades:**

- FadeMode : LODFadeMode
- AnimateCrossFading : bool
- AutoCollectRenderers : bool
- SimplificationOptions : SimplificationOptions
- SaveAssetsPath : string
- Levels : LODLevel[]
- IsGenerated : bool

**Campos:**

- fadeMode : LODFadeMode
- animateCrossFading : bool
- autoCollectRenderers : bool
- simplificationOptions : SimplificationOptions
- saveAssetsPath : string
- levels : LODLevel[]
- isGenerated : bool

**Métodos:**

- private Reset() : void

#### struct LODLevel

- Archivo: `LODLevel.cs` (210 líneas)

**Propiedades:**

- ScreenRelativeTransitionHeight : float
- FadeTransitionWidth : float
- Quality : float
- CombineMeshes : bool
- CombineSubMeshes : bool
- Renderers : Renderer[]
- SkinQuality : SkinQuality
- ShadowCastingMode : ShadowCastingMode
- ReceiveShadows : bool
- MotionVectorGenerationMode : MotionVectorGenerationMode
- SkinnedMotionVectors : bool
- LightProbeUsage : LightProbeUsage
- ReflectionProbeUsage : ReflectionProbeUsage

**Campos:**

- screenRelativeTransitionHeight : float
- fadeTransitionWidth : float
- quality : float
- combineMeshes : bool
- combineSubMeshes : bool
- renderers : Renderer[]
- skinQuality : SkinQuality
- shadowCastingMode : ShadowCastingMode
- receiveShadows : bool
- motionVectorGenerationMode : MotionVectorGenerationMode
- skinnedMotionVectors : bool
- lightProbeUsage : LightProbeUsage
- reflectionProbeUsage : ReflectionProbeUsage

**Métodos:**

- public LODLevel(float screenRelativeTransitionHeight, float quality) : (constructor)
- public LODLevel(float screenRelativeTransitionHeight, float fadeTransitionWidth, float quality, bool combineMeshes, bool combineSubMeshes) : (constructor)
- public LODLevel(float screenRelativeTransitionHeight, float fadeTransitionWidth, float quality, bool combineMeshes, bool combineSubMeshes, Renderer[] renderers) : (constructor)

#### static class MathHelper

- Archivo: `MathHelper.cs` (81 líneas)
- Usa: `Vector3d`

**Campos:**

- PI : float
- PId : double
- Deg2Rad : float
- Deg2Radd : double
- Rad2Deg : float
- Rad2Degd : double

**Métodos:**

- public static Min(double val1, double val2, double val3) : double
- public static Clamp(double value, double min, double max) : double
- public static TriangleArea(ref Vector3d p0, ref Vector3d p1, ref Vector3d p2) : double

#### static class MeshCombiner

- Archivo: `MeshCombiner.cs` (444 líneas)
- Usa: `Mesh`, `Vector3`, `Vector4`, `BoneWeight`, `MeshUtils`

**Métodos:**

- public static CombineMeshes(Transform rootTransform, MeshRenderer[] renderers, out Material[] resultMaterials) : Mesh
- public static CombineMeshes(Transform rootTransform, SkinnedMeshRenderer[] renderers, out Material[] resultMaterials, out Transform[] resultBones) : Mesh
- public static CombineMeshes(Mesh[] meshes, Matrix4x4[] transforms, Material[][] materials, out Material[] resultMaterials) : Mesh
- public static CombineMeshes(Mesh[] meshes, Matrix4x4[] transforms, Material[][] materials, Transform[][] bones, out Material[] resultMaterials, out Transform[] resultBones) : Mesh
- private static CopyVertexPositions(ICollection<Vector3> list, Vector3[] arr) : void
- private static CopyVertexAttributes(ref List<T> dest, IEnumerable<T> src, int previousVertexCount, int meshVertexCount, int totalVertexCount, T defaultValue) : void
- private static MergeArrays(T[] arr1, T[] arr2) : T[]
- private static TransformVertices(Vector3[] vertices, ref Matrix4x4 transform) : void
- private static TransformNormals(Vector3[] normals, ref Matrix4x4 transform) : void
- private static TransformTangents(Vector4[] tangents, ref Matrix4x4 transform) : void
- private static RemapBones(BoneWeight[] boneWeights, int[] boneIndices) : void
- private static CanReadMesh(Mesh mesh) : bool

#### class MeshSimplifier

- Archivo: `MeshSimplifier.cs` (2265 líneas)
- Usa: `MeshUtils`, `SimplificationOptions`, `Triangle`, `Vertex`, `Ref`, `Vector3`, `Vector4`, `Vector2`, `BoneWeight`, `BlendShapeContainer`, `Mesh`, `SymmetricMatrix`, `Vector3d`, `MathHelper`, `BorderVertex`, `BorderVertexComparer`, `BlendShape`, `ValidateSimplificationOptionsException`, `ResizableArray`, `UVChannels`

**Propiedades:**

- SimplificationOptions : SimplificationOptions
- PreserveBorderEdges : bool
- PreserveUVSeamEdges : bool
- PreserveUVFoldoverEdges : bool
- PreserveSurfaceCurvature : bool
- EnableSmartLink : bool
- MaxIterationCount : int
- Agressiveness : double
- Verbose : bool
- VertexLinkDistance : double
- VertexLinkDistanceSqr : double
- Vertices : Vector3[]
- SubMeshCount : int
- BlendShapeCount : int
- Normals : Vector3[]
- Tangents : Vector4[]
- UV1 : Vector2[]
- UV2 : Vector2[]
- UV3 : Vector2[]
- UV4 : Vector2[]
- Colors : Color[]
- BoneWeights : BoneWeight[]

**Campos:**

- TriangleEdgeCount : int
- TriangleVertexCount : int
- DoubleEpsilon : double
- DenomEpilson : double
- UVChannelCount : int
- simplificationOptions : SimplificationOptions
- verbose : bool
- subMeshCount : int
- subMeshOffsets : int[]
- triangles : ResizableArray<Triangle>
- vertices : ResizableArray<Vertex>
- refs : ResizableArray<Ref>
- vertNormals : ResizableArray<Vector3>
- vertTangents : ResizableArray<Vector4>
- vertUV2D : UVChannels<Vector2>
- vertUV3D : UVChannels<Vector3>
- vertUV4D : UVChannels<Vector4>
- vertColors : ResizableArray<Color>
- vertBoneWeights : ResizableArray<BoneWeight>
- blendShapes : ResizableArray<BlendShapeContainer>
- bindposes : Matrix4x4[]
- errArr : double[]
- attributeIndexArr : int[]
- triangleHashSet1 : HashSet<Triangle>
- triangleHashSet2 : HashSet<Triangle>

**Métodos:**

- public MeshSimplifier() : (constructor)
- public MeshSimplifier(Mesh mesh) : (constructor)
- private InitializeVertexAttribute(T[] attributeValues, ref ResizableArray<T> attributeArray, string attributeName) : void
- private static VertexError(ref SymmetricMatrix q, double x, double y, double z) : double
- private CurvatureError(ref Vertex vert0, ref Vertex vert1) : double
- private CalculateError(ref Vertex vert0, ref Vertex vert1, out Vector3d result) : double
- private static CalculateBarycentricCoords(ref Vector3d point, ref Vector3d a, ref Vector3d b, ref Vector3d c, out Vector3 result) : void
- private static NormalizeTangent(Vector4 tangent) : Vector4
- private Flipped(ref Vector3d p, int i0, int i1, ref Vertex v0, bool[] deleted) : bool
- private UpdateTriangles(int i0, int ia0, ref Vertex v, ResizableArray<bool> deleted, ref int deletedTriangles) : void
- private InterpolateVertexAttributes(int dst, int i0, int i1, int i2, ref Vector3 barycentricCoord) : void
- private AreUVsTheSame(int channel, int indexA, int indexB) : bool
- private RemoveVertexPass(int startTrisCount, int targetTrisCount, double threshold, ResizableArray<bool> deleted0, ResizableArray<bool> deleted1, ref int deletedTris) : void
- private UpdateMesh(int iteration) : void
- private UpdateReferences() : void
- private CompactMesh() : void
- private CalculateSubMeshOffsets() : void
- private GetTrianglesContainingVertex(ref Vertex vert, HashSet<Triangle> tris) : void
- private GetTrianglesContainingBothVertices(ref Vertex vert0, ref Vertex vert1, HashSet<Triangle> tris) : void
- public GetAllSubMeshTriangles() : int[][]
- public GetSubMeshTriangles(int subMeshIndex) : int[]
- public ClearSubMeshes() : void
- public AddSubMeshTriangles(int[] triangles) : void
- public AddSubMeshTriangles(int[][] triangles) : void
- public GetUVs2D(int channel) : Vector2[]
- public GetUVs3D(int channel) : Vector3[]
- public GetUVs4D(int channel) : Vector4[]
- public GetUVs(int channel, List<Vector2> uvs) : void
- public GetUVs(int channel, List<Vector3> uvs) : void
- public GetUVs(int channel, List<Vector4> uvs) : void
- public SetUVs(int channel, IList<Vector2> uvs) : void
- public SetUVs(int channel, IList<Vector3> uvs) : void
- public SetUVs(int channel, IList<Vector4> uvs) : void
- public SetUVs(int channel, IList<Vector4> uvs, int uvComponentCount) : void
- public SetUVsAuto(int channel, IList<Vector4> uvs) : void
- public GetAllBlendShapes() : BlendShape[]
- public GetBlendShape(int blendShapeIndex) : BlendShape
- public ClearBlendShapes() : void
- public AddBlendShape(BlendShape blendShape) : void
- public AddBlendShapes(BlendShape[] blendShapes) : void
- public Initialize(Mesh mesh) : void
- public SimplifyMesh(float quality) : void
- public SimplifyMeshLossless() : void
- public ToMesh() : Mesh
- public static ValidateOptions(SimplificationOptions options) : void

#### static class MeshUtils

- Archivo: `MeshUtils.cs` (431 líneas)
- Usa: `Mesh`, `Vector3`, `Vector4`, `BoneWeight`, `Vector2`, `BlendShape`, `BlendShapeFrame`

**Campos:**

- UVChannelCount : int

**Métodos:**

- public static CreateMesh(Vector3[] vertices, int[][] indices, Vector3[] normals, Vector4[] tangents, Color[] colors, BoneWeight[] boneWeights, List<Vector2>[] uvs, Matrix4x4[] bindposes, BlendShape[] blendShapes) : Mesh
- public static CreateMesh(Vector3[] vertices, int[][] indices, Vector3[] normals, Vector4[] tangents, Color[] colors, BoneWeight[] boneWeights, List<Vector4>[] uvs, Matrix4x4[] bindposes, BlendShape[] blendShapes) : Mesh
- public static CreateMesh(Vector3[] vertices, int[][] indices, Vector3[] normals, Vector4[] tangents, Color[] colors, BoneWeight[] boneWeights, List<Vector2>[] uvs2D, List<Vector3>[] uvs3D, List<Vector4>[] uvs4D, Matrix4x4[] bindposes, BlendShape[] blendShapes) : Mesh
- public static GetMeshBlendShapes(Mesh mesh) : BlendShape[]
- public static ApplyMeshBlendShapes(Mesh mesh, BlendShape[] blendShapes) : void
- public static GetMeshUVs(Mesh mesh) : IList<Vector4>[]
- public static GetMeshUVs2D(Mesh mesh, int channel) : IList<Vector2>
- public static GetMeshUVs3D(Mesh mesh, int channel) : IList<Vector3>
- public static GetMeshUVs(Mesh mesh, int channel) : IList<Vector4>
- public static GetUsedUVComponents(IList<Vector4> uvs) : int
- public static ConvertUVsTo2D(IList<Vector4> uvs) : Vector2[]
- public static ConvertUVsTo3D(IList<Vector4> uvs) : Vector3[]
- public static GetSubMeshIndexMinMax(int[][] indices, out IndexFormat indexFormat) : Vector2Int[]
- private static GetIndexMinMax(int[] indices, out int minIndex, out int maxIndex) : void

#### struct RendererInfo

- Archivo: `LODGenerator.cs` (11 líneas)
- Usa: `Mesh`

**Campos:**

- name : string
- isStatic : bool
- isNewMesh : bool
- transform : Transform
- mesh : Mesh
- materials : Material[]
- rootBone : Transform
- bones : Transform[]

#### class ResizableArray

- Archivo: `ResizableArray.cs` (182 líneas)

**Propiedades:**

- Length : int
- Data : T[]

**Campos:**

- items : T[]
- length : int
- emptyArr : T[]

**Métodos:**

- public ResizableArray(int capacity) : (constructor)
- public ResizableArray(int capacity, int length) : (constructor)
- public ResizableArray(T[] initialArray) : (constructor)
- private IncreaseCapacity(int capacity) : void
- public Clear() : void
- public Resize(int length, bool trimExess = false, bool clearMemory = false) : void
- public TrimExcess() : void
- public Add(T item) : void
- public ToArray() : T[]

#### struct SimplificationOptions

- Archivo: `SimplificationOptions.cs` (86 líneas)

**Campos:**

- Default : SimplificationOptions
- PreserveBorderEdges : bool
- PreserveUVSeamEdges : bool
- PreserveUVFoldoverEdges : bool
- PreserveSurfaceCurvature : bool
- EnableSmartLink : bool
- VertexLinkDistance : double
- MaxIterationCount : int
- Agressiveness : double
- ManualUVComponentCount : bool
- UVComponentCount : int

#### struct SymmetricMatrix

- Archivo: `SymmetricMatrix.cs` (280 líneas)

**Campos:**

- m0 : double
- m1 : double
- m2 : double
- m3 : double
- m4 : double
- m5 : double
- m6 : double
- m7 : double
- m8 : double
- m9 : double

**Métodos:**

- public SymmetricMatrix(double c) : (constructor)
- public SymmetricMatrix(double m0, double m1, double m2, double m3, double m4, double m5, double m6, double m7, double m8, double m9) : (constructor)
- public SymmetricMatrix(double a, double b, double c, double d) : (constructor)
- internal Determinant1() : double
- internal Determinant2() : double
- internal Determinant3() : double
- internal Determinant4() : double
- public Determinant(int a11, int a12, int a13, int a21, int a22, int a23, int a31, int a32, int a33) : double

#### class ValidateSimplificationOptionsException

- Archivo: `ValidateSimplificationOptionsException.cs` (43 líneas)
- Hereda de: `Exception`

**Propiedades:**

- PropertyName : string
- Message : string

**Campos:**

- propertyName : string

**Métodos:**

- public ValidateSimplificationOptionsException(string propertyName, string message) : (constructor)
- public ValidateSimplificationOptionsException(string propertyName, string message, Exception innerException) : (constructor)

#### struct Vector3d

- Archivo: `Vector3d.cs` (469 líneas)
- Implementa: `IEquatable`
- Usa: `Vector3`, `MathHelper`

**Propiedades:**

- Magnitude : double
- MagnitudeSqr : double
- Normalized : Vector3d

**Campos:**

- zero : Vector3d
- Epsilon : double
- x : double
- y : double
- z : double

**Métodos:**

- public Vector3d(double value) : (constructor)
- public Vector3d(double x, double y, double z) : (constructor)
- public Vector3d(Vector3 vector) : (constructor)
- public Set(double x, double y, double z) : void
- public Scale(ref Vector3d scale) : void
- public Normalize() : void
- public Clamp(double min, double max) : void
- public GetHashCode() : int
- public Equals(object obj) : bool
- public Equals(Vector3d other) : bool
- public ToString() : string
- public ToString(string format) : string
- public static Dot(ref Vector3d lhs, ref Vector3d rhs) : double
- public static Cross(ref Vector3d lhs, ref Vector3d rhs, out Vector3d result) : void
- public static Angle(ref Vector3d from, ref Vector3d to) : double
- public static Lerp(ref Vector3d a, ref Vector3d b, double t, out Vector3d result) : void
- public static Scale(ref Vector3d a, ref Vector3d b, out Vector3d result) : void
- public static Normalize(ref Vector3d value, out Vector3d result) : void

### Namespace `UnityMeshSimplifier.Editor`

#### class LODGeneratorHelperEditor

- Archivo: `LODGeneratorHelperEditor.cs` (657 líneas)
- Hereda de: `Editor`
- Usa: `LODGeneratorHelper`, `IOUtils`, `LODGenerator`

**Campos:**

- FadeModeFieldName : string
- AnimateCrossFadingFieldName : string
- AutoCollectRenderersFieldName : string
- SimplificationOptionsFieldName : string
- SaveAssetsPathFieldName : string
- LevelsFieldName : string
- IsGeneratedFieldName : string
- LevelScreenRelativeHeightFieldName : string
- LevelFadeTransitionWidthFieldName : string
- LevelQualityFieldName : string
- LevelCombineMeshesFieldName : string
- LevelCombineSubMeshesFieldName : string
- LevelRenderersFieldName : string
- SimplificationOptionsEnableSmartLinkFieldName : string
- SimplificationOptionsVertexLinkDistanceFieldName : string
- RemoveLevelButtonSize : float
- RendererButtonWidth : float
- RemoveRendererButtonSize : float
- fadeModeProperty : SerializedProperty
- animateCrossFadingProperty : SerializedProperty
- autoCollectRenderersProperty : SerializedProperty
- simplificationOptionsProperty : SerializedProperty
- saveAssetsPathProperty : SerializedProperty
- levelsProperty : SerializedProperty
- isGeneratedProperty : SerializedProperty
- overrideSaveAssetsPath : bool
- settingsExpanded : bool[]
- lodGeneratorHelper : LODGeneratorHelper
- createLevelButtonContent : GUIContent
- deleteLevelButtonContent : GUIContent
- generateLODButtonContent : GUIContent
- destroyLODButtonContent : GUIContent
- copyVisibilityChangesContent : GUIContent
- settingsContent : GUIContent
- renderersHeaderContent : GUIContent
- removeRendererButtonContent : GUIContent
- addRendererButtonContent : GUIContent
- overrideSaveAssetsPathContent : GUIContent
- removeColor : Color
- ObjectPickerControlID : int

**Métodos:**

- private OnEnable() : void
- public OnInspectorGUI() : void
- private DrawGeneratedView() : void
- private DrawNotGeneratedView() : void
- private DrawSimplificationOptions() : void
- private DrawLevel(int index, SerializedProperty levelProperty, bool hasCrossFade) : void
- private DrawRendererList(SerializedProperty renderersProperty, float availableWidth) : void
- private DrawRendererButton(Rect position, SerializedProperty renderersProperty, int rendererIndex, Renderer renderer) : void
- private HandleAddRenderer(Rect position, Rect listArea, SerializedProperty renderersProperty) : void
- private AddRenderers(SerializedProperty renderersProperty, IEnumerable<Renderer> renderers, bool append) : void
- private CreateLevel() : void
- private DeleteLevel(int index) : void
- private GenerateLODs() : void
- private DestroyLODs() : void
- private VisibilitySettingsHaveChanged() : bool
- private CopyVisibilityChanges() : void
- private GetRenderers(IEnumerable<GameObject> gameObjects, bool searchChildren) : Renderer[]
- private MarkSceneAsDirty() : void
- private static DisplayError(string title, string message, string ok, Object context) : void

#### static class SerializedPropertyExtensions

- Archivo: `SerializedPropertyExtensions.cs` (18 líneas)

**Métodos:**

- public static GetChildProperties(this SerializedProperty property) : IEnumerable<SerializedProperty>

### Namespace `UnityMeshSimplifier.Editor.Tests`

#### class MeshUtilsTest

- Archivo: `MeshUtilsTest.cs` (507 líneas)
- Usa: `Mesh`, `Vector3`, `BlendShape`, `BlendShapeFrame`, `MeshUtils`, `Vector4`, `Vector2`, `BoneWeight`

**Métodos:**

- public ShouldApplyBlendShapes() : void
- public ShouldConvertUVsTo2D() : void
- public ShouldConvertUVsTo3D() : void
- public ShouldCreateMesh() : void
- public ShouldGetMeshBlendShapes() : void
- public ShouldGetMeshUVs() : void
- public ShouldGetSubMeshIndexMinMax() : void
- public ShouldGetUsedUVComponents() : void

### Namespace `UnityMeshSimplifier.Internal`

#### class BlendShapeContainer

- Archivo: `BlendShapeContainer.cs` (51 líneas)
- Usa: `BlendShapeFrameContainer`, `BlendShape`, `Vector3`, `BlendShapeFrame`

**Campos:**

- shapeName : string
- frames : BlendShapeFrameContainer[]

**Métodos:**

- public BlendShapeContainer(BlendShape blendShape) : (constructor)
- public MoveVertexElement(int dst, int src) : void
- public InterpolateVertexAttributes(int dst, int i0, int i1, int i2, ref Vector3 barycentricCoord) : void
- public Resize(int length, bool trimExess = false) : void
- public ToBlendShape() : BlendShape

#### class BlendShapeFrameContainer

- Archivo: `BlendShapeFrameContainer.cs` (46 líneas)
- Usa: `Vector3`, `BlendShapeFrame`, `ResizableArray`

**Campos:**

- frameWeight : float
- deltaVertices : ResizableArray<Vector3>
- deltaNormals : ResizableArray<Vector3>
- deltaTangents : ResizableArray<Vector3>

**Métodos:**

- public BlendShapeFrameContainer(BlendShapeFrame frame) : (constructor)
- public MoveVertexElement(int dst, int src) : void
- public InterpolateVertexAttributes(int dst, int i0, int i1, int i2, ref Vector3 barycentricCoord) : void
- public Resize(int length, bool trimExess = false) : void
- public ToBlendShapeFrame() : BlendShapeFrame

#### struct BorderVertex

- Archivo: `BorderVertex.cs` (12 líneas)

**Campos:**

- index : int
- hash : int

**Métodos:**

- public BorderVertex(int index, int hash) : (constructor)

#### class BorderVertexComparer

- Archivo: `BorderVertex.cs` (10 líneas)
- Implementa: `IComparer`
- Usa: `BorderVertex`

**Campos:**

- instance : BorderVertexComparer

**Métodos:**

- public Compare(BorderVertex x, BorderVertex y) : int

#### struct Ref

- Archivo: `Ref.cs` (12 líneas)

**Campos:**

- tid : int
- tvertex : int

**Métodos:**

- public Set(int tid, int tvertex) : void

#### struct Triangle

- Archivo: `Triangle.cs` (132 líneas)
- Implementa: `IEquatable`
- Usa: `Vector3d`

**Campos:**

- index : int
- v0 : int
- v1 : int
- v2 : int
- subMeshIndex : int
- va0 : int
- va1 : int
- va2 : int
- err0 : double
- err1 : double
- err2 : double
- err3 : double
- deleted : bool
- dirty : bool
- n : Vector3d

**Métodos:**

- public Triangle(int index, int v0, int v1, int v2, int subMeshIndex) : (constructor)
- public GetAttributeIndices(int[] attributeIndices) : void
- public SetAttributeIndex(int index, int value) : void
- public GetErrors(double[] err) : void
- public GetHashCode() : int
- public Equals(object obj) : bool
- public Equals(Triangle other) : bool

#### class UVChannels

- Archivo: `UVChannels.cs` (61 líneas)
- Usa: `MeshUtils`, `ResizableArray`

**Propiedades:**

- Data : TVec[][]

**Campos:**

- UVChannelCount : int
- channels : ResizableArray<TVec>[]
- channelsData : TVec[][]

**Métodos:**

- public UVChannels() : (constructor)
- public Resize(int capacity, bool trimExess = false) : void

#### struct Vertex

- Archivo: `Vertex.cs` (45 líneas)
- Implementa: `IEquatable`
- Usa: `Vector3d`, `SymmetricMatrix`

**Campos:**

- index : int
- p : Vector3d
- tstart : int
- tcount : int
- q : SymmetricMatrix
- borderEdge : bool
- uvSeamEdge : bool
- uvFoldoverEdge : bool

**Métodos:**

- public Vertex(int index, Vector3d p) : (constructor)
- public GetHashCode() : int
- public Equals(object obj) : bool
- public Equals(Vertex other) : bool

