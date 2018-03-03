using System;
using AppKit;
using Foundation;

namespace FTAnalyzer.Mac.DataSources
{
    public class IndividualsTableSource : NSTableViewSource
    {
        FamilyTree _familyTree;
        readonly Utilities.SortableBindingList<IDisplayIndividual> _individuals;
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
            var individual = _individuals[(int)row];
            switch (tableColumn.Title)
            {
                case "ID":
                    view.StringValue = individual.IndividualID;
                    break;
                case "Forenames":
                    view.StringValue = individual.Forenames;
                    break;
                case "Surname":
                    view.StringValue = individual.Surname;
                    break;
                case "Gender":
                    view.StringValue = individual.Gender;
                    break;
                case "BirthDate":
                    view.StringValue = individual.BirthDate.ToString();
                    break;
                case "BirthLocation":
                    view.StringValue = individual.BirthLocation.ToString();
                    break;
                case "DeathDate":
                    view.StringValue = individual.DeathDate.ToString();
                    break;
                case "DeathLocation":
                    view.StringValue = individual.DeathLocation.ToString();
                    break;
                case "Occupation":
                    view.StringValue = individual.Occupation;
                    break;
                case "LifeSpan":
                    view.StringValue = individual.LifeSpan.ToString();
                    break;
                case "Relation":
                    view.StringValue = individual.Relation;
                    break;
                case "RelationToRoot":
                    view.StringValue = individual.RelationToRoot;
                    break;
                case "MarriageCount":
                    view.StringValue = individual.MarriageCount.ToString();
                    break;
                case "ChildrenCount":
                    view.StringValue = individual.ChildrenCount.ToString();
                    break;
                case "BudgieCode":
                    view.StringValue = individual.BudgieCode;
                    break;
                case "Ahnentafel":
                    view.StringValue = individual.Ahnentafel.ToString();
                    break;
                case "HasNotes":
                    view.StringValue = individual.HasNotes ? "Yes" : "No";
                    break;
            }

            return view;
        }
    }
}
