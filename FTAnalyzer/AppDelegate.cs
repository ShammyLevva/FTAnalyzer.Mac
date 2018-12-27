using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using AppKit;
using Foundation;
using FTAnalyzer.Exports;
using FTAnalyzer.Utilities;

namespace FTAnalyzer
{
    [Register("AppDelegate")]
    public partial class AppDelegate : NSApplicationDelegate
    {
        bool _documentOpening;
        public GedcomDocument Document { get; set; }
        public GedcomDocumentViewController DocumentViewController { get; set;  }
        NSWindow Window { get; set; }
        public NSViewController CurrentViewController { get; set; }
        public NSWindow CurrentWindow { get; set; }

        public override void DidFinishLaunching(NSNotification notification)
        {
            // Insert code here to initialize your application
            Window = NSApplication.SharedApplication.Windows[0];
            Window.MakeKeyAndOrderFront(Self);
            Window.Title = $"FTAnalyzer {Version} - Family Tree Analyzer";
            Window.Delegate = new MainWindowDelegate();
            SetMenus(false);
            CheckWebVersion();
        }

        public override bool ApplicationShouldTerminateAfterLastWindowClosed(NSApplication sender)
        {
            return !_documentOpening;
        }

        public override bool OpenFile(NSApplication sender, string filename)
        {
            _documentOpening = true;
            if (Document != null)
            {
                Document.Close();
                Document = null;
            }
            SetMenus(false);
            ResetMainWindow();
            CloseAllSubWindows();
            _documentOpening = false;
            return false;
        }

        private void ResetMainWindow()
        {
            Window.MakeKeyAndOrderFront(Self);
            var controller = Window.ContentViewController as NSTabViewController;
            controller.SelectedTabViewItemIndex = 0;
            DocumentViewController.ClearAllProgress();
        }

        public void SetMenus(bool enabled)
        {
            PrintMenu.Enabled = enabled;
            PageSetupMenu.Enabled = enabled;
            ExportIndividualsMenu.Enabled = enabled;
            ExportFamiliesMenu.Enabled = enabled;
            ExportFactsMenu.Enabled = enabled;
            ExportLocationsMenu.Enabled = enabled;
            ExportSourcesMenu.Enabled = enabled;
            ExportDataErrorsMenu.Enabled = enabled;
            ExportLooseBirthsMenu.Enabled = enabled;
            ExportLooseDeathsMenu.Enabled = enabled;
            ExportDNAGedcomMenu.Enabled = enabled;
        }

        public void ShowFacts(NSViewController factsViewController)
        {
            var storyboard = NSStoryboard.FromName("Facts", null);
            var factsWindow = storyboard.InstantiateControllerWithIdentifier("FactsWindow") as NSWindowController;
            factsWindow.ContentViewController.AddChildViewController(factsViewController);
            factsWindow.Window.Title = factsViewController.Title;
            factsWindow.Window.SetFrame(new CoreGraphics.CGRect(350, 350, 800, 500), true);
            factsWindow.ShowWindow(this);
        }

        public void CloseAllSubWindows()
        {
            foreach(NSWindow openWindow in NSApplication.SharedApplication.Windows)
            {
                if (openWindow.Title.StartsWith("Facts", StringComparison.Ordinal))
                    openWindow.Close();
                else if (openWindow.ContentViewController is PeopleViewController)
                    openWindow.Close();
                else if (openWindow.Title.StartsWith("Census Research", StringComparison.Ordinal))
                    openWindow.Close();
            }
        }

        public string Version
        {
            get
            {
                var version = NSBundle.MainBundle.ObjectForInfoDictionary("CFBundleShortVersionString").ToString();
                var build = NSBundle.MainBundle.ObjectForInfoDictionary("CFBundleVersion").ToString();
                return $"v{version}.{build}";
            }
        }

        async void CheckWebVersion()
        {
            try
            {
                await Analytics.CheckProgramUsageAsync();
            }
            catch (Exception e) 
                { Console.WriteLine(e.Message); }
        }

