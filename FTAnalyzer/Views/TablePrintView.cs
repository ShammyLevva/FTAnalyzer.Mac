using AppKit;
using CoreAnimation;
using CoreGraphics;
using Foundation;

namespace FTAnalyzer
{
    public class TablePrintView : NSView
    {
        NSTableView _tableView;

        public TablePrintView(NSTableView tableView)
        {
            var x = this;
            _tableView = tableView;
            WantsLayer = true;
            Layer = NewLayer();
            Hidden = false;
            base.Bounds = new CGRect(tableView.Bounds.Location, tableView.Bounds.Size);
            //var headerView = new NSTableHeaderView
            //{
            //    TableView = tableView,
            //    WantsLayer = true,
            //    Layer = NewLayer(),
            //    Bounds = new CGRect(0, 0, 0, 0)
            //};
            //AddSubview(headerView);
            AddSubview(tableView);
        }

        static CALayer NewLayer() => new CALayer { Bounds = new CGRect(0, 0, 0, 0) };

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

        public override CGRect Bounds { get => base.Bounds; set => base.Bounds = value; }

        public int RowCount => (int)_tableView.RowCount;

        public void PreparePrintView(float width, float height)
        {
            Hidden = false;
            var x1 = Bounds;
            var y1 = Layer.Bounds;
            var a1 = VisibleRect();

            _tableView.ReloadData();
            SetFrameSize(new CGSize(width, height));
            Layer.Bounds = new CGRect(0,0, width, height);
            SetBoundsSize(new CGSize(width, height));
           
            var x2 = Bounds;
            var y2 = Layer.Bounds;

            var a2 = VisibleRect();
        }
    }
}
