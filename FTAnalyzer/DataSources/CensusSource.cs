using System;
using AppKit;
using FTAnalyzer.Utilities;

namespace FTAnalyzer.DataSources
{
    public class CensusSource : BindingListTableSource<IDisplayCensus>
    {
        public CensusSource(SortableBindingList<IDisplayCensus> reportList) : base(reportList)
        {
        }

        public override NSView GetViewForItem(NSTableView tableView, NSTableColumn tableColumn, nint row)
        {
            var index = Array.IndexOf(_fieldNames, tableColumn.Identifier);
            NSTableCellView cellView = GetFTAnalyzerGridCell(tableView, tableColumn, row);
            SetTextField(cellView.TextField, tableColumn, row);
            return cellView;
        }

        void SetTextField(NSTextField view, NSTableColumn tableColumn, nint row)
        {
            // Setup view based on the column selected
            var index = Array.IndexOf(_fieldNames, tableColumn.Identifier);
            if (row >= 0)
            {
                var item = _bindingList[(int)row];
                var propertyValue = _properties[index].GetValue(item);
                view.StringValue = propertyValue == null ? string.Empty : propertyValue.ToString();
            }
            else
                view.StringValue = string.Empty;
        }

    }
}
