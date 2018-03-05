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

                mainListsViewController.AddChildViewController(individualsViewController);

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

            return true;
        }
    }
}
