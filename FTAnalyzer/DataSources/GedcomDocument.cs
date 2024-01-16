using System.Diagnostics;
using FTAnalyzer.Utilities;
using FTAnalyzer.ViewControllers;

namespace FTAnalyzer
{
    [Register("GedcomDocument")]
    public class GedcomDocument : NSDocument
    {
        AppDelegate App => (AppDelegate)NSApplication.SharedApplication.Delegate;

        [Export("canConcurrentlyReadDocumentsOfType:")]
        public static new bool CanConcurrentlyReadDocumentsOfType(string fileType) => false;

        public override bool ReadFromUrl(NSUrl url, string typeName, out NSError outError)
        {
            outError = NSError.FromDomain(NSError.OsStatusErrorDomain, -4);

            InvokeOnMainThread(async () =>
            {
                if (App.Document != null)
                    App.ResetDocument();
                FamilyTree _familyTree = FamilyTree.Instance;
                var outputText = App.DocumentViewController.Messages;
                var sourcesProgress = App.DocumentViewController.Sources;
                var individualsProgress = App.DocumentViewController.Individuals;
                var familiesProgress = App.DocumentViewController.Families;
                var relationshipProgress = App.DocumentViewController.Relationships;
                Stopwatch timer = new();
                timer.Start();
                var stream = new FileStream(url.Path, FileMode.Open, FileAccess.Read);
                var document = await Task.Run(() => _familyTree.LoadTreeHeader(url.Path, stream, outputText));
                if (document == null)
                    outputText.Report($"\n\nUnable to load file {url.Path}\n");
                else
                {
                    timer.Stop();
                    WriteTime("File Loaded", outputText, timer);
                    timer.Start();
                    await Task.Run(() => _familyTree.LoadTreeSources(document, sourcesProgress, outputText));
                    await Task.Run(() => _familyTree.LoadTreeIndividuals(document, individualsProgress, outputText));
                    await Task.Run(() => _familyTree.LoadTreeFamilies(document, familiesProgress, outputText));
                    await Task.Run(() => _familyTree.LoadTreeRelationships(document, relationshipProgress, outputText));
                    timer.Stop();
                    WriteTime("\nFile Loaded and Analysed", outputText, timer);
                    WriteMemory(outputText);
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

        static void WriteTime(string prefixText, IProgress<string> outputText, Stopwatch timer)
        {
            TimeSpan ts = timer.Elapsed;
            // Format and display the TimeSpan value.
            string elapsedTime = string.Format("{0:00}h {1:00}m {2:00}.{3:00}s", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
            outputText.Report($"{prefixText} in {elapsedTime}\n\n");
        }

        static void WriteMemory(IProgress<string> outputText)
        {
            long memoryBefore = GC.GetTotalMemory(false);
            long memoryAfter = GC.GetTotalMemory(true);
            string sizeBefore = SpecialMethods.SizeSuffix(memoryBefore, 2);
            string sizeAfter = SpecialMethods.SizeSuffix(memoryAfter, 2);
            outputText.Report($"File used {sizeBefore} during loading, reduced to {sizeAfter} after processing.");
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
        public delegate void DocumentLoadRequestDelegate();
        public event DocumentModifiedDelegate DocumentModified;
        public event DocumentLoadRequestDelegate DocumentLoadRequest;

        internal void RaiseDocumentModified(GedcomDocument document)
        {
            // Inform caller
            DocumentModified?.Invoke(document);
        }
        internal void RaiseDocumentLoadRequest()
        {
            // Inform caller
            DocumentLoadRequest?.Invoke();

        }
        #endregion
    }
}