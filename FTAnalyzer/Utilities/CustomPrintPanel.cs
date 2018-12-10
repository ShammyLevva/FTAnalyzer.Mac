using System;
using AppKit;
using FTAnalyzer.ViewControllers;

namespace FTAnalyzer.Utilities
{
    public class CustomPrintPanel : NSPrintPanel
    {
        PrintPanelViewController _viewController;

        public bool Refresh
        {
            get => _viewController.Refresh;
            set => _viewController.Refresh = value;
        }

        public CustomPrintPanel()
        {
            Options = NSPrintPanelOptions.ShowsCopies | NSPrintPanelOptions.ShowsPageRange | NSPrintPanelOptions.ShowsPreview |
                      NSPrintPanelOptions.ShowsPageSetupAccessory | NSPrintPanelOptions.ShowsScaling;
            _viewController = new PrintPanelViewController();
            //AddAccessoryController(_viewController);
        }
    }
}
