namespace NShave.Console
{
    using System;
    using System.IO;

    public class Program 
    {
        public string MustacheTemplate { get; set; }
        public string Data { get; set; }

        public static void Main(string[] args)
        {
            try
            {
                //var mustachePath = args[0];
                //var dataPath = args[1];

                const string mustachePath = "test.mustache";
                const string dataPath = "data.json";

                var adapter = new Adapter
                {
                    MustacheTemplate = File.ReadAllText(mustachePath),
                    Data = File.ReadAllText(dataPath)
                };

                var domain = new Domain(adapter);
                domain.ToRazor((razor) =>
                {
                    Console.WriteLine(razor);
                    var fileName = ToRazorFileName(mustachePath);
                    File.WriteAllText(fileName, razor);
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error, please review the parameters.");
                Console.WriteLine($"Failed with error: {ex.Message}");
            }
            Console.ReadKey();
        }

        private static string ToRazorFileName(string mustachePath)
        {
            var fileName = mustachePath.Replace(".mustache", ".cshtml");
            fileName = fileName.Replace(fileName, char.ToUpper(fileName[0]) + fileName.Substring(1));
            fileName = $"_{fileName}";
            return fileName;
        }

        private class Adapter : IDomainAdapter
        {
            public string MustacheTemplate { get; set; }
            public string Data { get; set; }
        }
    }
}