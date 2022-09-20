namespace MoogleEngine
{
    static class SnippetManager
    {
        public static void GetSnippet(string[] queryTerms, List<string> fileNames, List<string> mustBePresentTerms, List<Tuple<string, float>> starTerms, List<Tuple<string, string>> closeTerms, List<SearchItem> similarItems, Dictionary<string, int[]> corpus, Dictionary<string, Dictionary<string, string>> lineTracker)
        {
            foreach (var item in similarItems)
            {
                if (mustBePresentTerms.Count != 0)
                {
                    foreach (var term in mustBePresentTerms)
                    {
                        if (mustBePresentTerms.Last() == term)
                        {
                            item.Snippet += lineTracker[item.Title][term];
                            continue;
                        }
                        item.Snippet += lineTracker[item.Title][term] + "...";
                    }
                    continue;
                }

                List<string> actualStarTerms = new List<string>();
                foreach (var key in starTerms)
                {
                    if (OperatorsManager.ContainsWord(key.Item1, item.Title, fileNames, corpus))
                    {
                        actualStarTerms.Add(key.Item1);
                    }
                }

                List<string> actualCloseTerms = new List<string>();
                foreach (var key in closeTerms)
                {
                    if (OperatorsManager.ContainsWord(key.Item1, item.Title, fileNames, corpus) && OperatorsManager.ContainsWord(key.Item2, item.Title, fileNames, corpus))
                    {
                        actualCloseTerms.Add(key.Item1);
                        actualCloseTerms.Add(key.Item2);
                    }
                }

                List<string> actualQueryTerms = new List<string>();
                for (int i = 0; i < queryTerms.Length; i++)
                {
                    if (OperatorsManager.ContainsWord(queryTerms[i], item.Title, fileNames, corpus))
                    {
                        actualQueryTerms.Add(queryTerms[i]);
                    }
                }

                if (actualStarTerms.Count != 0)
                {
                    foreach (var term in actualStarTerms)
                    {
                        if (actualStarTerms.Last() == term)
                        {
                            item.Snippet += lineTracker[item.Title][term];
                            continue;
                        }
                        item.Snippet += lineTracker[item.Title][term] + "...";
                    }
                }
                else if (actualCloseTerms.Count != 0)
                {
                    foreach (var term in actualCloseTerms)
                    {
                        if (actualCloseTerms.Last() == term)
                        {
                            item.Snippet += lineTracker[item.Title][term];
                            continue;
                        }
                        item.Snippet += lineTracker[item.Title][term] + "...";
                    }
                }
                else
                {
                    foreach (var term in actualQueryTerms)
                    {
                        if (actualQueryTerms.Last() == term)
                        {
                            item.Snippet += lineTracker[item.Title][term];
                            continue;
                        }
                        item.Snippet += lineTracker[item.Title][term] + "...";
                    }
                }
            }
        }

        // Adds html tags to hightlight query terms
        public static void HighlightTerms(string[] queryTerms, List<SearchItem> similarItems)
        {
            foreach (var item in similarItems)
            {
                for (int i = 0; i < queryTerms.Length; i++)
                {
                    item.Snippet = Hightlight(queryTerms[i], item.Snippet);
                }
            }
        }
        private static string Hightlight(string term, string snippet)
        {
            List<string> words = snippet.Split(" ").ToList();

            string result = "";

            foreach (var key in words)
            {
                try
                {
                    if (Tools.Tokenize(key)[0] == term)
                    {
                        result += ($"<strong>{key}</strong>") + " ";
                        continue;
                    }
                }
                catch
                {
                    result += key + " ";
                    continue;
                }

                result += key + " ";
            }

            return result;
        }
    }
}