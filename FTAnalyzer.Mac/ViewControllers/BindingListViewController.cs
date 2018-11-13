using System;
using AppKit;
using Foundation;
using FTAnalyzer.Mac.DataSources;
using FTAnalyzer.Utilities;

namespace FTAnalyzer.Mac.ViewControllers
{
    public class BindingListViewController<T> : NSViewController
    {
        internal NSTableView _tableView;
        string CountText { get; set; }
        public string TooltipText { get; set; }

        public BindingListViewController(string title, string tooltip)
        {
            //UIHelpers.ShowMessage($"Creating window: {title}");
            SetupView(title);
            Title = title;
            TooltipText = tooltip;
            //UIHelpers.ShowMessage($"window: {title} created");
        }

        public void UpdateTooltip()
        {
            View.ToolTip = $"{CountText}. {TooltipText}";
        }

        void SetupView(string title)
        {
            //UIHelpers.ShowMessage($"Setting view for window: {title}");
            //_tableView = new NSTableView
            //{
            //    Identifier = title,
            //    RowSizeStyle = NSTableViewRowSizeStyle.Default,
            //    Enabled = true,
            //    UsesAlternatingRowBackgroundColors = true,
            //    ColumnAutoresizingStyle = NSTableViewColumnAutoresizingStyle.None,
            //    WantsLayer = true,
            //    Layer = new CoreAnimation.CALayer(),
            //    Bounds = new CoreGraphics.CGRect(0, 0, 500, 500),
            //    AutoresizingMask = NSViewResizingMask.HeightSizable | NSViewResizingMask.WidthSizable,
            //    AllowsMultipleSelection = false,
            //    AllowsColumnResizing = true,
            //    //AutosaveName = title,
            //    //AutosaveTableColumns = true,
            //    Target = Self,
            //    DoubleAction = new ObjCRuntime.Selector("ViewFactsSelector")
            //};
            _tableView = new NSTableView
            {
                Identifier = title,
                RowSizeStyle = NSTableViewRowSizeStyle.Default,
                Enabled = true,
                UsesAlternatingRowBackgroundColors = true,
                ColumnAutoresizingStyle = NSTableViewColumnAutoresizingStyle.None,
                WantsLayer = true,
                Layer = new CoreAnimation.CALayer()
            };
            //UIHelpers.ShowMessage("layer set");
            _tableView.Layer.Bounds = new CoreGraphics.CGRect(0, 0, 0, 0);
            //UIHelpers.ShowMessage("layer Bounds set");
            //UIHelpers.ShowMessage($"layer set\\n\nnx={(nfloat.IsNaN(_tableView.Layer.Bounds.X) ? "NaN" : _tableView.Layer.Bounds.X.ToString())}" +
                                  //$"\ny={(nfloat.IsNaN(_tableView.Layer.Bounds.Y) ? "NaN" : _tableView.Layer.Bounds.Y.ToString())}" +
                                  //$"\nwidth={(nfloat.IsNaN(_tableView.Layer.Bounds.Width) ? "NaN" : _tableView.Layer.Bounds.Width.ToString())}" +
                                  //$"\nheight={(nfloat.IsNaN(_tableView.Layer.Bounds.Height) ? "NaN" : _tableView.Layer.Bounds.Height.ToString())}" +
                                  //$"\ntop={(nfloat.IsNaN(_tableView.Layer.Bounds.Top) ? "NaN" : _tableView.Layer.Bounds.Top.ToString())}" +
                                  //$"\nbottom={(nfloat.IsNaN(_tableView.Layer.Bounds.Bottom) ? "NaN" : _tableView.Layer.Bounds.Bottom.ToString())}" +
                                  //$"\nleft={(nfloat.IsNaN(_tableView.Layer.Bounds.Left) ? "NaN" : _tableView.Layer.Bounds.Left.ToString())}" +
                                  //$"\nright={(nfloat.IsNaN(_tableView.Layer.Bounds.Right) ? "NaN" : _tableView.Layer.Bounds.Right.ToString())}");
            _tableView.Bounds = new CoreGraphics.CGRect(0, 0, 0, 0);
            //UIHelpers.ShowMessage("table Bounds set");
            _tableView.AutoresizingMask = NSViewResizingMask.HeightSizable | NSViewResizingMask.WidthSizable;
            //UIHelpers.ShowMessage($"resizing mask set");
            _tableView.AllowsMultipleSelection = false;
            //UIHelpers.ShowMessage($"multiselect = false");
            _tableView.AllowsColumnResizing = true;
            //UIHelpers.ShowMessage($"allow column resizing = true");
            //AutosaveName = title,
            //AutosaveTableColumns = true,
            _tableView.Target = Self;
            //UIHelpers.ShowMessage($"target set");
            _tableView.DoubleAction = new ObjCRuntime.Selector("ViewFactsSelector");

            //UIHelpers.ShowMessage($"View created for window: {title}");
            var properties = typeof(T).GetProperties();
            float width;
            foreach (var property in properties)
            {
                width = 100;
                ColumnWidth[] x = property.GetCustomAttributes(typeof(ColumnWidth),false) as ColumnWidth[];
                if (x?.Length == 1)
                    width = x[0].ColWidth;
                var tableColumn = new NSTableColumn
                {
                    Identifier = property.Name,
                    Width = width,
                    Editable = false,
                    Hidden = false,
                    Title = property.Name
                };
                _tableView.AddColumn(tableColumn);
            }
            //UIHelpers.ShowMessage($"Properties created for window: {title}");

            var scrollView = new NSScrollView
            {
                DocumentView = _tableView,
                HasVerticalScroller = true,
                HasHorizontalScroller = true,
                WantsLayer = true,
                Layer = new CoreAnimation.CALayer(),
            };
            //UIHelpers.ShowMessage($"Created scroll view for window: {title}");
            scrollView.Layer.Bounds = new CoreGraphics.CGRect(0, 0, 0, 0);
            //UIHelpers.ShowMessage($"set scroll view layer bounds");
            scrollView.Bounds = new CoreGraphics.CGRect(0, 0, 0, 0);
            //UIHelpers.ShowMessage($"set scroll view bounds");
            View = scrollView;
            //UIHelpers.ShowMessage($"Assigned Scroll view for window: {title}");
        }

