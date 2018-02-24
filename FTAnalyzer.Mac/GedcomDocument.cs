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

            var textProgress = new Progress<string>(m => { viewController.StatusText += m; });
            var sourcesProgress = new Progress<int>(p => { viewController.SourcesProgressValue = p; });
            var individualsProgress = new Progress<int>(p => { viewController.IndividualsProgressValue = p; });
            var familiesProgress = new Progress<int>(p => { viewController.FamiliesProgressValue = p; });
            var relationshipsProgress = new Progress<int>(p => { viewController.RelationshipsProgressValue = p; });

            Task.Run(async () =>
            {
                var document = await Task.Run(() => _familyTree.LoadTreeHeader(url.Path, textProgress));
                if (document == null)
                {
                    return;
                }

                await Task.Run(() => _familyTree.LoadTreeSources(document, sourcesProgress, textProgress));
                await Task.Run(() => _familyTree.LoadTreeIndividuals(document, individualsProgress, textProgress));
                await Task.Run(() => _familyTree.LoadTreeFamilies(document, familiesProgress, textProgress));
                await Task.Run(() => _familyTree.LoadTreeRelationships(document, relationshipsProgress, textProgress));
            });

            return true;
        }
    }
}
