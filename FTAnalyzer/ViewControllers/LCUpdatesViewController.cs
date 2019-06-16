using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AppKit;
using Foundation;
using FTAnalyzer.DataSources;
using FTAnalyzer.Exports;
using FTAnalyzer.Utilities;
using Security;

namespace FTAnalyzer.ViewControllers
{
    public partial class LCUpdatesViewController : NSViewController
	{
        List<CensusIndividual> LCUpdates;
        List<CensusIndividual> LCInvalidReferences;
        RelationTypes RelationshipTypes;
        ProgressController ProgressController;
        LCReportsViewController LCReport;
        bool WebsiteAvailable;

        public LCUpdatesViewController (IntPtr handle) : base (handle)
		{
		}

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            StatsTextbox.Value = "";
            var userDefaults = new NSUserDefaults();
            string email = userDefaults.StringForKey("LostCousinsEmail");
            EmailAddressField.StringValue = string.IsNullOrEmpty(email) ? string.Empty : email;
            if (UseKeychainOutlet.State == NSCellStateValue.On)
                GetPasswordFromKeychain();
            EmailAddressField.Changed += (sender, e) => { ClearLogin(); };
            PasswordField.Changed += (sender, e) => { ClearLogin(); };
        }

        public override void ViewDidAppear()
        {
            base.ViewDidAppear();
            ConfirmRootPerson.Title = $"Confirm {FamilyTree.Instance.RootPerson} as root person";
            Analytics.TrackAction(Analytics.MainFormAction, Analytics.LostCousinsTabEvent);
        }

        public void UpdateLostCousinsReport() => UpdateLostCousinsReport(RelationshipTypes, ProgressController, LCReport);
        public void UpdateLostCousinsReport(RelationTypes relationshipTypes, ProgressController progressController, LCReportsViewController lcReport)
        {
            RelationshipTypes = relationshipTypes;
            ProgressController = progressController;
            LCReport = lcReport;
            InvokeOnMainThread(() =>
            {
                Predicate<CensusIndividual> relationFilter = relationshipTypes.BuildFilter<CensusIndividual>(x => x.RelationType, true);
                LCUpdates = new List<CensusIndividual>();
                LCInvalidReferences = new List<CensusIndividual>();
                string reportText = FamilyTree.Instance.LCOutput(LCUpdates, LCInvalidReferences, relationFilter);
                StatsTextbox.Value = reportText;
            });
        }

        void GetPasswordFromKeychain()
        {
            if (UseKeychainOutlet.State == NSCellStateValue.On)
            {
                var code = SecKeyChain.FindInternetPassword("lostcousins.com", EmailAddressField.StringValue, out byte[] password);
                var code2 = SecKeyChain.FindInternetPassword("www.lostcousins.com", EmailAddressField.StringValue, out byte[] password2);
                if (code == SecStatusCode.Success)
                {
                    var passwordString = Encoding.UTF8.GetString(password);
                    PasswordField.StringValue = passwordString;
                }
            }
        }

        void SetProgress(ProgressController progressController, int percent)
        {
            if (!NSThread.IsMain)
            {
                InvokeOnMainThread(() => SetProgress(progressController, percent));
                return;
            }
            progressController.ProgressBar = percent;
        }

        public void Clear() => StatsTextbox.Value = string.Empty;

        partial void LoginButtonClicked(NSObject sender)
        {
            if (!string.IsNullOrEmpty(EmailAddressField.StringValue))
            {
                var userDefaults = new NSUserDefaults();
                var email = EmailAddressField.StringValue;
                userDefaults.SetString(email, "LostCousinsEmail");
                userDefaults.Synchronize();
            }
            WebsiteAvailable = ExportToLostCousins.CheckLostCousinsLogin(EmailAddressField.StringValue, PasswordField.StringValue);
            //LoginButtonOutlet.BezelColor = websiteAvailable ? Color.LightGreen : Color.Red;
            LoginButtonOutlet.Enabled = !WebsiteAvailable;
            LostCousinsUpdateButton.Enabled = WebsiteAvailable && ConfirmRootPerson.State == NSCellStateValue.On;
            if (WebsiteAvailable)
                UIHelpers.ShowMessage("Lost Cousins login succeeded.");
            else
                UIHelpers.ShowMessage("Unable to login to Lost Cousins website. Check email/password and try again.");
        }

