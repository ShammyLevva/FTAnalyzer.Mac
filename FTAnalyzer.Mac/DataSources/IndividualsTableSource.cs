using System;
using System.Linq;
using System.Reflection;
using AppKit;
using Foundation;

namespace FTAnalyzer.Mac.DataSources
{
    [Register ("IndividualsTableSource")]
    public class IndividualsTableSource : NSTableViewSource
    {
        FamilyTree _familyTree;
        readonly Utilities.SortableBindingList<IDisplayIndividual> _individuals;
        readonly PropertyInfo[] _properties;
        readonly string[] _fieldNames;

        string CellIdentifier = "TableView";

        public IndividualsTableSource()
        {
            _familyTree = FamilyTree.Instance;
            _individuals = _familyTree.AllDisplayIndividuals;
            _properties = typeof(IDisplayIndividual).GetProperties();
            _fieldNames = typeof(IDisplayIndividual).GetProperties().Select(f => f.Name).ToArray();

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
            int indexID = Array.IndexOf(_fieldNames, tableColumn.Title);
            var propertyValue = _properties[indexID].GetValue(individual);
            view.StringValue = propertyValue.ToString();

            return view;
        }
    }
}
