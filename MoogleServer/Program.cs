using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace MoogleServer
{
    class Program
    {
        // Content address
        static string address = @"D:\Work\Businnes\CSharp\Moogle Project\Moogle Project Original 4.0\Content";
        static string[] fileNames = Directory.GetFiles(address);
        
        // Sum of square powers of vectors, required to calculate the norm
        public static float[] powers = new float[fileNames.Length + 1];

        // Dictionaries
        public static Dictionary<string, int[]> corpus;
        public static Dictionary<string, float[]> TF_IDF_Matrix;
        
        static void Main(string[] args)
        {
            // Loading up Dictionaries
            corpus = MoogleEngine.VectorSpaceModel.GetCorpus(fileNames);
            TF_IDF_Matrix = MoogleEngine.VectorSpaceModel.Get_TF_IDF_Matrix(corpus, fileNames, powers);

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