        async partial void LostCousinsUpdateClicked(NSObject sender)
        {
            LostCousinsUpdateButton.Enabled = false;
            if (LCUpdates?.Count > 0)
            {
                UpdateResultsTextbox.Value = string.Empty;
                int response = UIHelpers.ShowYesNo($"You have {LCUpdates.Count} possible records to add to Lost Cousins. Proceed?");
                if (response == UIHelpers.Yes)
                {
                    UpdateResultsTextbox.Value = "Started Processing Lost Cousins entries.\n\n";
                    Progress<string> outputText = new Progress<string>(AppendMessage);
                    int count = await Task.Run(() => ExportToLostCousins.ProcessList(LCUpdates, outputText));
                    string resultText = $"{DateTime.Now.ToUniversalTime().ToString("yyyy-MM-dd HH:mm")}: uploaded {count} records";
                    await Analytics.TrackActionAsync(Analytics.LostCousinsAction, Analytics.UpdateLostCousins, resultText);
                    SpecialMethods.VisitWebsite("https://www.lostcousins.com/pages/members/ancestors/");
                    UpdateLostCousinsReport();
                    LCReport.UpdateLostCousinsReport(ProgressController);
                }
            }
            else
                UIHelpers.ShowMessage("You have no records to add to Lost Cousins at this time. Use the Research Suggestions to find more people on the census, or enter/update missing or incomplete census references.");
            LostCousinsUpdateButton.Enabled = true;
        }

        public void AppendMessage(string message)
        {
            if (!NSThread.IsMain)
            {
                InvokeOnMainThread(() => AppendMessage(message));
                return;
            }
            if (UpdateResultsTextbox.Value == null)
                UpdateResultsTextbox.Value = message;
            else
                UpdateResultsTextbox.Value += message;
            NSRange range = new NSRange
            {
                Location = UpdateResultsTextbox.Value.Length
            };
            UpdateResultsTextbox.ScrollRangeToVisible(range);
        }


        void ClearLogin()
        {
            if (!LostCousinsUpdateButton.Hidden) // if we can login clear cookies to reset session
                ExportToLostCousins.EmptyCookieJar();
            //LoginButtonOutlet.BezelColor = Color.Red;
            LoginButtonOutlet.Enabled = true;
            LostCousinsUpdateButton.Enabled = false;
        }

        partial void UseKeychainChecked(NSObject sender) => GetPasswordFromKeychain();

        partial void ConfirmRootPersonChecked(NSObject sender)
        {
            LostCousinsUpdateButton.Enabled = WebsiteAvailable && ConfirmRootPerson.State == NSCellStateValue.On;
            //LostCousinsUpdateButton.BezelColor = ConfirmRootPerson.State == NSCellStateValue.On ? Color.LightGreen : Color.LightGray;
        }

        partial void ViewInvalidClicked(NSObject sender)
        {
            Census census = new Census(CensusDate.ANYCENSUS, true);
            census.SetupLCupdateList(LCInvalidReferences);
            census.ShowWindow($"Incompatible Census References in Records to upload to Lost Cousins Website");
            Analytics.TrackAction(Analytics.LostCousinsAction, Analytics.PreviewLostCousins);
        }

        partial void ViewPotentialClicked(NSObject sender)
        {
            Census census = new Census(CensusDate.ANYCENSUS, true);
            census.SetupLCupdateList(LCUpdates);
            census.ShowWindow($"Potential Records to upload to Lost Cousins Website");
            Analytics.TrackAction(Analytics.LostCousinsAction, Analytics.PreviewLostCousins);
        }
    }
}
