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

        int _sourcesProgress;
        int _individualsProgress;
        int _familiesProgress;
        int _relationshipsProgress;
        string _messages;

        public GedcomDocument()
        {
            _messages = string.Empty;
        }

        public override void MakeWindowControllers()
        {
            base.MakeWindowControllers();

            var window = NSApplication.SharedApplication.MainWindow;
            var viewController = window.ContentViewController as ViewController;
            viewController.Document = this;
        }

        [Export("SourcesProgress")]
        public int SourcesProgress
        {
            get { return _sourcesProgress; }
            set
            {
                WillChangeValue("SourcesProgress");
                _sourcesProgress = value;
                DidChangeValue("SourcesProgress");
            }
        }

        [Export("IndividualsProgress")]
        public int IndividualsProgress
        {
            get { return _individualsProgress; }
            set
            {
                WillChangeValue("IndividualsProgress");
                _individualsProgress = value;
                DidChangeValue("IndividualsProgress");
            }
        }

        [Export("FamiliesProgress")]
        public int FamiliesProgress
        {
            get { return _familiesProgress; }
            set
            {
                WillChangeValue("FamiliesProgress");
                _familiesProgress = value;
                DidChangeValue("FamiliesProgress");
            }
        }

        [Export("RelationshipsProgress")]
        public int RelationshipsProgress
        {
            get { return _relationshipsProgress; }
            set
            {
                WillChangeValue("RelationshipsProgress");
                _relationshipsProgress = value;
                DidChangeValue("RelationshipsProgress");
            }
        }

        [Export("Messages")]
        public string Messages
        {
            get { return _messages; }
            set
            {
                WillChangeValue("Messages");
                _messages = value;
                DidChangeValue("Messages");
            }
        }

        [Export("canConcurrentlyReadDocumentsOfType:")]
        public static new bool CanConcurrentlyReadDocumentsOfType(string typeName) => true;

        public override bool ReadFromUrl(NSUrl url, string typeName, out NSError outError)
        {
            outError = null;

            var textProgress = new Progress<string>(m => { Messages += m; });
            var sourcesProgress = new Progress<int>(p => { SourcesProgress = p; });
            var individualsProgress = new Progress<int>(p => { IndividualsProgress = p; });
            var familiesProgress = new Progress<int>(p => { FamiliesProgress = p; });
            var relationshipsProgress = new Progress<int>(p => { RelationshipsProgress = p; });

            var document = _familyTree.LoadTreeHeader(url.Path, textProgress);
            if (document == null)
            {
                return false;
            }

            _familyTree.LoadTreeSources(document, sourcesProgress, textProgress);
            _familyTree.LoadTreeIndividuals(document, individualsProgress, textProgress);
            _familyTree.LoadTreeFamilies(document, familiesProgress, textProgress);
            _familyTree.LoadTreeRelationships(document, relationshipsProgress, textProgress);

            return true;
        }
    }
}
