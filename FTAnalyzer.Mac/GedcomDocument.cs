using AppKit;
using Foundation;
using FTAnalyzer.Mac.DataSources;
using FTAnalyzer.Mac.ViewControllers;

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
            NSTabViewController tabbedViewController = null;

            InvokeOnMainThread(() =>
            {
                var window = NSApplication.SharedApplication.MainWindow;
                tabbedViewController = window.ContentViewController as NSTabViewController;

                documentViewController = tabbedViewController.ChildViewControllers[0] as GedcomDocumentViewController;
                documentViewController.ClearAllProgress();

                var mainListsViewController = tabbedViewController.ChildViewControllers[1] as NSTabViewController;

                individualsViewController = new BindingListViewController<IDisplayIndividual>();
                individualsViewController.LoadView();
                individualsViewController.Title = "Individuals";


                familiesViewController = new BindingListViewController<IDisplayFamily>();
                familiesViewController.LoadView();
                familiesViewController.Title = "Families";

                sourcesViewController = new BindingListViewController<IDisplaySource>();
                sourcesViewController.LoadView();
                sourcesViewController.Title = "Sources";

                occupationsViewController = new BindingListViewController<IDisplayOccupation>();
                occupationsViewController.LoadView();
                occupationsViewController.Title = "Occupations";

                factsViewController = new BindingListViewController<IDisplayFact>();
                factsViewController.LoadView();
                factsViewController.Title = "Facts";

                mainListsViewController.RemoveChildViewController(0);
                mainListsViewController.AddChildViewController(individualsViewController);
                mainListsViewController.AddChildViewController(familiesViewController);
                mainListsViewController.AddChildViewController(sourcesViewController);
                mainListsViewController.AddChildViewController(occupationsViewController);
                mainListsViewController.AddChildViewController(factsViewController);
            });

            var document = _familyTree.LoadTreeHeader(url.Path, documentViewController.Messages);
            if (document == null)
            {
                documentViewController.Messages.Report("\n\nUnable to load file " + url.Path + "\n");
                return false;
            }

            _familyTree.LoadTreeSources(document, documentViewController.Sources, documentViewController.Messages);
            _familyTree.LoadTreeIndividuals(document, documentViewController.Individuals, documentViewController.Messages);
            _familyTree.LoadTreeFamilies(document, documentViewController.Families, documentViewController.Messages);
            _familyTree.LoadTreeRelationships(document, documentViewController.Relationships, documentViewController.Messages);

            documentViewController.Messages.Report("\n\nFinished loading file " + url.Path + "\n");

            individualsViewController.RefreshDocumentView(_familyTree.AllDisplayIndividuals);
            familiesViewController.RefreshDocumentView(_familyTree.AllDisplayFamilies);
            sourcesViewController.RefreshDocumentView(_familyTree.AllDisplaySources);
            occupationsViewController.RefreshDocumentView(_familyTree.AllDisplayOccupations);
            factsViewController.RefreshDocumentView(_familyTree.AllDisplayFacts);
            return true;
        }
    }
}
