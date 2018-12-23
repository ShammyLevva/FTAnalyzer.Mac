using System;
using System.Threading.Tasks;
using AppKit;
using Foundation;
using FTAnalyzer.Utilities;
using FTAnalyzer.ViewControllers;

namespace FTAnalyzer
{
    [Register("GedcomDocument")]
    public class GedcomDocument : NSDocument
    {
        AppDelegate App => (AppDelegate) NSApplication.SharedApplication.Delegate;
        readonly FamilyTree _familyTree = FamilyTree.Instance;

        [Export("canConcurrentlyReadDocumentsOfType:")]
        public static new bool CanConcurrentlyReadDocumentsOfType(string fileType) => true;

        public override bool ReadFromUrl(NSUrl url, string typeName, out NSError outError)
        {
            outError = NSError.FromDomain(NSError.OsStatusErrorDomain, -4);

            InvokeOnMainThread(async () =>
            {
                var outputText = App.DocumentViewController.Messages;
                var sourcesProgress = App.DocumentViewController.Sources;
                var individualsProgress = App.DocumentViewController.Individuals;
                var familiesProgress = App.DocumentViewController.Families;
                var relationshipProgress = App.DocumentViewController.Relationships;

                var document = await Task.Run(() => _familyTree.LoadTreeHeader(url.Path, outputText));
                if (document == null)
                    App.DocumentViewController.Messages.Report($"\n\nUnable to load file {url.Path}\n");
                else
                {

                    await Task.Run(() => _familyTree.LoadTreeSources(document, sourcesProgress, outputText));
                    await Task.Run(() => _familyTree.LoadTreeIndividuals(document, individualsProgress , outputText));
                    await Task.Run(() => _familyTree.LoadTreeFamilies(document, familiesProgress, outputText));
                    await Task.Run(() => _familyTree.LoadTreeRelationships(document, relationshipProgress, outputText));

                    App.DocumentViewController.Messages.Report($"\n\nFinished loading file {url.Path}\n");

                    PrintInfo.Orientation = NSPrintingOrientation.Landscape;
                    PrintInfo.LeftMargin = 45;
                    PrintInfo.RightMargin = 30;
                    PrintInfo.TopMargin = 30;
                    PrintInfo.BottomMargin = 30;
                    PrintInfo.HorizontalPagination = NSPrintingPaginationMode.Auto;
                    PrintInfo.VerticallyCentered = false;
                    PrintInfo.HorizontallyCentered = false;
                    App.Document = this;
                    App.SetMenus(true);
                    await Analytics.TrackAction(Analytics.MainFormAction, Analytics.LoadGEDCOMEvent);
                    UIHelpers.ShowMessage($"Gedcom file {url.Path} loaded.", "FTAnalyzer");
                }
            });
            RaiseDocumentModified(this);
            return true;
        }

        public void PrintDocument(IPrintViewController tableViewController)
        {
            try
            {
                var printingViewController = new TablePrintingViewController(tableViewController);
                var printOperation = NSPrintOperation.FromView(printingViewController.PrintView, PrintInfo);
                printOperation.ShowsPrintPanel = true;
                printOperation.ShowsProgressPanel = true;
                printOperation.CanSpawnSeparateThread = true;
                printOperation.PrintPanel.Options = NSPrintPanelOptions.ShowsCopies | NSPrintPanelOptions.ShowsPageRange | NSPrintPanelOptions.ShowsPreview |
                                                    NSPrintPanelOptions.ShowsPageSetupAccessory | NSPrintPanelOptions.ShowsScaling;
                printOperation.RunOperation();
                printOperation.CleanUpOperation();
            }
            catch (Exception e)
            {
                UIHelpers.ShowMessage($"Sorry there was a problem printing.\nError was: {e.Message}");
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
