using System;
using AppKit;
using Foundation;
using FTAnalyzer.Mac.DataSources;
using FTAnalyzer.Utilities;

namespace FTAnalyzer.Mac.ViewControllers
{
    public class BindingListViewController<T> : NSViewController
    {
        NSTableView _tableView;
        const string CellIdentifier = "TableView";

        public override void LoadView()
        {
            //base.LoadView();

            _tableView = new NSTableView
            {
                Identifier = CellIdentifier,
                RowSizeStyle = NSTableViewRowSizeStyle.Default,
                Enabled = true,
                UsesAlternatingRowBackgroundColors = true,
                ColumnAutoresizingStyle = NSTableViewColumnAutoresizingStyle.Sequential,
                Bounds = new CoreGraphics.CGRect(0, 0, 500, 500),
                AutoresizingMask = NSViewResizingMask.HeightSizable | NSViewResizingMask.WidthSizable
            };

            var properties = typeof(T).GetProperties();
            foreach (var property in properties)
            {
                var tableColumn = new NSTableColumn
                {
                    Identifier = CellIdentifier,
                    Width = 100,
                    Editable = false,
                    Hidden = false,
                    Title = property.Name
                };
                _tableView.AddColumn(tableColumn);
            }
            var scrollView = new NSScrollView
            {
                DocumentView = _tableView
            };
            View = scrollView;
        }

        public void RefreshDocumentView(SortableBindingList<T> list)
        {
            if (!NSThread.IsMain)
            {
                InvokeOnMainThread(() => RefreshDocumentView(list));
                return;
            }

            var source = new BindingListTableSource<T>(list);
            _tableView.Source = source;
            _tableView.ReloadData();
        }

    }
}
