using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AppKit;
using Foundation;
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
            return GetFTAnalyzerGridCell(tableView, tableColumn, row);
        }

        NSView GetFTAnalyzerGridCell(NSTableView tableView, NSTableColumn tableColumn, nint row)
        {
            var index = Array.IndexOf(_fieldNames, tableColumn.Identifier);
            if (index < 0 || index > _properties.Length)
                return null;
            var property = _properties[index];
            NSTextAlignment alignment = NSTextAlignment.Left;
            ColumnDetail[] x = property.GetCustomAttributes(typeof(ColumnDetail), false) as ColumnDetail[];
            if (x?.Length == 1)
                alignment = x[0].Alignment;

            if (!(tableView.MakeView(CellIdentifier, this) is NSTableCellView cellView))
            {
                var textField = new NSTextField
                {
                    BackgroundColor = NSColor.Clear,
                    LineBreakMode = NSLineBreakMode.TruncatingTail,
                    NeedsLayout = true,
                    Bordered = false,
                    Selectable = false,
                    Editable = false,
                    Alignment = alignment,
                    AutoresizingMask = NSViewResizingMask.HeightSizable | NSViewResizingMask.WidthSizable,
                    TranslatesAutoresizingMaskIntoConstraints = false
                };
                if (tableView.AutosaveName == "PrintView")
                    textField.Font = NSFont.SystemFontOfSize(8);
                cellView = new NSTableCellView
                {
                    Identifier = CellIdentifier,
                    TextField = textField
                };
                cellView.AddSubview(textField);
                var views = new NSMutableDictionary
                {
                    { new NSString("textField"), textField }
                };
                cellView.AddConstraints(NSLayoutConstraint.FromVisualFormat("H:|[textField]|", NSLayoutFormatOptions.AlignAllTop, null, views));
                cellView.AddConstraints(NSLayoutConstraint.FromVisualFormat("V:|[textField]|", NSLayoutFormatOptions.AlignAllTop, null, views));
                NSLayoutConstraint.ActivateConstraints(cellView.Constraints);
            }
            // Setup view based on the column selected
            if (row >= 0)
            {
                var item = _bindingList[(int)row];
                var propertyValue = _properties[index].GetValue(item);
                cellView.TextField.StringValue = propertyValue == null ? string.Empty : propertyValue.ToString();
            }
            else
                cellView.TextField.StringValue = string.Empty;
            return cellView;
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
    }
}
