using FTAnalyzer.Properties;
using FTAnalyzer.Utilities;

namespace FTAnalyzer.ViewControllers
{
    public partial class FTAnalyzerViewController : NSTabViewController
    {
        AppDelegate App => (AppDelegate)NSApplication.SharedApplication.Delegate;
        readonly FamilyTree _familyTree = FamilyTree.Instance;

        BindingListViewController<IDisplayIndividual> individualsViewController;
        BindingListViewController<IDisplayFamily> familiesViewController;
        BindingListViewController<IDisplaySource> sourcesViewController;
        BindingListViewController<IDisplayOccupation> occupationsViewController;
        BindingListViewController<IDisplayFact> factsViewController;

        BindingListViewController<IDisplayDataError> dataErrorsViewController;
        //BindingListViewController<IDisplayDuplicateIndividual> duplicatesViewController = null;
        BindingListViewController<IDisplayLooseBirth> looseBirthsViewController;
        BindingListViewController<IDisplayLooseDeath> looseDeathsViewController;

        LocationViewController countriesViewController;
        LocationViewController regionsViewController;
        LocationViewController subregionsViewController;
        LocationViewController addressesViewController;
        LocationViewController placesViewController;

        LCReportsViewController lCReportsViewController;
        LCUpdatesViewController lCUpdatesViewController;

        bool MainListsLoaded { get; set; }
        bool ErrorsAndFixesLoaded { get; set; }
        bool LocationsLoaded { get; set; }

        public FTAnalyzerViewController(IntPtr handle) : base(handle)
        {
        }

        [Export("tabView:didSelectTabViewItem:")]
        public override void DidSelect(NSTabView tabView, NSTabViewItem item)
        {
            if (App.Document == null)
                return; // don't bother if we've not loaded a document yet
            NSViewController? viewController = null;
            switch (item.Label)
            {
                case "Gedcom Stats":
                    if (ChildViewControllers.Length > 0)
                        viewController = ChildViewControllers[0];
                    break;
                case "Main Lists":
                    LoadMainLists(ProgressController);
                    if (ChildViewControllers.Length > 1)
                        viewController = ChildViewControllers[1];
                    Analytics.TrackAction(Analytics.MainFormAction, Analytics.MainListsEvent);
                    break;
                case "Errors/Fixes":
                    LoadErrorsAndFixes(ProgressController);
                    if (ChildViewControllers.Length > 2)
                        viewController = ChildViewControllers[2];
                    Analytics.TrackAction(Analytics.MainFormAction, Analytics.ErrorsFixesEvent);
                    break;
                case "Locations":
                    LoadLocations(ProgressController);
                    if (ChildViewControllers.Length > 3)
                        viewController = ChildViewControllers[3];
                    Analytics.TrackAction(Analytics.MainFormAction, Analytics.LocationTabViewed);
                    break;
                case "Lost Cousins":
                    UpdateLostCousinsReport(ProgressController);
                    if (ChildViewControllers.Length > 5)
                        viewController = ChildViewControllers[5];
                    Analytics.TrackAction(Analytics.MainFormAction, Analytics.LostCousinsTabEvent);
                    break;
                default:
                    viewController = null;
                    break;
            }
            if (viewController != null)
            {
                if (viewController is GedcomDocumentViewController)
                    App.CurrentViewController = viewController;
                else
                {
                    var index = (viewController as NSTabViewController).SelectedTabViewItemIndex;
                    if (ChildViewControllers.Length > index)
                        App.CurrentViewController = viewController.ChildViewControllers[index];
                }
            }
            else
                App.CurrentViewController = null;
        }

        ProgressController ProgressController { get; set; }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            var storyboard = NSStoryboard.FromName("Main", null);
            ProgressController = storyboard.InstantiateControllerWithIdentifier("ProgressDisplay") as ProgressController;
            ProgressController.Presentor = this;
            SetupViewControllers();
        }

        void SetupViewControllers()
        {
            GedcomDocumentViewController? documentViewController = null;

            //Make sure the loading tab is seleceted.
            SelectedTabViewItemIndex = 0;
            documentViewController = ChildViewControllers[0] as GedcomDocumentViewController;
            documentViewController.ClearAllProgress();
            App.DocumentViewController = documentViewController;
            App.CurrentViewController = documentViewController;
            SetupMainListsTabController();
            SetupDataErrorsTabController();
            SetupLocationsTabController();
            SetupLostCousinsTabController();
        }

