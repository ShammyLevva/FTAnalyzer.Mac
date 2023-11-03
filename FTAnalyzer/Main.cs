using System.Net;
using FTAnalyzer.Exports;

namespace FTAnalyzer
{
    static class MainClass
    {
        public static HttpClient Client { get; private set; }
        public static CookieContainer Cookies { get; private set; }
        public static readonly LostCousinsClient LCClient = new();

        static void Main(string[] args)
        {
            NSApplication.Init();
            SetupHttpClient();
            NSApplication.Main(args);
        }

        static void SetupHttpClient()
        {
            Cookies = new();
            HttpClientHandler handler = new()
            {
                CookieContainer = Cookies
            };
            Client = new(handler);
        }
    }
}
