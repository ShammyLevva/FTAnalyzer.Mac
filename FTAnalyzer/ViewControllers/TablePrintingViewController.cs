using System;
using AppKit;
using CoreAnimation;
using CoreGraphics;
using Foundation;
using FTAnalyzer.Utilities;

namespace FTAnalyzer.ViewControllers
{
    public class TablePrintingViewController : NSViewController
    {
        float _tableWidth;
        NSTableView _printView;
        readonly float scale = 0.7f;
        public NSView PrintView => View;
        Type _classType;
       
        public TablePrintingViewController(IPrintViewController tableViewController)
        {
            Title = tableViewController.Title;
            _classType =tableViewController.GetGenericType(); ;
            _printView = SetupView();
            _printView.Source = tableViewController.TableSource;
            _printView.SortDescriptors = tableViewController.SortDescriptors;
            _printView.ReloadData();
            var window = new NSWindow { ContentViewController = this, ContentView = _printView };
            var x = _printView.Window;
            var y = _printView.Frame;
            var z = _printView.Window.Frame;
        }

        public NSTableView SetupView()
        {
            var printViewDetails = new NSTableView
            {
                Identifier = Title,
                Enabled = true,
                UsesAlternatingRowBackgroundColors = true,
                WantsLayer = true,
                Layer = NewLayer(),
                Bounds = new CGRect(0, 0, 0, 0),
                AutoresizesSubviews = true,
                AutoresizingMask = NSViewResizingMask.HeightSizable | NSViewResizingMask.WidthSizable,
                AllowsColumnResizing = true,
                Target = Self,
                AutosaveName = "PrintView",
                NeedsDisplay = true,
                ColumnAutoresizingStyle = NSTableViewColumnAutoresizingStyle.Sequential,
                HeaderView = new NSTableHeaderView
                {
                    WantsLayer = true,
                    Layer = NewLayer(),
                    Bounds = new CGRect(0, 0, 0, 0),
                    AutoresizingMask = NSViewResizingMask.HeightSizable | NSViewResizingMask.WidthSizable
                }
            };
            NSProcessInfo info = new NSProcessInfo();
            if (info.IsOperatingSystemAtLeastVersion(new NSOperatingSystemVersion(10, 13, 0)))
                printViewDetails.UsesAutomaticRowHeights = true; // only available in OSX 13 and above.

            AddTableColumns(printViewDetails, true);
            View = printViewDetails;
            //printViewDetails.Frame = new CGRect(0, 0, printViewDetails.Frame.Width, 30);
            return printViewDetails;
        }

        static CALayer NewLayer() => new CALayer { Bounds = new CGRect(0, 0, 0, 0) };

        internal void AddTableColumns(NSTableView view, bool printing)
        {
            _tableWidth = 0f;
            var properties = _classType.GetProperties();
            foreach (var property in properties)
            {
                float width = 100;
                string columnTitle = property.Name;
                ColumnDetail[] columnDetail = property.GetCustomAttributes(typeof(ColumnDetail), false) as ColumnDetail[];
                if (columnDetail?.Length == 1)
                {
                    columnTitle = columnDetail[0].ColumnName;
                    width = columnDetail[0].ColumnWidth;
                }
                var tableColumn = new NSTableColumn
                {
                    Identifier = property.Name,
                    Width = width * scale,
                    Editable = false,
                    Hidden = false,
                    Title = columnTitle,
                    ResizingMask = NSTableColumnResizing.Autoresizing
                };
                view.AddColumn(tableColumn);
                _tableWidth += width;
            }
        }
    }
}
