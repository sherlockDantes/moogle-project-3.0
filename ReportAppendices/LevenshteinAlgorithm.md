# LevenshteinDistance

De manera informal, la distancia de Levenshtein entre dos palabras es el número mínimo de ediciones de un solo carácter (inserciones, eliminaciones o sustituciones) necesarias para cambiar una palabra por otra.

```cs
private static int GetLevenshteinDistance(string word1, string word2)
{
    if (word1.Length == 0)
    {
        return word2.Length;
    }
    else if (word2.Length == 0)
    {
        return word1.Length;
    }
    else if (word1[0] == word2[0])
    {
        return GetLevenshteinDistance(word1.Substring(1), word2.Substring(1));
    }
    else
    {
        return 1 + GetLevenshteinDistance(word1.Substring(1), word2.Substring(1));
    }
}
```

`GetLevenshteinDistance` es un método recursivo que simplemente determina cuantas ediciones deben realizarse para transformar una palabra en la otra.

## Bibliografía

Wikipedia: [](https://en.wikipedia.org/wiki/Levenshtein_distance#:~:text=Informally%2C%20the%20Levenshtein%20distance%20between,considered%20this%20distance%20in%201965.).