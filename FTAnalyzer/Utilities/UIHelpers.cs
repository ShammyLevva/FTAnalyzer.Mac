using AppKit;

namespace FTAnalyzer
{
    public static class UIHelpers
    {
        public static int ShowMessage(string message) => ShowMessage(message, "FTAnalyzer");
        public static int ShowMessage(string message, string title)
        {
            var alert = new NSAlert
            {
                AlertStyle = NSAlertStyle.Informational,
                MessageText = title,
                InformativeText = message
            };
            return (int)alert.RunModal();
        }

        public static int ShowYesNo(string message) => ShowYesNo(message, "FTAnalyzer");
        public static int ShowYesNo(string message, string title)
        {
            var alert = new NSAlert
            {
                AlertStyle = NSAlertStyle.Informational,
                MessageText = title,
                InformativeText = message
            };
            alert.AddButton("Yes");
            alert.AddButton("No");
            return (int)alert.RunModal();
        }
        public static int Yes => 1000;
        public static int No => 1001;
    }
}