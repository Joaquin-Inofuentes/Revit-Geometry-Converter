# ConvertidorGeometrias — Documentación técnica

## Propósito
Herramienta de línea de comandos (.NET Framework 4.8) que convierte geometría exportada desde
Revit (formato binario `.bin` propio) en mallas optimizadas exportadas a `.obj` y `.glb`, listas
para usar en Unity u otros visores 3D. Pensada para modelos BIM pesados: agrupa por elemento,
suelda vértices, decima con Quadric Error Metrics (QEM) y recalcula normales.

## Estructura del repo
```
.
├── export.bin                          # entrada binaria de ejemplo (salida de un exportador Revit externo)
├── export.obj / export_optimizado.glb  # salidas de ejemplo generadas por el conversor
├── ConvertidorGeometrias.exe           # build publicado (autocontenido, vía Costura.Fody)
└── codigo/
    ├── ConvertidorGeometrias.slnx          # solución
    ├── ConvertidorGeometrias/              # PROYECTO PRINCIPAL (el único que se compila/usa)
    │   ├── Program.cs                      # toda la lógica: lectura, decimado, export OBJ/GLB
    │   ├── ConvertidorGeometrias.csproj     # net48, exe, output a la raíz del repo
    │   ├── FodyWeavers.xml/.xsd             # Costura.Fody: embebe las DLLs de deps en el .exe
    │   └── MeshDecimatorLib/                # copia vendorizada (no NuGet) de MeshDecimator
    │       ├── Algorithms/
    │       │   ├── DecimationAlgorithm.cs           # clase base abstracta
    │       │   └── FastQuadricMeshSimplification.cs # algoritmo QEM (implementación real, 1539 líneas)
    │       ├── Mesh.cs, MeshDecimation.cs           # modelo de malla y punto de entrada del decimado
    │       ├── Math/ (Vector2/3/4, SymmetricMatrix, MathHelper)
    │       ├── Collections/ (ResizableArray, UVChannels)
    │       ├── BoneWeight.cs, Logging.cs, Loggers/ConsoleLogger.cs
    │       └── MeshDecimator.csproj                 # librería separada, referenciada por el .exe
    ├── temp_decimator/                     # fuente ORIGINAL de referencia (Whinarn/UnityMeshDecimator)
    │                                        #  — no se compila desde acá; MeshDecimatorLib es su copia adaptada
    └── temp_simplifier/                    # fuente ORIGINAL de referencia (UnityMeshSimplifier, paquete UPM)
                                             #  — tampoco se compila; queda como material de consulta
```

`temp_decimator` y `temp_simplifier` son clones de librerías de terceros usados como referencia al
portar el algoritmo de decimado a este proyecto. No participan del build (`ConvertidorGeometrias.slnx`
solo referencia `ConvertidorGeometrias.csproj`, que a su vez referencia `MeshDecimatorLib`).

## Dependencias (NuGet)
- **geometry3Sharp** 1.0.324 — no usado directamente en `Program.cs` actualmente (posible remanente).
- **SharpGLTF.Toolkit** 1.0.0-alpha0031 — construcción y export de `.glb` (glTF binario).
- **Costura.Fody** 5.7.0 — embebe todas las DLLs de dependencias dentro del `.exe` final; el target
  `CleanUpOutputs` borra `.dll/.pdb/.config/.deps.json` residuales tras el build para dejar un único exe.