        public void RefreshLists()
        {
            MainListsLoaded = false; // forces refresh on LoadMainLists
            LoadMainLists(ProgressController);
            if(ErrorsAndFixesLoaded)
            {
                ErrorsAndFixesLoaded = false;
                LoadErrorsAndFixes(ProgressController);
                lCReportsViewController.Clear();
                lCUpdatesViewController.Clear();
            }
        }

        void SetupMainListsTabController()
        {
            var mainListsViewController = ChildViewControllers[1] as ListsTabViewController;
            RemoveOldTabs(mainListsViewController);

            individualsViewController = new BindingListViewController<IDisplayIndividual>("Individuals", "Double click to show a list of facts for the selected individual.");
            familiesViewController = new BindingListViewController<IDisplayFamily>("Families", "Double click to show a list of facts for the selected family.");
            sourcesViewController = new BindingListViewController<IDisplaySource>("Sources", "Double click to show a list of facts referenced by the selected source.");
            occupationsViewController = new BindingListViewController<IDisplayOccupation>("Occupations", "Double click to show a list of individuals with that occupation.");
            factsViewController = new BindingListViewController<IDisplayFact>("Facts", string.Empty);

            mainListsViewController.AddChildViewController(individualsViewController);
            mainListsViewController.AddChildViewController(familiesViewController);
            mainListsViewController.AddChildViewController(sourcesViewController);
            mainListsViewController.AddChildViewController(occupationsViewController);
            mainListsViewController.AddChildViewController(factsViewController);

            individualsViewController.IndividualFactRowClicked += IndividualsFactRowClicked;
            familiesViewController.FamilyFactRowClicked += FamiliesFactRowClicked;
            occupationsViewController.OccupationRowClicked += OccupationRowClicked;
            sourcesViewController.SourceFactRowClicked += SourcesFactRowClicked;
        }

        void SetupDataErrorsTabController()
        {
            var errorsAndFixesTabViewController = ChildViewControllers[2] as ListsTabViewController;
            RemoveOldTabs(errorsAndFixesTabViewController);

            dataErrorsViewController = new BindingListViewController<IDisplayDataError>("Data Errors", "Double click to show Individual/Family with this error.");
            //duplicatesViewController = new BindingListViewController<IDisplayDuplicateIndividual>("Duplicates", "Double click to show the facts for both the individual and their possible match.");
            looseBirthsViewController = new BindingListViewController<IDisplayLooseBirth>
                ("Loose Births", "List of Births where you could limit the date range.\nDouble click to show a list of facts for the selected individual.");
            looseDeathsViewController = new BindingListViewController<IDisplayLooseDeath>
                ("Loose Deaths", "List of Deaths where you could limit the date range.\nDouble click to show a list of facts for the selected individual.");

            errorsAndFixesTabViewController.AddChildViewController(dataErrorsViewController);
            //errorsAndFixesTabViewController.AddChildViewController(duplicatesViewController);
            errorsAndFixesTabViewController.AddChildViewController(looseBirthsViewController);
            errorsAndFixesTabViewController.AddChildViewController(looseDeathsViewController);

            dataErrorsViewController.DataErrorRowClicked += DataErrorRowClicked;
            looseBirthsViewController.IndividualFactRowClicked += LooseBirthFactRowClicked;
            looseDeathsViewController.IndividualFactRowClicked += LooseDeathFactRowClicked;
        }

        void SetupLocationsTabController()
        {
            var locationsTabViewController = ChildViewControllers[3] as ListsTabViewController;
            RemoveOldTabs(locationsTabViewController);

            countriesViewController = new LocationViewController("Countries", "Double click to show all the individuals and families in that Country.");
            regionsViewController = new LocationViewController("Regions", "Double click to show all the individuals and families in that Region.");
            subregionsViewController = new LocationViewController("Sub-Regions", "Double click to show all the individuals and families in that Sub-Region.");
            addressesViewController = new LocationViewController("Addresses", "Double click to show all the individuals and families at that Address.");
            placesViewController = new LocationViewController("Places", "Double click to show all the individuals and families at that Place.");

            locationsTabViewController.AddChildViewController(countriesViewController);
            locationsTabViewController.AddChildViewController(regionsViewController);
            locationsTabViewController.AddChildViewController(subregionsViewController);
            locationsTabViewController.AddChildViewController(addressesViewController);
            locationsTabViewController.AddChildViewController(placesViewController);

            countriesViewController.LocationRowClicked += LocationRowClicked;
            regionsViewController.LocationRowClicked += LocationRowClicked;
            subregionsViewController.LocationRowClicked += LocationRowClicked;
            addressesViewController.LocationRowClicked += LocationRowClicked;
            placesViewController.LocationRowClicked += LocationRowClicked;
        }

