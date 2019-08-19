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
        public PostScorePopup(CourseSelection p, CourseClickPopup c)
        {
            InitializeComponent();
            mp = p;
            cc = c;
        }

        private void PostScore_Clicked(object sender, EventArgs e)
        {
            var score = new Scores();
            var selectedCourse = mp.GetSelectedCourse();
            var teeinfo = App.Database.GetTeeInfo(selectedCourse.PlayedID);
            score.Score = int.Parse(EnteredScore.Text);
            score.Date = ScoreDate.Date;
            score.RoundType = SelectedRoundType.SelectedItem.ToString();
            if (score.RoundType == "18")
            {
                score.Differential = Math.Round((score.Score - selectedCourse.Rating) * 113 / selectedCourse.Slope, 2);
            }
            else if (score.RoundType == "Front")
            {
                score.Differential = Math.Round((score.Score - teeinfo.FrontRating) * 113 / selectedCourse.Slope, 2);
            }
            else if (score.RoundType == "Back")
            {
                //some courses dont have a back rating so need to remove the back rating setting if they dont
                score.Differential = Math.Round((score.Score - teeinfo.BackRating) * 113 / selectedCourse.Slope, 2);
            }
            score.PlayedID = selectedCourse.PlayedID;
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
    }
}