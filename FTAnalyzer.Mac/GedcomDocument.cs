using System;
using AppKit;
using Foundation;

namespace FTAnalyzer.Mac
{
    [Register("GedcomDocument")]
    public class GedcomDocument : NSDocument
    {
        FamilyTree _familyTree = FamilyTree.Instance;

        public GedcomDocument()
        {
        }

        public override bool ReadFromUrl(NSUrl url, string typeName, out NSError outError)
        {
            var textProgress = new Progress<string>(m => { });
            var document = _familyTree.LoadTreeHeader(url.Path, textProgress);
            if (document ==  null) {
                outError = null;
                return false;
            }

            var sourceProgress = new Progress<int>(s => { });
            _familyTree.LoadTreeSources(document, sourceProgress, textProgress);
            _familyTree.LoadTreeIndividuals(document, sourceProgress, textProgress);
            _familyTree.LoadTreeFamilies(document, sourceProgress, textProgress);
            _familyTree.LoadTreeRelationships(document, sourceProgress, textProgress);

            outError = null;
            return true;
        }
    }
}
