using System;
using AppKit;
using Foundation;
using FTAnalyzer.DataSources;
using FTAnalyzer.Utilities;

namespace FTAnalyzer.ViewControllers
{
    public partial class ColourBMDViewController : BindingListViewController<IDisplayColourBMD>
    {
        public readonly static int BMDColumnsStart = 5;
        public readonly static int BMDColumnsEnd = 11;
        int BMDProviderIndex { get; }
        string BMDProvider { get; }
        string BMDRegion { get; }

        #region Constructors

        // Called when created from unmanaged code
        public ColourBMDViewController(IntPtr handle) : base(string.Empty,string.Empty)
        { 
        }

        // Call to load from the XIB/NIB file
        public ColourBMDViewController(string bmdRegion, int providerIndex) : base("BMD Research Suggestions", string.Empty)
        {
            BMDProviderIndex = providerIndex;
            BMDProvider = FamilyTree.ProviderName(providerIndex);
            BMDRegion = bmdRegion;
        }

        #endregion

        public override void RefreshDocumentView(SortableBindingList<IDisplayColourBMD> list)
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
            _tableView.Source = new ColourBMDSource(BMDProvider, list);
            _tableView.ReloadData();
            _tableView.SelectionHighlightStyle = NSTableViewSelectionHighlightStyle.None;
            Title = $"BMD Research Suggestions. {list.Count} records listed.";
        }



        [Export("ViewDetailsSelector")]
        public override void ViewDetailsSelector()
        {
            if (!NSThread.IsMain)
            {
                InvokeOnMainThread(ViewDetailsSelector);
                return;
            }
            nint row = _tableView.ClickedRow;
            int columnIndex = (int)_tableView.ClickedColumn;
            if (row >= 0 && columnIndex >= ColourBMDViewController.BMDColumnsStart && columnIndex <= ColourBMDViewController.BMDColumnsEnd)
            {
                ColourBMDSource source = _tableView.Source as ColourBMDSource;
                IDisplayColourBMD person = source.GetRowObject(row) as IDisplayColourBMD;
                FamilyTree.SearchType st = FamilyTree.SearchType.BIRTH;
                FactDate factDate = null;
                var ft = FamilyTree.Instance; 
                Individual ind = ft.GetIndividual(person.IndividualID);
                Individual spouse = null;
                switch (columnIndex)
                {
                    case 5:
                    case 6:
                        st = FamilyTree.SearchType.BIRTH;
                        factDate = ind.BirthDate;
                        break;
                    case 7:
                        st = FamilyTree.SearchType.MARRIAGE;
                        spouse = ind.FirstSpouse;
                        factDate = ind.FirstMarriageDate;
                        break;
                    case 8:
                        st = FamilyTree.SearchType.MARRIAGE;
                        spouse = ind.SecondSpouse;
                        factDate = ind.SecondMarriageDate;
                        break;
                    case 9:
                        st = FamilyTree.SearchType.MARRIAGE;
                        spouse = ind.ThirdSpouse;
                        factDate = ind.ThirdMarriageDate;
                        break;
                    case 10:
                    case 11:
                        st = FamilyTree.SearchType.DEATH;
                        factDate = ind.DeathDate;
                        break;
                    default:
                        break;
                }
                try
                { 
                    ft.SearchBMD(st, ind, factDate, BMDProviderIndex, BMDRegion, spouse);
                }
                catch (CensusSearchException ex)
                {
                    UIHelpers.ShowMessage(ex.Message);
                }

            }
        }

    }
}
