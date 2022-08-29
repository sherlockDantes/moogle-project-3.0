using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace MoogleServer
{
    class Program
    {
        // Write here content's address
        static string address = @"";

        static string[] fileNames = Directory.GetFiles(address);

        // Sum of powers of each file vectors, required to calculate the norm. Adding one for future query
        public static float[] powers = new float[fileNames.Length + 1];
        
        // Dictionaries of files according to the Vector Space Model(VSM)
        public static Dictionary<string, int[]> corpus;
        public static Dictionary<string, float[]> TF_IDF_Matrix;

        // Sum of powers of each word vector, required to calculate norm too
        public static float[] charPowers;

        // Dictionaries of words according to the VSM
        public static Dictionary<char, int[]> characterCorpus;
        public static Dictionary<string, float[]> words_TF_IDF_Matrix;
        static void Main(string[] args)
        {
            // Loading up files as vectors dictionaries
            corpus = MoogleEngine.FilesVectorSpaceModel.GetCorpus(fileNames);
            TF_IDF_Matrix = MoogleEngine.FilesVectorSpaceModel.Get_TF_IDF_Matrix(corpus, fileNames, powers);

            // Updating charPowers. Adding one for possible unknown word
            charPowers = new float[corpus.Count + 1];

            // Loading up words as vectors dictionaries
            characterCorpus = MoogleEngine.WordsVectorSpaceModel.GetCharacterCorpus(corpus);
            words_TF_IDF_Matrix = MoogleEngine.WordsVectorSpaceModel.Get_Words_TF_IDF_Matrix(characterCorpus, corpus, charPowers);
            

            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorPages();
            builder.Services.AddServerSideBlazor();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
            }


            app.UseStaticFiles();

            app.UseRouting();

            app.MapBlazorHub();
            app.MapFallbackToPage("/_Host");

            app.Run();
        }
    }
}