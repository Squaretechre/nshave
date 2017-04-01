using System.Reflection;

namespace NShave.Tests.Support
{
    public class FilePath
    {
        public static string WorkingDirectory()
        {
            return 
                Assembly.Load(new AssemblyName("NShave.Tests"))
                    .Location
                    .Replace("NShave.Tests.dll", string.Empty);
        }
    }
}