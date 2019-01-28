using System;
using System.Collections.Generic;
using AppKit;
using Foundation;
using FTAnalyzer.ViewControllers;

namespace FTAnalyzer
{
    public partial class LCUpdatesViewController : NSViewController
	{
        List<CensusIndividual> LCUpdates;
        List<CensusIndividual> LCInvalidRef;

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
        }

        public void UpdateLostCousinsReport(RelationshipTypesView relationshipTypes, ProgressController progressController)
        {
            IProgress<int> progress = new Progress<int>(percent => SetProgress(progressController, percent));
            InvokeOnMainThread(() =>
            {
                Predicate<CensusIndividual> relationFilter = relationshipTypes.BuildFilter<CensusIndividual>(x => x.RelationType, true);
                LCUpdates = new List<CensusIndividual>();
                LCInvalidRef = new List<CensusIndividual>();
                string reportText = FamilyTree.Instance.LCOutput(LCUpdates, LCInvalidRef, relationFilter, progress);
                StatsTextbox.Value = reportText;
            });
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

        partial void LostCousinsUpdateClicked(NSObject sender)
        {
            if (!string.IsNullOrEmpty(EmailAddressField.StringValue))
            {
                var userDefaults = new NSUserDefaults();
                var email = EmailAddressField.StringValue;
                userDefaults.SetString(email, "LostCousinsEmail");
                userDefaults.Synchronize();


            }
        }

        partial void ConfirmRootPersonChecked(NSObject sender)
        {
            throw new NotImplementedException();
        }

        partial void ViewInvalidClicked(NSObject sender)
        {
            throw new NotImplementedException();
        }

        partial void ViewPotentialClicked(NSObject sender)
        {
            throw new NotImplementedException();
        }
    }
}
