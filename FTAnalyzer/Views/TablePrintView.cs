using AppKit;
using CoreAnimation;
using CoreGraphics;
using Foundation;

namespace FTAnalyzer
{
    public class TablePrintView : NSView
    {
        NSTableView _tableView;
        NSTableHeaderView _headerView;
        float _headerRowSize = 60;

        public TablePrintView(NSTableView tableView, float width)
        {
             _tableView = tableView;
            _tableView.SetFrameOrigin(new CGPoint(0, _headerRowSize));
            var x = _tableView.Font;

            AutoresizesSubviews = false; 
            WantsLayer = true;
            Layer = NewLayer();
            Bounds = new CGRect(0, 0, 0, 0);
            _headerView = new NSTableHeaderView
            {
                TableView = tableView,
                WantsLayer = true,
                Layer = NewLayer(),
                Bounds = new CGRect(0, 0, 0, 0),
                Frame = new CGRect(0, 0, width, 0),
                AutoresizingMask = NSViewResizingMask.HeightSizable | NSViewResizingMask.WidthSizable
            };
            AddSubview(_headerView);
            AddSubview(_tableView);
        }

        public NSTableViewSource Source
        {
            get => _tableView.Source;
            set => _tableView.Source = value;
        }

        public NSSortDescriptor[] SortDescriptors
        {
            get => _tableView.SortDescriptors;
            set => _tableView.SortDescriptors = value;
        }

        public int RowCount => (int)_tableView.RowCount;

        public void PreparePrintView(float width, float height, NSPrintInfo info)
        {
            float headerHeight = 30;
            width += 51;
            _tableView.ReloadData();
            _tableView.SetFrameSize(new CGSize(width, height));
            _tableView.SetFrameOrigin(new CGPoint(0, 0));

            _headerView.SetFrameSize(new CGSize(width, headerHeight));
            _headerView.SetFrameOrigin(new CGPoint(0, height));
            SetFrameSize(new CGSize(width, height + headerHeight));
            //SetFrameOrigin(new CGPoint(0, 0));
            var x1 = _headerView.Frame;
            var x2 = _tableView.Frame;
            var y = Frame;
        }

        static CALayer NewLayer() => new CALayer { Bounds = new CGRect(0, 0, 0, 0) };

    }
}