        partial void ExportIndividuals(NSObject sender)
        {
            try
            {
                if (Document == null)
                    return;
                ListtoDataTableConvertor convertor = new ListtoDataTableConvertor();
                DataTable dt = convertor.ToDataTable(new List<IExportIndividual>(FamilyTree.Instance.AllIndividuals));
                ExportToExcel.Export(dt);
                Analytics.TrackAction(Analytics.ExportAction, Analytics.ExportIndEvent);
            } catch (Exception e)
            {
                UIHelpers.ShowMessage($"Problem exporting Individuals: {e.Message}");
            }
        }

        partial void ExportFamilies(NSObject sender)
        {
            try
            {
                if (Document == null)
                    return;
                ListtoDataTableConvertor convertor = new ListtoDataTableConvertor();
                DataTable dt = convertor.ToDataTable(new List<IDisplayFamily>(FamilyTree.Instance.AllDisplayFamilies));
                ExportToExcel.Export(dt);
                Analytics.TrackAction(Analytics.ExportAction, Analytics.ExportFamEvent);
            }
            catch (Exception e)
            {
                UIHelpers.ShowMessage($"Problem exporting Individuals: {e.Message}");
            }
        }

        partial void ExportFacts(NSObject sender)
        {
            try
            {
                if (Document == null)
                    return;
                ListtoDataTableConvertor convertor = new ListtoDataTableConvertor();
                DataTable dt = convertor.ToDataTable(new List<ExportFact>(FamilyTree.Instance.AllExportFacts));
                ExportToExcel.Export(dt);
                Analytics.TrackAction(Analytics.ExportAction, Analytics.ExportFactsEvent);
            }
            catch (Exception e)
            {
                UIHelpers.ShowMessage($"Problem exporting Individuals: {e.Message}");
            }
        }

        partial void ExportLocations(NSObject sender)
        {
            if (Document == null)
                return;
            ListtoDataTableConvertor convertor = new ListtoDataTableConvertor();
            DataTable dt = convertor.ToDataTable(new List<IDisplayLocation>(FamilyTree.Instance.AllDisplayPlaces));
            ExportToExcel.Export(dt);
            Analytics.TrackAction(Analytics.ExportAction, Analytics.ExportLocationsEvent);
        }

        partial void ExportSources(NSObject sender)
        {
            ListtoDataTableConvertor convertor = new ListtoDataTableConvertor();
            DataTable dt = convertor.ToDataTable(new List<IDisplaySource>(FamilyTree.Instance.AllSources));
            ExportToExcel.Export(dt);
            Analytics.TrackAction(Analytics.ExportAction, Analytics.ExportSourcesEvent);
        }

        partial void ExportDataErrors(NSObject sender)
        {
            ListtoDataTableConvertor convertor = new ListtoDataTableConvertor();
            DataTable dt = convertor.ToDataTable(new List<DataError>(FamilyTree.Instance.AllDataErrors));
            ExportToExcel.Export(dt);
            Analytics.TrackAction(Analytics.ExportAction, Analytics.ExportDataErrorsEvent);
        }

        partial void ExportLooseBirths(NSObject sender)
        {
            ListtoDataTableConvertor convertor = new ListtoDataTableConvertor();
            List<IDisplayLooseBirth> list = FamilyTree.Instance.LooseBirths().ToList();
            list.Sort(new LooseBirthComparer());
            DataTable dt = convertor.ToDataTable(list);
            ExportToExcel.Export(dt);
            Analytics.TrackAction(Analytics.ExportAction, Analytics.ExportLooseBirthsEvent);
        }

        partial void ExportLooseDeaths(NSObject sender)
        {
            ListtoDataTableConvertor convertor = new ListtoDataTableConvertor();
            List<IDisplayLooseDeath> list = FamilyTree.Instance.LooseDeaths().ToList();
            list.Sort(new LooseDeathComparer());
            DataTable dt = convertor.ToDataTable(list);
            ExportToExcel.Export(dt);
            Analytics.TrackAction(Analytics.ExportAction, Analytics.ExportLooseDeathsEvent);
        }

