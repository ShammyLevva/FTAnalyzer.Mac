using System;
using AppKit;
using CoreAnimation;
using CoreGraphics;
using Foundation;
using FTAnalyzer.Utilities;

namespace FTAnalyzer
{
    public class TablePrintView : NSView
    {
        NSTableView _tableView;
        NSTableHeaderView _headerView;
        readonly float _headerRowSize = 30;
        CustomPrintPanel _printPanel;

        public TablePrintView(NSTableView tableView)
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

        public void PreparePrintView(CustomPrintPanel printPanel)
        {
            _printPanel = printPanel;
            _tableView.ReloadData();
            SetFrameSizes();
        }

        static CALayer NewLayer() => new CALayer { Bounds = new CGRect(0, 0, 0, 0) };

        public override void ViewWillDraw()
        {
            //ResizeColumns();
            SetFrameSizes();
            base.ViewWillDraw();
            _printPanel.Refresh = true;
        }

        void SetFrameSizes()
        {
            _tableView.SetFrameOrigin(new CGPoint(0, 0));
            _headerView.SetFrameSize(new CGSize(_tableView.Frame.Width, _headerRowSize));
            _headerView.SetFrameOrigin(new CGPoint(0, _tableView.Frame.Height));
            SetFrameSize(new CGSize(_tableView.Frame.Width, _tableView.Frame.Height + _headerRowSize));
            var head = _headerView.Frame;
            var body = _tableView.Frame;
            var overall = Frame;
        }

        void ResizeColumns()
        {
            foreach(NSTableColumn col in _tableView.TableColumns())
            {
                col.Width = 123; // TODO: Set width as whatever user has set width of column on table adjusted by scaling of screen font vs printing font.
            }
        }
    }
}
