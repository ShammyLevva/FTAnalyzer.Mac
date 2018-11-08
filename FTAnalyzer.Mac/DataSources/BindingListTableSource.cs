using System;
using System.Linq;
using System.Reflection;
using AppKit;
using FTAnalyzer.Utilities;

namespace FTAnalyzer.Mac.DataSources
{
    public class BindingListTableSource<T> : NSTableViewSource
    {
        const string CellIdentifier = "TableView";

        readonly SortableBindingList<T> _bindingList;
        readonly PropertyInfo[] _properties;
        readonly string[] _fieldNames;

        public BindingListTableSource(SortableBindingList<T> bindingList)
        {
            _bindingList = bindingList;
            _properties = typeof(T).GetProperties();
            _fieldNames = typeof(T).GetProperties().Select(f => f.Name).ToArray();
        }

        public override nint GetRowCount(NSTableView tableView)
        {
            return _bindingList.Count;
        }

        public override NSView GetViewForItem(NSTableView tableView, NSTableColumn tableColumn, nint row)
        {
            if (!(tableView.MakeView(CellIdentifier, this) is NSTextField view))
            {
                view = new NSTextField
                {
                    Identifier = CellIdentifier,
                    BackgroundColor = NSColor.Clear,
                    Bordered = false,
                    Selectable = false,
                    Editable = false
                };
            }
            // Setup view based on the column selected
            if (row >= 0)
            {
                var item = _bindingList[(int)row];
                var index = Array.IndexOf(_fieldNames, tableColumn.Title);
                var propertyValue = _properties[index].GetValue(item);
                view.StringValue = propertyValue == null ? string.Empty : propertyValue.ToString();
            }
            else
                view.StringValue = string.Empty;

            return view;
        }
    }
}
