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

        public override bool ReadFromUrl(NSUrl url, string typeName, out NSError outError)
        {

            outError = NSError.FromDomain(NSError.OsStatusErrorDomain, -4);

            GedcomDocumentViewController documentViewController = null;

            BindingListViewController<IDisplayIndividual> individualsViewController = null;
            BindingListViewController<IDisplayFamily> familiesViewController = null;
            BindingListViewController<IDisplaySource> sourcesViewController = null;
            BindingListViewController<IDisplayOccupation> occupationsViewController = null;
            BindingListViewController<IDisplayFact> factsViewController = null;

            BindingListViewController<DataError> dataErrorsViewController = null;
            //BindingListViewController<IDisplayDuplicateIndividual> duplicatesViewController = null;
            BindingListViewController<IDisplayLooseBirth> looseBirthsViewController = null;
            BindingListViewController<IDisplayLooseDeath> looseDeathsViewController = null;

            NSTabViewController tabbedViewController = null;

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


                individualsViewController = new BindingListViewController<IDisplayIndividual>("Individuals", Messages.Hints_Individual);
                familiesViewController = new BindingListViewController<IDisplayFamily>("Families", Messages.Hints_Family);
                sourcesViewController = new BindingListViewController<IDisplaySource>("Sources", Messages.Hints_Sources);
                occupationsViewController = new BindingListViewController<IDisplayOccupation>("Occupations", string.Empty); //TODO allow double click
                factsViewController = new BindingListViewController<IDisplayFact>("Facts", string.Empty);

                mainListsViewController.AddChildViewController(individualsViewController);
                mainListsViewController.AddChildViewController(familiesViewController);
                mainListsViewController.AddChildViewController(sourcesViewController);
                mainListsViewController.AddChildViewController(occupationsViewController);
                mainListsViewController.AddChildViewController(factsViewController);


                dataErrorsViewController = new BindingListViewController<DataError>("Data Errors", string.Empty); // TODO allow double click
                //duplicatesViewController = new BindingListViewController<IDisplayDuplicateIndividual>("Duplicates");
                looseBirthsViewController = new BindingListViewController<IDisplayLooseBirth>("Loose Births", Messages.Hints_Loose_Births);
                looseDeathsViewController = new BindingListViewController<IDisplayLooseDeath>("Loose Deaths", Messages.Hints_Loose_Deaths);

                errorsAndFixesTabViewController.AddChildViewController(dataErrorsViewController);
                //errorsAndFixesTabViewController.AddChildViewController(duplicatesViewController);
                errorsAndFixesTabViewController.AddChildViewController(looseBirthsViewController);
                errorsAndFixesTabViewController.AddChildViewController(looseDeathsViewController);

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

            individualsViewController.RefreshDocumentView(_familyTree.AllDisplayIndividuals);
            familiesViewController.RefreshDocumentView(_familyTree.AllDisplayFamilies);
            sourcesViewController.RefreshDocumentView(_familyTree.AllDisplaySources);
            occupationsViewController.RefreshDocumentView(_familyTree.AllDisplayOccupations);
            factsViewController.RefreshDocumentView(_familyTree.AllDisplayFacts);

            // Flatten the data error groups into a single list until filtering implemented.
            var errors = new SortableBindingList<DataError>(_familyTree.DataErrorTypes.SelectMany(dg => dg.Errors));
            dataErrorsViewController.RefreshDocumentView(errors);

            //duplicatesViewController.RefreshDocumentView(new SortableBindingList<IDisplayDuplicateIndividual>());
            looseBirthsViewController.RefreshDocumentView(_familyTree.LooseBirths());
            looseDeathsViewController.RefreshDocumentView(_familyTree.LooseDeaths());

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

        void IndividualsFactRowClicked(Individual individual)
        {
            InvokeOnMainThread(() =>
            {
                var app = (AppDelegate)NSApplication.SharedApplication.Delegate;
                var factsViewController = new FactsViewController<IDisplayFact>($"Facts Report for {individual.IndividualID}: {individual.Name}", individual);
                app.ShowFacts(factsViewController); 
                Analytics.TrackAction(Analytics.FactsFormAction, Analytics.FactsIndividualsEvent);
            });
        }

        void FamiliesFactRowClicked(Family family)
        {
            InvokeOnMainThread(() =>
            {
                var app = (AppDelegate)NSApplication.SharedApplication.Delegate;
                var factsViewController = new FactsViewController<IDisplayFact>($"Facts Report for {family.FamilyRef}", family);

                app.ShowFacts(factsViewController);
                Analytics.TrackAction(Analytics.FactsFormAction, Analytics.FactsFamiliesEvent);
            });
        }

        void SourcesFactRowClicked(FactSource source)
        {
            InvokeOnMainThread(() =>
            {
                var app = (AppDelegate)NSApplication.SharedApplication.Delegate;
                var factsViewController = new FactsViewController<IDisplayFact>($"Facts Report for source: {source.ToString()}", source);
                app.ShowFacts(factsViewController);
                Analytics.TrackAction(Analytics.FactsFormAction, Analytics.FactsSourceEvent);
            });
        }

        void LooseBirthFactRowClicked(Individual individual)
        {
            InvokeOnMainThread(() =>
            {
                var app = (AppDelegate)NSApplication.SharedApplication.Delegate;
                var factsViewController = new FactsViewController<IDisplayFact>($"Facts Report for {individual.IndividualID}: {individual.Name}", individual);
                app.ShowFacts(factsViewController);
                Analytics.TrackAction(Analytics.FactsFormAction, Analytics.LooseBirthsEvent);
            });
        }
  
        void LooseDeathFactRowClicked(Individual individual)
        {
            InvokeOnMainThread(() =>
            {
                var app = (AppDelegate)NSApplication.SharedApplication.Delegate;
                var factsViewController = new FactsViewController<IDisplayFact>($"Facts Report for {individual.IndividualID}: {individual.Name}", individual);
                app.ShowFacts(factsViewController);
                Analytics.TrackAction(Analytics.FactsFormAction, Analytics.LooseDeathsEvent);
            });
        }

        static void RemoveOldTabs(NSTabViewController viewController)
        {
            // Remove any existing lists from a previous document.
            while (viewController.ChildViewControllers.Length > 0)
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
