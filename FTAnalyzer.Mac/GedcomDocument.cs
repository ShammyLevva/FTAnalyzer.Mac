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

        public override bool ReadFromUrl(NSUrl url, string typeName, out NSError outError)
        {
            outError = null;

            var window = NSApplication.SharedApplication.Windows[0];
            var viewController = window.ContentViewController as ViewController;

            var textProgress = new Progress<string>(m => { viewController.StatusText.Value += m; });

            Task.Run(async () =>
            {
                var document = await Task.Run(() => _familyTree.LoadTreeHeader(url.Path, textProgress));
                if (document == null)
                {
                    return;
                }

                var sourceProgress = new Progress<int>(s => { });
                await Task.Run(() => _familyTree.LoadTreeSources(document, sourceProgress, textProgress));
                await Task.Run(() => _familyTree.LoadTreeIndividuals(document, sourceProgress, textProgress));
                await Task.Run(() => _familyTree.LoadTreeFamilies(document, sourceProgress, textProgress));
                await Task.Run(() => _familyTree.LoadTreeRelationships(document, sourceProgress, textProgress));
            });

            return true;
        }
    }
}
