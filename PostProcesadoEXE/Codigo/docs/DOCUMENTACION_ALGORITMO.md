# Algoritmo de Decimación de Mallas (QEM)

Este documento explica las bases matemáticas y la configuración técnica del motor de decimación ubicado en la biblioteca [MeshDecimatorLib](file:///c:/.TBT/Proyectos/_Revit_EXE_Geometrias/codigo/ConvertidorGeometrias/MeshDecimatorLib/).

---

## Origen de la Biblioteca
`MeshDecimatorLib` es una copia adaptada ("vendorizada") e integrada en el proyecto del algoritmo de decimado rápido por cuadrículas de error desarrollado por **Mattias Edlund** (`UnityMeshSimplifier` / `UnityMeshDecimator`). Es un puerto en C# puro optimizado para entornos .NET.

---

## Fundamento Matemático: Quadric Error Metrics (QEM)

El algoritmo se basa en el artículo clásico de Michael Garland y Paul Heckbert: *"Surface Simplification Using Quadric Error Metrics"* (SIGGRAPH 97). Su objetivo es reducir el número de triángulos de una malla minimizando la desviación visual de la forma 3D original.

### 1. Representación del Error en un Vértice
Para cada plano de un triángulo que se encuentra en un vértice $v$, podemos definir la distancia de cualquier punto de la malla a dicho plano. La suma de las distancias al cuadrado desde un punto $v = [x, y, z, 1]^T$ a todos los planos incidentes se expresa mediante una forma cuadrática:

$$\Delta(v) = v^T Q v$$

Donde $Q$ es una matriz simétrica de $4 \times 4$ llamada **Cuádrica de Error** (Error Quadric). Para un plano definido por la ecuación $p^T v = 0$ (donde $p = [a, b, c, d]^T$ es el vector normal del plano y su distancia al origen):

$$Q = \sum (p p^T) = \sum \begin{pmatrix} a^2 & ab & ac & ad \\ ab & b^2 & bc & bd \\ ac & bc & c^2 & cd \\ ad & bd & cd & d^2 \end{pmatrix}$$

### 2. Contracción de Aristas (Edge Collapse)
La operación básica de simplificación consiste en tomar una arista que conecta dos vértices $(v_1, v_2)$ y contraerla en un único nuevo vértice $v_{new}$, eliminando los triángulos adyacentes a la arista.

```
       v1                  
      /  \                 
     /    \                
    /      \       ==>      v_new
   /  Arista\              /     \
  /    v1-v2 \            /       \
 v2-----------             -------
```

### 3. Selección del Vértice Óptimo y Costo
Cuando se contrae la arista, la nueva cuádrica de error del vértice es la suma de las cuádricas de sus predecesores:
$$Q_{new} = Q_1 + Q_2$$

El algoritmo busca el punto espacial $v_{new}$ que minimice el error $v_{new}^T Q_{new} v_{new}$. Esto se calcula resolviendo el sistema lineal derivando la ecuación respecto a sus coordenadas espaciales:

$$\nabla(v_{new}^T Q_{new} v_{new}) = 0$$

Lo que equivale a resolver:

$$\begin{pmatrix} q_{11} & q_{12} & q_{13} & q_{14} \\ q_{12} & q_{22} & q_{23} & q_{24} \\ q_{13} & q_{23} & q_{33} & q_{34} \\ 0 & 0 & 0 & 1 \end{pmatrix} v_{new} = \begin{pmatrix} 0 \\ 0 \\ 0 \\ 1 \end{pmatrix}$$

Si el determinante de la matriz es muy cercano a cero (la matriz no es invertible, lo cual ocurre en planos perfectos o regiones coplanares), el algoritmo evalúa los puntos extremos $v_1$, $v_2$ y el punto medio $(v_1+v_2)/2$, eligiendo el que arroje menor costo.

### 4. Cola de Prioridad
Todas las aristas de la malla se ingresan en una cola de prioridad ordenadas por su costo de contracción (error mínimo). En cada iteración:
1. Se extrae la arista con menor costo.
2. Se realiza la contracción en $v_{new}$.
3. Se actualizan las posiciones y los costos de las aristas adyacentes en la cola de prioridad.

---

## Configuración y Parámetros en el Conversor

En [Program.cs](file:///c:/.TBT/Proyectos/_Revit_EXE_Geometrias/codigo/ConvertidorGeometrias/Program.cs#L521-L524), el algoritmo se parametriza de la siguiente forma:

```csharp
var algorithm = new MeshDecimator.Algorithms.FastQuadricMeshSimplification();
algorithm.PreserveBorders = true;
algorithm.PreserveSeams = true;
algorithm.PreserveFoldovers = true;
algorithm.EnableSmartLink = false;
```

### Parámetros Explicados:

* **`PreserveBorders = true`**:
  * **Qué hace**: Identifica bordes abiertos en la malla (aristas que pertenecen a un solo triángulo, no compartido). Multiplica el peso de las cuádricas de error de estos bordes por una constante penalizadora alta.
  * **Por qué es necesario**: Evita que los contornos exteriores de piezas planas (paneles, chapas, perfiles abiertos) se encojan o deformen hacia adentro, manteniendo la silueta del objeto original.

* **`PreserveSeams = true`**:
  * **Qué hace**: Protege las uniones donde colindan múltiples sub-mallas con diferentes ID de material o costuras de coordenadas UV.
  * **Por qué es necesario**: Impide que el decimado deforme las fronteras texturizadas o mapeadas de forma diferente, previniendo microgaps o costuras visibles en las texturas.

* **`PreserveFoldovers = true`**:
  * **Qué hace**: Antes de proceder con la contracción de una arista, simula la posición de los nuevos triángulos y calcula sus normales. Si el ángulo de la normal de algún triángulo adyacente cambia más de un límite tolerado (lo que significa que el triángulo se "volteó" o invirtió su cara orientándose hacia adentro), la operación se cancela y se salta a la siguiente arista de la cola.
  * **Por qué es necesario**: Previene la creación de geometría superpuesta o invertida. Los triángulos invertidos causan sombras negras ("z-fighting" y errores de oclusión ambiental) extremadamente feos en motores gráficos como Unity.

* **`EnableSmartLink = false`**:
  * **Qué hace**: Desactiva la vinculación inteligente de vértices distantes que no comparten aristas pero están muy cerca en el espacio tridimensional.
  * **Por qué es necesario**: Si se activa en modelos BIM, el algoritmo podría soldar caras opuestas de muros delgados, o unir tuberías paralelas muy juntas. Al mantenerlo en `false`, garantizamos que las piezas desconectadas conserven su separación física e integridad geométrica sin crear "puentes" no deseados.
