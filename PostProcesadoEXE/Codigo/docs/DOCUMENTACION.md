# Convertidor de Geometrías Revit — Documentación Técnica Principal

**Intención Principal:** Extracción automatizada y optimización de geometría BIM compleja desde Autodesk Revit hacia formatos ligeros e interactivos en WebGL (.obj, .glb, .tbv), garantizando la integridad geométrica y eficiencia de renderizado en plataformas web y dispositivos móviles.

---

## 📥 Entradas y Salidas Globales (IN / OUT)

El flujo de información general del proyecto opera de la siguiente manera:

```
[Autodesk Revit 2021]
         │
         ▼ (Macro / C# Exporter)
    export.bin (Formato binario secuencial v3 "TBT2")  <--- [ENTRADA (IN)]
         │
         ▼ (ConvertidorGeometrias.exe - Pipeline .NET 4.8)
 ┌───────┼──────────────────────────────┬──────────────────────────────┐
 │       │                              │                              │
 ▼       ▼                              ▼                              ▼
export.obj    export_optimizado.glb    export.tbv             export_reporte.txt
(Texto)       (glTF Instanciado)       (Binario Visor "TBTV") (Métricas y Errores)
                                        │
                                        ▼ [SALIDAS (OUT)]
                                   [index.html] (Visor WebGL en Tres.js)
```

---

## 🗺️ Mapa de la Documentación

La documentación del proyecto se encuentra dividida en varios módulos especializados para facilitar su comprensión e implementación:

1. 📥 **[Exportador de Revit (C# y Macro)](file:///c:/.TBT/Proyectos/_Revit_EXE_Geometrias/DOCUMENTACION_MACRO.md)**:
   * Detalla la macro ejecutada en Revit 2021 para extraer la geometría BIM.
   * Explica los filtros por categoría, la resolución recursiva de sólidos anidados y el motor heurístico avanzado de color y transparencias para vidrios.
2. ⚙️ **[Pipeline de Conversión y Optimización](file:///c:/.TBT/Proyectos/_Revit_EXE_Geometrias/DOCUMENTACION_PIPELINE.md)**:
   * Explica los pasos de procesamiento de la herramienta de consola (.NET Framework 4.8).
   * Describe la soldadura de vértices (`weld`), limpieza de degenerados, decimado selectivo con filtros protectores, cálculo de normales y el motor de auto-reparación geométrica.
3. 💾 **[Especificación de Formatos de Archivo](file:///c:/.TBT/Proyectos/_Revit_EXE_Geometrias/DOCUMENTACION_FORMATOS.md)**:
   * Desglose completo a nivel de bytes de `export.bin` (v1, v2 y v3 "TBT2") y el formato de visor súper-ligero `.tbv` ("TBTV").
   * Detalles sobre la exportación optimizada de glTF/GLB e instanciación de mallas compartidas.
4. 📐 **[Algoritmo de Decimación QEM](file:///c:/.TBT/Proyectos/_Revit_EXE_Geometrias/DOCUMENTACION_ALGORITMO.md)**:
   * Explicación matemática de Quadric Error Metrics (QEM) y la contracción de aristas.
   * Justificación técnica de las directivas y parámetros aplicados (`PreserveBorders`, `PreserveFoldovers`, `EnableSmartLink`).
5. 🖥️ **[Visor Web WebGL (index.html y test.js)](file:///c:/.TBT/Proyectos/_Revit_EXE_Geometrias/DOCUMENTACION_HTML.md)**:
   * Detalla la arquitectura de visualización en cliente web con Three.js.
   * Explica el renderizado instanciado, la soldadura por shaders para rellenos de planos de corte, el culling en CPU, minimapa e interacciones táctiles y de ratón, junto con la suite de Puppeteer.

---

## 🏗️ Arquitectura y Estructura del Repositorio

El repositorio está organizado de la siguiente manera:

```
.
├── export.bin                          # Entrada binaria (generada por la macro en Revit)
├── export.obj                          # Malla OBJ tradicional de ejemplo
├── export_optimizado.glb               # Salida GLB de ejemplo (instanciada y optimizada)
├── export.tbv                          # Salida para el visor web (con índice espacial)
├── export_reporte.txt                  # Reporte con métricas, estadísticas y validación
├── ConvertidorGeometrias.exe           # Binario compilado y autocontenido de producción
└── codigo/
    ├── ConvertidorGeometrias.slnx          # Solución C# (.NET)
    ├── ConvertidorGeometrias/              # PROYECTO PRINCIPAL DE CONSOLA
    │   ├── Program.cs                      # Lógica del pipeline, validación y exportadores
    │   ├── ConvertidorGeometrias.csproj     # Configuración de compilación .NET 4.8
    │   ├── FodyWeavers.xml/.xsd             # Configuración de Costura.Fody para embeber DLLs
    │   └── MeshDecimatorLib/                # Librería de decimación QEM adaptada al proyecto
    ├── Revit/                              # EXPORTADOR DE REVIT
    │   └── ExportarGeometria.cs             # Código fuente de producción para Revit 2021
    ├── MacroRevit.txt                      # Macro de referencia para Revit (mismo motor)
    ├── temp_decimator/                     # Código de referencia original (Whinarn/UnityMeshDecimator)
    └── temp_simplifier/                    # Código de referencia original (UnityMeshSimplifier)
```

> [!NOTE]
> Las carpetas `temp_decimator` y `temp_simplifier` son clones de repositorios de terceros utilizados como base de investigación al migrar el código. No forman parte del proceso de compilación (`ConvertidorGeometrias.slnx` solo referencia al proyecto de consola y su librería local `MeshDecimatorLib`).

---

## 🚀 Uso Rápido de la Herramienta

El convertidor se ejecuta como una herramienta de consola independiente:

```powershell
ConvertidorGeometrias.exe [ruta\a\archivo.bin] [--nopause]
```

### Parámetros:
* **`[ruta\a\archivo.bin]`** *(Opcional)*: Ruta completa al archivo binario generado por Revit. Si no se especifica, por defecto busca `C:\.TBT\Proyectos\_Revit_EXE_Geometrias\export.bin`.
* **`--nopause`** *(Opcional)*: Evita pausar la consola al finalizar el proceso (útil para automatización mediante scripts de integración continua).

### Salida:
El proceso genera tres archivos en el mismo directorio donde se encuentre el archivo `.bin` de entrada:
1. `.obj`: Modelo tridimensional de texto tradicional con su respectivo sombreado por grupos.
2. `_optimizado.glb`: Modelo estándar glTF binario con deduplicación geométrica de instancias repetidas y metadatos BIM incrustados.
3. `.tbv`: Archivo optimizado para el visor web propietario, incluyendo deduplicación geométrica y un índice espacial AABB para renders eficientes.
4. `_reporte.txt`: Resumen analítico completo del procesamiento de mallas (métricas de reducción, listas de errores geométricos detectados y el resultado del motor de auto-reparación).
