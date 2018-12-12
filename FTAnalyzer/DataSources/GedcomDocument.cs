using System;
using System.Linq;
using System.Threading.Tasks;
using AppKit;
using Foundation;
using FTAnalyzer.Utilities;
using FTAnalyzer.ViewControllers;
using ObjCRuntime;

namespace FTAnalyzer
{
    [Register("GedcomDocument")]
    public class GedcomDocument : NSDocument
    {
        readonly FamilyTree _familyTree = FamilyTree.Instance;
        AppDelegate App => (AppDelegate) NSApplication.SharedApplication.Delegate;

        [Export("canConcurrentlyReadDocumentsOfType:")]
        public static new bool CanConcurrentlyReadDocumentsOfType(string fileType) => true;

        FTAnalyzerTabViewController tabbedViewController;
        BindingListViewController<IDisplayIndividual> individualsViewController;
        BindingListViewController<IDisplayFamily> familiesViewController;
        BindingListViewController<IDisplaySource> sourcesViewController;
        BindingListViewController<IDisplayOccupation> occupationsViewController;
        BindingListViewController<IDisplayFact> factsViewController;

        BindingListViewController<DataError> dataErrorsViewController;
        //BindingListViewController<IDisplayDuplicateIndividual> duplicatesViewController = null;
        BindingListViewController<IDisplayLooseBirth> looseBirthsViewController;
        BindingListViewController<IDisplayLooseDeath> looseDeathsViewController;

        LocationViewController countriesViewController;
        LocationViewController regionsViewController;
        LocationViewController subregionsViewController;
        LocationViewController addressesViewController;
        LocationViewController placesViewController;

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
                tabbedViewController = window.ContentViewController as FTAnalyzerTabViewController;

                //Make sure the loading tab is seleceted.
                tabbedViewController.SelectedTabViewItemIndex = 0;
                documentViewController = tabbedViewController.ChildViewControllers[0] as GedcomDocumentViewController;
                documentViewController.ClearAllProgress();

                var mainListsViewController = tabbedViewController.ChildViewControllers[1] as ListsTabViewController;
                RemoveOldTabs(mainListsViewController);

                var errorsAndFixesTabViewController = tabbedViewController.ChildViewControllers[2] as ListsTabViewController;
                RemoveOldTabs(errorsAndFixesTabViewController);
                var locationsTabViewController = tabbedViewController.ChildViewControllers[3] as ListsTabViewController;
                RemoveOldTabs(locationsTabViewController);

                individualsViewController = new BindingListViewController<IDisplayIndividual>("Individuals", "Double click to show a list of facts for the selected individual.");
                familiesViewController = new BindingListViewController<IDisplayFamily>("Families", "Double click to show a list of facts for the selected family.") ;
                sourcesViewController = new BindingListViewController<IDisplaySource>("Sources", "Double click to show a list of facts referenced by the selected source.");
                occupationsViewController = new BindingListViewController<IDisplayOccupation>("Occupations", "Double click to show a list of individuals with that occupation.");
                factsViewController = new BindingListViewController<IDisplayFact>("Facts", string.Empty);

                mainListsViewController.AddChildViewController(individualsViewController);
                mainListsViewController.AddChildViewController(familiesViewController);
                mainListsViewController.AddChildViewController(sourcesViewController);
                mainListsViewController.AddChildViewController(occupationsViewController);
                mainListsViewController.AddChildViewController(factsViewController);

                dataErrorsViewController = new BindingListViewController<DataError>("Data Errors", "Double click to show Individual/Family with this error."); 
                //duplicatesViewController = new BindingListViewController<IDisplayDuplicateIndividual>("Duplicates", "Double click to show the facts for both the individual and their possible match.");
                looseBirthsViewController = new BindingListViewController<IDisplayLooseBirth>
                    ("Loose Births", "List of Births where you could limit the date range.\nDouble click to show a list of facts for the selected individual.");
                looseDeathsViewController = new BindingListViewController<IDisplayLooseDeath>
                    ("Loose Deaths", "List of Deaths where you could limit the date range.\nDouble click to show a list of facts for the selected individual.");

