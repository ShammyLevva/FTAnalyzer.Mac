using CoreAnimation;
using FTAnalyzer.Utilities;
using FTAnalyzer.Views;

namespace FTAnalyzer.ViewControllers
{
    public class TablePrintingViewController : NSViewController
    {
        Type _classType;
        float _tableWidth;
        NSTableView _printView;
        readonly Dictionary<string, float> _columnWidths;
        readonly Dictionary<string, bool> _columnVisibility;
        public NSView PrintView => View;
        public nfloat TotalHeight => _printView.HeaderView.Frame.Height + _printView.Frame.Height;
        public nfloat TotalWidth => _printView.Frame.Width;

        public TablePrintingViewController(IPrintViewController tableViewController)
        {
            Title = tableViewController.Title;
            _columnWidths = tableViewController.ColumnWidths();
            _columnVisibility = tableViewController.ColumnVisibility();
            _classType = tableViewController.GetGenericType();
            View = SetupView();
            _printView.Source = tableViewController.TableSource;
            _printView.SortDescriptors = tableViewController.SortDescriptors;
            _printView.ReloadData();
            View.SetFrameSize(new CGSize(_printView.Frame.Width, _printView.Frame.Height + _printView.HeaderView.Frame.Height));
        }

        public NSScrollView SetupView()
        {
            _printView = new GridTableView("PrintView", Self);
            AddTableColumns(_printView);
            NSProcessInfo info = new();
            if (info.IsOperatingSystemAtLeastVersion(new NSOperatingSystemVersion(10, 13, 0)))
                _printView.UsesAutomaticRowHeights = true; // only available in OSX 13 and above.
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
            scrollView.ContentView.AutoresizesSubviews = true;
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
                ColumnDetail[]? columnDetail = property.GetCustomAttributes(typeof(ColumnDetail), false) as ColumnDetail[];
                if (columnDetail?.Length == 1)
                   columnTitle = columnDetail[0].ColumnName;
                var tableColumn = new NSTableColumn
                {
                    Identifier = property.Name,
                    Width = _columnWidths[property.Name],
                    Editable = false,
                    Hidden = _columnVisibility[property.Name],
                    Title = columnTitle,
                    ResizingMask = NSTableColumnResizing.Autoresizing | NSTableColumnResizing.UserResizingMask
                };
                view.AddColumn(tableColumn);
                _tableWidth += width;
            }
        }
    }
}
