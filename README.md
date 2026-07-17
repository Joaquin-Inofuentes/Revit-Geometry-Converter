# _Revit_EXE_Geometrias

- **IN (Entrada):**
  - Archivos de modelo de Revit (`.rvt`) procesados por la macro `RevitMacro/ExportarGeometria.cs` corriendo dentro de Revit 2021.
  - Archivo binario de extracción de geometría `export.bin` (formato TBT2 / v3) generado por la macro y depositado para el post-procesador.
  - Parámetros de decimación y exportación (calidad QEM, factor de reducción, soldadura de vértices, etc.).
- **OUT (Salida):**
  - Malla tridimensional OBJ clásica (`export.obj`).
  - Archivo glTF binario con instanciación deduplicada de mallas compartidas (`export_optimizado.glb`).
  - Archivo binario indexado con índice espacial AABB para visualización rápida en web (`export.tbv`).
  - Reporte analítico de decimación y errores (`export_reporte.txt`).
  - Vista interactiva servida por `WebEjemplo/index.html` (Three.js r128 + OrbitControls) consumiendo el `.tbv`.
- **Intención:** Extraer y optimizar la geometría 3D de modelos de Revit a formatos WebGL ligeros y eficientes (`.obj`, `.glb`, `.tbv`) aplicando **decimación QEM (Quadric Error Metrics)**, soldadura de vértices, eliminación de geometrías degeneradas e instanciación de mallas compartidas, para que se rendericen de forma óptima en navegadores web y dispositivos móviles.

---

## Estructura del repositorio

```
_Revit_EXE_Geometrias/
├── RevitMacro/
│   └── ExportarGeometria.cs        # Macro de Revit 2021: exporta el modelo a binario TBT2
│
├── PostProcesadoEXE/
│   └── Codigo/                     # ⭐ EXE de post-proceso (C# .NET)
│       ├── ConvertidorGeometrias/  # Proyecto principal del conversor
│       │   ├── MeshDecimatorLib/   # Librería de decimación (QEM, FastQuadric, etc.)
│       │   └── Program.cs          # Entry point
│       ├── docs/                   # Documentación técnica del conversor
│       └── README.md               # Detalle profundo de algoritmos y formatos
│
├── WebEjemplo/                     # ⭐ Visor web de ejemplo (Three.js)
│   ├── index.html
│   ├── src/viewer.html
│   ├── vendor/three.min.js
│   ├── vendor/OrbitControls.js
│   └── scripts/                    # Utilidades Python/Node de test (selenium, screenshot)
│
├── _Docs_IA/                       # Docs autogeneradas por el _Analizador
└── README.md                       # Este archivo
```

## Pieza por pieza

| Carpeta | Qué hace | Tamaño aprox. |
|---|---|---|
| `RevitMacro/` | Corre dentro de Revit 2021, recorre los elementos del `.rvt` y vuelca la geometría a `export.bin` (formato TBT2). | 1 archivo `.cs` (177 líneas). |
| `PostProcesadoEXE/Codigo/` | Lee `export.bin`, aplica decimación QEM + simplificación, escribe `.obj` / `.glb` / `.tbv` + reporte. | 78 archivos, 93 clases, 790 métodos, ~28.5k líneas. |
| `WebEjemplo/` | Visor HTML/JS estático con Three.js que carga el `.tbv` y permite orbitar/zoom. | 1 `index.html` + 1 `viewer.html` + vendor + scripts de test. |
| `_Docs_IA/` | Documentación autogenerada (diagramas, CSV, reportes) por `_Otros_EXE_Analizador`. | Carpeta de output. |

## Pipeline completo

```
.rvt ─► RevitMacro/ExportarGeometria.cs ─► export.bin (TBT2/v3)
                                              │
                                              ▼
                                PostProcesadoEXE/Codigo/
                                              │
                          ┌───────────────────┼───────────────────┐
                          ▼                   ▼                   ▼
                     export.obj         export_optimizado.glb    export.tbv (AABB)
                                                                       │
                                                                       ▼
                                                       WebEjemplo/index.html (Three.js)
```

## Dependencias clave del conversor

- `MeshDecimatorLib` (incluida en el repo): `MeshSimplifier.cs` (2322 líneas), `FastQuadricMeshSimplification.cs` (1540 líneas), `Mesh.cs` (955 líneas), `ObjMesh.cs` (891 líneas), `MeshDecimatorUtility.cs` (924 líneas), `MeshUtils.cs` (473 líneas).
- Vector/matrix primitives: `Vector2/3/4` + variantes `i`/`d` (single/double/int) + `SymmetricMatrix`, `MathHelper`, `Triangle`, `Vertex`, `BorderVertex`, `Ref`, `UVChannels`, `BlendShape`, `BoneWeight`.
- Fody (post-compilador IL weaving) — ver `FodyWeavers.xml`.

## Para profundizar

El detalle fino de algoritmos, formatos de archivo y notas de implementación está en:

- [`PostProcesadoEXE/Codigo/README.md`](./PostProcesadoEXE/Codigo/README.md) — descripción del flujo del conversor.
- [`PostProcesadoEXE/Codigo/docs/DOCUMENTACION.md`](./PostProcesadoEXE/Codigo/docs/DOCUMENTACION.md) — manual técnico.
- [`PostProcesadoEXE/Codigo/docs/DOCUMENTACION_ALGORITMO.md`](./PostProcesadoEXE/Codigo/docs/DOCUMENTACION_ALGORITMO.md) — detalle de QEM y decimación.
- [`PostProcesadoEXE/Codigo/docs/DOCUMENTACION_FORMATOS.md`](./PostProcesadoEXE/Codigo/docs/DOCUMENTACION_FORMATOS.md) — formato TBT2/v3.
- [`PostProcesadoEXE/Codigo/docs/DOCUMENTACION_HTML.md`](./PostProcesadoEXE/Codigo/docs/DOCUMENTACION_HTML.md) — visor web.
