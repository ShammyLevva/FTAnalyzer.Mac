using AppKit;
using Foundation;
using FTAnalyzer.Mac.ViewControllers;
using FTAnalyzer.Properties;
using FTAnalyzer.Utilities;
using System.Linq;
using System.Threading.Tasks;

namespace FTAnalyzer.Mac
{
    [Register("GedcomDocument")]
    public class GedcomDocument : NSDocument
    {
        readonly FamilyTree _familyTree = FamilyTree.Instance;
        AppDelegate App => (AppDelegate) NSApplication.SharedApplication.Delegate;

        [Export("canConcurrentlyReadDocumentsOfType:")]
        public static new bool CanConcurrentlyReadDocumentsOfType(string fileType) => true;

        NSTabViewController tabbedViewController;
        BindingListViewController<IDisplayIndividual> individualsViewController;
        BindingListViewController<IDisplayFamily> familiesViewController;
        BindingListViewController<IDisplaySource> sourcesViewController;
        BindingListViewController<IDisplayOccupation> occupationsViewController;
        BindingListViewController<IDisplayFact> factsViewController;

        BindingListViewController<DataError> dataErrorsViewController;
        //BindingListViewController<IDisplayDuplicateIndividual> duplicatesViewController = null;
        BindingListViewController<IDisplayLooseBirth> looseBirthsViewController;
        BindingListViewController<IDisplayLooseDeath> looseDeathsViewController;

        BindingListViewController<IDisplayLocation> countriesViewController;
        BindingListViewController<IDisplayLocation> regionsViewController;
        BindingListViewController<IDisplayLocation> subregionsViewController;
        BindingListViewController<IDisplayLocation> addressesViewController;
        BindingListViewController<IDisplayLocation> placesViewController;

        bool MainListsLoaded { get; set; }
        bool ErrorsAndFixesLoaded { get; set; }
        bool LocationsLoaded { get; set; }

