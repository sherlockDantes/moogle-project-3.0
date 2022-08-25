using System.Text.RegularExpressions;

namespace MoogleEngine
{
    static class Tools
    {
        public static string[] Tokenize(string text)
        {
            return RemoveAccent(text).Split(" @$/#.-:&+=[]*^~!?(){},''\">_<;%\\".ToCharArray()).Select(word => Regex.Replace(word, "[^a-zA-Z]", "").ToLower()).Where(word => word != "").ToArray();
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

        // public static string GetSnippet(List<float> tf_idfs_ofQuery, List<string> words, string fileName)
        // {
        //     string word = "";

        //     string[] textTerms = Tokenize(File.ReadAllText(fileName));

        //     while(tf_idfs_ofQuery.Count != 0)
        //     {
        //         int index = tf_idfs_ofQuery.IndexOf(tf_idfs_ofQuery.Max());

        //         if(textTerms.Contains(words[index]))
        //         {
        //             word = words[index];
        //             break;
        //         }

        //         tf_idfs_ofQuery.RemoveAt(index);
        //     }

        //     StreamReader file = new StreamReader(fileName);
            
        //     string line = file.ReadLine();

        //     while(line != null)
        //     {
        //         if(line.Contains(word))
        //         {
        //             break;
        //         }
        //         line = file.ReadLine();
        //     }

        //     return line;
        // }
    }
}