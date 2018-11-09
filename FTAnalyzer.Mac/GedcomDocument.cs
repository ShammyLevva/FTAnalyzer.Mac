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

                individualsViewController = new BindingListViewController<IDisplayIndividual>("Individuals", "Double click to show a list of facts for the selected individual."); //Messages.Hints_Individual);
                familiesViewController = new BindingListViewController<IDisplayFamily>("Families", "Double click to show a list of facts for the selected family."); //Messages.Hints_Family);
                sourcesViewController = new BindingListViewController<IDisplaySource>("Sources", "Double click to show a list of facts referenced by the selected source."); //Messages.Hints_Sources);
                occupationsViewController = new BindingListViewController<IDisplayOccupation>("Occupations", string.Empty); //TODO allow double click
                factsViewController = new BindingListViewController<IDisplayFact>("Facts", string.Empty);

                mainListsViewController.AddChildViewController(individualsViewController);
                mainListsViewController.AddChildViewController(familiesViewController);
                mainListsViewController.AddChildViewController(sourcesViewController);
                mainListsViewController.AddChildViewController(occupationsViewController);
                mainListsViewController.AddChildViewController(factsViewController);

                dataErrorsViewController = new BindingListViewController<DataError>("Data Errors", string.Empty); // TODO allow double click
                //duplicatesViewController = new BindingListViewController<IDisplayDuplicateIndividual>("Duplicates", Messages.Hints_Duplicates);
                looseBirthsViewController = new BindingListViewController<IDisplayLooseBirth>("Loose Births", "List of Births where you could limit the date range. "); //Messages.Hints_Loose_Births);
                looseDeathsViewController = new BindingListViewController<IDisplayLooseDeath>("Loose Deaths", "List of Deaths where you could limit the date range. "); //Messages.Hints_Loose_Deaths);

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
                var app = (AppDelegate)NSApplication.SharedApplication.Delegate;
                app.Document = this;
                Analytics.TrackAction(Analytics.MainFormAction, Analytics.LoadGEDCOMEvent);
                UIHelpers.ShowMessage($"Gedcom file {url.Path} loaded.", "FTAnalyzer");
            });

            RaiseDocumentModified(this);
            return true;
        }

        internal void LoadMainLists()
        {
            if (!MainListsLoaded)
            {
                individualsViewController.RefreshDocumentView(_familyTree.AllDisplayIndividuals);
                familiesViewController.RefreshDocumentView(_familyTree.AllDisplayFamilies);
                sourcesViewController.RefreshDocumentView(_familyTree.AllDisplaySources);
                occupationsViewController.RefreshDocumentView(_familyTree.AllDisplayOccupations);
                factsViewController.RefreshDocumentView(_familyTree.AllDisplayFacts);
                MainListsLoaded = true;
            }
        }

        internal void LoadErrorsAndFixes()
        {
            if (!ErrorsAndFixesLoaded)
            {
                // Flatten the data error groups into a single list until filtering implemented.
                var errors = new SortableBindingList<DataError>(_familyTree.DataErrorTypes.SelectMany(dg => dg.Errors));
                dataErrorsViewController.RefreshDocumentView(errors);

                //duplicatesViewController.RefreshDocumentView(new SortableBindingList<IDisplayDuplicateIndividual>());
                looseBirthsViewController.RefreshDocumentView(_familyTree.LooseBirths());
                looseDeathsViewController.RefreshDocumentView(_familyTree.LooseDeaths());
                ErrorsAndFixesLoaded = true;
            }
        }

        internal void LoadLocations()
        {
            if (!LocationsLoaded)
            {
                countriesViewController.RefreshDocumentView(_familyTree.AllDisplayCountries);
                regionsViewController.RefreshDocumentView(_familyTree.AllDisplayRegions);
                subregionsViewController.RefreshDocumentView(_familyTree.AllDisplaySubRegions);
                addressesViewController.RefreshDocumentView(_familyTree.AllDisplayAddresses);
                placesViewController.RefreshDocumentView(_familyTree.AllDisplayPlaces);
                LocationsLoaded = true;
            }
        }

        void IndividualsFactRowClicked(Individual individual)
        {
            InvokeOnMainThread(() =>
            {
                var app = (AppDelegate)NSApplication.SharedApplication.Delegate;
                app.ShowFacts(new FactsViewController<IDisplayFact>($"Facts Report for {individual.IndividualID}: {individual.Name}", individual)); 
                Analytics.TrackAction(Analytics.FactsFormAction, Analytics.FactsIndividualsEvent);
            });
        }

        void FamiliesFactRowClicked(Family family)
        {
            InvokeOnMainThread(() =>
            {
                var app = (AppDelegate)NSApplication.SharedApplication.Delegate;
                app.ShowFacts(new FactsViewController<IDisplayFact>($"Facts Report for {family.FamilyRef}", family));
                Analytics.TrackAction(Analytics.FactsFormAction, Analytics.FactsFamiliesEvent);
            });
        }

        void SourcesFactRowClicked(FactSource source)
        {
            InvokeOnMainThread(() =>
            {
                var app = (AppDelegate)NSApplication.SharedApplication.Delegate;
                app.ShowFacts(new FactsViewController<IDisplayFact>($"Facts Report for source: {source.ToString()}", source));
                Analytics.TrackAction(Analytics.FactsFormAction, Analytics.FactsSourceEvent);
            });
        }

        void LooseBirthFactRowClicked(Individual individual)
        {
            InvokeOnMainThread(() =>
            {
                var app = (AppDelegate)NSApplication.SharedApplication.Delegate;
                app.ShowFacts(new FactsViewController<IDisplayFact>($"Facts Report for {individual.IndividualID}: {individual.Name}", individual));
                Analytics.TrackAction(Analytics.FactsFormAction, Analytics.LooseBirthsEvent);
            });
        }
  
        void LooseDeathFactRowClicked(Individual individual)
        {
            InvokeOnMainThread(() =>
            {
                var app = (AppDelegate)NSApplication.SharedApplication.Delegate;
                app.ShowFacts(new FactsViewController<IDisplayFact>($"Facts Report for {individual.IndividualID}: {individual.Name}", individual));
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
