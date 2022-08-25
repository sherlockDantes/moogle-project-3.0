namespace MoogleEngine;
public static class Moogle
{
    public static SearchResult Query(string query)
    {
        // Write here content's address
        string address = @"";

        // Future query.txt address
        string queryFileName = Path.Combine(address, "query.txt");

        if (File.Exists(queryFileName))
        {
            File.Delete(queryFileName);
        }

        // Creating a file that contains the query
        StreamWriter queryFile = new StreamWriter(queryFileName);

        queryFile.WriteLine(query);

        queryFile.Close();

        string[] fileNames = Directory.GetFiles(address);

        // Powers is the sum of tf_idf vectors square powers
        float[] powers = new float[fileNames.Length];

        int indexOfQuery;

        Dictionary<string, int[]> corpus = VectorSpaceModel.GetCorpus(fileNames, out indexOfQuery, queryFileName);

        // Erasing the query file
        File.Delete(queryFileName);

        Dictionary<string, float[]> TF_IDF_Matrix = VectorSpaceModel.Get_TF_IDF_Matrix(corpus, fileNames, ref powers);

        List<SearchItem> cosineSimilarities = VectorSpaceModel.GetCosineSimilarity(TF_IDF_Matrix, queryFileName, powers, indexOfQuery);

        // SearchItem[] items = new SearchItem[3] {
        //     new SearchItem("Hello World", "Lorem ipsum dolor sit amet", 0.9f),
        //     new SearchItem("Hello World", "Lorem ipsum dolor sit amet", 0.5f),
        //     new SearchItem("Hello world", "Lorem ipsum dolor sit amet", 0.95f),
        // };

        return new SearchResult(cosineSimilarities.ToArray());
    }
}
