using AppKit;
using Foundation;
using FTAnalyzer.Mac.ViewControllers;
using FTAnalyzer.Properties;
using FTAnalyzer.Utilities;
using System.Linq;

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

        internal void LoadMainLists(ProgressController progressController, string text)
        {
            if (!MainListsLoaded)
            {
                InvokeOnMainThread(() =>
                {
                    progressController.ProgressText = text;
                    progressController.ProgressBar.ToolTip = text;
                    progressController.ShowWindow(this);
                });
                InvokeOnMainThread(() => progressController.ProgressBar.DoubleValue = 0);
                individualsViewController.RefreshDocumentView(_familyTree.AllDisplayIndividuals);
                InvokeOnMainThread(() => progressController.ProgressBar.DoubleValue = 20);
                familiesViewController.RefreshDocumentView(_familyTree.AllDisplayFamilies);
                InvokeOnMainThread(() => progressController.ProgressBar.DoubleValue = 40);
                sourcesViewController.RefreshDocumentView(_familyTree.AllDisplaySources);
                InvokeOnMainThread(() => progressController.ProgressBar.DoubleValue = 60);
                occupationsViewController.RefreshDocumentView(_familyTree.AllDisplayOccupations);
                InvokeOnMainThread(() => progressController.ProgressBar.DoubleValue = 80);
                factsViewController.RefreshDocumentView(_familyTree.AllDisplayFacts);
                InvokeOnMainThread(() =>
                {
                    progressController.ProgressBar.DoubleValue = 100;
                    //progressController.Close();
                });
                MainListsLoaded = true;
             }
        }

        internal void LoadErrorsAndFixes(ProgressController progressController, string text)
        {
            if (!ErrorsAndFixesLoaded)
            {
                NSProgressIndicator progress = progressController.ProgressBar;
                progressController.ProgressText = text;
                progressController.ProgressBar.ToolTip = text;
                progressController.ShowWindow(this);
                // Flatten the data error groups into a single list until filtering implemented.
                progress.DoubleValue = 0;
                var errors = new SortableBindingList<DataError>(_familyTree.DataErrorTypes.SelectMany(dg => dg.Errors));
                dataErrorsViewController.RefreshDocumentView(errors);
                progress.DoubleValue = 25;
                //duplicatesViewController.RefreshDocumentView(new SortableBindingList<IDisplayDuplicateIndividual>());
                progress.DoubleValue = 50;
                looseBirthsViewController.RefreshDocumentView(_familyTree.LooseBirths());
                progress.DoubleValue = 75;
                looseDeathsViewController.RefreshDocumentView(_familyTree.LooseDeaths());
                progress.DoubleValue = 100;
                ErrorsAndFixesLoaded = true;
                //progressController.Close();
            }
        }

        internal void LoadLocations(ProgressController progressController, string text)
        {
            if (!LocationsLoaded)
            {
                NSProgressIndicator progress = progressController.ProgressBar;
                progressController.ProgressText = text;
                progressController.ProgressBar.ToolTip = text;
                progressController.ShowWindow(this);
                progress.DoubleValue = 0;
                countriesViewController.RefreshDocumentView(_familyTree.AllDisplayCountries);
                progress.DoubleValue = 20;
                regionsViewController.RefreshDocumentView(_familyTree.AllDisplayRegions);
                progress.DoubleValue = 40;
                subregionsViewController.RefreshDocumentView(_familyTree.AllDisplaySubRegions);
                progress.DoubleValue = 60;
                addressesViewController.RefreshDocumentView(_familyTree.AllDisplayAddresses);
                progress.DoubleValue = 80;
                placesViewController.RefreshDocumentView(_familyTree.AllDisplayPlaces);
                progress.DoubleValue = 100;
                LocationsLoaded = true;
                //progressController.Close();
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
