using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace MoogleServer
{
    class Program
    {
        // Write here content's address
        static string address = @"../Content";
        static string[] fileNames = Directory.GetFiles(address);

        // Sum of powers of each file vectors, required to calculate the norm. Adding one for future query
        public static float[] powers = new float[fileNames.Length + 1];

        // Dictionaries of files according to the Vector Space Model(VSM)
        public static Dictionary<string, int[]> corpus = new Dictionary<string, int[]>();
        public static Dictionary<string, float[]> TF_IDF_Matrix;

        // Position tracker dictionary
        public static Dictionary<string, Dictionary<string, List<int>>> positionTracker = new Dictionary<string, Dictionary<string, List<int>>>();

        // Line tracker dictionary
        public static Dictionary<string, Dictionary<string, string>> lineTracker;
        static void Main(string[] args)
        {
            //** Loading up data
            // Loading up files as vectors dictionaries
            MoogleEngine.VectorSpaceModel.ProcessingFiles(fileNames, corpus, positionTracker);
            TF_IDF_Matrix = MoogleEngine.VectorSpaceModel.Get_TF_IDF_Matrix(corpus, fileNames, powers);

            // Loading up lineTracker
            lineTracker = MoogleEngine.VectorSpaceModel.GetLineTracker(fileNames);

            //** Loading up server
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