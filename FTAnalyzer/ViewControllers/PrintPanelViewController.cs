using System;
using AppKit;
using Foundation;

namespace FTAnalyzer.ViewControllers
{
    public partial class PrintPanelViewController : NSViewController
	{
        public bool Refresh { get; set; }
        readonly string[] observedProperties = { "Refresh" };

        public PrintPanelViewController() => Refresh = false;
        public PrintPanelViewController (IntPtr handle) : base (handle) { }

        public NSSet KeyPathsForValuesAffectingPreview() => new NSSet(observedProperties);

        public NSDictionary[] LocalizedSummaryItems() => null;
    }
}
