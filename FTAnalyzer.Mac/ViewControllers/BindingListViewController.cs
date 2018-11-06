using AppKit;
using Foundation;
using FTAnalyzer.Mac.DataSources;
using FTAnalyzer.Utilities;

namespace FTAnalyzer.Mac.ViewControllers
{
    public class BindingListViewController<T> : NSViewController
    {
        internal NSTableView _tableView;

        public BindingListViewController(string title)
        {
            SetupView(title);
            Title = title;
        }

        void SetupView(string title)
        {
            _tableView = new NSTableView
            {
                Identifier = title,
                RowSizeStyle = NSTableViewRowSizeStyle.Default,
                Enabled = true,
                UsesAlternatingRowBackgroundColors = true,
                ColumnAutoresizingStyle = NSTableViewColumnAutoresizingStyle.None,
                Bounds = new CoreGraphics.CGRect(0, 0, 500, 500),
                AutoresizingMask = NSViewResizingMask.HeightSizable | NSViewResizingMask.WidthSizable,
                AllowsMultipleSelection = false,
                AutosaveName = title,
                AllowsColumnResizing = true,
                AutosaveTableColumns = true,
                Target = Self,
                DoubleAction = new ObjCRuntime.Selector("ViewFactsSelector")
            };

            var properties = typeof(T).GetProperties();
            foreach (var property in properties)
            {
                var tableColumn = new NSTableColumn
                {
                    Identifier = property.Name,
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

        public virtual void RefreshDocumentView(SortableBindingList<T> list)
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

        [Export ("ViewFactsSelector")]
        public void ViewFactsSelector()
        {
            if (!NSThread.IsMain)
            {
                InvokeOnMainThread(ViewFactsSelector);
                return;
            }
            NSTableColumn column = GetColumnID("IndividualID");
            if (column != null)
            {
                if (_tableView.Source.GetViewForItem(_tableView, column, _tableView.SelectedRow) is NSTextField cell)
                {
                    string indID = cell.StringValue;
                    Individual ind = FamilyTree.Instance.GetIndividual(indID);

                    var storyboard = NSStoryboard.FromName("Facts", null);
                    var factsWindowController = storyboard.InstantiateControllerWithIdentifier("FactsWindow") as FactsWindowController;
                    factsWindowController.ContentViewController = new FactsViewController<Individual>($"Facts View for {ind.Name}", ind);
                    factsWindowController.ShowWindow(this);
                }
            }
        }

        NSTableColumn GetColumnID(string identifier)
        {
            int count = 0;
            foreach (NSTableColumn column in _tableView.TableColumns())
            {
                if (column.Identifier == identifier)
                    return column;
                count++;
            }
            return null;
        }

    }
}
