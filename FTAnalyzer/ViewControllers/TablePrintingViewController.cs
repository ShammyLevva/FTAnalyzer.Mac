using System;
using System.Collections.Generic;
using AppKit;
using CoreAnimation;
using CoreGraphics;
using Foundation;
using FTAnalyzer.Utilities;

namespace FTAnalyzer.ViewControllers
{
    public class TablePrintingViewController : NSViewController
    {
        Type _classType;
        float _tableWidth;
        NSTableView _printView;
        readonly float scale = 10f/15f; // (8pt print font is size 10, 12pt screen font is size 15)
        readonly Dictionary<string, float> _columnWidths;
        public NSView PrintView => View;
        public nfloat TotalHeight => _printView.HeaderView.Frame.Height + _printView.Frame.Height;
        public nfloat TotalWidth => _printView.Frame.Width;

        public TablePrintingViewController(IPrintViewController tableViewController)
        {
            Title = tableViewController.Title;
            _columnWidths = tableViewController.ColumnWidths();
            _classType = tableViewController.GetGenericType();
            View = SetupView();
            _printView.Source = tableViewController.TableSource;
            _printView.SortDescriptors = tableViewController.SortDescriptors;
            _printView.ReloadData();
            View.SetFrameSize(new CGSize(_printView.Frame.Width, _printView.Frame.Height + _printView.HeaderView.Frame.Height));
        }

        public NSScrollView SetupView()
        {
            _printView = new NSTableView
            {
                Identifier = Title,
                //RowSizeStyle = NSTableViewRowSizeStyle.Default,
                Enabled = true,
                UsesAlternatingRowBackgroundColors = true,
                //ColumnAutoresizingStyle = NSTableViewColumnAutoresizingStyle.Sequential,
                WantsLayer = true,
                Layer = NewLayer(),
                Bounds = new CGRect(0, 0, 0, 0),
                //AutoresizingMask = NSViewResizingMask.HeightSizable | NSViewResizingMask.WidthSizable,
                //AutoresizesSubviews = true,
                Target = Self,
                AutosaveName = "PrintView"
            };
            _printView.HeaderView.AutoresizingMask = NSViewResizingMask.WidthSizable;
            NSProcessInfo info = new NSProcessInfo();
            if (info.IsOperatingSystemAtLeastVersion(new NSOperatingSystemVersion(10, 13, 0)))
                _printView.UsesAutomaticRowHeights = true; // only available in OSX 13 and above.

            AddTableColumns(_printView);
            var scrollView = new NSScrollView
            {
                DocumentView = _printView,
                HasVerticalScroller = false,
                HasHorizontalScroller = false,
                WantsLayer = true,
                Layer = NewLayer(),
                Bounds = new CGRect(0, 0, 0, 0)
            };
            scrollView.ContentView.AutoresizesSubviews = true;
            scrollView.ContentView.AutoresizingMask = NSViewResizingMask.HeightSizable | NSViewResizingMask.WidthSizable;
            return scrollView;
        }

        static CALayer NewLayer() => new CALayer { Bounds = new CGRect(0, 0, 0, 0) };

        internal void AddTableColumns(NSTableView view)
        {
            _tableWidth = 0f;
            var properties = _classType.GetProperties();
            foreach (var property in properties)
            {
                float width = 100;
                string columnTitle = property.Name;
                var tableColumn = new NSTableColumn
                {
                    Identifier = property.Name,
                    Width = _columnWidths[property.Name] * scale,
                    Editable = false,
                    Hidden = false,
                    Title = columnTitle,
                    //ResizingMask = NSTableColumnResizing.Autoresizing | NSTableColumnResizing.UserResizingMask
                };
                view.AddColumn(tableColumn);
                _tableWidth += width;
            }
        }
    }
}