        public void RefreshDocumentView(SortableBindingList<T> list)
        {
            if (!NSThread.IsMain)
            {
                InvokeOnMainThread(() => RefreshDocumentView(list));
                return;
            }
            CountText = $"Count: {list.Count}";
            UpdateTooltip();

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
                    RaiseFactRowClicked(ind);
                    return;
                }
            } 
            column = GetColumnID("FamilyID");
            if (column != null)
            {
                if (_tableView.Source.GetViewForItem(_tableView, column, _tableView.SelectedRow) is NSTextField cell)
                {
                    string familyID = cell.StringValue;
                    Family family = FamilyTree.Instance.GetFamily(familyID);
                    RaiseFactRowClicked(family);
                    return;
                }
            }
            column = GetColumnID("SourceID");
            if (column != null)
            {
                if (_tableView.Source.GetViewForItem(_tableView, column, _tableView.SelectedRow) is NSTextField cell)
                {
                    string sourceID = cell.StringValue;
                    FactSource source = FamilyTree.Instance.GetSource(sourceID);
                    RaiseFactRowClicked(source);
                    return;
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

        #region Events
        public delegate void IndividualRowClickedDelegate(Individual individual);
        public delegate void FamilyRowClickedDelegate(Family family);
        public delegate void SourceRowClickedDelegate(FactSource source);
        public event IndividualRowClickedDelegate IndividualFactRowClicked;
        public event FamilyRowClickedDelegate FamilyFactRowClicked;
        public event SourceRowClickedDelegate SourceFactRowClicked;

        internal void RaiseFactRowClicked(Individual individual)
        {
            // Inform caller
            IndividualFactRowClicked?.Invoke(individual);
        }
        internal void RaiseFactRowClicked(Family family)
        {
            // Inform caller
            FamilyFactRowClicked?.Invoke(family);
        }
        internal void RaiseFactRowClicked(FactSource source)
        {
            // Inform caller
            SourceFactRowClicked?.Invoke(source);
        }
        #endregion
    }
}