                errorsAndFixesTabViewController.AddChildViewController(dataErrorsViewController);
                //errorsAndFixesTabViewController.AddChildViewController(duplicatesViewController);
                errorsAndFixesTabViewController.AddChildViewController(looseBirthsViewController);
                errorsAndFixesTabViewController.AddChildViewController(looseDeathsViewController);

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

                individualsViewController.IndividualFactRowClicked += IndividualsFactRowClicked;
                individualsViewController.SetRootPersonClicked += SetRootPersonClicked;
                familiesViewController.FamilyFactRowClicked += FamiliesFactRowClicked;
                sourcesViewController.SourceFactRowClicked += SourcesFactRowClicked;
                dataErrorsViewController.DataErrorRowClicked += DataErrorRowClicked;
                looseBirthsViewController.IndividualFactRowClicked += LooseBirthFactRowClicked;
                looseDeathsViewController.IndividualFactRowClicked += LooseDeathFactRowClicked;
                occupationsViewController.OccupationRowClicked += OccupationRowClicked;
                countriesViewController.LocationRowClicked +=  LocationRowClicked;
                regionsViewController.LocationRowClicked += LocationRowClicked;
                subregionsViewController.LocationRowClicked += LocationRowClicked;
                addressesViewController.LocationRowClicked += LocationRowClicked;
                placesViewController.LocationRowClicked += LocationRowClicked;
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
                PrintInfo.Orientation = NSPrintingOrientation.Landscape;
                PrintInfo.LeftMargin = 45;
                PrintInfo.RightMargin = 45;
                PrintInfo.TopMargin = 30;
                PrintInfo.BottomMargin = 30;
                PrintInfo.HorizontalPagination = NSPrintingPaginationMode.Auto;
                PrintInfo.VerticallyCentered = false;
                PrintInfo.HorizontallyCentered = false;
                App.Document = this;
                App.SetMenus(true);
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

        public void PrintDocument(IPrintViewController viewController)
        {
            try
            {
                var window = new NSWindow
                {
                    ContentViewController = viewController as NSViewController,
                    ContentView = viewController.PrintView
                };
                window.Display();
                viewController.PreparePrintView();
                var printOperation = NSPrintOperation.FromView(viewController.PrintView as NSView, PrintInfo);
                //var printDelegate = new PrintDelegate();
                //printOperation.RunOperationModal(viewController.PrintView.Window, printDelegate, new Selector("printOperationDidRun:success:contextInfo:"), IntPtr.Zero);
                printOperation.ShowsPrintPanel = true;
                printOperation.ShowsProgressPanel = true;
                printOperation.CanSpawnSeparateThread = true;
                printOperation.PrintPanel.Options = NSPrintPanelOptions.ShowsCopies | NSPrintPanelOptions.ShowsPageRange | NSPrintPanelOptions.ShowsPreview |
                                 NSPrintPanelOptions.ShowsPageSetupAccessory | NSPrintPanelOptions.ShowsScaling;
                printOperation.RunOperation();
                printOperation.CleanUpOperation();
                window.Dispose();
            }
            catch (Exception e)
            {
                UIHelpers.ShowMessage($"Sorry there was a problem printing.\nError was: {e.Message}");
            }
        }

        void SetRootPersonClicked(Individual individual)
        {
            InvokeOnMainThread(() =>
            {
                // need to check that user right clicked and that they are happy to proceed with setting this root person before calling update root individual
                //FamilyTree.Instance.UpdateRootIndividual(individual.IndividualID, null, null);
               //Analytics.TrackAction(Analytics.FactsFormAction, Analytics.FactsIndividualsEvent);
            });

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

        void OccupationRowClicked(People people)
        {
            InvokeOnMainThread(() =>
            {
                people.ShowWindow(App);
                //Analytics.TrackAction(Analytics.Peo, Analytics.);
            });
        }

        void DataErrorRowClicked(Individual individual, Family family)
        {
            InvokeOnMainThread(() =>
            {
                if(individual == null)
                {
                    App.ShowFacts(new FactsViewController<IDisplayFact>($"Facts Report for {family.FamilyRef}", family));
                    Analytics.TrackAction(Analytics.FactsFormAction, Analytics.FactsFamiliesEvent);
                }
                else
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
                people.ShowWindow(App);
                //Analytics.TrackAction(Analytics.Peo, Analytics.);
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
