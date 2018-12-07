using AppKit;
using CoreGraphics;
using Foundation;
using FTAnalyzer.DataSources;
using FTAnalyzer.Utilities;

namespace FTAnalyzer.ViewControllers
{
    public class BindingListViewController<T> : NSViewController, IPrintViewController
    {
        public string TooltipText { get; set; }

        float _tableWidth;
        internal NSTableView _printView;
        internal NSTableView _tableView;
        internal string CountText { get; set; }

        public NSView PrintView => SetupPrintView();

        public BindingListViewController(string title, string tooltip)
        {
            Title = title;
            TooltipText = tooltip;
            SetupTableView();
            SetupPrintView();
            UpdateTooltip();
        }

        public void UpdateTooltip() => View.ToolTip = $"{CountText}. {TooltipText}";

        void SetupTableView()
        {
            _tableView = new NSTableView
            {
                Identifier = Title,
                RowSizeStyle = NSTableViewRowSizeStyle.Default,
                Enabled = true,
                UsesAlternatingRowBackgroundColors = true,
                ColumnAutoresizingStyle = NSTableViewColumnAutoresizingStyle.None,
                WantsLayer = true,
                Layer = NewLayer(),
                Bounds = new CGRect(0, 0, 0, 0),
                AutoresizingMask = NSViewResizingMask.HeightSizable | NSViewResizingMask.WidthSizable,
                AllowsMultipleSelection = false,
                AllowsColumnResizing = true,
                AutosaveName = Title,
                AutosaveTableColumns = true,
                Target = Self,
                DoubleAction = new ObjCRuntime.Selector("ViewDetailsSelector"),
                Action = new ObjCRuntime.Selector("SetRootPersonSelector")
            };
            AddTableColumns(_tableView);
            var scrollView = new NSScrollView
            {
                DocumentView = _tableView,
                HasVerticalScroller = true,
                HasHorizontalScroller = true,
                WantsLayer = true,
                Layer = NewLayer(),
                Bounds = new CGRect(0, 0, 0, 0)
            };
            View = scrollView;
        }

        public NSView SetupPrintView()
        {
            _printView = new NSTableView
            {
                Identifier = Title,
                RowSizeStyle = NSTableViewRowSizeStyle.Small,
                Enabled = true,
                UsesAlternatingRowBackgroundColors = true,
                ColumnAutoresizingStyle = NSTableViewColumnAutoresizingStyle.None,
                WantsLayer = true,
                Layer = NewLayer(),
                Bounds = new CGRect(0, 0, 0, 0),
                AutoresizingMask = NSViewResizingMask.HeightSizable | NSViewResizingMask.WidthSizable,
                Target = Self,
            };
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
            return _printView;
        }

        static CoreAnimation.CALayer NewLayer() => new CoreAnimation.CALayer { Bounds = new CGRect(0, 0, 0, 0) };

        public virtual void RefreshDocumentView(SortableBindingList<T> list)
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
            _printView.Source = source;
            _printView.ReloadData();
        }

        private void AddTableColumns(NSTableView view)
        {
            _tableWidth = 0f;
            var properties = typeof(T).GetProperties();
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
                    Width = width,
                    Editable = false,
                    Hidden = false,
                    Title = columnTitle
                };
                view.AddColumn(tableColumn);
                _tableWidth += width;
             }
        }

        [Export ("SetRootPersonSelector")]
        public void SetRootPersonSelector()
        {
            if (!NSThread.IsMain)
            {
                InvokeOnMainThread(SetRootPersonSelector);
                return;
            }
            NSTableColumn column = GetColumnID("IndividualID");
            if (column != null)
            {
                if (_tableView.Source.GetViewForItem(_tableView, column, _tableView.SelectedRow) is NSTextField cell)
                {
                    string indID = cell.StringValue;
                    Individual ind = FamilyTree.Instance.GetIndividual(indID);
                    RaiseSetRootPersonClicked(ind);
                    return;
                }
            }
        }

        [Export ("ViewDetailsSelector")]
        public virtual void ViewDetailsSelector()
        {
            if (!NSThread.IsMain)
            {
                InvokeOnMainThread(ViewDetailsSelector);
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
            column = GetColumnID("Occupation");
            if (column != null)
            {
                if (_tableView.Source.GetViewForItem(_tableView, column, _tableView.SelectedRow) is NSTextField cell)
                {
                    string occupation = cell.StringValue;
                    People people = new People();
                    people.SetWorkers(occupation, FamilyTree.Instance.AllWorkers(occupation));
                    RaiseOccupationRowClicked(people);
                    return;
                }
            }
            column = GetColumnID("ErrorType");
            if (column != null)
            {
                var source = _tableView.Source as BindingListTableSource<DataError>;
                if (source.GetRowObject(_tableView.SelectedRow) is DataError error)
                {
                    if (error.IsFamily == "Yes")
                    {
                        Family family = FamilyTree.Instance.GetFamily(error.Reference);
                        RaiseDataErrorRowClicked(null, family);
                    }
                    else
                    {
                        Individual ind = FamilyTree.Instance.GetIndividual(error.Reference);
                        RaiseDataErrorRowClicked(ind, null);
                    }
                }
            }
        }

        protected NSTableColumn GetColumnID(string identifier)
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

        protected int GetColumnIndex(string identifier)
        {
            int count = 0;
            foreach (NSTableColumn column in _tableView.TableColumns())
            {
                if (column.Identifier == identifier)
                    return count;
                count++;
            }
            return -1;
        }

        public void Sort(NSSortDescriptor[] columns)
        {
            _tableView.SortDescriptors = columns;
            _tableView.ReloadData();
        }

        #region Events
        public delegate void IndividualRowClickedDelegate(Individual individual);
        public delegate void FamilyRowClickedDelegate(Family family);
        public delegate void SourceRowClickedDelegate(FactSource source);
        public delegate void OccupationRowClickedDelegate(People people);
        public delegate void DataErrorRowClickedDelegate(Individual individual, Family family);
        public delegate void SetRootPersonDelegate(Individual individual);
        public event IndividualRowClickedDelegate IndividualFactRowClicked;
        public event FamilyRowClickedDelegate FamilyFactRowClicked;
        public event SourceRowClickedDelegate SourceFactRowClicked;
        public event OccupationRowClickedDelegate OccupationRowClicked;
        public event DataErrorRowClickedDelegate DataErrorRowClicked;
        public event SetRootPersonDelegate SetRootPersonClicked;

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
        internal void RaiseOccupationRowClicked(People people)
        {
            OccupationRowClicked?.Invoke(people);
        }
        internal void RaiseDataErrorRowClicked(Individual individual, Family family)
        {
            DataErrorRowClicked?.Invoke(individual,family);
        }
        internal void RaiseSetRootPersonClicked(Individual individual)
        {
            SetRootPersonClicked?.Invoke(individual);
        }

        public void RefreshView()
        {
            throw new System.NotImplementedException();
        }
        #endregion
    }
}