        void SetupLostCousinsTabController()
        {
            lCReportsViewController = ChildViewControllers[5].ChildViewControllers[0] as LCReportsViewController;
            lCUpdatesViewController = ChildViewControllers[5].ChildViewControllers[1] as LCUpdatesViewController;
            ChildViewControllers[5].RemoveChildViewController(2); // hide Verification tab for now
            lCReportsViewController.LoadView();
            lCUpdatesViewController.LoadView();
        }

        void IndividualsFactRowClicked(Individual individual)
        {
            InvokeOnMainThread(() =>
            {
                if (individual != null)
                {
                    App.ShowFacts(new FactsViewController<IDisplayFact>($"Facts Report for {individual.IndividualID}: {individual.Name}", individual));
                    Analytics.TrackAction(Analytics.FactsFormAction, Analytics.FactsIndividualsEvent);
                }
            });
        }

        void FamiliesFactRowClicked(Family family)
        {
            InvokeOnMainThread(() =>
            {
                if (family != null)
                {
                    App.ShowFacts(new FactsViewController<IDisplayFact>($"Facts Report for {family.FamilyRef}", family));
                    Analytics.TrackAction(Analytics.FactsFormAction, Analytics.FactsFamiliesEvent);
                }
            });
        }

        void SourcesFactRowClicked(FactSource source)
        {
            InvokeOnMainThread(() =>
            {
                if (source != null)
                {
                    App.ShowFacts(new FactsViewController<IDisplayFact>($"Facts Report for source: {source.ToString()}", source));
                    Analytics.TrackAction(Analytics.FactsFormAction, Analytics.FactsSourceEvent);
                }
            });
        }

        void OccupationRowClicked(People people)
        {
            InvokeOnMainThread(() =>
            {
                if(people != null)
                    people.ShowWindow(App);
                //Analytics.TrackAction(Analytics.Peo, Analytics.);
            });
        }

        void DataErrorRowClicked(Individual individual, Family family)
        {
            InvokeOnMainThread(() =>
            {
                if (individual == null && family != null)
                {
                    App.ShowFacts(new FactsViewController<IDisplayFact>($"Facts Report for {family.FamilyRef}", family));
                    Analytics.TrackAction(Analytics.FactsFormAction, Analytics.FactsFamiliesEvent);
                }
                else if(individual != null && family == null)
                {
                    App.ShowFacts(new FactsViewController<IDisplayFact>($"Facts Report for {individual.IndividualID}: {individual.Name}", individual));
                    Analytics.TrackAction(Analytics.FactsFormAction, Analytics.FactsIndividualsEvent);
                }
            });
        }

        void LocationRowClicked(People people)
        {
            InvokeOnMainThread(() =>
            {
                if(people != null)
                    people.ShowWindow(App);
                //Analytics.TrackAction(Analytics.Peo, Analytics.);
            });
        }

        void LooseBirthFactRowClicked(Individual individual)
        {
            InvokeOnMainThread(() =>
            {
                if (individual != null)
                {
                    App.ShowFacts(new FactsViewController<IDisplayFact>($"Facts Report for {individual.IndividualID}: {individual.Name}", individual));
                    Analytics.TrackAction(Analytics.FactsFormAction, Analytics.LooseBirthsEvent);
                }
            });
        }

        void LooseDeathFactRowClicked(Individual individual)
        {
            InvokeOnMainThread(() =>
            {
                if (individual != null)
                {
                    App.ShowFacts(new FactsViewController<IDisplayFact>($"Facts Report for {individual.IndividualID}: {individual.Name}", individual));
                    Analytics.TrackAction(Analytics.FactsFormAction, Analytics.LooseDeathsEvent);
                }
            });
        }

        static void RemoveOldTabs(NSTabViewController viewController)
        {
            // Remove any existing lists from a previous document.
            while (viewController?.ChildViewControllers.Length > 0)
            {
                viewController.RemoveChildViewController(0);
            }
        }

