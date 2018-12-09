using System;
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
        readonly float _headerRowSize = 30;

        public TablePrintView(NSTableView tableView, float width)
        {
            _tableView = tableView;
            AutoresizesSubviews = true; 
            WantsLayer = true;
            Layer = NewLayer();
            Bounds = new CGRect(0, 0, 0, 0);
            _headerView = new NSTableHeaderView
            {
                TableView = tableView,
                WantsLayer = true,
                Layer = NewLayer(),
                Bounds = new CGRect(0, 0, 0, 0),
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

        public void PreparePrintView()
        {
            _tableView.ReloadData();
            SetFrameSizes();
        }

        static CALayer NewLayer() => new CALayer { Bounds = new CGRect(0, 0, 0, 0) };

        public override void ViewWillDraw()
        {
            SetFrameSizes();
            base.ViewWillDraw();
        }

        private void SetFrameSizes()
        {
            _tableView.SetFrameOrigin(new CGPoint(0, 0));
            _headerView.SetFrameSize(new CGSize(_tableView.Frame.Width, _headerRowSize));
            _headerView.SetFrameOrigin(new CGPoint(0, _tableView.Frame.Height));
            SetFrameSize(new CGSize(_tableView.Frame.Width, _tableView.Frame.Height + _headerRowSize));
        }
    }
}
