namespace MoogleEngine;
public static class Moogle
{
    public static SearchResult Query(string query, Dictionary<string, int[]> corpus, Dictionary<string, float[]> TF_IDF_Matrix, float[] powers, Dictionary<char, int[]> characterCorpus, Dictionary<string, float[]> words_TF_IDF_Matrix, float[] charPowers)
    {
        // Reseting VSM(Vector Space Model) files dictionaries
        FilesVectorSpaceModel.ResetSpaceForQuery(corpus, TF_IDF_Matrix, powers);

        // Updating VSM files dictionaries
        FilesVectorSpaceModel.AddQueryToCorpus(query, corpus);
        FilesVectorSpaceModel.AddQueryToTF_IDF_Matrix(TF_IDF_Matrix, corpus, powers);

        // Comparing file vectors according to the cosine similarity
        List<SearchItem> similarItems = FilesVectorSpaceModel.GetCosineSimilarity(TF_IDF_Matrix, powers);


        string[] queryTerms = Tools.Tokenize(query);

        // Removing repeated words in query terms to avoid an error
        string[] nonRepeatedQueryTerms = Tools.RemoveRepeatedTerms(queryTerms);

        bool addSuggestion = false;

        for (int i = 0; i < nonRepeatedQueryTerms.Length; i++)
        {
            if (!corpus.ContainsKey(nonRepeatedQueryTerms[i]))
            {
                // Reseting VSM words dictionaries
                WordsVectorSpaceModel.ResetSpaceForUnknown(characterCorpus, words_TF_IDF_Matrix, charPowers);

                // Adding unknown to VSM words dictionaries
                WordsVectorSpaceModel.AddUnknownWordToCharacterCorpus(characterCorpus, nonRepeatedQueryTerms[i]);
                WordsVectorSpaceModel.AddUnknownWordToWords_TF_IDF_Matrix(words_TF_IDF_Matrix, characterCorpus, charPowers, nonRepeatedQueryTerms[i]);

                string suggestion = WordsVectorSpaceModel.GetSuggestion(words_TF_IDF_Matrix, charPowers);
                Tuple<string, string> pair = new Tuple<string, string>(nonRepeatedQueryTerms[i], suggestion);

                // Replace all query terms with the suggestion
                Tools.ReplaceAllTerms(queryTerms, pair);

                addSuggestion = true;
            }
        }

        if (addSuggestion)
        {
            string finalSuggestion = Tools.GetFinalSuggestion(queryTerms);

            return new SearchResult(similarItems.ToArray(), finalSuggestion);
        }

        return new SearchResult(similarItems.ToArray());
    }
}