Target framework: `net48`. El `.exe` se publica directamente a la raíz del repo
(`OutputPath = ..\..\`).

## Formato de entrada: `export.bin`
Binario secuencial (little-endian, `BinaryReader`). Dos versiones, auto-detectadas por el conversor:

**v3 ("TBT2")** — generado por `codigo/Revit/ExportarGeometria.cs` (Revit 2021). Empieza con
cabecera `[int32 magic 0x32544254 ("TBT2")][int32 version=3]`, luego un bloque por malla:

| Campo | Tipo | Descripción |
|---|---|---|
| ElementId | `int32` | Id del elemento en Revit |
| Guid | `string` (.NET length-prefixed) | UniqueId del elemento (45 chars) |
| MaterialId | `int32` | Id de material (-1 si no tiene) |
| CategoryId | `int32` | Id de BuiltInCategory (p.ej. Muros = -2000011); protege categorías del decimado |
| R, G, B, A | `byte × 4` | Color superficial simple del material; A<255 = vidrio/transparente |
| vertexCount | `int32` | Cantidad de vértices |
| Vertices | `vertexCount × (float,float,float)` | Posiciones X,Y,Z |
| triangleCount | `int32` | Cantidad de triángulos |
| Indices | `triangleCount × (uint32,uint32,uint32)` | Índices de triángulo |

**v2** — igual pero sin CategoryId. **v1 (legado)** — sin cabecera, color ni categoría; el GLB
usa colores aleatorios determinísticos por MaterialId. Las tres versiones se auto-detectan.

Al leer, cada vértice se rota de Z-up (Revit) a Y-up (GLTF/Unity):
`(x, y, z) → (x, z, -y)`.

## Exportador de Revit (`codigo/Revit/ExportarGeometria.cs`)
Clase estática para Revit 2021 (misma firma `ExportRawDump(Document, string)` que la versión
anterior — se reemplaza directo). Recorre categorías BIM (muros, pisos, techos, puertas,
ventanas, columnas, estructura, muebles, sanitarios), extrae sólidos recursivamente
(instancias anidadas), triangula cada cara y escribe el formato v2. El color RGB sale de
`Material.Color` y el alpha de `Material.Transparency` (vidrios translúcidos, piso de alpha 20
para que nunca desaparezcan). Fallbacks: material de la categoría → gris neutro. Cachea colores
por material y aísla elementos corruptos con try/catch por elemento.

## Pipeline (`Program.Main`)
Optimizado para Unity WebGL en móviles de gama baja servido desde Vercel: piezas separadas por
GUID, materiales compartidos (permite batching en Unity) y validación por pieza para que el
decimado nunca entregue geometría rota.

1. **Leer** `export.bin` → lista de `MeshData` crudas (una por "pieza" exportada de Revit).
   Se validan índices fuera de rango (exports corruptos se ignoran por triángulo).
2. **Optimizar** (`OptimizeMeshes`, con `PipelineStats` acumulando métricas):
   - Agrupa mallas por `Guid` (mismo elemento Revit) y las fusiona en una sola.
   - **Weld de vértices** (`WeldVertices`): grilla entera de 1 mm (clave `(long,long,long)`,
     sin strings) — repara microfisuras de topología antes del QEM.
   - **Elimina triángulos degenerados** (índices repetidos o área nula) y compacta vértices
     huérfanos (`RemoveDegenerateTriangles` + `CompactVertices`). Alimentar QEM con caras de
     área cero era la causa típica de mallas rotas.
   - **Categorías protegidas** (`CategoriasProtegidas` en `Program.cs`, editable): muros,
     suelos y techos NO se decimán nunca — ya vienen optimizados de Revit y son los que más
     feo se rompen. Solo reciben soldadura y limpieza (sin pérdida). Requiere bin v3.
   - **Guarda anti-cubos**: solo se decima si la pieza soldada tiene ≥16 vértices Y >20
     triángulos (un cubo soldado tiene 8 v / 12 t — margen de seguridad 2×). El decimado se
     aplica solo cuando es necesario, nunca a cubos/prismas/detalles chicos.
   - Calcula un `targetTriangles` dinámico CONSERVADOR (`ComputeTarget`):
     - `>10000` tris → 25% | `>2000` → 35% | `>500` → 50% | `>100` → 65% | resto → 80% (mín. 16).
     La banda de área del decimado es ±30%: cualquier plegado/agujero grande lo anula.
   - **Decima con validación** (`TryDecimate`), usando `FastQuadricMeshSimplification` con
     `PreserveBorders/Seams/Foldovers = true` y `EnableSmartLink = false`. El resultado se
     descarta (devuelve `null`) si:
     - contiene NaN/Infinity,
     - queda sin triángulos,
     - se sale de la bounding box original (>0.5% de la diagonal → spike),
     - su área superficial cambió más de ±50% (malla colapsada o con agujeros).
   - Si falla, **reintenta** con target conservador (50%); si vuelve a fallar, **fallback**:
     conserva la malla soldada sin decimar. Nunca se entrega una pieza rota.
   - Recalcula normales suavizadas ponderadas por área (`ComputeNormals`).
3. **Exportar**:
   - `ExportToObj`: un objeto (`o <Guid>` / `usemtl MAT_<MaterialId>`) por pieza.
   - `ExportToGlb` (SharpGLTF): una malla por pieza; **el nodo (GameObject en Unity) lleva el
     Guid de Revit como nombre**. Un `MaterialBuilder` compartido por `MaterialId` (`MAT_<id>`,
     color real del material en v2 con `alphaMode BLEND` para vidrios; aleatorio determinístico
     en v1) — compartir materiales habilita batching en Unity (menos draw calls).
     **Instanciado**: piezas que son copias trasladadas exactas de otra (ventanas, muebles
     repetidos) reusan la misma malla del GLB con un nodo trasladado — menos descarga y menos
     RAM en el móvil; cada nodo conserva su propio GUID.
4. **Validación + reparación** (`ValidarPiezas` / `RepararPiezas`): antes de exportar, chequeo
   independiente pieza por pieza contra el original soldado. Detecta piezas perdidas, vértices
   NaN/Infinity, spikes (vértices fuera de la caja original), colapsos (>50% de encogimiento)
   y área anómala (±40%). **Toda pieza rota se repara restaurando su geometría original
   soldada** y se re-valida. El reporte lista GUID, ElementId, problema y vectores implicados
   por consola y en `<nombre>_reporte.txt`.
5. **Estadísticas**: vértices/triángulos antes-después, degenerados eliminados, piezas
   protegidas sin decimar, reintentos, fallbacks, transparentes, instanciadas, tamaños, tiempo.

Salidas: `<nombre>.obj`, `<nombre>_optimizado.glb` y `<nombre>_reporte.txt` junto al `.bin`.

## Uso
```
ConvertidorGeometrias.exe [ruta\a\export.bin] [--nopause]
```
Sin argumentos usa `C:\.TBT\Proyectos\_Revit_EXE_Geometrias\export.bin` como default.
`--nopause` (o stdin redirigido) omite la pausa final — útil para automatización.

## Librería de decimado (`MeshDecimatorLib`)
Puerto a .NET Framework del algoritmo QEM (Garland & Heckbert) tal como lo implementa
`UnityMeshDecimator`/`UnityMeshSimplifier` de Mattias Edlund/Whinarn:
- `DecimationAlgorithm` — clase base con hooks `Initialize`/`DecimateMesh`.
- `FastQuadricMeshSimplification` — implementación completa: matrices simétricas de error por
  vértice, colapso de aristas por menor costo, protección de bordes/costuras/foldovers, soporte
  de múltiples canales UV, bone weights (rigging) y sub-mallas.
- `Mesh.cs` / `MeshDecimation.cs` — modelo de datos de malla intermedio y orquestación del proceso.

## Notas de historial (según commits recientes)
- Se corrigió `PreserveFoldovers` y se desactivó `SmartLink` para evitar mallas rotas/sombras negras.
- Se implementó weld de vértices + `PreserveBorders`/`SmartLink` para reparar topología de Revit
  antes de decimar.
- Se migró de un decimado simple a QEM completo con export limpio (normales + bounding box
  conservados).
- Versión estable anterior: separación por Guid + voxelización de 2cm (reemplazada por QEM).

## Puntos a considerar / posible deuda técnica
- `geometry3Sharp` está referenciado en el `.csproj` pero no se usa en `Program.cs` — candidato a
  remover si no se usa en otro lado.
- `temp_decimator/` y `temp_simplifier/` son carpetas de librerías de terceros completas (incluyen
  ejemplos de Unity, CI, tests) dejadas como referencia dentro del repo — no se compilan pero
  aumentan bastante el tamaño del repo.
- No hay tests automatizados ni CI para `ConvertidorGeometrias`.
