using FTAnalyzer.Exports;

namespace FTAnalyzer
{
    static class MainClass
    {
        public static HttpClient Client = new();
        public static LostCousinsClient LCClient = new();

        static void Main(string[] args)
        {
            NSApplication.Init();
            NSApplication.Main(args);
        }
    }
}
