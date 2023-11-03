using FTAnalyzer.Exports;

namespace FTAnalyzer
{
    static class MainClass
    {
        public static readonly HttpClient Client = new();
        public static readonly LostCousinsClient LCClient = new();
        static void Main(string[] args)
        {
            NSApplication.Init();
            NSApplication.Main(args);
        }
    }
}
