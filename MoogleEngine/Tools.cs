using System.Text.RegularExpressions;

namespace MoogleEngine
{
    static class Tools
    {
        public static string[] Tokenize(string text)
        {
            return RemoveAccent(text).Split(" @$/#.-:&+=[]*^~!?(){},''\">_<;%\\".ToCharArray()).Select(word => Regex.Replace(word, "[^a-zA-Z]", "").ToLower()).Where(word => word != "").ToArray();
        }

        public static string[] RemoveRepeatedTerms(string[] terms)
        {
            List<string> nonRepeatedTerms = new List<string>();

            for (int i = 0; i < terms.Length; i++)
            {
                if (!nonRepeatedTerms.Contains(terms[i]))
                {
                    nonRepeatedTerms.Add(terms[i]);
                }
            }

            return nonRepeatedTerms.ToArray();
        }
        public static void ReplaceAllTerms(string[] terms, Tuple<string, string> pair)
        {
            for (int i = 0; i < terms.Length; i++)
            {
                if (terms[i] == pair.Item1)
                {
                    terms[i] = pair.Item2;
                }
            }
        }
        public static string GetFinalSuggestion(string[] terms)
        {
            string suggestion = "";

            for (int i = 0; i < terms.Length; i++)
            {
                if(i == terms.Length - 1)
                {
                    suggestion += terms[i];
                    continue;
                }
                suggestion += terms[i] + " ";
            }

            return suggestion;
        }
        private static string RemoveAccent(string text)
        {
            string result = text;
            result = result.Replace('á', 'a');
            result = result.Replace('é', 'e');
            result = result.Replace('í', 'i');
            result = result.Replace('ó', 'o');
            result = result.Replace('ú', 'u');

            return result;
        }


    }
}