        partial void ExportDNAGedcom(NSObject sender)
        {
            if(Document != null)
                DNA_GEDCOM.Export();
        }

        partial void PrintClicked(NSObject sender)
        {
            var keyViewController = NSApplication.SharedApplication.KeyWindow.ContentViewController;
            if (keyViewController is FTAnalyzerViewController)
            {
                if (CurrentViewController is IPrintViewController)
                    Document.PrintDocument(CurrentViewController as IPrintViewController);
                else if(CurrentViewController is GedcomDocumentViewController)
                {
                    ((GedcomDocumentViewController)CurrentViewController).Print(sender);
                }
                else
                    UIHelpers.ShowMessage("Sorry Printing Not currently available for this view");

            } 
            else if(keyViewController is PeopleViewController)
            {
                ((PeopleViewController)keyViewController).Print(sender);
            } 
            else if(keyViewController is FactsWindowViewController)
            {
                if (keyViewController.ChildViewControllers.Length > 0)
                {
                    CurrentViewController = keyViewController.ChildViewControllers[0];
                    Document.PrintDocument(CurrentViewController as IPrintViewController);
                }
                else
                    UIHelpers.ShowMessage("Sorry unknown problem with printing facts report");
            }
            else
                UIHelpers.ShowMessage("Sorry Printing Not currently available for this view");
        }

        partial void ViewOnlineManual(NSObject sender)
        {
            Analytics.TrackAction(Analytics.MainFormAction, Analytics.OnlineManualEvent);
            HttpUtility.VisitWebsite("http://www.ftanalyzer.com");
        }

        partial void ViewOnlineGuides(NSObject sender)
        {
            Analytics.TrackAction(Analytics.MainFormAction, Analytics.OnlineGuideEvent);
            HttpUtility.VisitWebsite("http://www.ftanalyzer.com/guides");
        }

        partial void ReportIssue(NSObject sender)
        {
            UIHelpers.ShowMessage("Please note this is a early development version if you find a crashing bug please report it.\nOtherwise assume I'll get round to fixing things later.\nYou may find it more useful to raise issue at the Facebook User Group.");
            Analytics.TrackAction(Analytics.MainFormAction, Analytics.ReportIssueEvent);
            HttpUtility.VisitWebsite("https://github.com/ShammyLevva/FTAnalyzer.Mac/issues");
        }

        partial void VisitFacebookSupport(NSObject sender)
        {
            HttpUtility.VisitWebsite("https://www.facebook.com/ftanalyzer");
            Analytics.TrackAction(Analytics.MainFormAction, Analytics.FacebookSupportEvent);
        }

        partial void VisitFacebookUserGroup(NSObject sender)
        {
            HttpUtility.VisitWebsite("https://www.facebook.com/groups/ftanalyzer");
            Analytics.TrackAction(Analytics.MainFormAction, Analytics.FacebookUsersEvent);
        }

        partial void VisitPrivacyPolicy(NSObject sender)
        {
            Analytics.TrackAction(Analytics.MainFormAction, Analytics.PrivacyEvent);
            HttpUtility.VisitWebsite("http://www.ftanalyzer.com/privacy");
        }

        partial void VisitWhatsNew(NSObject sender)
        {
            Analytics.TrackAction(Analytics.MainFormAction, Analytics.WhatsNewEvent);
            HttpUtility.VisitWebsite("http://mac.ftanalyzer.com/Whats%20New%20in%20this%20Release");
        }
    }

    class MainWindowDelegate : NSWindowDelegate
    {
        public override void WillClose(NSNotification notification)
        {
            foreach (NSWindow window in NSApplication.SharedApplication.Windows)
            {
                if (window != NSApplication.SharedApplication.Windows[0])
                {
                    window.Close();
                    window.Dispose();
                }
            }
        }
    }
}
