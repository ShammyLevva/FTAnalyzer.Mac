using System.Data;
using System.Drawing;
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
            if (cellview != null)
                SetCellView(cellview, tableColumn, row);
            return cellview;
        }

        internal NSTableCellView GetFTAnalyzerGridCell(NSTableView tableView, NSTableColumn tableColumn, nint row)
        {
            int index = Array.IndexOf(_fieldNames, tableColumn.Identifier);
            if (index < 0 || index > _properties.Length)
                return null;
            PropertyInfo property = _properties[index];
            NSTextAlignment alignment = NSTextAlignment.Left;
            nfloat width = tableColumn.Width;
            ColumnDetail[]? x = property.GetCustomAttributes(typeof(ColumnDetail), false) as ColumnDetail[];
            if (x?.Length == 1)
            {
                alignment = x[0].Alignment;
                width = x[0].ColumnWidth;
            }
            if (!(tableView.MakeView(CellIdentifier, this) is NSTableCellView cellView))
            {
                tableView.IntercellSpacing = new SizeF(0, 0); //KI: Reduces spacing round textfield to make coloured boxes pretty
                NSTextField textField = new NSTextField
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
                //KI: Put border round coloured boxes so they look pretty when the row is highlighted (and coloured blue by the system)
                //KI: This is a hack. "C1", etc. must be census column
                if (tableColumn.Identifier.StartsWith("C1") || tableColumn.Identifier.StartsWith("US1") || tableColumn.Identifier.StartsWith("Can1") || tableColumn.Identifier.StartsWith("Ire1") || tableColumn.Identifier.StartsWith("V1"))
                    textField.Bordered = true;
                //KI: This is a hack. "Birth", etc. must be BMD column
                if (tableColumn.Identifier == "Birth" || tableColumn.Identifier == "BaptChri" || tableColumn.Identifier == "Marriage1" || tableColumn.Identifier == "Marriage2" || tableColumn.Identifier == "Marriage3" || tableColumn.Identifier == "Death" || tableColumn.Identifier == "CremBuri")
                    textField.Bordered = true;
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
                NSMutableDictionary views = new NSMutableDictionary
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
            int index = Array.IndexOf(_fieldNames, tableColumn.Identifier);
            // Set cell view content based on the column selected
            if (row >= 0)
            {
                T item = _bindingList[(int)row];
                object? propertyValue = _properties[index].GetValue(item);
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
                IComparer<T> comparer = _bindingList.First().GetComparer(key, ascending);
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
                object[] values = new object[dataTable.Columns.Count];
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