        internal void LoadMainLists(ProgressController progressController)
        {
            if (!MainListsLoaded)
            {
                Task.Run(() =>
                {
                    InvokeOnMainThread(() => progressController.ShowWindow(this));
                    progressController.ProgressBar = 0;
                    progressController.ProgressText = "Loading Individuals Data. Please Wait";
                    individualsViewController.RefreshDocumentView(_familyTree.AllDisplayIndividuals);
                    progressController.ProgressBar = 20;
                    progressController.ProgressText = "Loading Families Data. Please Wait";
                    familiesViewController.RefreshDocumentView(_familyTree.AllDisplayFamilies);
                    progressController.ProgressBar = 40;
                    progressController.ProgressText = "Loading Sources Data. Please Wait";
                    sourcesViewController.RefreshDocumentView(_familyTree.AllDisplaySources);
                    progressController.ProgressBar = 60;
                    progressController.ProgressText = "Loading Occupations Data. Please Wait";
                    occupationsViewController.RefreshDocumentView(_familyTree.AllDisplayOccupations);
                    progressController.ProgressBar = 80;
                    progressController.ProgressText = "Loading Facts Data. Please Wait";
                    factsViewController.RefreshDocumentView(_familyTree.AllDisplayFacts);
                    progressController.ProgressBar = 100;
                    MainListsLoaded = true;
                    InvokeOnMainThread(progressController.Close);
                });
            }
        }

        internal void LoadErrorsAndFixes(ProgressController progressController)
        {
            if (!ErrorsAndFixesLoaded)
            {
                Task.Run(() =>
                {
                    InvokeOnMainThread(() => progressController.ShowWindow(this));
                    progressController.ProgressBar = 0;
                    progressController.ProgressText = "Loading Data Errors. Please Wait";
                    // Flatten the data error groups into a single list until filtering implemented.
                    var errors = new SortableBindingList<IDisplayDataError>(_familyTree.AllDataErrors);
                    dataErrorsViewController.RefreshDocumentView(errors);
                    progressController.ProgressBar = 25;
                    //duplicatesViewController.RefreshDocumentView(new SortableBindingList<IDisplayDuplicateIndividual>());
                    progressController.ProgressBar = 50;
                    progressController.ProgressText = "Loading Loose Births. Please Wait";
                    looseBirthsViewController.RefreshDocumentView(_familyTree.LooseBirths());
                    progressController.ProgressBar = 75;
                    progressController.ProgressText = "Loading Loose Deaths. Please Wait";
                    looseDeathsViewController.RefreshDocumentView(_familyTree.LooseDeaths());
                    progressController.ProgressBar = 100;
                    ErrorsAndFixesLoaded = true;
                    InvokeOnMainThread(progressController.Close);
                });
            }
        }

        internal void LoadLocations(ProgressController progressController)
        {
            if (!LocationsLoaded)
            {
                Task.Run(() =>
                {
                    InvokeOnMainThread(() => progressController.ShowWindow(this));
                    progressController.ProgressBar = 0;
                    progressController.ProgressText = "Loading List of Countries. Please Wait";
                    countriesViewController.RefreshDocumentView(_familyTree.AllDisplayCountries);
                    progressController.ProgressBar = 20;
                    progressController.ProgressText = "Loading List of Regions. Please Wait";
                    regionsViewController.RefreshDocumentView(_familyTree.AllDisplayRegions);
                    progressController.ProgressBar = 40;
                    progressController.ProgressText = "Loading List of SubRegions. Please Wait";
                    subregionsViewController.RefreshDocumentView(_familyTree.AllDisplaySubRegions);
                    progressController.ProgressBar = 60;
                    progressController.ProgressText = "Loading List of Addresses. Please Wait";
                    addressesViewController.RefreshDocumentView(_familyTree.AllDisplayAddresses);
                    progressController.ProgressBar = 80;
                    progressController.ProgressText = "Loading List of Places. Please Wait";
                    placesViewController.RefreshDocumentView(_familyTree.AllDisplayPlaces);
                    progressController.ProgressBar = 100;
                    LocationsLoaded = true;
                    InvokeOnMainThread(progressController.Close);
                });
            }
        }

        void UpdateLostCousinsReport(ProgressController progressController)
        {

            Task.Run(() =>
            {
                progressController.ProgressText = "Loading Census Statistics";
                progressController.ProgressBar = 0;
                InvokeOnMainThread(() => progressController.ShowWindow(this));
                lCReportsViewController.UpdateLostCousinsReport(progressController);
                progressController.ProgressBar = 50;
                progressController.ProgressText = "Loading Lost Cousins Statistics";
                //KI lCUpdatesViewController.UpdateLostCousinsReport(lCReportsViewController.RelationshipTypes, progressController, lCReportsViewController);
                progressController.ProgressBar = 100;
                InvokeOnMainThread(progressController.Close);
            });
            if (GeneralSettings.Default.SkipCensusReferences)
                UIHelpers.ShowMessage("Option to skip Census reference is set. No data can be uploaded to Lost Cousins without a Census Reference.");
        }
    }
}