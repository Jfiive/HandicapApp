using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GolfHandicapApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PostScorePopup : Rg.Plugins.Popup.Pages.PopupPage
    {
        public CourseSelection mp;
        public CourseClickPopup cc;
        private DisplayCourse selectedcourse = new DisplayCourse();
        public DisplayCourse SelectedCourse { get { return selectedcourse; } }
        public PostScorePopup(CourseSelection p, CourseClickPopup c)
        {
            InitializeComponent();
            mp = p;
            cc = c;
            SelectedRoundType.SelectedIndex = 0;
            selectedcourse = mp.GetSelectedCourse();
            TeePicker.ItemsSource = App.Database.GetCourseTees(selectedcourse.CourseID);
            TeePicker.ItemDisplayBinding = new Binding("DisplayName");
            TeePicker.IsEnabled = true;
        }

        private void PostScore_Clicked(object sender, EventArgs e)
        {
            var score = new Scores();
            var teeinfo = App.Database.GetTeeInfo(selectedcourse.PlayedID);
            score.Score = int.Parse(EnteredScore.Text);
            score.Date = ScoreDate.Date;
            score.RoundType = SelectedRoundType.SelectedItem.ToString();
            if (score.RoundType == "18")
            {
                score.Differential = Math.Round((score.Score - teeinfo.Rating) * 113 / teeinfo.Slope, 2);
            }
            else if (score.RoundType == "Front")
            {
                var slope = teeinfo.Slope;
                if (teeinfo.FrontSlope > 0)
                {
                    slope = teeinfo.FrontSlope;
                }
                score.Differential = Math.Round((score.Score - teeinfo.FrontRating) * 113 / slope, 2);
            }
            else if (score.RoundType == "Back")
            {
                //some courses dont have a back rating so need to remove the back rating setting if they dont
                var slope = teeinfo.Slope;
                if (teeinfo.BackSlope > 0)
                {
                    slope = teeinfo.BackSlope;
                }
                score.Differential = Math.Round((score.Score - teeinfo.BackRating) * 113 / slope, 2);
            }
            score.PlayedID = selectedcourse.PlayedID;
            App.Database.SaveScore(score);
            App.Database.CalculateHandicap(score.RoundType);
            SelectedRoundType.SelectedIndex = 0;

            PopupNavigation.Instance.PopAllAsync();
            ((App.Current.MainPage as MasterDetailPage).Detail as NavigationPage).Navigation.PushAsync(new MyScores());
            (App.Current.MainPage as MasterDetailPage).IsPresented = false;
        }
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            cc.DeselectItem();
        }
        private void ValidityCheck()
        {
            if (!string.IsNullOrEmpty(EnteredScore.Text) && SelectedRoundType.SelectedIndex >= 0 && TeePicker.SelectedIndex >= 0)
            {
                PostScore.IsEnabled = true;
            }
            else
            {
                PostScore.IsEnabled = false;
            }
        }

        private void SelectedRoundType_SelectedIndexChanged(object sender, EventArgs e)
        {
            ValidityCheck();
        }

        private void EnteredScore_Unfocused(object sender, FocusEventArgs e)
        {
            ValidityCheck();
        }

        private void TeePicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            ValidityCheck();
        }
    }
}