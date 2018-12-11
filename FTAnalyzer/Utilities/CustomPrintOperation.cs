using System;
using AppKit;

namespace FTAnalyzer.Utilities
{
    public class CustomPrintOperation : NSPrintOperation
    {
        public CustomPrintOperation(NSView printView, NSPrintInfo printInfo)
        {
            FromView(printView, printInfo);
            ShowsPrintPanel = true;
            ShowsProgressPanel = true;
            CanSpawnSeparateThread = true;
            PrintPanel.Options = NSPrintPanelOptions.ShowsCopies | NSPrintPanelOptions.ShowsPageRange | NSPrintPanelOptions.ShowsPreview |
                                 NSPrintPanelOptions.ShowsPageSetupAccessory | NSPrintPanelOptions.ShowsScaling;
        }

    }
}
