using System;
using AppKit;
using Foundation;

namespace FTAnalyzer.Mac.DataSources
{
    public class IndividualsTableSource : NSTableViewSource
    {
        FamilyTree _familyTree;
        Utilities.SortableBindingList<IDisplayIndividual> _individuals;
        string CellIdentifier = "TableCell";

        public IndividualsTableSource()
        {
            _familyTree = FamilyTree.Instance;
            _individuals = _familyTree.AllDisplayIndividuals;
        }

        public override nint GetRowCount(NSTableView tableView)
        {
            return _individuals.Count;
        }

        public override NSView GetViewForItem(NSTableView tableView, NSTableColumn tableColumn, nint row)
        {
            var view = tableView.MakeView(CellIdentifier, this) as NSTextField;
            if (view == null)
            {
                view = new NSTextField()
                {
                    Identifier = CellIdentifier,
                    BackgroundColor = NSColor.Clear,
                    Bordered = false,
                    Selectable = false,
                    Editable = false
                };
            }

            // Setup view based on the column selected
            switch (tableColumn.Title)
            {
                case "ID":
                    view.StringValue = _individuals[(int)row].IndividualID;
                    break;
                case "Forenames":
                    view.StringValue = _individuals[(int)row].Forenames;
                    break;
                case "Surname":
                    view.StringValue = _individuals[(int)row].Surname;
                    break;
            }

            return view;
        }
    }
}
