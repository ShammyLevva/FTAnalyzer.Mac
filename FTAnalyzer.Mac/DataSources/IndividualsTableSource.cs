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

        public override NSCell GetCell(NSTableView tableView, NSTableColumn tableColumn, nint row)
        {
            NSCell cell = base.GetCell(tableView, tableColumn, row);
            // set value of cell based on column and row
            return cell;
        }
    }
}
