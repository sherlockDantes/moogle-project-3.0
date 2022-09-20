namespace MoogleEngine
{
    static class SuggestionsManager
    {
        private struct Suggestion
        {
            public string Word
            {
                get;
                set;
            }
            public int Score
            {
                get;
                set;
            }
            public Suggestion(string word, int score)
            {
                Word = word;
                Score = score;
            }
        }
        public static string GetSuggestion(string word, List<string> words)
        {
            Suggestion result = new Suggestion(words[0], GetLevenshteinDistance(word, words[0]));

            if(result.Score == 1)
            {
                return result.Word;
            }

            words.RemoveAt(0);

            foreach (var key in words)
            {
                int distance = GetLevenshteinDistance(word, key);
                if (result.Score > distance)
                {
                    result.Word = key;
                    result.Score = distance;
                }
            }

            return result.Word;
        }

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
    }
}