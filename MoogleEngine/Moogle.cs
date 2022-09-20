namespace MoogleEngine;
public static class Moogle
{
    public static SearchResult Query(string query, Dictionary<string, int[]> corpus, Dictionary<string, float[]> TF_IDF_Matrix, float[] powers, Dictionary<string, Dictionary<string, List<int>>> positionTracker, Dictionary<string, Dictionary<string, string>> lineTracker)
    {
        string[] queryTerms = Tools.Tokenize(query);

        // Removing repeated words in query terms for efficiency when working on snippets and to avoid an error when working on suggestions
        string[] nonRepeatedQueryTerms = Tools.RemoveRepeatedTerms(queryTerms);

        List<string> fileNames = TF_IDF_Matrix.Keys.ToList();
        List<string> words = corpus.Keys.ToList();

        //** Working on files comparison
        // Reseting VSM(Vector Space Model) files dictionaries
        VectorSpaceModel.ResetSpaceForQuery(corpus, TF_IDF_Matrix, powers);

        // Updating VSM files dictionaries
        VectorSpaceModel.AddQueryToCorpus(query, corpus);
        VectorSpaceModel.AddQueryToTF_IDF_Matrix(TF_IDF_Matrix, corpus, powers);

        // Comparing file vectors according to the cosine similarity
        List<SearchItem> similarItems = VectorSpaceModel.GetCosineSimilarity(TF_IDF_Matrix, powers);

        //** Working on operators
        string[] queryTermsWithOperators = Tools.TokenizeForOperators(query);
        List<string> forbiddenTerms = new List<string>();
        List<string> mustBePresentTerms = new List<string>();
        List<Tuple<string, float>> starTerms = new List<Tuple<string, float>>();
        List<Tuple<string, string>> closeTerms = new List<Tuple<string, string>>();

        for (int i = 0; i < queryTermsWithOperators.Length; i++)
        {
            if (OperatorsManager.ContainsLetter(queryTermsWithOperators[i]))
            {
                // Preventing an error if the user does not use operators the right way
                if (queryTermsWithOperators[i].Contains('!') && queryTermsWithOperators[i].Contains('^') || queryTermsWithOperators[i].Contains('!') && queryTermsWithOperators[i].Contains('*') || queryTermsWithOperators[i].Contains('*') && queryTermsWithOperators[i].Contains('^'))
                {
                    continue;
                }
                else if (queryTermsWithOperators[i].Contains('!'))
                {
                    forbiddenTerms.Add(OperatorsManager.RemoveOperator(queryTermsWithOperators[i]));
                }
                else if (queryTermsWithOperators[i].Contains('^'))
                {
                    mustBePresentTerms.Add(OperatorsManager.RemoveOperator(queryTermsWithOperators[i]));
                }
                else if (queryTermsWithOperators[i].Contains('*'))
                {
                    // Adding the term and its increase percentage
                    starTerms.Add(new Tuple<string, float>(OperatorsManager.RemoveOperator(queryTermsWithOperators[i]), OperatorsManager.GetStarOperatorIncrease(queryTermsWithOperators[i])));
                }
                else
                {
                    continue;
                }
            }
            else
            {
                if (queryTermsWithOperators[i] == "~")
                {
                    try
                    {
                        // Adding the terms that are next to "~"
                        closeTerms.Add(new Tuple<string, string>(OperatorsManager.RemoveOperator(queryTermsWithOperators[i - 1]), OperatorsManager.RemoveOperator(queryTermsWithOperators[i + 1])));
                    }
                    // Preventing an exception if the user does not use operator the right way
                    catch
                    {
                        continue;
                    }
                }
            }
        }

        // Managing all operator, except for "~"
        OperatorsManager.ManageAbsoluteOperators(forbiddenTerms, mustBePresentTerms, starTerms, similarItems, fileNames, corpus);

        // Managing closeness operator "~"
        OperatorsManager.ManageCloseness(closeTerms, positionTracker, similarItems, fileNames, corpus);

        similarItems.Sort();

        //** Working on snippets
        SnippetManager.GetSnippet(nonRepeatedQueryTerms, fileNames, mustBePresentTerms, starTerms, closeTerms, similarItems, corpus, lineTracker);

        // // Highlighting terms
        SnippetManager.HighlightTerms(nonRepeatedQueryTerms, similarItems);

        //** Working on suggestions
        bool addedSuggestion = false;

        for (int i = 0; i < nonRepeatedQueryTerms.Length; i++)
        {
            if (!corpus.ContainsKey(nonRepeatedQueryTerms[i]))
            {
                string suggestion = SuggestionsManager.GetSuggestion(nonRepeatedQueryTerms[i], words);

                Tuple<string, string> pair = new Tuple<string, string>(nonRepeatedQueryTerms[i], suggestion);

                // Replace all query terms with the suggestion
                Tools.ReplaceAllTerms(queryTerms, pair);

                addedSuggestion = true;
            }
        }

        if (addedSuggestion)
        {
            string finalSuggestion = Tools.GetFinalSuggestion(queryTerms);

            return new SearchResult(similarItems.ToArray(), finalSuggestion);
        }

        return new SearchResult(similarItems.ToArray());
    }
}
