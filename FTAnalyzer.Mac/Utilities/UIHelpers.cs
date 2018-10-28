using AppKit;

namespace FTAnalyzer.Mac
{
    public static class UIHelpers
    {
        public static int ShowMessage(string message) => ShowMessage(message, string.Empty);

        public static int ShowMessage(string message, string title)
        {
            var alert = new NSAlert
            {
                MessageText = title,
                InformativeText = message
            };
            return (int)alert.RunModal();
        }
    }
}
