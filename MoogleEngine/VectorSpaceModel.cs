namespace MoogleEngine
{
    static class VectorSpaceModel
    {
        public static Dictionary<string, int[]> GetCorpus(string[] fileNames, out int indexOfQuery, string queryFileName)
        {
            Dictionary<string, int[]> corpus = new Dictionary<string, int[]>();

            indexOfQuery = 0;

            for (int i = 0; i < fileNames.Length; i++)
            {
                // Getting the index of query
                if (fileNames[i] == queryFileName)
                {
                    indexOfQuery = i;
                }

                // Cleaning up words
                string[] words = Tools.Tokenize(File.ReadAllText(fileNames[i]));

                for (int j = 0; j < words.Length; j++)
                {
                    int[] wordsFrecuency;
                    if (corpus.ContainsKey(words[j]))
                    {
                        wordsFrecuency = corpus[words[j]];
                        wordsFrecuency[i]++;
                    }
                    else
                    {
                        wordsFrecuency = new int[fileNames.Length];
                        wordsFrecuency[i]++;
                        corpus.Add(words[j], wordsFrecuency);
                    }
                }
            }

            return corpus;
        }
        public static Dictionary<string, float[]> Get_TF_IDF_Matrix(Dictionary<string, int[]> corpus, string[] fileNames, ref float[] powers)
        {
            Dictionary<string, float[]> TF_IDF_Matrix = new Dictionary<string, float[]>();

            int count = 0;
            foreach (var key in corpus)
            {
                // Getting IDF of each word
                float IDF = GetIDF(key.Value);

                for (int i = 0; i < key.Value.Length; i++)
                {
                    // Adding a file if it wasn't there
                    if (!TF_IDF_Matrix.ContainsKey(fileNames[i]))
                    {
                        TF_IDF_Matrix.Add(fileNames[i], new float[corpus.Count]);
                    }

                    // Getting tf_idf of the word in an specific file
                    float TF_IDF = key.Value[i] * IDF;

                    // Updating the tf_idf vector
                    TF_IDF_Matrix[fileNames[i]][count] = TF_IDF;

                    // Updating the sum of powers of each vector
                    powers[i] += (float)Math.Pow(TF_IDF, 2);
                }
                count++;
            }

            return TF_IDF_Matrix;
        }
        public static List<SearchItem> GetCosineSimilarity(Dictionary<string, float[]> TF_IDF_Matrix, string queryFileName, float[] powers, int indexOfQuery)
        {
            List<SearchItem> searchItems = new List<SearchItem>();

            int count = 0;
            foreach (var key in TF_IDF_Matrix)
            {
                // Removing query from comparisons
                if (key.Key == queryFileName)
                {
                    count++;
                    continue;
                }

                // Getting norms of each vector
                float norm1 = (float)Math.Sqrt(powers[count]);
                float norm2 = (float)Math.Sqrt(powers[indexOfQuery]);

                // Calculating cosine similarity between vectors
                float cosine = GetCosine(key.Value, norm1, TF_IDF_Matrix[queryFileName], norm2);

                // Removing vectors that aren't similar enough
                if (cosine < 0.006f)
                {
                    count++;
                    continue;
                }

                searchItems.Add(new SearchItem(Path.GetFileName(key.Key), "", cosine));

                count++;
            }

            searchItems.Sort();

            return searchItems;
        }
        private static float GetCosine(float[] firstVector, float norm1, float[] secondVector, float norm2)
        {
            float result = 0;
            for (int i = 0; i < firstVector.Length; i++)
            {
                
                // Normalizing vectors before multiplying them
                float TF_IDF_Normalized1 = firstVector[i] * (1 / norm1);
                float TF_IDF_Normalized2 = secondVector[i] * (1 / norm2);

                // Getting the dot product of vectors
                result += TF_IDF_Normalized1 * TF_IDF_Normalized2;
            }

            return result;
        }
        private static float GetIDF(int[] termFrecuency)
        {
            float IDF = 0;
            for (int i = 0; i < termFrecuency.Length; i++)
            {
                // Getting the amount of files that contains the word
                if (termFrecuency[i] != 0)
                {
                    IDF++;
                }
            }

            // Getting IDF
            return (float)Math.Log10(termFrecuency.Length / (1 + IDF));
        }
    }
}