using FTAnalyzer.DataSources;
using FTAnalyzer.Utilities;

namespace FTAnalyzer.ViewControllers
{
    public partial class CensusViewController : BindingListViewController<IDisplayCensus>
    {
        public CensusViewController(IntPtr handle) : base(string.Empty, string.Empty)
        {
        }

        public override void RefreshDocumentView(SortableBindingList<IDisplayCensus> list)
        {
            if (!NSThread.IsMain)
            {
                InvokeOnMainThread(() => RefreshDocumentView(list));
                return;
            }
            CountText = $"Count: {list.Count}";
            UpdateTooltip();
            _tableView.AutosaveName = string.Empty; // don't autosave as screws up different countries
            _tableView.AutosaveTableColumns = false;
            _tableView.Source = new CensusSource(list);
            _tableView.ReloadData();
            //TODO: Does this need to be changed to .Regular (not sure where this is used) (KI)
            _tableView.SelectionHighlightStyle = NSTableViewSelectionHighlightStyle.None;
        }


        public void SortIndividuals()
        {
            NSSortDescriptor[] descriptors =
            {
                new NSSortDescriptor("FamilyID", true),
                new NSSortDescriptor("Position", true)
            };
            Sort(descriptors);
        }
    }
}
