namespace MoogleEngine;
public static class Moogle
{
    public static SearchResult Query(string query, Dictionary<string, int[]> corpus, Dictionary<string, float[]> TF_IDF_Matrix, float[] powers)
    {
        // Write here content's address
        string address = @"";

        string[] fileNames = Directory.GetFiles(address);

        // Reseting dictionaries
        VectorSpaceModel.ResetSpaceForQuery(corpus, TF_IDF_Matrix, powers);

        // Updating dictionaries
        VectorSpaceModel.AddQueryToCorpus(query, corpus);
        VectorSpaceModel.AddQueryToTF_IDF_Matrix(TF_IDF_Matrix, corpus, powers);

        // Comparing vectors according to the cosine similarity
        List<SearchItem> cosineSimilarities = VectorSpaceModel.GetCosineSimilarity(TF_IDF_Matrix,powers);

        // SearchItem[] items = new SearchItem[3] {
        //     new SearchItem("Hello World", "Lorem ipsum dolor sit amet", 0.9f),
        //     new SearchItem("Hello World", "Lorem ipsum dolor sit amet", 0.5f),
        //     new SearchItem("Hello world", "Lorem ipsum dolor sit amet", 0.95f),
        // };

        return new SearchResult(cosineSimilarities.ToArray());
    }
}
