using System.IO;

namespace NShave.Console
{
    public class Program 
    {
        public string MustacheTemplate { get; set; }
        public string Data { get; set; }

        public static void Main(string[] args)
        {
            var mustachePath = args[0];
            var dataPath = args[1];

            var adapter = new Adapter
            {
                MustacheTemplate = File.ReadAllText(mustachePath),
                Data = File.ReadAllText(dataPath)
            };

            System.Console.WriteLine(adapter.MustacheTemplate);
            System.Console.WriteLine(adapter.Data);

            var domain = new Domain(adapter);
            domain.ToRazor(System.Console.WriteLine);
            System.Console.Read();
        }

        private class Adapter : IDomainAdapter
        {
            public string MustacheTemplate { get; set; }
            public string Data { get; set; }
        }
    }
}