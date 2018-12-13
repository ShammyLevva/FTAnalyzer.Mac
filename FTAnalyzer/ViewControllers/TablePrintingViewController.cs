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
        readonly float scale = 0.65f;
        public NSView PrintView => View;
        Type _classType;

        public TablePrintingViewController(IPrintViewController tableViewController)
        {
            Title = tableViewController.Title;
            _classType = tableViewController.GetGenericType(); ;
            View = SetupView();
            _printView.Source = tableViewController.TableSource;
            _printView.SortDescriptors = tableViewController.SortDescriptors;
            _printView.ReloadData();
            View.SetFrameSize(new CGSize(_printView.Frame.Width, _printView.Frame.Height));
            //var x1 = View.Frame;
            //var y1 = _printView.Frame;
            //var window = new NSWindow { ContentView = View, ContentViewController = this, ViewsNeedDisplay = true };
            //window.Display();
        }

        public NSScrollView SetupView()
        {
            _printView = new NSTableView
            {
                Identifier = Title,
                RowSizeStyle = NSTableViewRowSizeStyle.Default,
                Enabled = true,
                UsesAlternatingRowBackgroundColors = true,
                ColumnAutoresizingStyle = NSTableViewColumnAutoresizingStyle.Sequential,
                WantsLayer = true,
                Layer = NewLayer(),
                Bounds = new CGRect(0, 0, 0, 0),
                AutoresizingMask = NSViewResizingMask.HeightSizable | NSViewResizingMask.WidthSizable,
                AutoresizesSubviews = true,
                AllowsColumnResizing = true,
                Target = Self,
                AutosaveName = "PrintView",
                NeedsDisplay = true
            };
            NSProcessInfo info = new NSProcessInfo();
            if (info.IsOperatingSystemAtLeastVersion(new NSOperatingSystemVersion(10, 13, 0)))
                _printView.UsesAutomaticRowHeights = true; // only available in OSX 13 and above.

            AddTableColumns(_printView, true);
            var scrollView = new NSScrollView
            {
                DocumentView = _printView,
                HasVerticalScroller = false,
                HasHorizontalScroller = false,
                WantsLayer = true,
                Layer = NewLayer(),
                Bounds = new CGRect(0, 0, 0, 0)
            };
            scrollView.ContentView.AutoresizingMask = NSViewResizingMask.HeightSizable | NSViewResizingMask.WidthSizable;
            return scrollView;
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
                    MaxWidth = 1000,
                    Editable = false,
                    Hidden = false,
                    Title = columnTitle,
                    ResizingMask = NSTableColumnResizing.Autoresizing | NSTableColumnResizing.UserResizingMask
                };
                view.AddColumn(tableColumn);
                _tableWidth += width;
            }
        }
    }
}