        public override bool ReadFromUrl(NSUrl url, string typeName, out NSError outError)
        {
            MainListsLoaded = false;
            ErrorsAndFixesLoaded = false;
            LocationsLoaded = false;
            outError = NSError.FromDomain(NSError.OsStatusErrorDomain, -4);

            GedcomDocumentViewController documentViewController = null;

            InvokeOnMainThread(() =>
            {
                var window = NSApplication.SharedApplication.MainWindow;

                tabbedViewController = window.ContentViewController as NSTabViewController;
                //Make sure the loading tab is seleceted.
                tabbedViewController.SelectedTabViewItemIndex = 0;
                documentViewController = tabbedViewController.ChildViewControllers[0] as GedcomDocumentViewController;
                documentViewController.ClearAllProgress();
            
                var mainListsViewController = tabbedViewController.ChildViewControllers[1] as NSTabViewController;
                RemoveOldTabs(mainListsViewController);
                var errorsAndFixesTabViewController = tabbedViewController.ChildViewControllers[2] as NSTabViewController;
                RemoveOldTabs(errorsAndFixesTabViewController);
                var locationsTabViewController = tabbedViewController.ChildViewControllers[3] as NSTabViewController;
                RemoveOldTabs(locationsTabViewController);

                individualsViewController = new BindingListViewController<IDisplayIndividual>("Individuals", "Double click to show a list of facts for the selected individual.");
                familiesViewController = new BindingListViewController<IDisplayFamily>("Families", "Double click to show a list of facts for the selected family.") ;
                sourcesViewController = new BindingListViewController<IDisplaySource>("Sources", "Double click to show a list of facts referenced by the selected source.");
                occupationsViewController = new BindingListViewController<IDisplayOccupation>("Occupations", string.Empty); //TODO allow double click
                factsViewController = new BindingListViewController<IDisplayFact>("Facts", string.Empty);

                mainListsViewController.AddChildViewController(individualsViewController);
                mainListsViewController.AddChildViewController(familiesViewController);
                mainListsViewController.AddChildViewController(sourcesViewController);
                mainListsViewController.AddChildViewController(occupationsViewController);
                mainListsViewController.AddChildViewController(factsViewController);

                dataErrorsViewController = new BindingListViewController<DataError>("Data Errors", string.Empty); // TODO allow double click
                //duplicatesViewController = new BindingListViewController<IDisplayDuplicateIndividual>("Duplicates", "Double click to show the facts for both the individual and their possible match.");
                looseBirthsViewController = new BindingListViewController<IDisplayLooseBirth>("Loose Births", "List of Births where you could limit the date range.");
                looseDeathsViewController = new BindingListViewController<IDisplayLooseDeath>("Loose Deaths", "List of Deaths where you could limit the date range.");

                errorsAndFixesTabViewController.AddChildViewController(dataErrorsViewController);
                //errorsAndFixesTabViewController.AddChildViewController(duplicatesViewController);
                errorsAndFixesTabViewController.AddChildViewController(looseBirthsViewController);
                errorsAndFixesTabViewController.AddChildViewController(looseDeathsViewController);

                countriesViewController = new BindingListViewController<IDisplayLocation>("Countries", string.Empty); //TODO allow double click
                regionsViewController = new BindingListViewController<IDisplayLocation>("Regions", string.Empty); //TODO allow double click
                subregionsViewController = new BindingListViewController<IDisplayLocation>("Sub-Regions", string.Empty); //TODO allow double click
                addressesViewController = new BindingListViewController<IDisplayLocation>("Addresses", string.Empty); //TODO allow double click
                placesViewController = new BindingListViewController<IDisplayLocation>("Places", string.Empty); //TODO allow double click

                locationsTabViewController.AddChildViewController(countriesViewController);
                locationsTabViewController.AddChildViewController(regionsViewController);
                locationsTabViewController.AddChildViewController(subregionsViewController);
                locationsTabViewController.AddChildViewController(addressesViewController);
                locationsTabViewController.AddChildViewController(placesViewController);

                individualsViewController.IndividualFactRowClicked += IndividualsFactRowClicked;
                familiesViewController.FamilyFactRowClicked += FamiliesFactRowClicked;
                sourcesViewController.SourceFactRowClicked += SourcesFactRowClicked;
                looseBirthsViewController.IndividualFactRowClicked += LooseBirthFactRowClicked;
                looseDeathsViewController.IndividualFactRowClicked += LooseDeathFactRowClicked;
            });

            var document = _familyTree.LoadTreeHeader(url.Path, documentViewController.Messages);
            if (document == null)
            {
                documentViewController.Messages.Report($"\n\nUnable to load file {url.Path}\n");
                return false;
            }

            _familyTree.LoadTreeSources(document, documentViewController.Sources, documentViewController.Messages);
            _familyTree.LoadTreeIndividuals(document, documentViewController.Individuals, documentViewController.Messages);
            _familyTree.LoadTreeFamilies(document, documentViewController.Families, documentViewController.Messages);
            _familyTree.LoadTreeRelationships(document, documentViewController.Relationships, documentViewController.Messages);

            documentViewController.Messages.Report($"\n\nFinished loading file {url.Path}\n");

            InvokeOnMainThread(() =>
            {
                App.Document = this;
                Analytics.TrackAction(Analytics.MainFormAction, Analytics.LoadGEDCOMEvent);
                UIHelpers.ShowMessage($"Gedcom file {url.Path} loaded.", "FTAnalyzer");
            });

            RaiseDocumentModified(this);
            return true;
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
                    var errors = new SortableBindingList<DataError>(_familyTree.DataErrorTypes.SelectMany(dg => dg.Errors));
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

        void IndividualsFactRowClicked(Individual individual)
        {
            InvokeOnMainThread(() =>
            {
                App.ShowFacts(new FactsViewController<IDisplayFact>($"Facts Report for {individual.IndividualID}: {individual.Name}", individual)); 
                Analytics.TrackAction(Analytics.FactsFormAction, Analytics.FactsIndividualsEvent);
            });
        }

        void FamiliesFactRowClicked(Family family)
        {
            InvokeOnMainThread(() =>
            {
                App.ShowFacts(new FactsViewController<IDisplayFact>($"Facts Report for {family.FamilyRef}", family));
                Analytics.TrackAction(Analytics.FactsFormAction, Analytics.FactsFamiliesEvent);
            });
        }

        void SourcesFactRowClicked(FactSource source)
        {
            InvokeOnMainThread(() =>
            {
                App.ShowFacts(new FactsViewController<IDisplayFact>($"Facts Report for source: {source.ToString()}", source));
                Analytics.TrackAction(Analytics.FactsFormAction, Analytics.FactsSourceEvent);
            });
        }

        void LooseBirthFactRowClicked(Individual individual)
        {
            InvokeOnMainThread(() =>
            {
                App.ShowFacts(new FactsViewController<IDisplayFact>($"Facts Report for {individual.IndividualID}: {individual.Name}", individual));
                Analytics.TrackAction(Analytics.FactsFormAction, Analytics.LooseBirthsEvent);
            });
        }
  
        void LooseDeathFactRowClicked(Individual individual)
        {
            InvokeOnMainThread(() =>
            {
                App.ShowFacts(new FactsViewController<IDisplayFact>($"Facts Report for {individual.IndividualID}: {individual.Name}", individual));
                Analytics.TrackAction(Analytics.FactsFormAction, Analytics.LooseDeathsEvent);
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

        #region Events
        public delegate void DocumentModifiedDelegate(GedcomDocument document);
        public event DocumentModifiedDelegate DocumentModified;

        internal void RaiseDocumentModified(GedcomDocument document)
        {
            // Inform caller
            DocumentModified?.Invoke(document);
        }
        #endregion
    }
}
