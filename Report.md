# Moogle!

> Informe del Proyecto de Programación I.
> Facultad de Matemática y Computación - Universidad de La Habana.
> Cursos 2022 - 2023.
> Estudiante: Javier Lima García

Moogle! es una aplicación cuyo propósito es buscar inteligentemente un texto en un conjunto de documentos.

Es una aplicación web, desarrollada con tecnología .NET Core 6.0, específicamente usando Blazor como *framework* web para la interfaz gráfica, y en el lenguaje C#.
La aplicación está dividida en dos componentes fundamentales:

- `MoogleServer` es un servidor web que renderiza la interfaz gráfica y sirve los resultados.
- `MoogleEngine` es una biblioteca de clases donde está implementada la lógica del algoritmo de búsqueda.

## Arquitectura básica y flujo de la aplicación

### 1- Procesamiento de los documentos

Utilizando la clase estática de `MoogleEngine` llamada `VectorSpaceModel` son procesados todos los documentos durante la carga del servidor en `MoogleServer`. Específicamente son cargados un total de 4 diccionarios: 
- *corpus*: Almacena la frecuencia de todos los términos por documento.
- *TF_IDF_Matrix*: Almacena los valores de TF-IDF de todos los términos por documento (se podrían considerar la representación vectorial de cada documento).
- *positionTracker*: Almacena las posiciones que tienen cada término en cada documento.
- *lineTracker*: Almacena por cada término de cada documento una línea que lo contenga.

Además de la carga de los diccionarios, se produce la carga del array *powers* encargado de almacenar la suma de los cuadrados de los elementos de cada representación vectorial de los documentos.

Con el procesamiento de los documentos durante la carga del servidor se consigue una enorme reducción del tiempo de búsqueda; al solo tener que procesar, añadir y eliminar la query con cada búsqueda realizada por el usuario, como se verá más adelante.

### 2- Método Query

Tras el procesamiento de los datos durante la carga del servidor, los diccionarios resultantes, junto con el array *powers*, son utilizados como argumentos del método `Query` de la clase `Moogle`. Esta clase con la ayuda del resto de `MoogleEngine` es la principal responsable del correcto funcionamiento de la aplicación.

La estructura del método `Query` está bien definida por 4 secciones de código, podría decirse, que llamaremos en este informe (para facilitar la comprensión) como: *trabajando con los vectores*, *trabajando con los operadores*, *trabajando con los snippets*, *trabajando con las sugerencias*.

**Nota*: Vale recalcar nuevamente que el funcionamiento del método `Query` de la clase `Moogle` depende del resto de clases existentes en `MoogleEngine`. Para comprender el funcionamiento de estas clases, por favor, seguir el siguiente enlace: ["MoogleEngine_Classes"](ReportAppendices\MoogleEngine_Classes.md).

#### 2.1- Trabajando con los vectores:

**Nota*: Antes de entrar en detalles de esta sección del método `Query` es necesario aclarar que todo el proceso de vectorización de los documentos, el cálculo del TF-IDF, etc, no es más que el resultado de aplicar el *Modelo de Espacio Vectorial* a los documentos y a la query. Para más informacióm sobre el *Modelo de Espacio Vectorial*, por favor, seguir el siguiente enlace: ["VectorSpaceModel"](ReportAppendices\VectorSpaceModel.md)

Esta sección es la encargada, como su nombre indica, de trabajar con la representación vectorial de los documentos, con el objetivo de establecer una jerarquía de similitud entre estos y la query.

Utilizando métodos de la clase estática `VectorSpaceModel` realiza lo siguiente:
- Resetear los diccionarios *corpus*, *TF_IDF_Matrix*, y el array, *powers* con el objetivo de que los datos introducidos por una búsqueda anterior no altere los resultados.
- Actualizar los diccionarios *corpus*, *TF_IDF_Matrix*, y el array, *powers* con los datos de la query para calcular correctamente la representación vectorial de esta.
- Calcular la similitud (de acuerdo a la similitud del coseno) entre todos las representaciones vectoriales de los documentos y la query.

#### 2.2- Trabajando con los operadores

Esta sección, utilizando métodos de la clase estática `OperatorsManager` realiza lo siguiente:
- Verificar la validez del uso de los operadores por parte del usuario.
- Clasificar y añadir los términos en grupos, de acuerdo a los operadores bajo los que se encuentran influenciados.
- Se procesa el score de los documentos de acuerdo a los operadores.

#### 2.3- Trabajando con los snippets

Esta sección, utilizando métodos de la clase estática `SnippetManager` realiza lo siguiente:
- Añadir el snippet correspondiente a cada *SearchItem*, utilizando el diccionario *lineTracker*.
- Resaltar las palabras de la query que aparecen en el snippet.

#### 2.4- Trabajando con las sugerencias

**Nota*: Para obtener las sugerencias se ha utilizado el *Algoritmo de Levenshtein*. Para más información, por favor, seguir el siguiente enlace: ["LevenshteinAlgorithm"](ReportAppendices\LevenshteinAlgorithm.md).

Esta sección, utilizando métodos de la clase estática `SuggestionManager` realiza lo siguiente:
- Determinar que palabras de la query no aparecen en los documentos.
- Obtener la sugerencia más acertada utilizando el *Algoritmo de Levenshtein*.

## Funcionalidades implementadas

Las principales funcionalidades utilizadas son: las sugerencias y los operadores de búsqueda.

### 1- Sugerencias

El mecanismo utilizado para brindar las sugerencias al usuario cuando ha introducido una palabra que no aparece en los documentos, está contenido en la clase `SuggestionManager` de `MoogleEngine`, fundamentalmente en el método `GetSuggestion`. Una explicación detallada de estos está en el siguiente enlace: ["MoogleEngine_Classes"](ReportAppendices\MoogleEngine_Classes.md).

### 2- Operadores

La lógica detrás de los operadores de búsqueda es la siguiente:

- Un símbolo `!` delante de una palabra (e.j., `"algoritmos de búsqueda !ordenación"`) indica que esa palabra **no debe aparecer** en ningún documento que sea devuelto.
- Un símbolo `^` delante de una palabra (e.j., `"algoritmos de ^ordenación"`) indica que esa palabra **tiene que aparecer** en cualquier documento que sea devuelto.
- Un símbolo `~` entre dos o más términos indica que esos términos deben **aparecer cerca**, o sea, que mientras más cercanos estén en el documento mayor será la relevancia. Por ejemplo, para la búsqueda `"algoritmos ~ ordenación"`, mientras más cerca están las palabras `"algoritmo"` y `"ordenación"`, más alto debe ser el `score` de ese documento.
- Cualquier cantidad de símbolos `*` delante de un término indican que ese término es más importante, por lo que su influencia en el `score` debe ser mayor que la tendría normalmente (este efecto será acumulativo por cada `*`, por ejemplo `"algoritmos de **ordenación"` indica que la palabra `"ordenación"` tiene dos veces más prioridad que `"algoritmos"`).

El mecanismo detrás del funcionamiento de los operadores está contenido en la clase `OperatorManager` de `MoogleEngine`, fundamentalmente en los métodos `ManageAbsoluteOperators` y `ManageCloseness`. Una explicación detallada de estos está en el siguiente enlace: ["MoogleEngine_Classes"](ReportAppendices\MoogleEngine_Classes.md).