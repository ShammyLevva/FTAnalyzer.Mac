using System;
using System.Collections.Generic;
using System.Linq;
using AppKit;
using Foundation;
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
        string censusCountry;
        bool CensusDone;

        public Census(CensusDate censusDate, bool censusDone)
        {
            LostCousins = false;
            CensusDate = censusDate;
            censusCountry = CensusDate.Country;
            RecordCount = 0;
            CensusDone = censusDone;
            var storyboard = NSStoryboard.FromName("Census", null);
            censusWindow = storyboard.InstantiateControllerWithIdentifier("CensusWindow") as NSWindowController;
            censusWindow.Window.SetFrame(new CoreGraphics.CGRect(300, 300, 800, 500), true);
            censusView = censusWindow.ContentViewController as CensusViewController;
            //string defaultProvider = (string)Application.UserAppDataRegistry.GetValue("Default Search Provider");
            //if (defaultProvider == null)
            //    defaultProvider = "FamilySearch";
            //cbCensusSearchProvider.Text = defaultProvider;
            //CensusSettingsUI.CompactCensusRefChanged += new EventHandler(RefreshCensusReferences);
        }

        List<CensusIndividual> FilterDuplicateIndividuals(List<CensusIndividual> individuals)
        {
            List<CensusIndividual> result = individuals.Filter(i => i.FamilyMembersCount > 1).ToList();
            HashSet<string> ids = new HashSet<string>(result.Select(i => i.IndividualID));
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

        public void SetupLCPotentials(List<CensusIndividual> potentials)
        {
            LostCousins = true;
            RecordCount = potentials.Count;
            SetupDataGridView(true, potentials);
        }

        public void ShowWindow(string title)
        {
            censusWindow.Window.Title = title;
            censusWindow.ShowWindow(App);
        }


        void SetupDataGridView(bool censusDone, List<CensusIndividual> individuals)
        {
            censusView.LoadCensusIndividuals(new SortableBindingList<IDisplayCensus>(individuals));
            //int numIndividuals = (from x in individuals select x.IndividualID).Distinct().Count();
            //int numFamilies = (from x in individuals select x.FamilyID).Distinct().Count();

            //tsRecords.Text = $"{individuals.Count} Rows containing {numIndividuals} Individuals and {numFamilies} Families. {CensusProviderText()}";
        }
    }
}
