﻿using System;
using System.Linq;
using System.Reflection;
using AppKit;
using FTAnalyzer.Utilities;

namespace FTAnalyzer.DataSources
{
    public class BindingListTableSource<T> : NSTableViewSource
    {
        const string CellIdentifier = "TableView";

        readonly SortableBindingList<T> _bindingList;
        readonly PropertyInfo[] _properties;
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

        internal NSView GetFTAnalyzerGridCell(NSTableView tableView, NSTableColumn tableColumn, nint row)
        {
            var index = Array.IndexOf(_fieldNames, tableColumn.Identifier);
            var property = _properties[index];
            NSTextAlignment alignment = NSTextAlignment.Left;
            ColumnDetail[] x = property.GetCustomAttributes(typeof(ColumnDetail), false) as ColumnDetail[];
            if (x?.Length == 1)
                alignment = x[0].Alignment;

            if (!(tableView.MakeView(CellIdentifier, this) is NSTextField view))
            {
                view = new NSTextField
                {
                    Identifier = CellIdentifier,
                    BackgroundColor = NSColor.Clear,
                    Bordered = false,
                    Selectable = false,
                    Editable = false,
                    Alignment = alignment
                };
            }
            // Setup view based on the column selected
            if (row >= 0)
            {
                var item = _bindingList[(int)row];
                var propertyValue = _properties[index].GetValue(item);
                view.StringValue = propertyValue == null ? string.Empty : propertyValue.ToString();
            }
            else
                view.StringValue = string.Empty;
            return view;
        }

        public object GetRowObject(nint row) => _bindingList[(int)row];
    }
}