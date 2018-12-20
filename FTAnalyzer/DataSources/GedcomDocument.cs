using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AppKit;
using CoreGraphics;
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

            InvokeOnMainThread(() =>
            {
                var document = _familyTree.LoadTreeHeader(url.Path, App.DocumentViewController.Messages);
                if (document == null)
                    App.DocumentViewController.Messages.Report($"\n\nUnable to load file {url.Path}\n");
                else
                {

                    _familyTree.LoadTreeSources(document, App.DocumentViewController.Sources, App.DocumentViewController.Messages);
                    _familyTree.LoadTreeIndividuals(document, App.DocumentViewController.Individuals, App.DocumentViewController.Messages);
                    _familyTree.LoadTreeFamilies(document, App.DocumentViewController.Families, App.DocumentViewController.Messages);
                    _familyTree.LoadTreeRelationships(document, App.DocumentViewController.Relationships, App.DocumentViewController.Messages);

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
                    Analytics.TrackAction(Analytics.MainFormAction, Analytics.LoadGEDCOMEvent);
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
