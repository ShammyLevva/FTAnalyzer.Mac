using System.Data;
using System.Reflection;
using FTAnalyzer.Utilities;

namespace FTAnalyzer.DataSources
{
    public class BindingListTableSource<T> : NSTableViewSource where T : IColumnComparer<T>
    {
        internal const string CellIdentifier = "TableView";
        internal readonly SortableBindingList<T> _bindingList;
        internal readonly PropertyInfo[] _properties;
        internal readonly string[] _fieldNames;
 
        public BindingListTableSource(SortableBindingList<T> bindingList)
        {
            _bindingList = bindingList;
            _properties = typeof(T).GetProperties();
            _fieldNames = typeof(T).GetProperties().Select(f => f.Name).ToArray();
        }

        public override nint GetRowCount(NSTableView tableView) => _bindingList.Count;

        public override NSView GetViewForItem(NSTableView tableView, NSTableColumn tableColumn, nint row)
        {
            NSTableCellView cellview = GetFTAnalyzerGridCell(tableView, tableColumn, row);
            if(cellview != null)
                SetCellView(cellview, tableColumn, row);
            return cellview;
        }

        internal NSTableCellView GetFTAnalyzerGridCell(NSTableView tableView, NSTableColumn tableColumn, nint row)
        {
            var index = Array.IndexOf(_fieldNames, tableColumn.Identifier);
            if (index < 0 || index > _properties.Length)
                return null;
            var property = _properties[index];
            NSTextAlignment alignment = NSTextAlignment.Left;
            var width = tableColumn.Width;
            ColumnDetail[]? x = property.GetCustomAttributes(typeof(ColumnDetail), false) as ColumnDetail[];
            if (x?.Length == 1)
            {
                alignment = x[0].Alignment;
                width = x[0].ColumnWidth;
            }
            if (!(tableView.MakeView(CellIdentifier, this) is NSTableCellView cellView))
            {
                var textField = new NSTextField
                {
                    BackgroundColor = NSColor.Clear,
                    LineBreakMode = NSLineBreakMode.TruncatingTail,
                    Bordered = false,
                    Selectable = false,
                    Editable = false,
                    Alignment = alignment,
                    AutoresizingMask = NSViewResizingMask.MinYMargin | NSViewResizingMask.WidthSizable,
                    AutoresizesSubviews = true,
                    TranslatesAutoresizingMaskIntoConstraints = false,
                    AllowsDefaultTighteningForTruncation = true,
               };
                if (tableView.AutosaveName == "PrintView")
                    textField.Font = NSFont.SystemFontOfSize(8);
                cellView = new NSTableCellView
                {
                    Identifier = CellIdentifier,
                    TextField = textField,
                    AutoresizesSubviews = true,
                    AutoresizingMask = NSViewResizingMask.HeightSizable | NSViewResizingMask.WidthSizable
                };
                cellView.AddSubview(textField);
                var views = new NSMutableDictionary
                {
                    { new NSString("textField"), textField }
                };
                cellView.AddConstraints(NSLayoutConstraint.FromVisualFormat($"H:|[textField({width}@750)]|", NSLayoutFormatOptions.AlignAllTop, null, views));
                cellView.AddConstraints(NSLayoutConstraint.FromVisualFormat("V:|[textField]|", NSLayoutFormatOptions.AlignAllTop, null, views));
                NSLayoutConstraint.ActivateConstraints(cellView.Constraints);
            }
            return cellView;
        }

        void SetCellView(NSTableCellView cellView, NSTableColumn tableColumn, nint row)
        {
            var index = Array.IndexOf(_fieldNames, tableColumn.Identifier);
            // Set cell view content based on the column selected
            if (row >= 0)
            {
                var item = _bindingList[(int)row];
                var propertyValue = _properties[index].GetValue(item);
                cellView.TextField.StringValue = propertyValue == null ? string.Empty : propertyValue.ToString();
            }
            else
                cellView.TextField.StringValue = string.Empty;
        }

        public object GetRowObject(nint row) => _bindingList[(int)row];

        void Sort(string key, bool ascending)
        {
            if (_bindingList.Any()) // only sort if array contains something
            {
                var comparer = _bindingList.First().GetComparer(key, ascending);
                if (comparer != null)
                    _bindingList.Sort(comparer);
            }
        }

        public override void SortDescriptorsChanged(NSTableView tableView, NSSortDescriptor[] oldDescriptors)
        {
            // Grab current descriptors and update sort
            NSSortDescriptor[] tbSort = tableView.SortDescriptors;
            if (tbSort.Length > 0)
            {
                Sort(tbSort[0].Key, tbSort[0].Ascending);
                tableView.ReloadData();
            }
        }

        public DataTable GetDataTable()
        {
            DataTable dataTable = new(typeof(T).Name);
            foreach (string fieldName in _fieldNames)
                dataTable.Columns.Add(fieldName);
            foreach (T item in _bindingList)
            {
                var values = new object[dataTable.Columns.Count];
                for (int i = 0; i < _properties.Length; i++)
                {
                    values[i] = _properties[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            return dataTable;
        }
    }
}
