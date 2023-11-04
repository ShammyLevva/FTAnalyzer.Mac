using FTAnalyzer.Filters;
using FTAnalyzer.Utilities;
using FTAnalyzer.ViewControllers;

namespace FTAnalyzer.DataSources
{
    public class Census
    {
        AppDelegate App => (AppDelegate)NSApplication.SharedApplication.Delegate;
        NSWindowController censusWindow;
        CensusViewController censusView;
        public CensusDate CensusDate { get; private set; }
        public int RecordCount { get; private set; }
        public bool LostCousins { get; private set; }
        string _censusCountry;
        bool _censusDone;
        int CensusProviderIndex { get; }
        string CensusProvider { get; }

        public Census(CensusDate censusDate, bool censusDone)
        {
            LostCousins = false;
            CensusDate = censusDate;
            RecordCount = 0;
            _censusCountry = CensusDate.Country;
            _censusDone = censusDone;
            var storyboard = NSStoryboard.FromName("ColourCensus", null);
            censusWindow = storyboard.InstantiateControllerWithIdentifier("ColourCensusWindow") as NSWindowController;
            censusWindow.Window.SetFrame(new CGRect(300, 300, 800, 500), true);
            //string defaultProvider = (string)Application.UserAppDataRegistry.GetValue("Default Search Provider");
            //if (defaultProvider == null)
            //    defaultProvider = "FamilySearch";
            //cbCensusSearchProvider.Text = defaultProvider;
            //CensusSettingsUI.CompactCensusRefChanged += new EventHandler(RefreshCensusReferences);
        }

        static List<CensusIndividual> FilterDuplicateIndividuals(List<CensusIndividual> individuals)
        {
            List<CensusIndividual> result = individuals.Filter(i => i.FamilyMembersCount > 1).ToList();
            HashSet<string> ids = new(result.Select(i => i.IndividualID));
            foreach (CensusIndividual i in individuals.Filter(i => i.FamilyMembersCount == 1))
                if (!ids.Contains(i.IndividualID))
                {
                    result.Add(i);
                    ids.Add(i.IndividualID);
                }
            return result;
        }

        public void SetupLCCensus(Predicate<CensusIndividual> relationFilter, bool showEnteredLostCousins, Predicate<Individual> individualRelationFilter)
        {
            LostCousins = true;
            Predicate<CensusIndividual> predicate;
            Predicate<Individual> individualPredicate;
            if (showEnteredLostCousins)
            {
                predicate = x => x.IsLostCousinsEntered(CensusDate, false);
                individualPredicate = x => x.IsLostCousinsEntered(CensusDate, false);
            }
            else
            {
                predicate = x => x.MissingLostCousins(CensusDate, false);
                individualPredicate = x => x.MissingLostCousins(CensusDate, false);
            }
            Predicate<CensusIndividual> filter = FilterUtils.AndFilter(relationFilter, predicate);
            Predicate<Individual> individualFilter = FilterUtils.AndFilter(individualRelationFilter, individualPredicate);
            IEnumerable<CensusFamily> censusFamilies = FamilyTree.Instance.GetAllCensusFamilies(CensusDate, true, false);
            List<CensusIndividual> individuals = censusFamilies.SelectMany(f => f.Members).Filter(filter).ToList();
            individuals = FilterDuplicateIndividuals(individuals);
            List<Individual> listToCheck = FamilyTree.Instance.AllIndividuals.Filter(individualFilter).ToList();
            //CompareLists(individuals, listToCheck);
            RecordCount = individuals.Count;
            SetupDataGridView(true, individuals);
        }

        public void SetupLCupdateList(List<CensusIndividual> listItems)
        {
            LostCousins = true;
            RecordCount = listItems.Count;
            SetupDataGridView(true, listItems);
        }

        public void ShowWindow(string title)
        {
            censusWindow.Window.Title = title;
            censusWindow.ShowWindow(App);
        }

        void SetupDataGridView(bool censusDone, List<CensusIndividual> individuals)
        {
            censusView = new CensusViewController(new IntPtr());
            censusView.RefreshDocumentView(new SortableBindingList<IDisplayCensus>(individuals));
            censusView.SortIndividuals();
            censusWindow.ContentViewController.AddChildViewController(censusView);
            int numIndividuals = (from x in individuals select x.IndividualID).Distinct().Count();
            int numFamilies = (from x in individuals select x.FamilyID).Distinct().Count();
            censusView.TooltipText = $"{individuals.Count} Rows containing {numIndividuals} Individuals and {numFamilies} Families."; // {CensusProviderText()}";
        }

        string CensusProviderText() => CensusDate.VALUATIONROLLS.Contains(CensusDate)
            ? string.Empty
            : $"Double click to search {CensusProvider} for this person's census record. Shift Double click to display their facts.";

    }
}
