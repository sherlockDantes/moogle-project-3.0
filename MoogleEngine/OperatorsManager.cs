namespace MoogleEngine
{
    static class OperatorsManager
    {
        //** Processing terms
        public static bool ContainsLetter(string term)
        {
            for (int i = 97; i < 123; i++)
            {
                if (term.Contains(((char)i))) return true;
            }

            return false;
        }
        public static string RemoveOperator(string term)
        {
            return Tools.Tokenize(term)[0];
        }

        // Calculates the percentage of increase according to the "*" operator
        public static float GetStarOperatorIncrease(string term)
        {
            float result = 1;

            for (int i = 0; i < term.Length; i++)
            {
                if (term[i] == '*')
                {
                    // Increasing a 10%
                    result += 0.10f;
                }
                else
                {
                    break;
                }
            }

            return result;
        }

        //** Main
        public static void ManageAbsoluteOperators(List<string> forbiddenTerms, List<string> mustBePresentTerms, List<Tuple<string, float>> starTerms, List<SearchItem> similarItems, List<string> fileNames, Dictionary<string, int[]> corpus)
        {
            List<SearchItem> itemsToRemove = new List<SearchItem>();
            foreach (var item in similarItems)
            {
                foreach (var term in forbiddenTerms)
                {
                    if (ContainsWord(term, item.Title, fileNames, corpus))
                    {
                        itemsToRemove.Add(item);
                    }
                }

                foreach (var term in mustBePresentTerms)
                {
                    if (!ContainsWord(term, item.Title, fileNames, corpus))
                    {
                        itemsToRemove.Add(item);
                    }
                }

                foreach (var tuple in starTerms)
                {
                    if (ContainsWord(tuple.Item1, item.Title, fileNames, corpus))
                    {
                        item.Score *= tuple.Item2;
                    }
                }
            }

            // Removing unnecessary search items from search result
            foreach (var item in itemsToRemove)
            {
                similarItems.Remove(item);
            }
        }

        public static void ManageCloseness(List<Tuple<string, string>> closeTerms, Dictionary<string, Dictionary<string, List<int>>> positionTracker, List<SearchItem> similarItems, List<string> fileNames, Dictionary<string, int[]> corpus)
        {
            foreach(var tuple in closeTerms)
            {
                List<ClosenessData> positiveFiles = new List<ClosenessData>();

                foreach(var item in similarItems)
                {
                    if(!ContainsWord(tuple.Item1, item.Title, fileNames, corpus) || !ContainsWord(tuple.Item2, item.Title, fileNames, corpus))
                    {
                        continue;
                    }
                    else
                    {

                        int closeness = GetCloseness(positionTracker[item.Title][tuple.Item1], positionTracker[item.Title][tuple.Item2]);

                        positiveFiles.Add(new ClosenessData(item.Title, closeness));
                    }
                }

                positiveFiles.Sort();

                float increment = 2;

                foreach(var key in positiveFiles)
                {
                    foreach(var item in similarItems)
                    {
                        if(key.FileName == item.Title)
                        {
                            item.Score *= increment;
                            increment -= 0.2f;
                        }
                    }
                }
            }
        }

        public static int GetCloseness(List<int> positions1, List<int> positions2)
        {
            int closeness = positions1[0] - positions2[0];

            foreach (var key1 in positions1)
            {
                foreach (var key2 in positions2)
                {
                    int newDistance = Math.Abs(key1 - key2);
                    if (closeness > newDistance)
                    {
                        closeness = newDistance;
                    }
                }
            }

            return closeness;
        }
        public static bool ContainsWord(string word, string fileName, List<string> fileNames, Dictionary<string, int[]> corpus)
        {
            if (!corpus.ContainsKey(word)) return false;

            if (!fileNames.Contains(fileName)) return false;

            if (corpus[word][fileNames.IndexOf(fileName)] != 0)
            {
                return true;
            }

            return false;
        }
    }
}