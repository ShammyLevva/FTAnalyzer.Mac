using System;
using System.Threading.Tasks;
using AppKit;
using Foundation;

namespace FTAnalyzer.Mac
{
    [Register("GedcomDocument")]
    public class GedcomDocument : NSDocument
    {
        readonly FamilyTree _familyTree = FamilyTree.Instance;

        public GedcomDocument()
        {
        }

        [Export("canConcurrentlyReadDocumentsOfType:")]
        public static new bool CanConcurrentlyReadDocumentsOfType(string fileType) => true;

        public override bool ReadFromUrl(NSUrl url, string typeName, out NSError outError)
        {
            outError = null;

            ViewController viewController = null;

            InvokeOnMainThread(() =>
            {
                var window = NSApplication.SharedApplication.Windows[0];
                viewController = window.ContentViewController as ViewController;
                viewController.ClearAllProgress();
            });

            var document = _familyTree.LoadTreeHeader(url.Path, viewController.Messages);
            if (document == null)
            {
                return false;
            }

            _familyTree.LoadTreeSources(document, viewController.Sources, viewController.Messages);
            _familyTree.LoadTreeIndividuals(document, viewController.Individuals, viewController.Messages);
            _familyTree.LoadTreeFamilies(document, viewController.Families, viewController.Messages);
            _familyTree.LoadTreeRelationships(document, viewController.Relationships, viewController.Messages);

            return true;
        }
    }
}
