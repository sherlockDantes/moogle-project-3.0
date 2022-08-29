namespace MoogleEngine
{
    public static class FilesVectorSpaceModel
    {
        public static Dictionary<string, int[]> GetCorpus(string[] fileNames)
        {
            Dictionary<string, int[]> corpus = new Dictionary<string, int[]>();

            for (int i = 0; i < fileNames.Length; i++)
            {
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
                        // Adding one more space for query
                        wordsFrecuency = new int[fileNames.Length + 1];
                        wordsFrecuency[i]++;
                        corpus.Add(words[j], wordsFrecuency);
                    }
                }
            }

            return corpus;
        }
        public static Dictionary<string, float[]> Get_TF_IDF_Matrix(Dictionary<string, int[]> corpus, string[] fileNames, float[] powers)
        {
            Dictionary<string, float[]> TF_IDF_Matrix = new Dictionary<string, float[]>();

            int count = 0;
            foreach (var key in corpus)
            {
                // Getting IDF of each word
                float IDF = GetIDF(key.Value);

                // Subtracting 1 to key.Value.Length, because that the space we added for future query
                for (int i = 0; i < key.Value.Length - 1; i++)
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
        public static List<SearchItem> GetCosineSimilarity(Dictionary<string, float[]> TF_IDF_Matrix, float[] powers)
        {
            List<SearchItem> searchItems = new List<SearchItem>();

            int amountOfFiles = powers.Length;

            int count = 0;
            foreach (var key in TF_IDF_Matrix)
            {
                // Removing query from comparisons
                if (key.Key == "query")
                {
                    count++;
                    continue;
                }

                // Getting norms of each vector
                float norm1 = (float)Math.Sqrt(powers[count]);
                float norm2 = (float)Math.Sqrt(powers[amountOfFiles - 1]);

                // Calculating cosine similarity between vectors
                float cosine = GetCosine(key.Value, norm1, TF_IDF_Matrix["query"], norm2);

                // Removing vectors that aren't similar enough
                if (cosine < 0.006f || cosine.ToString() == "NaN")
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
        public static void ResetSpaceForQuery(Dictionary<string, int[]> corpus, Dictionary<string, float[]> TF_IDF_Matrix, float[] powers)
        {
            int amountOfFiles = corpus.First().Value.Length;

            // Reseting corpus
            foreach (var key in corpus)
            {
                key.Value[amountOfFiles - 1] = 0;
            }

            // Reseting TF_IDF_Matrix
            float[] resetedQueryVector = new float[corpus.Count];

            if (TF_IDF_Matrix.ContainsKey("query"))
            {
                TF_IDF_Matrix["query"] = resetedQueryVector;
            }
            else
            {
                TF_IDF_Matrix.Add("query", resetedQueryVector);
            }

            // Reseting sum of powers
            powers[amountOfFiles - 1] = 0;
        }
        public static void AddQueryToCorpus(string query, Dictionary<string, int[]> corpus)
        {
            // Cleaning up query
            string[] queryTerms = Tools.Tokenize(query);

            int amountOfFiles = corpus.First().Value.Length;

            // Updating query's termFrecuency
            for (int i = 0; i < queryTerms.Length; i++)
            {
                if (corpus.ContainsKey(queryTerms[i]))
                {
                    // We're now using the space we saved for query
                    corpus[queryTerms[i]][amountOfFiles - 1]++;
                }
            }
        }
        public static void AddQueryToTF_IDF_Matrix(Dictionary<string, float[]> TF_IDF_Matrix, Dictionary<string, int[]> corpus, float[] powers)
        {
            int amountOfFiles = corpus.First().Value.Length;

            // Updating query's vector
            int count = 0;
            foreach (var key in corpus)
            {
                float IDF = GetIDF(key.Value);

                // Getting each TF_IDF
                float TF_IDF = key.Value[amountOfFiles - 1] * IDF;

                TF_IDF_Matrix["query"][count] = TF_IDF;

                // Updating the sum of powers of query vector
                powers[amountOfFiles - 1] += (float)Math.Pow(TF_IDF, 2);

                count++;
            }
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