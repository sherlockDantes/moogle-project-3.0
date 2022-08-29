namespace MoogleEngine
{
    public static class WordsVectorSpaceModel
    {
        static char[] alphabet;

        struct Suggestion
        {
            public string Word
            {
                get;
                set;
            }
            public float Score
            {
                get;
                set;
            }
            public Suggestion(string word, float score)
            {
                Word = word;
                Score = score;
            }
        }
        public static Dictionary<char, int[]> GetCharacterCorpus(Dictionary<string, int[]> wordCorpus)
        {
            Dictionary<char, int[]> characterCorpus = new Dictionary<char, int[]>();

            // Length according to ASCII
            alphabet = new char[26];

            // Iterating through ASCII code for lowered alphabet letters
            for (int i = ((byte)'a'); i <= ((byte)'z'); i++)
            {
                char letter = (char)i;

                // Subtracting 97 'cause of the ASCII code above
                alphabet[i - 97] = letter;

                // Adding one more space for possible unknown word in query
                characterCorpus.Add(((char)i), new int[wordCorpus.Count + 1]);
            }

            int wordCount = 0;
            foreach (var key in wordCorpus.Keys)
            {
                // Getting frecuency of all letters in the alphabet
                int[] charFrecuencyPerWord = GetCharFrecuency(alphabet, key);

                int charCount = 0;
                foreach (var pair in characterCorpus)
                {
                    // Updating characterCorpus
                    pair.Value[wordCount] = charFrecuencyPerWord[charCount];

                    charCount++;
                }

                wordCount++;
            }

            return characterCorpus;
        }

        public static Dictionary<string, float[]> Get_Words_TF_IDF_Matrix(Dictionary<char, int[]> characterCorpus, Dictionary<string, int[]> wordCorpus, float[] charPowers)
        {
            Dictionary<string, float[]> words_TF_IDF_Matrix = new Dictionary<string, float[]>();

            int charCount = 0;
            foreach (var pair in characterCorpus)
            {
                // float IDF = GetCharIDF(pair.Value);

                int wordCount = 0;
                foreach (var key in wordCorpus.Keys)
                {
                    // Adding a word if it wasn't there
                    if (!words_TF_IDF_Matrix.ContainsKey(key))
                    {
                        words_TF_IDF_Matrix.Add(key, new float[characterCorpus.Count]);
                    }

                    // Getting tf_idf of the char in an specific file
                    float TF_IDF = pair.Value[wordCount];

                    // Updating the tf_idf vector
                    words_TF_IDF_Matrix[key][charCount] = TF_IDF;

                    // Updating the sum of powers of each word vector
                    charPowers[wordCount] += (float)Math.Pow(TF_IDF, 2);

                    wordCount++;
                }

                charCount++;
            }

            return words_TF_IDF_Matrix;
        }
        public static void ResetSpaceForUnknown(Dictionary<char, int[]> characterCorpus, Dictionary<string, float[]> words_TF_IDF_Matrix, float[] charPowers)
        {
            int amountOfWords = characterCorpus.First().Value.Length;

            // Reseting characterCorpus
            foreach (var key in characterCorpus)
            {
                key.Value[amountOfWords - 1] = 0;
            }

            // Reseting words_TF_IDF_Matrix
            // If words_IDF_Matrix.Count is less than amountOfWords, then words_TF_IDF_Matrix is already reseted, 'cause it contains no unknown
            if (!(words_TF_IDF_Matrix.Count < amountOfWords))
            {
                // Remove last one, 'cause it's the unknown
                words_TF_IDF_Matrix.Remove(words_TF_IDF_Matrix.Last().Key);
            }

            // Reseting charPowers
            charPowers[amountOfWords - 1] = 0;
        }
        public static void AddUnknownWordToCharacterCorpus(Dictionary<char, int[]> characterCorpus, string unknown)
        {
            int amountOfWords = characterCorpus.First().Value.Length;

            int[] charFrecuencyPerWord = GetCharFrecuency(alphabet, unknown);

            // Adding unknown
            int count = 0;
            foreach (var pair in characterCorpus)
            {
                // We're now using the space we saved for the unknown word
                pair.Value[amountOfWords - 1] = charFrecuencyPerWord[count];
                count++;
            }
        }

        public static void AddUnknownWordToWords_TF_IDF_Matrix(Dictionary<string, float[]> words_TF_IDF_Matrix, Dictionary<char, int[]> characterCorpus, float[] charPowers, string unknown)
        {
            int amountOfWords = characterCorpus.First().Value.Length;

            // Adding unknown word to matrix
            words_TF_IDF_Matrix.Add(unknown, new float[characterCorpus.Count]);

            int count = 0;
            foreach (var key in characterCorpus)
            {
                // float IDF = GetCharIDF(key.Value);

                // Getting each TF_IDF
                float TF_IDF = characterCorpus[key.Key][amountOfWords - 1];

                words_TF_IDF_Matrix[unknown][count] = TF_IDF;

                // Updating the sum of powers of unknown vector
                charPowers[amountOfWords - 1] += (float)Math.Pow(TF_IDF, 2);

                count++;
            }
        }

        public static string GetSuggestion(Dictionary<string, float[]> words_TF_IDF_Matrix, float[] charPowers)
        {
            Suggestion suggestion = new Suggestion(" ", 0);

            int amountOfWords = words_TF_IDF_Matrix.Count;
            string unknown = words_TF_IDF_Matrix.Last().Key;

            int count = 0;
            foreach (var key in words_TF_IDF_Matrix)
            {
                // Removing unknown word from comparisons
                if (key.Key == unknown)
                {
                    count++;
                    continue;
                }

                // Getting norms of each vector
                float norm1 = (float)Math.Sqrt(charPowers[count]);
                float norm2 = (float)Math.Sqrt(charPowers[amountOfWords - 1]);

                // Calculating cosine similarity between vectors
                float cosine = GetCharCosine(key.Value, norm1, words_TF_IDF_Matrix[unknown], norm2);

                // Getting the word with the highest similarity
                if (cosine > suggestion.Score)
                {
                    suggestion.Word = key.Key;
                    suggestion.Score = cosine;
                }

                count++;
            }

            return suggestion.Word;
        }

        private static int[] GetCharFrecuency(char[] alphabet, string word)
        {
            int[] charFrecuency = new int[alphabet.Length];

            for (int i = 0; i < alphabet.Length; i++)
            {
                for (int j = 0; j < word.Length; j++)
                {
                    if (word[j] == alphabet[i])
                    {
                        charFrecuency[i]++;
                    }
                }
            }

            return charFrecuency;
        }
        private static float GetCharCosine(float[] firstVector, float norm1, float[] secondVector, float norm2)
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
        private static float GetCharIDF(int[] charFrecuency)
        {
            int IDF = 0;

            for (int i = 0; i < charFrecuency.Length; i++)
            {
                // Getting the amount of words that contains the char
                if (charFrecuency[i] != 0)
                {
                    IDF++;
                }
            }

            // Getting IDF
            return (float)Math.Log10(charFrecuency.Length / (1 + IDF));
        }
    }
}