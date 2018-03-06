using AppKit;
using Foundation;
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

                individualsViewController = new BindingListViewController<IDisplayIndividual>("Individuals");
                familiesViewController = new BindingListViewController<IDisplayFamily>("Families");
                sourcesViewController = new BindingListViewController<IDisplaySource>("Sources");
                occupationsViewController = new BindingListViewController<IDisplayOccupation>("Occupations");
                factsViewController = new BindingListViewController<IDisplayFact>("Facts");

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

            individualsViewController.RefreshDocumentView(_familyTree.AllDisplayIndividuals);
            familiesViewController.RefreshDocumentView(_familyTree.AllDisplayFamilies);
            sourcesViewController.RefreshDocumentView(_familyTree.AllDisplaySources);
            occupationsViewController.RefreshDocumentView(_familyTree.AllDisplayOccupations);
            factsViewController.RefreshDocumentView(_familyTree.AllDisplayFacts);

            documentViewController.Messages.Report("\n\nFinished loading file " + url.Path + "\n");
            return true;
        }
    }
}
