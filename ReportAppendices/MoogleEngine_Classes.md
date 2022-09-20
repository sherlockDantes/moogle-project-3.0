# MoogleEngine_Classes

## 1- VectorSpaceModel.cs

La clase estática `VectorSpaceModel` es la encargada de procesar los documentos para aplicar sobre estos el [*Modelo de Espacio Vectorial*](VectorSpaceModel.md).

### 1.1- ProcessingFiles()

Este método se encarga de procesar los documentos para cargar los diccionarios *corpus* y *positionTracker*.

### 1.2- GetLineTracker()

Este método se encarga de procesar los documentos para cargar el diccionario *lineTracker*.

### 1.3- GetTF_IDF_Matrix()

Utilizando el diccionario *corpus*, este método procesa las frecuencias de las palabras para calcular sus valores de TF_IDF, cargando el diccionario *TF_IDF_Matrix*, así como el array *powers*.

Utiliza el método privado GetIDF para calcular el IDF de cada palabra.

### 1.4- GetCosineSimilarity()

Dando por sentado que la query es el último elemento del diccionario *TF_IDF_Matrix*, este método calcula la similitud del coseno entre la query con el resto de los documentos. Devolviendo una lista que contiene a todos los *searchItem*s con un score mayor que 0.

Utiliza el método privado GetCosine para calcular el coseno del ángulo que existe entre los vectores.

### 1.5- ResetSpaceForQuery()

Para prevenir errores en los resultados de búsqueda por posibles valores de una query anterior, este método resetea todos los valores reservados para la query.

### 1.6- AddQueryToCorpus()

Actualiza el diccionario *corpus* de acuerdo a las palabras presentes en la query.

### 1.7- AddQueryToTF_IDF_Matrix()

Actualiza diccionario *TF_IDF_Matrix* y el array *powers* de acuerdo al corpus, que se supone, ha sido previamente actualizado.

### 1.8- GetCosine()

Este método calcula el coseno del ángulo entre los documentos.

### 1.9- GetIDF()

Este método calcula el IDF de cada palabra.

## 2- OperatorsManager.cs

La clase estática `OperatorsManager` es a encargada de manejar todo lo relacionado con los operadores.

### 2.1- ContainsLetter()

Este método determina si un string contiene o no una letra del abecedario en minúscula.

### 2.2- RemoveOperator()

Este método elimina los operadores de un string.

### 2.3- GetStarOperatorIncrease()

Este método incrementa el porcentaje de incremento del score de un string de acuerdo al número de "*" que presente.

### 2.4- ManageAbsoluteOperators()

Utilizando entre sus parámetros las listas que contienen los *términos prohibidos*(!), los *indispensables*(^) y los *beneficiados*(\*). Este método elimina aquellos *searchItem*s que no contengan a los términos indispensables(^) y a los que contengan a los prohibidos(!), e incrementa el score de los beneficiados.

### 2.5- ManageCloseness()

Determina los documentos que presentan las *palabras cercanas*(~) y de acuerdo a la cercanía de estas palabras los organiza y modifica su score.

### 2.6- GetCloseness()

Utilizando dos listas que contienen posiciones, determina cuál es la menor distancia entre estas.

## 3- SnippetManager.cs

La clase estática `SnippetManager` es la encargada de manejar todo lo relacionado con los snippets.

### 3.1- GetSnippet()

Este método responde en base a los operadores:
- Si está presente el operador "^", la palabra bajo su influencia aparecerá en el snippet.
- Si está presente el operador "*" y la palabra bajo su influencia está en el documento aparecerá en el snippet.
- Si está presente el operador "~" y las palabras bajo su influencia está en el documento aparecerán el snippet.
- Si ninguno de estos casos se cumple las palabras de la query que aparezcan en el documento, aparecerán en el snippet.

Hay que tener en cuenta que las líneas donde aparecen las palabras de cada documento están almacenadas en el diccionario *lineTracker*. Razón por la cual es un argumento de este método.

### 3.2- HighlightTerms()

Este método resalta las palabras de la query que aparezcan en los snippets.

### 3.3- Highlight()

Este método resalta una sola palabra que aparezca en un determinado snippet.

## 4- SuggestionManager.cs

La clase estática `SuggestionManager` es la encargada de manejar todo lo relacionado con las sugerencias.

### 4.1- struct Suggestion

Este struct almacena una palabra con su score (distancia de Levenshtein con respecto a un término desconocido de la query).

### 4.2- GetSuggestion()

Este método devuelve la palabra presente en el corpus que menor distancia de Levenshtein presenta con respecto a la palabra desconocida de la query.

### 4.3- GetLevenshtenDistance()

Este método calcula la [distancia de Levenshtein](LevenshteinAlgorithm.md).

## 5- SearchItem.cs

Cada `SearchItem` recibe 3 argumentos en su constructor: `title`, `snippet` y `score`. El parámetro `title` es el título del documento (el nombre del archivo de texto correspondiente). El parámetro `snippet` contiene una porción del documento donde se encontró el contenido del `query`. El parámetro `score` tiene un valor de tipo `float` que es más alto mientras más relevante sea este item.

Utilizando la herencia se convirtió a *SearchItem* en un `IComparable<SearchItem>`, con la respectiva adición del método:

```cs
public int CompareTo(SearchItem other)
{
    if (Score < other.Score) return 1;
    else if (Score > other.Score) return -1;
    else return 0;
}
```
Todo esto con el objetivo de poder utilizar el método Sort() en una lista de *searchItem*s.

## 6- SearchResult.cs

El tipo `SearchResult` recibe en su constructor dos argumentos: `items` y `suggestion`. El parámetro `items` es un array de objetos de tipo `SearchItem`. Cada uno de estos objetos representa un posible documento que coincide al menos parcialmente con la consulta en `query`.

## 7- ClosenessData.cs

El tipo `ClosenessData` recibe en su constructor dos argumentos: `fileName` y `closeness`. El parámetro `fileName` es un string que representa el nombre de un documento. El parámetro closeness es un int que representa la menor distancia que presentan dos determinadas palabras en es documento.

Utilizando la herencia se convirtió a *ClosenessData* en un `IComparable<ClosenessData>`, con la respectiva adición del método:

```cs
public int CompareTo(SearchItem other)
{
    if (Score < other.Score) return 1;
    else if (Score > other.Score) return -1;
    else return 0;
}
```
Todo esto con el objetivo de poder utilizar el método Sort() en una lista de *ClosenessData*s.

## 8- Tools.cs

La clase `Tools` es una clase auxiliar de las demás clases, aportando métodos diversos a cada una de ellas.

### 8.1- Tokenize()

Este método devuelve un array con todos los términos de la query, sin símbolos ni espacios, etc.

### 8.2- TokenizeQuery()

Este método se comporta de manera muy parecida al anterior, con la única diferencia que mantiene a los operadores entre los elementos del array que devuelve.

### 8.3- RemoveAccent()

Elimina los acentos.

### 8.4- ReplaceAllTerms()

Remplaza los términos desconocidos por las sugerencias.

### 8.5- GetFinalSuggestion()

Construye la sugerencia final.